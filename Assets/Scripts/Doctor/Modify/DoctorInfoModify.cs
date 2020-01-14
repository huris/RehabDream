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
using UnityEngine.EventSystems;

public class DoctorInfoModify : MonoBehaviour {

    public InputField OldPassword;    // 医生旧密码
    public InputField NewPassword;    // 医生新密码
    public InputField NewPasswordAgain;   // 再次输入医生的新密码

    public GameObject OldPasswordError;   // 旧密码错误
    public GameObject NewPasswordError;   // 新密码错误 
    public GameObject NewPasswordIsNotSame;  // 两次密码不一致

    public GameObject ModifyPasswordSuccess;   // 修改信息成功

    public EventSystem system;
    private Selectable SelecInput;   // 当前焦点所处的Input
    private Selectable NextInput;   // 目标Input

    // use this for initialization
    void OnEnable()
    {
        OldPassword = transform.Find("OldPassword").GetComponent<InputField>();    //  绑定旧密码
        NewPassword = transform.Find("NewPassword").GetComponent<InputField>();    //  绑定新密码
        NewPasswordAgain = transform.Find("NewPasswordAgain").GetComponent<InputField>();   // 绑定再次输入新密码

        OldPasswordError = transform.Find("OldPasswordError").gameObject;    //  旧密码绑定
        OldPasswordError.SetActive(false);

        NewPasswordError = transform.Find("NewPasswordError").gameObject;    //  新密码绑定
        NewPasswordError.SetActive(false);

        NewPasswordIsNotSame = transform.Find("NewPasswordIsNotSame").gameObject;  // 两次新密码输入不一致
        NewPasswordIsNotSame.SetActive(false);

        ModifyPasswordSuccess = transform.Find("ModifyPasswordSuccess").gameObject;    //  修改密码成功绑定
        ModifyPasswordSuccess.SetActive(false);

        system = EventSystem.current;       // 获取当前的事件
    }


    // update is called once per frame
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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ModifyOnclick();
        }
    }

    public void ModifyOnclick()   // 点击修改按钮
    {
        try
        {
            OldPasswordError.SetActive(false);
            NewPasswordError.SetActive(false);
            NewPasswordIsNotSame.SetActive(false);
            ModifyPasswordSuccess.SetActive(false);

            if (MD5Encrypt(OldPassword.text) == DoctorDataManager.instance.doctor.DoctorPassword)
            {
                if(NewPassword.text == NewPasswordAgain.text)
                {
                    DoctorDataManager.instance.doctor.SetDoctorPassword(MD5Encrypt(NewPassword.text));
                    DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.DoctorModify(DoctorDataManager.instance.doctor);

                    if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
                    {
                        OldPasswordError.SetActive(false);
                        NewPasswordError.SetActive(false);
                        NewPasswordIsNotSame.SetActive(false);
                        ModifyPasswordSuccess.SetActive(true);

                        StartCoroutine(DelayTime(3));
                    }
                    else if (RETURN == DoctorDatabaseManager.DatabaseReturn.Fail)
                    {
                        NewPasswordError.SetActive(true);
                        OldPasswordError.SetActive(false);
                        NewPasswordIsNotSame.SetActive(false);
                        ModifyPasswordSuccess.SetActive(false);
                    }
                }
                else
                {
                    NewPassword.text = NewPasswordAgain.text = "";
                    OldPasswordError.SetActive(false);
                    NewPasswordError.SetActive(false);
                    NewPasswordIsNotSame.SetActive(true);
                    ModifyPasswordSuccess.SetActive(false);
                }
            }
            else
            {
                OldPassword.text = "";
                OldPasswordError.SetActive(true);
                NewPasswordError.SetActive(false);
                NewPasswordIsNotSame.SetActive(false);
                ModifyPasswordSuccess.SetActive(false);
            }
        }
        catch (Exception e) // 抛出异常
        {
            OldPasswordError.SetActive(true);
            NewPasswordError.SetActive(false);
            NewPasswordIsNotSame.SetActive(false);
            ModifyPasswordSuccess.SetActive(false);
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
