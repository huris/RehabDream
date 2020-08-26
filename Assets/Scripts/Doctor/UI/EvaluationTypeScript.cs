using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationTypeScript : MonoBehaviour {

	public Toggle ThroughWallToggle;
	public Toggle BobathToggle;

	public Image ThroughWallImage;
	public Image BobathImage;

	public Text ThroughWallText;
	public Text BobathText;

	public GameObject Data;
	public GameObject ThroughData;

	// Use this for initialization

	void OnEnable()
	{
		//print(DoctorDataManager.instance.EvaluationType + "!!!!!");

		if(DoctorDataManager.instance.EvaluationType == 0)
		{
			ThroughWallToggle.isOn = true;
		}
		else if (DoctorDataManager.instance.EvaluationType == 1)
		{
			BobathToggle.isOn = true;
		}

	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ThroughWallToggleIsOn()
	{
		if(ThroughWallToggle.isOn)
		{
			DoctorDataManager.instance.EvaluationType = 0;

			ThroughWallImage.color = new Color32(107, 125, 247, 255);
			BobathImage.color = new Color32(233, 239, 244, 255);

			ThroughWallText.color = new Color32(255, 255, 255, 255);
			BobathText.color = new Color32(165, 165, 165, 255);

			ThroughData.SetActive(true);
			Data.SetActive(false);
		}
	}

	public void BobathToggleIsOn()
	{
		if (BobathToggle.isOn)
		{
			DoctorDataManager.instance.EvaluationType = 1;

			ThroughWallImage.color = new Color32(233, 239, 244, 255); 
			BobathImage.color = new Color32(107, 125, 247, 255);

			ThroughWallText.color = new Color32(165, 165, 165, 255);
			BobathText.color = new Color32(255, 255, 255, 255); 

			Data.SetActive(true);
			ThroughData.SetActive(false);
		}
	}
}
