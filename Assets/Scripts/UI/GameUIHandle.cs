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

    [Header("GameUI Objects")]
    public Text GameUIPatientNameText;
    public Text GameUISuccessCountText;
    public Text GameUIMaxSuccessCountText;
    public Text GameUITrainProgressText;
    public Text GameUITipsText;
    public Text GameUIAddSuccessCountText;
    public Slider GameUIProgressSlider;

    [Header("GameoverUI Objects")]
    public Text SuccessCountText;
    public Text GameCountText;
    public Text TrainingDifficulty;
    public Text TrainingTime;

    [Header("SettingUI Toggle")]
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
        // SetDataPatientRecord();
        InitUIValue();
    }

    // Update is called once per frame
    void Update()
    {

     
    }




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
        DataManager.instance.ResetGameData();
        // reset PatientRecord
        SetDataPatientRecord();
        // reset TrainingPlan

        // reset TrainingPlan
        // 退出游戏回Start界面时，从数据库重新读取PlanDifficulty,GameCount, PlanCount（可能读不到，使用默认返回值）
        SetDataTrainingPlan();

        Debug.Log("@GameUIHandle: Exit success");
        LoadScene("03-DoctorUI");
    }


    //Restart game
    public void OnClickRestartButton()   
    {
        // reset game data
        DataManager.instance.ResetGameData();
        // reset PatientRecord
        SetDataPatientRecord();

        // 再来一次游戏时，保留不变PlanDifficulty,GameCount，仅令PlanCount--（可能为负）
        DataManager.instance.SetPlanCount(DataManager.instance.PlanCount - 1);

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
        SuccessCountText.text = "成功次数  " + DataManager.instance.SuccessCount.ToString();
        GameCountText.text = "训练总量  " + DataManager.instance.GameCount.ToString();
        TrainingDifficulty.text = "训练难度  " + DataManager.DifficultyType2Str(DataManager.instance.PlanDifficulty);
        TrainingTime.text = "训练时长  " + ((int)(DataManager.instance.TrainingEndTime - DataManager.instance.TrainingStartTime).TotalSeconds).ToString();
        Debug.Log("@GameUIHandle: Set GeneralComment success");
    }

    //read PatientRecord and set PatientRecord(TrainingID, MaxSuccessCount)
    public void SetDataPatientRecord()
    {
        // set TrainingID
        DataManager.instance.SetTrainingID(DatabaseManager.instance.GenerateTrainingID());

        // set  MaxSuccessCount
        DataManager.instance.SetMaxSuccessCount(
            DatabaseManager.instance.ReadMaxSuccessCount(
                DataManager.instance.PatientID,
                DataManager.DifficultyType2Str(DataManager.instance.TrainingDifficulty)
                )
            );
    }

    //read TrainingPlan and Set TrainingPlan(PlanDifficulty GameCount PlanCount)
    public void SetDataTrainingPlan()
    {
        // read TrainingPlan(PlanDifficulty, GameCount, PlanCount)
        Tuple<string, long, long> TrainingPlan = DatabaseManager.instance.ReadTrainingPlan(DataManager.instance.PatientID);
        // set TrainingPlan(PlanDifficulty, GameCount, PlanCount)
        DataManager.instance.SetTrainingPlan(
            DataManager.Str2DifficultyType(TrainingPlan.Item1),
            TrainingPlan.Item2,
            TrainingPlan.Item3
            );
    }


    // set TipsText in GameUI
    public void SetTipsText(string TipsLimb)
    {
        if (TipsLimb == "")
        {
            GameUITipsText.text = "";
        }
        else
        {
            GameUITipsText.text = "请使用"+ TipsLimb + "阻拦足球";
        }
        
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
    public void SetPatientInfoText(string PatientName,long Max_SuccessCount)
    {
        GameUIPatientNameText.text = "用户名：" + PatientName;
        GameUIMaxSuccessCountText.text = Max_SuccessCount.ToString();
    }

    // set Progress of game
    public void SetTrainingProgress(long FinishCount, long GameCount)
    {
        GameUITrainProgressText.text = "训练进度："+ FinishCount.ToString()+ " / "+ GameCount.ToString() + " 次";
        GameUIProgressSlider.value = FinishCount / (float)GameCount;
        //Debug.Log("@GameUIHandle: SetProgress = " + FinishCount / (float)GameCount);
    }

    // set SuccessCount of GameUI
    public void SetSuccessCountText(long SuccessCount)
    {
        GameUISuccessCountText.text = SuccessCount.ToString();
        //Debug.Log("@GameUIHandle: "+GameUISuccessCountText.text);
    }

    //set Text of GameUI
    public void InitGameUIText()
    {
        SetPatientInfoText(DataManager.instance.PatientName,DataManager.instance.MaxSuccessCount);
        SetTrainingProgress(DataManager.instance.FinishCount, DataManager.instance.GameCount);
        SetSuccessCountText(DataManager.instance.SuccessCount);
    }

    //set PlanDifficultyToggle as Plan
    public void SetPlanDifficultyToggle()
    {
        PrimaryToggle.isOn = false;
        GeneralToggle.isOn = false;
        IntermediateToggle.isOn = false;
        AdvancedToggle.isOn = false;

        switch (DataManager.instance.TrainingDifficulty)
        {
            case DataManager.DifficultyType.Primary:
                PrimaryToggle.isOn = true;
                break;
            case DataManager.DifficultyType.General:        
                GeneralToggle.isOn = true;
                break;
            case DataManager.DifficultyType.Intermediate:
                IntermediateToggle.isOn = true;
                break;
            case DataManager.DifficultyType.Advanced:
                AdvancedToggle.isOn = true;
                break;
        }

    }

    // set game Tips
    public void SetTipsToggle()
    {
        SoccerTrackTipsToggle.isOn = DataManager.instance.SoccerTrackTips;
        WordTipsToggle.isOn = DataManager.instance.WordTips;
    }

    //set Music Slider
    public void SetMusicVolumeSlider()
    {
        BgmSlider.value = DataManager.instance.bgmVolume;
        SeSlider.value = DataManager.instance.seVolume;
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


    // Primary Toggle
    public void OnPrimaryToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.Primary);
            Debug.Log("@PrimaryToggle: Primary difficulty is set");
        }

    }


    // General Toggle
    public void OnGeneralToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.General);
            Debug.Log("@GeneralToggle: General difficulty is set");
        }

    }

    // Intermediate Toggle
    public void OnIntermediateToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.Intermediate);
            Debug.Log("@IntermediateToggle: Intermediate difficulty is set");
        }

    }

    // Advanced Toggle
    public void OnAdvancedToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.Advanced);
            Debug.Log("@AdvancedToggle: Advanced difficulty is set");
        }

    }

    // LimbTips Toggle
    public void OnSoccerTrackTipsToggleValueChanged(bool isOn)
    {

        DataManager.instance.SetSoccerTrackTipss(isOn);
        Debug.Log("@SoccerTrackTipsToggle: SoccerTrackTipss is set " + isOn.ToString());

    }

    // WordTips Toggle
    public void OnWordTipsToggleValueChanged(bool isOn)
    {

        DataManager.instance.SetWordTips(isOn);
        Debug.Log("@WordTipsToggle: WordTips is set " + isOn.ToString());

    }

    #endregion



    #region Music Slider

    //bgmVolume Slider
    public void OnbgmSliderValueChange(float value)
    {
        DataManager.instance.SetbgmVolume(value);
        Debug.Log("@bgmSlider: change bgmVolume to " + value.ToString());
    }

    //bgsVolume Slider
    public void OnbgsSliderValueChange(float value)
    {
        DataManager.instance.SetbgsVolume(value);
        Debug.Log("@bgsSlider: change bgsVolume to " + value.ToString());
    }

    //seVolume Slider
    public void OnseSliderValueChange(float value)
    {
        DataManager.instance.SetseVolume(value);
        Debug.Log("@seSlider: change seVolume to " + value.ToString());
    }

    #endregion
}
