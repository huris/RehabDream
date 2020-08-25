using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class KinectJointController : MonoBehaviour
    {

        public GameObject sphere;//预制体
        private KinectManager manager = null;
        private GameObject[] joints;//关节数组
        private bool isCreate = false;//用于标注骨骼点物体是否创建
        private long userID = 0;
        // Use this for initialization
        void Start()
        {
            manager = KinectManager.Instance;//初始化KinectManager对象
        }
        // Update is called once per frame
        void Update()
        {
            userID = manager.GetPrimaryUserID();//获取用户userID , 如果未获得用户返回0
            if (userID != 0)
            {
                if (!isCreate)
                {
                    joints = new GameObject[manager.GetJointCount()];
                    for (int i = 0; i < manager.GetJointCount(); i++) //创建25个关节点
                    {
                        //manager.GetParentJoint();
                        //print(manager.GetParentJoint());
                        joints[i] = Instantiate(sphere);
                    }
                    isCreate = true;
                }
                else
                {
                    for (int i = 0; i < manager.GetJointCount(); i++)
                    {
                        Vector3 vec3 = manager.GetJointKinectPosition(userID, i);
                        //print("joint " + i + vec3);//打印关节点坐标
                        joints[i].transform.position = manager.GetJointKinectPosition(userID, i);
                    }
                }
            }
        }
    }
}

