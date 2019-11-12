using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientInformationQueryButtonScript : MonoBehaviour {

    public InputField PatientName;
    public string PatientSex;
    public InputField PatientAge;
    public Toggle Man;
    public Toggle Woman;

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start() {

        PatientName = transform.parent.Find("QueryPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("QueryPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("QueryPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("QueryPatientSex/Woman").GetComponent<Toggle>();

        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
    }

    // Update is called once per frame
    void Update() {

    }

    public void PatientInformationQueryButtonOnClick()
    {
        PatientSex = "";
        if (Man.isOn) PatientSex = "男";
        else if (Woman.isOn) PatientSex = "女";

        if (PatientAge.text == "") PatientAge.text = "0"; 
        
        DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.PatientQueryInformation(PatientName.text, PatientSex, long.Parse(PatientAge.text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowLeadingSign), DoctorDataManager.instance.doctor.DoctorID);

        PatientName.text = "";
        PatientAge.text = "";
        Man.isOn = false;
        Woman.isOn = false;
        
        PatientQuery.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
