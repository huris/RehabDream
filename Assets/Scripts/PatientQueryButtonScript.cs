using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientQueryButtonScript : MonoBehaviour {

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientAdd;

    public InputField PatientName;
    public string PatientSex;
    public InputField PatientAge;
    public Toggle Man;
    public Toggle Woman;

    // Use this for initialization
    void OnEnable () {
        PatientQuery = transform.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;
        PatientAdd = transform.parent.Find("PatientAdd").gameObject;

        PatientName = transform.parent.Find("PatientQuery/QueryPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("PatientQuery/QueryPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("PatientQuery/QueryPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("PatientQuery/QueryPatientSex/Woman").GetComponent<Toggle>();

        PatientName.text = "";
        PatientAge.text = "";
        Man.isOn = false;
        Woman.isOn = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PatientQueryButtonOnclick()
    {
        PatientQuery.SetActive(true);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
        PatientAdd.SetActive(false);
    }

}
