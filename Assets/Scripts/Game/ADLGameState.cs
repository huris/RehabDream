using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HighlightingSystem;

public class ADLGameState : MonoBehaviour
{


    [Header("GameObjects in Game")]
    public GameObject BedroomDoor;

    public GameObject HumanBedroom;
    public GameObject HumanToilet;
    public GameObject HumanKitchen;

    public GameObject BedroomCamera;
    public GameObject ToiletCamera;
    public GameObject KitchenCamera;

    public GameObject[] Foods;

    [Header("Transform")]
    public Transform BedroomTransform;
    public Transform ToiletTransform;
    public Transform KitchenTransform;

    public Transform SitTransform;

    public Transform LeftHandTransform;
    public Transform RightHandTransform;

    [Header("Transform")]
    public Text TipsText;



    [Header("Music")]
    public AudioClip CheerUpSE;         //cheer up music



    [Header("UI")]
    public ADLUIHandle UIHandle;

    [Header("Time Paraments")]
    public static float ShooterTime = 1.0f;
    public static float SessionRestTime = 3.0f;      // rest after each shoot


    [Header("Distance Paraments")]
    public static float KinectDetectTime = 3f;
    public static float OpenDoorCutDown = 3f;
    public static float SitCutDown = 3f;
    public static float StandCutDown = 3f;
    public static float CookCutDown = 3f;
    public static float RestTime = 3f;

    [Header("Other Paraments")]
    public static int AddCount = 1;


    [Header("Random Range")]
    public float RandomXmin = -0.2f;


    private AvatarController _BedroomPlayerController;
    private AvatarController _ToiletPlayerController;
    private AvatarController _KitchenPlayerController;

    private Animator _BedroomPlayerAnimator;
    private Animator _ToiletPlayerAnimator;
    private Animator _KitchenPlayerAnimator;

    private DoorHandle _DoorHandle;   // Door collision
    private FoodCollisionHandle _LeftHandHandle;
    private FoodCollisionHandle _RightHandHandle;

    private int _NowFoodIndex = 0;

    private string _OpenDoorTips = "请站在原地，用手推开房门";
    private string _SitTips = "请坐在马桶上";
    private string _StandTips = "请从马桶上站起";
    private string _CookTips = "请拿起指定食物，并把它放入洗菜池中";
    private string _GetFoodTips = "请先拿起FOOD，FOOD已高亮显示";
    private string _PutFoodTips = "你已拿到FOOD，现在把它放入洗菜池中";


    private float _KinectTimeCount = 0f;
    private float _CutDownCount = 0f;
    private float _RestTimeCount = 0f;

    private bool _LeftRight = true;
    private bool _HaveGotten = false;



    // Moore FSM，使用Moore型有限状态机
    // Begin -> PrepareOpenDoor -> OpeningDoor -> 
    // OpenDoorOver -> PrepareSit -> Sitting -> SitOver -> 
    // PrepareStand -> Standing -> StandOver -> PrepareCook ->
    // Cooking -> CookOver -> GameOver

    private delegate void Begin2PrepareOpenDoor();
    private event Begin2PrepareOpenDoor _OnBegin2PrepareOpenDoor;

    private delegate void PrepareOpenDoor2OpeningDoor();
    private event PrepareOpenDoor2OpeningDoor _OnPrepareOpenDoor2OpeningDoor;

    private delegate void OpeningDoor2OpenDoorOver();
    private event OpeningDoor2OpenDoorOver _OnOpeningDoor2OpenDoorOver;

    private delegate void OpenDoorOver2PrepareSit();
    private event OpenDoorOver2PrepareSit _OnOpenDoorOver2PrepareSit;

    private delegate void PrepareSit2Sitting();
    private event PrepareSit2Sitting _OnPrepareSit2Sitting;

    private delegate void Sitting2SitOver();
    private event Sitting2SitOver _OnSitting2SitOver;

    private delegate void SitOver2PrepareStand();
    private event SitOver2PrepareStand _OnSitOver2PrepareStand;

    private delegate void PrepareStand2Standing();
    private event PrepareStand2Standing _OnPrepareStand2Standing;

