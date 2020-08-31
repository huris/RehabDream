using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ADLUIHandle : UIHandle
{

    [Header("UI")]
    public GameObject GameUI;
    public GameObject SettingUI;
    public GameObject PauseUI;
    public GameObject GameoverUI;
    public GameObject KinectDetectUI;
    public GameObject EncouragePicture;

    [Header("GameUI Objects")]
    public Text GameUIPatientNameText;
    public Text GameUITimeHistoryText;
    public Text GameUIADLTimeCountText;
    public Text GameUICutDownText;
    public Text GameUIEncourageText;



    public Text GameUITrainProgressText;
    public Text GameUITipsText;
    public Slider GameUIProgressSlider;

    [Header("KinectDetectUI Objects")]
    public Slider KinectDetectUIProgressSlider;
    public Text KinectStatusText;


    [Header("GameoverUI Objects")]
    public Text OpenDoorTimeText;
    public Text SitTimeText;
    public Text StandTimeText;
    public Text CookTimeText;
    public Text TotalTimeText;

    //[Header("SettingUI Toggle")]
    //public Toggle EntryToggle;
    //public Toggle PrimaryToggle;
    //public Toggle GeneralToggle;
    //public Toggle IntermediateToggle;
    //public Toggle AdvancedToggle;
    //public Toggle SoccerTrackTipsToggle;
    //public Toggle WordTipsToggle;
    //public Toggle SETipsTogle;

    [Header("SettingUI Slider")]
    public Slider BgmSlider;
    public Slider BgsSlider;
    public Slider SeSlider;

    [Header("GameState")]
    public ADLGameState ADLGameState;

    [Header("Parameter")]



    private float _GestureTimeCount = 0;
    private float _DetectTime = 3.0f;
    private float _ShowWrongLimbTime = 1.0f;

    private float _KinectTimeCount = 0f;

    public float WaitTime;
    public int playerIndex = 0;
    private Vector3 HandTipLeft;
    public Image Introduction;
    public Image Buttons;   // 下面三个button的背景
    public bool IsOver;



    // Use this for initialization
    void Start()
    {

        InitUIValue();

    }

    void Update()
    {

    }


    public override void InitUIValue()
    {
        //SetPlanDifficultyToggle();
        //SetTipsToggle();
        //SetMusicVolumeSlider();
        InitGameUIText();
    }

    public void InitGameUIText()
    {
        SetPatientInfoText(PatientDataManager.instance.PatientName, PatientDataManager.instance.MaxSuccessCount);
        SetTrainingProgress(0);
        SetADLTimeCountText(PatientDataManager.instance.ADLTimeCount);
    }






    #region SetUIValue according to DataManager

    // set patientinfo of GameUI
    public void SetPatientInfoText(string PatientName, long TimeHistory)
    {
        GameUIPatientNameText.text = "用户名：" + PatientName;
        GameUITimeHistoryText.text = TimeHistory.ToString() + "秒";
    }

    // set Progress of game
    public void SetTrainingProgress(int Stage)
    {
        GameUIProgressSlider.value = Stage / 3.0f;
        GameUITrainProgressText.text = "训练进度：" + Stage.ToString() + " / 3 阶段";
    }

    public void SetADLTimeCountText(float ADLTimeCount)
    {
        GameUIADLTimeCountText.text = ADLTimeCount.ToString() + "秒";
    }


    #endregion




    #region UI Button

    public void OnClickKinectDetectQuit()
    {
        PatientDataManager.instance.ResetGameData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    public void OnClickRestart()
    {
        PatientDataManager.instance.ResetGameData();
        LoadScene("ADLGame");
    }


    //open PauseUI
    public void OnClickOpenPauseUI()
    {
        ADLGameState.Pause();
        this.OpenUIAnimation(PauseUI);
        base.DisableUIButton(GameUI);
    }


    //pause continue game
    public void OnClickPauseUIContinue()
    {
        this.CloseUIAnimation(PauseUI);
        ADLGameState.Continue();
        base.EnableUIButton(GameUI);
    }



    #endregion

    //RegisterUI->LoginUI
    public void KinectDetectUI2GameUI()
    {

        base.CloseUIAnimation(KinectDetectUI);
        base.OpenUIAnimation(GameUI);
    }

    public void GameUI2GameOverUI()
    {
        SetGeneralComment();
        base.CloseUIAnimation(GameUI);
        base.OpenUIAnimation(GameoverUI);
    }

    #region UI Animation

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

    #endregion

    public void SetGeneralComment()
    {
        OpenDoorTimeText.text = "开门用时 " + PatientDataManager.instance.OpenDoorTimeCount.ToString("#.#");
        SitTimeText.text = "坐下用时 " + PatientDataManager.instance.SitTimeCount.ToString("#.#");
        StandTimeText.text = "站起用时 " + PatientDataManager.instance.StandTimeCount.ToString("#.#");
        CookTimeText.text = "烹饪用时 " + PatientDataManager.instance.CookTimeCount.ToString("#.#");
        TotalTimeText.text = "总用时	  " + PatientDataManager.instance.ADLTimeCount.ToString("#.#");
    }

    public void SetKinectDetectProgress(float value)
    {
        KinectDetectUIProgressSlider.value = value;
    }



    public void SetTips(string Tips)
    {
        GameUITipsText.text = Tips;
    }


    public void ShowCutDown(string CutDown)
    {
        GameUICutDownText.text = CutDown;
    }

    public void UpdateTimeCount(string TimeCount)
    {
        GameUIADLTimeCountText.text = TimeCount;
    }

    public IEnumerator ShowEncouragePicture(float ShowTime, float DisapperaTime, string Tips)
    {
        GameUIEncourageText.text = Tips;
        this.ShowCanvaUI(this.EncouragePicture, ShowTime);
        yield return new WaitForSeconds(ShowTime);
        this.CloseCanvaUI(this.EncouragePicture, DisapperaTime);
    }

}
