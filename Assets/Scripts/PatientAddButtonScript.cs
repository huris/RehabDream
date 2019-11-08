using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientAddButtonScript : MonoBehaviour {

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start()
    {
        PatientAdd = transform.parent.Find("PatientAdd").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientAddButtonOnclick()
    {
        PatientAdd.SetActive(true);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
    }
}
