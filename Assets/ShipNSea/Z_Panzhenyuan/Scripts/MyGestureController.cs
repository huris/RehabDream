using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ShipNSea 
{
    public class MyGestureController : MonoBehaviour
    {
        public int playerIndex = 0;
        public GameObject comfirmBtnGO;
        private ComfirmBtn cfb;
        private SceneGestureController scl;
        public AvatarController playerAvatarController;
        public void Start()
        {
            KinectManager manager = KinectManager.Instance;//初始化KinectManager对象
            manager.refreshAvatarControllers();
            if (!manager.avatarControllers.Contains(playerAvatarController))
            {
                //Debug.LogError(1111);
                manager.avatarControllers.Add(playerAvatarController);
            }
        }
    }
}

