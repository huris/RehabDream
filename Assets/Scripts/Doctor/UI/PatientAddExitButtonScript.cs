using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientAddExitButtonScript : MonoBehaviour {

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientQuery;

    public InputField PatientName;
    public string PatientSex;
    public InputField PatientAge;
    public Toggle Man;
    public Toggle Woman;
    public InputField PatientHeight;
    public InputField PatientWeight;
    public InputField PatientID;
    //public InputField PatientPassword;
    //public long TryPatientID;
    public InputField PatientSymptom;   // 病症
    public Dropdown PatientDoctor;

    // Use this for initialization
    void OnEnable()
    {
        PatientAdd = transform.parent.parent.Find("PatientAdd").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;

        PatientName = transform.parent.Find("AddPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("AddPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("AddPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("AddPatientSex/Woman").GetComponent<Toggle>();
        PatientHeight = transform.parent.Find("AddPatientHeight/InputField").GetComponent<InputField>();
        PatientWeight = transform.parent.Find("AddPatientWeight/InputField").GetComponent<InputField>();
        PatientID = transform.parent.Find("AddPatientID/InputField").GetComponent<InputField>();
        PatientSymptom = transform.parent.Find("AddPatientSymptom/InputField").GetComponent<InputField>();
        PatientDoctor = transform.parent.Find("AddPatientDoctor/Dropdown").GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientAddExitButtonOnClick()
    {
        PatientName.text = "";
        PatientAge.text = "";
        Man.isOn = false;
        Woman.isOn = false;
        PatientHeight.text = "";
        PatientWeight.text = "";
        PatientID.text = "";
        PatientSymptom.text = "";
        PatientDoctor.value = PatientDoctor.options.Count-1;

        PatientQuery.SetActive(false);
        PatientAdd.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
