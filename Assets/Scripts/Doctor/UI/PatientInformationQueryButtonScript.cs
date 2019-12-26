using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PatientInformationQueryButtonScript : MonoBehaviour {

    public InputField PatientName;
    public Dropdown PatientDoctor;
    //public string PatientSex;
    //public InputField PatientAge;
    //public Toggle Man;
    //public Toggle Woman;

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    public Dictionary<string, int> DoctorString2Int;
    public Dictionary<int, string> DoctorInt2String;

    // Use this for initialization
    void OnEnable() {

        PatientName = transform.parent.Find("QueryPatientName/InputField").GetComponent<InputField>();

        PatientDoctor = transform.parent.Find("QueryPatientDoctor/Dropdown").GetComponent<Dropdown>();
        
        
        
        //PatientAge = transform.parent.Find("QueryPatientAge/InputField").GetComponent<InputField>();
        //Man = transform.parent.Find("QueryPatientSex/Man").GetComponent<Toggle>();
        //Woman = transform.parent.Find("QueryPatientSex/Woman").GetComponent<Toggle>();

        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;

        DoctorDataManager.instance.Doctors = DoctorDataManager.instance.Doctors.OrderBy(s => s.DoctorPinyin).ToList();

        DoctorString2Int = new Dictionary<string, int>();
        DoctorInt2String = new Dictionary<int, string>();
        //for(int i = 0; i < DoctorDataManager.instance.Doctors.Count; i++)
        //{
        //    DoctorString2Int.Add();
        //    DoctorInt2String.Add()
        //}

        //DoctorString2Int.Add("请选择难度", 0);
        //DoctorString2Int.Add("初级", 1);
        //DoctorString2Int.Add("一般", 2);
        //DoctorString2Int.Add("中级", 3);
        //DoctorString2Int.Add("高级", 4);

        
        //DoctorInt2String.Add(0, "请选择难度");
        //DoctorInt2String.Add(1, "初级");
        //DoctorInt2String.Add(2, "一般");
        //DoctorInt2String.Add(3, "中级");
        //DoctorInt2String.Add(4, "高级");
    }

    // Update is called once per frame
    void Update() {

    }

    public void PatientInformationQueryButtonOnClick()
    {
        //PatientSex = "";
        //if (Man.isOn) PatientSex = "男";
        //else if (Woman.isOn) PatientSex = "女";

        //if (PatientAge.text == "") PatientAge.text = "0"; 

        // DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.PatientQueryInformation(PatientName.text, PatientSex, long.Parse(PatientAge.text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowLeadingSign), DoctorDataManager.instance.doctor.DoctorID);

        //DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.PatientQueryInformation(PatientName.text, DoctorInt2String[PatientDoctor.value]);

        PatientName.text = "";
        PatientDoctor.value = 0;
        //PatientAge.text = "";
        //Man.isOn = false;
        //Woman.isOn = false;
        
        PatientQuery.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
