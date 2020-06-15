using DG.Tweening;
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
    public GameObject Shooter;
    public Transform SoccerStart;
    public Transform ControlPoint;
    public Transform SoccerTarget;
    public Transform ShooterStart;

    [Header("Range of Gate")]
    public Transform BottomLeft;
    public Transform BottomRight;
    public Transform TopLeft;
    public Transform TopRight;

    [Header("Music")]
    public AudioClip CheerUpSE;         //cheer up music

    public AudioClip UponSeTip;         // Se tips
    public AudioClip UponRightSeTip;
    public AudioClip RightSeTip;
    public AudioClip DownRightSeTip;
    public AudioClip DownSeTip;
    public AudioClip DownLeftSeTip;
    public AudioClip LeftSeTip;
    public AudioClip UponLeftSeTip;

    [Header("UI")]
    public GameUIHandle GameUIHandle;

    [Header("Time Paraments")]
    public static float ShooterTime = 1.0f;
    public static float SessionRestTime = 3.0f;      // rest after each shoot
    public static float AddSuccessCountTime = 1.0f;  // show "+1" in ui
    public static float RecordTime = 0.2f;           // record gravity,angles... each 0.2s 
    public float PrepareTime => PatientDataManager.instance.LaunchSpeed;          // prepare for shoot

    [Header("Distance Paraments")]
    public static float AddDistancePercent = 0.2f;
    public static float MaxDistancePercent = 1.1f;
    public static float MinDistancePercent = 0.8f;

    [Header("Other Paraments")]
    public static int AddCount = 1;
    public static float MinDis = 0.1f;
    public static float MinGate = 0.6f;
    public static float MaxGate = 0.8f;

    [Header("Random Range")]
    public float RandomXmin = -0.2f;
    public float RandomXmax = 0.2f;
    public float RandomYmin = -0.2f;
    public float RandomYmax = 0.2f;
    public float RandomZmin = -0.2f;
    public float RandomZmax = 0.2f;

    private Track _Track;                       // track of soccerball
    private Shooting _Shooting;
    private CollisionHandle _CollisionHandle;   // handle collision
    private AvatarCaculator _Caculator;         // caculate gravity,angles...
    private AvatarController _GoalkeeperController;

    // record of last shoot
    private int _FailCount = 0;      //number of failures
    private PatientDataManager.DirectionType _Direction = PatientDataManager.DirectionType.UponDirection;   //direction of shoot
    private float _TargetDistance = 0f;  //Target of shoot
    private bool _HasInit = false;
    private bool _OutOfRange = false;

    // Time counter
    private float _RestTimeCount = 0;
    private float _RecordTimeCount = 0;

    // tips
    private string _Tips = "";

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

        // record gravitycenter, angle and update TimeCount
        if (RecordTimeOver())
        {
            this.WriteDatabaseInGame();
            GameUIHandle.SetTrainingProgress(PatientDataManager.instance.TimeCount, PatientDataManager.Minute2Second(PatientDataManager.instance.TrainingTime));
        }
        
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            _OnSessionOver2GameOver?.Invoke();
            Debug.Log(_state);
        }

        PatientDataManager.instance.SetTimeCount(PatientDataManager.instance.TimeCount + Time.deltaTime);
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
        _CollisionHandle.OnGoalkeeperWin += this.AddGameCountt;
        _CollisionHandle.OnGoalkeeperWin += this._OnShoot2SessionOver.Invoke;
    }

    //Set OnGoalkeeperFail delegate event
    private void InitGoalkeeperFailDelegate()
    {
        _CollisionHandle.OnGoalkeeperFail += this.WriteDatabaseInGame;
        _CollisionHandle.OnGoalkeeperFail += this.AddGameCountt;
        _CollisionHandle.OnGoalkeeperFail += this.AddFailCount;
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
        _Win += this.ResetFailCount;
        _Win += this.UpdateOneMaxDirection;
    }

    //Set Fail delegate event
    private void InitFailDelegate()
    {
        _Fail += this.WriteDatabaseInGame;
        _Fail += this.AddFailCount;

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
        StartCoroutine(GameUIHandle.ShowAddSuccessCountText(AddCount, AddSuccessCountTime));
    }

    private void Shoot2SessionOverFunc()
    {
        StateShoot2SessionOver();
        ShootOver();
        ResetShoot();
    }

    // FinishCount ++
    private void AddGameCountt()
    {
        PatientDataManager.instance.SetGameCount(PatientDataManager.instance.GameCount + 1);
    }

    // check if player obey rules
    private void CheckObeyRules()
    {
        //  CheckObeyRules()需要修改
        //if (PatientDataManager.instance.TrainingDifficulty == PatientDataManager.DifficultyType.Entry||
        //    PatientDataManager.instance.TrainingDifficulty == PatientDataManager.DifficultyType.Intermediate ||
        //    PatientDataManager.instance.TrainingDifficulty == PatientDataManager.DifficultyType.Advanced)
        //{
        //    this._Win?.Invoke();
        //    return;
        //}

        //HumanBodyBones Point = _Caculator.NearestPoint(Soccer.transform.position);
        //if (_TipsLimb.Equals(_Caculator.Point2Limb(Point)))
        //{
        //    Debug.Log("@GameState: Nearest " + _TipsLimb);
        //    this._Win?.Invoke();
        //}
        //else if (_Caculator.CloseEnough(Soccer.transform.position, _TipsLimb, _MinDis))
        //{
        //    Debug.Log("@GameState: Nearest _MinDis");
        //    this._Win?.Invoke();
        //}
        //else
        //{
        //    this.ShowWrongLimb();
        //    this._Fail?.Invoke();
        //}

        this._Win?.Invoke();
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
           _Caculator.LeftSideAngle(),
           _Caculator.RightSideAngle(),
           _Caculator.UponSideAngle(),
           _Caculator.DownSideAngle(),
           System.DateTime.Now);

        Thread.StartThread();
    }

    // stop playing Se
    private void StopSeSounds()
    {
        SoundManager.instance.StopSounds(SoundManager.SoundType.SE);
    }

    // write Database after game over
    private void GameOverWriteDatabase()
    {
        
        //write Patient Record
        PatientDatabaseManager.instance.WritePatientRecord(
            PatientDataManager.instance.TrainingID,
            PatientDataManager.instance.PatientID,
            PatientDataManager.instance.TrainingStartTime.ToString("yyyyMMdd HH:mm:ss"),
            PatientDataManager.instance.TrainingEndTime.ToString("yyyyMMdd HH:mm:ss"),
            PatientDataManager.DifficultyType2Str(PatientDataManager.instance.TrainingDifficulty),
            PatientDataManager.instance.GameCount,
            PatientDataManager.instance.SuccessCount,
            PatientDataManager.DirectionType2Str(PatientDataManager.instance.TrainingDirection),
            PatientDataManager.instance.TrainingTime,
            PatientDataManager.instance.IsEvaluated,
            EvaluationSocre(PatientDataManager.instance.MaxDirection)
            );

        Debug.Log("@GameState: WritePatientRecord Over");

        // write MaxDirection
        PatientDatabaseManager.instance.WriteMaxDirection(
            PatientDataManager.instance.TrainingID,
            PatientDataManager.instance.NewMaxDirection
        );
        Debug.Log("@GameState: WriteMaxDirection Over");

        //update Training Plan
        PatientDatabaseManager.instance.UpdateTrainingPlan(
            PatientDataManager.instance.PatientID,
            PatientDataManager.DifficultyType2Str(PatientDataManager.instance.PlanDifficulty),
            PatientDataManager.instance.PlanCount - 1
            );

        //DoctorDataManager.instance.doctor.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID, DoctorDataManager.instance.doctor.DoctorName);
        //DoctorDataManager.instance.doctor.patient.SetPatientData();

        Debug.Log("@GameState: UpdateTrainingPlan Over");
    }


    private float EvaluationSocre(float[] MaxDirections)
    {
        float Sin45 = Mathf.Sqrt(2) / 2;
        float Socre = 0;

        for (int i = 0; i < MaxDirections.Length; i++)
        {
            Socre += 0.5f * MaxDirections[i] * MaxDirections[(i + 1) % 8] * Sin45;
        }
        return Socre;
    }

    //game is over now
    private void GameoverUI()
    {
        PatientDataManager.instance.SetTrainingEndTime();
        GameUIHandle.LoadGameoverUI();


    }

    // Shoot
    private void Shoot() {
        StartCoroutine(StartShoot());
    }

    //Shooter run and shoot
    private IEnumerator StartShoot() {
        Animator animator = Shooter.GetComponent<Animator>();
        AnimationClip[] Clips = animator.runtimeAnimatorController.animationClips;

        // distance between shooter and soccer
        float Distance = Mathf.Abs(SoccerStart.transform.position.x - ShooterStart.transform.position.x);

        animator.CrossFade("BlendTree", 0.2f, 0);
        animator.SetFloat("Blend", 0.5f);
        // update shooter position
        while (true)
        {
            if(this._state == State.Pause)
            {
                animator.speed = 0;     //stop animation
            }
            else{
                float Offset = Time.deltaTime / ShooterTime * Distance;
                float Percent = 1.0f - Mathf.Abs(Shooter.transform.position.x - SoccerStart.transform.position.x) / Distance;
                Debug.Log(animator.GetFloat("Blend"));

                if (Percent > 0.8f)     // arrive soccer
                {
                    Debug.Log(Percent);
                    // time to shoot
                    animator.speed = 1f;
                    animator.CrossFade("shoot", 0.1f, 0);
                    yield return new WaitForSeconds(0.2f);  // play shooting animation for 0.2f
                    break;
                }
                // Update Animation parament
                animator.SetFloat("Blend", Percent>0.5f?Percent:0.5f);
            }
            yield return null;
        }
        _Shooting.Shoot(SoccerStart.position, ControlPoint.position, SoccerTarget.position, PatientDataManager.instance.BallSpeed);
    }

    // shoot over
    private void ShootOver()
    {
        _Shooting.ShootOver();
    }


    private void ResetShooter()
    {
        Shooter.transform.position = ShooterStart.position;
        Shooter.GetComponent<Animator>().CrossFade("rest", 0f, 0);
        Shooter.GetComponent<Animator>().SetFloat("Blend", 0f);

    }

    // reset after every shooting
    private void GameObjectReset()
    {
        ResetShooter();
        _Shooting.Reset(SoccerStart.position);
        _CollisionHandle.Reset();
    }

    //play cheer up Se
    private void PlayWinSe()
    {
        SoundManager.instance.Play(CheerUpSE, SoundManager.SoundType.SE, PatientDataManager.instance.seVolume);
    }

    //play tips se
    private void PlaySeTips(AudioClip SETips)
    {
        SoundManager.instance.Play(SETips, SoundManager.SoundType.SE, PatientDataManager.instance.seVolume);
    }

    // wordtip to SEtip
    private AudioClip Word2Se(string Tip)
    {
        string[] Tips = new string[] { "正上方", "右上方", "正右方", "右下方", "正下方", "左下方", "正左方", "左上方" };
        AudioClip[] SeTips = new AudioClip[]{UponSeTip, UponRightSeTip, RightSeTip, DownRightSeTip, DownSeTip, DownLeftSeTip,LeftSeTip,UponLeftSeTip};
        if (System.Array.IndexOf(Tips, Tip) != -1)
        {
            return SeTips[System.Array.IndexOf(Tips, Tip)];
        }
        else
        {
            return SeTips[0];
        }
    }

    //SuccessCount+1
    private void AddSuccessCount()
    {
        PatientDataManager.instance.SetSuccessCount(PatientDataManager.instance.SuccessCount + 1);
        GameUIHandle.SetSuccessCountText(PatientDataManager.instance.SuccessCount);
    }

    // FailCount++
    private void AddFailCount()
    {
        _FailCount++;
    }

    // FailCount=0
    private void ResetFailCount()
    {
        _FailCount = 0;
    }

    // update maxDirection
    private void UpdateOneMaxDirection()
    {
        PatientDataManager.instance.UpdateNewMaxDirection(_TargetDistance, _Direction);
    }


    #endregion


    #region Generate and Reset Shoot


    private bool FailTooMuch()
    {
        return (_FailCount == 3);
    }

    private bool NoFail()
    {
        return (_FailCount == 0);
    }

    // show the path of soccerball
    private void ShowSoccerTrackTips(Vector3 Start, Vector3 ControlPoint, Vector3 End)
    {
        TrackRoot.SetActive(true);
        _Track.GenerateTrack(Start, ControlPoint, End);
    }

    // hide the path of soccerball
    private void HideSoccerballTrackTips()
    {
        TrackRoot.SetActive(false);
    }

    // generate SoccerTarget
    private Vector3 GenerateTarget()
    {
        Vector3 Target = _Caculator.GetSpinePosition();
        if (PatientDataManager.instance.IsEvaluated == 1)
        { //评估
            Target = GenerateEvaluateTarget();
        }
        else
        {
            //训练 
            switch (PatientDataManager.instance.TrainingDifficulty)
            {
                case PatientDataManager.DifficultyType.Entry:       // Target is near(left shoulder, right shoulder)
                    Target = GenerateEntryTarget();
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
        }

        return Target;
    }

    // evaluate
    private Vector3 GenerateEvaluateTarget()
    {
        Vector3 Target = _Caculator.GetSpinePosition(); //default target
        if (IsFinish())
        {
            _OnSessionOver2GameOver?.Invoke();
        }
        else if (NoFail())
        {
            if (_OutOfRange)    //已经最大，但是患者仍然成功
            {
                _TargetDistance = _Caculator.InitDistance();
                _Direction = PatientDataManager.ChangeDirection(_Direction);    //change direction   
            }
            else
            {
                _TargetDistance += _TargetDistance * AddDistancePercent;   //+5%
            }
        }
        else if(FailTooMuch())
        {
            ResetFailCount();
            _TargetDistance = _Caculator.InitDistance();
            _Direction = PatientDataManager.ChangeDirection(_Direction);    //change direction   
        }
        else
        {
                //nothing
        }

        //Target = Target + Distance * e_Direction
        Target = Distance2Vector3(_TargetDistance, _Direction, Target);
        _Tips = PatientDataManager.DirectionType2Str(_Direction);
        return Target;
    }


    // Target is near(left shoulder, right shoulder)
    private Vector3 GenerateEntryTarget(){
        Vector3 Target = _Caculator.GetSpinePosition(); //default target
        if (NoFail())
        {
            if (_OutOfRange && PatientDataManager.instance.TrainingDirection == PatientDataManager.DirectionType.AnyDirection)       
            {
                //已经最大，但是患者仍然成功
                //如果训练方向为任意方向，则切换方向
                _Direction = PatientDataManager.ChangeDirection(_Direction);    //change direction  
                _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] * MinDistancePercent;
            }
            else
            {
                _TargetDistance += _TargetDistance * AddDistancePercent;   //+5%
            }
            
        }
        else if (FailTooMuch())
        {
            if(PatientDataManager.instance.TrainingDirection == PatientDataManager.DirectionType.AnyDirection)
            {
                // 如果训练方向为任意方向，则切换方向
                ResetFailCount();
                _Direction = PatientDataManager.ChangeDirection(_Direction);    //change direction  
                _TargetDistance= PatientDataManager.instance.MaxDirection[(int)_Direction] * MinDistancePercent;
            }
            else
            {
                // 否则，保持方向，重新开始
                ResetFailCount();
                _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] * MinDistancePercent;
            } 
        }
        else
        {
            //nothing
        }

        //Target = Target + Distance * e_Direction
        Target = Distance2Vector3(_TargetDistance, _Direction, Target);
        _Tips = PatientDataManager.DirectionType2Str(_Direction);
        return Target;
    }



    // Target is near(left hand, right hand)
    private Vector3 GeneratePrimaryTarget()
    {
        Vector3 Target = _Caculator.GetSpinePosition(); //default target
        if (NoFail())
        {
            if (_OutOfRange && PatientDataManager.instance.TrainingDirection == PatientDataManager.DirectionType.AnyDirection)
            {
                //已经最大，但是患者仍然成功
                //如果训练方向为任意方向，则切换方向
                _Direction = PatientDataManager.ChangeDirection(_Direction);    //change direction  
                _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] * MinDistancePercent;
            }
            else
            {
                _TargetDistance += _TargetDistance * AddDistancePercent;   //+5%
            }
        }
        else if (FailTooMuch())
        {
            if (PatientDataManager.instance.TrainingDirection == PatientDataManager.DirectionType.AnyDirection)
            {
                // 如果训练方向为任意方向，则切换方向
                ResetFailCount();
                _Direction = PatientDataManager.ChangeDirection(_Direction);    //change direction  
                _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] * MinDistancePercent;
            }
            else
            {
                // 否则，保持方向，重新开始
                ResetFailCount();
                _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] * MinDistancePercent;
            }
        }
        else
        {
            //nothing
        }

        //Target = Target + Distance * e_Direction
        Target = Distance2Vector3(_TargetDistance, _Direction, Target);
        _Tips = PatientDataManager.DirectionType2Str(_Direction);
        return Target;
    }

    // Target is near(left foot, right foot)
    private Vector3 GenerateGeneralTarget()
    {
        Vector3 Target = _Caculator.GetSpinePosition(); //default target
        if(PatientDataManager.instance.TrainingDirection == PatientDataManager.DirectionType.AnyDirection)
        {
            _Direction = (PatientDataManager.DirectionType)((int)Random.Range(0f, 8f));
        }
        _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] * 
            Random.Range(MinDistancePercent, MaxDistancePercent);   //80% ~ 110%

        //Target = Target + Distance * e_Direction
        Target = Distance2Vector3(_TargetDistance, _Direction, Target);
        _Tips = PatientDataManager.DirectionType2Str(_Direction);
        return Target;
    }

    // Target is near(left foot, right foot, left hand, right hand)
    private Vector3 GenerateIntermediateTarget()
    {
        Vector3 Target = _Caculator.GetSpinePosition(); //default target
        if (PatientDataManager.instance.TrainingDirection == PatientDataManager.DirectionType.AnyDirection)
        {
            _Direction = (PatientDataManager.DirectionType)((int)Random.Range(0f, 8f));
        }
        _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] *
            Random.Range(MinDistancePercent, MaxDistancePercent);   //80% ~ 110%

        //Target = Target + Distance * e_Direction
        Target = Distance2Vector3(_TargetDistance, _Direction, Target);
        _Tips = PatientDataManager.DirectionType2Str(_Direction);
        return Target;
    }

    // any area of gate
    private Vector3 GenerateAdvancedTarget()
    {
        Vector3 Target = _Caculator.GetSpinePosition(); //default target
        if (_Direction == PatientDataManager.DirectionType.AnyDirection)
        {
            _Direction = (PatientDataManager.DirectionType)((int)Random.Range(0f, 8f));
        }
        _TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] *
            Random.Range(MinDistancePercent, MaxDistancePercent);   //80% ~ 110%

        //Target = Target + Distance * e_Direction
        Target = Distance2Vector3(_TargetDistance, _Direction, Target);
        _Tips = "请左右走动，朝" + PatientDataManager.DirectionType2Str(_Direction) + "方接球";
        return Target;
    }

    // return InitTarget + Distance * e_Direction
    private Vector3 Distance2Vector3(float Distance, PatientDataManager.DirectionType Direction, Vector3 InitTarget)
    {
        Vector3 e = new Vector3(0f, 0f, 1f);
        Vector3 Target ;
        float HalfSqrt2= 0.5f * Mathf.Sqrt(2);

        // get e_Direction
        switch (Direction)
        {
            case PatientDataManager.DirectionType.UponDirection:    //上
                e= new Vector3(0f, 1f, 0f);
                break;
            case PatientDataManager.DirectionType.UponLeftDirection:    //左上
                e = new Vector3(0f, HalfSqrt2, -HalfSqrt2);
                break;
            case PatientDataManager.DirectionType.UponRightDirection:   //右上
                e = new Vector3(0f, HalfSqrt2, HalfSqrt2);
                break;
            case PatientDataManager.DirectionType.DownDirection:    //下
                e = new Vector3(0f, -1f, 0f);
                break;
            case PatientDataManager.DirectionType.DownLeftDirection:    //左下
                e = new Vector3(0f, -HalfSqrt2, -HalfSqrt2);
                break;
            case PatientDataManager.DirectionType.DownRightDirection:   //右下
                e = new Vector3(0f, -HalfSqrt2, HalfSqrt2);
                break;
            case PatientDataManager.DirectionType.LeftDirection:    //左
                e = new Vector3(0f, 0f, -1f);
                break;
            case PatientDataManager.DirectionType.RightDirection:   //右
                e = new Vector3(0f, 0f, 1f);
                break;
            default:
                Debug.Log("@GameState: Distance2Vector3 Error");
                break;
        }

        // InitTarget + Distance * e_Direction
        Target = InitTarget + e * Distance;

        // Target must in range of gate
        Target = RestrictTarget(Target);

        return Target;
    }

    // Restrict Target in gate
    private Vector3 RestrictTarget(Vector3 Target)
    {
        _OutOfRange = false;
        Vector3 result = Target;

        //Restrict x
        result.x = TopLeft.position.x + Random.Range(0,RandomXmax) * 2;

        // Restrict y
        if (result.y > TopLeft.position.y)
        {
            result.y = TopLeft.position.y - Random.Range(0, RandomYmax);
            _OutOfRange = true;
        }
        else if(result.y < BottomLeft.position.y)
        {
            result.y = BottomLeft.position.y + Random.Range(0, RandomYmin);
            _OutOfRange = true;
        }

        //Restrict z
        if (result.z > TopRight.position.z)
        {
            result.z = TopRight.position.z - Random.Range(0, RandomZmax);
            _OutOfRange = true;
        }
        else if(result.z < TopLeft.position.z)
        {
            result.z = TopLeft.position.z + Random.Range(0, RandomZmax);
            _OutOfRange = true;
        }
        return result;
    }


    // change size of gate
    private void GenerateGate()
    {
        switch (PatientDataManager.instance.TrainingDifficulty)
        {
            case PatientDataManager.DifficultyType.Entry:
                Gate.transform.localScale = new Vector3(1, MinGate, 1.2f);
                break;
            case PatientDataManager.DifficultyType.Primary:
                Gate.transform.localScale = new Vector3(1, MinGate, 1.2f);
                break;
            case PatientDataManager.DifficultyType.General:
                Gate.transform.localScale = new Vector3(1, MinGate, 1.2f);
                break;
            case PatientDataManager.DifficultyType.Intermediate:
                Gate.transform.localScale = new Vector3(1, MinGate, 1.2f);
                break;
            case PatientDataManager.DifficultyType.Advanced:
                Gate.transform.localScale = new Vector3(1, MaxGate, 1.2f);
                break;
        }
    }

    // generate Tips
    private void ShowWordTips(string Tip)
    {
        GameUIHandle.SetTipsText(Tip);
    }

    // 提示使用了错误肢体
    private void ShowWrongLimb()
    {
        GameUIHandle.ShowWrongLimb();
    }

    // Generate everything for next Shoot
    private void GenerateShoot()
    {
        if (_HasInit == false)
        {
            InitValue();
            _HasInit = true;
        }

        // set size of gate
        GenerateGate();
        // set target of shoot
        SoccerTarget.position = GenerateTarget();
        // set Heightest point 
        // heigh + 
        // show track
        if (PatientDataManager.instance.SoccerTrackTips)
        {
            // generate ControlPoint of Bezier
            GenerateControlPoint();
            ShowSoccerTrackTips(SoccerStart.position, ControlPoint.position, SoccerTarget.position);
        }
        else
        {
            HideSoccerballTrackTips();
        }

        // set tips in GameUI
        if (PatientDataManager.instance.WordTips)
        {
            ShowWordTips(_Tips);
        }
        else
        {
            ShowWordTips("");   // no word tips
        }

        //play Se
        if (PatientDataManager.instance.SETips)
        {
            PlaySeTips(Word2Se(_Tips));
        }

        Debug.Log("@GameState: GenerateShoot Over");
    }

    // generate ControlPoint of Bezier randomly
    private void GenerateControlPoint()
    {
        ControlPoint.position = (SoccerStart.position + SoccerTarget.position) / 2;
        float RandomX = Random.Range(RandomXmin, RandomXmax);
        float RandomY = Random.Range(RandomYmin, RandomYmax) + Random.Range(1, 2) * (TopLeft.position.y-BottomLeft.position.y);
        float RandomZ;

        // 目标在左侧，则曲线向左侧弯曲，右侧同理
        if (SoccerTarget.position.z > (TopRight.position.z+TopLeft.position.z)/2) {
            RandomZ = Random.Range(RandomZmin, RandomZmax) + Random.Range(0, 1) * (TopRight.position.z - TopLeft.position.z) / 2;
        }
        else
        {
            RandomZ = Random.Range(RandomZmin, RandomZmax) - Random.Range(0, 1) * (TopRight.position.z - TopLeft.position.z) / 2;
        }
        Vector3 RandomVector = new Vector3(RandomX, RandomY, RandomZ);
        ControlPoint.position += RandomVector;
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
        _Tips = "";
        ShowWordTips(_Tips);

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

    public void InitValue()
    {
        if (PatientDataManager.instance.IsEvaluated == 1)
        {
            this._Direction = PatientDataManager.DirectionType.UponDirection;   //direction of shoot
            this._TargetDistance = _Caculator.InitDistance();   //distance of shoot
        }
        else if(PatientDataManager.instance.TrainingDirection == PatientDataManager.DirectionType.AnyDirection)
        {
            this._Direction = PatientDataManager.DirectionType.UponDirection;   //direction of shoot
            this._TargetDistance = PatientDataManager.instance.MaxDirection[0] * MinDistancePercent;   //distance of shoot
        }
        else
        {
            this._Direction = PatientDataManager.instance.TrainingDirection;
            this._TargetDistance = PatientDataManager.instance.MaxDirection[(int)_Direction] * MinDistancePercent;
        }
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
        // time over or evaluate over
        if (PatientDataManager.instance.TimeCount > PatientDataManager.Minute2Second(PatientDataManager.instance.TrainingTime)
            || EvaluateOver())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // evaluate over
    private bool EvaluateOver()
    {
        if(_Direction == PatientDataManager.DirectionType.RightDirection && FailTooMuch())
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // rest after a session
    // if rest is over return true
    private bool SessionRestTimeOver()
    {
        _RestTimeCount += Time.deltaTime;
        if (_RestTimeCount >= SessionRestTime)
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
        if (_RestTimeCount < PrepareTime)
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
        if (_RecordTimeCount < RecordTime)
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
