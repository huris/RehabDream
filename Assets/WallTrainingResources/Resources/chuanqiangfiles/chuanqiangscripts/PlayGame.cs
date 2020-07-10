using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayGame : MonoBehaviour
{
    User currentUser;
    public long userId;
    public List<GameObject> wallGoList;
    public GameObject parent;
    public float time = 0;
    float progress = 0f;
    List<GameObject> walls = new List<GameObject>();
    List<AnimatorStateInfo> anistateinfos = new List<AnimatorStateInfo>();
    float averageTime = 3;
    Level currentLevel;
    //int currentLevelID = 0;
    List<int> actionIds;//动作id，该动作出现几次就有几次
    List<int> wallActionIds;//墙对应动作的id
    string state;
    int StateTimes = 0;
    public Text name;
    public Text leftTime;
    public Text wallprogress;
    int currentItemProgress;
    public GameObject wallProgressItems;
    public Image personFront;
    public Image personSide;
    int passNum = 0;
    int perfectNum = 0;
    int greatNum = 0;
    int goodNum = 0;
    int totalNum = 0;
    //Dictionary<string,actionNum> passDict;
    Dictionary<int, Sprite> wallactionSprite;
    Dictionary<int, Sprite> remindactionSprite;
    Dictionary<int, Sprite> remindactionSpriteSide;
    int actionNum = 0;      //墙的总数
    float rate = 0f;
    public GameObject CompleteScoreImg;
    public GameObject OverallMeritScoreImg;
    public Text lastscore;
    public Text actionnum;
    public Text duration;
    int NTimesSpeed = 1;
    //public GameObject fog;
    public float perfectOffset = 0.08f;
    public float greatOffset = 0.12f;
    public float goodOffset = 0.15f;
    public GameObject performance;
    public GameObject performanceTimes;
    public GameObject redScreen;
    int AllLoadedActionNum = 0;
    int tmpActionNum = 0;
    static OneTrainingData trainingData;
    static OnePeriodTrainingData periodData;
    bool SaveTrainingDataDetail1 = false;
    bool SaveTrainingDataDetail2 = true;
    List<float> wallAnimationTimes = new List<float>();
    float leaveTime = 0;
    float restartTime = 0;
    public Text restartText;
    float CacheTime = 2;
    float overmit_time = 0;
    float over_yes_time = 0;
    float over_no_time = 0;
    public Image overmitCircle;
    public Image overYesCircle;
    public Image overNoCircle;
    List<Sprite> sprites;
    public Text debug_text;

    private void OnEnable()
    {

        AudiosManager.instance.PlayAudioEffect("change_page");
        AudiosManager.instance.PlayBGaudio("bg");
        state = "ready";
        time = 0;
        StateTimes = 0;
        AllLoadedActionNum = 0;
    }
    private void OnDisable()
    {
        AudiosManager.instance.StopBGaudio();
        state = "ready";
        StateTimes = 0;
        AllLoadedActionNum = 0;
    }
    // Update is called once per frame
    public void Pause()
    {
        Time.timeScale = 0;
        GameObject.Find("Playing").transform.Find("Pause").gameObject.SetActive(false);
        GameObject.Find("Playing").transform.Find("continue").gameObject.SetActive(true);

    }
    public void Contin()
    {
        Time.timeScale = 1;
        GameObject.Find("Playing").transform.Find("Pause").gameObject.SetActive(true);
        GameObject.Find("Playing").transform.Find("continue").gameObject.SetActive(false);
    }
    void Update()
    {

        // ready-->start-->complete-->overall_merit-->
        switch (state)
        {
            case "ready":
                {
                    if (StateTimes == 0)
                    {
                        #region 设置关卡参数，初始化界面数据，预备游戏图片
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            transform.GetChild(i).gameObject.SetActive(false);
                        }
                        foreach (var user in GameData.user_info)
                        {
                            if (user.ID == GameData.current_user_id)
                            {
                                currentLevel = user.level;
                                currentUser = user;
                            }
                        }


                        //生成游戏中以此出现的动作序列
                        actionIds = new List<int>();
                        actionNum = 0;
                        foreach (var item in currentLevel.actionRates)
                        {
                            actionNum += item.Value;
                            for (int i = 0; i < item.Value; i++)
                            {
                                actionIds.Add(item.Key);
                            }
                        }//以上是生成该关卡所包含的动作的index,关卡内该动作出现几次生成几次。






                        averageTime = currentLevel.wallSpeed;       //averageTime为墙运动到人所需要的时间
                        wallActionIds = new List<int>();
                        transform.root.Find("Game/Playing").gameObject.SetActive(true);
                        name.text = currentUser.name;
                        leftTime.text = (currentLevel.wallSpeed * currentLevel.actionNum).ToString() + "秒";
                        wallprogress.text = "0 / " + actionNum;
                        totalNum = 0;
                        passNum = 0;
                        goodNum = 0;
                        greatNum = 0;
                        perfectNum = 0;
                        time = 0f;
                        currentItemProgress = 0;
                        for (int i = 0; i < wallProgressItems.transform.childCount; i++)
                        {
                            wallProgressItems.transform.GetChild(i).gameObject.SetActive(false);
                        }
                        GameObject go1 = transform.Find("Playing").Find("loading").gameObject;
                        go1.SetActive(true);    //在游戏界面显示Loading...
                        go1.transform.Find("percent").gameObject.GetComponent<Text>().text = 0 + "%";
                        StateTimes++;   //本if只运行一次
                        trainingData = new OneTrainingData();   //保存训练的参数
                        trainingData.startTime = DateTime.Now.ToString("yy-MM-dd-HH-mm");
                        trainingData.type = currentUser.trainingTypeId;
                        trainingData.overrall.duration = currentLevel.wallSpeed * currentLevel.actionNum / 60;
                        if (AllLoadedActionNum == 0)
                        {//没有加在过图片
                            wallactionSprite = new Dictionary<int, Sprite>();       //墙上的动作图
                            remindactionSprite = new Dictionary<int, Sprite>();    //正面演示图
                            remindactionSpriteSide = new Dictionary<int, Sprite>(); //侧面演示图
                            AllLoadedActionNum = 0;
                            tmpActionNum = -1;
                            foreach (var item in currentUser.level.actionRates)     //为每种动作加载图片
                            {
                                if (!wallactionSprite.ContainsKey(item.Key))
                                {
                                    wallactionSprite.Add(item.Key, new Sprite());
                                    remindactionSprite.Add(item.Key, new Sprite());
                                    remindactionSpriteSide.Add(item.Key, new Sprite());
                                }
                            }
                        }
                        else
                        {
                            // 加载完图片后关闭Playing/loading界面
                            transform.Find("Playing").Find("loading").gameObject.SetActive(false);
                            transform.Find("Playing").Find("loading").Find("percent").gameObject.GetComponent<Text>().text = 0 + "%";
                            StartCoroutine(Prompt1());
                            state = "start";
                            StateTimes = 0;
                            time = 0;
                        }

                        #endregion
                    }


                    #region 加载图片
                    if (AllLoadedActionNum > tmpActionNum)
                    {
                        //AllLoadedActionNum 初始值为0
                        //tmpActionNum 初始值为-1
                        tmpActionNum = AllLoadedActionNum;
                        if (AllLoadedActionNum < wallactionSprite.Count)
                        {
                            int key = wallactionSprite.Keys.ToList()[AllLoadedActionNum];

                            // 加载墙上的人物图案
                            StartCoroutine(LoadImage(key, Environment.CurrentDirectory + DATA.actionForGameSavePath + key + ".png", 0));
                            GameObject go1 = transform.Find("Playing").Find("loading").gameObject;
                            go1.SetActive(true);

                            // 更新加载资源百分数
                            go1.transform.Find("percent").gameObject.GetComponent<Text>().text = AllLoadedActionNum * 100 / (wallactionSprite.Count + remindactionSprite.Count + remindactionSpriteSide.Count) + "%";
                        }
                        else if (AllLoadedActionNum - wallactionSprite.Count < remindactionSprite.Count)
                        {
                            int key = remindactionSprite.Keys.ToList()[AllLoadedActionNum - wallactionSprite.Count];

                            // 加载正面动作演示图案
                            StartCoroutine(LoadImage(key, Environment.CurrentDirectory + DATA.actionSavePath + key + ".png", 1));
                            GameObject go1 = transform.Find("Playing").Find("loading").gameObject;
                            go1.SetActive(true);
                            go1.transform.Find("percent").gameObject.GetComponent<Text>().text = AllLoadedActionNum * 100 / (wallactionSprite.Count + remindactionSprite.Count + remindactionSpriteSide.Count) + "%";

                        }
                        else if (AllLoadedActionNum - wallactionSprite.Count - remindactionSprite.Count < remindactionSpriteSide.Count)
                        {
                            int key = remindactionSpriteSide.Keys.ToList()[AllLoadedActionNum - wallactionSprite.Count - remindactionSprite.Count];

                            // 加载侧面动作演示图案
                            StartCoroutine(LoadImage(key, Environment.CurrentDirectory + DATA.sideactionSavePath + key + ".png", 2));
                            GameObject go1 = transform.Find("Playing").Find("loading").gameObject;
                            go1.SetActive(true);
                            go1.transform.Find("percent").gameObject.GetComponent<Text>().text = AllLoadedActionNum * 100 / (wallactionSprite.Count + remindactionSprite.Count + remindactionSpriteSide.Count) + "%";

                        }
                        else
                        {
                            // 加载完图片后关闭Playing/loading界面
                            // 此处代码不可能被运行
                            GameObject go1 = transform.Find("Playing").Find("loading").gameObject;
                            go1.active = false;
                            go1.transform.Find("percent").gameObject.GetComponent<Text>().text = 0 + "%";
                            StartCoroutine(Prompt1());
                            state = "start";
                            StateTimes = 0;
                            time = 0;
                        }

                    }
                    #endregion
                }
                break;
            case "start":
                {

                    #region 如果患者离开检测范围
                    if (KinectManager.Instance.IsUserDetected() == false)
                    {
                        //pause wall for once
                        if (wallAnimationTimes.Count == 0)
                        {//如果没有保存暂停状态，保存
                            wallAnimationTimes = new List<float>();
                            foreach (var wall in walls) // walls初始为空
                            {
                                wallAnimationTimes.Add(wall.GetComponent<Animation>()["wall_move"].time);
                                wall.GetComponent<Animation>().Stop();  //终止wall的动画
                            }
                        }
                        transform.Find("BackRemind").gameObject.SetActive(true);    // 激活页面“回到监测范围继续训练”
                        float tmpTime = leaveTime;
                        leaveTime += Time.deltaTime;
                        Text text = transform.Find("BackRemind").Find("Image").Find("seconds").gameObject.GetComponent<Text>(); //初始值为10
                        int leftTime = int.Parse(text.text);    //倒计时剩余时间
                        if ((int)leaveTime > (int)tmpTime)      //deltaTime累计超过1s，需要更新倒计时的数字
                        {
                            leftTime--;
                            text.text = leftTime + "";      //更新倒计时的数字
                            if (leftTime == 0)              //倒计时到达0
                            {//患者不再进行训练
                                transform.Find("BackRemind").Find("Image").Find("seconds").gameObject.GetComponent<Text>().text = "10";     //恢复倒计时为10
                                transform.Find("BackRemind").gameObject.SetActive(false);
                                leaveTime = 0;//初始化退出倒计时
                                restartText.gameObject.SetActive(false);
                                restartText.text = 3 + "";
                                restartTime = 0;//初始化倒计时状态
                                //设置训练数据
                                CompleteTrainingData();
                                print("trainingData.overrall.compliance=" + trainingData.overrall.compliance);
                                if (trainingData.overrall.compliance > 20)      //退出时，若完成了超过20%的训练，则记录数据
                                {
                                    UpdateDatabase();
                                    state = "complete";
                                    StateTimes = 0;
                                }
                                else
                                {
                                    //transform.root.Find("Login").gameObject.SetActive(true);    //!
                                    transform.root.Find("Game").gameObject.SetActive(false);
                                    ClickNo();
                                }
                            }
                        }
                        return;
                    }
                    #endregion



                    else
                    {//检测到了患者


                        #region 如果当前游戏仍处于暂停阶段
                        if (wallAnimationTimes.Count != 0)      //wallAnimationTimes记录的是各个墙组件动画在暂停时刻的播放进度
                        {//如果墙体动画有暂停，倒计时三秒开始
                            transform.Find("BackRemind").gameObject.SetActive(false);
                            restartText.gameObject.SetActive(true);
                            float tmpTime = restartTime;
                            restartTime += Time.deltaTime;
                            int leftTime = int.Parse(restartText.text);
                            if ((int)restartTime > (int)tmpTime)
                            {
                                leftTime--;
                                restartText.text = leftTime + "";
                                if (leftTime == 0)
                                {//倒计时结束，继续墙体动画
                                    for (int i = 0; i < walls.Count; i++)
                                    {
                                        GameObject wall = walls[i];
                                        wall.GetComponent<Animation>()["wall_move"].time = wallAnimationTimes[i];
                                        wall.GetComponent<Animation>().Play();  //从暂停点开始，继续播放动画
                                    }
                                    wallAnimationTimes.Clear();     //清空
                                    restartText.gameObject.SetActive(false);
                                    restartText.text = 3 + "";      //恢复原倒计时
                                    restartTime = 0;//初始化倒计时状态
                                    transform.Find("BackRemind").Find("Image").Find("seconds").gameObject.GetComponent<Text>().text = "10";
                                    transform.Find("BackRemind").gameObject.SetActive(false);
                                    leaveTime = 0;//初始化退出提示倒计时
                                }
                            }
                            return;//不执行后边代码
                        }

                        #endregion


                    }
                    float before = time;
                    time += Time.deltaTime;
                    float after = time;
                    int tmp = (currentLevel.wallSpeed * currentLevel.actionNum - (int)time) >= 0 ? (currentLevel.wallSpeed * currentLevel.actionNum - (int)time) : 0;
                    leftTime.text = tmp.ToString() + "秒";       //更新游戏时间倒计时


                    #region 如果游戏刚开始或时间间隔大于averageTime，则需要生成一个墙
                    if (((int)(before / averageTime) < (int)(after / averageTime) || before == 0f) && actionIds.Count != 0)
                    {
                        int index = 0;
                        if (currentLevel.isWallRandom) index = UnityEngine.Random.Range(0, actionIds.Count);    //随机选取动作，如果不随机，则每次选取index=0的动作，玩成后从列表中删除
                        int actionId = actionIds[index];
                        foreach (var item in DATA.actionList)
                        {
                            if (actionId == item.id)
                            {
                                AudiosManager.instance.PlayActionAudio(item.name);      //播放动作对应的音效
                            }
                        }
                        wallActionIds.Add(actionId);        //将选取的动作加入动作队列
                        GameObject go = wallGoList[UnityEngine.Random.Range(0, wallGoList.Count - 1)];      //随机选取一种墙的风格
                        go.transform.Find("sprite").gameObject.GetComponent<SpriteRenderer>().sprite = wallactionSprite[actionId];  //在墙上增加动作图片
                        GameObject newWall = Instantiate(go);       //实例化墙
                        Animation animation = newWall.GetComponent<Animation>();    //实例墙的动画
                        string output = "";
                        print("当前速度为:" + currentLevel.wallSpeed);
                        foreach (AnimationState state in animation)     //调整动画速度
                        {
                            state.speed = 12f / currentLevel.wallSpeed;
                            output += "wallspeed=" + currentLevel.wallSpeed + "state.speed=" + state.speed;
                        }

                        //debug_text.text = output;
                        newWall.transform.parent = parent.transform;    //为实例化墙制定父物体
                        walls.Add(newWall);     //将墙加入墙实例化列表
                        personFront.sprite = remindactionSprite[actionId];      //展示正面、侧面示意图
                        personSide.sprite = remindactionSpriteSide[actionId];

                        actionIds.RemoveAt(index);      //删除一个动作

                    }

                    #endregion
                    if (walls != null && walls.Count != 0)  //如果墙实例化列表不为空
                    {


                        if (walls[0].transform.position.x < -214)       //如果第一个墙实例已经穿过了人物模型
                        {
                            totalNum++;


                            //使用角度检测对动作评分
                            int value = CheckThePosition();


                            //使用Kinect模型对动作评分
                            Action standord_action = new Action();
                            for (int i = 0; i < DATA.actionList.Count; i++)
                            {
                                if (wallActionIds[0] == DATA.actionList[i].id)
                                {
                                    standord_action = DATA.actionList[i];       //读取标准动作
                                }
                            }
                            int KinectScore = 0;


                            // 存在该动作的Kinect模型
                            if (DATA.Name2Gesture.ContainsKey(standord_action.name))
                            {
                                KinectScore = (int)(GestureSourceManager.instance.GetGestureConfidence(DATA.Name2Gesture[standord_action.name]) * 100);
                                Debug.Log("Gesture " + DATA.Name2Gesture[standord_action.name] + " : " + KinectScore);
                            }
                            else
                            {
                                Debug.Log("No Kinect gesture model: " + standord_action.name);
                                KinectScore = -100;
                            }

                            int KinectValue = Scores2CodeResult(KinectScore);


                            // 动作可以使用Kinect检测
                            if (KinectScore != -100)
                            {
                                #region 按照Kinect模型评分在屏幕上显示结果
                                if (KinectValue < 0)
                                {
                                    passNum++;
                                    //AudiosManager.instance.PlayAudioEffect("pass");
                                    performance.SetActive(true);
                                    performanceTimes.SetActive(false);
                                    if (KinectValue == -1)//good
                                    {
                                        AudiosManager.instance.PlayAudioEffect("棒");
                                        perfectNum = 0;
                                        greatNum = 0;
                                        goodNum++;
                                        StartCoroutine(ComboFadeInFadeOut("good", goodNum));


                                    }
                                    else if (KinectValue == -2)//GREAT
                                    {
                                        AudiosManager.instance.PlayAudioEffect("很棒");
                                        perfectNum = 0;
                                        greatNum++;
                                        goodNum = 0;
                                        StartCoroutine(ComboFadeInFadeOut("great", greatNum));

                                    }
                                    else//PERFECT
                                    {
                                        AudiosManager.instance.PlayAudioEffect("完美");
                                        perfectNum++;
                                        greatNum = 0;
                                        goodNum = 0;
                                        StartCoroutine(ComboFadeInFadeOut("perfect", perfectNum));
                                    }
                                }
                                else
                                {
                                    StartCoroutine(RedScreenForNotPass());  //红屏1.5s
                                    performance.SetActive(false);
                                    performanceTimes.SetActive(false);
                                }

                                #endregion
                            }
                            else
                            {
                                #region 按照角度检测评分
                                if (value < 0)
                                {
                                    passNum++;
                                    //AudiosManager.instance.PlayAudioEffect("pass");
                                    performance.SetActive(true);
                                    performanceTimes.SetActive(false);
                                    if (value == -1)//good
                                    {
                                        AudiosManager.instance.PlayAudioEffect("棒");
                                        perfectNum = 0;
                                        greatNum = 0;
                                        goodNum++;
                                        StartCoroutine(ComboFadeInFadeOut("good", goodNum));


                                    }
                                    else if (value == -2)//GREAT
                                    {
                                        AudiosManager.instance.PlayAudioEffect("很棒");
                                        perfectNum = 0;
                                        greatNum++;
                                        goodNum = 0;
                                        StartCoroutine(ComboFadeInFadeOut("great", greatNum));

                                    }
                                    else//PERFECT
                                    {
                                        AudiosManager.instance.PlayAudioEffect("完美");
                                        perfectNum++;
                                        greatNum = 0;
                                        goodNum = 0;
                                        StartCoroutine(ComboFadeInFadeOut("perfect", perfectNum));
                                    }
                                }
                                else
                                {

                                    StartCoroutine(RedScreenForNotPass());  //红屏1.5s
                                    performance.SetActive(false);
                                    performanceTimes.SetActive(false);

                                    //重置连续成功次数
                                    perfectNum = 0;
                                    greatNum = 0;
                                    goodNum = 0;
                                    //AudiosManager.instance.PlayAudioEffect("not_pass");
                                    if (value == 2)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("脖子歪了");
                                    }
                                    else if (value == 4)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("左肩不标准");
                                    }
                                    else if (value == 34 || value == 38)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("肩关节不标准");
                                    }
                                    else if (value == 5)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("左肘不标准");
                                    }
                                    else if (value == 35 || value == 39)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("肘关节不标准");
                                    }
                                    else if (value == 8)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("右肩不标准");
                                    }
                                    else if (value == 9)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("右肘不标准");
                                    }
                                    else if (value == 12)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("左髋不标准");
                                    }
                                    else if (value == 42 || value == 46)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("髋关节不标准");
                                    }
                                    else if (value == 16)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("右髋不标准");
                                    }
                                    else if (value == 13)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("左膝不标准");
                                    }
                                    else if (value == 43 || value == 47)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("膝关节不标准");
                                    }
                                    else if (value == 14)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("左脚不标准");
                                    }
                                    else if (value == 17)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("右膝不标准");
                                    }
                                    else if (value == 18)
                                    {
                                        AudiosManager.instance.PlayAudioEffect("右脚不标准");
                                    }
                                }
                                #endregion
                            }


                            GameObject tmpgo = walls[0];
                            walls.RemoveAt(0);      //删除已经评分的墙实例
                            StartCoroutine(DelayDestoryObj(tmpgo, 0.5f));   //0.5s后销毁墙实例
                            int passedWalls = int.Parse(wallprogress.text.Split(' ')[0]) + 1;   //训练数目+1
                            wallprogress.text = passedWalls + " / " + actionNum;        //重新显示训练进度
                            int itemNum = wallProgressItems.transform.childCount;
                            int itemProgress = (int)((float)itemNum / actionNum * passedWalls);
                            if (currentItemProgress < itemProgress)     //修改进度条
                            {
                                for (int i = currentItemProgress; i < itemNum; i++)
                                {
                                    if (i < itemProgress)
                                    {
                                        wallProgressItems.transform.GetChild(i).gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        wallProgressItems.transform.GetChild(i).gameObject.SetActive(false);
                                    }
                                }
                            }
                            wallActionIds.RemoveAt(0);      //删除已完成的动作
                            if (wallActionIds.Count > 0)
                            {
                                personFront.sprite = remindactionSprite[wallActionIds[0]];      //展示下一个动作的正面、侧面图片
                                personSide.sprite = remindactionSpriteSide[wallActionIds[0]];
                            }


                        }
                    }


                    if (actionIds.Count == 0 && wallActionIds.Count == 0)   //如果动作训练完了
                    {
                        CompleteTrainingData(); //总结训练数据
                        UpdateDatabase();       //更新数据库
                        state = "complete";     //改变状态
                        StateTimes = 0;
                    }

                }
                break;
            case "complete":
                {
                    if (StateTimes == 0)
                    {
                        StateTimes = 1;
                        StartCoroutine(Complete());
                    }
                }
                break;
            case "overall_merit":       //进入总评界面
                {
                    if (transform.root.Find("Game/OverallMerit").gameObject.activeInHierarchy == false)
                    {
                        transform.root.Find("Game/Complete").gameObject.SetActive(false);
                        transform.root.Find("Game/OverallMerit").gameObject.SetActive(true);
                        AudiosManager.instance.PlayAudioEffect("change_page");
                        if (OverallMeritScoreImg.transform.childCount != 0)
                        {
                            for (int i = 0; i < OverallMeritScoreImg.transform.childCount; i++)
                            {
                                Destroy(OverallMeritScoreImg.transform.GetChild(i).gameObject);
                            }
                        }
                        GameObject go = new GameObject();
                        rate = trainingData.overrall.passScore / 100f;
                        if (trainingData.overrall.lastScore == -1)
                        {
                            lastscore.text = "无";
                        }
                        else
                        {
                            lastscore.text = trainingData.overrall.lastScore + "";
                        }
                        actionnum.text = trainingData.overrall.actionNum + "";
                        duration.text = trainingData.overrall.duration + "";
                        if (trainingData.overrall.duration >= 10)       //两位数显示需要调整UI位置
                        {
                            Vector3 localpostion = duration.transform.GetChild(0).gameObject.GetComponent<RectTransform>().localPosition;
                            duration.transform.GetChild(0).gameObject.GetComponent<RectTransform>().localPosition = new Vector3(localpostion.x * 2, localpostion.y, localpostion.z);
                        }

                        // 按成功率给出评分
                        if (rate >= 0.9 && rate <= 1)//S
                        {
                            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Sscore") as GameObject);

                        }
                        else if (rate >= 0.8 && rate < 0.9)//A
                        {
                            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Ascore") as GameObject);
                        }
                        else if (rate >= 0.7 && rate < 0.8)//B
                        {
                            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Bscore") as GameObject);
                        }
                        else if (rate >= 0.6 && rate < 0.7)//C
                        {
                            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Cscore") as GameObject);
                        }
                        else //D
                        {
                            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Dscore") as GameObject);
                        }
                        go.transform.parent = OverallMeritScoreImg.transform;
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localScale = Vector3.one;
                        go.transform.Find("Text").gameObject.GetComponent<Text>().text = "评分：    分";
                        go.transform.Find("Text").Find("Text").gameObject.GetComponent<Text>().text = trainingData.overrall.score + "";
                        if (sprites == null || sprites.Count == 0)
                        {
                            sprites = new List<Sprite>();
                            for (int i = 0; i < 12; i++)
                            {
                                sprites.Add(Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/完成阶段训练/" + (i + 1), typeof(Sprite)) as Sprite);
                            }
                        }
                        print("overmit_time:" + overmit_time);

                    }


                    #region 右手离两肩中点的距离小于0.5达到2s，则触发按钮
                    //userId = KinectManager.Instance.GetPrimaryUserID();
                    //Vector3 handtipright = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandTipRight);        // 右手末梢
                    //Vector3 spineshoulder = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.SpineShoulder);      // 两肩中点
                    //if (((handtipright.y-spineshoulder.y)>0&& (handtipright.y - spineshoulder.y)<0.5)
                    //    || ((spineshoulder.y - handtipright.y)>0&& (spineshoulder.y - handtipright.y) < 0.5))
                    //{
                    //    float tmpTime = overmit_time;
                    //    overmit_time += Time.deltaTime;
                    //    if (overmit_time >= CacheTime)
                    //    {
                    //        ClickYesOverallMerit();
                    //    }
                    //    else
                    //    {
                    //        if ((int)(tmpTime * 12 / CacheTime) != (int)(overmit_time * 12 / CacheTime))
                    //        {
                    //            overmitCircle.sprite = sprites[(int)(overmit_time * 12 / CacheTime)];
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    overmit_time = 0;
                    //    overmitCircle.sprite = sprites[0];
                    //}

                    #endregion


                }
                break;
            case "over":            //进入游戏结束界面
                {
                    if (transform.root.Find("Game/Over").gameObject.activeInHierarchy == false)
                    {
                        transform.root.Find("Game/OverallMerit").gameObject.SetActive(false);
                        transform.root.Find("Game/Over").gameObject.SetActive(true);
                        if (sprites == null || sprites.Count == 0)
                        {
                            sprites = new List<Sprite>();
                            for (int i = 0; i < 12; i++)
                            {
                                sprites.Add(Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/完成阶段训练/" + (i + 1), typeof(Sprite)) as Sprite);
                            }
                        }
                        print("over_no_time:" + over_no_time);
                        print("over_yes_time:" + over_yes_time);
                    }

                    #region 左右手分别控制是、不是两个按钮

                    //    userId = KinectManager.Instance.GetPrimaryUserID();
                    //    Vector3 handtipleft = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandTipLeft);
                    //    Vector3 handtipright = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandTipRight);
                    //    Vector3 spineshoulder = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.SpineShoulder);
                    //    if (((handtipright.y - spineshoulder.y) > 0 && (handtipright.y - spineshoulder.y) < 0.5)
                    //        || ((spineshoulder.y - handtipright.y) > 0 && (spineshoulder.y - handtipright.y) < 0.5))
                    //    {
                    //        over_no_time = 0;
                    //        overNoCircle.sprite = sprites[0];
                    //        float tmpTime = over_yes_time;
                    //        over_yes_time += Time.deltaTime;
                    //        if (over_yes_time >= CacheTime)
                    //        {
                    //            ClickYes();


                    //        }
                    //        else
                    //        {
                    //            if ((int)(tmpTime * 12 / CacheTime) != (int)(over_yes_time * 12 / CacheTime))
                    //            {
                    //                overYesCircle.sprite = sprites[(int)(over_yes_time * 12 / CacheTime)];
                    //            }
                    //        }
                    //    }
                    //    else if (((handtipleft.y - spineshoulder.y) > 0 && (handtipleft.y - spineshoulder.y) < 0.5)
                    //        || ((spineshoulder.y - handtipleft.y) > 0 && (spineshoulder.y - handtipleft.y) < 0.5))
                    //    {
                    //        over_yes_time = 0;
                    //        overYesCircle.sprite = sprites[0];
                    //        float tmpTime = over_no_time;
                    //        over_no_time += Time.deltaTime;
                    //        if (over_no_time >= CacheTime)
                    //        {
                    //            ClickNo();

                    //        }
                    //        else
                    //        {
                    //            if ((int)(tmpTime * 12 / CacheTime) != (int)(over_no_time * 12 / CacheTime))
                    //            {
                    //                overNoCircle.sprite = sprites[(int)(over_no_time * 12 / CacheTime)];
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        over_yes_time = 0;
                    //        over_no_time = 0;
                    //        overYesCircle.sprite = sprites[0];
                    //        overNoCircle.sprite = sprites[0];
                    //    }
                    //}

                    #endregion


                    break;

                }
        }
    }
    IEnumerator Complete()
    {
        yield return new WaitForSeconds(2.5f);
        transform.root.Find("Game/Playing").gameObject.SetActive(false);
        transform.root.Find("Game/Complete").gameObject.SetActive(true);        //激活complete组件
        AudiosManager.instance.PlayAudioEffect("victory");
        if (CompleteScoreImg.transform.childCount != 0)     //销毁CompleteScoreImg下的子组件
        {
            for (int i = 0; i < CompleteScoreImg.transform.childCount; i++)
            {
                Destroy(CompleteScoreImg.transform.GetChild(i).gameObject);
            }
        }
        GameObject go = new GameObject();

        Debug.Log("trainingData.overrall.passScore: " + trainingData.overrall.passScore);

        rate = trainingData.overrall.passScore / 100f;


        if (trainingData.overrall.duration >= 10)
        {
            Vector3 localpostion = duration.transform.GetChild(0).gameObject.GetComponent<RectTransform>().localPosition;
            duration.transform.GetChild(0).gameObject.GetComponent<RectTransform>().localPosition = new Vector3(localpostion.x * 2, localpostion.y, localpostion.z);
        }

        // 根据准确率显示评分: S A B C D
        if (rate >= 0.9 && rate <= 1)//S
        {
            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Sscore") as GameObject);

        }
        else if (rate >= 0.8 && rate < 0.9)//A
        {
            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Ascore") as GameObject);
        }
        else if (rate >= 0.7 && rate < 0.8)//B
        {
            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Bscore") as GameObject);
        }
        else if (rate >= 0.6 && rate < 0.7)//C
        {
            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Cscore") as GameObject);
        }
        else //D
        {
            go = Instantiate<GameObject>(Resources.Load("chuanqiangfiles/chuanqiangprefabs/Dscore") as GameObject);
        }
        go.transform.parent = CompleteScoreImg.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.Find("Text").gameObject.GetComponent<Text>().text = "恭喜您，过关啦！";
        go.transform.Find("Text").Find("Text").gameObject.GetComponent<Text>().text = "";
        if (sprites == null || sprites.Count == 0)
        {
            sprites = new List<Sprite>();
            for (int i = 0; i < 12; i++)
            {
                sprites.Add(Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/完成阶段训练/" + (i + 1), typeof(Sprite)) as Sprite);
            }
        }
        yield return new WaitForSeconds(3f);
        state = "overall_merit";
        overmit_time = 0;

    }
    IEnumerator DelayDestoryObj(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
    /// <summary>
    /// return 0 means not pass,1 means good,2 means great,3 means perfect.
    /// </summary>
    /// <returns></returns>
    int CheckThePosition()
    {

        if (!KinectManager.Instance.IsUserDetected())
        {
            Debug.Log("user not detected");

            return 0;
        }
        userId = KinectManager.Instance.GetPrimaryUserID();
        List<Vector3> tmpPosition = new List<Vector3>();
        for (int i = 0; i < KinectManager.Instance.GetJointCount(); i++)
        {
            Vector3 pos = KinectManager.Instance.GetJointKinectPosition(userId, i);
            tmpPosition.Add(pos);       //读取所有可读取的关节点，读取不到的关节点坐标用0坐标表示
        }
        #region 根据坐标距离差判定动作是否标准
        ////action1 is the current position
        //Action action1 = new Action();
        //action1.Normalization_Param = GameData.normalization_param;
        //action1.Position = tmpPosition;
        ////Vector3 rootJoint1 = (action1.Position[(int)KinectInterop.JointType.HandTipLeft] + action1.Position[(int)KinectInterop.JointType.HandTipRight]) / 2;
        //Vector3 rootJoint1 = action1.Position[(int)KinectInterop.JointType.SpineBase];
        //List <Vector3> normalizedAction1 = NormalizationUtil.NormalizeAction(action1, rootJoint1);
        //string output = "current position:";
        //foreach (var pos in normalizedAction1)
        //{
        //    output += " " + pos.magnitude;
        //}
        //print(output);
        ////action2 is the standard position
        ////print(wallActionindexs[0]);
        //Action action2 = currentLevel.actions[wallActionindexs[0]];
        ////Vector3 rootJoint2 = (action2.Position[(int)KinectInterop.JointType.HandTipLeft] + action2.Position[(int)KinectInterop.JointType.HandTipRight]) / 2;
        //Vector3 rootJoint2 = action2.Position[(int)KinectInterop.JointType.SpineBase];
        //List<Vector3> normalizedAction2 = NormalizationUtil.NormalizeAction(action2, rootJoint2);

        //output = "standard position:";
        //foreach (var pos in normalizedAction2)
        //{
        //    output += " " + pos.magnitude;
        //}
        //print(output);
        ////if (NormalizationUtil.CheckSimilarity(normalizedAction2, normalizedAction1, GameData.jointIndex, 0.8f))
        ////{
        ////    return true;
        ////}
        //float maxOffset = NormalizationUtil.MaxOffset(normalizedAction2, normalizedAction1, action2.CheckJoints);
        //if (maxOffset<=perfectOffset)
        //{
        //    return 3;
        //}
        //else if (maxOffset > perfectOffset&&maxOffset<=greatOffset)
        //{
        //    return 2;
        //}
        //else if (maxOffset > greatOffset && maxOffset <= goodOffset)
        //{
        //    return 1;
        //}
        //else
        //{
        //    return 0;
        //}

        Action current_action = new Action();
        Action standord_action = new Action();
        for (int i = 0; i < DATA.actionList.Count; i++)
        {
            if (wallActionIds[0] == DATA.actionList[i].id)
            {
                standord_action = DATA.actionList[i];       //读取标准动作
            }
        }


        // 对于特殊需求的动作（双足并拢、双手笔直向前等），需要加入坐标距离检测
        PositionCalculator PC = new PositionCalculator(standord_action, tmpPosition);
        int PCScore = PC.CheckPosition();
        Debug.Log(PCScore);

        #endregion
        #region 根据角度差判定动作是否标准

        AngleCalculator AG = new AngleCalculator();
        AG.actionName = standord_action.name;
        current_action.actionData = AG.GetActionData(tmpPosition);      //计算使用多个检测方法对患者动作的检测结果，以及结果是否有效

        //使用多个方法对多个关节点角度检测，取最差结果返回，同时将结果记录在trainingData中
        int AGScore = AG.ActionMatching(current_action.actionData, standord_action, ref trainingData);
        #endregion


        if (PCScore >= 0)   //特殊动作坐标评测不通过
        {
            return PCScore;
        }
        else if (PCScore > AGScore)  //取角度、坐标评测的最低分
        {
            return PCScore;
        }
        else
        {
            return AGScore;
        }
    }


    private int Scores2CodeResult(int lowestAccuracy)
    {
        if (lowestAccuracy >= DATA.ActionMatchThreshold["PERFECT"])       //最低准确率大于0.9 * 100
        {
            return -3;
        }
        else if (lowestAccuracy >= DATA.ActionMatchThreshold["GREAT"])     //最低准确率大于0.8 * 100
        {
            return -2;
        }
        else if (lowestAccuracy > DATA.ActionMatchThreshold["GOOD"])        //最低准确率大于0.7 * 100
        {
            return -1;
        }
        return 1;
    }



    void CompleteTrainingData()
    {
        //List<float> accuracyList = new List<float>();
        if (trainingData.actionDatas.Count == 0)
        {
            print("训练数据为空！");
            return;
        }
        //加载所有动作id（不重复）
        List<int> actionId = new List<int>();
        for (int i = 0; i < trainingData.actionDatas.Count; i++)
        {
            if (!actionId.Contains(trainingData.actionDatas[i].actionId)) actionId.Add(trainingData.actionDatas[i].actionId);
        }
        //加载总览动作id
        for (int i = 0; i < actionId.Count; i++)
        {
            trainingData.overview.actionDatas.Add(actionId[i], new OneActionData());
        }
        for (int i = 0; i < trainingData.actionDatas.Count; i++)
        {//遍历每一个训练动作（有重复）
            ActionData actionData = trainingData.actionDatas[i];
            List<float> OneActionAccuracyList = new List<float>();
            for (int j = 0; j < DATA.defaultMatchingCheckJoints.Count; j++)
            {//当前动作的所有关节，用于详细数据
                int joint = DATA.defaultMatchingCheckJoints[j];
                if (!trainingData.detail.jointDatas.ContainsKey(joint))     //detail初始为空
                {//如果详细数据没有当前关节，加载关节和检测方法
                    OneJointData jointData = new OneJointData();
                    for (int num = 0; num < actionId.Count; num++)
                    {
                        jointData.actionPassBool.Add(actionId[num], new List<int>());
                    }
                    foreach (var method in DATA.JointCheckMethod[joint])
                    {
                        jointData.methodDatas.Add(method, new OneMethodData());
                    }
                    trainingData.detail.jointDatas.Add(joint, jointData);
                }
                if (actionData.jointDatas.ContainsKey(joint))
                {//当前训练动作检测关节包含当前关节，则记录当前关节的检测方法的数据
                    float MinJointAccuracy = 100000;
                    for (int k = 0; k < DATA.JointCheckMethod[joint].Count; k++)
                    {//遍历当前关节的每一个检测方法
                        int method = DATA.JointCheckMethod[joint][k];
                        if (!actionData.jointDatas[joint].invalidateMethods.Contains(method))       //只选取有效的检测结果
                        {//如果当前动作的检测关节包含当前检测方法，则记录当前动作的当前关节的当前方法的数据到详细数据中

                            float accuracy = actionData.jointDatas[joint].methodDatas[method].accuracy;//当前动作的当前关节的当前检测方法的准确率
                            trainingData.detail.jointDatas[joint].methodDatas[method].ep.Add(float.Parse((100 - accuracy).ToString("0.#")));//当前检测方法的误差百分比
                                                                                                                                            //print("actionPassBool=" + trainingData.detail.jointDatas[joint].actionPassBool.ToString());
                                                                                                                                            //print("actionData.actionId=" + actionData.actionId);
                            if (MinJointAccuracy > accuracy) MinJointAccuracy = accuracy;

                            OneActionAccuracyList.Add(accuracy);//记录一个动作各个检测方法的准确率
                        }
                    }
                    trainingData.detail.jointDatas[joint].actionPassBool[actionData.actionId].Add(MinJointAccuracy > DATA.ActionMatchThreshold["GOOD"] ? 1 : 0);//记录当前检测方法对于当前动作是否通过
                }
            }
            //计算该动作每次出现时各关节点准确度最小值作为该动作的准确度
            float minAccuracy = 100f;
            for (int j = 0; j < OneActionAccuracyList.Count; j++)
            {
                if (minAccuracy > OneActionAccuracyList[j]) minAccuracy = OneActionAccuracyList[j];
            }
            trainingData.overview.actionDatas[actionData.actionId].accuracyList.Add(float.Parse((minAccuracy).ToString("0.#")));

        }

        //完善详细数据
        foreach (var jointData in trainingData.detail.jointDatas)       //jointData为每一个默认检测的关节点
        {
            int joint = jointData.Key;      //一个默认检测的关节点
            int passNum = 0;
            int totalNum = 0;
            foreach (var actionPassBoolList in jointData.Value.actionPassBool)      //读取<动作,关节点在该动作中的评分列表>列表
            {//计算每个关节的每个动作的通过率
                if (actionPassBoolList.Value.Count == 0) jointData.Value.passPercent.Add(actionPassBoolList.Key, -1);//如果当前关节的当前动作为空，表示该动作不检测这个关节点，那么通过率就直接是100
                else
                {
                    jointData.Value.passPercent.Add(actionPassBoolList.Key, 0);
                    for (int i = 0; i < actionPassBoolList.Value.Count; i++)
                    {
                        jointData.Value.passPercent[actionPassBoolList.Key] += actionPassBoolList.Value[i];     //通过的Value[i]为1，不通过为0，全加起来
                        passNum += actionPassBoolList.Value[i];
                        totalNum++;
                    }

                    // 计算训练中某个关节在某种动作中的通过率
                    jointData.Value.passPercent[actionPassBoolList.Key] = (jointData.Value.passPercent[actionPassBoolList.Key] * 100f / actionPassBoolList.Value.Count);
                    jointData.Value.passPercent[actionPassBoolList.Key] = float.Parse((jointData.Value.passPercent[actionPassBoolList.Key]).ToString("0.#"));
                }
            }
            if (totalNum == 0)
            {//表示没有动作检测这个关节
                jointData.Value.passPercentScore = -1;
            }
            else
            {
                jointData.Value.passPercentScore = float.Parse((passNum * 100f / totalNum).ToString("0.#"));//计算该关节总体的通过率
            }
            foreach (var methodData in jointData.Value.methodDatas)     //methodData记录的是某个方法在游戏中进行的所有检测的误差列表
            {//计算每个检测方法的误差百分比
                int method = methodData.Key;
                float errorPercentScore = 0;
                if (methodData.Value.ep.Count == 0)     //该检测方法从没被使用过
                {
                    methodData.Value.eps = -1;
                }
                else
                {
                    for (int i = 0; i < methodData.Value.ep.Count; i++)
                    {
                        errorPercentScore += Mathf.Abs(methodData.Value.ep[i]);//这里必须计算绝对误差，不然总体值偏小
                    }
                    methodData.Value.eps = float.Parse((errorPercentScore / methodData.Value.ep.Count).ToString("0.#"));
                }

            }
        }
        //完善总体数据
        float passPercentFactor = 0.8f;
        float accuracyFactor = 1 - passPercentFactor;
        float totalPassNum = 0;
        float totalAccuracy = 0;
        int actionNum = 0;
        foreach (var actionData in trainingData.overview.actionDatas)
        {
            //计算每个动作的整体准确度及通过率
            List<float> accuracyList = trainingData.overview.actionDatas[actionData.Key].accuracyList;
            float average = 0;
            int passNum = 0;
            for (int i = 0; i < accuracyList.Count; i++)
            {
                average += accuracyList[i];
                passNum += accuracyList[i] > DATA.ActionMatchThreshold["GOOD"] ? 1 : 0;
            }
            totalAccuracy += average;
            totalPassNum += passNum * 100;
            actionNum += accuracyList.Count;
            average /= accuracyList.Count;
            trainingData.overview.actionDatas[actionData.Key].accuracy = (int)(average);
            trainingData.overview.actionDatas[actionData.Key].passPercent = (int)(passNum * 100f / accuracyList.Count);
            //计算每个动作待提高的关节
            List<int> toBeImprovedJoint = new List<int>();
            List<float> minValue = new List<float>();
            for (int i = 0; i < DATA.ToBeImprovedJointNum + 1; i++)     //每个动作需要提高的关节最多有DATA.ToBeImprovedJointNum个
            {
                minValue.Add(100);
                toBeImprovedJoint.Add(0);
            }
            foreach (var jointData in trainingData.detail.jointDatas)       //遍历每一个默认检查的关节点
            {
                if (jointData.Value.passPercent.ContainsKey(actionData.Key))
                {
                    for (int i = 0; i < minValue.Count; i++)
                    {
                        float passPercent = jointData.Value.passPercent[actionData.Key];       //读取某关节在某动作中的通过率
                        if (passPercent < minValue[i] && passPercent < DATA.ActionMatchThreshold["GOOD"])
                        {
                            for (int j = minValue.Count - 1; j > i; j--)
                            {
                                minValue[j] = minValue[j - 1];
                                toBeImprovedJoint[j] = toBeImprovedJoint[j - 1];        //按通过率升序排列
                            }
                            minValue[i] = passPercent;
                            toBeImprovedJoint[i] = jointData.Key;       //更新通过率最小的关节,没有用上？？？
                            break;
                        }
                    }

                }
            }

        }

        //score;//评分，根据该次训练所有动作的通过率和准确率进行评分
        //passScore;//通过率评分
        //accuracyScore;//准确率评分
        //duration;//持续时间
        //lastScore;//上次同类型训练开始时间
        //compliance;//依从指数,指一次训练中出现的动作（包括没通过的）占总动作个数的比例
        //actionNum;//该次训练总动作个数


        trainingData.overrall.actionNum = currentLevel.actionNum;
        trainingData.overrall.passScore = float.Parse((totalPassNum / actionNum).ToString("0.#"));
        trainingData.overrall.accuracyScore = float.Parse((totalAccuracy / actionNum).ToString("0.#"));
        trainingData.overrall.score = (int)(passPercentFactor * trainingData.overrall.passScore + accuracyFactor * trainingData.overrall.accuracyScore);
        trainingData.overrall.compliance = totalNum * 100 / currentLevel.actionNum;
        SQLiteHelper sql;
        SqliteDataReader reader;
        sql = new SQLiteHelper("data source=" + DATA.databasePath);
        OneTrainingData oldData = new OneTrainingData();
        try
        {
            string queryString = "SELECT * FROM trainingdata WHERE UserID=0 order by ID DESC limit 1";  //只返回UserID=0且ID最大的行
            reader = sql.ExecuteQuery(queryString);
            reader.Read();
            oldData.overrall = JsonHelper.DeserializeJsonToObject<OneTrainingOverrall>(reader.GetString(4));        //Json解码
            reader.Close();
        }
        catch (Exception e)
        {
            print("训练数据为空！");
        }

        trainingData.overrall.lastScore = oldData.overrall.score;
        //如果当前训练是某个阶段训练的类型，更新阶段训练数据。
        try
        {
            //queryString中的 StartTime 为疗程开始的时间
            string queryString = "SELECT * FROM periodtrainingdata WHERE UserId=" + GameData.current_user_id + " and StartTime='" + currentUser.level.StartTime + "' and TrainingType=" + trainingData.type;
            reader = sql.ExecuteQuery(queryString);
            reader.Read();
            periodData = new OnePeriodTrainingData();
            periodData.startTime = reader.GetString(2);
            periodData.type = reader.GetInt32(3);
            periodData.overview = JsonHelper.DeserializeJsonToObject<PeriodOverview>(reader.GetString(4));
            periodData.detail = JsonHelper.DeserializeJsonToObject<PeriodDetail>(reader.GetString(5));
            reader.Close();
        }
        catch (Exception e)
        {
            print("阶段训练数据为空！");
            periodData = new OnePeriodTrainingData();       //新建阶段训练数据
            string[] times = trainingData.startTime.Split('-');
            periodData.startTime = times[0] + '-' + times[1] + '-' + times[2];
            periodData.type = trainingData.type;
        }
        sql.CloseConnection();
        //完善阶段训练数据
        //完善总体数据
        string[] dateItems = trainingData.startTime.Split('-');
        string date = dateItems[0] + "-" + dateItems[1] + "-" + dateItems[2];// + "-" + dateItems[3] + "-" + dateItems[4];//到分
        if (periodData.overview.dayDatas.ContainsKey(date))
        {//总览中包括当前天的数据
            OneDayTrainingData td = periodData.overview.dayDatas[date];     //td在外部没有被使用
            td.passScore = (td.passScore * td.trainingTimes + trainingData.overrall.passScore) / (td.trainingTimes + 1);
            td.passScore = float.Parse(td.passScore.ToString("0.#"));
            td.accuracyScore = (td.accuracyScore * td.trainingTimes + trainingData.overrall.accuracyScore) / (td.trainingTimes + 1);
            td.accuracyScore = float.Parse(td.accuracyScore.ToString("0.#"));
            td.compliance = (td.compliance * td.trainingTimes + trainingData.overrall.compliance) / (td.trainingTimes + 1);
        }
        else
        {//新建一天的总体数据
            OneDayTrainingData td = new OneDayTrainingData();
            td.passScore = (td.passScore * td.trainingTimes + trainingData.overrall.passScore) / (td.trainingTimes + 1);
            td.passScore = float.Parse(td.passScore.ToString("0.#"));
            td.accuracyScore = (td.accuracyScore * td.trainingTimes + trainingData.overrall.accuracyScore) / (td.trainingTimes + 1);
            td.accuracyScore = float.Parse(td.accuracyScore.ToString("0.#"));
            td.compliance = (td.compliance * td.trainingTimes + trainingData.overrall.compliance) / (td.trainingTimes + 1);
            periodData.overview.dayDatas.Add(date, td);
        }
        //完善详细数据
        if (periodData.detail.jointDatas.Count == 0)
        {//关节数据为空，初始化关节数据和检测指标数据
            foreach (var joint in DATA.JointCheckMethod)        //遍历需要检查的关节点，为何不与上文一样用 defaultCheckJoints 读取关节点?
            {
                PeriodJointData jd = new PeriodJointData();
                foreach (var method in joint.Value)
                {
                    jd.methodDatas.Add(method, new PeriodMethodData());
                }
                periodData.detail.jointDatas.Add(joint.Key, jd);
            }
        }
        foreach (var jointData in periodData.detail.jointDatas)
        {
            //关节通过率
            if (jointData.Value.passPercent.ContainsKey(date))
            {//详细数据中当前关节包括当前天的数据
                if (jointData.Value.passPercent[date] == -1)
                {//表示该关节当天一直没有被检测过，因为做的是同一套动作，所以这个关节也一直不会被检测，值也一直是-1（不过一套动作一直不检测这个关节其实概率偏低）

                }
                else
                {

                    //合并当天的数据
                    float passpercent = jointData.Value.passPercent[date];
                    int trainingtimes = periodData.overview.dayDatas[date].trainingTimes;
                    jointData.Value.passPercent[date] = (passpercent * trainingtimes + trainingData.detail.jointDatas[jointData.Key].passPercentScore) / (trainingtimes + 1);
                    jointData.Value.passPercent[date] = float.Parse((jointData.Value.passPercent[date]).ToString("0.#"));
                }
            }
            else
            {
                //periodData.detail中没有当前天的条目，则需要新建，并传入训练结果

                if (trainingData.detail.jointDatas[jointData.Key].passPercentScore != -1)
                {//-1表示该关节在当次训练未被检测
                    jointData.Value.passPercent.Add(date, trainingData.detail.jointDatas[jointData.Key].passPercentScore);
                }
                else
                {
                    jointData.Value.passPercent.Add(date, -1);
                }
            }
            //关节通过率表现
            List<float> passPercentList = new List<float>();
            foreach (var pp in jointData.Value.passPercent)     //应为periodData.detail.passPercent的各项为按时间创建，所以隐含了先后顺序
            {
                passPercentList.Add(pp.Value);
            }
            jointData.Value.performance = linefit(passPercentList); //最小二乘法，线性拟合
            //检测指标数据
            foreach (var methodData in jointData.Value.methodDatas)
            {
                if (methodData.Value.errorPercent.ContainsKey(date))
                {//当前指标包含当天数据
                    if (methodData.Value.errorPercent[date] == -1)
                    {

                    }
                    else
                    {

                        //合并当天的数据
                        float errorpercent = methodData.Value.errorPercent[date];
                        int trainingtimes = periodData.overview.dayDatas[date].trainingTimes;
                        methodData.Value.errorPercent[date] = (errorpercent * trainingtimes + trainingData.detail.jointDatas[jointData.Key].methodDatas[methodData.Key].eps) / (trainingtimes + 1);
                        methodData.Value.errorPercent[date] = float.Parse(methodData.Value.errorPercent[date].ToString("0.#"));
                    }

                }
                else
                {
                    if (trainingData.detail.jointDatas[jointData.Key].methodDatas[methodData.Key].eps == -1)
                    {//该检测指标是无效的，不被检测
                        methodData.Value.errorPercent.Add(date, -1);
                    }
                    else
                    {
                        methodData.Value.errorPercent.Add(date, trainingData.detail.jointDatas[jointData.Key].methodDatas[methodData.Key].eps);
                    }

                }
                //指标错误率表现
                List<float> errorPercentList = new List<float>();
                foreach (var ep in methodData.Value.errorPercent)
                {
                    errorPercentList.Add(ep.Value);
                }
                methodData.Value.performance = linefit(errorPercentList);       //线性拟合
            }

        }
        periodData.overview.dayDatas[date].trainingTimes++;     //当日训练次数+1



    }
    float linefit(List<float> points)
    {
        if (points.Count == 0) return -1;
        if (points.Count == 1) return 0;
        double a, b, c;
        int size = points.Count;
        float x_mean = (size - 1.0f) / 2.0f;
        float y_mean = 0;
        for (int i = 0; i < points.Count; i++)
        {
            y_mean += points[i];

        }
        y_mean /= size;
        float Dxx = 0;
        float Dxy = 0;
        float Dyy = 0;
        for (int i = 0; i < size; i++)
        {
            Dxx += (i - x_mean) * (i - x_mean);
            Dxy += (i - x_mean) * (points[i] - y_mean);
            Dyy += (points[i] - y_mean) * (points[i] - y_mean);
            double lambda = ((Dxx + Dyy) - Math.Sqrt((Dxx - Dyy) * (Dxx - Dyy) + 4 * Dxy * Dxy)) / 2.0;
            double den = Math.Sqrt(Dxy * Dxy + (lambda - Dxx) * (lambda - Dxx));
            a = Dxy / den;
            b = (lambda - Dxx) / den;
            c = -a * x_mean - b * y_mean;
            //print("Xmean=" + x_mean + "Ymean=" + y_mean+"Dxx=" + Dxx + "Dxy=" + Dxy + "Dyy=" + Dyy+"lamda="+lambda+"den="+den+"a=" + a + "b=" + b + "c=" + c);
            return float.Parse(((float)(-a / b)).ToString("0.#"));
        }
        return 0;
    }
    IEnumerator Prompt1()
    {
        // 激活“根据提示做出穿墙动作”组件
        transform.Find("Playing/prompt1").gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        for (float i = 1; i >= 0; i = i - 0.05f)
        {
            yield return new WaitForSeconds(0.01f);
            Image[] images = transform.Find("Playing/prompt1").gameObject.GetComponentsInChildren<Image>();
            for (int j = 0; j < images.Length; j++)
            {

                images[j].color = new Color(images[j].color.r, images[j].color.g, images[j].color.b, i);
            }
            Text[] texts = transform.Find("Playing/prompt1").gameObject.GetComponentsInChildren<Text>();
            for (int j = 0; j < texts.Length; j++)
            {
                // 每0.01s不透明度减去0.05，直到变为透明
                texts[j].color = new Color(texts[j].color.r, texts[j].color.g, texts[j].color.b, i);
            }

        }
        transform.Find("Playing/prompt1").gameObject.SetActive(false);

    }
    IEnumerator Prompt2()
    {
        transform.Find("Playing/prompt2").gameObject.SetActive(true);
        AudiosManager.instance.PlayAudioEffect("levelup");
        yield return new WaitForSeconds(2f);
        for (float i = 1; i >= 0; i = i - 0.01f)
        {
            yield return new WaitForSeconds(0.01f);
            Image[] images = transform.Find("Playing/prompt2").gameObject.GetComponentsInChildren<Image>();
            for (int j = 0; j < images.Length; j++)
            {

                images[j].color = new Color(images[j].color.r, images[j].color.g, images[j].color.b, i);
            }
            Text[] texts = transform.Find("Playing/prompt2").gameObject.GetComponentsInChildren<Text>();
            for (int j = 0; j < texts.Length; j++)
            {
                texts[j].color = new Color(texts[j].color.r, texts[j].color.g, texts[j].color.b, i);
            }

        }
        transform.Find("Playing/prompt2").gameObject.SetActive(false);
    }
    IEnumerator RedScreenForNotPass()
    {
        transform.Find("Playing/redscreen").gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        transform.Find("Playing/redscreen").gameObject.SetActive(false);
    }
    IEnumerator ComboFadeInFadeOut(string degree, int num)
    {
        int shiwei = num / 10;
        int gewei = num % 10;
        switch (degree)
        {
            case "good":
                performance.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/cGOOD", typeof(Sprite)) as Sprite;
                if (num > 1)
                {
                    performanceTimes.SetActive(true);

                    // 读取对应的数字图片
                    performanceTimes.transform.Find("shiwei").gameObject.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/反馈补充/" + shiwei, typeof(Sprite)) as Sprite;
                    performanceTimes.transform.Find("gewei").gameObject.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/反馈补充/" + gewei, typeof(Sprite)) as Sprite;
                }
                break;
            case "great":
                performance.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/cGREAT", typeof(Sprite)) as Sprite;
                if (num > 1)
                {
                    performanceTimes.SetActive(true);
                    performanceTimes.transform.Find("shiwei").gameObject.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/反馈补充/" + shiwei, typeof(Sprite)) as Sprite;
                    performanceTimes.transform.Find("gewei").gameObject.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/反馈补充/" + gewei, typeof(Sprite)) as Sprite;
                }
                break;
            case "perfect":
                performance.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/cperfect", typeof(Sprite)) as Sprite;
                if (num > 1)
                {
                    performanceTimes.SetActive(true);
                    performanceTimes.transform.Find("shiwei").gameObject.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/反馈补充/" + shiwei, typeof(Sprite)) as Sprite;
                    performanceTimes.transform.Find("gewei").gameObject.GetComponent<Image>().sprite = Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/反馈补充/" + gewei, typeof(Sprite)) as Sprite;
                }
                break;

        }
        if (num > 3)
        {
            GameObject comboboom = performance.transform.Find("boom").gameObject;
            comboboom.SetActive(true);
        }
        //for (float i = 0; i <= 1; i = i + 0.05f)
        //{
        //    performance.GetComponent<Image>().color = new Color(1, 1, 1, i);
        //    performanceTimes.GetComponent<Image>().color = new Color(1, 1, 1, i);
        //    performanceTimes.transform.Find("shiwei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, i);
        //    performanceTimes.transform.Find("gewei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, i);
        //    yield return new WaitForSeconds(0.01f);
        //}
        yield return new WaitForSeconds(0.5f);
        for (float i = 1; i >= 0; i = i - 0.1f)
        {
            performance.GetComponent<Image>().color = new Color(1, 1, 1, i);
            performanceTimes.GetComponent<Image>().color = new Color(1, 1, 1, i);
            performanceTimes.transform.Find("shiwei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, i);
            performanceTimes.transform.Find("gewei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(0.01f);
        }
        performance.SetActive(false);
        performanceTimes.SetActive(false);
        performance.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        performanceTimes.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        performanceTimes.transform.Find("shiwei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        performanceTimes.transform.Find("gewei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        //performance.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        //performanceTimes.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        //performanceTimes.transform.Find("shiwei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        //performanceTimes.transform.Find("gewei").gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        if (num > 3)
        {
            GameObject comboboom = performance.transform.Find("boom").gameObject;
            comboboom.active = false;
        }
    }
    public void ClickYes()
    {
        AudiosManager.instance.PlayAudioEffect("click_button");
        transform.root.Find("Game/Over").gameObject.SetActive(false);
        state = "ready";
        StateTimes = 0;

        SceneManager.LoadScene("08-WallEvaluation");

    }
    public void ClickNo()
    {
        // 训练结束后退出游戏
        AudiosManager.instance.PlayAudioEffect("click_button");
        //transform.root.Find("Login").gameObject.SetActive(true);
        //transform.root.Find("Loading").gameObject.SetActive(true);

        //#if UNITY_EDITOR
        //    UnityEditor.EditorApplication.isPlaying = false;
        //#else
        //        Application.Quit();
        //#endif

        SceneManager.LoadScene("03-DoctorUI");
    }
    void UpdateDatabase()
    {
        OneTrainingData td = trainingData;
        if (!SaveTrainingDataDetail1) trainingData.actionDatas.Clear();     //1-false, 2-true
        if (!SaveTrainingDataDetail2)
        {
            foreach (var oneActionData in trainingData.overview.actionDatas)
            {
                oneActionData.Value.accuracyList.Clear();
            }
            foreach (var oneJointData in trainingData.detail.jointDatas)
            {
                oneJointData.Value.actionPassBool.Clear();
            }
        }
        SQLiteHelper sql;
        SqliteDataReader reader;
        sql = new SQLiteHelper("data source=" + DATA.databasePath);
        try
        {
            string queryString = "SELECT * FROM trainingdata order by id DESC limit 1";
            reader = sql.ExecuteQuery(queryString);
            reader.Read();
            int id = reader.GetInt32(0) + 1;
            sql.InsertValues("trainingdata", new string[] { "" + id,
                "" + GameData.current_user_id,
                "'" + td.startTime + "'",
                "" + td.type,
                "'" + JsonHelper.SerializeObject(td.overrall) + "'",
                "'" + JsonHelper.SerializeObject(td.overview) + "'",
                "'" + JsonHelper.SerializeObject(td.detail) + "'" });
            reader.Close();

            DoctorDataManager.instance.doctor.patient.WallEvaluations.Add(td);
        }
        catch (Exception e)
        {
            Debug.Log("训练数据为空!");
            sql.InsertValues("trainingdata", new string[] { "" + 0,
                "" + GameData.current_user_id,
                "'" + td.startTime + "'",
                "" + td.type,
                "'" + JsonHelper.SerializeObject(td.overrall) + "'",
                "'" + JsonHelper.SerializeObject(td.overview) + "'",
                "'" + JsonHelper.SerializeObject(td.detail) + "'" });

        }
        try
        {
            // 更新periodtrainingdata

            string queryString = "SELECT * FROM periodtrainingdata WHERE StartTime = " + "'" + periodData.startTime + "'";
            reader = sql.ExecuteQuery(queryString);
            if (reader.Read())
            {//表示存在当天的数据
                sql.UpdateValues("periodtrainingdata", new string[] { "Overview", "Detail" }, new string[] { "'" + JsonHelper.SerializeObject(periodData.overview) + "'", "'" + JsonHelper.SerializeObject(periodData.detail) + "'" }, "StartTime", "=", "'" + periodData.startTime + "'");
            }
            else
            {
                Debug.Log("当前用户阶段训练数据为空!");
                try
                {
                    queryString = "SELECT * FROM periodtrainingdata order by ID DESC limit 1";
                    reader = sql.ExecuteQuery(queryString);
                    reader.Read();
                    int id = reader.GetInt32(0) + 1;
                    sql.InsertValues("periodtrainingdata",
                        new string[] {
                            "" + id,
                            "" + GameData.current_user_id,
                            "'" + periodData.startTime + "'",
                            "" + periodData.type,
                            "'" + JsonHelper.SerializeObject(periodData.overview) + "'",
                            "'" + JsonHelper.SerializeObject(periodData.detail) + "'"
                        });
                    reader.Close();
                }
                catch (Exception e2)
                {
                    Debug.Log("阶段训练数据库为空!");
                    sql.InsertValues("periodtrainingdata", new string[] { "" + 0, "" + GameData.current_user_id, "'" + periodData.startTime + "'", "" + periodData.type, "'" + JsonHelper.SerializeObject(periodData.overview) + "'", "'" + JsonHelper.SerializeObject(periodData.detail) + "'" });
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("periodtrainingdata table is not exist!");
        }
        sql.CloseConnection();
    }
    public void ClickYesOverallMerit()
    {
        AudiosManager.instance.PlayAudioEffect("click_button");
        state = "over";
        over_no_time = 0;
        over_yes_time = 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">image name</param>
    /// <param name="path">image path</param>
    /// <param name="label">label==0 means load wallactionsprite,else means load remindactionsprite</param>
    /// <returns></returns>
    public IEnumerator LoadImage(int key, string path, int label)
    {
        yield return 1;
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        // 原图片大小为1000*1000

        //创建Texture
        int width = 192;
        int height = 108;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        if (label == 0)
        {
            wallactionSprite[key] = sprite;
        }
        else if (label == 1)
        {
            remindactionSprite[key] = sprite;
        }
        else
        {
            remindactionSpriteSide[key] = sprite;
        }

        // 已加载的图片数加一
        AllLoadedActionNum++;
        //if (wallactionSprite.Count + remindactionSprite.Count +remindactionSpriteSide.Count== AllLoadedActionNum)
        //{
        //    GameObject go = transform.Find("Playing").Find("loading").gameObject;
        //    go.active = false;
        //    StartCoroutine(Prompt1());
        //    state = "start";
        //    StateTimes = 0;
        //}
        //else
        //{
        //    GameObject go = transform.Find("Playing").Find("loading").gameObject;
        //    go.active = true;
        //    go.transform.Find("percent").gameObject.GetComponent<Text>().text = AllLoadedActionNum * 100 / (wallactionSprite.Count + remindactionSprite.Count + remindactionSpriteSide.Count) + "%";

        //}

    }
}
