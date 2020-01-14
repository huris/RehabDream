using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.EventSystems;

public class DoctorRegisterScript : MonoBehaviour {

    public InputField DoctorID;     // 医生账号
    public InputField DoctorPassword;    // 医生密码
    public InputField DoctorPasswordAgain;    // 医生密码确认
    public InputField DoctorName;    // 医生姓名
    // public long TryDoctorID;

    public GameObject ErrorInformation;   // 输入内容为空
    public GameObject RegisterSuccess;   // 账号注册成功
    public GameObject DoctorPasswordIsNotSame;  // 两次密码输入不一致
    public GameObject DoctorIDisAlreadyExist;   // 注册工号已经存在

    public EventSystem system;

    private Selectable SelecInput;   // 当前焦点所处的Input
    private Selectable NextInput;   // 目标Input

    public bool EnterKeyIsAlreadyInput = false;  // 判断回车是否已经被按下，防止连续两次输入回车出问题

    // Use this for initialization
    void OnEnable()
    {

        DoctorID = transform.Find("DoctorID/InputField").GetComponent<InputField>();    //  绑定账号
        DoctorPassword = transform.Find("DoctorPassword/InputField").GetComponent<InputField>();    //  绑定密码
        DoctorPasswordAgain = transform.Find("DoctorPasswordAgain/InputField").GetComponent<InputField>();    //  绑定密码
        DoctorName = transform.Find("DoctorName/InputField").GetComponent<InputField>();    //  绑定姓名

        ErrorInformation = transform.Find("ErrorInput").gameObject;   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        RegisterSuccess = transform.Find("RegisterSuccess").gameObject;   // 绑定成功信息
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态

        DoctorPasswordIsNotSame = transform.Find("DoctorPasswordIsNotSame").gameObject;  // 绑定错误信息
        DoctorPasswordIsNotSame.SetActive(false);

        DoctorIDisAlreadyExist = transform.Find("DoctorIDisAlreadyExist").gameObject; // 绑定错误信息
        DoctorIDisAlreadyExist.SetActive(false);

        //TryDoctorID = GetRandom(1000, 9999);  // 患者账号为6位
        //while (DoctorDatabaseManager.instance.CheckDoctor(TryDoctorID) != DoctorDatabaseManager.DatabaseReturn.Success)
        //{
        //    TryDoctorID = GetRandom(1000, 9999);  // 直到找到一个不重复DoctorID
        //}
        //DoctorID.text = TryDoctorID.ToString();

        system = EventSystem.current;       // 获取当前的事件
    }

    // Update is called once per frame
    void Update()
    {
        //在Update内监听Tap键的按下
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //是否聚焦Input
            if (system.currentSelectedGameObject != null)
            {
                //获取当前选中的Input
                SelecInput = system.currentSelectedGameObject.GetComponent<Selectable>();
                //监听Shift
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    //Shift按下则选择出去上方的Input
                    NextInput = SelecInput.FindSelectableOnUp();
                    //上边没有找左边的
                    if (NextInput == null) NextInput = SelecInput.FindSelectableOnLeft();
                }
                else
                {
                    //没按shift就找下边的Input
                    NextInput = SelecInput.FindSelectableOnDown();
                    //或者右边的
                    if (NextInput == null) NextInput = SelecInput.FindSelectableOnRight();
                }
            }

            //下一个Input不空的话就聚焦
            if (NextInput != null) NextInput.Select();
        }

        // 按回车键进行登录
        if (Input.GetKeyDown(KeyCode.Return) && EnterKeyIsAlreadyInput == false)
        {
            EnterKeyIsAlreadyInput = true;
            RegisterOnClick();
        }
    }

    public void RegisterOnClick()   // 点击注册按钮,进入医生注册界面
    {
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态
        DoctorPasswordIsNotSame.SetActive(false);
        DoctorIDisAlreadyExist.SetActive(false);

        try
        {
            if(DoctorPassword.text != DoctorPasswordAgain.text)
            {
                DoctorPasswordIsNotSame.SetActive(true);
            }
            else
            {
                if(DoctorDatabaseManager.instance.CheckDoctor(long.Parse(DoctorID.text)) == DoctorDatabaseManager.DatabaseReturn.Success)
                {
                    //Doctor doctor = new Doctor();
                    //doctor.SetDoctorMessage(long.Parse(DoctorID.text), MD5Encrypt(DoctorPassword.text), DoctorName.text);

                    Doctor doctor = new Doctor(long.Parse(DoctorID.text), MD5Encrypt(DoctorPassword.text), DoctorName.text);

                    DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.DoctorRegister(doctor);

                    if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
                    {
                        //print("成功");
                        RegisterSuccess.SetActive(true);

                        StartCoroutine(DelayTime(3));
                    }
                    else if (RETURN == DoctorDatabaseManager.DatabaseReturn.NullInput)
                    {
                        ErrorInformation.SetActive(true);
                    }
                }
                else
                {
                    DoctorIDisAlreadyExist.SetActive(true);
                }
            }
        }
        catch (Exception e) // 抛出异常
        {
            ErrorInformation.SetActive(true);
        }
    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        EnterKeyIsAlreadyInput = false;   // 需要改回来

        // 如果登录成功,则进入医生管理界面
        SceneManager.LoadScene("01-DoctorLogin");
    }

    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <param name="minVal">最小值（包含）</param>
    /// <param name="maxVal">最大值（不包含）</param>
    /// <returns></returns>
    public static long GetRandom(long minVal, long maxVal)
    {
        //这样产生0 ~ 100的强随机数（不含100）
        long m = maxVal - minVal;
        long rnd = long.MinValue;
        decimal _base = (decimal)long.MaxValue;
        byte[] rndSeries = new byte[8];
        System.Security.Cryptography.RNGCryptoServiceProvider rng
        = new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(rndSeries);
        long l = BitConverter.ToInt64(rndSeries, 0);
        rnd = (long)(Math.Abs(l) / _base * m);
        return minVal + rnd;
    }

    /// <summary>
    /// 用MD5加密字符串
    /// </summary>
    /// <param name="password">待加密的字符串</param>
    /// <returns></returns>
    public string MD5Encrypt(string password)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes;
        hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));
        }
        return tmp.ToString();
    }
}
