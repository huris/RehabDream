using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatienPhysicalConditionsQueryExitButtonScript : MonoBehaviour {

    public GameObject PatienPhysicalConditionsQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start()
    {
        PatienPhysicalConditionsQuery = transform.parent.parent.Find("PatienPhysicalConditionsQuery").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatienPhysicalConditionsQueryExitButtonOnClick()
    {
        PatienPhysicalConditionsQuery.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
