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
    public Text PatientSymptom;
    public Text PatientDoctor;

    public GameObject TrainingPlanNullText;

    public GameObject PlanDifficulty;
    public Text PlanDifficultyText;

    public GameObject PlanDirection;
    public Text PlanDirectionText;

    public GameObject PlanTime;
    public Text PlanTimeText;

    public Toggle PatientInfoManagerItem;

    // Use this for initialization
    void OnEnable () 
    {
        PatientName = transform.Find("Name/PatientName").GetComponent<Text>();
        PatientSex = transform.Find("Sex/PatientSex").GetComponent<Text>();
        PatientAge = transform.Find("Age/PatientAge").GetComponent<Text>();
        PatientHeight = transform.Find("Height/PatientHeight").GetComponent<Text>();
        PatientWeight = transform.Find("Weight/PatientWeight").GetComponent<Text>();
        PatientSymptom = transform.Find("Symptom/PatientSymptom").GetComponent<Text>();
        PatientDoctor = transform.Find("Doctor/PatientDoctor").GetComponent<Text>();

        PatientInfoManagerItem = transform.parent.parent.parent.Find("FunctionManager/PatentInfoManagerItem").GetComponent<Toggle>();

        if(DoctorDataManager.instance.doctor.patient != null)
        {
            PatientName.text = DoctorDataManager.instance.doctor.patient.PatientName;
            PatientSex.text = DoctorDataManager.instance.doctor.patient.PatientSex;
            PatientAge.text = DoctorDataManager.instance.doctor.patient.PatientAge.ToString();

            if (DoctorDataManager.instance.doctor.patient.PatientHeight == -1)
            {
                PatientHeight.text = "NULL";
            }
            else
            {
                PatientHeight.text = DoctorDataManager.instance.doctor.patient.PatientHeight.ToString()+" CM";
            }

            if (DoctorDataManager.instance.doctor.patient.PatientWeight == -1)
            {
                PatientWeight.text = "NULL";
            }
            else
            {
                PatientWeight.text = DoctorDataManager.instance.doctor.patient.PatientWeight.ToString() + " KG";
            }

            PatientSymptom.text = DoctorDataManager.instance.doctor.patient.PatientSymptom;
            //PatientDoctor.text = DoctorDatabaseManager.instance.ReadDoctorIDInfo(DoctorDataManager.instance.doctor.patient.PatientDoctorID).DoctorName;
            PatientDoctor.text = DoctorDataManager.instance.doctor.patient.PatientDoctorName;

            TrainingPlanNullText = transform.Find("TrainingPlan/TrainingPlanImage/TrainingPlanNullText").gameObject;
            PlanDifficulty = transform.Find("TrainingPlan/TrainingPlanImage/PlanDifficulty").gameObject;
            PlanDifficultyText = transform.Find("TrainingPlan/TrainingPlanImage/PlanDifficulty/Text").GetComponent<Text>();
            PlanDirection = transform.Find("TrainingPlan/TrainingPlanImage/PlanDirection").gameObject;
            PlanDirectionText = transform.Find("TrainingPlan/TrainingPlanImage/PlanDirection/Text").GetComponent<Text>();
            PlanTime = transform.Find("TrainingPlan/TrainingPlanImage/PlanTime").gameObject;
            PlanTimeText = transform.Find("TrainingPlan/TrainingPlanImage/PlanTime/Text").GetComponent<Text>();

            //print(DoctorDataManager.instance.doctor.patient.PlanIsMaking + "!!!!!");

            if (DoctorDataManager.instance.doctor.patient.trainingPlan == null) DoctorDataManager.instance.doctor.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID);
            if (DoctorDataManager.instance.doctor.patient.trainingPlan != null) DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(true);
            else DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(false);

            if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
            {
                TrainingPlanNullText.SetActive(false);
                PlanDifficulty.SetActive(true);
                PlanDirection.SetActive(true);
                PlanTime.SetActive(true);
            }
            else
            {
                TrainingPlanNullText.SetActive(true);
                PlanDifficulty.SetActive(false);
                PlanDirection.SetActive(false);
                PlanTime.SetActive(false);
            }

            if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
            {
                PlanDifficultyText.text = DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty;
                PlanDirectionText.text = DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDirection.ToString();
                PlanTimeText.text = DoctorDataManager.instance.doctor.patient.trainingPlan.PlanTime.ToString() + " min";
            }
        }
        else
        {
            PatientInfoManagerItem.isOn = true;
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
