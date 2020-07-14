using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*public enum ActionType//动作类型
{
    ActionType1,
    ActionType2,
    ActionType3,
    NONE
}
public enum TrainingType//训练类型，如上肢训练...
{
    TrainingType1,
    TrainingType2,
    TrainingType3,
    NONE
}
//康复类型和训练类型是否有必要合并尚待确定
public enum ActionDifficulty
{
    EASY,
    MEDIUM,
    DIFFICULT,
    NONE
}*/
public class User {
    public int ID;
    public string name;
    public int age;
    public string sex;
    public int weight;
    public int trainingTypeId;
    public string pwd;
    public Level level;
    public User()
    {
        ID = 0;
        name = "";
        age = -1;
        sex = "";
        pwd = "";
        weight = -1;
        trainingTypeId = -1;
        pwd = "";
        level = new Level();
    }
    public User(int id1,string name1,string sex1,int age1,int weight1,int trainingTypeId1, string pwd1, Level level1)
    {
        ID = id1;
        name = name1;
        sex = sex1;
        age = age1;
        weight = weight1;
        trainingTypeId = trainingTypeId1;
        pwd = pwd1;
        level = new Level(level1) ;
    }
    public User(User user)
    {
        ID = user.ID;
        name = user.name;
        sex = user.sex;
        age = user.age;
        weight = user.weight;
        trainingTypeId = user.trainingTypeId;
        pwd = user.pwd;
        level = new Level(user.level);
    }
}
public class WallDoctor
{
    public int id;
    public string name;
    public string pwd;
    public WallDoctor()
    {
        id = -1;
        name = "";
        pwd = "";
    }
    public WallDoctor(int ID1,string name1,string pwd1)
    {
        id = ID1;
        name = name1;
        pwd = pwd1;
    }
}
public class Level
{
    public int wallSpeed;//墙板速度
    public string StartTime;//训练开始时间
    public int trainingDays;//训练天数
    public bool isWallRandom;//墙体是否按顺序生成
    public int actionNum;//动作个数
    public Dictionary<int, int> actionRates;
    public Level()
    {
        wallSpeed = 3;
        StartTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        trainingDays = 5;
        isWallRandom = false;
        actionNum = 0;
        actionRates = new Dictionary<int, int>();
    }
    public Level(int ws, string st, int td, bool wr, int an, Dictionary<int, int> ar)
    {
        wallSpeed = ws;
        StartTime = st;
        trainingDays = td;
        isWallRandom = wr;
        actionNum = an;
        actionRates = new Dictionary<int, int>();
        foreach (var item in ar)
        {
            actionRates.Add(item.Key, item.Value);
        }
    }
    public Level(Level level)
    {
        wallSpeed = level.wallSpeed;
        StartTime = level.StartTime;
        trainingDays = level.trainingDays;
        isWallRandom = level.isWallRandom;
        actionNum = level.actionNum;
        actionRates = new Dictionary<int, int>();
        foreach (var item in level.actionRates)
        {
            actionRates.Add(item.Key, item.Value);
        }

    }
}
/// <summary>
/// the information of body actions.
/// </summary>
public class Action
{


    public int id;//动作ID
    public string name;//中文名称
    public string describe;//动作描述
    public string createTime;//录入时间
    public string filename;//动作图片文件根目录
    public string sideFilename;//侧面动作图片文件目录
    public string gameFilename;//游戏动作图片文件目录
    public List<int> checkJoints;//检测的关节
    public ActionData actionData;//动作数据
    public Action()
    {
        id = -1;
        name = "";
        describe = "";
        createTime = "";
        actionData = new ActionData();
        filename = "";
        sideFilename = "";
        gameFilename = "";
        checkJoints = new List<int>();
    }
    public Action(Action action)
    {
        id = action.id;
        name = action.name;
        describe = action.describe;
        createTime = action.createTime;
        actionData = action.actionData;
        filename = action.filename;
        sideFilename = action.sideFilename;
        gameFilename = action.gameFilename;
        checkJoints = new List<int>();
        foreach (var checkJoint in action.checkJoints)
        {
            checkJoints.Add(checkJoint);
        }
    }
}
public class PreAction
{
    public int id;
    public string filename;//动作图片文件目录
    public string sideFilename;//侧面动作图片文件目录
    public ActionData actionData;//动作数据
    public PreAction()
    {
        id = -1;
        filename = "";
        sideFilename = "";
        actionData = new ActionData();
    }
    public PreAction(PreAction preaction)
    {
        id = preaction.id;
        filename = preaction.filename;
        sideFilename = preaction.sideFilename;
        actionData = preaction.actionData;
    }

}