    private delegate void Standing2StandOver();
    private event Standing2StandOver _OnStanding2StandOver;

    private delegate void StandOver2PrepareCook();
    private event StandOver2PrepareCook _OnStandOver2PrepareCook;

    private delegate void PrepareCook2Cooking();
    private event PrepareCook2Cooking _OnPrepareCook2Cooking;

    private delegate void Cooking2CookOver();
    private event Cooking2CookOver _OnCooking2CookOver;

    private delegate void CookOver2GameOver();
    private event CookOver2GameOver _OnCookOver2GameOver;



    private delegate void Start2GetFood();
    private event Start2GetFood _OnStart2GetFood;

    private delegate void GetingFood2PutingFood();
    private event GetingFood2PutingFood _OnGetingFood2PutingFood;

    private delegate void PutingFood2SessionOver();
    private event PutingFood2SessionOver _OnPutingFood2SessionOver;

    private delegate void SessionOver2Start();
    private event SessionOver2Start _OnSessionOver2Start;


    public enum State
    {
        Begin,

        PrepareOpenDoor,    //准备开门
        OpeningDoor,    //正在开门
        OpenDoorOver,   //开门完毕

        PrepareSit, //准备坐下
        Sitting,    //坐下
        SitOver,    //已经坐下

        PrepareStand,   //准备站起
        Standing,   //正在站起
        StandOver,  //已经站起

        PrepareCook,    //准备烹饪
        Cooking,    //烹饪中
        CookOver,   //烹饪完毕


        GameOver,    //游戏结束
        Pause       //游戏暂停

    }

    // Cooking状态的子状态
    private enum CookSubState
    {
        Start,
        GetingFood,
        PutingFood,
        SessionOver
    }

    private State _state = State.Begin;
    private CookSubState _SubState = CookSubState.Start;

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


        // 保证切换场景时kinect 与 AvatarController不会失效
        _BedroomPlayerController.playerId = KinectManager.Instance.GetPrimaryUserID();
        _ToiletPlayerController.playerId = KinectManager.Instance.GetPrimaryUserID();
        _KitchenPlayerController.playerId = KinectManager.Instance.GetPrimaryUserID();

