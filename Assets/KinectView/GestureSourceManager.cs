using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;

// Adapted from DiscreteGestureBasics-WPF by Momo the Monster 2014-11-25
// For Helios Interactive - http://heliosinteractive.com

public class GestureSourceManager : MonoBehaviour
{

    public struct EventArgs
    {
        public string name;
        public float confidence;

        public EventArgs(string _name, float _confidence)
        {
            name = _name;
            confidence = _confidence;
        }
    }

    public BodySourceManager _BodySource;
    public string databasePath = "Gestures.gbd";
    public Text Result;

    // 姿势名称列表
    public static string[] GestureNames =
    new string[]{
        "ArmExtend",    //双手前平举
        "Bobath",
        "FeetTogetherStand",    //双足并拢站立
        "LeftLegStand", //左脚单脚站立
        "RightLegStand",    //右脚单脚站立
        "Sit",  //坐
        "Stand",    //站
        "Sit2Stand",    //由坐到站
        "Stand2Sit", //由站到坐
        "leftArmRise",
        "RightArmRise",
        "LeftStep",
        "RightStep"
    };

    // 优先级
    private static Dictionary<string, int> Priority =
    new Dictionary<string, int>{
        { "ArmExtend", 1 },
        {"Bobath", 1 },
        {"FeetTogetherStand", 1 },
        {"LeftLegStand", 2 },
        {"RightLegStand", 2 },
        {"Sit", 1 },
        {"Stand", 0 },
        {"Sit2Stand", 1 },
        {"Stand2Sit", 1 },
        {"leftArmRise", 2},
        {"RightArmRise", 2},
        {"LeftStep", 2},
        {"RightStep", 2 }
    };



    // 阈值，超过阈值则认定姿势完成
    public static Dictionary<KinectGestures.Gestures, float> Threshold =
    new Dictionary<KinectGestures.Gestures, float>{
        { KinectGestures.Gestures.ArmExtend, 0.75f },
        {KinectGestures.Gestures.Bobath, 0.75f },
        {KinectGestures.Gestures.FeetTogetherStand, 0.75f },
        {KinectGestures.Gestures.LeftLegStand, 0.75f },
        {KinectGestures.Gestures.RightLegStand, 0.75f },
        {KinectGestures.Gestures.Sit, 0.75f },
        {KinectGestures.Gestures.Stand, 0.75f },
        {KinectGestures.Gestures.Sit2Stand, 0.75f },
        {KinectGestures.Gestures.Stand2Sit, 0.75f },
        {KinectGestures.Gestures.LeftArmRise, 0.75f },
        {KinectGestures.Gestures.RightArmRise, 0.75f },
        {KinectGestures.Gestures.LeftStep, 0.75f },
        {KinectGestures.Gestures.RightStep, 0.75f }
};


    private KinectSensor _Sensor;
    private VisualGestureBuilderFrameSource _Source;
    private VisualGestureBuilderFrameReader _Reader;
    private VisualGestureBuilderDatabase _Database;

    // Gesture Detection Events
    public delegate void GestureAction(EventArgs e);
    public event GestureAction OnGesture;


    public static GestureSourceManager instance = null;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("@GestureSourceManager: Singleton created.");

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


