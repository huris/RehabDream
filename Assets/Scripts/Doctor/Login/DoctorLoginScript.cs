using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using System.IO;

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

        //print(DoctorDatabaseManager.instance.ReadMaxEvaluationID());
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
            //print("2!!!");

            if (DoctorDataManager.instance.DoctorLoginCheck(DoctorID.text, DoctorPassword.text) == true)
            {
                DoctorDataManager.instance.FunctionManager = 0; // 进入患者主界面
                SceneManager.LoadScene("03-DoctorUI");
            }
            else
            {
                //print("3!!!");
                ErrorInformation.SetActive(true);
            }
        }
        catch (Exception e) // 抛出异常
        {
            //print("4!!!");
            ErrorInformation.SetActive(true);
        }
        
    }

    public void RegisterOnClick()   // 点击注册按钮,进入医生注册界面
    {
        SceneManager.LoadScene("02-DoctorRegister");
    }

}
