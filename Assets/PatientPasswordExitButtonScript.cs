using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientPasswordExitButtonScript : MonoBehaviour {

    public GameObject PatientPasswordModify;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void OnEnable () {
        PatientPasswordModify = transform.parent.parent.Find("PatientPasswordModify").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PatientPasswordExitButtonOnClick()
    {
        PatientPasswordModify.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }

}