    // Use this for initialization
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }

            // Set up Gesture Source
            _Source = VisualGestureBuilderFrameSource.Create(_Sensor, 0);

            // open the reader for the vgb frames
            _Reader = _Source.OpenReader();
            if (_Reader != null)
            {
                _Reader.IsPaused = true;
                // _Reader.FrameArrived += GestureFrameArrived;    //此处为逐帧调用可能卡
            }

            // load the 'Seated' gesture from the gesture database
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, databasePath);
            Debug.Log("@GestureSourceManager: Model path is: " + path);
            _Database = VisualGestureBuilderDatabase.Create(path);

            // Load all gestures
            IList<Gesture> gesturesList = _Database.AvailableGestures;

            Debug.Log("gesturesList.Count: " + gesturesList.Count);

            foreach(var gesture in gesturesList)
            {

                Debug.Log("@GestureSourceManager: Model name is: " + gesture.Name);


                try { 

                    _Source.AddGesture(gesture);

                }
                catch(COMException e)
                {

                }

            }

        }

        OnGesture += ShowInText;
        instance = this;
    }

    // Public setter for Body ID to track
    private void SetBody(ulong id)
    {
        if (id > 0)
        {
            _Source.TrackingId = id;
            _Reader.IsPaused = false;
        }
        else
        {
            _Source.TrackingId = 0;
            _Reader.IsPaused = true;
        }
    }

    // Update Loop, set body if we need one
    void Update()
    {
        if (!_Source.IsTrackingIdValid)
        {
            FindValidBody();
        }

        //     UpdateGestureData();
        // Debug.Log("Bobath: "+ GetGestureConfidence(KinectGestures.Gestures.Bobath));
    }

    // Check Body Manager, grab first valid body
    private void FindValidBody()
    {

        if (_BodySource != null)
        {
            Body[] bodies = _BodySource.GetData();
            if (bodies != null)
            {
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        SetBody(body.TrackingId);
                        break;
                    }
                }
            }
        }

    }

    /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
    private void GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
    {
        VisualGestureBuilderFrameReference frameReference = e.FrameReference;
        using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
        {
            if (frame != null)
            {
                // get the discrete gesture results which arrived with the latest frame
                IDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                if (discreteResults != null)
                {

                    List<EventArgs> Results = new List<EventArgs>();

                    foreach (Gesture gesture in _Source.Gestures)
                    {
                        //Debug.Log(gesture.Name);
                        //Debug.Log(gesture.GestureType);
                        if (gesture.GestureType == GestureType.Discrete)    //离散型
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (IsDetected(Priority[gesture.Name], result.Confidence))    //该动作是否完成
                                {
                                    Results.Add(new EventArgs(gesture.Name, result.Confidence));    //完成则加入判断列表

                                    //Debug.Log("Detected Gesture " + gesture.Name + " with Confidence " + result.Confidence);
                                    // Fire Event
                                    //OnGesture(new EventArgs(gesture.Name, result.Confidence));
                                    //return;
                                }

                            }
                        }
                    }

                    EventArgs Result = GestureJudgement(Results);
                    Debug.Log("Detected Gesture " + Result.name + " with Confidence " + Result.confidence.ToString());
                    OnGesture(Result);
                }
            }

            //OnGesture(new EventArgs("NO-Gesture", 0));
        }
    }

    /// <summary>
    /// 封装调用，调用此函数以更新UI上显示的姿势
    /// </summary>
    private void UpdateGestureData()
    {
        using (var frame = this._Reader.CalculateAndAcquireLatestFrame())//计算并生成最新的VGB帧
        {
            if (frame != null)
            {
                var discreteResults = frame.DiscreteGestureResults;
                var continuousResults = frame.ContinuousGestureResults;

                if (discreteResults != null)
                {
                    List<EventArgs> Results = new List<EventArgs>();

                    foreach (var gesture in this._Source.Gestures)
                    {
                        if (gesture.GestureType == GestureType.Discrete)
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                if (IsDetected(Priority[gesture.Name], result.Confidence))    //该动作是否完成
                                {
                                    Results.Add(new EventArgs(gesture.Name, result.Confidence));    //完成则加入判断列表

                                    //Debug.Log("Detected Gesture " + gesture.Name + " with Confidence " + result.Confidence);
                                    // Fire Event
                                    //OnGesture(new EventArgs(gesture.Name, result.Confidence));
                                    //return;
                                }

                            }

                        }
                    }

                    EventArgs Result = GestureJudgement(Results);
                    Debug.Log("Detected Gesture " + Result.name + " with Confidence " + Result.confidence.ToString());
                    OnGesture(Result);
                }
            }
        }
    }


    /// <summary>
    /// 检测人物是否做出了输入的gesture，返回gesture的准确度
    /// </summary>
    /// <param name="gesture">需要检测的gesture</param>

    public float GetGestureConfidence(KinectGestures.Gestures gesture)
    {
        using (var frame = this._Reader.CalculateAndAcquireLatestFrame())//计算并生成最新的VGB帧
        {
            if (frame != null)
            {
                var discreteResults = frame.DiscreteGestureResults;
                var continuousResults = frame.ContinuousGestureResults;

                if (discreteResults != null)
                {
                    List<EventArgs> Results = new List<EventArgs>();

                    foreach (var GestureModel in this._Source.Gestures) //遍历各个姿势模型
                    {
                        if (GestureModel.Name == gesture.ToString() && 
                            GestureModel.GestureType == GestureType.Discrete)   //找到对应模型
                        {
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(GestureModel, out result);  //得到匹配准确度

                            if (result != null)
                            {
                                return result.Confidence;

                            }

                            Debug.Log("@GestureSourceManager: gesture result is null!");
                        }
                    }
                }
            }

            Debug.Log("@GestureSourceManager: No Kinect frame!");

            return -1f;
        }
    }

    /// <summary>
    /// 完成姿势返回True，否则返回False
    /// </summary>
    /// <param name="gesture">待检测的姿势</param>
    /// <returns></returns>
    public bool CheckGesture(KinectGestures.Gestures gesture)
    {
        return GetGestureConfidence(gesture) > Threshold[gesture];
    }



    private void ShowInText(EventArgs e)
    {
        this.Result.text = "Detected Gesture " + e.name + " with Confidence " + e.confidence;
    }


    /// <summary>
    /// 由Rank和Confidence判断该动作是否可能被识别
    /// </summary>
    /// <param name="Rank"></param>
    /// <param name="Confidence"></param>
    /// <returns></returns>
    private bool IsDetected(int Rank, float Confidence)
    {
        switch (Rank)
        {
            case 2:
                return Confidence > 0.5;
            case 1:
                return Confidence > 0.7;
            case 0:
                return Confidence > 0.95;
            default:
                return Confidence > 0.95;
        }
    }

    /// <summary>
    /// 从Results中判断当前最符合哪个姿势
    /// </summary>
    /// <param name="Results"></param>
    /// <returns></returns>
    private EventArgs GestureJudgement(List<EventArgs> Results)
    {
        List<EventArgs> Rank2 = new List<EventArgs>();
        List<EventArgs> Rank1 = new List<EventArgs>();
        List<EventArgs> Rank0 = new List<EventArgs>();

        foreach (EventArgs result in Results)
        {
            switch (Priority[result.name])
            {
                case 2:
                    Rank2.Add(result);
                    break;
                case 1:
                    Rank1.Add(result);
                    break;
                case 0:
                    Rank0.Add(result);
                    break;
            }
        }

        if (Rank2.Count != 0)
        {
            return SelectGesture(Rank2);
        }
        else if (Rank1.Count != 0)
        {
            return SelectGesture(Rank1);
        }
        else if (Rank0.Count != 0)
        {
            return SelectGesture(Rank0);
        }
        else
        {
            return new EventArgs("NO-Gesture", 0);
        }

    }

    /// <summary>
    /// 返回最大confidence对应的元素
    /// </summary>
    /// <param name="Rank">输入的EventArgs列表</param>
    /// <returns></returns>
    private EventArgs SelectGesture(List<EventArgs> Rank)
    {
        if (Rank.Count == 0)
        {
            throw new System.Exception("List<EventArgs> Rank is void.");
        }
        Rank.Sort(new GestureComparer());
        return Rank.Last<EventArgs>();
    }




    /// <summary>
    /// 自定义比较器
    /// </summary>
    class GestureComparer : IComparer<EventArgs>
    {
        public int Compare(EventArgs x, EventArgs y)
        {
            if (x.confidence > y.confidence)
            {
                return 1;
            }
            else if (Mathf.Approximately(x.confidence, y.confidence))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }


    }
}
