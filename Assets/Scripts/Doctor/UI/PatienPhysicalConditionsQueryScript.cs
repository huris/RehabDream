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

    // Use this for initialization
    void OnEnable () {

        PatientName = transform.Find("QueryPatientName/Text").GetComponent<Text>();
        PatientSex = transform.Find("QueryPatientSex/Text").GetComponent<Text>();
        PatientAge = transform.Find("QueryPatientAge/Text").GetComponent<Text>();
        PatientHeight = transform.Find("QueryPatientHeight/Text").GetComponent<Text>();
        PatientWeight = transform.Find("QueryPatientWeight/Text").GetComponent<Text>();

        PatientName.text = DoctorDataManager.instance.patient.PatientName;
        PatientSex.text = DoctorDataManager.instance.patient.PatientSex;
        PatientAge.text = DoctorDataManager.instance.patient.PatientAge.ToString();
        PatientHeight.text = DoctorDataManager.instance.patient.PatientHeight.ToString();
        PatientWeight.text = DoctorDataManager.instance.patient.PatientWeight.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