public static class DATA
{
    /// <summary>
    /// 动作库
    /// </summary>
    public static List<Action> actionList=new List<Action>();
    /// <summary>
    /// 预备动作库，录入后尚未添加到动作库中的动作
    /// </summary>
    public static List<PreAction> preActionList = new List<PreAction>();
    
    public static string actionSavePath = "/ActionImages/screenshot/action/";
    public static string sideactionSavePath = "/ActionImages/screenshot/sideaction/";
    public static string preactionSavePath = "/ActionImages/screenshot/preaction/";
    public static string sidepreactionSavePath = "/ActionImages/screenshot/sidepreaction/";
    public static string actionForGameSavePath =  "/ActionImages/gameimage/";
    public static string databasePath = "sqlite4unity.db";
    public static string doctorInfoFile = System.Environment.CurrentDirectory+"/doctorinfo/doctorinfo.txt";
    //public static List<Level> levels = new List<Level>();
    //public static Dictionary<int, string> level_name = new Dictionary<int, string>() { { 0, "第一关" }, { 1, "第二关" }, { 2, "第三关" } };
    public static List<string> difficulty_name = new List<string>();
    public static List<string> recoverType_name = new List<string>();
    public static List<string> actionType_name = new List<string>();
    public static List<WallDoctor> doctor_info = new List<WallDoctor>();
    public static List<User> user_info = new List<User>();
    public static int max_action_amount_for_level = 100;
    public static int action_amount_interval = 5;
    public static int max_level_time = 50;
    public static int level_time_interval = 1;
    public static int max_training_amount = 100;
    public static int training_amount_interval = 5;
    public static int max_level_amount = 3;
    public static int max_difficulty_level = 5;
    public static int max_recoverType_amount = 10;
    public static int max_actionType_amount = 10;
    public static int max_trainingType_amount = 10;
    public static int default_actionRate = 6;
    public static string doctor_name="未登录";
    public static float normalization_param=0;
    public static List<int> defaultCheckJoints = new List<int>(){ 0, 2, 3, 4, 5, 6, 8, 9, 10,12,13,14,15,16,17,18,19,20 };
    public static List<int> defaultMatchingCheckJoints = new List<int>() { 2, 4, 5, 8, 9, 12, 13, 14, 16, 17, 18 };
    public static Dictionary<string,float> ActionMatchThreshold=new Dictionary<string, float>() { {"PERFECT",90F }, { "GREAT", 80F }, { "GOOD", 70F } };
    //24个关节夹角检测方法
    public static Dictionary<int, List<int>> JointCheckMethod = new Dictionary<int, List<int>>() {
        { 2, new List<int>() { 23,24 } },
        { 4, new List<int>() { 1, 2, 3, 4 } },
        { 5, new List<int>() { 9 } },
        { 12, new List<int>() { 11,12,13 } },
        { 13, new List<int>() { 17 } },
        { 14, new List<int>() { 20 } },//21号检测方法不可靠，暂时去掉
        { 8, new List<int>() { 5,6,7,8 } },
        { 9, new List<int>() { 10 } },
        { 16, new List<int>() { 14,15,16 } },
        { 17, new List<int>() { 18 } },
        { 18, new List<int>() { 22 } }};//21号检测方法不可靠，暂时去掉

