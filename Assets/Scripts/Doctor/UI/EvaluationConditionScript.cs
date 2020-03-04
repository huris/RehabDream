using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationConditionScript : MonoBehaviour
{

    public Text Difficult;
    public Text SuccessCount;
    public Text GameCount;
    public Text StartTime;
    public Text EndTime;

    public GameObject Rank1;
    public GameObject Rank2;
    public GameObject Rank3;
    public GameObject Rank4;
    public GameObject Rank5;


    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {
        Difficult = transform.Find("DifficultTitle/Difficult").GetComponent<Text>();
        SuccessCount = transform.Find("SuccessCountTitle/SuccessCount").GetComponent<Text>();
        GameCount = transform.Find("GameCountTitle/GameCount").GetComponent<Text>();
        StartTime = transform.Find("StartTimeTitle/StartTime").GetComponent<Text>();
        EndTime = transform.Find("EndTimeTitle/EndTime").GetComponent<Text>();

        Rank1 = transform.Find("Rank/1").gameObject;
        Rank1.SetActive(false);
        Rank2 = transform.Find("Rank/2").gameObject;
        Rank2.SetActive(false);
        Rank3 = transform.Find("Rank/3").gameObject;
        Rank3.SetActive(false);
        Rank4 = transform.Find("Rank/4").gameObject;
        Rank4.SetActive(false);
        Rank5 = transform.Find("Rank/5").gameObject;
        Rank5.SetActive(false);

        if(DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
        {
            int LastEvaluation = DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1;

            //Difficult.text = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].TrainingDifficulty;
            //SuccessCount.text = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].SuccessCount.ToString();
            //GameCount.text = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].GameCount.ToString();
            StartTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].EvaluationStartTime;
            EndTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].EvaluationEndTime;

            //double TrainingEvaluationRate = 1.0 * DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].SuccessCount / DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].GameCount;

            //if (TrainingEvaluationRate >= 0.95) Rank1.SetActive(true);
            //else if (TrainingEvaluationRate >= 0.90) Rank2.SetActive(true);
            //else if (TrainingEvaluationRate >= 0.80) Rank3.SetActive(true);
            //else if (TrainingEvaluationRate >= 0.70) Rank4.SetActive(true);
            //else Rank5.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
