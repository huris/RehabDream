using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationConditionScript : MonoBehaviour
{

    public Text EvaluationRank;
    public Text EvaluationScore;
    public Text EvaluationTime;
    public Text EvaluationStartTime;
    public Text EvaluationEndTime;

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
        EvaluationRank = transform.Find("EvaluationRank/Rank").GetComponent<Text>();
        EvaluationScore = transform.Find("EvaluationScore/Score").GetComponent<Text>();
        EvaluationTime = transform.Find("EvaluationTime/Time").GetComponent<Text>();
        EvaluationStartTime = transform.Find("StartTimeTitle/StartTime").GetComponent<Text>();
        EvaluationEndTime = transform.Find("EndTimeTitle/EndTime").GetComponent<Text>();

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

            //double TrainingEvaluationRate = 1.0 * DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].SuccessCount / DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].GameCount;

            float TrainingEvaluationRate = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].EvaluationScore;

            if (TrainingEvaluationRate >= 0.95f) { Rank1.SetActive(true); EvaluationRank.text = "1 级"; }
            else if (TrainingEvaluationRate >= 0.90f) { Rank2.SetActive(true); EvaluationRank.text = "2 级"; }
            else if (TrainingEvaluationRate >= 0.80f) { Rank3.SetActive(true); EvaluationRank.text = "3 级"; }
            else if (TrainingEvaluationRate >= 0.70f) { Rank4.SetActive(true); EvaluationRank.text = "4 级"; }
            else { Rank5.SetActive(true); EvaluationRank.text = "5 级"; }

            EvaluationScore.text = TrainingEvaluationRate.ToString("0.00");

            EvaluationStartTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].EvaluationStartTime;
            EvaluationEndTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[LastEvaluation].EvaluationEndTime;
            
            // 计算有训练时长
            EvaluationTime.text = (long.Parse(EvaluationEndTime.text.Substring(9, 2)) * 60 + long.Parse(EvaluationEndTime.text.Substring(12, 2))
                                       - long.Parse(EvaluationStartTime.text.Substring(9, 2)) * 60 - long.Parse(EvaluationStartTime.text.Substring(12, 2))).ToString() + "min";

        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
