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

    // Use this for initialization
    void Start() {

        PatientName = transform.parent.Find("QueryPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("QueryPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("QueryPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("QueryPatientSex/Woman").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void PatientInformationQueryButtonOnClick()
    {
        PatientSex = "";
        if (Man.isOn) PatientSex = "男";
        else if (Woman.isOn) PatientSex = "女";

        print(PatientSex);
        print(PatientAge.text);
        print(DoctorDataManager.instance.doctor.DoctorID);

        DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.PatientQueryInformation(PatientName.text, PatientSex, long.Parse(PatientAge.text, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowLeadingSign), DoctorDataManager.instance.doctor.DoctorID);
       
    }
}
