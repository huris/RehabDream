using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainEvaluationInitScript : MonoBehaviour {

    public Text Difficult;
    public Text SuccessCount;
    public Text GameCount;
    public Text StartTime;
    public Text EndTime;

    // Use this for initialization
    void Start () {
		
	}

    void OnEnable()
    {
        Difficult = transform.Find("DifficultTitle/Difficult").GetComponent<Text>();
        SuccessCount = transform.Find("SuccessCountTitle/SuccessCount").GetComponent<Text>();
        GameCount = transform.Find("GameCountTitle/GameCount").GetComponent<Text>();
        StartTime = transform.Find("StartTimeTitle/StartTime").GetComponent<Text>();
        EndTime = transform.Find("EndTimeTitle/EndTime").GetComponent<Text>();

        if(DoctorDataManager.instance.patient.trainingPlays.Count > 0)
        {
            int LastTrainingPlay = DoctorDataManager.instance.patient.trainingPlays.Count - 1;
            
            Difficult.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].TrainingDifficulty;
            SuccessCount.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].SuccessCount.ToString();
            GameCount.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].GameCount.ToString();
            StartTime.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].TrainingStartTime;
            EndTime.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].TrainingEndTime;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
