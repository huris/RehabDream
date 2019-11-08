using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientAddExitButtonScript : MonoBehaviour {

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start()
    {
        PatientAdd = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientAdd");
        PatientInfo = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientInfo");
        PatientListBG = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientListBG");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientAddExitButtonOnClick()
    {
        PatientAdd.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
