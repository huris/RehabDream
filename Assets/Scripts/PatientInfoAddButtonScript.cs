using System;
using System.Collections;
using System.Collections.Generic;
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
    public Text PatientID;
    public InputField PatientPassword;
    public long TryPatientID;

    public GameObject ErrorInformation;   // 输入内容为空
    public GameObject RegisterSuccess;   // 账号注册成功

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientQuery;

    // Use this for initialization
    void OnEnable()
    {
        PatientName = transform.parent.Find("AddPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("AddPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("AddPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("AddPatientSex/Woman").GetComponent<Toggle>();
        PatientHeight = transform.parent.Find("AddPatientHeight/InputField").GetComponent<InputField>();
        PatientWeight = transform.parent.Find("AddPatientWeight/InputField").GetComponent<InputField>();
        PatientID = transform.parent.Find("AddPatientID/PatientID").GetComponent<Text>();
        PatientPassword = transform.parent.Find("AddPatientPassword/InputField").GetComponent<InputField>();

        ErrorInformation = transform.parent.Find("ErrorInput").gameObject;   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        RegisterSuccess = transform.parent.Find("RegisterSuccess").gameObject;   // 绑定错误信息
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态

        PatientAdd = transform.parent.parent.Find("PatientAdd").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;

        TryPatientID = GetRandom(100000, 999999);  // 患者账号为6位
        while (DoctorDatabaseManager.instance.CheckPatient(TryPatientID) != DoctorDatabaseManager.DatabaseReturn.Success)
        {
            TryPatientID = GetRandom(100000, 999999);  // 直到找到一个不重复PatientID
        }
        PatientID.text = TryPatientID.ToString();
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

            PatientSex = "";
            if (Man.isOn) PatientSex = "男";
            else if (Woman.isOn) PatientSex = "女";

            Patient patient = new Patient();
            patient.setPatientCompleteMessage(TryPatientID, PatientName.text,MD5Encrypt(PatientPassword.text), DoctorDataManager.instance.doctor.DoctorID, long.Parse(PatientAge.text), PatientSex, long.Parse(PatientHeight.text), long.Parse(PatientWeight.text));

            DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.PatientRegister(patient);

            if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
            {
                //print("成功");
                RegisterSuccess.SetActive(true);

                DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);

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

        PatientName.text = "";
        PatientAge.text = "";
        Man.isOn = false;
        Woman.isOn = false;
        PatientHeight.text = "";
        PatientWeight.text = "";
        PatientPassword.text = "";

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
