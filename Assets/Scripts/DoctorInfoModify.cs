using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class DoctorInfoModify : MonoBehaviour {

    public InputField OldPassword;    // 医生旧密码
    public InputField NewPassword;    // 医生新密码

    public GameObject OldPasswordError;   // 旧密码错误
    public GameObject NewPasswordError;   // 新密码错误 
    public GameObject ModifyPasswordSuccess;   // 修改信息成功

    // use this for initialization
    void Start()
    {
        OldPassword = GameObject.Find("OldPassword/InputField").GetComponent<InputField>();    //  绑定旧密码
        NewPassword = GameObject.Find("NewPassword/InputField").GetComponent<InputField>();    //  绑定新密码

        OldPasswordError = GameObject.Find("OldPasswordError");    //  旧密码绑定
        OldPasswordError.SetActive(false);

        NewPasswordError = GameObject.Find("NewPasswordError");    //  新密码绑定
        NewPasswordError.SetActive(false);

        ModifyPasswordSuccess = GameObject.Find("ModifyPasswordSuccess");    //  修改密码成功绑定
        ModifyPasswordSuccess.SetActive(false);
    }


    // update is called once per frame
    void Update()
    {
    }

    public void ModifyOnclick()   // 点击修改按钮
    {
        try
        {
            if (MD5Encrypt(OldPassword.text) == DoctorDataManager.instance.doctor.DoctorPassword)
            {
          
                DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.DoctorModify(DoctorDataManager.instance.doctor.DoctorID, DoctorDataManager.instance.doctor.DoctorName, NewPassword.text);

                if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
                {
                    ModifyPasswordSuccess.SetActive(true);

                    StartCoroutine(DelayTime(3));
                }
                else if(RETURN == DoctorDatabaseManager.DatabaseReturn.Fail)
                {
                    NewPasswordError.SetActive(true);
                }
            }
        }
        catch (Exception e) // 抛出异常
        {
            NewPasswordError.SetActive(true);
        }
    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        // 如果修改成功,则重新返回医生管理界面
        SceneManager.LoadScene("03-DoctorUI");
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