    public static List<int> JointOfMethod = new List<int>() { 0, 4, 4, 4, 4, 8, 8, 8, 8, 5, 9, 12, 12, 12, 16, 16, 16, 13, 17, 14, 14, 18, 18, 2, 2 };
    public static Dictionary<int, int> MaxValueOfMethod = new Dictionary<int, int>() { { 1,180}, { 2, 180 }, { 3, 180 }, { 4, 180 }, { 5, 180 },
    { 6,180}, { 7, 180 }, { 8, 180 }, { 9, 180 }, { 10, 180 },
    { 11,90}, { 12, 90 }, { 13, 150 }, { 14, 90 }, { 15, 90 },
    { 16,150}, { 17, 180 }, { 18, 180 }, { 19, 180 }, { 20, 180 },
    { 21,180}, { 22, 180 }, { 23, 135 }, { 24, 135 }};
    public static Dictionary<int, int> MinValueOfMethod = new Dictionary<int, int>() { { 1,0}, { 2, 0 }, { 3, 0 }, { 4, 20 }, { 5, 0 },
    { 6,0}, { 7, 0 }, { 8, 20 }, { 9, 25 }, { 10, 25 },
    { 11,0}, { 12, 0 }, { 13, 60 }, { 14, 0 }, { 15, 0 },
    { 16,60}, { 17, 50 }, { 18, 50 }, { 19, 60 }, { 20, 0 },
    { 21,60}, { 22, 0 }, { 23, 45 }, { 24, 45 }};
    public static float ShoulderInvalidateAngle = 20;
    public static int ToBeImprovedJointNum = 3;
    public static float KinectErrorAngle = 10;
    public static Dictionary<int, List<int>> TrainingTypeToJoint = new Dictionary<int, List<int>>() {//训练类型（类型id,对应包含的关节id）
    { 0,new List<int>() { 4,5} },
    { 1,new List<int>() { 8,9} },
    { 2,new List<int>() { 4} },
    { 3,new List<int>() { 8} },
    { 4,new List<int>() { 13,12} },
    { 5,new List<int>() { 12} },
    { 6,new List<int>() { 16,17} },
    { 7,new List<int>() { 16} },
    };
    public static Dictionary<int, string> TrainingTypeIDToName = new Dictionary<int, string>() {//训练类型（类型id,对应包含的关节id）
    { 0,"左小臂截肢" },
    { 1,"右小臂截肢" },
    { 2,"左大臂截肢" },
    { 3,"右大臂截肢" },
    { 4,"左小腿截肢" },
    { 5,"左大腿截肢" },
    { 6,"右小腿截肢" },
    { 7,"右大腿截肢" },
    };
    public static List<int> wallSpeed = new List<int>() {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    public static List<int> trainingDays = new List<int>() { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
    public static Dictionary<int, string> methodIDToName = new Dictionary<int, string>()
    {
        { 1,"屈曲伸展"},
        { 2,"外展内收"},
        { 3,"内旋外旋"},
        { 4,"水平收展"},
        { 5,"屈曲伸展"},
        { 6,"外展内收"},
        { 7,"内旋外旋"},
        { 8,"水平收展"},
        { 9,"屈曲伸展"},
        { 10,"屈曲伸展"},
        { 11,"屈曲伸展"},
        { 12,"外展内收"},
        { 13,"内旋外旋"},
        { 14,"屈曲伸展"},
        { 15,"外展内收"},
        { 16,"内旋外旋"},
        { 17,"屈曲伸展"},
        { 18,"屈曲伸展"},
        { 19,"背屈跖屈"},
        { 20,"内翻外翻"},
        { 21,"背屈跖屈"},
        { 22,"内翻外翻"},
        { 23,"左右侧屈"},
        { 24,"屈曲伸展"},
    };
    public static Dictionary<int, string> checkjointIDToName = new Dictionary<int, string>()
    {
        {2,"颈关节" },
        {4,"左肩关节" },
        {5,"左肘关节" },
        {8,"右肩关节" },
        {9,"右肘关节" },
        {12,"左髋关节" },
        {13,"左膝关节" },
        {14,"左踝关节" },
        {16,"右髋关节" },
        {17,"右膝关节" },
        {18,"右踝关节" },
    };
    public static Dictionary<int, string> TrainingProgramIDToName = new Dictionary<int, string>();
    public static Dictionary<int, List<int>> TrainingProgramIDToActionIDs = new Dictionary<int, List<int>>();


    // 动作名称对应的gesture
    public static Dictionary<string, KinectGestures.Gestures> Name2Gesture =
    new Dictionary<string, KinectGestures.Gestures>{
        {"无支撑下坐位", KinectGestures.Gestures.Sit},
        {"无支撑的站立", KinectGestures.Gestures.Stand},
        {"站立双上肢前伸", KinectGestures.Gestures.ArmExtend},
        {"双足并拢站立", KinectGestures.Gestures.FeetTogetherStand},
        {"左脚单脚站立", KinectGestures.Gestures.LeftLegStand},
        {"右脚单脚站立", KinectGestures.Gestures.RightLegStand},
        {"独坐举左臂", KinectGestures.Gestures.LeftArmRise},
        {"独坐举右臂", KinectGestures.Gestures.RightArmRise},
        {"左脚前跨步", KinectGestures.Gestures.LeftStep},
        {"右脚前跨步", KinectGestures.Gestures.RightStep},
    };
}
public static class GameData
{
    public static List<Action> reservedActionList = new List<Action>();
    public static string ReservedActionsDatabasePath = "reservedactions.db";
    public static float normalization_param = 0;
    //public static List<int> jointIndex=new List<int>();
    public static List<User> user_info = new List<User>();      // 从数据库中读取到的user信息
    public static int current_user_id=-1;
}
public class OnePeriodTrainingData
{
    public int type;//训练类型
    public string startTime;//开始时间
    public PeriodOverview overview;
    public PeriodDetail detail;
    public OnePeriodTrainingData()
    {
        type = -1;
        startTime = "";
        overview = new PeriodOverview();
        detail = new PeriodDetail();
    }
}
public class PeriodOverview
{
    public Dictionary<string, OneDayTrainingData> dayDatas;
    public PeriodOverview()
    {
        dayDatas = new Dictionary<string, OneDayTrainingData>();
    }
}
public class OneDayTrainingData
{
    public string date;//当天训练时间
    public float passScore;//当天总通过率
    public float accuracyScore;//当天总准确率
    public int compliance;//依从指数
    public List<int> toBeImproved;//有待提高
    public int trainingTimes;//当天训练次数
    public OneDayTrainingData()
    {
        date = "";
        passScore = -1;
        accuracyScore = -1;
        compliance = 0;
        toBeImproved = new List<int>();
        trainingTimes = 0;
    }
}
public class PeriodDetail
{
    public Dictionary<int, PeriodJointData> jointDatas;
    public PeriodDetail()
    {
        jointDatas = new Dictionary<int, PeriodJointData>();
    }
}
public class PeriodJointData
{
    public Dictionary<string, float> passPercent;//（开始日期，通过率）
    public float performance;//该关节通过率总体进步程度表现，以拟合直线的斜率体现
    public Dictionary<int, PeriodMethodData> methodDatas;//（检测方法ID，关节检测方法的数据）
    public PeriodJointData()
    {
        passPercent = new Dictionary<string, float>();
        performance = 0;
        methodDatas = new Dictionary<int, PeriodMethodData>();
    }
}
public class PeriodMethodData
{
    public Dictionary<string, float> errorPercent;
    public float performance;//该检测指标总体进步表现，以拟合直线的斜率体现
    public PeriodMethodData()
    {
        errorPercent = new Dictionary<string, float>();
        performance = 0;
    }
}


public class OneTrainingData
{
    public string startTime;//开始时间
    public int type;//训练类型
    public List<ActionData> actionDatas;//
    public OneTrainingOverrall overrall;//总评
    public OneTrainingOverview overview;//总览
    public OneTrainingDetail detail;//详细数据
    public OneTrainingData()
    {
        startTime = "";
        type = 0;
        overrall = new OneTrainingOverrall();
        actionDatas = new List<ActionData>();
        overview = new OneTrainingOverview();
        detail = new OneTrainingDetail();
    }

    public OneTrainingData(string startTime, int type, OneTrainingOverrall overrall, OneTrainingOverview overview, OneTrainingDetail detail)
    {
        this.startTime = startTime;
        this.type = type;
        this.actionDatas = new List<ActionData>();
        this.overrall = overrall;
        this.overview = overview;
        this.detail = detail;
    }

}
public class ActionData
{
    public Dictionary<int, JointData> jointDatas;//（关节ID,该动作关节全部数据）
    public int actionId;
    public ActionData()
    {
        jointDatas = new Dictionary<int, JointData>();
        actionId = 0;
    }
}
public class JointData
{
    public Dictionary<int, MethodData> methodDatas;//（检测方法ID,该动作检测方法全部数据）
    public List<int> invalidateMethods;
    public JointData()
    {
        methodDatas = new Dictionary<int, MethodData>();
        invalidateMethods = new List<int>();
    }
}
public class MethodData
{
    public float currentAngle;//当前动作夹角
    public float standardAngle;//标准动作夹角
    public float accuracy;//准确率
    public MethodData()
    {
        currentAngle = 0;
        standardAngle = 0;
        accuracy = 0;
    }
    public MethodData(float CurrentAngle,float StandardAngle,float Accuracy)
    {
        currentAngle = CurrentAngle;
        standardAngle = StandardAngle;
        accuracy = Accuracy;
    }
}
public class OneTrainingOverrall
{
    public int score;//评分，根据该次训练所有动作的通过率和准确率进行评分
    public float passScore;//通过率评分
    public float accuracyScore;//准确率评分
    public int duration;//持续时间
    public int lastScore;//上次同类型训练开始时间
    public int compliance;//依从指数,指一次训练中出现的动作（包括没通过的）占总动作个数的比例
    public int actionNum;//该次训练总动作个数
    public OneTrainingOverrall()
    {

        score = -1;
        passScore = -1;
        accuracyScore = -1;
        duration = 0;
        lastScore = -1;
        compliance = 0;
        actionNum = 0;
    }
}
public class OneTrainingOverview
{
    public Dictionary<int, OneActionData> actionDatas;//（动作ID，动作总体数据）
    public OneTrainingOverview()
    {
        actionDatas = new Dictionary<int, OneActionData>();
    }
}
public class OneActionData
{
    public int accuracy;//动作准确性，优良中差，为该动作所有出现的准确度的平均值。即accuracy值为accuracyList平均值
    public int passPercent;//动作整体通过率，优良中都算通过，差为不通过
    public List<int> toBeImproved;//有待提高的关节ID，将该动作的每个关节完成度从小到大排列，依次取不合格的最多三个关节显示，超过三个则添加"..."，无不合格关节则显示"---"
    public List<float> accuracyList;//用于计算accuracy，该动作每次出现时的准确度，准确度计算为该动作各关节点所有检测方法准确度的最小值。检测方法准确度=1-|实际角度-标准角度|/标准角度
    public OneActionData()
    {
        accuracy = 0;
        passPercent = 0;
        toBeImproved = new List<int>();
        accuracyList = new List<float>();
    }
}
public class OneTrainingDetail
{
    public Dictionary<int, OneJointData> jointDatas;//（关节ID，关节数据）
    public OneTrainingDetail()
    {
        jointDatas = new Dictionary<int, OneJointData>();
    }
}
public class OneJointData
{
    public Dictionary<int, List<int>> actionPassBool;//（动作ID,该动作每次出现时是否通过）
    public Dictionary<int, float> passPercent;//（动作ID，该动作的通过率）
    public float passPercentScore;//该关节总体通过率
    public Dictionary<int, OneMethodData> methodDatas;//（检测方法ID，关节检测方法的数据）
    public OneJointData()
    {
        actionPassBool = new Dictionary<int, List<int>>();
        passPercent = new Dictionary<int, float>();
        passPercentScore = 0;
        methodDatas = new Dictionary<int, OneMethodData>();
    }

}
public class OneMethodData
{
    public List<float> ep;//描述出现的每个动作和标准的误差百分比
    public float eps;//误差百分比的评分
    public OneMethodData()
    {
        ep = new List<float>();
        eps = 0;
    }
}
