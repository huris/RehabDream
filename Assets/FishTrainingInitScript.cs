using ShipNSea;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishTrainingInitScript : MonoBehaviour {

	public GameObject TrainingPlanData;

	public GameObject NoTrainingData;

	public Dropdown TrainingDirection;
	public InputField TrainingDuration;


	void OnEnable()
	{
		if (DoctorDataManager.instance.doctor.patient.FishTrainingPlays == null)
		{
			DoctorDataManager.instance.doctor.patient.FishTrainingPlays = DoctorDatabaseManager.instance.ReadPatientFishTrainings(DoctorDataManager.instance.doctor.patient.PatientID);
			if (DoctorDataManager.instance.doctor.patient.FishTrainingPlays != null && DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count > 0)
			{
				//foreach (var item in DoctorDataManager.instance.doctor.patient.Fish)
				//{
				//	if (DoctorDataManager.instance.doctor.patient.MaxSuccessCount < item.SuccessCount)
				//	{
				//		DoctorDataManager.instance.doctor.patient.SetMaxSuccessCount(item.SuccessCount);
				//	}
				//}

				DoctorDataManager.instance.doctor.patient.SetFishTrainingPlayIndex(DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count - 1);
				//print(DoctorDataManager.instance.doctor.patient.TrainingPlayIndex);
			}
		}

		if (DoctorDataManager.instance.doctor.patient.FishTrainingPlays != null && DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count > 0) NoTrainingData.SetActive(false);
		else
		{
			NoTrainingData.SetActive(true);

			//DoctorDataManager.instance.doctor.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID);
			//if (DoctorDataManager.instance.doctor.patient.trainingPlan != null) DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(true);
			//else DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(false);

			//if (DoctorDataManager.instance.doctor.patient.Evaluations == null || DoctorDataManager.instance.doctor.patient.Evaluations.Count == 0)
			//{
			//	NoTrainingDataText.text = "该患者目前未进行Bobath评估\n\n请患者评估后再进行训练";

			//	TrainingButton.gameObject.SetActive(false);
			//}
			//else
			//{
			//	NoTrainingDataText.text = "该患者目前暂未制定训练计划\n\n请制定计划后再进行训练";
			//	TrainingButton.gameObject.SetActive(true);
			//}


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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FishTrainingStartButtonOnClick()
	{
		if(DoctorDataManager.instance.doctor.patient.FishPlanIsMaking)
		{
			if (DoctorDataManager.instance.doctor.patient.FishTrainingPlays == null)
			{
				DoctorDataManager.instance.doctor.patient.FishTrainingPlays = new List<FishTrainingPlay>();
			}

			DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Add(new FishTrainingPlay());
			DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Last().SetTrainingDirection(TrainingDirection.value);

			MapDetectionController.mapOccupyDis.Clear();

			ShipNSea.GameState.returnScene = "03-DoctorUI";
			
			//关闭页面按钮事件
			ShipNSea.GameState.CloseFunc = FishTrainingIsComplete;

			//存放id 
			IntroState.pPwd = DoctorDataManager.instance.doctor.patient.PatientID.ToString();
			//存放姓名
			IntroState.pName = DoctorDataManager.instance.doctor.patient.PatientName;
			//改变训练时间
			GameTime.gameTimeTotal = int.Parse(TrainingDuration.text) * 60;
			//改变训练着重侧
			BodySetting.setBody = (BodySettingEnum)DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Last().TrainingDirection;

			SceneManager.LoadScene("Intro");  // 如果登录成功,则进入医生管理界面
		}
		else
		{

		}
	}

	public void FishTrainingIsComplete()
	{
		DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Last().SetFishTrainingPlay(
			long.Parse(ShipNSea.GameState.outUserDAO.gotStaticFishCount) * 100 + long.Parse(ShipNSea.GameState.outUserDAO.gotDynamicFishCount) * 150,
			long.Parse(ShipNSea.GameState.outUserDAO.gotStaticFishCount),
			ShipNSea.GameState.outUserDAO.staticFishCount,
			long.Parse(ShipNSea.GameState.outUserDAO.gotDynamicFishCount),
			ShipNSea.GameState.outUserDAO.dynamicFishCount,
			ShipNSea.GameState.outUserDAO.eachFishGotCastTime,
			long.Parse(ShipNSea.GameState.outUserDAO.gotExp),
			long.Parse(ShipNSea.GameState.outUserDAO.distance),
			ShipNSea.GameState.outUserDAO.gList,
			long.Parse(ShipNSea.GameState.outUserDAO.gotStaticFishCount) * 100 + long.Parse(ShipNSea.GameState.outUserDAO.gotDynamicFishCount) * 150
			);

		PatientDatabaseManager.instance.WritePatientFishRecord(DoctorDataManager.instance.doctor.patient.PatientID,
			DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Last());
	}


	public void FishTrainingPlanMakeButtonOnClick()
	{
		TrainingPlanData.SetActive(true);
	}

	public void FishTrainingPlanReturnButtonOnClick()
	{
		TrainingPlanData.SetActive(false);
	}


	public void ReadFishReportButtonOnclick()
	{
		//Report.SetActive(true);
		//TrainingToggle.isOn = true;
		//Training.isOn = true;
	}

}
