using UnityEngine;
using UnityEngine.UI;

public class FistButton : MonoBehaviour
{       //FistButton按钮，握拳超过1s将触发

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
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue); //将对应触发按钮变大
            if (!isFocusing)
            {
                Vector3 pos = transform.position;
                pos += new Vector3(offsetValue.x, offsetValue.y, 0);                //移动变大后的按钮位置
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
        {      //// 是否人物被检测到
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
                {//左右手均握拳
                    _currentTime += Time.deltaTime;     //保证当满足触发要求保持一秒钟时，进度条刚好走完
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
                {  //"左手握拳"
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
                { //"右手握拳"
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

        UpdateProgressBarImage(_currentTime / waitingTime); //_currentTime / waitingTime=1时，进度条填充满

        if (_currentTime > waitingTime && !clickOnceLock)
        { // _currentTime 超过 waitingTime 且 没有click过时，自动触发按钮
            Click();
        }
    }

    private void UpdateProgressBarImage(float percent)
    {    //填充进度条
        if (radialProgressBarImage == null)
        {
            return;
        }

        radialProgressBarImage.fillAmount = percent;
    }

    private void Click()
    {
        button.onClick.Invoke();
        Debug.Log("Button " + button.name + " Clicked.");
        if (disableWhenTriggered)
        {
            this.enabled = false;
        }
        _currentTime = 0f;
        UpdateProgressBarImage(0f);         // 触发一次按钮之后 进度条归零
        clickOnceLock = true;
    }
}
