using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingTypeScript : MonoBehaviour
{

	public Toggle SoccerTrainingToggle;
	public Toggle FishTrainingToggle;
	public Toggle SimsTrainingToggle;

	public Image SoccerTrainingImage;
	public Image FishTrainingImage;
	public Image SimsTrainingImage;

	public Text SoccerTrainingText;
	public Text FishTrainingText;
	public Text SimsTrainingText;

	public GameObject Data;
	public GameObject FishData;
	public GameObject SimsData;

	// Use this for initialization

	void OnEnable()
	{
		//print(DoctorDataManager.instance.EvaluationType + "!!!!!");
		DoctorDataManager.instance.FunctionManager = 3;

		if (DoctorDataManager.instance.TrainingType == 0)
		{
			SoccerTrainingToggle.isOn = true;
		}
		else if (DoctorDataManager.instance.TrainingType == 1)
		{
			FishTrainingToggle.isOn = true;
		}else if(DoctorDataManager.instance.TrainingType == 2)
		{
			SimsTrainingToggle.isOn = true;
		}
	}

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SoccerTrainingToggleIsOn()
	{
		if (SoccerTrainingToggle.isOn)
		{
			DoctorDataManager.instance.TrainingType = 0;

			SoccerTrainingImage.color = new Color32(107, 125, 247, 255);
			FishTrainingImage.color = new Color32(233, 239, 244, 255);
			SimsTrainingImage.color = new Color32(233, 239, 244, 255);

			SoccerTrainingText.color = new Color32(255, 255, 255, 255);
			FishTrainingText.color = new Color32(165, 165, 165, 255);
			SimsTrainingText.color = new Color32(165, 165, 165, 255);

			Data.SetActive(true);
			FishData.SetActive(false);
			SimsData.SetActive(false);
		}
	}

	public void FishTrainingToggleIsOn()
	{
		if (FishTrainingToggle.isOn)
		{
			DoctorDataManager.instance.TrainingType = 1;

			SoccerTrainingImage.color = new Color32(233, 239, 244, 255);
			FishTrainingImage.color = new Color32(107, 125, 247, 255);
			SimsTrainingImage.color = new Color32(233, 239, 244, 255);

			SoccerTrainingText.color = new Color32(165, 165, 165, 255);
			FishTrainingText.color = new Color32(255, 255, 255, 255);
			SimsTrainingText.color = new Color32(165, 165, 165, 255);

			Data.SetActive(false);
			FishData.SetActive(true);
			SimsData.SetActive(false);
		}
	}

	public void SimsTrainingToggleIsOn()
	{
		if (SimsTrainingToggle.isOn)
		{
			DoctorDataManager.instance.TrainingType = 2;

			SoccerTrainingImage.color = new Color32(233, 239, 244, 255);
			FishTrainingImage.color = new Color32(233, 239, 244, 255);
			SimsTrainingImage.color = new Color32(107, 125, 247, 255);

			SoccerTrainingText.color = new Color32(165, 165, 165, 255);
			FishTrainingText.color = new Color32(165, 165, 165, 255);
			SimsTrainingText.color = new Color32(255, 255, 255, 255);

			Data.SetActive(false);
			FishData.SetActive(false);
			SimsData.SetActive(true);
		}
	}
}
