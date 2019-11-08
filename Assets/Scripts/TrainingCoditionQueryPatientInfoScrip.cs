using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCoditionQueryPatientInfoScrip : MonoBehaviour {

    public Text PatientName;
    public Text PatientSex;
    public Text PatientAge;
    public Text PatientHeight;
    public Text PatientWeight;

    // Use this for initialization
    void Start () {
        PatientName = GameObject.Find("Canvas/Background/FunctionUI/TrainingCoditionQueryUI/PatientInfo/Name/PatientName").GetComponent<Text>();
        PatientSex = GameObject.Find("Canvas/Background/FunctionUI/TrainingCoditionQueryUI/PatientInfo/Sex/PatientSex").GetComponent<Text>();
        PatientAge = GameObject.Find("Canvas/Background/FunctionUI/TrainingCoditionQueryUI/PatientInfo/Age/PatientAge").GetComponent<Text>();
        PatientHeight = GameObject.Find("Canvas/Background/FunctionUI/TrainingCoditionQueryUI/PatientInfo/Height/PatientHeight").GetComponent<Text>();
        PatientWeight = GameObject.Find("Canvas/Background/FunctionUI/TrainingCoditionQueryUI/PatientInfo/Weight/PatientWeight").GetComponent<Text>();

        PatientName.text = DoctorDataManager.instance.patient.PatientName;
        PatientSex.text = DoctorDataManager.instance.patient.PatientSex;
        PatientAge.text = DoctorDataManager.instance.patient.PatientAge.ToString();
        PatientHeight.text = DoctorDataManager.instance.patient.PatientHeight.ToString();
        PatientWeight.text = DoctorDataManager.instance.patient.PatientWeight.ToString();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
