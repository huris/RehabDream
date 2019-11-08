using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientQueryButtonScript : MonoBehaviour {

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start () {
        PatientQuery = transform.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PatientQueryButtonOnclick()
    {
        PatientQuery.SetActive(true);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
    }

}
