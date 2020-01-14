using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatienPhysicalConditionsQueryScript : MonoBehaviour {

    public Text PatientName;
    public Text PatientSex;
    public Text PatientAge;
    public Text PatientHeight;
    public Text PatientWeight;
    public Text PatientSymptom;

    // Use this for initialization
    void OnEnable () {

        PatientName = transform.Find("QueryPatientName/Text").GetComponent<Text>();
        PatientSex = transform.Find("QueryPatientSex/Text").GetComponent<Text>();
        PatientAge = transform.Find("QueryPatientAge/Text").GetComponent<Text>();
        PatientHeight = transform.Find("QueryPatientHeight/Text").GetComponent<Text>();
        PatientWeight = transform.Find("QueryPatientWeight/Text").GetComponent<Text>();
        PatientSymptom = transform.Find("QueryPatientSymptom/Text").GetComponent<Text>();

        PatientName.text = DoctorDataManager.instance.doctor.patient.PatientName;
        PatientSex.text = DoctorDataManager.instance.doctor.patient.PatientSex;
        PatientAge.text = DoctorDataManager.instance.doctor.patient.PatientAge.ToString();

        if (DoctorDataManager.instance.doctor.patient.PatientHeight == -1) { 
            PatientHeight.text = "未填写";
        }
        else {
            PatientHeight.text = DoctorDataManager.instance.doctor.patient.PatientHeight.ToString();
        }

        if (DoctorDataManager.instance.doctor.patient.PatientWeight == -1) {
            PatientWeight.text = "未填写";
        }
        else{
            PatientWeight.text = DoctorDataManager.instance.doctor.patient.PatientWeight.ToString();
        }

        PatientSymptom.text = DoctorDataManager.instance.doctor.patient.PatientSymptom.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
