using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DoctorTestScript: MonoBehaviour
{
    public RawImage kinectImg;

    void Update()
    {
        // 判断 Kinect 设备是否初始化完成
        bool isInit = KinectManager.Instance.IsInitialized();
        if (isInit)
        {
            // 设备准备好,可以读取了
            // 彩色数据
            // Texture2D kinectPic =   KinectManager.Instance.GetUsersClrTex();// 从设备获取彩色数据
            //if (kinectImg.texture == null)
            //{
            //    kinectImg.texture = kinectPic;// 把彩色数据给控件显示
            //}
            // 深度数据
            Texture2D kinectUseMap = KinectManager.Instance.GetUsersLblTex();
            if (kinectImg.texture == null)
            {
                kinectImg.texture = kinectUseMap;// 把彩色数据给控件显示
            }
            // 检测到玩家了
            if (KinectManager.Instance.IsUserDetected())
            {
                // 获取用户 ID
                long userId = KinectManager.Instance.GetPrimaryUserID();
                Vector3 userPos = KinectManager.Instance.GetUserPosition(userId);
                print(userPos);
                // print("用户的位置" + userPos);
                int jointType = (int)KinectInterop.JointType.HandLeft;
                // 关节点是否被追踪到了
                if (KinectManager.Instance.IsJointTracked(userId, jointType))
                {
                    // 关节点的 Vector3 的向量数据
                    // 下面两个方法传的参数都是一样的,有什么区别呢,区别在于 y 值 ,
                    // GetJointKinectPosition计算了KinectManager下面的 Sensor Height属性,人相对与摄像机的y值的位置,
                    // GetJointPosition 则是把摄像机高度当成是 0 的位置获取的关节点的位置 
                    Vector3 leftHandKinectPos = KinectManager.Instance.GetJointKinectPosition(userId, jointType);// 获取左手信息
                    Vector3 leftHandPos = KinectManager.Instance.GetJointPosition(userId, jointType);
                    //  Debug.Log("kx=" + leftHandKinectPos.x + "ky=" + leftHandKinectPos.y + "kz=" + leftHandKinectPos.z + "/n<-------->" + "x=" + leftHandPos.x + "y=" + leftHandPos.y + "z=" + leftHandPos.z);

                    KinectInterop.HandState leftHandState = KinectManager.Instance.GetLeftHandState(userId);
                    //if (leftHandState == KinectInterop.HandState.Closed)
                    //{
                    //    print("_>" + "左手握拳");
                    //}
                    //else if (leftHandState == KinectInterop.HandState.Open)
                    //{
                    //    print("_>" + "左手张开");
                    //}
                    //else if (leftHandState == KinectInterop.HandState.Lasso)
                    //{
                    //    print("_>" + "yes 手势");
                    //}
                }
            }


        }
    }
}