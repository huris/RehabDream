using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAllPatientScript : MonoBehaviour {

    public GameObject PatientList;  // 让PatientList重新激活一下，刷新界面
    
    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    public GameObject PatientAdd;

    // Use this for initialization
    void Start () {
        PatientList = transform.parent.Find("PatientListBG/PatientList").gameObject;

        PatientQuery = transform.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;

        PatientAdd = transform.parent.Find("PatientAdd").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayAllPatientButonOnClick(){
        DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);

        PatientList.SetActive(false);
        PatientList.SetActive(true);

        PatientQuery.SetActive(false);
        PatientAdd.SetActive(false);

        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
