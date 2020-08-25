using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class SceneGestureController : MonoBehaviour, KinectGestures.GestureListenerInterface
    {
        private void GetCfbAndComfirm()
        {
            if (GameObject.FindGameObjectWithTag("DerectionPanel") == null)
            {
                return;
            }
            ComfirmBtn cfb = GameObject.FindGameObjectWithTag("DerectionPanel").GetComponent<ComfirmBtn>();
            if (cfb)
            {
                cfb.ComfirmBtnFunc();
            }
        }
        //private ComfirmBtn
        public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
        {
            return true;
        }

        public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
        {

            if (gesture == KinectGestures.Gestures.KickRight)//判断如果完成对姿势是双手向上
            {
                //print("踢踢右腿");
                GetCfbAndComfirm();
                return true;

            }
            if (gesture == KinectGestures.Gestures.KickLeft)
            {
                //print("踢踢左腿");
                GetCfbAndComfirm();
                return true;

            }
            if (gesture == KinectGestures.Gestures.RaiseRightHand)
            {
                //print("举起右手");
                GetCfbAndComfirm();
                return true;
            }
            if (gesture == KinectGestures.Gestures.RaiseLeftHand)
            {
                //print("举起左手");
                GetCfbAndComfirm();
                return true;
            }

            return false;
        }

        public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
        {
            //throw new System.NotImplementedException();
        }

        public void UserDetected(long userId, int userIndex)
        {
            // print("检测到用户");
            KinectManager manager = KinectManager.Instance;//初始化KinectManager对象
            manager.DetectGesture(userId, KinectGestures.Gestures.Tpose);//添加双手向上保持1秒的姿势检测
            manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftHand);
            manager.DetectGesture(userId, KinectGestures.Gestures.RaiseRightHand);
            manager.DetectGesture(userId, KinectGestures.Gestures.KickLeft);
            manager.DetectGesture(userId, KinectGestures.Gestures.KickRight);
        }

        public void UserLost(long userId, int userIndex)
        {
            // print("丢失用户");
        }
    }
}

