using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientQueryExitButtonScript : MonoBehaviour {

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start()
    {
        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
    }

    // Update is called once per frame
    void Update () {

    }

    public void PatientQueryExitButtonOnClick()
    {
        PatientQuery.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
