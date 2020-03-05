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

    ////用来索引端点
    //private int index = 0;

    //记录左右两个指尖的坐标
    private Vector3 HandTipLeft;
    private Vector3 SpineMid;   // 患者重心坐标
    private float FirstFistZ;   // 患者初始拳头距离
    private Vector2 LastPosition;
    private Vector2 NowPosition;
    private float LastNowDis;  // 两个点的差值

    public Canvas canvas;

    // 上:0,右上:1,右:2,右下:3,下:4,左下:5,左:6,左上:7,中间:8
    public GameObject Soccerball;

    public Camera camera;
    public LineRenderer line;

    public Image Introduction;
    public float WaitTime;   // 双手握拳的时间,初始设为3秒
    public Slider KinectDetectUIProgressSlider;  // 进度条

    public Image Buttons;   // 下面三个button的背景
    //public GameObject FinishedButton;  // 完成测评按钮

    public Evaluation evaluation;   // 新建一个评估测试
    public List<Point> tempPoints;  // 临时用于画凸包的点集
    public ConvexHull convexHull;   // 新建一个凸包

    private VectorLine ColorFistLine;   // 彩色手势线
    private VectorLine ConvexHullLine;   // 凸包线

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
        //new VectorLine("L1", new List<Vector3> {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f)}, 7.0f);

        KinectManager manager = KinectManager.Instance;

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
        evaluation = new Evaluation();   // 新建一个评估测试

        if(DoctorDataManager.instance.doctor.patient.Evaluations == null || DoctorDataManager.instance.doctor.patient.Evaluations.Count == 0)
        {
            evaluation.SetEvaluationID(0);
        }
        else
        {
            evaluation.SetEvaluationID(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);
        }

        //Points = new List<Point>();
        //index = 0;

        // always mirrored
        initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        //Soccerball = transform.Find("Soccerball").gameObject;
        //Soccerball.transform.position = new Vector3(0.1f, 0.8f, 46f);

        //line = gameObject.AddComponent<LineRenderer>();
        //line = GetComponent<LineRenderer>();
        Soccerball = null;

        Introduction.transform.DOLocalMove(new Vector3(0f, 101.9f,0), 2.5f);
        Buttons.transform.DOLocalMove(new Vector3(0f, -438.05f, 0), 2.5f);
        //Introduction.transform.DOLocalMove(new Vector3(-0.024902f, 979f, 0), 2.5f);

        WaitTime = 0f;

        // 把线去掉
        if (ColorFistLine != null) VectorLine.Destroy(ref ColorFistLine);  // 握拳轨迹图
        if (ConvexHullLine != null) VectorLine.Destroy(ref ConvexHullLine);  // 凸包图

        //FinishedButton.SetActive(false);   // 刚开始返回按钮不显示
    }

    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        //鼠标下滑出现按钮框
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

        if (manager && manager.IsInitialized() && foregroundCamera)
        {
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

                                //posJoint.z = 0f;  // 将z轴改为0,平面显示

                                joints[i].SetActive(true);
                                joints[i].transform.position = posJoint;

                                if (i == 21) { HandTipLeft = posJoint; }

                                if (i == 1) { SpineMid = posJoint; }

                                // 当左右手距离小于0.1f的时候画线
                                if (i == 23 && (HandTipLeft - posJoint).magnitude < 0.13f)   // 患者开始握拳了
                                {

                                    if(WaitTime < 3.0f)
                                    {
                                        WaitTime += Time.deltaTime;
                                        KinectDetectUIProgressSlider.value = WaitTime / 3.0f;
                                    }
                                    else
                                    {

                                        if (evaluation.Points.Count == 0)
                                        {
                                            evaluation.SetEvaluationStartTime(DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                                            Introduction.transform.DOLocalMove(new Vector3(0f, 978f, 0), 0.5f);

                                            LastPosition = Kinect2UIPosition(posJoint);
                                            evaluation.Points.Add(new Point(LastPosition.x, LastPosition.y));
                                            WritePointInGame();

                                            // 初始放置足球
                                            transform.GetChild(0).position = SpineMid;
                                            FirstFistZ = posJoint.z;
                                            SoccerballReset();
                                        }
                                        else
                                        {
                                            Buttons.transform.DOLocalMove(new Vector3(0f, -641.9f, 0), 0.5f);

                                            NowPosition = Kinect2UIPosition(posJoint);

                                            LastNowDis = Point.PointsDistance(new Point(LastPosition), new Point(NowPosition));
                                            LastPosition = NowPosition;

                                            if (LastNowDis > 0.0f)
                                            {
                                                evaluation.Points.Add(new Point(NowPosition));
                                                WritePointInGame();
                                            }

                                        }
                                        // 判断是否碰到球
                                        //print(Soccerball.transform.position + " " + posJoint + " " + (posJoint - Soccerball.transform.position).magnitude);
                                        //if((posJoint - Soccerball.transform.position).magnitude < 0.1f)
                                        //{
                                        //	Soccerball.transform.position = new Vector3(1f, 1f, 46f);
                                        //}
                                        for (int z = 0; z < 9; z++)
                                        {
                                            transform.GetChild(z).gameObject.SetActive(true);
                                        }

                                        RayCastResult(posJoint);    // 发出射线射到哪个足球
                                                                    //DrawRayLine(camera.transform.position, posJoint);
                                    }

                                }
                                else if (i == 23 && (HandTipLeft - posJoint).magnitude > 0.8f)
                                {
                                    if (Soccerball != null && Soccerball.GetComponent<Highlighter>() != null)
                                    {
                                        Soccerball.GetComponent<Highlighter>().ConstantOff();
                                    }

                                    //print(Buttons.transform.localPosition);
                                    Buttons.transform.DOLocalMove(new Vector3(0f, -438.05f, 0), 0.5f);
                                    //for (int z = 0; z < 9; z++)
                                    //{
                                    //    transform.GetChild(z).gameObject.SetActive(false);
                                    //}
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

                                //骨骼连线z轴改为0,平面显示
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

            }
        }
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
        List<Vector3> PositionOffset = new List<Vector3> {		// 8个方向的偏移量
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
                Soccerball = hit.collider.gameObject;
                if (Soccerball.GetComponent<Highlighter>() != null)
                {
                    Soccerball.GetComponent<Highlighter>().ConstantOn(Color.red);
                }
            }
            else if (Soccerball != hit.collider.gameObject)
            {
                if (Soccerball.GetComponent<Highlighter>() != null)
                {
                    Soccerball.GetComponent<Highlighter>().ConstantOff();
                }
                Soccerball = hit.collider.gameObject;
                if (Soccerball.GetComponent<Highlighter>() != null)
                {
                    Soccerball.GetComponent<Highlighter>().ConstantOn(Color.red);
                }
            }
            else if (Soccerball.GetComponent<Highlighter>() != null)
            {
                Soccerball.GetComponent<Highlighter>().ConstantOn(Color.red);
            }

            SoccerballMove(FistPos);
        }
        else
        {
            if (Soccerball != null && Soccerball.GetComponent<Highlighter>() != null)
            {
                Soccerball.GetComponent<Highlighter>().ConstantOff();
            }
        }

    }

    public void SoccerballMove(Vector3 FistPos)  // 足球移动
    {
        float sdir = 0.001f, ddir = 0.0007f;  // 根号2倍
        float ScaleOffset = 2f;
        // 上:2,右上:3,右:4,右下:5,下:6,左下:7,左:0,左上:1,
        if (Soccerball.name == "Soccerball0")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.y += sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball1")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x -= ddir;
            TempPos.y += ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball2")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x -= sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball3")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x -= ddir;
            TempPos.y -= ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball4")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.y -= sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball5")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x += ddir;
            TempPos.y -= ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball6")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x += sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball7")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x += ddir;
            TempPos.y += ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball")
        {
            Vector3 TempPos = Soccerball.transform.localScale;

            // 更新最大最小值
            if(evaluation.soccerDistance.CenterSoccerMin > TempPos.x)
            {
                evaluation.soccerDistance.CenterSoccerMin = TempPos.x;
            }

            if(evaluation.soccerDistance.CenterSoccerMax < TempPos.x)
            {
                evaluation.soccerDistance.CenterSoccerMax = TempPos.x;
            }

            float ZOffset = FirstFistZ - FistPos.z;
            
            if(TempPos.x == 3.5f) { Soccerball.GetComponent<Highlighter>().ConstantOn(Color.red); }
            else if(TempPos.y > 3.5f) { Soccerball.GetComponent<Highlighter>().ConstantOn(Color.green); }
            else if(TempPos.z < 3.5f) { Soccerball.GetComponent<Highlighter>().ConstantOn(Color.yellow);}

            TempPos.x = TempPos.y = TempPos.z = TempPos.x + ZOffset * ScaleOffset;
            //print(FistPos + " " + FirstFistZ);
            FirstFistZ = FistPos.z;
            Soccerball.transform.localScale = TempPos;
        }
    }

    public void DrawRayLine(Vector3 pos1, Vector3 pos2) // 画线函数
    {
        ////添加LineRenderer组件
        ////设置材质
        //FistLine.material = new Material(Resources.Load("Prefabs/RadarPrefabs/VirtualLineYellow") as Material);

        //////设置颜色
        ////FistLine.startColor = Color.yellow;
        ////FistLine.endColor = Color.red;
        ////设置宽度
        //FistLine.startWidth = 0.01f;
        //FistLine.endWidth = 0.02f;

        line.positionCount = 2; //　设置该线段由几个点组成

        // 设置线段的起点颜色和终点颜色
        line.startColor = Color.blue;
        line.endColor = Color.red;
        // 设置线段起点宽度和终点宽度
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.SetPosition(0, pos1);
        line.SetPosition(1, pos2);

    }

    public void EvaluationReStart()
    {
        OnEnable();
    }

    public void EvaluationTrack() // 求凸包并显示轨迹
    {

        if (evaluation.Points != null && evaluation.Points.Count > 0)
        {
            StartCoroutine(DrawColorFistLine());
        }

    }

    IEnumerator DrawColorFistLine()
    {
        // 结束后画图
        evaluation.SetEvaluationEndTime(DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));

        tempPoints = new List<Point>();
        evaluation.Points.ForEach(i => tempPoints.Add(i));  // 将所有的点复制给temppoints用于画凸包图

        ColorFistLine = new VectorLine("ColorFistLine", new List<Vector2>(), 7.0f, LineType.Continuous, Joins.Weld);
        ColorFistLine.smoothColor = false;   // 设置平滑颜色
        ColorFistLine.smoothWidth = false;   // 设置平滑宽度
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

        // 画凸包圈
        ConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 8.0f, LineType.Continuous, Joins.Weld);
        ConvexHullLine.smoothColor = false;   // 设置平滑颜色
        ConvexHullLine.smoothWidth = false;   // 设置平滑宽度
        ConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed
        
        Color32 ConvexHullLineColor = new Color32((Byte)0, (Byte)191, (Byte)255, (Byte)255);

        // 先把初始点存入画图函数
        ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[0].x, convexHull.ConvexHullSet[0].y));
        convexHull.ConvexHullArea = 0f;   // 令凸包面积初始为0

        for (int i = 1; i < convexHull.ConvexHullNum; i++)
        {
            ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[i].x, convexHull.ConvexHullSet[i].y));
            ConvexHullLine.SetColor(ConvexHullLineColor);  // 设置颜色

            if (i < convexHull.ConvexHullNum - 1)
            {
                convexHull.ConvexHullArea += Math.Abs(ConvexHull.isLeft(convexHull.ConvexHullSet[0], convexHull.ConvexHullSet[i], convexHull.ConvexHullSet[i + 1]));
            }

            ConvexHullLine.Draw();
            yield return new WaitForSeconds(0.15f);
        }

        //button.transform.GetChild(0).GetComponent<Text>().text = (ConvexHullArea / 2).ToString("0.00");// 最后求出来的面积要除以2

        ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[0].x, convexHull.ConvexHullSet[0].y));
        //ConvexHullLine.SetColor(Color.blue);  // 设置颜色
        ConvexHullLine.SetColor(ConvexHullLineColor);  // 设置颜色
        ConvexHullLine.Draw();

        // 记录足球的距离
        evaluation.soccerDistance.UponSoccer = (transform.GetChild(1).position - transform.GetChild(0).position).magnitude;
        evaluation.soccerDistance.UponRightSoccer = (transform.GetChild(2).position - transform.GetChild(0).position).magnitude;
        evaluation.soccerDistance.RightSoccer = (transform.GetChild(3).position - transform.GetChild(0).position).magnitude;
        evaluation.soccerDistance.DownRightSoccer = (transform.GetChild(4).position - transform.GetChild(0).position).magnitude;
        evaluation.soccerDistance.DownSoccer = (transform.GetChild(5).position - transform.GetChild(0).position).magnitude;
        evaluation.soccerDistance.DownLeftSoccer = (transform.GetChild(6).position - transform.GetChild(0).position).magnitude;
        evaluation.soccerDistance.LeftSoccer = (transform.GetChild(7).position - transform.GetChild(0).position).magnitude;
        evaluation.soccerDistance.UponLeftSoccer = (transform.GetChild(8).position - transform.GetChild(0).position).magnitude;       

        //FinishedButton.SetActive(true);   // 刚开始返回按钮不显示

    }

    public void ReturnBackUI()
    {
        // 先判断数据库中是否有多余数据再删除
        PatientDatabaseManager.instance.DelTempEvaluationPoints(evaluation.EvaluationID);

        SceneManager.LoadScene("03-DoctorUI");
    }

    public void EvaluationFinished() // 将数据写入数据库
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

        SceneManager.LoadScene("03-DoctorUI");
    }

    //IEnumerator WriteEvaluationData()
    //{
    //    //PatientDatabaseManager.instance.wri
    //    int a = 3;

       
    //}

    public Vector2 Kinect2UIPosition(Vector3 pos)
    {
        //print(pos);
        Vector2 uisize = canvas.GetComponent<RectTransform>().sizeDelta;//得到画布的尺寸
        Vector2 screenpos = Camera.main.WorldToScreenPoint(pos);//将世界坐标转换为屏幕坐标
        Vector2 screenpos2;
        screenpos2.x = screenpos.x;//转换为以屏幕中心为原点的屏幕坐标
        screenpos2.y = screenpos.y;
        Vector2 uipos;
        uipos.x = (screenpos2.x / Screen.width) * uisize.x;
        uipos.y = (screenpos2.y / Screen.height) * uisize.y;//得到UGUI的anchoredPosition

        return uipos;
        //Mouse.transform.DOMove(uipos, 0.02f);
        //print(uipos);
    }
}
