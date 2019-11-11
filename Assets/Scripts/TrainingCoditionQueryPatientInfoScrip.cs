using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCoditionQueryPatientInfoScrip : MonoBehaviour {

    public Text PatientName;
    public Text PatientSex;
    public Text PatientAge;
    public Text PatientHeight;
    public Text PatientWeight;

    // Use this for initialization
    void OnEnable () {
        PatientName = transform.Find("Name/PatientName").GetComponent<Text>();
        PatientSex = transform.Find("Sex/PatientSex").GetComponent<Text>();
        PatientAge = transform.Find("Age/PatientAge").GetComponent<Text>();
        PatientHeight = transform.Find("Height/PatientHeight").GetComponent<Text>();
        PatientWeight = transform.Find("Weight/PatientWeight").GetComponent<Text>();

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
