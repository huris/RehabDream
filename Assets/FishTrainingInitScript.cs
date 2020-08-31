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

	public int SingleTraining;

	public Dropdown TrainingSelect;
	public Dictionary<string, int> TrainingString2Int = new Dictionary<string, int>();
	public Dictionary<int, string> TrainingInt2String = new Dictionary<int, string>();
	public List<string> ListTrainingTime = new List<string>();

	// Training Evaluation
	// Rank
	public GameObject RankS;
	public GameObject RankA;
	public GameObject RankB;
	public GameObject RankC;
	public GameObject RankD;
	public GameObject RankE;

	public Text Direction;
	public Text Duration;
	public Text Score;
	public Text Experience;
	public Text Distance;
	public Text StartTime;
	public Text EndTime;

	


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

			SingleTraining = DoctorDataManager.instance.doctor.patient.FishTrainingIndex;

			TrainingString2Int.Clear();
			TrainingInt2String.Clear();
			ListTrainingTime.Clear();
			TrainingSelect.ClearOptions();

			for (int i = DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count - 1; i >= 0; i--)
			{
				string tempTrainingTime = DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].TrainingStartTime;
				ListTrainingTime.Add("第" + (i + 1).ToString() + "次 | " + tempTrainingTime.Substring(4, 2) + "." + tempTrainingTime.Substring(6, 2));
			}

			TrainingSelect.AddOptions(ListTrainingTime);
			TrainingSelect.value = DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count - 1 - DoctorDataManager.instance.doctor.patient.FishTrainingIndex;

			

		}
	}

	public void TrainingChange()
	{
		// 刷新评估界面
		DoctorDataManager.instance.doctor.patient.SetFishTrainingPlayIndex(DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count - 1 - TrainingSelect.value);

		OnEnable();
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
