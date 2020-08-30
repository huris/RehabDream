using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoading : MonoBehaviour {
    float constant_time = 2f;
    List<float> time;
    public GameObject progressbar;      // 识别人物进度条
    public AvatarController avatar;

	// Use this for initialization
	void Start () {
        time = new List<float>();
        //Debug.Log(GameData.user_info.Count);
        //GameData.current_user_id = GameData.user_info[0].ID;
        //Debug.Log("ID" + GameData.user_info[0].ID.ToString());

        Debug.Log(GameData.user_info.Count);
        GameData.current_user_id = (int)PatientDataManager.instance.PatientID;
        Debug.Log("ID" + GameData.user_info[0].ID.ToString());
    }
    private void OnEnable()
    {
        
    }
    // Update is called once per frame
    void Update () {
        if (!KinectManager.Instance.IsUserDetected()) return;
        else
        {
            if (time.Count < KinectManager.Instance.GetUsersCount())
            {
                for (int i = time.Count; i < KinectManager.Instance.GetUsersCount(); i++)
                {
                    time.Add(0f);       //为每一个识别到的人物创建一个计时器
                }
            }
        }


        // 保证切换场景时kinect 与 AvatarController不会失效
        avatar.playerId = KinectManager.Instance.GetPrimaryUserID();
        if (!KinectManager.Instance.avatarControllers.Contains(avatar))
        {
            KinectManager.Instance.avatarControllers.Add(avatar);
        }

        for (int i=0;i<KinectManager.Instance.GetUsersCount();i++)
        {
            long userId = KinectManager.Instance.GetUserIdByIndex(i);
            Vector3 handtipleft = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandTipLeft);
            Vector3 handtipright = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.HandTipRight);
            Vector3 spineshoulder = KinectManager.Instance.GetJointKinectPosition(userId, (int)KinectInterop.JointType.SpineShoulder);

            if (KinectManager.Instance.GetJointCount() > 20)        //识别到了超过20个关节点
            {

                //Debug.Log(0);
                time[i] += Time.deltaTime;

                //Debug.Log(time[i]);

                //显示进度条
                if (time[i] < constant_time / 4)
                {
                    progressbar.transform.GetChild(0).gameObject.SetActive(false);
                    progressbar.transform.GetChild(1).gameObject.SetActive(false);
                    progressbar.transform.GetChild(2).gameObject.SetActive(false);
                    progressbar.transform.GetChild(3).gameObject.SetActive(false);
                }
                else if (time[i] >= constant_time / 4 && time[i] < constant_time / 2)
                {
                    progressbar.transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (time[i] >= constant_time / 2 && time[i] < constant_time / 4 * 3)
                {
                    progressbar.transform.GetChild(0).gameObject.SetActive(true);
                    progressbar.transform.GetChild(1).gameObject.SetActive(true);
                }
                else if (time[i] >= constant_time / 4 * 3 && time[i] < constant_time)
                {
                    progressbar.transform.GetChild(0).gameObject.SetActive(true);
                    progressbar.transform.GetChild(1).gameObject.SetActive(true);
                    progressbar.transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    progressbar.transform.GetChild(0).gameObject.SetActive(true);
                    progressbar.transform.GetChild(1).gameObject.SetActive(true);
                    progressbar.transform.GetChild(2).gameObject.SetActive(true);
                    progressbar.transform.GetChild(3).gameObject.SetActive(true);
                    GameData.normalization_param = Vector3.Distance(handtipleft, handtipright);     //记录下本次测试条件下的距离参数
                    //print(transform.parent.ToString());
                    KinectManager.Instance.SetPrimaryUserID(userId);
                    transform.root.Find("Loading").gameObject.SetActive(false);
                    transform.root.Find("Game").gameObject.SetActive(true);
                    transform.root.Find("Game").gameObject.GetComponent<PlayGame>().userId = userId;
                }
            }
            else
            {
                time[i] = 0;
            }
        }
        
    }
}
