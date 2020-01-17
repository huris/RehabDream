using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class DoctorTestScript : MonoBehaviour {

    public RawImage image; // 空白图片，用于显示

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 判断设备是否初始化完成。最开始的几帧没有完成，所以需要在Update中去每帧检测。
        bool isInit = KinectManager.IsKinectInitialized();
        if (isInit)
        {
            if (image.texture == null)
            {
                // 从设备获取彩色数据，需要勾选Compute Color Map。
                Texture2D colorMap = KinectManager.Instance.GetUsersClrTex();
                // 从设备获取深度数据，需要勾选Compute User Map。
                // Texture2D userMap = KinectManager.Instance.GetUsersLblTex ();
                // 把彩色数据设置给控件显示
                image.texture = colorMap;
            }
        }

    }
}
