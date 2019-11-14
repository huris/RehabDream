using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Security.Cryptography;
using System.Text;

public class DoctorRegisterScript : MonoBehaviour {

    public Text DoctorID;     // 医生账号
    public InputField DoctorPassword;    // 医生密码
    public InputField DoctorName;    // 医生姓名
    public long TryDoctorID;

    public GameObject ErrorInformation;   // 输入内容为空
    public GameObject RegisterSuccess;   // 账号注册成功

    // Use this for initialization
    void OnEnable()
    {
        DoctorID = transform.Find("DoctorID/DoctorIDItem").GetComponent<Text>();    //  绑定账号
        DoctorPassword = transform.Find("DoctorPassword/InputField").GetComponent<InputField>();    //  绑定密码
        DoctorName = transform.Find("DoctorName/InputField").GetComponent<InputField>();    //  绑定姓名

        ErrorInformation = transform.Find("ErrorInput").gameObject;   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        RegisterSuccess = transform.Find("RegisterSuccess").gameObject;   // 绑定错误信息
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态


        TryDoctorID = GetRandom(1000, 9999);  // 患者账号为6位
        while (DoctorDatabaseManager.instance.CheckDoctor(TryDoctorID) != DoctorDatabaseManager.DatabaseReturn.Success)
        {
            TryDoctorID = GetRandom(1000, 9999);  // 直到找到一个不重复DoctorID
        }
        DoctorID.text = TryDoctorID.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //print(instance.DoctorID);
    }

    public void RegisterOnClick()   // 点击注册按钮,进入医生注册界面
    {
        try
        {
            ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态
            RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态

            Doctor doctor = new Doctor();
            doctor.SetDoctorMessage(TryDoctorID, MD5Encrypt(DoctorPassword.text), DoctorName.text);

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
        catch (Exception e) // 抛出异常
        {
            ErrorInformation.SetActive(true);
        }
    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

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
