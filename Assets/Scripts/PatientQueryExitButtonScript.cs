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
        PatientQuery = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientQuery");
        PatientInfo = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientInfo");
        PatientListBG = GameObject.Find("Canvas/Background/FunctionUI/PatentInfoManagerUI/PatientListBG");
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
