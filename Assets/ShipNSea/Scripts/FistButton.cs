using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ShipNSea 
{
    public class FistButton : MonoBehaviour
    {

        public enum FistMode { LeftHand, RightHand, Both }

        public Button button;
        public FistMode triggerMode;
        public float waitingTime = 1f;
        public Image radialProgressBarImage;
        public bool disableWhenTriggered = true;
        [HideInInspector]
        public float scaleValue = 1.2f;
        public Vector2 offsetValue;


        public float WaitProgress { get { return _currentTime / waitingTime; } }

        private float _currentTime = 0f;
        private bool clickOnceLock = false;
        private bool isFocusing = false;

        void Start()
        {

        }

        // Run inside Update()
        private void Focus(bool isFocus)
        {
            if (isFocus)
            {
                transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                if (!isFocusing)
                {
                    Vector3 pos = transform.position;
                    pos += new Vector3(offsetValue.x, offsetValue.y, 0);
                    transform.position = pos;
                    isFocusing = true;
                }
            }
            else
            {
                transform.localScale = Vector3.one;
                if (isFocusing)
                {
                    Vector3 pos = transform.position;
                    pos -= new Vector3(offsetValue.x, offsetValue.y, 0);
                    transform.position = pos;
                    isFocusing = false;
                }
            }
        }

        void Update()
        {

            KinectManager kinectManager = KinectManager.Instance;

            long userId = kinectManager.GetPrimaryUserID();

            if (userId == 0)
            {
                _currentTime = 0;
                return;
            }

            var leftHandState = kinectManager.GetLeftHandState(userId);
            var rightHandState = kinectManager.GetRightHandState(userId);

            //this.log(leftHandState.ToString() + "  " + rightHandState.ToString());

            switch (triggerMode)
            {
                case FistMode.Both:
                    if (leftHandState == KinectInterop.HandState.Closed && rightHandState == KinectInterop.HandState.Closed)
                    {
                        _currentTime += Time.deltaTime;
                        Focus(true);
                    }
                    else
                    {
                        _currentTime = 0f;
                        Focus(false);
                        clickOnceLock = false;
                    }
                    break;
                case FistMode.LeftHand:
                    if (leftHandState == KinectInterop.HandState.Closed)
                    {
                        _currentTime += Time.deltaTime;
                        Focus(true);
                    }
                    else
                    {
                        _currentTime = 0f;
                        Focus(false);
                        clickOnceLock = false;
                    }
                    break;
                case FistMode.RightHand:
                    if (rightHandState == KinectInterop.HandState.Closed)
                    {
                        Focus(true);
                        _currentTime += Time.deltaTime;
                    }
                    else
                    {
                        Focus(false);
                        _currentTime = 0f;
                        clickOnceLock = false;
                    }
                    break;
            }

            UpdateProgressBarImage(_currentTime / waitingTime);

            if (_currentTime > waitingTime && !clickOnceLock)
            {
                Click();
            }
        }

        private void UpdateProgressBarImage(float percent)
        {
            if (radialProgressBarImage == null)
            {
                return;
            }

            radialProgressBarImage.fillAmount = percent;
        }

        private void Click()
        {
            button.onClick.Invoke();
            //print("Button " + button.name + " Clicked.");
            if (disableWhenTriggered)
            {
                this.enabled = false;
            }
            _currentTime = 0f;
            UpdateProgressBarImage(0f);
            clickOnceLock = true;
        }
    }

}
