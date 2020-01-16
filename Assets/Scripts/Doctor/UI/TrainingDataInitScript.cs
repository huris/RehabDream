using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingDataInitScript : MonoBehaviour {

    public GameObject NoTrainingData;

	public GameObject Report;
	public Toggle TrainingToggle;

	// Use this for initialization
	void Start () {
		
	}

    void OnEnable()
    {
        NoTrainingData = transform.Find("NoTrainingData").gameObject;
		Report = transform.parent.parent.parent.Find("Report").gameObject;
		TrainingToggle = transform.parent.parent.parent.Find("Report/Training").GetComponent<Toggle>();

		//DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);

		if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0) NoTrainingData.SetActive(false);
        else NoTrainingData.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ReadReportButtonOnclick()
	{
		Report.SetActive(true);
		TrainingToggle.isOn = true;
	}
}
