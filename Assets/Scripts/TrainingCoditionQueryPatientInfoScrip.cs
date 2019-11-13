using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCoditionQueryPatientInfoScrip : MonoBehaviour {

    public Text PatientName;
    public Text PatientSex;
    public Text PatientAge;
    public Text PatientHeight;
    public Text PatientWeight;

    public GameObject TrainingPlanNullText;

    public GameObject TrainingDifficulty;
    public Text TrainingDifficultyText;

    public GameObject GameCount;
    public Text GameCountText;

    public GameObject PlanCount;
    public Text PlanCountText;


    // Use this for initialization
    void OnEnable () 
    {
        PatientName = transform.Find("Name/PatientName").GetComponent<Text>();
        PatientSex = transform.Find("Sex/PatientSex").GetComponent<Text>();
        PatientAge = transform.Find("Age/PatientAge").GetComponent<Text>();
        PatientHeight = transform.Find("Height/PatientHeight").GetComponent<Text>();
        PatientWeight = transform.Find("Weight/PatientWeight").GetComponent<Text>();

        PatientName.text = DoctorDataManager.instance.patient.PatientName;
        PatientSex.text = DoctorDataManager.instance.patient.PatientSex;
        PatientAge.text = DoctorDataManager.instance.patient.PatientAge.ToString();
        PatientHeight.text = DoctorDataManager.instance.patient.PatientHeight.ToString();
        PatientWeight.text = DoctorDataManager.instance.patient.PatientWeight.ToString();

        TrainingPlanNullText = transform.Find("TrainingPlan/TrainingPlanImage/TrainingPlanNullText").gameObject;
        if (DoctorDataManager.instance.patient.trainingPlan.PlanIsMaking)
        {
            TrainingPlanNullText.SetActive(false);
            TrainingDifficulty.SetActive(true);
            GameCount.SetActive(true);
            PlanCount.SetActive(true);
        }
        else
        {
            TrainingPlanNullText.SetActive(true);
            TrainingDifficulty.SetActive(false);
            GameCount.SetActive(false);
            PlanCount.SetActive(false);
        }
        TrainingDifficulty = transform.Find("TrainingPlan/TrainingPlanImage/TrainingDifficulty").gameObject;
        TrainingDifficultyText = transform.Find("TrainingPlan/TrainingPlanImage/TrainingDifficulty/TrainingDifficultyText").GetComponent<Text>();
        GameCount = transform.Find("TrainingPlan/TrainingPlanImage/GameCount").gameObject;
        GameCountText = transform.Find("TrainingPlan/TrainingPlanImage/GameCount/GameCountText").GetComponent<Text>();
        PlanCount = transform.Find("TrainingPlan/TrainingPlanImage/PlanCount").gameObject;
        PlanCountText = transform.Find("TrainingPlan/TrainingPlanImage/PlanCount/PlanCountText").GetComponent<Text>();

        if (DoctorDataManager.instance.patient.trainingPlan.PlanIsMaking)
        {
            TrainingDifficultyText.text = DoctorDataManager.instance.patient.trainingPlan.PlanDifficulty;
            GameCountText.text = DoctorDataManager.instance.patient.trainingPlan.GameCount.ToString();
            PlanCountText.text = DoctorDataManager.instance.patient.trainingPlan.PlanCount.ToString();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
