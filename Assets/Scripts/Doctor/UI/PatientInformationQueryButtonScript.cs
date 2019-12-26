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

    public List<string> PatientDoctorName; 

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
        PatientDoctorName = new List<string>();

        for (int i = 0; i < DoctorDataManager.instance.Doctors.Count; i++)
        {
            DoctorString2Int.Add(DoctorDataManager.instance.Doctors[i].DoctorName, i);
            DoctorInt2String.Add(i, DoctorDataManager.instance.Doctors[i].DoctorName);
            PatientDoctorName.Add(DoctorDataManager.instance.Doctors[i].DoctorName);
        }

        PatientDoctorName.Add("请输入医生");
        PatientDoctor.AddOptions(PatientDoctorName);
        PatientDoctor.value = PatientDoctorName.Count;
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

        // 如果用户没有选择医生，则传入医生工号为-1
        DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.PatientQueryInformation(PatientName.text, PatientDoctor.value==PatientDoctorName.Count?-1:DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorID);

        PatientName.text = "";
        PatientDoctor.value = PatientDoctorName.Count;
        //PatientAge.text = "";
        //Man.isOn = false;
        //Woman.isOn = false;

        PatientQuery.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
