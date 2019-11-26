/* ============================================================================== 
* ClassName：PatientGestureListener 
* Author：ChenShuwei 
* CreateDate：2019/11/20 16:56:54 
* Version: 1.0
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PatientGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface{


    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("GUI-Text to display gesture-listener messages and gesture information.")]
    public GUIText gestureInfo;

    // singleton instance of the class
    private static PatientGestureListener instance = null;

    // internal variables to track if progress message has been displayed
    private bool progressDisplayed;
    private float progressGestureTime;
    private float _TposeLastTime=0;

    // whether the needed gesture has been detected or not
    private bool _Tpose;



    /// <summary>
    /// Gets the singleton CubeGestureListener instance.
    /// </summary>
    /// <value>The CubeGestureListener instance.</value>
    public static PatientGestureListener Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Determines whether Tpose is detected.
    /// </summary>
    /// <returns><c>true</c> if Tpose is detected; otherwise, <c>false</c>.</returns>
    public bool IsTpose()
    {
        if (_Tpose)
        {
            _Tpose = false;
            return true;
        }

        return false;
    }



    /// <summary>
    /// Invoked when a new user is detected. Here you can start gesture tracking by invoking KinectManager.DetectGesture()-function.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserDetected(long userId, int userIndex)
    {
        // the gestures are allowed for the primary user only
        KinectManager manager = KinectManager.Instance;
        if (!manager || (userIndex != playerIndex))
            return;

        // detect these user specific gestures
        manager.DetectGesture(userId, KinectGestures.Gestures.Tpose);

        Debug.Log("@PatientGestureListener: UserDetected");
    }

    /// <summary>
    /// Invoked when a user gets lost. All tracked gestures for this user are cleared automatically.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserLost(long userId, int userIndex)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
            return;

        Debug.Log("@PatientGestureListener: UserLost");
    }

    /// <summary>
    /// Invoked when a gesture is in progress.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    /// <param name="gesture">Gesture type</param>
    /// <param name="progress">Gesture progress [0..1]</param>
    /// <param name="joint">Joint type</param>
    /// <param name="screenPos">Normalized viewport position</param>
    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
            return;

        if ((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
        {
            if (gestureInfo != null)
            {
                //string sGestureText = string.Format("{0} - {1:F0}%", gesture, screenPos.z * 100f);
                //gestureInfo.text = sGestureText;

                progressDisplayed = true;
                progressGestureTime = Time.realtimeSinceStartup;
            }
        }
        else if ((gesture == KinectGestures.Gestures.Wheel || gesture == KinectGestures.Gestures.LeanLeft ||
                 gesture == KinectGestures.Gestures.LeanRight) && progress > 0.5f)
        {
            if (gestureInfo != null)
            {
                //string sGestureText = string.Format("{0} - {1:F0} degrees", gesture, screenPos.z);
                //gestureInfo.text = sGestureText;

                progressDisplayed = true;
                progressGestureTime = Time.realtimeSinceStartup;
            }
        }
        else if (gesture == KinectGestures.Gestures.Run && progress > 0.5f)
        {
            if (gestureInfo != null)
            {
                //string sGestureText = string.Format("{0} - progress: {1:F0}%", gesture, progress * 100);
                //gestureInfo.text = sGestureText;

                progressDisplayed = true;
                progressGestureTime = Time.realtimeSinceStartup;
            }
        }
    }

    /// <summary>
    /// Invoked if a gesture is completed.
    /// </summary>
    /// <returns>true</returns>
    /// <c>false</c>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    /// <param name="gesture">Gesture type</param>
    /// <param name="joint">Joint type</param>
    /// <param name="screenPos">Normalized viewport position</param>
    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint, Vector3 screenPos)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
        {
            _TposeLastTime = 0;
            Debug.Log("@PatientGestureListener: GestureCompleted Fail");
            return false;
        }

        if (gesture == KinectGestures.Gestures.Tpose)
        {
            _Tpose = true;
            _TposeLastTime += 1.0f;
            Debug.Log("_TposeLastTime: " + _TposeLastTime);
            Debug.Log("@PatientGestureListener: GestureCompleted");
        }
            
        return true;
        
    }

    /// <summary>
    /// Invoked if a gesture is cancelled.
    /// </summary>
    /// <returns>true</returns>
    /// <c>false</c>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    /// <param name="gesture">Gesture type</param>
    /// <param name="joint">Joint type</param>
    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint)
    {
        // the gestures are allowed for the primary user only
        if (userIndex != playerIndex)
            return false;

        if (progressDisplayed)
        {
            progressDisplayed = false;

            //if (gestureInfo != null)
            //{
            //    gestureInfo.text = String.Empty;
            //}
        }

        return true;
    }


    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
        {
            progressDisplayed = false;
            gestureInfo.text = String.Empty;

            //Debug.Log("Forced progress to end.");
        }
    }


    public bool TposeContinue(float Time)
    {
        if (_TposeLastTime < Time)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetTposeLastTime()
    {
        return _TposeLastTime;
    }

    public void ResetTposeLastTime()
    {
        _TposeLastTime = 0;
    }
}

