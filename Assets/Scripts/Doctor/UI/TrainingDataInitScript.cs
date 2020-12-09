using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingDataInitScript : MonoBehaviour {

	public GameObject NoTrainingData;

	public GameObject Report;
	public Toggle TrainingToggle;

	public Text NoTrainingDataText;
	public Button TrainingButton;

	public GameObject PlanIsNoMaking;

	public Toggle TrainingPlanMakingToggle;

	public Toggle Training;

	public GameObject TrainingPlanData;

	// Use this for initialization
	void Start() {

	}

	void OnEnable()
	{
		NoTrainingData = transform.Find("NoTrainingData").gameObject;
		Report = transform.parent.parent.parent.Find("Report").gameObject;
		TrainingToggle = transform.parent.parent.parent.Find("Report/ReportToggle/TrainingToggle").GetComponent<Toggle>();
		PlanIsNoMaking.SetActive(false);

		//DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);

		if (DoctorDataManager.instance.doctor.patient.TrainingPlays == null)
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
				//print(DoctorDataManager.instance.doctor.patient.TrainingPlayIndex);
			}
		}

		if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0) NoTrainingData.SetActive(false);
		else
		{
			NoTrainingData.SetActive(true);

			//DoctorDataManager.instance.doctor.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID);
			//if (DoctorDataManager.instance.doctor.patient.trainingPlan != null) DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(true);
			//else DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(false);

			if (DoctorDataManager.instance.doctor.patient.Evaluations == null)
			{
				DoctorDataManager.instance.doctor.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

				if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
				{
					DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);
				}
			}

			if (DoctorDataManager.instance.doctor.patient.Evaluations == null || DoctorDataManager.instance.doctor.patient.Evaluations.Count == 0)
			{
				NoTrainingDataText.text = "该患者目前未进行Bobath评估\n\n请患者评估后再进行训练";

				TrainingButton.gameObject.SetActive(false);
			}
			else
			{
				NoTrainingDataText.text = "该患者目前暂未制定训练计划\n\n请制定计划后再进行训练";
				TrainingButton.gameObject.SetActive(true);
			}


			//TrainingButton.gameObject.SetActive(true);

			//if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
			//{
			//	NoTrainingDataText.text = "抱歉，该患者目前未进行相关训练\n\n请患者训练后再查看数据";

			//	TrainingButton.transform.GetChild(0).GetComponent<Text>().text = "开始训练";
			//}
			//else
			//{
			//NoTrainingDataText.text = "抱歉，该患者目前暂未制定训练计划\n\n请先制定计划后开始训练";

			//TrainingButton.transform.GetChild(0).GetComponent<Text>().text = "制定计划";
			//}
		}
	}

	// Update is called once per frame
	void Update() {

	}

	public void ReadReportButtonOnclick()
	{
		Report.SetActive(true);
		TrainingToggle.isOn = true;
		Training.isOn = true;
	}

	public void StartTraining()
	{
		if(DoctorDataManager.instance.doctor.patient.PlanIsMaking == false)
		{
			//PlanIsNoMaking.SetActive(true);
			return;
		}

		PatientDataManager.instance.SetUserMessage(DoctorDataManager.instance.doctor.patient.PatientID, DoctorDataManager.instance.doctor.patient.PatientName, DoctorDataManager.instance.doctor.patient.PatientSex);
		//PatientDataManager.instance.SetTrainingPlan(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.patient.trainingPlan.PlanDifficulty), DoctorDataManager.instance.patient.trainingPlan.GameCount, DoctorDataManager.instance.patient.trainingPlan.PlanCount);


		TrainingPlay trainingPlay = new TrainingPlay();
		trainingPlay.SetTrainingID(DoctorDatabaseManager.instance.ReadPatientRecordCount(0));
		trainingPlay.SetTrainingDifficulty(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty);

		if (DoctorDataManager.instance.doctor.patient.Evaluations == null)
		{
			DoctorDataManager.instance.doctor.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

			if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
			{
				DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);
			}
		}

		PatientDataManager.instance.SetTrainingData(trainingPlay, DoctorDataManager.instance.doctor.patient.trainingPlan, DoctorDataManager.instance.doctor.patient.Evaluations.Last().soccerDistance, DoctorDataManager.instance.doctor.patient.MaxSuccessCount);

		//DoctorDataManager.instance.doctor.patient.TrainingPlays.Add(trainingPlay);

		//PatientDataManager.instance.SetTrainingID(trainingPlay.TrainingID);
		//PatientDataManager.instance.SetMaxSuccessCount();
		//PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty));
		//PatientDataManager.instance.SetPlanCount(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanCount);
		//PatientDataManager.instance.SetPlanDirection(PatientDataManager.Str2DirectionType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDirection));
		//PatientDataManager.instance.SetPlanTime(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanTime);
		//PatientDataManager.instance.SetIsEvaluated(0);

		DoctorDataManager.instance.FunctionManager = 3; // 返回的时候进入训练状况查询界面
		SceneManager.LoadScene("06-Game");  // 如果登录成功,则进入医生管理界面
	}

	public void MakingPlanOnClick()
	{
		PlanIsNoMaking.SetActive(false);
		TrainingPlanMakingToggle = transform.parent.parent.parent.Find("FunctionManager/TrainingPlanMakingItem").GetComponent<Toggle>();
		TrainingPlanMakingToggle.isOn = true;
	}	

	public void TrainingPlanMakeButtonOnClick()
	{
		TrainingPlanData.SetActive(true);
	}

	public void TrainingPlanMakeReturnButtonOnClick()
	{
		TrainingPlanData.SetActive(false);
	}

}
