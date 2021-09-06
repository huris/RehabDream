using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogin : MonoBehaviour {
    long userId=0;
    public InputField username;
    public InputField pwd;
    public Sprite NotChange;
    public Sprite Change;
    public Image circle;
    List<Sprite> sprites;
    float time=0;
    float CacheTime = 2f;
    float canceltime = 0;
    float savetime = 0;
    public Image yesCircle;
    public Image noCircle;
    // Use this for initialization
    void Start () {
        username.text = "";
        pwd.text = "";
        Load();
        username.onValueChanged.AddListener(delegate {
            username.image.sprite = Change;
            GameObject parent = GameObject.Find("board");
            GameObject wusername = parent.transform.Find("wrongusername").gameObject;
            if (wusername.activeInHierarchy == true)
            {
                wusername.SetActive(false);      
            }
        });
        pwd.onValueChanged.AddListener(delegate {
            pwd.image.sprite = Change;
            GameObject parent = GameObject.Find("board");
            GameObject wuserpwd = parent.transform.Find("wronguserpwd").gameObject;
            if (wuserpwd.activeInHierarchy == true)
            {
                wuserpwd.SetActive(false);
            }
        });
    }
    void Load()
    {
        username.image.sprite = NotChange;
        pwd.image.sprite = NotChange;
    }
    private void OnDisable()
    {
        ModelManager.isCheckTime = true;
    }
    private void OnEnable()
    {
        Load();
        ModelManager.isCheckTime = false;
    }
    // Update is called once per frame
    void Update () {

        Debug.Log(transform.Find("CompleteTraining")==null);
		if (transform.Find("CompleteTraining").gameObject.activeInHierarchy == true)
        {
            userId = KinectManager.Instance.GetPrimaryUserID();
            if (KinectManager.Instance.GetRightHandState(userId) == KinectInterop.HandState.Closed)
            {
                float tmpTime = time;
                time += Time.deltaTime;
                if (time >= CacheTime)
                {
                    Back();
                }
                else
                {
                    if ((int)(tmpTime * 12 / CacheTime) != (int)(time * 12 / CacheTime))
                    {
                        circle.sprite = sprites[(int)(time * 12 / CacheTime)];
                    }
                }
            }
            else
            {
                time = 0;
                circle.sprite = sprites[0];
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (transform.Find("quit").gameObject.activeInHierarchy == true)
                {
                    transform.Find("quit").gameObject.SetActive(false);
                    
                }
                else
                {
                    transform.Find("quit").gameObject.SetActive(true);
                    canceltime = 0;
                    savetime = 0;
                }
            }
            if (transform.Find("quit").gameObject.activeInHierarchy == true)
            {
                if (sprites==null||sprites.Count==0)
                {
                    sprites = new List<Sprite>();
                    for (int i = 0; i < 12; i++)
                    {
                        sprites.Add(Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/完成阶段训练/" + (i + 1), typeof(Sprite)) as Sprite);
                    }
                }
                userId = KinectManager.Instance.GetPrimaryUserID();
                if (KinectManager.Instance.GetRightHandState(userId) == KinectInterop.HandState.Closed)
                {
                    canceltime = 0;
                    noCircle.sprite = sprites[0];
                    float tmpTime = savetime;
                    savetime += Time.deltaTime;
                    if (savetime >= CacheTime)
                    {
                        Quit_yes();


                    }
                    else
                    {
                        if ((int)(tmpTime * 12 / CacheTime) != (int)(savetime * 12 / CacheTime))
                        {
                            yesCircle.sprite = sprites[(int)(savetime * 12 / CacheTime)];
                        }
                    }
                }
                else if (KinectManager.Instance.GetLeftHandState(userId) == KinectInterop.HandState.Closed)
                {
                    savetime = 0;
                    yesCircle.sprite = sprites[0];
                    float tmpTime = canceltime;
                    canceltime += Time.deltaTime;
                    if (canceltime >= CacheTime)
                    {
                        Quit_no();

                    }
                    else
                    {
                        if ((int)(tmpTime * 12 / CacheTime) != (int)(canceltime * 12 / CacheTime))
                        {
                            noCircle.sprite = sprites[(int)(canceltime * 12 / CacheTime)];
                        }
                    }
                }
                else
                {
                    canceltime = 0;
                    savetime = 0;
                    yesCircle.sprite = sprites[0];
                    noCircle.sprite = sprites[0];
                }
            }
        }
	}
    public void quitSystem()
    {
        if (transform.Find("quit").gameObject.activeInHierarchy == false)
        {
            transform.Find("quit").gameObject.SetActive(true);
        }
    }
    public void Click()
    {
        AudiosManager.instance.PlayAudioEffect("click_button");
        int flag = 0;
        for (int i=0;i<GameData.user_info.Count;i++)
        {
            if (GameData.user_info[i].name.CompareTo(username.text)==0)
            {
                flag = 1;
                if (GameData.user_info[i].pwd.CompareTo(pwd.text)==0)
                {
                    flag = 2;
                    GameData.current_user_id = GameData.user_info[i].ID;
                    print("starttime:" + GameData.user_info[GameData.current_user_id].level.StartTime);
                    string[] dates = GameData.user_info[GameData.current_user_id].level.StartTime.Split('-');
                    for(int j = 0; j < dates.Length; j++)
                    {
                        print("dates" + dates[j]);
                    }
                    DateTime date1 = new DateTime(int.Parse(dates[0]), int.Parse(dates[1]), int.Parse(dates[2]));
                    DateTime date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day);
                    //print(date2.Subtract(date1).Duration().Days);

                    // 当前时间-开始时间 > 训练疗程
                    if (date2.Subtract(date1).Duration().Days> GameData.user_info[GameData.current_user_id].level.trainingDays)
                    {
                        CompleteTraining();
                    }
                    else
                    {
                        transform.root.Find("Loading").gameObject.SetActive(true);
                        transform.root.Find("Login").gameObject.SetActive(false);
                    }


                    break;
                }
            }
        }
        if (flag == 0)
        {
            GameObject parent = GameObject.Find("board");
            GameObject wusername = parent.transform.Find("wrongusername").gameObject;         
            wusername.SetActive(true);      

        }
        if (flag == 1)
        {
            GameObject parent = GameObject.Find("board");
            GameObject wuserpwd = parent.transform.Find("wronguserpwd").gameObject;
            wuserpwd.active = true; 
        }
    }
    void CompleteTraining()
    {
        transform.Find("CompleteTraining").gameObject.SetActive(true);
        sprites = new List<Sprite>();
        for (int i=0;i<12;i++)
        {
            sprites.Add(Resources.Load("chuanqiangfiles/chuanqiangimages/5.7/完成阶段训练/"+(i+1), typeof(Sprite)) as Sprite);
        }
        circle.sprite= sprites[0];
        time = 0;

    }
    public void Back()
    {
        transform.Find("CompleteTraining").gameObject.SetActive(false);
        
    }
    public void Quit_yes()
    {
        Application.Quit();
    }
    public void Quit_no()
    {
        transform.Find("quit").gameObject.SetActive(false);
    }
}
