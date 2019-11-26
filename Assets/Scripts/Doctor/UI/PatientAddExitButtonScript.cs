using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientAddExitButtonScript : MonoBehaviour {

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientQuery;

    // Use this for initialization
    void Start()
    {
        PatientAdd = transform.parent.parent.Find("PatientAdd").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientAddExitButtonOnClick()
    {
        PatientQuery.SetActive(false);
        PatientAdd.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
