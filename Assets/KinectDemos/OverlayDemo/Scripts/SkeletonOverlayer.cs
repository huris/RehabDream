using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using Vectrosity;
using HighlightingSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;
//using Windows.Kinect;


public class SkeletonOverlayer : MonoBehaviour
{
    [Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
    public GUITexture backgroundImage;

    [Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
    public Camera foregroundCamera;

    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("Game object used to overlay the joints.")]
    public GameObject jointPrefab;
    public GameObject GreenPrefab;
    public GameObject RedPefab;

    [Tooltip("Line object used to overlay the bones.")]
    public LineRenderer linePrefab;
    //public float smoothFactor = 10f;

    //public GUIText debugText;

    private GameObject[] joints = null;
    private LineRenderer[] lines = null;

    private Quaternion initialRotation = Quaternion.identity;

    ////���������˵�
    //private int index = 0;

    //��¼��������ָ�������
    private Vector3 HandTipLeft;
    private Vector3 SpineMid;   // ������������
    private float FirstFistZ;   // ���߳�ʼȭͷ����
    private Vector2 LastPosition;
    private Vector2 NowPosition;
    private float LastNowDis;  // ������Ĳ�ֵ

    public Canvas canvas;

    // ��:0,����:1,��:2,����:3,��:4,����:5,��:6,����:7,�м�:8
    public GameObject Soccerball;

    public Camera camera;
    public LineRenderer line;

    public GameObject ReturnButton;
    public GameObject RestartButton;
    public GameObject TrackButton;
    public GameObject FinishedButton;
    public GameObject VoiceIntroductionButton;
    public GameObject ContinueButton;
    public GameObject UponButton;
    public GameObject DownButton;

    public Image Introduction;
    public float WaitTime;   // ˫����ȭ��ʱ��,��ʼ��Ϊ3��
    public Slider KinectDetectUIProgressSlider;  // ������

    public Image Buttons;   // ��������button�ı���
    //public GameObject FinishedButton;  // ��ɲ�����ť

    public Evaluation evaluation;   // �½�һ����������
    public List<Point> tempPoints;  // ��ʱ���ڻ�͹���ĵ㼯
    public ConvexHull convexHull;   // �½�һ��͹��

    private VectorLine ColorFistLine;   // ��ɫ������
    private VectorLine ConvexHullLine;   // ͹����

    //// ����EvaluationWidth
    //public float ShoulderLeftX; // ����xֵ
    //public float ShoulderRightX; // �Ҽ��xֵ

    // ����߶�EvaluationHeight
    public float SpineShoulderY; // ������Y
    public float SpineMidY; // ����Y

    public bool IsOver;     // �ж��Ƿ����

    public VectorLine ConvexHullArea;   // ��͹��͸�����
    public Color[] ConvexHullColors;    // ͹��������ɫ
    public Color ConvexHullLineColor;   // ͹����Ե��ɫ
    public Color ConvexHullAreaColor;   // ͹���ڲ���ɫ
    public bool ConvexHullIsDraw;   // ͹���������

    public long SoccerHighlightTime;    // �����������ʱ��

    // �������˳��
    public static int[] SoccerBallOrder = {1,2,8,3,7,4,6,5,0};
    public GameObject TargetSoccerBall;
    public int NowSoccerIndex;   // Ŀǰ��С������
    public static long ChangeBallWaitFrame = 1500;  // ����ȴ�1500֡
    public static long InterruptedBallWaitFrame = 250;  // �ж���ȴ�250
    public long LeftTouchFrame;   // ʣ�������֡��
    public Text LeftTimeText;
    public long CurrentScore;   //��ǰ�÷�
    public Text CurrentScoreText;   //��ǰ�÷�

    public VectorLine DirectionLine;


    public AudioSource audiosource;
    public AudioSource Background;

    public static SkeletonOverlayer _instance;

    public bool IsFirstGreen;

    public GameObject GravityCenterRadar;

    public bool IsMale = true;

    // mass of each segment / total mass
    private static float[] MaleMi =
        {
        0.0862f,    //Head
        0.1682f,    //upperbody
        0.2723f,    //lowerbody
        0.1419f,    //thigh
        0.0367f,    //shank
        0.0243f,    //arm
        0.0125f,    //forearm
        0.0064f,    //hand
        0.0148f     //foot
    };

    private static float[] FealeMi =
        {
        0.0820f,    //Head
        0.1635f,    //upperbody
        0.2748f,    //lowerbody
        0.1410f,    //thigh
        0.0443f,    //shank
        0.0266f,    //arm
        0.0114f,    //forearm
        0.0042f,    //hand
        0.0124f     //foot
    };

    // distance from gravity center to upperpoint / whole length
    private static float[] MaleLCS =
       {
        0.469f,    //Head
        0.536f,    //upperbody
        0.403f,    //lowerbody
        0.453f,    //thigh
        0.393f,    //shank
        0.478f,    //arm
        0.424f,    //forearm
        0.366f,    //hand
        0.438f     //foot
    };

    private static float[] FealeLCS =
       {
        0.473f,    //Head
        0.493f,    //upperbody
        0.446f,    //lowerbody
        0.442f,    //thigh
        0.425f,    //shank
        0.467f,    //arm
        0.453f,    //forearm
        0.349f,    //hand
        0.445f     //foot
    };

    public Vector3[] PersonJoints = new Vector3[25];

    private float _RecordTimeCount = 0;
    public static float RecordTime = 0.2f;           // record gravity,angles... each 0.2s 


    //public static SkeletonOverlayer instance = null;

    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        Debug.Log("@DataManager: Singleton created.");

    //    }
    //    else if (instance != this)
    //    {
    //        Destroy(gameObject);
    //    }

    //    DontDestroyOnLoad(this);
    //}

    void Start()
    {
        KinectManager manager = KinectManager.Instance;

        //new VectorLine("L1", new List<Vector3> {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f)}, 7.0f);


        if (manager && manager.IsInitialized())
        {
            int jointsCount = manager.GetJointCount();

            //if(jointPrefab)
            //{
            // array holding the skeleton joints
            joints = new GameObject[jointsCount];
            GreenPrefab = Resources.Load("Prefabs/RadarPrefabs/GreenBall") as GameObject;
            RedPefab = Resources.Load("Prefabs/RadarPrefabs/RedBall") as GameObject;

            for (int i = 0; i < joints.Length; i++)
            {
                if (i == 21 || i == 23)
                {
                    jointPrefab = RedPefab;
                }
                else
                {
                    jointPrefab = GreenPrefab;
                }

                joints[i] = Instantiate(jointPrefab) as GameObject;
                joints[i].transform.parent = transform;
                joints[i].name = ((KinectInterop.JointType)i).ToString();
                joints[i].SetActive(false);
            }
            //}

            // array holding the skeleton lines
            lines = new LineRenderer[jointsCount];

        }

        // always mirrored
        initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        if (!foregroundCamera)
        {
            // by default - the main camera
            foregroundCamera = Camera.main;
        }
    }

    void OnEnable()
    {
        //VectorLine.SetLine(Color.green, new Vector2(0, 0), new Vector2(222, 322));
        //PointHashSet = new HashSet<Point>();
        evaluation = new Evaluation();   // �½�һ����������
        // ��EvaluationID, ��0��ʼ
        evaluation.SetEvaluationID(DoctorDatabaseManager.instance.ReadMaxEvaluationID());

        //Points = new List<Point>();
        //index = 0;

        // always mirrored
        initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        //Soccerball = transform.Find("Soccerball").gameObject;
        //Soccerball.transform.position = new Vector3(0.1f, 0.8f, 46f);

        //line = gameObject.AddComponent<LineRenderer>();
        //line = GetComponent<LineRenderer>();
        Soccerball = null;

        UponButton.SetActive(false);  // ��ť��ͷ��ʧ
        DownButton.SetActive(false);  // ��ť��ͷ��ʧ

        //Introduction.transform.DOLocalMove(new Vector3(0f, 101.9f,0), 2.5f);
        //Buttons.transform.DOLocalMove(new Vector3(0f, -438.05f, 0), 2.5f);

        Introduction.transform.localPosition = new Vector3(0f, 101.9f, 0);
        Buttons.transform.localPosition = new Vector3(0f, -438.05f, 0);

        //Introduction.transform.DOLocalMove(new Vector3(-0.024902f, 979f, 0), 2.5f);

        WaitTime = 0f;

        // ����ȥ��
        if (ColorFistLine != null) VectorLine.Destroy(ref ColorFistLine);  // ��ȭ�켣ͼ
        if (ConvexHullLine != null) VectorLine.Destroy(ref ConvexHullLine);  // ͹��ͼ

        ReturnButton.SetActive(true);
        ReturnButton.transform.localPosition = new Vector3(-400f, -13.6f, 0f);
        VoiceIntroductionButton.SetActive(true);
        VoiceIntroductionButton.transform.localPosition = new Vector3(400f, -13.6f, 0f);

        RestartButton.SetActive(false);
        TrackButton.SetActive(false);
        FinishedButton.SetActive(false);
        ContinueButton.SetActive(false);// �տ�ʼ���ذ�ť����ʾ

        IsOver = false;

        // �������տ�ʼ����
        KinectDetectUIProgressSlider.gameObject.SetActive(false);

        SoccerHighlightTime = 100;   // ��������÷ָտ�ʼΪ100,����ʱÿ�γ���100

        NowSoccerIndex = -1;    // �տ�ʼ����-1������֮���1����
        TargetSoccerBall = null;

        LeftTouchFrame = ChangeBallWaitFrame;

        LeftTimeText.transform.parent.gameObject.SetActive(false);
        CurrentScoreText.transform.parent.gameObject.SetActive(false);
        GravityCenterRadar.SetActive(false);

        CurrentScore = 0;   // �տ�ʼ����Ϊ0

        DirectionLine = new VectorLine("ColorFistLine", new List<Vector2>(), 10.0f, LineType.Continuous, Joins.Weld);
        DirectionLine.smoothColor = false;   // ����ƽ����ɫ
        DirectionLine.smoothWidth = false;   // ����ƽ�����
        DirectionLine.color = Color.red;

        audiosource.playOnAwake = false;  //playOnAwake��Ϊfalseʱ��ͨ������play()��������
        Background.playOnAwake = false;
        Background.loop = true;

        AudioClip BackgroundClip = Resources.Load<AudioClip>("Sounds/Background1");

        Background.clip = BackgroundClip;

        _instance = this; //ͨ��Sound._instance.��������

        IsFirstGreen = false;

        if (DoctorDataManager.instance.doctor.patient.PatientSex == "��") IsMale = true;
        else IsMale = false;

        for(int i = 0; i < PersonJoints.Length; i++)
        {
            PersonJoints[i] = new Vector3(0f, 0f, 0f);
        }
    }

    void FixedUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (UponButton != null && UponButton.activeSelf == true)
            {
                UponButtonOnClick();
            }
        }

