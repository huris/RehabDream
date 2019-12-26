/* ============================================================================== 
* ClassName：GameState 
* Author：ChenShuwei 
* CreateDate：2019/10/14 16:27:26 
* Version: 1.0
* ==============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{


    [Header("GameObjects in Game")]
    public GameObject Soccer;
    public GameObject Gate;
    public GameObject TrackRoot;
    public GameObject GoalKeeper;
    public Transform SoccerStart;
    public Transform HeightestPoint;
    public Transform SoccerTarget;

    [Header("Range of Gate")]
    public Transform BottomLeft;
    public Transform BottomRight;
    public Transform TopLeft;
    public Transform TopRight;

    [Header("Random Range")]
    public float RandomXmin = -0.2f;
    public float RandomXmax = 0.2f;
    public float RandomYmin = -0.2f;
    public float RandomYmax = 0.2f;
    public float RandomZmin = -0.2f;
    public float RandomZmax = 0.2f;
    public float MinHeight = 0.2f;
    public float MaxHeight = 1.8f;


    [Header("Music")]
    public AudioClip CheerUpSE;         //cheer up music


    [Header("Other Classes")]
    public GameUIHandle GameUIHandle;

    [Header("Time Count")]
    private float _SessionRestTime = 3.0f;
    private float _PrepareTime = 3.0f;
    private float _AddSuccessCountTime = 1.0f;
    private float _RecordTime = 0.2f;

    private Track _Track;
    private Shooting _Shooting;
    private CollisionHandle _CollisionHandle;
    private AvatarCaculator _Caculator;
    private AvatarController _GoalkeeperController;


    private string _TipsLimb = "";
    private float _RestTimeCount = 0;
    private float _RecordTimeCount = 0;
    private int _AddCount = 1;
    private float _Gravity = Physics.gravity.y;
    private float _MinDis = 0.1f;
    private float _MinGate = 0.35f;
    private float _MaxGate = 0.70f;




    // Moore FSM，使用Moore型有限状态机
    private delegate void Prepare2Shoot(); // what will happen when state transform from Prepare to Shoot
    private event Prepare2Shoot _OnPrepare2Shoot;

    private delegate void Shoot2SessionOver(); // what will happen when state transform from Shoot to SessionOver
    private event Shoot2SessionOver _OnShoot2SessionOver;

    private delegate void SessionOver2Prepare(); // what will happen when state transform from SessionOver to Prepare
    private event SessionOver2Prepare _OnSessionOver2Prepare;

    private delegate void SessionOver2GameOver(); // what will happen when state transform from SessionOver to GameOver
    private event SessionOver2GameOver _OnSessionOver2GameOver;

    private delegate void Win();  // CheckObeyRules is true
    private event Win _Win;

    private delegate void Fail();
    private event Fail _Fail;

    public enum State
    {
        Prepare,    // 等待玩家准备
        Shoot,     //正在射门
        Pause,     //暂停
        SessionOver,      // 一次扑救结束
        GameOver    //游戏结束
    }

    private State _state = State.Pause;
    private State _oldstate;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {

        InitGameObject();
        InitDelegate();
        // set start time
        PatientDataManager.instance.SetTrainingStartTime();


        // 保证切换场景时kinect 与 AvatarController不会失效
        _GoalkeeperController.playerId = KinectManager.Instance.GetPrimaryUserID();
        if (!KinectManager.Instance.avatarControllers.Contains(_GoalkeeperController))
        {
            KinectManager.Instance.avatarControllers.Add(_GoalkeeperController);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Prepare --> Shoot --> SessionOver(take a rest) --> Prepare --> Shoot --> SessionOver(take a rest) ...... --> GameOver
        switch (_state)
        {
            case State.Prepare:
                // prepare for next session
                if (PrepareTimeOver() == true)    //prepare time is over, state transform from Prepare to Shoot
                {
                    _OnPrepare2Shoot?.Invoke();
                }
                break;

            case State.Shoot:
                break;

            case State.SessionOver:
                if (IsFinish()) //game is over, state transform form SessionOver to GameOver
                {
                    _OnSessionOver2GameOver?.Invoke();
                }
                else if (SessionRestTimeOver() == true)    //rest time is over, state transform form SessionOver to Prepare
                {
                    _OnSessionOver2Prepare?.Invoke();
                }
                break;

            case State.GameOver:
                return;
                break;

            case State.Pause:
                return;
                break;
        }

        // record gravitycenter, angle
        if (RecordTimeOver())
        {
            this.WriteDatabaseInGame();
        }


        if (Input.GetKeyDown(KeyCode.Delete))
        {
            _OnSessionOver2GameOver?.Invoke();
        }

    }

    #region State Transform

    // when Prepare-->Shoot, call Prepare2Shoot()
    public void StatePrepare2Shoot()
    {
        _state = State.Shoot;
    }


    public void StateShoot2SessionOver()
    {
        _state = State.SessionOver;
    }

    public void StateSessionOver2Prepare()
    {
        _state = State.Prepare;
    }


    public void StateSessionOver2GameOver()
    {
        _state = State.GameOver;
    }

    #endregion


    #region Init Delegate Event
    // init delegate event
    private void InitDelegate()
    {

        InitPrepare2Shoot();
        InitShoot2SessionOver();
        InitSessionOver2Prepare();
        InitSessionOver2GameOver();

        InitGoalkeeperWinDelegate();
        InitGoalkeeperFailDelegate();
        InitWinDelegate();
        InitFailDelegate();
    }

    //check if player obey rules
    private void InitGoalkeeperWinDelegate()
    {
        _CollisionHandle.OnGoalkeeperWin += this.CheckObeyRules;
    }

    //Set OnGoalkeeperFail delegate event
    private void InitGoalkeeperFailDelegate()
    {
        _CollisionHandle.OnGoalkeeperFail += this.WriteDatabaseInGame;
        _CollisionHandle.OnGoalkeeperFail += this.AddFinishCount;

        // 能否这样使用？，可以使用下一行替代
        _CollisionHandle.OnGoalkeeperFail += this._OnShoot2SessionOver.Invoke;
        //_CollisionHandle.OnGoalkeeperFail += Shoot2SessionOverFunc;
    }


    //Set Win delegate event
    private void InitWinDelegate()
    {
        _Win += this.WriteDatabaseInGame;
        _Win += this.PlayWinSe;
        _Win += this.ShowAddSuccessCountText;
        _Win += this.AddSuccessCount;
        _Win += this.AddFinishCount;

        // 能否这样使用？，可以使用下一行替代
        _Win += _OnShoot2SessionOver.Invoke;
        //_CollisionHandle.OnGoalkeeperWin += Shoot2SessionOverFunc;
    }

    //Set Fail delegate event
    private void InitFailDelegate()
    {
        _Fail += this.WriteDatabaseInGame;
        _Fail += this.AddFinishCount;

        // 能否这样使用？，可以使用下一行替代
        _Fail += this._OnShoot2SessionOver.Invoke;
        //_CollisionHandle.OnGoalkeeperFail += Shoot2SessionOverFunc;
    }

    // what will happen when state transform from Prepar to Shoot
    private void InitPrepare2Shoot()
    {
        this._OnPrepare2Shoot += this.StatePrepare2Shoot;
        this._OnPrepare2Shoot += this.Shoot;
    }

    // what will happen when state transform from Shoot to SessionOver
    private void InitShoot2SessionOver()
    {
        this._OnShoot2SessionOver += this.StateShoot2SessionOver;
        this._OnShoot2SessionOver += this.ShootOver;
        this._OnShoot2SessionOver += this.ResetShoot;
    }

    // what will happen when state transform from SessionOver to Prepare
    private void InitSessionOver2Prepare()
    {
        this._OnSessionOver2Prepare += this.WriteDatabaseInGame;
        // 必须依照顺序先调用GameObjectReset，再调用GenerateShoot
        this._OnSessionOver2Prepare += this.StateSessionOver2Prepare;
        this._OnSessionOver2Prepare += this.StopSeSounds;
        this._OnSessionOver2Prepare += this.GameObjectReset;
        this._OnSessionOver2Prepare += this.GenerateShoot;

    }

    // what will happen when state transform from SessionOver to GameOver
    private void InitSessionOver2GameOver()
    {
        this._OnSessionOver2GameOver += this.StateSessionOver2GameOver;
        this._OnSessionOver2GameOver += this.StopSeSounds;
        this._OnSessionOver2GameOver += this.GameoverUI;
        this._OnSessionOver2GameOver += this.GameOverWriteDatabase;
    }

    // show "+1" in gameUI
    private void ShowAddSuccessCountText()
    {
        //Debug.Log("StartCoroutine");
        StartCoroutine(GameUIHandle.ShowAddSuccessCountText(_AddCount, _AddSuccessCountTime));
    }

    private void Shoot2SessionOverFunc()
    {
        StateShoot2SessionOver();
        ShootOver();
        ResetShoot();
    }

    // check if player obey rules
    private void CheckObeyRules()
    {
        if (PatientDataManager.instance.TrainingDifficulty == PatientDataManager.DifficultyType.Entry||
            PatientDataManager.instance.TrainingDifficulty == PatientDataManager.DifficultyType.Intermediate ||
            PatientDataManager.instance.TrainingDifficulty == PatientDataManager.DifficultyType.Advanced)
        {
            this._Win?.Invoke();
            return;
        }

        HumanBodyBones Point = _Caculator.NearestPoint(Soccer.transform.position);
        if (_TipsLimb.Equals(_Caculator.Point2Limb(Point)))
        {
            Debug.Log("@GameState: Nearest " + _TipsLimb);
            this._Win?.Invoke();
        }
        else if (_Caculator.CloseEnough(Soccer.transform.position, _TipsLimb, _MinDis))
        {
            Debug.Log("@GameState: Nearest _MinDis");
            this._Win?.Invoke();
        }
        else
        {
            this.ShowWrongLimb();
            this._Fail?.Invoke();
        }
    }


    // use Thread to write database
    public void WriteDatabaseInGame()
    {
        WriteBodyDataThread Thread = new WriteBodyDataThread(
           PatientDataManager.instance.TrainingID,
           _Caculator.CalculateGravityCenter(PatientDataManager.instance.PatientSex.Equals("男") ? true : false),
           _Caculator.LeftArmAngle(),
           _Caculator.RightArmAngle(),
           _Caculator.LeftLegAngle(),
           _Caculator.RightLegAngle(),
           _Caculator.LeftElbowAngle(),
           _Caculator.RightElbowAngle(),
           _Caculator.LeftKneeAngle(),
           _Caculator.RightKneeAngle(),
           _Caculator.LeftAnkleAngle(),
           _Caculator.RightAnkleAngle(),
           _Caculator.LeftHipAngle(),
           _Caculator.RightHipAngle(),
           _Caculator.HipAngle(),
           System.DateTime.Now);

        Thread.StartThread();
    }

    // stop playing se
    private void StopSeSounds()
    {
        SoundManager.instance.StopSounds(SoundManager.SoundType.SE);
    }

    // write Database after game over
    private void GameOverWriteDatabase()
    {

        // set TrainingID
        //DataManager.instance.SetTrainingID(DatabaseManager.instance.GenerateTrainingID());

        //write Patient Record
        PatientDatabaseManager.instance.WritePatientRecord(
            PatientDataManager.instance.TrainingID,
            PatientDataManager.instance.PatientID,
            PatientDataManager.instance.TrainingStartTime.ToString("yyyyMMdd HH:mm:ss"),
            PatientDataManager.instance.TrainingEndTime.ToString("yyyyMMdd HH:mm:ss"),
            PatientDataManager.DifficultyType2Str(PatientDataManager.instance.TrainingDifficulty),
            PatientDataManager.instance.GameCount,
            PatientDataManager.instance.SuccessCount
            );

        Debug.Log("@GameState: WritePatientRecord Over");

        //update Training Plan
        PatientDatabaseManager.instance.UpdateTrainingPlan(
            PatientDataManager.instance.PatientID,
            PatientDataManager.DifficultyType2Str(PatientDataManager.instance.PlanDifficulty),
            PatientDataManager.instance.PlanCount - 1
            );

        Debug.Log("@GameState: UpdateTrainingPlan Over");
    }

    //game is over now
    private void GameoverUI()
    {
        PatientDataManager.instance.SetTrainingEndTime();
        GameUIHandle.LoadGameoverUI();


    }

    //shoot
    private void Shoot()
    {
        _Shooting.Shoot(SoccerStart.position, SoccerTarget.position, HeightestPoint.position.y, _Gravity);
    }

    // shoot over
    private void ShootOver()
    {
        _Shooting.ShootOver();
    }

    // reset after every shooting
    private void GameObjectReset()
    {
        _Shooting.Reset(SoccerStart.position);
        _CollisionHandle.Reset();
    }

    //play cheer up SE
    private void PlayWinSe()
    {
        SoundManager.instance.Play(CheerUpSE, SoundManager.SoundType.SE, PatientDataManager.instance.seVolume);
    }

    //SuccessCount+1
    private void AddSuccessCount()
    {
        PatientDataManager.instance.SetSuccessCount(PatientDataManager.instance.SuccessCount + 1);
        GameUIHandle.SetSuccessCountText(PatientDataManager.instance.SuccessCount);
    }

    //FinishCount+1 
    private void AddFinishCount()
    {
        PatientDataManager.instance.SetFinishCount(PatientDataManager.instance.FinishCount + 1);
        GameUIHandle.SetTrainingProgress(PatientDataManager.instance.FinishCount, PatientDataManager.instance.GameCount);
    }


    #endregion


    #region Generate and Reset Shoot

    // show the path of soccerball
    private void ShowSoccerTrackTips(Vector3 start, Vector3 end, float height, float gravity)
    {
        TrackRoot.SetActive(true);
        _Track.GenerateTrack(start, end, height, gravity);
    }

    // hide the path of soccerball
    private void HideSoccerballTrackTips()
    {
        TrackRoot.SetActive(false);
    }

    // generate SoccerTarget
    private Vector3 GenerateTarget()
    {
        Vector3 Mid = (BottomLeft.position + TopLeft.position + BottomRight.position + TopRight.position) / 4;
        Vector3 Target = Mid;
        switch (PatientDataManager.instance.TrainingDifficulty)
        {
            case PatientDataManager.DifficultyType.Entry:       // Target is near(left shoulder, right shoulder)
                Target=GenerateEntryTarget();
                break;
            case PatientDataManager.DifficultyType.Primary:    // Target is near(left hand, right hand)
                Target = GeneratePrimaryTarget();
                break;
            case PatientDataManager.DifficultyType.General:    // Target is near(left foot, right foot)
                Target = GenerateGeneralTarget();
                break;
            case PatientDataManager.DifficultyType.Intermediate:   // Target is near(left foot, right foot, left hand, right hand)
                Target = GenerateIntermediateTarget();
                break;
            case PatientDataManager.DifficultyType.Advanced:       // any area of gate
                Target = GenerateAdvancedTarget();
                break;
        }
        return Target;
    }


    // Target is near(left shoulder, right shoulder)
    private Vector3 GenerateEntryTarget(){
        Vector3 Mid = (BottomLeft.position + TopLeft.position + BottomRight.position + TopRight.position) / 4;
        Vector3 Target = Mid;
        if (Random.Range(0, 2) < 1)
        {
            // Target is near left shoulder
            Vector3 LeftMid = (BottomLeft.position + TopLeft.position) / 2;
            Target.z = _Caculator.GetLeftUpperArmPosition().z + Random.Range(RandomZmin/2, RandomZmin);
            Target.y = _Caculator.GetLeftUpperArmPosition().y + Random.Range(0, RandomYmax);
            this._TipsLimb = "左肩";
        }
        else
        {
            // Target is near Right shoulder
            Vector3 RightMid = (BottomRight.position + TopRight.position) / 2;
            Target.z = _Caculator.GetRightUpperArmPosition().z + Random.Range(RandomZmax / 2, RandomZmax);
            Target.y = _Caculator.GetRightUpperArmPosition().y + Random.Range(0, RandomYmax);
            this._TipsLimb = "右肩";
        }
        return Target;
    }



    // Target is near(left hand, right hand)
    private Vector3 GeneratePrimaryTarget()
    {
        Vector3 Mid = (BottomLeft.position + TopLeft.position + BottomRight.position + TopRight.position) / 4;
        Vector3 Target = Mid;
        if (Random.Range(0, 2) < 1)
        {
            // Target is near left hand
            Vector3 LeftMid = (BottomLeft.position + TopLeft.position) / 2;
            Target.z = Random.Range(LeftMid.z, Mid.z);
            Target.y = Target.y + Random.Range(RandomYmin, RandomYmax);
            this._TipsLimb = "左手";
        }
        else
        {
            // Target is near right hand
            Vector3 RightMid = (BottomRight.position + TopRight.position) / 2;
            Target.z = Random.Range(Mid.z, RightMid.z);
            Target.y = Target.y + Random.Range(RandomYmin, RandomYmax);
            this._TipsLimb = "右手";
        }
        return Target;
    }

    // Target is near(left foot, right foot)
    private Vector3 GenerateGeneralTarget()
    {
        Vector3 Mid = (BottomLeft.position + TopLeft.position + BottomRight.position + TopRight.position) / 4;
        Vector3 Target = Mid;
        if (Random.Range(0, 2) < 1)
        {
            // Target is near left foot
            Target.z = Random.Range(BottomLeft.position.z, Mid.z);
            Target.y = BottomLeft.position.y + Random.Range(RandomYmin, RandomYmax);
            this._TipsLimb = "左脚";
        }
        else
        {
            // Target is near right foot
            Vector3 RightMid = (BottomRight.position + TopRight.position) / 2;
            Target.z = Random.Range(Mid.z, RightMid.z);
            Target.y = BottomRight.position.y + Random.Range(RandomYmin, RandomYmax);
            this._TipsLimb = "右脚";
        }

        return Target;
    }

    // Target is near(left foot, right foot, left hand, right hand)
    private Vector3 GenerateIntermediateTarget()
    {
        Vector3 Mid = (BottomLeft.position + TopLeft.position + BottomRight.position + TopRight.position) / 4;
        Vector3 Target = Mid;

        Target.y = Random.Range(BottomLeft.position.y, TopLeft.position.y);
        Target.z = Random.Range(BottomLeft.position.z, BottomRight.position.z);

        this._TipsLimb = "任意肢体";
        return Target;
    }

    // any area of gate
    private Vector3 GenerateAdvancedTarget()
    {
        Vector3 Mid = (BottomLeft.position + TopLeft.position + BottomRight.position + TopRight.position) / 4;
        Vector3 Target = Mid;

        Target.y = Random.Range(BottomLeft.position.y, TopLeft.position.y);
        Target.z = Random.Range(BottomLeft.position.z, BottomRight.position.z);

        this._TipsLimb = "任意肢体";
        return Target;
    }

    // change size of gate
    private void GenerateGate()
    {
        switch (PatientDataManager.instance.TrainingDifficulty)
        {
            case PatientDataManager.DifficultyType.Entry:
                Gate.transform.localScale = new Vector3(1, _MinGate, 1);
                break;
            case PatientDataManager.DifficultyType.Primary:
                Gate.transform.localScale = new Vector3(1, _MinGate, 1);
                break;
            case PatientDataManager.DifficultyType.General:
                Gate.transform.localScale = new Vector3(1, _MinGate, 1);
                break;
            case PatientDataManager.DifficultyType.Intermediate:
                Gate.transform.localScale = new Vector3(1, _MinGate, 1);
                break;
            case PatientDataManager.DifficultyType.Advanced:
                Gate.transform.localScale = new Vector3(1, _MaxGate, 1);
                break;
        }
    }

    // generate Tips
    private void ShowWordTips(string TipsLimb)
    {
        GameUIHandle.SetTipsText(TipsLimb);
    }

    // 提示使用了错误肢体
    private void ShowWrongLimb()
    {
        GameUIHandle.ShowWrongLimb();
    }

    // Generate everything for next Shoot
    private void GenerateShoot()
    {
        // set size of gate
        GenerateGate();
        // set target of shoot
        SoccerTarget.position = GenerateTarget();
        // set Heightest point 
        // heigh + 
        HeightestPoint.position = new Vector3(0, Random.Range(MinHeight, MaxHeight), 0);
        // show track
        if (PatientDataManager.instance.SoccerTrackTips)
        {
            ShowSoccerTrackTips(SoccerStart.position, SoccerTarget.position, HeightestPoint.position.y, _Gravity);
        }
        else
        {
            HideSoccerballTrackTips();
        }

        // set tips in GameUI
        if (PatientDataManager.instance.WordTips)
        {
            ShowWordTips(_TipsLimb);
        }
        else
        {
            ShowWordTips("");
        }

        Debug.Log("@GameState: GenerateShoot Over");
    }

    // reset everything before shoot
    private void ResetShoot()
    {
        // Hide Track
        HideSoccerballTrackTips();
        // reset tips
        HideWordTips();
    }

    // reset Text in GameUI
    public void HideWordTips()
    {
        _TipsLimb = "";
        ShowWordTips(_TipsLimb);

    }

    #endregion


    // init gameobject in start()
    public void InitGameObject()
    {
        _Shooting = Soccer.GetComponent<Shooting>();
        _CollisionHandle = Soccer.GetComponent<CollisionHandle>();
        _Track = TrackRoot.GetComponent<Track>();
        _GoalkeeperController = GoalKeeper.GetComponent<AvatarController>();
        _Caculator = GoalKeeper.GetComponent<AvatarCaculator>();

        GenerateGate();
    }

    //game pause
    public void Pause()
    {
        _oldstate = _state;
        _state = State.Pause;
        _Shooting.PauseShooting();
    }


    //game continue
    public void Continue()
    {
        _state = _oldstate;
        _Shooting.ContinueShooting();
    }


    // is game finish
    private bool IsFinish()
    {
        return (PatientDataManager.instance.FinishCount == PatientDataManager.instance.GameCount);
    }


    // rest after a session
    // if rest is over return true
    private bool SessionRestTimeOver()
    {
        _RestTimeCount += Time.deltaTime;
        if (_RestTimeCount >= _SessionRestTime)
        {
            _RestTimeCount = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }


    // prepare
    private bool PrepareTimeOver()
    {
        _RestTimeCount += Time.deltaTime;
        if (_RestTimeCount < _PrepareTime)
        {
            return false;
        }
        else
        {
            _RestTimeCount = 0f;
            return true;

        }
    }


    // Record
    private bool RecordTimeOver()
    {
        _RecordTimeCount += Time.deltaTime;
        if (_RecordTimeCount < _RecordTime)
        {
            return false;
        }
        else
        {
            _RecordTimeCount = 0f;
            return true;

        }
    }
}
