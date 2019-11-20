/* ============================================================================== 
* ClassName：StartUIHandle 
* Author：ChenShuwei 
* CreateDate：2019/10/18 ‏‎‏‎‏‎21:30:08 
* Version: 1.0
* ==============================================================================*/

using System;
using UnityEngine;
using UnityEngine.UI;

public class StartUIHandle : UIHandle
{

    [Header("UI")]
    public GameObject StartUI;
    public GameObject RegisterUI;
    public GameObject LoginUI;
    public GameObject SettingUI;
    public GameObject ExitUI;

    [Header("Toggle")]
    public Toggle PrimaryToggle;
    public Toggle GeneralToggle;
    public Toggle IntermediateToggle;
    public Toggle AdvancedToggle;
    public Toggle SoccerTrackTipsToggle;
    public Toggle WordTipsToggle;

    [Header("Slider")]
    public Slider BgmSlider;
    public Slider BgsSlider;
    public Slider SeSlider;

    [Header("Input")]
    public InputField LoginPatientIDField;
    public InputField LoginPatientPasswordField;
    public InputField RegisterPatientIDField;
    public InputField RegisterPatientNameField;
    public InputField RegisterPatientPasswordField;

    [Header("Kinect Status")]
    public Text KinectStatusText;


    // Use this for initialization
    void Start () {

        //StartUI = GameObject.Find("Canvas/Start");
        //SettingUI = GameObject.Find("Canvas/Setting");
        //LoginUI = GameObject.Find("Canvas/Login");
        //RegisterUI = GameObject.Find("Canvas/Register");

        //PrimaryToggle = StartUI.transform.Find("ToggleGroup/PrimaryToggle").gameObject.GetComponent<Toggle>();
        //GeneralToggle = StartUI.transform.Find("ToggleGroup/GeneralToggle").gameObject.GetComponent<Toggle>();
        //IntermediateToggle = StartUI.transform.Find("ToggleGroup/IntermediateToggle").gameObject.GetComponent<Toggle>();
        //AdvancedToggle = StartUI.transform.Find("ToggleGroup/AdvancedToggle").gameObject.GetComponent<Toggle>();

        //PrimaryToggle.onValueChanged.AddListener((bool select) => { OnPrimaryToggleValueChanged(select); });
        //GeneralToggle.onValueChanged.AddListener((bool select) => { OnGeneralToggleValueChanged(select); });
        //IntermediateToggle.onValueChanged.AddListener((bool select) => { OnIntermediateToggleValueChanged(select); });
        //AdvancedToggle.onValueChanged.AddListener((bool select) => { OnAdvancedToggleValueChanged(select); });

        //PrimaryToggle.onValueChanged.AddListener(ifselect => { if (ifselect) OnToggleValueChanged(PrimaryToggle); });
        //GeneralToggle.onValueChanged.AddListener(ifselect => { if (ifselect) OnToggleValueChanged(GeneralToggle); });
        //IntermediateToggle.onValueChanged.AddListener(ifselect => { if (ifselect) OnToggleValueChanged(IntermediateToggle); });
        //AdvancedToggle.onValueChanged.AddListener(ifselect => { if (ifselect) OnToggleValueChanged(AdvancedToggle); });

        //PrimaryToggle.onValueChanged.AddListener(OnPrimaryToggleValueChanged);
        //GeneralToggle.onValueChanged.AddListener(OnGeneralToggleValueChanged);
        //IntermediateToggle.onValueChanged.AddListener(OnIntermediateToggleValueChanged);
        //AdvancedToggle.onValueChanged.AddListener(OnAdvancedToggleValueChanged);

        InitUIValue();
    }
	
	// Update is called once per frame
	void Update () {
        // Kinect status
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

        if (Input.GetKeyDown(KeyCode.Space))
        {      //按空格键 触发快速开始按钮
            OnClickQuickStartButton();
        }
    }