        if (!KinectManager.Instance.avatarControllers.Contains(_BedroomPlayerController))
        {
            KinectManager.Instance.avatarControllers.Add(_BedroomPlayerController);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // OpeningDoor();
        // Prepare --> Shoot --> SessionOver(take a rest) --> Prepare --> Shoot --> SessionOver(take a rest) ...... --> GameOver
        switch (_state)
        {

            case State.Begin:
                if (IsDetected3s())
                {
                    _OnBegin2PrepareOpenDoor?.Invoke();
                }
                break;

            case State.PrepareOpenDoor:

                if (IsCutDownOver(OpenDoorCutDown))
                {
                    _OnPrepareOpenDoor2OpeningDoor?.Invoke();
                }
                break;

            case State.OpeningDoor:
                OpeningDoor();
                UpdateOpenDoorTimeCount();
                break;

            case State.OpenDoorOver:
                if (RestTimeOver())
                {
                    _OnOpenDoorOver2PrepareSit?.Invoke();
                }

                break;

            case State.PrepareSit:
                if (IsCutDownOver(SitCutDown))
                {
                    _OnPrepareSit2Sitting?.Invoke();
                }

                break;

            case State.Sitting:

                UpdateSitTimeCount();

                if (IsSitOver())
                {
                    _OnSitting2SitOver?.Invoke();
                }

                break;

            case State.SitOver:
                if (RestTimeOver())
                {
                    _OnSitOver2PrepareStand?.Invoke();
                }
                break;

            case State.PrepareStand:

                if (IsCutDownOver(StandCutDown))
                {
                    _OnPrepareStand2Standing?.Invoke();
                }
                break;

            case State.Standing:

                UpdateStandTimeCount();

                if (IsStandOver())
                {
                    _OnStanding2StandOver?.Invoke();
                }

                break;


            case State.StandOver:
                if (RestTimeOver())
                {
                    _OnStandOver2PrepareCook?.Invoke();
                }
                break;

            case State.PrepareCook:
                if (IsCutDownOver(CookCutDown))
                {
                    _OnPrepareCook2Cooking?.Invoke();
                }
                break;

            case State.Cooking:
                UpdateCookTimeCount();

                // 分析Cooking的子状态
                switch (_SubState)
                {
                    case CookSubState.Start:
                        _OnStart2GetFood?.Invoke();
                        break;

                    case CookSubState.GetingFood:


                        break;

                    case CookSubState.PutingFood:

                        // 将食物绑定到手上
                        if (_LeftRight == true)
                        {
                            Attach2LeftHand();
                        }
                        else
                        {
                            Attach2RightHand();
                        }

                        break;

                    case CookSubState.SessionOver:
                        //Debug.Log("_NowFoodIndex: " + _NowFoodIndex);
                        //Debug.Log("Foods.Length: " + Foods.Length);

                        if (_NowFoodIndex == Foods.Length -1 )   // 所有食物都已经放入洗菜池
                        {
                            _OnCooking2CookOver?.Invoke();
                        }
                        else
                        {
                            _NowFoodIndex++;
                            _OnSessionOver2Start?.Invoke();
                        }
                        break;
                }

                break;

            case State.CookOver:


                if (RestTimeOver())
                {
                    _OnCookOver2GameOver?.Invoke();
                }
                break;




            case State.GameOver:

                break;

            case State.Pause:

                break;

        }

        //}

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            //_OnSessionOver2GameOver?.Invoke();
            Debug.Log(_state);
        }
    }

    #region State Transform

    private void State2PrepareOpenDoor()
    {
        _state = State.PrepareOpenDoor;
    }
    private void State2OpeningDoor()
    {
        _state = State.OpeningDoor;
    }
    private void State2OpenDoorOver()
    {
        _state = State.OpenDoorOver;
    }
    private void State2PrepareSit()
    {
        _state = State.PrepareSit;
    }
    private void State2Sitting()
    {
        _state = State.Sitting;
    }
    private void State2SitOver()
    {
        _state = State.SitOver;
    }
    private void State2PrepareStand()
    {
        _state = State.PrepareStand;
    }

    private void State2Standing()
    {
        _state = State.Standing;
    }

    private void State2StandOver()
    {
        _state = State.StandOver;
    }


    private void State2Pause()
    {
        _state = State.Pause;
    }

    private void State2PrepareCook()
    {
        _state = State.PrepareCook;
    }

    private void State2Cooking()
    {
        _state = State.Cooking;
    }

    private void State2CookOver()
    {
        _state = State.CookOver;
    }

    private void State2GameOver()
    {
        _state = State.GameOver;
    }



    #endregion


    #region Sub State Transform

    private void SubState2Start()
    {
        _SubState = CookSubState.Start;
    }

    private void SubState2GetFood()
    {
        _SubState = CookSubState.GetingFood;
    }

    private void SubState2PutFood()
    {
        _SubState = CookSubState.PutingFood;
    }

    private void SubState2SessionOver()
    {
        _SubState = CookSubState.SessionOver;
    }


    #endregion



    #region Init Delegate Event
    // init delegate event
    private void InitDelegate()
    {
        InitPrepareOpenDoor2OpeningDoor();
        InitOpeningDoor2OpenDoorOver();
        InitBegin2PrepareOpenDoor();
        InitOnOpenDoorOver();
        InitOpenDoorOver2PrepareSit();
        InitPrepareSit2Sitting();
        InitSitting2SitOver();
        InitSitOver2PrepareStand();
        InitPrepareStand2Standing();
        InitStanding2StandOver();
        InitStandOver2PrepareCook();
        InitPrepareCook2Cooking();
        InitCooking2CookingOver();
        InitCookOver2GameOver();


        InitStart2GetingFood();
        InitGetingFood2PutingFood();
        InitPutingFood2SessionOver();
        InitPutingFood2Start();
        InitGetingFood();
        InitPutingFood();
    }

    private void InitBegin2PrepareOpenDoor()
    {
        _OnBegin2PrepareOpenDoor += Player2Bedroom;
        _OnBegin2PrepareOpenDoor += State2PrepareOpenDoor;
        _OnBegin2PrepareOpenDoor += ShowOpenDoorTips;   // show tips
        _OnBegin2PrepareOpenDoor += KinectDetectUI2GameUI;

    }


    private void InitPrepareOpenDoor2OpeningDoor()
    {
        _OnPrepareOpenDoor2OpeningDoor += State2OpeningDoor;

    }

    private void InitOpeningDoor2OpenDoorOver()
    {
        _OnOpeningDoor2OpenDoorOver += State2OpenDoorOver;
        _OnOpeningDoor2OpenDoorOver += ShowOpenDoorEncourageTips;
        _OnOpeningDoor2OpenDoorOver += UpdateOpenDoorSlider;
    }

    private void InitOpenDoorOver2PrepareSit()
    {
        _OnOpenDoorOver2PrepareSit += State2PrepareSit;
        _OnOpenDoorOver2PrepareSit += CameraBedroom2Toilet;
        _OnOpenDoorOver2PrepareSit += PlayerBedroom2Toilet;
        _OnOpenDoorOver2PrepareSit += ShowSitTips;
    }

    private void InitPrepareSit2Sitting()
    {
        _OnPrepareSit2Sitting += State2Sitting;
    }

    private void InitSitting2SitOver()
    {
        _OnSitting2SitOver += State2SitOver;
        _OnSitting2SitOver += ShowSitEncourageTips;
        _OnSitting2SitOver += UpdateSitSlider;
    }

    private void InitSitOver2PrepareStand()
    {
        _OnSitOver2PrepareStand += State2PrepareStand;
        _OnSitOver2PrepareStand += ShowStandTips;
    }

    private void InitPrepareStand2Standing() {
        _OnPrepareStand2Standing += State2Standing;
    }

    private void InitStanding2StandOver()
    {
        _OnStanding2StandOver += State2StandOver;
        _OnStanding2StandOver += ShowStandEncourageTips;
        _OnStanding2StandOver += UpdateStandSlider;
    }

    private void InitStandOver2PrepareCook()
    {
        _OnStandOver2PrepareCook += CameraToilet2Kitchen;
        _OnStandOver2PrepareCook += State2PrepareCook;
        _OnStandOver2PrepareCook += ShowCookTips;
    }

    private void InitPrepareCook2Cooking()
    {
        _OnPrepareCook2Cooking += State2Cooking;
    }

    private void InitCooking2CookingOver()
    {
        _OnCooking2CookOver += State2CookOver;
        _OnCooking2CookOver += ShowCookEncourageTips;
        _OnCooking2CookOver += UpdateCookSlider;
    }

    //private void InitCookingOver2GameOver()
    //{
    //    _OnCookOver2GameOver +
    //}

    private void InitStart2GetingFood()
    {
        _OnStart2GetFood += SubState2GetFood;
        _OnStart2GetFood += SelectFood;
        _OnStart2GetFood += ShowGetFoodTips;
    }

    private void InitGetingFood2PutingFood()
    {
        _OnGetingFood2PutingFood += SubState2PutFood;
        _OnGetingFood2PutingFood += ShowPutFoodTips;
    }

    private void InitPutingFood2SessionOver()
    {
        _OnPutingFood2SessionOver += SubState2SessionOver;
        _OnPutingFood2SessionOver += CloseHighlightFood;
    }

    private void InitPutingFood2Start(){
        _OnSessionOver2Start += SubState2Start;

    }


    private void InitOnOpenDoorOver()
    {
        _DoorHandle.OnOpenDoorOver += _OnOpeningDoor2OpenDoorOver.Invoke;
    }

    private void InitCookOver2GameOver()
    {
        _OnCookOver2GameOver += State2GameOver;
        _OnCookOver2GameOver += GameUI2GameOverUI;
        _OnCookOver2GameOver += GameOverWriteDatabase;
    }

    private void InitGetingFood()
    {
        _LeftHandHandle.OnGetFood += _OnGetingFood2PutingFood.Invoke;
        _LeftHandHandle.OnGetFood += SetLeftHand;
        _LeftHandHandle.OnGetFood += ResetFoodTag;

        _RightHandHandle.OnGetFood += _OnGetingFood2PutingFood.Invoke;
        _RightHandHandle.OnGetFood += SetRightHand;
        _RightHandHandle.OnGetFood += ResetFoodTag;
    }

    private void InitPutingFood()
    {
        _LeftHandHandle.OnPutFood += _OnPutingFood2SessionOver.Invoke;
        _LeftHandHandle.OnPutFood += ReleaseFood;

        _RightHandHandle.OnPutFood += _OnPutingFood2SessionOver.Invoke;
        _RightHandHandle.OnPutFood += ReleaseFood;
    }

    #endregion


    //game pause
    public void Pause()
    {
        _oldstate = _state;
        _state = State.Pause;
    }


    //game continue
    public void Continue()
    {
        _state = _oldstate;
    }


    private bool IsKinectDetected()
    {
        if (KinectManager.Instance.IsInitialized())
        {
            if (KinectManager.Instance.IsUserDetected())
            {
                return true;
            }
        }

        return false;
    }

    private bool IsDetected3s()
    {
        if (IsKinectDetected())
        {
            _KinectTimeCount += Time.deltaTime;
            //Debug.Log("_KinectTimeCount "+ _KinectTimeCount);
            UIHandle.SetKinectDetectProgress(_KinectTimeCount / KinectDetectTime);
            if (_KinectTimeCount < 3f)
            {
                return false;
            }
            else
            {
                _KinectTimeCount = 0f;
                return true;

            }
        }

        return false;
    }

    private void KinectDetectUI2GameUI()
    {
        UIHandle.KinectDetectUI2GameUI();
    }

    private void GameUI2GameOverUI()
    {
        UIHandle.GameUI2GameOverUI();
    }

    private void GameOverWriteDatabase()
    {
        //此处记录游戏结果
    }



    #region Switch camera

    // move player and camera from bedroom to toilet
    private void CameraBedroom2Toilet()
    {
        BedroomCamera.SetActive(false);
        ToiletCamera.SetActive(true);
    }

    private void CameraToilet2Kitchen()
    {
        ToiletCamera.SetActive(false);
        KitchenCamera.SetActive(true);
    }

    #endregion

    #region Switch Player

    private void Player2Bedroom()
    {
        HumanBedroom.transform.rotation = BedroomTransform.transform.rotation;
        HumanBedroom.transform.position = BedroomTransform.transform.position;
    }


    private void PlayerBedroom2Toilet()
    {

        Destroy(HumanBedroom);
        HumanToilet.transform.position = ToiletTransform.transform.position;
        HumanToilet.transform.rotation = ToiletTransform.transform.rotation;
    }

    private void PlayerToilet2Kitchen()
    {
        Destroy(HumanToilet);
        HumanBedroom.transform.position = KitchenTransform.transform.position;
        HumanBedroom.transform.rotation = KitchenTransform.transform.rotation;
    }

    #endregion


    // show opendoor tips
    private void ShowOpenDoorTips()
    {
        UIHandle.SetTips(_OpenDoorTips);
    }

    private void ShowOpenDoorEncourageTips()
    {
        StartCoroutine(UIHandle.ShowEncouragePicture(0.3f, RestTime - 0.3f, "用时" + PatientDataManager.instance.OpenDoorTimeCount.ToString("#.#") + "秒"));
        //SoundManager.instance.Play(CheerUpSE, SoundManager.SoundType.SE, PatientDataManager.instance.seVolume);

    }

    private void ShowSitEncourageTips()
    {
        StartCoroutine(UIHandle.ShowEncouragePicture(0.3f, RestTime - 0.3f, "用时" + PatientDataManager.instance.SitTimeCount.ToString("#.#") + "秒"));
    }

    private void ShowStandEncourageTips()
    {
        StartCoroutine(UIHandle.ShowEncouragePicture(0.3f, RestTime - 0.3f, "用时" + PatientDataManager.instance.StandTimeCount.ToString("#.#") + "秒"));
    }

    private void ShowCookEncourageTips()
    {
        StartCoroutine(UIHandle.ShowEncouragePicture(0.3f, RestTime - 0.3f, "用时" + PatientDataManager.instance.CookTimeCount.ToString("#.#") + "秒"));
    }

    private void ShowSitTips()
    {
        UIHandle.SetTips(_SitTips);

    }

    private void ShowStandTips()
    {
        UIHandle.SetTips(_StandTips);
    }

    private void ShowCookTips()
    {
        UIHandle.SetTips(_CookTips);
    }

    private void ShowGetFoodTips()
    {   
        // 当前食物的名字
        string FoodName = Foods[_NowFoodIndex].name;

        UIHandle.SetTips(_GetFoodTips.Replace("FOOD", FoodEng2CHN(FoodName)));
    }

    private void ShowPutFoodTips()
    {
        // 当前食物的名字
        string FoodName = Foods[_NowFoodIndex].name;

        UIHandle.SetTips(_PutFoodTips.Replace("FOOD", FoodEng2CHN(FoodName)));
    }

    private bool IsSitOver()
    {
        //Debug.Log(GestureSourceManager.instance.GetGestureConfidence(KinectGestures.Gestures.Sit));

        if((GestureSourceManager.instance.GetGestureConfidence(KinectGestures.Gestures.Sit) > 0.75 &&
            GestureSourceManager.instance.GetGestureConfidence(KinectGestures.Gestures.Sit) <0.75 &&
            Mathf.Abs(_ToiletPlayerAnimator.GetBoneTransform(HumanBodyBones.Hips).position.y - SitTransform.position.y) < 0.2) ||
                Mathf.Abs(_ToiletPlayerAnimator.GetBoneTransform(HumanBodyBones.Hips).position.y - SitTransform.position.y) < 0.1
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsStandOver()
    {
        if ((GestureSourceManager.instance.GetGestureConfidence(KinectGestures.Gestures.Stand) > 0.75 &&
            GestureSourceManager.instance.GetGestureConfidence(KinectGestures.Gestures.Sit) < 0.75 &&
            Mathf.Abs(_ToiletPlayerAnimator.GetBoneTransform(HumanBodyBones.Hips).position.y - SitTransform.position.y) > 0.4) ||
            Mathf.Abs(_ToiletPlayerAnimator.GetBoneTransform(HumanBodyBones.Hips).position.y - SitTransform.position.y) > 0.5
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsCutDownOver(float time)
    {

        _CutDownCount += Time.deltaTime;

        if (_CutDownCount > time + 0.4f)     //“开始”持续0.2s
        {
            _CutDownCount = 0f;
            UIHandle.ShowCutDown("");
            return true;
        }
        else if (_CutDownCount > time - 0.05f)
        {
            UIHandle.ShowCutDown("开始");
        }
        else
        {

            UIHandle.ShowCutDown((((int)(time - _CutDownCount)) + 1).ToString());

        }
        return false;
    }


    private bool RestTimeOver()
    {
        _RestTimeCount += Time.deltaTime;
        if (_RestTimeCount > RestTime)
        {
            _RestTimeCount = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    // 传入手部位置
    public void OpeningDoor()
    {
        _DoorHandle.Rotation(
            _BedroomPlayerAnimator.GetBoneTransform(HumanBodyBones.LeftHand).position,
            _BedroomPlayerAnimator.GetBoneTransform(HumanBodyBones.RightHand).position
            );

    }

    // 初始化
    public void InitGameObject()
    {
        _DoorHandle = BedroomDoor.GetComponent<DoorHandle>();

        FoodCollisionHandle[] tmp = HumanKitchen.GetComponentsInChildren<FoodCollisionHandle>(false);
        foreach (var v in tmp)
        {
            if(v.gameObject.name.Equals("LeftHand"))
            {
                _LeftHandHandle = v;
            }
            else
            {
                _RightHandHandle = v;
            }
        }


        _BedroomPlayerController = HumanBedroom.GetComponent<AvatarController>();
        _ToiletPlayerController = HumanToilet.GetComponent<AvatarController>();
        _KitchenPlayerController = HumanKitchen.GetComponent<AvatarController>();

        _BedroomPlayerAnimator = HumanBedroom.GetComponent<Animator>();
        _ToiletPlayerAnimator = HumanToilet.GetComponent<Animator>();
        _KitchenPlayerAnimator = HumanKitchen.GetComponent<Animator>();

    }

    // 更新总计时
    private void UpdateOpenDoorTimeCount()
    {
        float NewTimeCount = PatientDataManager.instance.OpenDoorTimeCount + Time.deltaTime;
        PatientDataManager.instance.SetOpenDoorTimeCount(NewTimeCount);

        string NewTimeCountString = PatientDataManager.instance.ADLTimeCount.ToString(".#");
        UIHandle.UpdateTimeCount(NewTimeCountString + "秒");
    }

    // 更新总计时
    private void UpdateSitTimeCount()
    {
        float NewTimeCount = PatientDataManager.instance.SitTimeCount + Time.deltaTime;
        PatientDataManager.instance.SetSitTimeCount(NewTimeCount);

        string NewTimeCountString = PatientDataManager.instance.ADLTimeCount.ToString(".#");
        UIHandle.UpdateTimeCount(NewTimeCountString + "秒");
    }

    // 更新总计时
    private void UpdateStandTimeCount()
    {
        float NewTimeCount = PatientDataManager.instance.StandTimeCount + Time.deltaTime;
        PatientDataManager.instance.SetStandTimeCount(NewTimeCount);

        string NewTimeCountString = PatientDataManager.instance.ADLTimeCount.ToString(".#");
        UIHandle.UpdateTimeCount(NewTimeCountString + "秒");
    }

    // 更新总计时
    private void UpdateCookTimeCount()
    {
        float NewTimeCount = PatientDataManager.instance.CookTimeCount + Time.deltaTime;
        PatientDataManager.instance.SetCookTimeCount(NewTimeCount);

        string NewTimeCountString = PatientDataManager.instance.ADLTimeCount.ToString(".#");
        UIHandle.UpdateTimeCount(NewTimeCountString + "秒");
    }

    // 更新进度条
    private void UpdateOpenDoorSlider()
    {
        UIHandle.SetTrainingProgress(1);

    }

    // 更新进度条
    private void UpdateSitSlider()
    {
        UIHandle.SetTrainingProgress(1);

    }

    // 更新进度条
    private void UpdateStandSlider()
    {
        UIHandle.SetTrainingProgress(2);
    }

    // 更新进度条
    private void UpdateCookSlider()
    {
        UIHandle.SetTrainingProgress(3);
    }

    // 激活目标食物
    private void SelectFood()
    {
        HighlightFood();
        ActiveFood();
    }

    // 高亮食物
    private void HighlightFood()
    {
        Highlighter h = Foods[_NowFoodIndex].GetComponent("Highlighter") as Highlighter;
        h.FlashingOn();
    }

    // 关闭高亮
    private void CloseHighlightFood()
    {
        Highlighter h = Foods[_NowFoodIndex].GetComponent("Highlighter") as Highlighter;
        h.FlashingOff();
    }

    // 激活碰撞体、重力
    private void ActiveFood()
    {
        Foods[_NowFoodIndex].GetComponent<Collider>().enabled = true;
        //Foods[_NowFoodIndex].GetComponent<Rigidbody>().useGravity = true;
    }

    // 标记左手
    private void SetLeftHand()
    {
        _LeftRight = true;
    }

    private void ResetFoodTag()
    {
        Foods[_NowFoodIndex].tag = "Finished";
    }

    // 标记右手
    private void SetRightHand()
    {
        _LeftRight = false;
        Foods[_NowFoodIndex].tag = "Finished";
    }


    // 将食物绑定在左手上
    private void Attach2LeftHand()
    {
        Foods[_NowFoodIndex].transform.position = LeftHandTransform.position;
    }

    // 将食物绑定在右手上
    private void Attach2RightHand()
    {
        Foods[_NowFoodIndex].transform.position = RightHandTransform.position;
    }

    // 放下食物
    private void ReleaseFood()
    {
        Foods[_NowFoodIndex].GetComponent<Rigidbody>().useGravity = true;
    }


    private string FoodEng2CHN(string Eng)
    {
        switch(Eng){
            case "Apple":
                return "苹果";
                break;
            case "Potato":
                return "土豆";
                break;
            case "Cucumber":
                return "黄瓜";
                break;
            default:
                return "苹果";
                break;
        }
    }

}