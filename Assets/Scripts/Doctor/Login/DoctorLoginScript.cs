using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class DoctorLoginScript : MonoBehaviour {

    public InputField DoctorID;     // 医生账号
    public InputField DoctorPassword;    // 医生密码

    public GameObject ErrorInformation;   // 账号或者密码错误显示

    public EventSystem system;

    private Selectable SelecInput;   // 当前焦点所处的Input
    private Selectable NextInput;   // 目标Input

    // Use this for initialization
    void OnEnable () {

        DoctorID = transform.Find("DoctorID").GetComponent<InputField>();    //  绑定账号
        DoctorPassword = transform.Find("DoctorPassword").GetComponent<InputField>();    //  绑定密码

        ErrorInformation = transform.Find("ErrorInput").gameObject;   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        system = EventSystem.current;       // 获取当前的事件

        //DoctorDatabaseManager.instance.DeleteCS(241283);   // 数据库测试
    }

    // Update is called once per frame
    void Update() {

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
            LoginOnClick(); 
        }

    }
   

    public void LoginOnClick()
    {
        try
        {
            if(DoctorID.text == "root")
            {
                // 如果管理员账号不存在，则创建一个
                if(DoctorDatabaseManager.instance.CheckRoot() == DoctorDatabaseManager.DatabaseReturn.Success)
                {
                    Doctor doctor = new Doctor();
                    doctor.SetDoctorMessage(12345, DoctorDatabaseManager.instance.MD5Encrypt("root"), "root");
                    DoctorDatabaseManager.instance.DoctorRegister(doctor);
                }

                if (DoctorDatabaseManager.instance.DoctorNameLogin(DoctorID.text, DoctorPassword.text) == DoctorDatabaseManager.DatabaseReturn.Success)
                {

                    //print("成功");
                    DoctorDataManager.instance.doctor = DoctorDatabaseManager.instance.ReadDoctorNameInfo(DoctorID.text);

                    DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
                    DoctorDataManager.instance.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();

                    //foreach(var item in DoctorDataManager.instance.Patients)
                    //{
                    //    print(item.PatientPinyin);
                    //}

                    SceneManager.LoadScene("03-DoctorUI");  // 如果登录成功,则进入医生管理界面
                }
                else  // 如果账号密码不正确,则提示
                {
                    ErrorInformation.SetActive(true);
                }


            }
            // 判断是否存在该用户且账号密码正确
            else if(DoctorID.text[0] >= '0' && DoctorID.text[0] <= '9')    // 如果为数字
            {
                if (DoctorDatabaseManager.instance.DoctorIDLogin(long.Parse(DoctorID.text), DoctorPassword.text) == DoctorDatabaseManager.DatabaseReturn.Success)
                {
                    //print("成功");
                    DoctorDataManager.instance.doctor = DoctorDatabaseManager.instance.ReadDoctorIDInfo(long.Parse(DoctorID.text));

                    DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
                    DoctorDataManager.instance.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();

                    //foreach(var item in DoctorDataManager.instance.Patients)
                    //{
                    //    print(item.PatientPinyin);
                    //}

                    SceneManager.LoadScene("03-DoctorUI");  // 如果登录成功,则进入医生管理界面
                }
                else  // 如果账号密码不正确,则提示
                {
                    ErrorInformation.SetActive(true);
                }
            }
            else    // 否则为输入姓名
            {
                if (DoctorDatabaseManager.instance.DoctorNameLogin(DoctorID.text, DoctorPassword.text) == DoctorDatabaseManager.DatabaseReturn.Success)
                {

                    //print("成功");
                    DoctorDataManager.instance.doctor = DoctorDatabaseManager.instance.ReadDoctorNameInfo(DoctorID.text);

                    DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
                    DoctorDataManager.instance.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();

                    //foreach(var item in DoctorDataManager.instance.Patients)
                    //{
                    //    print(item.PatientPinyin);
                    //}

                    SceneManager.LoadScene("03-DoctorUI");  // 如果登录成功,则进入医生管理界面
                }
                else  // 如果账号密码不正确,则提示
                {
                    ErrorInformation.SetActive(true);
                }
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