    //confirm Register
    public void ConfirmRegister()  
    {
        //是否应添加一个提示框来输出 错误信息，并将PatientIDField设置成只允许输入数字以避免excepttion
        if (RegisterPatientIDField.text.Equals("") || RegisterPatientNameField.text.Equals("") || RegisterPatientPasswordField.Equals(""))
        {
            RegisterPatientIDField.text = "各表项不应为空";
            return;
        }

        long PatientID = 0;
        //可能exception
        try
        {
            PatientID = long.Parse(RegisterPatientIDField.text);
        }
        catch
        {
            RegisterPatientIDField.text = "用户ID只能为数字";
            return;
        }
       
        string PatientName = RegisterPatientNameField.text;
        string PatientPassword = RegisterPatientPasswordField.text;


        DatabaseManager.DatabaseReturn Return = DatabaseManager.instance.Register(PatientID, PatientName, PatientPassword);
        switch (Return)
        {
            case DatabaseManager.DatabaseReturn.Success:                //register success
                OnClickCloseRegisterUI();                               //close register UI
                base.EnableUIButton(StartUI);
                break;
            case DatabaseManager.DatabaseReturn.Fail:
                break;
            case DatabaseManager.DatabaseReturn.NullInput:
                RegisterPatientIDField.text = "各表项不应为空";
                break;
            case DatabaseManager.DatabaseReturn.AlreadyExist:
                RegisterPatientIDField.text = "用户ID已存在";
                break;
            case DatabaseManager.DatabaseReturn.Exception:
                RegisterPatientIDField.text = "数据库异常";
                break;

            default:
                break;
        }


    }

    

    //confirm login
    public void ConfirmLogin()  
    {
        if (LoginPatientIDField.text.Equals("") || LoginPatientPasswordField.text.Equals(""))
        {
            LoginPatientIDField.text = "用户ID、用户密码不能为空";
            return;
        }

        long PatientID = 0;

        //read login PatientID and PatientPassword
        try
        {
            Debug.Log(LoginPatientIDField.text);
            PatientID = long.Parse(LoginPatientIDField.text);
        }
        catch
        {
            LoginPatientIDField.text = "用户ID只能为数字";
            return;
        }
        string PatientPassword = LoginPatientPasswordField.text;

        DatabaseManager.DatabaseReturn Return = DatabaseManager.instance.Login(PatientID, PatientPassword);
        switch (Return)
        {
            case DatabaseManager.DatabaseReturn.Success:    //login success

                // set PatientID, PatientName
                DataManager.instance.SetUserMessage(PatientID, DatabaseManager.instance.ReadPatientName(PatientID),DatabaseManager.instance.ReadPatientSex(PatientID));

                // read TrainingPlan and Set TrainingPlan(PlanDifficulty GameCount PlanCount)
                SetDataTrainingPlan();

                // Init UIValue(music volume and difficulty) according to TrainingPlan
                this.InitUIValue();

                // read PatientRecord and set PatientRecord(TrainingID, MaxSuccessCount)
                // 无论登录与否，都需要生成 TrainingID 和 MaxSuccessCount，
                // 因此将 SetDataPatientRecord() 放在 GameUIHandle类/start() 调用
                // SetDataPatientRecord();

                //close login UI
                OnClickCloseLoginUI();
                //enable start UI buttons
                base.EnableUIButton(StartUI);
                //Login & Register button setactive(false)

                break;
            case DatabaseManager.DatabaseReturn.Fail:
                LoginPatientIDField.text = "用户ID或用户密码错误";
                break;
            case DatabaseManager.DatabaseReturn.NullInput:
                LoginPatientIDField.text = "用户ID、用户密码不能为空";

                break;
            default:
                break;
        }

        Debug.Log("@StartUIHandle: ConfirmLogin Success");

    }


    //StartUI->RegisterUI
    public void OnClickOpenRegisterUI()
    {
        base.OpenUIAnimation(RegisterUI);
        base.DisableUIButton(StartUI);
    }


    //close RegisterUI
    public void OnClickCloseRegisterUI()
    {
        base.CloseUIAnimation(RegisterUI);
        ResetRegisterField();
        base.EnableUIButton(StartUI);
    }


