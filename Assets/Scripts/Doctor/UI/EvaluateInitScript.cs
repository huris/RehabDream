using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EvaluateInitScript : MonoBehaviour
{

	public GameObject NoEvaluateData;

	public Text EvaluateTime;
	public Text EvaluateButtonText;

	public GameObject PrintButton;

	// Use this for initialization
	void Start()
	{

	}

	void OnEnable()
	{
		NoEvaluateData = transform.Find("NoEvaluateData").gameObject;
		EvaluateTime = transform.Find("DataBG/EvaluateTime").GetComponent<Text>();
		EvaluateButtonText = transform.Find("DataBG/EvaluateButton/Text").GetComponent<Text>();
		PrintButton = transform.Find("DataBG/PrintButton").gameObject;

		//DoctorDataManager.instance.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.patient.PatientID, 1);

		if (DoctorDataManager.instance.patient.Evaluations.Count > 0)
		{
			NoEvaluateData.SetActive(false);

			PrintButton.SetActive(true);

			TrainingPlay LastEvaluation = DoctorDataManager.instance.patient.Evaluations[DoctorDataManager.instance.patient.Evaluations.Count - 1];
			//print(LastEvaluation.TrainingStartTime);
			EvaluateTime.text = "第" + DoctorDataManager.instance.patient.Evaluations.Count.ToString() + "次评估时间：" + LastEvaluation.TrainingStartTime;
			EvaluateButtonText.text = "再次评估";
		}
		else
		{
			NoEvaluateData.SetActive(true);
			PrintButton.SetActive(false); ;

			EvaluateTime.text = "点击右侧按钮对患者进行状况评估";
			EvaluateButtonText.text = "状况评估";
		}

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void EvaluateButtonOnclick()
	{

		SceneManager.LoadScene("Game");
	}
}


