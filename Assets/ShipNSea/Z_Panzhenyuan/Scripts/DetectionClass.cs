using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ShipNSea 
{
    public class DetectionClass : MonoBehaviour
    {

        public int playerIndex = 0;
        private KinectManager manager = null;
        private long userID = 0;
        public Image[] imageArrs = new Image[25];
        public Transform[] imageTransform = new Transform[25];

        private void Start()
        {
            manager = KinectManager.Instance;//初始化KinectManager对象
        }
        // Update is called once per frame
        void Update()
        {
            userID = manager.GetPrimaryUserID();//获取用户userID , 如果未获得用户返回0
            if (userID != 0)
            {
                //Debug.LogError(manager.GetJointPosition(userID, 10));
                for (int i = 0; i < 25; i++)
                {
                    if (manager.GetJointPosition(userID, i) == Vector3.zero)
                    {
                        imageArrs[i].color = Color.red;
                    }
                    else
                    {
                        imageArrs[i].color = Color.green;
                    }
                }

                for (int i = 0; i < manager.GetJointCount(); i++)
                {
                    Vector3 vec3 = manager.GetJointKinectPosition(userID, i);
                    Vector3 newVec3 = new Vector3(vec3.x * 300 + 640, vec3.y * 300 + 360, 0);
                    //print("joint " + i + vec3);//打印关节点坐标
                    imageTransform[i].transform.position = newVec3;
                }
            }
        }
    }
}