    //StartUI->LoginUI
    public void OnClickOpenLoginUI()
    {
        base.OpenUIAnimation(LoginUI);
        base.DisableUIButton(StartUI);
    }

    // StartUI -> ExitUI
    public void OnClickOpenExitUI()
    {
        base.OpenUIAnimation(ExitUI);
        base.DisableUIButton(StartUI);
    }



    //close LoginUI
    public void OnClickCloseLoginUI()
    {
        ResetLoginField();
        base.CloseUIAnimation(LoginUI);
        base.EnableUIButton(StartUI);
    }

    //LoginUI->RegisterUI
    public void LoginUIToRegisterUI()
    {
        base.CloseUIAnimation(LoginUI);
        ResetLoginField();
        base.DisableUIButton(StartUI);
        base.OpenUIAnimation(RegisterUI);
    }


    //RegisterUI->LoginUI
    public void RegisterUIToLoginUI()
    {

        base.CloseUIAnimation(RegisterUI);
        ResetRegisterField();
        base.DisableUIButton(StartUI);
        base.OpenUIAnimation(LoginUI);
    }

    //StartUI->SettingUI
    public void OnClickOpenSettingUI()
    {
        base.OpenUIAnimation(SettingUI);
        base.DisableUIButton(StartUI);
    }


    //Close SettingUI
    public void OnClickCloseSettingUI()
    {
        base.CloseUIAnimation(SettingUI);
        base.EnableUIButton(StartUI);
    }


    // Close ExitUI
    public void OnClickCloseExitUI()
    {
        base.CloseUIAnimation(ExitUI);
        base.EnableUIButton(StartUI);
    }


    //quick start
    public void OnClickQuickStartButton()   
    {
        LoadScene("Game");
    }

    //quit game
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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

    

    public void ResetLoginField()
    {
        LoginPatientIDField.text = "";
        LoginPatientPasswordField.text = "";
    }

    public void ResetRegisterField()
    {
        RegisterPatientIDField.text = "";
        RegisterPatientNameField.text = "";
        RegisterPatientPasswordField.text = "";
    }


    #region SetUIValue(Difficulty, MusicVolume) according to DataManager

    //set PlanDifficultyToggle as Plan
    public void SetPlanDifficultyToggle()
    {
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
        Debug.Log("@StartUIHandle");

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

    //set UI value
    public override void InitUIValue()
    {
        SetPlanDifficultyToggle();
        SetTipsToggle();
        SetMusicVolumeSlider();
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


    #region Abandoned toggles handle

    // all of toggles(Difficulty)
    // abandoned
    public void OnToggleValueChanged(Toggle item)
    {
        switch (item.name)
        {
            case "PrimaryToggle":
                DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.Primary);
                Debug.Log("Primary difficulty is set");
                break;
            case "GeneralToggle":
                DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.General);
                Debug.Log("General difficulty is set");
                break;
            case "IntermediateToggle":
                DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.Intermediate);
                Debug.Log("Intermediate difficulty is set");
                break;
            case "AdvancedToggle":
                DataManager.instance.SetPlanDifficulty(DataManager.DifficultyType.Advanced);
                Debug.Log("Advanced difficulty is set");
                break;
            default:
                break;
        }
    }

    //all of Sliders(Volume)
    //abandoned
    public void OnSliderValueChange(float value, Slider EventSender)
    {
        
        switch (EventSender.name)
        {
            case "bgmVolume":
                DataManager.instance.SetbgmVolume(value);
                SoundManager.instance.SetVolume(SoundManager.SoundType.BGM, value);
                Debug.Log("change bgmVolume to "+ value.ToString());
                break;
            case "bgsVolume":
                DataManager.instance.SetbgsVolume(value);
                SoundManager.instance.SetVolume(SoundManager.SoundType.BGS, value);
                Debug.Log("change bgsVolume " + value.ToString());
                break;
            case "seVolume":
                DataManager.instance.SetseVolume(value);
                Debug.Log("change seVolume " + value.ToString());
                break;
            default:
                break;
        }
    }

    #endregion


}