        KinectManager manager = KinectManager.Instance;
        //����»����ְ�ť��
        //Vector2 _pos1 = Vector2.one;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
        //            Input.mousePosition, canvas.worldCamera, out _pos1);

        ////print(_pos1 + "  " + Buttons.transform.localPosition);

        ////print(evaluation.Points.Count + " " + _pos1 + " " + Buttons.transform.localPosition);

        //if (evaluation.Points.Count > 0 && _pos1.y < -330f && Mathf.Abs(Buttons.transform.localPosition.y + 641.9f) < 20f)
        //{
        //    Buttons.transform.DOLocalMove(new Vector3(0f, -438.05f, 0), 0.5f);

        //    Buttons.transform.localPosition = new Vector3(0f, -438.05f, 0);
        //}

        //if (evaluation.Points.Count > 0 && _pos1.y > -330f && Mathf.Abs(Buttons.transform.localPosition.y + 438.05f) < 20f)
        //{
        //    Buttons.transform.DOLocalMove(new Vector3(0f, -641.9f, 0), 0.5f);

        //    //Buttons.transform.localPosition = new Vector3(0f, -641.9f, 0);
        //}

        //if(KinectDetectUIProgressSlider != null)
        //{
        //if (WaitTime == 0)
        //{
        //    KinectDetectUIProgressSlider.gameObject.SetActive(false);
        //}
        //else if (WaitTime > 0)
        //{
        //    KinectDetectUIProgressSlider.gameObject.SetActive(true);
        //}
        //}


