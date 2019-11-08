using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DoctorRegisterScript : MonoBehaviour {

    public InputField DoctorID;     // 医生账号
    public InputField DoctorPassword;    // 医生密码
    public InputField DoctorName;    // 医生姓名

    public GameObject UserNameErrorInput;   // 账号由10位以下组成
    public GameObject ErrorInformation;   // 输入内容为空
    public GameObject UserNameAlreadyExist;   // 账号已存在
    public GameObject RegisterSuccess;   // 账号注册成功

    // Use this for initialization
    void Start()
    {
        DoctorID = GameObject.Find("DoctorID/InputField").GetComponent<InputField>();    //  绑定账号
        DoctorPassword = GameObject.Find("DoctorPassword/InputField").GetComponent<InputField>();    //  绑定密码
        DoctorName = GameObject.Find("DoctorName/InputField").GetComponent<InputField>();    //  绑定姓名

        UserNameErrorInput = GameObject.Find("UserNameErrorInput");   // 绑定错误信息
        UserNameErrorInput.SetActive(false);     // 设置语句刚开始处于未激活状态

        ErrorInformation = GameObject.Find("ErrorInput");   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        UserNameAlreadyExist = GameObject.Find("UserNameAlreadyExist");   // 绑定错误信息
        UserNameAlreadyExist.SetActive(false);     // 设置语句刚开始处于未激活状态

        RegisterSuccess = GameObject.Find("RegisterSuccess");   // 绑定错误信息
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态
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
            if (long.Parse(DoctorID.text) >= 10000000000)
            {
                UserNameErrorInput.SetActive(true);
            }
            else
            {

                DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.DoctorRegister(long.Parse(DoctorID.text), DoctorName.text, DoctorPassword.text);

                if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
                {
                    //print("成功");
                    RegisterSuccess.SetActive(true);

                    StartCoroutine(DelayTime(3));
                }
                else if (RETURN == DoctorDatabaseManager.DatabaseReturn.AlreadyExist)
                {
                    UserNameAlreadyExist.SetActive(true);
                }
                else if (RETURN == DoctorDatabaseManager.DatabaseReturn.NullInput)
                {
                    ErrorInformation.SetActive(true);
                }
            }
        }
        catch (Exception e) // 抛出异常
        {
            UserNameErrorInput.SetActive(true);
        }
    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        // 如果登录成功,则进入医生管理界面
        SceneManager.LoadScene("01-DoctorLogin");
    }
}
