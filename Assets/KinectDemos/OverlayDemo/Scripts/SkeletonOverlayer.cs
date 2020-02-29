using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using Vectrosity;
using HighlightingSystem;
using DG.Tweening;
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

    private VectorLine ColorFistLine; // 彩色手势线
    private VectorLine ConvexHullLine;   // 凸包线

    ////用来索引端点
    //private int index = 0;

    //记录左右两个指尖的坐标
    private Vector3 HandTipLeft;
    private Vector3 SpineMid;   // 患者重心坐标
    private float FirstFistZ;   // 患者初始拳头距离
    private Vector2 LastPosition;
    private Vector2 NowPosition;
    private float LastNowDis;  // 两个点的差值

    // 记录坐标集,取前两个坐标,舍弃z轴,求凸包
    // 求凸包使用Melkman算法
    public class Point
    {
        public float x { get; set; } = 0.0f;
        public float y { get; set; } = 0.0f;
        public Point(float _x = 0.0f, float _y = 0.0f)
        {
            x = _x;
            y = _y;
        }
    }
    public class CoordinateComparer : IComparer<Point>
    {
        //实现姓名升序
        public int Compare(Point x, Point y)
        {
            if (x.y.CompareTo(y.y) == 0) return (x.x.CompareTo(y.x));
            else return (x.y.CompareTo(y.y));
        }

    }
    public float PointsDistance(Vector2 A, Vector2 B)
    {
        float Distance = Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y);
        return Distance;
    }
    private List<Point> Points;  // 数据点的集合 
    private Point[] pointArray;  //坐标数组
    private int PointNum = 0;    // 数据点的个数
    private Point[] ConvexHull;  // 凸包集
    private int ConvexHullNum;  // 凸包点的个数
    private float ConvexHullArea;  // 凸包面积
    private Point[] TwoTable; // 数组索引，双向表

    public Canvas canvas;

    // 左:0,左上:1,上:2,右上:3,右:4,右下:5,下:6,左下:7,中间:8
    public GameObject Soccerball;

    public Camera camera;
    public LineRenderer line;

    public Button button;
    public Image Introduction;
    public float WaitTime;   // 双手握拳的时间,初始设为3秒

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
        Points = new List<Point>();
        //index = 0;

        ColorFistLine = new VectorLine("ColorFistLine", new List<Vector2>(), 7.0f, LineType.Continuous, Joins.Weld);
        ColorFistLine.smoothColor = false;   // 设置平滑颜色
        ColorFistLine.smoothWidth = false;   // 设置平滑宽度
        //ColorFistLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

        // always mirrored
        initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        //Soccerball = transform.Find("Soccerball").gameObject;
        //Soccerball.transform.position = new Vector3(0.1f, 0.8f, 46f);

        //line = gameObject.AddComponent<LineRenderer>();
        //line = GetComponent<LineRenderer>();
        Soccerball = null;

        Introduction.transform.DOLocalMove(new Vector3(-0.024902f, 101.9f,0), 2.5f);
        //Introduction.transform.DOLocalMove(new Vector3(-0.024902f, 979f, 0), 2.5f);

        WaitTime = 3f;
    }

    void Update()
    {
        KinectManager manager = KinectManager.Instance;

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

                                    if(WaitTime > 0f)
                                    {
                                        WaitTime -= Time.deltaTime;
                                    }
                                    else
                                    {
                                        Introduction.transform.DOLocalMove(new Vector3(-0.024902f, 979f, 0), 2.5f);
                                        if (Points.Count == 0)
                                        {
                                            LastPosition = Kinect2UIPosition(posJoint);
                                            Points.Add(new Point(LastPosition.x, LastPosition.y));

                                            ColorFistLine.points2.Add(LastPosition);
                                            //ColorFistLine.Draw();

                                            // 初始放置足球
                                            transform.GetChild(0).position = SpineMid;
                                            FirstFistZ = posJoint.z;
                                            SoccerballReset();
                                        }
                                        else
                                        {
                                            NowPosition = Kinect2UIPosition(posJoint);

                                            LastNowDis = PointsDistance(LastPosition, NowPosition);
                                            LastPosition = NowPosition;

                                            int DeltaBase = 0, DeltaColorR = 0, DeltaColorG = 0;

                                            if (LastNowDis > 0.0f)
                                            {
                                                DeltaBase = (int)(LastNowDis * 7);

                                                if (DeltaBase <= 0) { DeltaColorR = 0; DeltaColorG = 0; }
                                                else if (DeltaBase > 0 && DeltaBase <= 255) { DeltaColorR = DeltaBase; DeltaColorG = 0; }
                                                else if (DeltaBase > 255 && DeltaBase <= 510) { DeltaColorR = 255; DeltaColorG = DeltaBase - 255; }
                                                else if (DeltaBase > 510) { DeltaColorR = 255; DeltaColorG = 255; }

                                                Points.Add(new Point(NowPosition.x, NowPosition.y));
                                                ColorFistLine.points2.Add(NowPosition);
                                                ColorFistLine.SetColor(new Color32((Byte)DeltaColorR, (Byte)(255 - DeltaColorG), 0, (Byte)255), Points.Count - 2);
                                                //ColorFistLine.SetWidth(7.0f * LastNowDis / 20, Points.Count - 2);

                                                //ColorFistLine.Draw();
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

    public void SoccerballReset()
    {
        List<Vector3> PositionOffset = new List<Vector3> {		// 8个方向的偏移量
			new Vector3(0.5f, 0f, 0f),
            new Vector3(0.32f, 0.32f, 0f),
            new Vector3(0f, 0.5f, 0f),
            new Vector3(-0.32f, 0.32f, 0f),
            new Vector3(-0.5f, 0f, 0f),
            new Vector3(-0.32f, -0.32f, 0f),
            new Vector3(0f, -0.5f, 0f),
            new Vector3(0.32f, -0.32f, 0f),
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
        // 左:0,左上:1,上:2,右上:3,右:4,右下:5,下:6,左下:7
        if (Soccerball.name == "Soccerball0")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x += sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball1")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x += ddir;
            TempPos.y += ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball2")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.y += sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball3")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x -= ddir;
            TempPos.y += ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball4")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x -= sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball5")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x -= ddir;
            TempPos.y -= ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball6")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.y -= sdir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball7")
        {
            Vector3 TempPos = Soccerball.transform.position;
            TempPos.x += ddir;
            TempPos.y -= ddir;
            Soccerball.transform.position = TempPos;
        }
        else if (Soccerball.name == "Soccerball")
        {
            Vector3 TempPos = Soccerball.transform.localScale;
            float ZOffset = FirstFistZ - FistPos.z;
            
            if(ZOffset == 0f) { Soccerball.GetComponent<Highlighter>().ConstantOn(Color.red); }
            else if(ZOffset > 0f) { Soccerball.GetComponent<Highlighter>().ConstantOn(Color.green); }
            else if(ZOffset < 0f) { Soccerball.GetComponent<Highlighter>().ConstantOn(Color.blue);}

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

    public void EvaluationFinished() // 求凸包
    {

        if (Points != null && Points.Count > 0)
        {
            ColorFistLine.Draw();

            Points.Sort(new CoordinateComparer());   // 对点排序一下才能用凸包算法

            pointArray = new Point[Points.Count];
            Points.Add(new Point(0.0f, 0.0f));    // 加入一个点方便下面的循环计算

            //print(Points.Count);
            for (int i = 0; i < Points.Count - 1; i++)
            {

                //print(Points[i].x+" "+Points[i].y+" "+i);
                // 去掉一些重复的点
                if (Points[i].x == Points[i + 1].x && Points[i].y == Points[i + 1].y)
                {
                    Points.RemoveAt(i + 1);
                    i--;
                }
                else
                {
                    // 记录不重复的点
                    pointArray[PointNum++] = Points[i];
                }
            }

            ConvexHullNum = ConvexHullMelkman(pointArray, PointNum);

            // 画凸包圈
            ConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 8.0f, LineType.Continuous, Joins.Weld);
            ConvexHullLine.smoothColor = false;   // 设置平滑颜色
            ConvexHullLine.smoothWidth = false;   // 设置平滑宽度
            ConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

            StartCoroutine(DrawConvexHull());

        }

    }

    IEnumerator DrawConvexHull()
    {
        // 先把初始点存入画图函数
        ConvexHullLine.points2.Add(new Vector2(ConvexHull[0].x, ConvexHull[0].y));
        ConvexHullArea = 0f;   // 令凸包面积初始为0

        for (int i = 1; i < ConvexHullNum; i++)
        {
            ConvexHullLine.points2.Add(new Vector2(ConvexHull[i].x, ConvexHull[i].y));
            ConvexHullLine.SetColor(new Color32((Byte)0, (Byte)191, (Byte)255, (Byte)255));  // 设置颜色

            if (i < ConvexHullNum - 1)
            {
                ConvexHullArea += Math.Abs(isLeft(ConvexHull[0], ConvexHull[i], ConvexHull[i + 1]));
            }

            ConvexHullLine.Draw();
            yield return new WaitForSeconds(0.15f);
        }

        button.transform.GetChild(0).GetComponent<Text>().text = (ConvexHullArea / 2).ToString("0.00");// 最后求出来的面积要除以2

        ConvexHullLine.points2.Add(new Vector2(ConvexHull[0].x, ConvexHull[0].y));
        //ConvexHullLine.SetColor(Color.blue);  // 设置颜色
        ConvexHullLine.SetColor(new Color32((Byte)0, (Byte)191, (Byte)255, (Byte)255));  // 设置颜色
        ConvexHullLine.Draw();
    }

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

    // isLeft(): test if a point is Left|On|Right of an infinite line.
    //    Input:  three points P0, P1, and P2
    //    Return: >0 for P2 left of the line through P0 and P1
    //            =0 for P2 on the line
    //            <0 for P2 right of the line
    //    See: Algorithm 1 on Area of Triangles
    public float isLeft(Point P0, Point P1, Point P2)
    {
        //print(P0.x + " " + P0.y +" "+P1.x+ " " + P1.y + " " + P2.x+" "+P2.y);
        return (P1.x - P0.x) * (P2.y - P0.y) - (P2.x - P0.x) * (P1.y - P0.y);
    }

    // ConvexHullMelkman(): Melkman's 2D simple polyline O(n) convex hull algorithm
    //    Input:  P[] = array of 2D vertex points for a simple polyline
    //            n   = the number of points in V[]
    //    Output: H[] = output convex hull array of vertices (max is n)
    //    Return: h   = the number of points in H[]
    public int ConvexHullMelkman(Point[] P, int n)
    {
        // initialize a deque D[] from bottom to top so that the
        // 1st three vertices of P[] are a ccw triangle
        TwoTable = new Point[2 * n + 1];
        ConvexHull = new Point[n];

        int bot = n - 2, top = bot + 3;    // initial bottom and top deque indices
        TwoTable[bot] = TwoTable[top] = P[2];        // 3rd vertex is at both bot and top
        if (isLeft(P[0], P[1], P[2]) > 0)
        {
            TwoTable[bot + 1] = P[0];
            TwoTable[bot + 2] = P[1];           // ccw vertices are: 2,0,1,2
        }
        else
        {
            TwoTable[bot + 1] = P[1];
            TwoTable[bot + 2] = P[0];           // ccw vertices are: 2,1,0,2
        }

        // compute the hull on the deque D[]
        for (int i = 3; i < n; i++)
        {
            //print(i + "!!!!!");
            // process the rest of vertices
            // test if next vertex is inside the deque hull
            if ((isLeft(TwoTable[bot], TwoTable[bot + 1], P[i]) > 0) &&
                (isLeft(TwoTable[top - 1], TwoTable[top], P[i]) > 0))
                continue;         // skip an interior vertex

            // incrementally add an exterior vertex to the deque hull
            // get the rightmost tangent at the deque bot
            while (isLeft(TwoTable[bot], TwoTable[bot + 1], P[i]) <= 0)
                ++bot;                 // remove bot of deque
            TwoTable[--bot] = P[i];           // insert P[i] at bot of deque

            // get the leftmost tangent at the deque top
            while (isLeft(TwoTable[top - 1], TwoTable[top], P[i]) <= 0)
                --top;                 // pop top of deque
            TwoTable[++top] = P[i];           // push P[i] onto top of deque
        }
        //print("!!!!\n");
        // transcribe deque D[] to the output hull array H[]
        int h;        // hull vertex counter
        for (h = 0; h <= (top - bot); h++)
        {
            ConvexHull[h] = TwoTable[bot + h];
        }

        return h - 1;
    }

}
