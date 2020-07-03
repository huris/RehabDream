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
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ThroughWallToggleIsOn()
	{
		if(ThroughWallToggle.isOn)
		{
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
			ThroughWallImage.color = new Color32(233, 239, 244, 255); 
			BobathImage.color = new Color32(107, 125, 247, 255);

			ThroughWallText.color = new Color32(165, 165, 165, 255);
			BobathText.color = new Color32(255, 255, 255, 255); 

			Data.SetActive(true);
			ThroughData.SetActive(false);
		}
	}
}
