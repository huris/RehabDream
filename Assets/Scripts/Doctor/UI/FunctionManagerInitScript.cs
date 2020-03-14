using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionManagerInitScript : MonoBehaviour {

	// Use this for initialization

	public Toggle[] FunctionToggle; 
	void Start () {
		
	}

	void OnEnable()
	{	
		for(int i = 0; i < 4; i++)
		{
			FunctionToggle[0].isOn = false;
		}

		FunctionToggle[DoctorDataManager.instance.FunctionManager].isOn = true;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
