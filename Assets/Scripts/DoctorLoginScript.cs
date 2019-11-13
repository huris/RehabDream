using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class DoctorLoginScript : MonoBehaviour {

    public InputField DoctorID;     // 医生账号
    public InputField DoctorPassword;    // 医生密码

    public GameObject ErrorInformation;   // 账号或者密码错误显示

    // Use this for initialization
    void Start () {
        DoctorID = transform.Find("DoctorID/InputField").GetComponent<InputField>();    //  绑定账号
        DoctorPassword = transform.Find("DoctorPassword/InputField").GetComponent<InputField>();    //  绑定密码

        ErrorInformation = transform.Find("ErrorInput").gameObject;   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void LoginOnClick()
    {
        try
        {
            // 判断是否存在该用户且账号密码正确
            if (DoctorDatabaseManager.instance.DoctorLogin(long.Parse(DoctorID.text), DoctorPassword.text) == DoctorDatabaseManager.DatabaseReturn.Success)
            {
                //print("成功");
                DoctorDataManager.instance.doctor = DoctorDatabaseManager.instance.ReadDoctorInfo(long.Parse(DoctorID.text));

                DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);

                SceneManager.LoadScene("03-DoctorUI");  // 如果登录成功,则进入医生管理界面
            }
            else  // 如果账号密码不正确,则提示
            {
                ErrorInformation.SetActive(true);
            }
        }
        catch (Exception e) // 抛出异常
        {
            ErrorInformation.SetActive(true);
        }
        
    }

    public void RegisterOnClick()   // 点击注册按钮,进入医生注册界面
    {
        SceneManager.LoadScene("02-DoctorRegister");
    }
}
