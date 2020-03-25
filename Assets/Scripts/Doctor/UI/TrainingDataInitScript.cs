using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingDataInitScript : MonoBehaviour {

	public GameObject NoTrainingData;

	public GameObject Report;
	public Toggle TrainingToggle;

	public Text NoTrainingDataText;
	public Button TrainingButton;

	// Use this for initialization
	void Start() {

	}

	void OnEnable()
	{
		NoTrainingData = transform.Find("NoTrainingData").gameObject;
		Report = transform.parent.parent.parent.Find("Report").gameObject;
		TrainingToggle = transform.parent.parent.parent.Find("Report/Training").GetComponent<Toggle>();

		//DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);

		if(DoctorDataManager.instance.doctor.patient.TrainingPlays == null)
		{
			DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);
			if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
			{
				foreach (var item in DoctorDataManager.instance.doctor.patient.TrainingPlays)
				{
					if (DoctorDataManager.instance.doctor.patient.MaxSuccessCount < item.SuccessCount)
					{
						DoctorDataManager.instance.doctor.patient.SetMaxSuccessCount(item.SuccessCount);
					}
				}

				DoctorDataManager.instance.doctor.patient.SetTrainingPlayIndex(DoctorDataManager.instance.doctor.patient.TrainingPlays.Count - 1);
			}
		}

		if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0) NoTrainingData.SetActive(false);
		else
		{ 
			NoTrainingData.SetActive(true);

			if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
			{
				TrainingButton.gameObject.SetActive(true);
				NoTrainingDataText.text = "抱歉，该患者目前未进行相关训练\n\n请患者训练后再查看数据";
			}
			else
			{
				TrainingButton.gameObject.SetActive(false);
				NoTrainingDataText.text = "抱歉，该患者目前暂未制定训练计划\n\n请先制定计划后开始训练";
			}
		}
	}

	// Update is called once per frame
	void Update() {

	}

	public void ReadReportButtonOnclick()
	{
		Report.SetActive(true);
		TrainingToggle.isOn = true;
	}

	public void StartTraining()
	{
		PatientDataManager.instance.SetUserMessage(DoctorDataManager.instance.doctor.patient.PatientID, DoctorDataManager.instance.doctor.patient.PatientName, DoctorDataManager.instance.doctor.patient.PatientSex);
		//PatientDataManager.instance.SetTrainingPlan(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.patient.trainingPlan.PlanDifficulty), DoctorDataManager.instance.patient.trainingPlan.GameCount, DoctorDataManager.instance.patient.trainingPlan.PlanCount);

		TrainingPlay trainingPlay = new TrainingPlay();
		trainingPlay.SetTrainingID(DoctorDatabaseManager.instance.ReadPatientRecordCount(0) + DoctorDatabaseManager.instance.ReadPatientRecordCount(1));

		//DoctorDataManager.instance.doctor.patient.TrainingPlays.Add(trainingPlay);

		PatientDataManager.instance.SetTrainingID(trainingPlay.TrainingID);
		PatientDataManager.instance.SetMaxSuccessCount(DoctorDataManager.instance.doctor.patient.MaxSuccessCount);
		PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty));
		PatientDataManager.instance.SetPlanCount(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanCount);
		PatientDataManager.instance.SetPlanDirection(PatientDataManager.Str2DirectionType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDirection));
		PatientDataManager.instance.SetPlanTime(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanTime);
		PatientDataManager.instance.SetIsEvaluated(0);

		DoctorDataManager.instance.FunctionManager = 3; // 返回的时候进入训练状况查询界面
		SceneManager.LoadScene("06-Game");  // 如果登录成功,则进入医生管理界面
	}
}
