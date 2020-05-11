using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoctorNameDisplayScript : MonoBehaviour {

    public Text DoctorNameDisplay;

	// Use this for initialization
	void Start () {
        try
        {
            DoctorNameDisplay = GameObject.Find("DoctorNameDisplay").GetComponent<Text>();
            DoctorNameDisplay.text = DoctorDataManager.instance.doctor.DoctorName;
        }
        catch (Exception e) { 
        
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
