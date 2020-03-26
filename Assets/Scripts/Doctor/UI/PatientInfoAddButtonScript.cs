using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PatientInfoAddButtonScript : MonoBehaviour
{

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
    public GameObject PatientAlreadyExist;  // 已存在该患者

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientQuery;

    public Dictionary<string, int> DoctorString2Int = new Dictionary<string, int>();
    public Dictionary<int, string> DoctorInt2String = new Dictionary<int, string>();

    public List<string> PatientDoctorName = new List<string>();

    public EventSystem system;

    private Selectable SelecInput;   // 当前焦点所处的Input
    private Selectable NextInput;   // 目标Input

    // Use this for initialization
    void OnEnable()
    {
        PatientName = transform.parent.Find("AddPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("AddPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("AddPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("AddPatientSex/Woman").GetComponent<Toggle>();
        PatientHeight = transform.parent.Find("AddPatientHeight/InputField").GetComponent<InputField>();
        PatientWeight = transform.parent.Find("AddPatientWeight/InputField").GetComponent<InputField>();
        PatientID = transform.parent.Find("AddPatientID/InputField").GetComponent<InputField>();
        PatientSymptom = transform.parent.Find("AddPatientSymptom/InputField").GetComponent<InputField>();
        PatientDoctor = transform.parent.Find("AddPatientDoctor/Dropdown").GetComponent<Dropdown>();

        ErrorInformation = transform.parent.Find("ErrorInput").gameObject;   // 绑定错误信息
        ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态

        RegisterSuccess = transform.parent.Find("RegisterSuccess").gameObject;   // 绑定错误信息
        RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态

        WithNoDoctor = transform.parent.Find("WithNoDoctor").gameObject;   // 绑定错误信息
        WithNoDoctor.SetActive(false);     // 设置语句刚开始处于未激活状

        PatientAlreadyExist = transform.parent.Find("PatientAlreadyExist").gameObject;
        PatientAlreadyExist.SetActive(false);

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

        if (DoctorDataManager.instance.DoctorsIDAndName != null && DoctorDataManager.instance.DoctorsIDAndName.Count > 0)
        {
            DoctorString2Int.Clear();
            DoctorInt2String.Clear();
            PatientDoctorName.Clear();
            PatientDoctor.ClearOptions();

            for (int i = 0; i < DoctorDataManager.instance.DoctorsIDAndName.Count; i++)
            {
                DoctorString2Int.Add(DoctorDataManager.instance.DoctorsIDAndName[i].Item2, i);
                DoctorInt2String.Add(i, DoctorDataManager.instance.DoctorsIDAndName[i].Item2);
                PatientDoctorName.Add(DoctorDataManager.instance.DoctorsIDAndName[i].Item2);
            }

            PatientDoctorName.Add("请输入医生");
            //print("请输入医生");
            //print(PatientDoctorName.Count);
            PatientDoctor.AddOptions(PatientDoctorName);
            PatientDoctor.value = PatientDoctorName.Count;
        }

        system = EventSystem.current;       // 获取当前的事件
    }

    // Update is called once per frame
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
            PatientInformationAddButtonOnClick();
        }
    }

    public void PatientInformationAddButtonOnClick()
    {
        try
        {

            ErrorInformation.SetActive(false);     // 设置语句刚开始处于未激活状态
            RegisterSuccess.SetActive(false);     // 设置语句刚开始处于未激活状态
            WithNoDoctor.SetActive(false);     // 设置语句刚开始处于未激活状态
            PatientAlreadyExist.SetActive(false);

            PatientSex = "";
            if (Man.isOn) PatientSex = "男";
            else if (Woman.isOn) PatientSex = "女";

            //print("!!!!!");

            if (PatientDoctorName.Count == PatientDoctor.value)
            {
                WithNoDoctor.SetActive(true);
            }
            else
            {
                if (DoctorDatabaseManager.instance.CheckPatient(long.Parse(PatientID.text)) == DoctorDatabaseManager.DatabaseReturn.Fail)
                {
                    PatientAlreadyExist.SetActive(true);

                }
                else if (PatientName.text == "" || PatientSex == "" || PatientAge.text == "")
                {
                    //print("1");
                    ErrorInformation.SetActive(true);
                }
                else
                {
                    Patient patient = new Patient(long.Parse(PatientID.text), PatientName.text, PatientSymptom.text, DoctorDataManager.instance.DoctorsIDAndName[PatientDoctor.value].Item1, DoctorDataManager.instance.DoctorsIDAndName[PatientDoctor.value].Item2, long.Parse(PatientAge.text), PatientSex, PatientHeight.text == "" ? -1 : long.Parse(PatientHeight.text), PatientWeight.text == "" ? -1 : long.Parse(PatientWeight.text));

                    //print(DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorID);
                    DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.PatientRegister(patient);

                    if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
                    {
                        //print("成功");
                        RegisterSuccess.SetActive(true);

                        //DoctorDataManager.instance.doctor.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID, DoctorDataManager.instance.doctor.DoctorName);

                        // 插入用户
                        int z = 0;
                        for (z = 0; z < DoctorDataManager.instance.doctor.Patients.Count; z++)
                        {
                            if(patient.PatientPinyin.CompareTo(DoctorDataManager.instance.doctor.Patients[z].PatientPinyin) < 0)
                            {
                                DoctorDataManager.instance.doctor.Patients.Insert(z, patient);
                                break;
                            }
                        }
                        if(z == DoctorDataManager.instance.doctor.Patients.Count)
                        {
                            DoctorDataManager.instance.doctor.Patients.Insert(z, patient);
                        }

                        DoctorDataManager.instance.doctor.patient = patient;

                        StartCoroutine(DelayTime(3));
                    }
                    else if (RETURN == DoctorDatabaseManager.DatabaseReturn.NullInput)
                    {
                        // print("2");
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
        PatientID.text = "";
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
