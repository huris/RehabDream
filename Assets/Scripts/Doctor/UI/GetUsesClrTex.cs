using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetUsesClrTex : MonoBehaviour
{
    public bool isAngetKinect;
    // 获取人的彩色图
    public RawImage UserTexture;
    // 获取人的红外线图
    public RawImage UsersLblTex;

    void Update()
    {
        //判断设备是否准备好
        isAngetKinect = KinectManager.Instance.IsInitialized();

        if (isAngetKinect)
        {
            if (UserTexture.texture == null)
            {
                //获取彩色数据
                Texture2D texture = KinectManager.Instance.GetUsersClrTex();
                UserTexture.texture = texture;

                //获取深度数据
                Texture2D texture1 = KinectManager.Instance.GetUsersLblTex();
                UsersLblTex.texture = texture1;
            }
            //判断是否检测到玩家
            if (KinectManager.Instance.IsUserDetected())
            {
                //获取玩家的ID
                long userId = KinectManager.Instance.GetPrimaryUserID();
                //获取右手
                int jointType = (int)KinectInterop.JointType.HandRight;
                //获取左手
                int LeftType = (int)KinectInterop.JointType.HandLeft;

                //KinectInterop.JointType 可以获取人物身上的任意骨骼点需要什么直接获取就行

                //追踪到关节点
                if (KinectManager.Instance.IsJointTracked(userId, jointType))
                {
                    //获取右手位置
                    Vector3 rightHandpos = KinectManager.Instance.GetJointKinectPosition(userId, jointType);
                    Debug.Log("----------------->" + rightHandpos);
                }
            }
        }
    }
}