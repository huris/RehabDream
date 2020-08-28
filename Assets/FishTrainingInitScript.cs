using ShipNSea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishTrainingInitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FishTrainingStartButtonOnClick()
	{
		ReportPanelController.closeBtnFun = () => { MapDetectionController.mapOccupyDis.Clear(); SceneManager.LoadScene("TestScene"); };
		//IntroState.outName = "潘振远";
		IntroState.pPwd = DoctorDataManager.instance.doctor.patient.PatientName;


		SceneManager.LoadScene("Intro");  // 如果登录成功,则进入医生管理界面
	}

}
