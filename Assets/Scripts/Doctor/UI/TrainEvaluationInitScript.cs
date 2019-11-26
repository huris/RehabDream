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
    
    public GameObject RankS;
    public GameObject RankA;
    public GameObject RankB;
    public GameObject RankC;
    public GameObject RankD;
    public GameObject RankE;


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

        RankS = transform.Find("Rank/S").gameObject;
        RankS.SetActive(false);
        RankA = transform.Find("Rank/A").gameObject;
        RankA.SetActive(false);
        RankB = transform.Find("Rank/B").gameObject;
        RankB.SetActive(false);
        RankC = transform.Find("Rank/C").gameObject;
        RankC.SetActive(false);
        RankD = transform.Find("Rank/D").gameObject;
        RankD.SetActive(false);
        RankE = transform.Find("Rank/E").gameObject;
        RankE.SetActive(false);

        if (DoctorDataManager.instance.patient.trainingPlays.Count > 0)
        {
            int LastTrainingPlay = DoctorDataManager.instance.patient.trainingPlays.Count - 1;
            
            Difficult.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].TrainingDifficulty;
            SuccessCount.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].SuccessCount.ToString();
            GameCount.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].GameCount.ToString();
            StartTime.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].TrainingStartTime;
            EndTime.text = DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].TrainingEndTime;
            
            double TrainingEvaluationRate = 1.0 * DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].SuccessCount / DoctorDataManager.instance.patient.trainingPlays[LastTrainingPlay].GameCount;

            if (TrainingEvaluationRate >= 0.95) RankS.SetActive(true);
            else if (TrainingEvaluationRate >= 0.90) RankA.SetActive(true);
            else if (TrainingEvaluationRate >= 0.80) RankB.SetActive(true);
            else if (TrainingEvaluationRate >= 0.70) RankC.SetActive(true);
            else if (TrainingEvaluationRate >= 0.60) RankD.SetActive(true);
            else RankE.SetActive(true);

        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
