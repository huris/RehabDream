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

    public Dictionary<string, int> DoctorString2Int = new Dictionary<string, int>();
    public Dictionary<int, string> DoctorInt2String = new Dictionary<int, string>();

    public List<string> PatientDoctorName = new List<string>(); 

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

        if(DoctorDataManager.instance.DoctorsIDAndName != null && DoctorDataManager.instance.DoctorsIDAndName.Count > 0)
        {
            //DoctorDataManager.instance.Doctors = DoctorDataManager.instance.Doctors.OrderBy(s => s.DoctorPinyin).ToList();

            DoctorString2Int.Clear();
            DoctorInt2String.Clear();
            PatientDoctorName.Clear();
            PatientDoctor.ClearOptions();

            //print(DoctorDataManager.instance.Doctors.Count);
            for (int i = 0; i < DoctorDataManager.instance.DoctorsIDAndName.Count; i++)
            {
                DoctorString2Int.Add(DoctorDataManager.instance.DoctorsIDAndName[i].Item2, i);
                DoctorInt2String.Add(i, DoctorDataManager.instance.DoctorsIDAndName[i].Item2);
                PatientDoctorName.Add(DoctorDataManager.instance.DoctorsIDAndName[i].Item2);
            }

            PatientDoctorName.Add("请选择医生");
            PatientDoctor.AddOptions(PatientDoctorName);
            PatientDoctor.value = PatientDoctorName.Count;
            //print(PatientDoctor.options.Count);
        }
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
        DoctorDataManager.instance.doctor.Patients = DoctorDatabaseManager.instance.PatientQueryInformation(PatientName.text, PatientDoctor.value==PatientDoctorName.Count?-1:DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorID, PatientDoctor.value == PatientDoctorName.Count ? "root" : DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorName);
        if(DoctorDataManager.instance.doctor.Patients != null && DoctorDataManager.instance.doctor.Patients.Count > 0)
        {
            //DoctorDataManager.instance.doctor.Patients[0].SetPatientData();
            DoctorDataManager.instance.doctor.patient = DoctorDataManager.instance.doctor.Patients[0];
        }

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
