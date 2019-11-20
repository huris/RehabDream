using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientModifyExitButtonScript : MonoBehaviour {

    public GameObject PatientModify;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start()
    {
        PatientModify = transform.parent.parent.Find("PatientModify").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientModifyExitButtonOnClick()
    {
        PatientModify.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
