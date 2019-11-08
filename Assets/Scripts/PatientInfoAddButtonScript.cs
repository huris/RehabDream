using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientInfoAddButtonScript : MonoBehaviour {

    public InputField PatientName;
    public string PatientSex;
    public InputField PatientAge;
    public Toggle Man;
    public Toggle Woman;
    public InputField PatientHeight;
    public InputField PatientWeight;
    public InputField PatientID;
    public InputField PatientPassword;

    public GameObject UserNameErrorInput;   // 账号由10位以下组成
    public GameObject ErrorInformation;   // 输入内容为空
    public GameObject UserNameAlreadyExist;   // 账号已存在
    public GameObject RegisterSuccess;   // 账号注册成功

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start()
    {
        PatientName = transform.parent.Find("AddPatientName").GetComponent<InputField>();
        PatientAge = transform.parent.Find("AddPatientAge").GetComponent<InputField>();
        Man = transform.parent.Find("AddPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("AddPatientSex/Woman").GetComponent<Toggle>();
        PatientHeight = transform.parent.Find("AddPatientHeight").GetComponent<InputField>();
        PatientWeight = transform.parent.Find("AddPatientWeight").GetComponent<InputField>();
        PatientID = transform.parent.Find("AddPatientID").GetComponent<InputField>();
        PatientPassword = transform.parent.Find("AddPatientPassword").GetComponent<InputField>();

        UserNameErrorInput = GameObject.Find("UserNameErrorInput");   // 绑定错误信息
        UserNameErrorInput.SetActive(false);     // 设置语句刚开始处于未激活状态

        ErrorInformation = GameObject.Find("ErrorInput");   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        UserNameAlreadyExist = GameObject.Find("UserNameAlreadyExist");   // 绑定错误信息
        UserNameAlreadyExist.SetActive(false);     // 设置语句刚开始处于未激活状态

        RegisterSuccess = GameObject.Find("RegisterSuccess");   // 绑定错误信息
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态

        PatientAdd = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientAdd");
        PatientInfo = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientInfo");
        PatientListBG = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientListBG");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientInformationAddButtonOnClick()
    {
        try
        {
            if (long.Parse(PatientID.text) >= 10000000000)
            {
                UserNameErrorInput.SetActive(true);
            }
            else
            {
                PatientSex = "";
                if (Man.isOn) PatientSex = "男";
                else if (Woman.isOn) PatientSex = "女";

                DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.PatientRegister(long.Parse(PatientID.text), PatientName.text, PatientPassword.text, DoctorDataManager.instance.doctor.DoctorID, long.Parse(PatientAge.text), PatientSex, long.Parse(PatientHeight.text), long.Parse(PatientWeight.text));

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

        // 如果注册成功,则进入医生管理界面
        PatientAdd.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
