using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDataInitScript : MonoBehaviour {

    public GameObject NoTrainingData;


	// Use this for initialization
	void Start () {
		
	}

    void OnEnable()
    {
        NoTrainingData = transform.Find("NoTrainingData").gameObject;

		//DoctorDataManager.instance.doctor.patient.trainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);

		if (DoctorDataManager.instance.doctor.patient.trainingPlays != null && DoctorDataManager.instance.doctor.patient.trainingPlays.Count > 0) NoTrainingData.SetActive(false);
        else NoTrainingData.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
