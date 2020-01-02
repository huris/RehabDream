﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    //public InputField PatientPassword;
    //public long TryPatientID;
    public InputField PatientSymptom;   // 病症
    public Dropdown PatientDoctor;

    public GameObject ErrorInformation;   // 输入内容为空
    public GameObject RegisterSuccess;   // 账号注册成功
    public GameObject WithNoDoctor;   // 未添加主治医生

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientQuery;

    public Dictionary<string, int> DoctorString2Int;
    public Dictionary<int, string> DoctorInt2String;

    public List<string> PatientDoctorName;

    // Use this for initialization
    void OnEnable()
    {
        PatientName = transform.parent.Find("AddPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("AddPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("AddPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("AddPatientSex/Woman").GetComponent<Toggle>();
        PatientHeight = transform.parent.Find("AddPatientHeight/InputField").GetComponent<InputField>();
        PatientWeight = transform.parent.Find("AddPatientWeight/InputField").GetComponent<InputField>();
        PatientID = transform.parent.Find("AddPatientID/PatientID").GetComponent<InputField>();
        PatientSymptom = transform.parent.Find("AddPatientSymptom/InputField").GetComponent<InputField>();
        PatientDoctor = transform.parent.Find("AddPatientDoctor/Dropdown").GetComponent<Dropdown>();

        ErrorInformation = transform.parent.Find("ErrorInput").gameObject;   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        RegisterSuccess = transform.parent.Find("RegisterSuccess").gameObject;   // 绑定错误信息
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态

        WithNoDoctor = transform.parent.Find("WithNoDoctor").gameObject;   // 绑定错误信息
        WithNoDoctor.SetActive(false);     // 设置语句刚开始处于未激活状态

        PatientAdd = transform.parent.parent.Find("PatientAdd").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;

        //TryPatientID = GetRandom(100000, 999999);  // 患者账号为6位
        //while (DoctorDatabaseManager.instance.CheckPatient(TryPatientID) != DoctorDatabaseManager.DatabaseReturn.Success)
        //{
        //    TryPatientID = GetRandom(100000, 999999);  // 直到找到一个不重复PatientID
        //}
        //PatientID.text = TryPatientID.ToString();


        //print(DoctorDataManager.instance.Doctors.Count);

        DoctorString2Int = new Dictionary<string, int>();
        DoctorInt2String = new Dictionary<int, string>();
        PatientDoctorName = new List<string>();

        for (int i = 0; i < DoctorDataManager.instance.Doctors.Count; i++)
        {
            DoctorString2Int.Add(DoctorDataManager.instance.Doctors[i].DoctorName, i);
            DoctorInt2String.Add(i, DoctorDataManager.instance.Doctors[i].DoctorName);
            PatientDoctorName.Add(DoctorDataManager.instance.Doctors[i].DoctorName);
        }

        PatientDoctorName.Add("请输入医生");
        //print("请输入医生");
        //print(PatientDoctorName.Count);
        PatientDoctor.AddOptions(PatientDoctorName);
        PatientDoctor.value = PatientDoctorName.Count;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientInformationAddButtonOnClick()
    {
        try
        {

            ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态
            RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态
            WithNoDoctor.SetActive(false);     // 设置语句刚开始处于未激活状态

            PatientSex = "";
            if (Man.isOn) PatientSex = "男";
            else if (Woman.isOn) PatientSex = "女";

            print("!!!!!");

            if (PatientDoctorName.Count == PatientDoctor.value)
            {
                WithNoDoctor.SetActive(true);
            }
            else
            {
                if (PatientName.text == "" || PatientSex == "" || PatientAge.text == "")
                {
                    print("1");
                    ErrorInformation.SetActive(true);
                }
                else
                {
                    Patient patient = new Patient();
                    patient.setPatientCompleteMessage(long.Parse(PatientID.text), PatientName.text, PatientSymptom.text, DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorID, long.Parse(PatientAge.text), PatientSex, PatientHeight.text == ""?-1:long.Parse(PatientHeight.text), PatientWeight.text == ""?-1:long.Parse(PatientWeight.text));

                    DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.PatientRegister(patient);

                    if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
                    {
                        //print("成功");
                        RegisterSuccess.SetActive(true);

                        DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
                        DoctorDataManager.instance.patient = patient;

                        StartCoroutine(DelayTime(3));
                    }
                    else if (RETURN == DoctorDatabaseManager.DatabaseReturn.NullInput)
                    {
                        print("2");
                        ErrorInformation.SetActive(true);
                    }
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

        PatientName.text = "";
        PatientAge.text = "";
        Man.isOn = false;
        Woman.isOn = false;
        PatientHeight.text = "";
        PatientWeight.text = "";
        PatientSymptom.text = "";

        // 如果注册成功,则进入医生管理界面
        PatientAdd.SetActive(false);
        PatientQuery.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
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