        if (!IsOver && manager && manager.IsInitialized() && foregroundCamera)
        {

            if (WaitTime == 0)
            {
                KinectDetectUIProgressSlider.gameObject.SetActive(false);
            }
            else if (WaitTime > 0)
            {
                KinectDetectUIProgressSlider.gameObject.SetActive(true);
            }


            //backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
            if (backgroundImage && (backgroundImage.texture == null))
            {
                backgroundImage.texture = manager.GetUsersClrTex();
            }

            // get the background rectangle (use the portrait background, if available)
            Rect backgroundRect = foregroundCamera.pixelRect;
            PortraitBackground portraitBack = PortraitBackground.Instance;

            if (portraitBack && portraitBack.enabled)
            {
                backgroundRect = portraitBack.GetBackgroundRect();
            }

            // overlay all joints in the skeleton
            if (manager.IsUserDetected(playerIndex))
            {
                long userId = manager.GetUserIdByIndex(playerIndex);
                int jointsCount = manager.GetJointCount();

                for (int i = 0; i < jointsCount; i++)
                {
                    int joint = i;

                    if (manager.IsJointTracked(userId, joint))
                    {
                        Vector3 posJoint = manager.GetJointPosColorOverlay(userId, joint, foregroundCamera, backgroundRect);

                        //print(posJoint.x + " " + posJoint.y + " " + posJoint.z);
                        //Vector3 posJoint = manager.GetJointPosition(userId, joint);

                        if (joints != null)
                        {
                            // overlay the joint
                            if (posJoint != Vector3.zero)
                            {
                                PersonJoints[i] = posJoint;
                                //posJoint.z = 0f;  // ��z���Ϊ0,ƽ����ʾ

                                joints[i].SetActive(true);
                                joints[i].transform.position = posJoint;

                                //// ��ȡ���
                                //if(i == 4) { ShoulderLeftX = Kinect2UIPosition(posJoint).x; }
                                //if(i == 8) { ShoulderRightX = Kinect2UIPosition(posJoint).x; }
                                
                                // ��ȡ��߶�
                                if(i == 20) { SpineShoulderY = Kinect2UIPosition(posJoint).y; }

                                if (i == 21) { HandTipLeft = posJoint; }

                                if (i == 1) { SpineMid = posJoint; SpineMidY = Kinect2UIPosition(posJoint).y;}

                                // �������־���С��0.1f��ʱ����
                                if (i == 23 && (HandTipLeft - posJoint).magnitude < 0.13f)   // ���߿�ʼ��ȭ��
                                //if (i == 23 && GestureSourceManager.instance.CheckGesture(KinectGestures.Gestures.Bobath))
                                {
                                    if (WaitTime < 3.0f)
                                    {
                                        WaitTime += Time.deltaTime;
                                        KinectDetectUIProgressSlider.value = WaitTime / 3.0f;
                                    }
                                    else
                                    {
                                        //if (evaluation.Points == null) evaluation.Points = new List<Point>();
                                        if (evaluation.Points.Count == 0)
                                        {
                                            evaluation.SetEvaluationStartTime(DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                                            Introduction.transform.DOLocalMove(new Vector3(0f, 978f, 0), 0.5f);

                                            DownButtonOnClick();

                                            LeftTimeText.transform.parent.gameObject.SetActive(true);
                                            CurrentScoreText.transform.parent.gameObject.SetActive(true);
                                            GravityCenterRadar.SetActive(true);

                                            ButtonChange();

                                            LastPosition = Kinect2UIPosition(SpineMid);
                                            evaluation.Points.Add(new Point(LastPosition.x, LastPosition.y));
                                            WritePointInGame();

                                            // ��ʼ��������
                                            transform.GetChild(0).position = SpineMid;
                                            FirstFistZ = SpineMid.z;
                                            SoccerballReset();

                                            for (int z = 0; z < 9; z++)
                                            {
                                                transform.GetChild(z).gameObject.SetActive(false);
                                            }
                                            WaitPeopleTouchBall(ChangeBallWaitFrame);

                                            _instance.PlayMusicByName("RedMove");
                                            Background.Play();
                                        }
                                        else
                                        {

                                            NowPosition = Kinect2UIPosition(posJoint);

                                            LastNowDis = Point.PointsDistance(new Point(LastPosition), new Point(NowPosition));
                                            LastPosition = NowPosition;

                                            if (LastNowDis > 0.0f)
                                            {
                                                evaluation.Points.Add(new Point(NowPosition));
                                                WritePointInGame();
                                            }

                                        }
                                        // �ж��Ƿ�������
                                        //print(Soccerball.transform.position + " " + posJoint + " " + (posJoint - Soccerball.transform.position).magnitude);
                                        //if((posJoint - Soccerball.transform.position).magnitude < 0.1f)
                                        //{
                                        //	Soccerball.transform.position = new Vector3(1f, 1f, 46f);
                                        //}
                                        //for (int z = 0; z < 9; z++)
                                        //{
                                        //    transform.GetChild(z).gameObject.SetActive(true);
                                        //}
                                        LeftTimeText.text = (LeftTouchFrame / 50).ToString();
                                        CurrentScoreText.text = CurrentScore.ToString();

                                        RayCastResult(posJoint);    // ���������䵽�ĸ�����
                                                                    //DrawRayLine(camera.transform.position, posJoint);
                                    }

                                }
                                else if (i == 23 && (HandTipLeft - posJoint).magnitude > 1.0f)
                                {
                                    SoccerHighlightTime = 100;  // ���100

                                    if(evaluation.Points.Count > 0 && NowSoccerIndex == 8)
                                    {
                                        if (Soccerball != null && Soccerball.GetComponent<Highlighter>() != null)
                                        {
                                            Soccerball.GetComponent<Highlighter>().ConstantOff();
                                        }

                                        //print(Buttons.transform.localPosition);

                                        UponButtonOnClick();

                                        // ������ͼ
                                        evaluation.SetEvaluationEndTime(DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));


                                        //_GraivtyCenter.ToString().Replace("(", "").Replace(")", ""),

                                        // ��¼����ľ���
                                        evaluation.soccerDistance.UponSoccerDistance = (transform.GetChild(1).position - transform.GetChild(0).position).magnitude;
                                        evaluation.soccerDistance.UponRightSoccerDistance = (transform.GetChild(2).position - transform.GetChild(0).position).magnitude;
                                        evaluation.soccerDistance.RightSoccerDistance = (transform.GetChild(3).position - transform.GetChild(0).position).magnitude;
                                        evaluation.soccerDistance.DownRightSoccerDistance = (transform.GetChild(4).position - transform.GetChild(0).position).magnitude;
                                        evaluation.soccerDistance.DownSoccerDistance = (transform.GetChild(5).position - transform.GetChild(0).position).magnitude;
                                        evaluation.soccerDistance.DownLeftSoccerDistance = (transform.GetChild(6).position - transform.GetChild(0).position).magnitude;
                                        evaluation.soccerDistance.LeftSoccerDistance = (transform.GetChild(7).position - transform.GetChild(0).position).magnitude;
                                        evaluation.soccerDistance.UponLeftSoccerDistance = (transform.GetChild(8).position - transform.GetChild(0).position).magnitude;

                                        IsOver = true;

                                        _instance.PlayMusicByName("EvaluationOver");
                                        Background.Stop();

                                        //for (int z = 0; z < 9; z++)
                                        //{
                                        //    transform.GetChild(z).gameObject.SetActive(false);
                                        //}
                                    }
                                }

                                Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
                                rotJoint = initialRotation * rotJoint;
                                joints[i].transform.rotation = rotJoint;
                            }
                            else
                            {
                                joints[i].SetActive(false);
                            }
                        }

                        if (lines[i] == null && linePrefab != null)
                        {
                            lines[i] = Instantiate(linePrefab) as LineRenderer;
                            lines[i].transform.parent = transform;
                            lines[i].gameObject.SetActive(false);
                        }

                        if (lines[i] != null)
                        {
                            // overlay the line to the parent joint
                            int jointParent = (int)manager.GetParentJoint((KinectInterop.JointType)joint);
                            Vector3 posParent = manager.GetJointPosColorOverlay(userId, jointParent, foregroundCamera, backgroundRect);

                            if (posJoint != Vector3.zero && posParent != Vector3.zero)
                            {
                                lines[i].gameObject.SetActive(true);

                                ////lines[i].SetVertexCount(2);
                                //lines[i].SetPosition(0, posParent);
                                //lines[i].SetPosition(1, posJoint);

                                //��������z���Ϊ0,ƽ����ʾ
                                lines[i].positionCount = 2;
                                lines[i].SetPosition(0, new Vector3(posParent.x, posParent.y, 0));
                                lines[i].SetPosition(1, new Vector3(posJoint.x, posJoint.y, 0));
                            }
                            else
                            {
                                lines[i].gameObject.SetActive(false);
                            }
                        }

                    }
                    else
                    {
                        if (joints != null)
                        {
                            joints[i].SetActive(false);
                        }

                        if (lines[i] != null)
                        {
                            lines[i].gameObject.SetActive(false);
                        }
                    }

                }

                // ���¼��
                //ShoulderRightX = Math.Abs(ShoulderRightX - ShoulderLeftX);
                //if (ShoulderRightX > evaluation.EvaluationWidth)
                //{
                //    evaluation.SetEvaluationWidth(ShoulderRightX);
                //}

                // ������߶�
                SpineShoulderY = Math.Abs(SpineShoulderY - SpineMidY);
                if(SpineShoulderY > evaluation.EvaluationHeight)
                {
                    evaluation.SetEvaluationHeight(SpineShoulderY);
                }

                if (RecordTimeOver())
                {

                }
            }
        }
    }


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

    // �����ڶη�������������
    //Calculate Gravity Center using Avatar
    public Vector3 CalculateGravityCenter(bool IsMale)
    {

        Vector3 HeadTop = PersonJoints[3] + (PersonJoints[3] - PersonJoints[2]);
        Vector3 HeadDown = PersonJoints[2];

        Vector3 UpperTorsoTop = PersonJoints[2];
        Vector3 UpperTorsoDown = PersonJoints[1];

        Vector3 LowerTorsoTop = PersonJoints[1];
        Vector3 LowerTorsoDown = (PersonJoints[12] + PersonJoints[16]) / 2;

        Vector3 LeftUpperArmTop = PersonJoints[4];
        Vector3 LeftUpperArmDown = PersonJoints[5];

        Vector3 RightUpperArmTop = PersonJoints[8];
        Vector3 RightUpperArmDown = PersonJoints[9];

        Vector3 LeftForearmTop = PersonJoints[5];
        Vector3 LeftForearmDown = PersonJoints[6];

        Vector3 RightForearmTop = PersonJoints[9];
        Vector3 RightForearmDown = PersonJoints[10];

        Vector3 LeftHandTop = PersonJoints[6];
        Vector3 LeftHandDown = PersonJoints[21];

        Vector3 RightHandTop = PersonJoints[10];
        Vector3 RightHandDown = PersonJoints[23];

        Vector3 LeftLegTop = PersonJoints[12];
        Vector3 LeftLegDown = PersonJoints[13];

        Vector3 RightLegTop = PersonJoints[16];
        Vector3 RightLegDown = PersonJoints[17];

        Vector3 LeftShankTop = PersonJoints[13];
        Vector3 LeftShankDown = PersonJoints[14];

        Vector3 RightShankTop = PersonJoints[17];
        Vector3 RightShankDown = PersonJoints[18];

        Vector3 LeftFootTop = PersonJoints[14];
        Vector3 LeftFootDown = PersonJoints[15] + (PersonJoints[15] - PersonJoints[14]);

        Vector3 RightFootTop = PersonJoints[18];
        Vector3 RightFootDown = PersonJoints[19] + (PersonJoints[19] - PersonJoints[18]);

        float[] Mi;
        float[] LCS;
        Vector3 com = Vector3.zero;
        if (IsMale)
        {
            Mi = MaleMi;
            LCS = MaleLCS;

        }
        else
        {
            Mi = FealeMi;
            LCS = FealeLCS;
        }

        com = Center(HeadTop, HeadDown, LCS[0]) * Mi[0]  //head
            + Center(UpperTorsoTop, UpperTorsoDown, LCS[1]) * Mi[1]   //upper body
            + Center(LowerTorsoTop, LowerTorsoDown, LCS[2]) * Mi[2]   //Lower body
            + Center(LeftLegTop, LeftLegDown, LCS[3]) * Mi[3]    //left thigh
            + Center(RightLegTop, RightLegDown, LCS[3]) * Mi[3]    //right thigh
            + Center(LeftShankTop, LeftShankDown, LCS[4]) * Mi[4]    //left shank
            + Center(RightShankTop, RightShankDown, LCS[4]) * Mi[4]    //right shank
            + Center(LeftUpperArmTop, LeftUpperArmDown, LCS[5]) * Mi[5]    //left arm
            + Center(RightUpperArmTop, RightUpperArmDown, LCS[5]) * Mi[5]    //right arm
            + Center(LeftForearmTop, LeftForearmDown, LCS[6]) * Mi[6]    //left forearm
            + Center(RightForearmTop, RightForearmDown, LCS[6]) * Mi[6]    //right aforerm
            + Center(LeftHandTop, LeftHandDown, LCS[7]) * Mi[7]    //left hand
            + Center(RightHandTop, RightHandDown, LCS[7]) * Mi[7]    //right hand
            + Center(LeftFootTop, LeftFootDown, LCS[8]) * Mi[8]    //left foot
            + Center(RightFootTop, RightFootDown, LCS[8]) * Mi[8];   //right foot
        
        return com;
    }

    // return gravity center of segment
    private Vector3 Center(Vector3 up, Vector3 low, float LCS)
    {
        return up - LCS * (up - low);
    }

    public void WaitPeopleTouchBall(long WaitFrame)
    {
        if (WaitFrame == ChangeBallWaitFrame && NowSoccerIndex < 8)
        {
            if (TargetSoccerBall != null) TargetSoccerBall.SetActive(false);

            TargetSoccerBall = transform.GetChild(SoccerBallOrder[++NowSoccerIndex]).gameObject;
            TargetSoccerBall.SetActive(true);

            LeftTouchFrame = ChangeBallWaitFrame;
        }
        else if (WaitFrame == InterruptedBallWaitFrame)
        {
            LeftTouchFrame = InterruptedBallWaitFrame;
        }
    }
    

    public void ButtonChange()
    {
        ReturnButton.SetActive(true);
        ReturnButton.transform.localPosition = new Vector3(-700f, -13.6f, 0f);
        RestartButton.SetActive(true);
        RestartButton.transform.localPosition = new Vector3(-350f, -13.6f, 0f);

        ContinueButton.SetActive(true);
        ContinueButton.transform.localPosition = new Vector3(0f, -13.6f, 0f);

        TrackButton.SetActive(true);
        TrackButton.transform.localPosition = new Vector3(350f, -13.6f, 0f);
        FinishedButton.SetActive(true);   // �տ�ʼ���ذ�ť����ʾ
        FinishedButton.transform.localPosition = new Vector3(700f, -13.6f, 0f);

        VoiceIntroductionButton.SetActive(false);
    }

    public void ContinueButtonOnClick()
    {
        IsOver = false;
        DownButtonOnClick();
    }

    public void UponButtonOnClick()
    {
        UponButton.SetActive(false);
        DownButton.SetActive(true);

        Buttons.transform.DOLocalMove(new Vector3(0f, -438.05f, 0), 0.5f);
    }

    public void DownButtonOnClick()
    {
        UponButton.SetActive(true);
        DownButton.SetActive(false);

        Buttons.transform.DOLocalMove(new Vector3(0f, -620f, 0), 0.5f);
    }


    // use Thread to write database
    public void WritePointInGame()
    {
        //print(evaluation.Points[evaluation.Points.Count - 1].x);
        WritePointDataThread Thread = new WritePointDataThread(
           evaluation.EvaluationID,
           evaluation.Points[evaluation.Points.Count-1]);

        Thread.StartThread();      
    }

    public void SoccerballReset()
    {
        List<Vector3> PositionOffset = new List<Vector3> {		// 8�������ƫ����
            new Vector3(0f, 0.5f, 0f),
            new Vector3(-0.32f, 0.32f, 0f),
            new Vector3(-0.5f, 0f, 0f),
            new Vector3(-0.32f, -0.32f, 0f),
            new Vector3(0f, -0.5f, 0f),
            new Vector3(0.32f, -0.32f, 0f),
            new Vector3(0.5f, 0f, 0f),
            new Vector3(0.32f, 0.32f, 0f),
        };
        for (int i = 1; i <= 8; i++)
        {
            transform.GetChild(i).transform.position = PositionOffset[i - 1] + transform.GetChild(0).position;
        }
    }

    //public LineRenderer lineRenderer = new LineRenderer();

    public void RayCastResult(Vector3 FistPos)
    {

        //print(FistPos+" "+ SoccerballPos);
        //Ray ray = new Ray(FistPos, FistPos + Vector3.forward);

        Ray ray = new Ray(camera.transform.position, FistPos - camera.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (Soccerball == null)
            {
                SoccerHighlightTime = 100;  // ���100

                Soccerball = hit.collider.gameObject;
                if (Soccerball.GetComponent<Highlighter>() != null)
                {
                    if (IsFirstGreen == false)
                    {
                        _instance.PlayMusicByName("GreenMove");
                        IsFirstGreen = true;
                    }
                    Soccerball.GetComponent<Highlighter>().ConstantOn(Color.green);
                }
            }
            else if (Soccerball != hit.collider.gameObject)
            {
                SoccerHighlightTime = 100;  // ���100
                if (Soccerball.GetComponent<Highlighter>() != null)
                {
                    Soccerball.GetComponent<Highlighter>().ConstantOff();
                }
                Soccerball = hit.collider.gameObject;
                if (Soccerball.GetComponent<Highlighter>() != null)
                {
                    Soccerball.GetComponent<Highlighter>().ConstantOn(Color.green);
                }
            }
            else if (Soccerball.GetComponent<Highlighter>() != null)
            {
                SoccerHighlightTime++;  // ÿ�μ�1

                Soccerball.GetComponent<Highlighter>().ConstantOn(Color.green);

                WaitPeopleTouchBall(InterruptedBallWaitFrame);

                DirectionLine.points2.Clear();
                DirectionLine.Draw();
            }

            if (TargetSoccerBall != Soccerball)
            {
                LeftTouchFrame--;

                long IsChangeHighlighter = LeftTouchFrame % 50;
                //print(IsChangeHighlighter);
                if (IsChangeHighlighter == 0)
                {
                    TargetSoccerBall.GetComponent<Highlighter>().ConstantOn(Color.red);
                }
                else if (IsChangeHighlighter == 25)
                {
                    TargetSoccerBall.GetComponent<Highlighter>().ConstantOff();
                }

                if (LeftTouchFrame == 0)
                {
                    WaitPeopleTouchBall(ChangeBallWaitFrame);
                }
                DirectionLine.color = Color.red;
                DrawDirectionLine(Kinect2UIPosition(FistPos), Kinect2UIPosition(TargetSoccerBall.transform.position));
            }

            SoccerballMove(FistPos);
        }
        else
        {
            SoccerHighlightTime = 100;  // ���100

            if (Soccerball != null && Soccerball.GetComponent<Highlighter>() != null)
            {
                Soccerball.GetComponent<Highlighter>().ConstantOff();
            }
        }
    }

    public void DrawDirectionLine(Vector2 origin, Vector2 destination)
    {
        DirectionLine.points2.Clear();

        Vector2 Offset = (destination - origin)/5;

        destination -= Offset;
        origin += Offset;

        double slopy = 0, cosy = 0, siny = 0;
        double par = 50.0;  //length of Arrow (>)
        slopy = Math.Atan2((double)(origin.y - destination.y), (double)(origin.x - destination.x));
        cosy = Math.Cos(slopy);
        siny = Math.Sin(slopy);

        DirectionLine.points2.Add(origin);
        DirectionLine.points2.Add(destination);

        // ��������Ǽ�ͷ�����˵������
        DirectionLine.points2.Add(new Vector2(destination.x + (int)(par * cosy - (par / 2.0 * siny)),
                                              destination.y + (int)(par * siny + (par / 2.0 * cosy))));
        
        DirectionLine.points2.Add(destination);

        DirectionLine.points2.Add(new Vector2(destination.x + (int)(par * cosy + par / 2.0 * siny),
                                              destination.y - (int)(par / 2.0 * cosy - par * siny)));

        DirectionLine.Draw();
    }

    public void ChangeCurrentScore()
    {
        CurrentScore += SoccerHighlightTime / 100;
    }


    public void SoccerballMove(Vector3 FistPos)  // �����ƶ�
    {
        float sdir = 0.001f, ddir = 0.0007f;  // ����2��
        float ScaleOffset = 2f;
        float SpeedIncrease = 1.0f * SoccerHighlightTime / 100;

        // ��:2,����:3,��:4,����:5,��:6,����:7,��:0,����:1,
        if (Soccerball.name == "Soccerball0")   
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos)-Kinect2UIPosition(SpineMid)).magnitude;
           
            while(TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.y += sdir;
                evaluation.soccerDistance.UponSoccerTime++;
                evaluation.soccerDistance.UponSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.y += sdir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.UponSoccerTime++;
            evaluation.soccerDistance.UponSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
            TempPos = Kinect2UIPosition(TempPos);
            DirectionLine.color = Color.green;
            DrawDirectionLine(TempPos, new Vector3(TempPos.x, Math.Min(TempPos.y + 250, 1079), TempPos.z));
            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball1")
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos) - Kinect2UIPosition(SpineMid)).magnitude;

            while (TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.x -= ddir;
                TempPos.y += ddir;
                evaluation.soccerDistance.UponRightSoccerTime++;
                evaluation.soccerDistance.UponRightSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.x -= ddir * SpeedIncrease;
            TempPos.y += ddir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.UponRightSoccerTime++;
            evaluation.soccerDistance.UponRightSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ

            Vector3 origin = Kinect2UIPosition(TempPos);
            Vector3 destination = origin;
            destination.x += 176;
            destination.y += 176;

            if (destination.x < 1920 && destination.y >= 1080)
            {
                destination.x = destination.x - (destination.y - 1080 + 1);
                destination.y = 1079;
            }
            else if (destination.x >= 1920 && destination.y >= 1080)
            {
                float maxdiff = Math.Max(destination.x - 1920 + 1, destination.y - 1080 + 1);
                destination.x -= maxdiff;
                destination.y -= maxdiff;
            }
            else if (destination.x >= 1920 && destination.y < 1080)
            {
                destination.y = destination.y - (destination.x - 1920 + 1);
                destination.x = 1919;
            }

            DirectionLine.color = Color.green;
            DrawDirectionLine(origin, destination);

            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball2")
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos) - Kinect2UIPosition(SpineMid)).magnitude;

            while (TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.x -= ddir;
                evaluation.soccerDistance.RightSoccerTime++;
                evaluation.soccerDistance.RightSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.x -= sdir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.RightSoccerTime++;
            evaluation.soccerDistance.RightSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ

            TempPos = Kinect2UIPosition(TempPos);
            DirectionLine.color = Color.green;
            DrawDirectionLine(TempPos, new Vector3(Math.Min(TempPos.x + 250, 1919), TempPos.y, TempPos.z));

            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball3")
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos) - Kinect2UIPosition(SpineMid)).magnitude;

            while (TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.x -= ddir;
                TempPos.y -= ddir;
                evaluation.soccerDistance.DownRightSoccerTime++;
                evaluation.soccerDistance.DownRightSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.x -= ddir * SpeedIncrease;
            TempPos.y -= ddir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.DownRightSoccerTime++;
            evaluation.soccerDistance.DownRightSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ

            Vector3 origin = Kinect2UIPosition(TempPos);
            Vector3 destination = origin;
            destination.x += 176;
            destination.y -= 176;
            if (destination.y < 0 && destination.x < 1920)
            {
                destination.x += destination.y;
                destination.y = 0;
            }
            else if (destination.y < 0 && destination.x >= 1920)
            {
                float maxdiff = Math.Max(-destination.y, destination.x - 1920 + 1);
                destination.y += maxdiff;
                destination.x -= maxdiff;
            }
            else if (destination.y >= 0 && destination.x >= 1920)
            {
                destination.y = destination.y + (destination.x - 1920 + 1);
                destination.x = 1919;
            }

            DirectionLine.color = Color.green;
            DrawDirectionLine(origin, destination);

            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball4")
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos) - Kinect2UIPosition(SpineMid)).magnitude;

            while (TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.y -= ddir;
                evaluation.soccerDistance.DownSoccerTime++;
                evaluation.soccerDistance.DownSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.y -= sdir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.DownSoccerTime++;
            evaluation.soccerDistance.DownSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ

            TempPos = Kinect2UIPosition(TempPos);
            DirectionLine.color = Color.green;
            DrawDirectionLine(TempPos, new Vector3(TempPos.x, Math.Max(TempPos.y - 250, 0), TempPos.z));

            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball5")
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos) - Kinect2UIPosition(SpineMid)).magnitude;

            while (TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.x += ddir;
                TempPos.y -= ddir;
                evaluation.soccerDistance.DownLeftSoccerTime++;
                evaluation.soccerDistance.DownLeftSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.x += ddir * SpeedIncrease;
            TempPos.y -= ddir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.DownLeftSoccerTime++;
            evaluation.soccerDistance.DownLeftSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ

            Vector3 origin = Kinect2UIPosition(TempPos);
            Vector3 destination = origin;
            destination.x -= 176;
            destination.y -= 176;
            if (destination.x < 0 && destination.y >= 0)
            {
                destination.y -= destination.x;
                destination.x = 0;
            }
            else if (destination.x < 0 && destination.y < 0)
            {
                float maxdiff = Math.Max(-destination.x, -destination.y);
                destination.x += maxdiff;
                destination.y += maxdiff;
            }
            else if (destination.x >= 0 && destination.y < 0)
            {
                destination.x -= destination.y;
                destination.y = 0;
            }

            DirectionLine.color = Color.green;
            DrawDirectionLine(origin, destination);

            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball6")
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos) - Kinect2UIPosition(SpineMid)).magnitude;

            while (TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.x += ddir;
                evaluation.soccerDistance.LeftSoccerTime++;
                evaluation.soccerDistance.LeftSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.x += sdir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.LeftSoccerTime++;
            evaluation.soccerDistance.LeftSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ

            TempPos = Kinect2UIPosition(TempPos);
            DirectionLine.color = Color.green;
            DrawDirectionLine(TempPos, new Vector3(Math.Max(TempPos.x - 250, 0), TempPos.y, TempPos.z));

            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball7")
        {
            Vector3 TempPos = Soccerball.transform.position;

            float TempDiff = (Kinect2UIPosition(FistPos) - Kinect2UIPosition(SpineMid)).magnitude;

            while (TempDiff > (Kinect2UIPosition(TempPos) - Kinect2UIPosition(SpineMid)).magnitude)
            {
                TempPos.x += ddir;
                TempPos.y += ddir;
                evaluation.soccerDistance.UponLeftSoccerTime++;
                evaluation.soccerDistance.UponLeftSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                ChangeCurrentScore();
            }

            TempPos.x += ddir * SpeedIncrease;
            TempPos.y += ddir * SpeedIncrease;
            Soccerball.transform.position = TempPos;
            evaluation.soccerDistance.UponLeftSoccerTime++;
            evaluation.soccerDistance.UponLeftSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ

            Vector3 origin = Kinect2UIPosition(TempPos);
            Vector3 destination = origin;
            destination.x -= 176;
            destination.y += 176;
            if (destination.x < 0 && destination.y < 1080)
            {
                destination.y += destination.x;
                destination.x = 0;
            }
            else if (destination.x < 0 && destination.y >= 1080)
            {
                float maxdiff = Math.Max(-destination.x, destination.y - 1080 + 1);
                destination.x += maxdiff;
                destination.y -= maxdiff;
            }
            else if (destination.x >= 0 && destination.y >= 1080)
            {
                destination.x = destination.x + (destination.y - 1080 + 1);
                destination.y = 1079;
            }

            DirectionLine.color = Color.green;
            DrawDirectionLine(origin, destination);

            ChangeCurrentScore();
        }
        else if (Soccerball.name == "Soccerball")
        {
            Vector3 TempPos = Soccerball.transform.localScale;

            //// ���������Сֵ
            //if (evaluation.soccerDistance.CenterSoccerMin > TempPos.x)
            //{
            //    evaluation.soccerDistance.CenterSoccerMin = TempPos.x;
            //}

            //if(evaluation.soccerDistance.CenterSoccerMax < TempPos.x)
            //{
            //    evaluation.soccerDistance.CenterSoccerMax = TempPos.x;
            //}

            float ZOffset = FirstFistZ - FistPos.z;
            
            if(TempPos.x == 3.5f) 
            {
                Soccerball.GetComponent<Highlighter>().ConstantOn(Color.red);
            }
            else if(TempPos.x > 3.5f) 
            {
                Soccerball.GetComponent<Highlighter>().ConstantOn(Color.green);

                float tempDis = (FistPos - transform.GetChild(0).position).magnitude;                
                if(tempDis > evaluation.soccerDistance.FrontSoccerDistance)
                {
                    evaluation.soccerDistance.FrontSoccerDistance = tempDis;
                    evaluation.soccerDistance.FrontSoccerTime++;
                    evaluation.soccerDistance.FrontSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                    ChangeCurrentScore();
                }
            }
            else if(TempPos.x < 3.5f) 
            { 
                Soccerball.GetComponent<Highlighter>().ConstantOn(Color.yellow);

                float tempDis = (FistPos - transform.GetChild(0).position).magnitude;
                if (tempDis > evaluation.soccerDistance.BehindSoccerDistance)
                {
                    evaluation.soccerDistance.BehindSoccerDistance = tempDis;
                    evaluation.soccerDistance.BehindSoccerTime++;
                    evaluation.soccerDistance.BehindSoccerScore += SoccerHighlightTime / 100;  // ÿ�μӳ���100��ֵ
                    ChangeCurrentScore();
                }
            }
            TempPos.x = TempPos.y = TempPos.z = TempPos.x + ZOffset * ScaleOffset;
            //print(FistPos + " " + FirstFistZ);
            FirstFistZ = FistPos.z;
            Soccerball.transform.localScale = TempPos;
        }
    }

    public void DrawRayLine(Vector3 pos1, Vector3 pos2) // ���ߺ���
    {
        ////���LineRenderer���
        ////���ò���
        //FistLine.material = new Material(Resources.Load("Prefabs/RadarPrefabs/VirtualLineYellow") as Material);

        //////������ɫ
        ////FistLine.startColor = Color.yellow;
        ////FistLine.endColor = Color.red;
        ////���ÿ��
        //FistLine.startWidth = 0.01f;
        //FistLine.endWidth = 0.02f;

        line.positionCount = 2; //�����ø��߶��ɼ��������

        // �����߶ε������ɫ���յ���ɫ
        line.startColor = Color.blue;
        line.endColor = Color.red;
        // �����߶�����Ⱥ��յ���
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.SetPosition(0, pos1);
        line.SetPosition(1, pos2);

    }

    public void EvaluationReStart()
    {
        OnEnable();
    }

    public void EvaluationTrack() // ��͹������ʾ�켣
    {

        if (evaluation.Points != null && evaluation.Points.Count > 0)
        {
            for (int z = 0; z < 9; z++)
            {
                transform.GetChild(z).gameObject.SetActive(true);
            }

            StartCoroutine(DrawColorFistLine());
        }

    }

    IEnumerator DrawColorFistLine()
    {
        DownButtonOnClick();

        tempPoints = new List<Point>();

        foreach (var point in evaluation.Points)
        {
            tempPoints.Add(new Point(point.x, point.y));
        }

        //evaluation.Points.ForEach(i => tempPoints.Add(i));  // �����еĵ㸴�Ƹ�temppoints���ڻ�͹��ͼ

        ColorFistLine = new VectorLine("ColorFistLine", new List<Vector2>(), 7.0f, LineType.Continuous, Joins.Weld);
        ColorFistLine.smoothColor = false;   // ����ƽ����ɫ
        ColorFistLine.smoothWidth = false;   // ����ƽ�����
        ColorFistLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

        int DeltaBase = 0, DeltaColorR = 0, DeltaColorG = 0;

        ColorFistLine.points2.Add(new Vector2(tempPoints[0].x, tempPoints[0].y));

        for (int i = 1; i < tempPoints.Count; i++)
        {
            //ColorFistLine.Draw();

            ColorFistLine.points2.Add(new Vector2(tempPoints[i].x, tempPoints[i].y));

            DeltaBase = (int)(Point.PointsDistance(tempPoints[i - 1], tempPoints[i]) * 7);

            if (DeltaBase <= 0) { DeltaColorR = 0; DeltaColorG = 0; }
            else if (DeltaBase > 0 && DeltaBase <= 255) { DeltaColorR = DeltaBase; DeltaColorG = 0; }
            else if (DeltaBase > 255 && DeltaBase <= 510) { DeltaColorR = 255; DeltaColorG = DeltaBase - 255; }
            else if (DeltaBase > 510) { DeltaColorR = 255; DeltaColorG = 255; }

            ColorFistLine.SetColor(new Color32((Byte)DeltaColorR, (Byte)(255 - DeltaColorG), 0, (Byte)255), i-1);
            //ColorFistLine.SetWidth(7.0f * LastNowDis / 20, Points.Count - 2);
            ColorFistLine.Draw();
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(DrawConvexHull());
    }
    IEnumerator DrawConvexHull()
    {
        convexHull = new ConvexHull(tempPoints);

        // ��͹��Ȧ
        ConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 8.0f, LineType.Continuous, Joins.Weld);
        ConvexHullLine.smoothColor = false;   // ����ƽ����ɫ
        ConvexHullLine.smoothWidth = false;   // ����ƽ�����
        ConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

        ConvexHullLineColor = new Color32((Byte)0, (Byte)191, (Byte)255, (Byte)255);

        ConvexHullLine.color = ConvexHullLineColor;

        int MinX = Mathf.FloorToInt(convexHull.ConvexHullSet[0].x), MaxX = Mathf.CeilToInt(convexHull.ConvexHullSet[0].x);   // ͹���������СX
        int MinY = Mathf.FloorToInt(convexHull.ConvexHullSet[0].y), MaxY = Mathf.CeilToInt(convexHull.ConvexHullSet[0].y);   // ͹���������СY

        // �Ȱѳ�ʼ����뻭ͼ����
        ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[0].x, convexHull.ConvexHullSet[0].y));
        convexHull.ConvexHullArea = 0f;   // ��͹�������ʼΪ0

        for (int i = 1; i < convexHull.ConvexHullNum; i++)
        {
            ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[i].x, convexHull.ConvexHullSet[i].y));
            //ConvexHullLine.SetColor(ConvexHullLineColor);  // ������ɫ

            if (i < convexHull.ConvexHullNum - 1)
            {
                convexHull.ConvexHullArea += Math.Abs(ConvexHull.isLeft(convexHull.ConvexHullSet[0], convexHull.ConvexHullSet[i], convexHull.ConvexHullSet[i + 1]));
            }

            if (MinX > Mathf.FloorToInt(convexHull.ConvexHullSet[i].x)) MinX = Mathf.FloorToInt(convexHull.ConvexHullSet[i].x);
            if (MaxX < Mathf.CeilToInt(convexHull.ConvexHullSet[i].x)) MaxX = Mathf.CeilToInt(convexHull.ConvexHullSet[i].x);
            if (MinY > Mathf.FloorToInt(convexHull.ConvexHullSet[i].y)) MinY = Mathf.FloorToInt(convexHull.ConvexHullSet[i].y);
            if (MaxY < Mathf.CeilToInt(convexHull.ConvexHullSet[i].y)) MaxY = Mathf.CeilToInt(convexHull.ConvexHullSet[i].y);

            ConvexHullLine.Draw();
            yield return new WaitForSeconds(0.15f);
        }

        //button.transform.GetChild(0).GetComponent<Text>().text = (ConvexHullArea / 2).ToString("0.00");// �������������Ҫ����2

        ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[0].x, convexHull.ConvexHullSet[0].y));
        //ConvexHullLine.SetColor(Color.blue);  // ������ɫ
        //ConvexHullLine.SetColor(ConvexHullLineColor);  // ������ɫ
        ConvexHullLine.Draw();

        ConvexHullIsDraw = false;

        StartCoroutine(DrawConvexHullArea(MinX - 10, MaxX + 10, MinY - 10, MaxY + 10));

        UponButtonOnClick();
    }


    IEnumerator DrawConvexHullArea(int MinX, int MaxX, int MinY, int MaxY)
    {
        yield return new WaitForEndOfFrame();

        if (!ConvexHullIsDraw)
        {
            //if (ConvexHullArea.points2 != 0 )
            //{
            // ��͸������

            ConvexHullIsDraw = true;

            Color32 ConvexHullAreaColor = new Color32((Byte)0, (Byte)191, (Byte)255, (Byte)40);

            ConvexHullArea = new VectorLine("ConvexHullLine", new List<Vector2>(), 1f, Vectrosity.LineType.Continuous, Joins.Weld);
            ConvexHullArea.smoothColor = false;   // ����ƽ����ɫ
            ConvexHullArea.smoothWidth = false;   // ����ƽ�����
            ConvexHullArea.color = ConvexHullAreaColor;

            Texture2D m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            m_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            //m_texture.Apply();

            MinX = Math.Max(0, MinX);
            MinY = Math.Max(0, MinY);
     
            ConvexHullColors = m_texture.GetPixels(MinX, MinY, Math.Min(MaxX - MinX, 1919 - MinX), Math.Min(MaxY - MinY, 1079 - MinY));

            MaxY = MaxY - MinY - 1;

            int x, y;
            for (int i = 0; i < MaxY; i++)
            {
                x = i * (MaxX - MinX); y = (i + 1) * (MaxX - MinX);

                while ((x < y) && (ConvexHullColors[x] != ConvexHullLineColor)) x++;    // ������ߵ�͹���߽�
                while ((x < y) && (ConvexHullColors[y] != ConvexHullLineColor)) y--;    // �����ұߵ�͹���߽�

                if (x != y)
                {
                    ConvexHullArea.points2.Add(new Vector2(MinX + x - i * (MaxX - MinX), MinY + i));
                    ConvexHullArea.points2.Add(new Vector2(MinX + y - i * (MaxX - MinX), MinY + i));
                }
            }
            ConvexHullArea.Draw();
            //}
        }
    }


    public void ReturnBackUI()
    {
        // ���ж����ݿ����Ƿ��ж���������ɾ��
        PatientDatabaseManager.instance.DelTempEvaluationPoints(evaluation.EvaluationID);
        DoctorDataManager.instance.FunctionManager = 1;
        DoctorDataManager.instance.EvaluationType = 1;


        SceneManager.LoadScene("03-DoctorUI");
    }

    public void EvaluationFinished() // ������д�����ݿ�
    {
        if (DoctorDataManager.instance.doctor.patient.Evaluations == null) 
        {
            DoctorDataManager.instance.doctor.patient.Evaluations = new List<Evaluation>();
        }

        if(evaluation.Points.Count > 0)
        {
            PatientDatabaseManager.instance.WriteEvaluationData(evaluation);

            DoctorDataManager.instance.doctor.patient.Evaluations.Add(evaluation);
        }
        DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);        
        DoctorDataManager.instance.FunctionManager = 1;
        DoctorDataManager.instance.EvaluationType = 1;


        SceneManager.LoadScene("03-DoctorUI");
    }

    //IEnumerator WriteEvaluationData()
    //{
    //    //PatientDatabaseManager.instance.wri
    //    int a = 3;


    //}
    public void PlayMusicByName(string name)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);

        if (audiosource.isPlaying)
        {
            audiosource.Stop();
        }

        audiosource.clip = clip;
        audiosource.Play();
    }

    public Vector2 Kinect2UIPosition(Vector3 pos)
    {
        //print(pos);
        Vector2 uisize = canvas.GetComponent<RectTransform>().sizeDelta;//�õ������ĳߴ�
        Vector2 screenpos = Camera.main.WorldToScreenPoint(pos);//����������ת��Ϊ��Ļ����
        Vector2 screenpos2;
        screenpos2.x = screenpos.x;//ת��Ϊ����Ļ����Ϊԭ�����Ļ����
        screenpos2.y = screenpos.y;
        Vector2 uipos;
        uipos.x = (screenpos2.x / Screen.width) * uisize.x;
        uipos.y = (screenpos2.y / Screen.height) * uisize.y;//�õ�UGUI��anchoredPosition

        return uipos;
        //Mouse.transform.DOMove(uipos, 0.02f);
        //print(uipos);
    }
}
