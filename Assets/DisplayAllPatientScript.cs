using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAllPatientScript : MonoBehaviour {

    public GameObject PatientList;  // 让PatientList重新激活一下，刷新界面


	// Use this for initialization
	void Start () {
        PatientList = transform.parent.Find("PatientListBG/PatientList").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayAllPatientButonOnClick(){
        DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
        PatientList.SetActive(false);
        PatientList.SetActive(true);
    }
}
