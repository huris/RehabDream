using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class ThroughWallDataInitScript : MonoBehaviour {

	void OnEnable()
	{
		if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
		{
			List<int> test = new List<int>(DoctorDataManager.instance.doctor.patient.WallEvaluations[1].overview.actionDatas.Keys);

			for (int i = 0; i < DoctorDataManager.instance.doctor.patient.WallEvaluations[1].overview.actionDatas.Count; i++)
			{

			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


   
}
