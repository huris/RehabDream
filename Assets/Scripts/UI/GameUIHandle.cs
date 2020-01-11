/* ============================================================================== 
* ClassName：GameUIHandle 
* Author：ChenShuwei 
* CreateDate：2019/10/18 ‏‎‏‎21:32:01 
* Version: 1.0
* ==============================================================================*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandle : UIHandle
{

    [Header("UI")]
    public GameObject GameUI;
    public GameObject SettingUI;
    public GameObject PauseUI;
    public GameObject GameoverUI;
    public GameObject KinectDetectUI;

    [Header("GameUI Objects")]
    public Text GameUIPatientNameText;
    public Text GameUISuccessCountText;
    public Text GameUIMaxSuccessCountText;
    public Text GameUITrainProgressText;
    public Text GameUITipsText;
    public Text GameUIAddSuccessCountText;
    public GameObject Tips;
    public GameObject WrongLimb;
    public Slider GameUIProgressSlider;

    [Header("KinectDetectUI Objects")]
    public Slider KinectDetectUIProgressSlider;
    public Text KinectStatusText;


    [Header("GameoverUI Objects")]
    public Text SuccessCountText;
    public Text GameCountText;
    public Text TrainingDifficulty;
    public Text TrainingTime;

    [Header("SettingUI Toggle")]
    public Toggle EntryToggle;
    public Toggle PrimaryToggle;
    public Toggle GeneralToggle;
    public Toggle IntermediateToggle;
    public Toggle AdvancedToggle;
    public Toggle SoccerTrackTipsToggle;
    public Toggle WordTipsToggle;

    [Header("SettingUI Slider")]
    public Slider BgmSlider;
    public Slider BgsSlider;
    public Slider SeSlider;


    [Header("GameState")]
    public GameState GameState;

    private float _GestureTimeCount = 0;
    private float _DetectTime = 3.0f;
    private float _ShowWrongLimbTime = 1.0f;

    // Use this for initialization
    void Start()
    {
        //GameUI = GameObject.Find("Canvas/Game");
        //SettingUI = GameObject.Find("Canvas/Setting");
        //PauseUI = GameObject.Find("Canvas/Pause");
        //GameoverUI = GameObject.Find("Canvas/Gameover");


        // read PatientRecord and set PatientRecord(TrainingID, MaxSuccessCount)
        // 无论登录与否，都需要生成 TrainingID 和 MaxSuccessCount，
        // 因此将 SetDataPatientRecord() 放在 GameUIHandle类/start() 调用
        SetDataPatientRecord();
        InitUIValue();
        PatientGestureListener.Instance.ResetTposeLastTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (KinectDetectUI.activeSelf == true)
        {
            if (KinectManager.Instance.displaySkeletonLines == false)
            {
                KinectManager.Instance.displaySkeletonLines = true;
                KinectManager.Instance.displayColorMap = true;
                KinectManager.Instance.displayUserMap = true;
            }
            SetKinectStatus();
            long userId = KinectManager.Instance.GetPrimaryUserID();

            if (userId == 0)
            {      // 是否人物被检测到
                _GestureTimeCount = 0;
            }
            else
            {
                if (GestureOver())
                {
                    SetKinectDetectProgress(1);
                    OnClickDirectStart();
                }
                else
                {
                    SetKinectDetectProgress(_GestureTimeCount / _DetectTime);
                }
                
            }
        }
        else
        {
            if (KinectManager.Instance.displaySkeletonLines == true)
            {
                KinectManager.Instance.displaySkeletonLines = false;
                KinectManager.Instance.displayColorMap = false;
                KinectManager.Instance.displayUserMap = false;
            }
        }
    }

    #region KinectDetectUI Button

    // direct start
    public void OnClickDirectStart()
    {
        this.CloseUIAnimation(KinectDetectUI);
        this.OpenUIAnimation(GameUI);
        GameState.StateShoot2SessionOver();
    }

    // 
    public void OnClickQuitGame()
    {
        //base.LoadScene("Start");
        base.LoadScene("03-DoctorUI");
    }

    #endregion


    //open SettingUI
    public void OnClickOpenSettingUI()
    {
        GameState.Pause();
        this.OpenUIAnimation(SettingUI);
        base.DisableUIButton(GameUI);
    }


    //Close SettingUI
    public void OnClickCloseSettingUI()
    {
        this.CloseUIAnimation(SettingUI);
        GameState.Continue();
        base.EnableUIButton(GameUI);
    }


    //open PauseUI
    public void OnClickOpenPauseUI()
    {
        GameState.Pause();
        this.OpenUIAnimation(PauseUI);
        base.DisableUIButton(GameUI);
    }


    //pause continue game
    public void OnClickPauseUIContinue()
    {
        this.CloseUIAnimation(PauseUI);
        GameState.Continue();
        base.EnableUIButton(GameUI);
    }


    //pause exit game
    //可能为pause界面的exit按钮，也可能为Gameover界面的exit按钮
    public void OnClickExit()
    {
        PatientDataManager.instance.ResetGameData();
        // reset PatientRecord
        SetDataPatientRecord();
        // reset TrainingPlan

        // reset TrainingPlan
        // 退出游戏回Start界面时，从数据库重新读取PlanDifficulty,GameCount, PlanCount（可能读不到，使用默认返回值）
        SetDataTrainingPlan();

        Debug.Log("@GameUIHandle: Exit success");
        //LoadScene("Start");
        base.LoadScene("03-DoctorUI");
    }


    //Restart game
    public void OnClickRestartButton()
    {
        // reset game data
        PatientDataManager.instance.ResetGameData();
        // reset PatientRecord
        SetDataPatientRecord();

        // 再来一次游戏时，保留不变PlanDifficulty,GameCount，仅令PlanCount--（可能为负）
        PatientDataManager.instance.SetPlanCount(PatientDataManager.instance.PlanCount - 1);

        Debug.Log("@GameUIHandle: Restart success");
        LoadScene("Game");

    }


    //load GameoverUI
    public void LoadGameoverUI()
    {
        SetGeneralComment();
        this.OpenUIAnimation(GameoverUI);
        base.DisableUIButton(GameUI);
        Debug.Log("@GameUIHandle: Load GameoverUI success");
    }

    //set General Comment of GameoverUI
    public void SetGeneralComment()
    {
        SuccessCountText.text = "成功次数  " + PatientDataManager.instance.SuccessCount.ToString();
        GameCountText.text = "训练总量  " + PatientDataManager.instance.GameCount.ToString();
        TrainingDifficulty.text = "训练难度  " + PatientDataManager.DifficultyType2Str(PatientDataManager.instance.PlanDifficulty);
        TrainingTime.text = "训练时长  " + ((int)(PatientDataManager.instance.TrainingEndTime - PatientDataManager.instance.TrainingStartTime).TotalSeconds).ToString();
        Debug.Log("@GameUIHandle: Set GeneralComment success");
    }

    //read PatientRecord and set PatientRecord(TrainingID, MaxSuccessCount) ,MaxDirection
    public void SetDataPatientRecord()
    {
        // set TrainingID
        PatientDataManager.instance.SetTrainingID(PatientDatabaseManager.instance.GenerateTrainingID());

        // set  MaxSuccessCount
        PatientDataManager.instance.SetMaxSuccessCount(
            PatientDatabaseManager.instance.ReadMaxSuccessCount(
                PatientDataManager.instance.PatientID,
                PatientDataManager.DifficultyType2Str(PatientDataManager.instance.TrainingDifficulty)
                )
            );

        // set MaxDirection = Evaluate Direction
        PatientDataManager.instance.SetMaxDirection(
            PatientDatabaseManager.instance.ReadEvaluateDirection(PatientDataManager.instance.TrainingID)
            );
    }

    //read TrainingPlan and Set TrainingPlan(PlanDifficulty GameCount PlanCount)
    public void SetDataTrainingPlan()
    {
        // read TrainingPlan(PlanDifficulty, GameCount, PlanCount)
        PatientDatabaseManager.TrainingPlanResult result = PatientDatabaseManager.instance.ReadTrainingPlan(PatientDataManager.instance.PatientID);
        // set TrainingPlan(PlanDifficulty, GameCount, PlanCount)
        PatientDataManager.instance.SetTrainingPlan(
            PatientDataManager.Str2DifficultyType(result.PlanDifficulty),
            result.GameCount,
            result.PlanCount,
            PatientDataManager.Str2DirectionType(result.PlanDirection),
            result.PlanTime
            );
    }


    // set TipsText in GameUI
    public void SetTipsText(string Tip)
    {
        string[] Tips = new string[] { "上", "左上", "右上", "下", "左下", "右下", "左", "右" };
        if (Array.IndexOf(Tips, Tip)==-1)
        {
            // 不是方向字符串
            GameUITipsText.text = Tip;
        }
        else{

            // 是方向字符串
            GameUITipsText.text = "请双手握拳，朝" + Tip + "方接球";
        }

    }

    // 提示使用了错误肢体
    public void ShowWrongLimb()
    {
        Tips.SetActive(false);
        WrongLimb.SetActive(true);
        StartCoroutine(CloseWrongLimb(_ShowWrongLimbTime));
    }

    // 结束报错
    private IEnumerator CloseWrongLimb(float ShowWrongLimbTime)
    {
        yield return new WaitForSeconds(ShowWrongLimbTime); //先直接返回，之后的代码等待给定的时间周期过完后执行
        WrongLimb.SetActive(false);
        Tips.SetActive(true);
    }

    // show ShowAddSuccessCountText
    public IEnumerator ShowAddSuccessCountText(int AddCount, float AddSuccessCountTime)
    {
        GameUIAddSuccessCountText.enabled = true;
        GameUIAddSuccessCountText.text = "+" + AddCount.ToString();
        yield return new WaitForSeconds(AddSuccessCountTime);
        GameUIAddSuccessCountText.enabled = false;
    }



    #region SetUIValue according to DataManager

    // set patientinfo of GameUI
    public void SetPatientInfoText(string PatientName, long Max_SuccessCount)
    {
        GameUIPatientNameText.text = "用户名：" + PatientName;
        GameUIMaxSuccessCountText.text = Max_SuccessCount.ToString();
    }

    // set Progress of game
    public void SetTrainingProgress(float TimeCount, float TrainingTime)
    {
        GameUIProgressSlider.value = TimeCount / TrainingTime;
        GameUITrainProgressText.text = "训练进度：" + GameUIProgressSlider.value.ToString("0%");
    }

    // set Progress of KinectDetect
    public void SetKinectDetectProgress(float value)
    {
        KinectDetectUIProgressSlider.value = value;
    }

    // KinectDetect Gesture is over
    public bool GestureOver()
    {
        _GestureTimeCount += Time.deltaTime;
        if (_GestureTimeCount < _DetectTime)
        {
            return false;
        }
        else
        {
            _GestureTimeCount = 0f;
            return true;

        }
    }


    // set SuccessCount of GameUI
    public void SetSuccessCountText(long SuccessCount)
    {
        GameUISuccessCountText.text = SuccessCount.ToString();
        //Debug.Log("@GameUIHandle: "+GameUISuccessCountText.text);
    }

    // set KinectStatus in KinectDetectUI
    public void SetKinectStatus()
    {
        if (KinectManager.Instance.IsInitialized())
        {
            if (KinectManager.Instance.IsUserDetected())
            {
                KinectStatusText.text = "已检测到用户";
            }
            else
            {
                KinectStatusText.text = "未检测到用户";
            }
        }
        else
        {
            KinectStatusText.text = "Kinect 未就绪";
        }
    }

    //set Text of GameUI
    public void InitGameUIText()
    {
        SetPatientInfoText(PatientDataManager.instance.PatientName, PatientDataManager.instance.MaxSuccessCount);
        SetTrainingProgress(PatientDataManager.instance.TimeCount, PatientDataManager.Minute2Second(PatientDataManager.instance.TrainingTime));
        SetSuccessCountText(PatientDataManager.instance.SuccessCount);
    }

    //set PlanDifficultyToggle as Plan
    public void SetPlanDifficultyToggle()
    {
        EntryToggle.isOn = false;
        PrimaryToggle.isOn = false;
        GeneralToggle.isOn = false;
        IntermediateToggle.isOn = false;
        AdvancedToggle.isOn = false;

        switch (PatientDataManager.instance.TrainingDifficulty)
        {
            case PatientDataManager.DifficultyType.Entry:
                EntryToggle.isOn = true;
                break;
            case PatientDataManager.DifficultyType.Primary:
                PrimaryToggle.isOn = true;
                break;
            case PatientDataManager.DifficultyType.General:
                GeneralToggle.isOn = true;
                break;
            case PatientDataManager.DifficultyType.Intermediate:
                IntermediateToggle.isOn = true;
                break;
            case PatientDataManager.DifficultyType.Advanced:
                AdvancedToggle.isOn = true;
                break;
        }

    }

    // set game Tips
    public void SetTipsToggle()
    {
        SoccerTrackTipsToggle.isOn = PatientDataManager.instance.SoccerTrackTips;
        WordTipsToggle.isOn = PatientDataManager.instance.WordTips;
    }

    //set Music Slider
    public void SetMusicVolumeSlider()
    {
        BgmSlider.value = PatientDataManager.instance.bgmVolume;
        SeSlider.value = PatientDataManager.instance.seVolume;
    }


    //open UI
    public override void OpenUIAnimation(GameObject UI)
    {
        DoTweenUIAnimation temp = UI.GetComponent<DoTweenUIAnimation>();

        UI.SetActive(true);
        temp?.OpenUIAnimation();

    }


    // close UI
    public override void CloseUIAnimation(GameObject UI)
    {
        DoTweenUIAnimation temp = UI.GetComponent<DoTweenUIAnimation>();
        if (temp == null)   // no animation to be played
        {
            UI.SetActive(false);
        }
        else
        {
            temp.CloseUIAnimation();
        }
    }


    //set UI value(Text of GameUI; slider,toggle of SettingUI)
    public override void InitUIValue()
    {
        SetPlanDifficultyToggle();
        SetTipsToggle();
        SetMusicVolumeSlider();
        InitGameUIText();
    }


    #endregion



    #region Toggles


    // Entry Toggle
    public void OnEntryToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.DifficultyType.Entry);
            Debug.Log("@EntryToggle: Entry difficulty is set");
        }

    }

    // Primary Toggle
    public void OnPrimaryToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.DifficultyType.Primary);
            Debug.Log("@PrimaryToggle: Primary difficulty is set");
        }

    }

    // General Toggle
    public void OnGeneralToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.DifficultyType.General);
            Debug.Log("@GeneralToggle: General difficulty is set");
        }

    }

    // Intermediate Toggle
    public void OnIntermediateToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.DifficultyType.Intermediate);
            Debug.Log("@IntermediateToggle: Intermediate difficulty is set");
        }

    }

    // Advanced Toggle
    public void OnAdvancedToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.DifficultyType.Advanced);
            Debug.Log("@AdvancedToggle: Advanced difficulty is set");
        }

    }

    // LimbTips Toggle
    public void OnSoccerTrackTipsToggleValueChanged(bool isOn)
    {

        PatientDataManager.instance.SetSoccerTrackTipss(isOn);
        Debug.Log("@SoccerTrackTipsToggle: SoccerTrackTipss is set " + isOn.ToString());

    }

    // WordTips Toggle
    public void OnWordTipsToggleValueChanged(bool isOn)
    {

        PatientDataManager.instance.SetWordTips(isOn);
        Debug.Log("@WordTipsToggle: WordTips is set " + isOn.ToString());

    }

    #endregion



    #region Music Slider

    //bgmVolume Slider
    public void OnbgmSliderValueChange(float value)
    {
        PatientDataManager.instance.SetbgmVolume(value);
        Debug.Log("@bgmSlider: change bgmVolume to " + value.ToString());
    }

    //bgsVolume Slider
    public void OnbgsSliderValueChange(float value)
    {
        PatientDataManager.instance.SetbgsVolume(value);
        Debug.Log("@bgsSlider: change bgsVolume to " + value.ToString());
    }

    //seVolume Slider
    public void OnseSliderValueChange(float value)
    {
        PatientDataManager.instance.SetseVolume(value);
        Debug.Log("@seSlider: change seVolume to " + value.ToString());
    }

    #endregion
}
