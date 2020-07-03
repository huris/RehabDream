using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.UI;

public class ThroughWallDataInitScript : MonoBehaviour {

    public List<GameObject> Ranks;

    public Text WallEvaluationScore;
    public Text LastWallEvaluationScore;

    public Text WallEvaluationPassScore;
    public Text WallEvaluationAccuracyScore;
    public Text WallEvaluationDuration;
    public Text WallEvaluationActionNum;
    public Text WallEvaluationCompliance;

    public Text WallEvaluationStartTime;

    void OnEnable()
	{
        if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
        {
            int WallEvaluationIndex = DoctorDataManager.instance.doctor.patient.WallEvaluationIndex;

            float rate = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.passScore / 100f;

            foreach (var item in Ranks)
            {
                item.SetActive(false);
            }
            // 根据准确率显示评分: S A B C D
            if (rate >= 0.9 && rate <= 1)//S
            {
                Ranks[0].SetActive(true);
            }
            else if (rate >= 0.8 && rate < 0.9)//A
            {
                Ranks[1].SetActive(true);
            }
            else if (rate >= 0.7 && rate < 0.8)//B
            {
                Ranks[2].SetActive(true);
            }
            else if (rate >= 0.6 && rate < 0.7)//C
            {
                Ranks[3].SetActive(true);
            }
            else //D
            {
                Ranks[4].SetActive(true);
            }

            WallEvaluationScore.text = "评分: " + DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.score.ToString();
            LastWallEvaluationScore.text = "上次评分: " + DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.lastScore.ToString();

            WallEvaluationPassScore.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.passScore.ToString() + "%";
            WallEvaluationAccuracyScore.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.accuracyScore.ToString() + "%";
            WallEvaluationDuration.text = (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.duration * 60).ToString() + "s";
            WallEvaluationActionNum.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.actionNum.ToString() + "个";
            WallEvaluationCompliance.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.compliance.ToString();

            string s = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].startTime;
            WallEvaluationStartTime.text = "20" + s.Substring(0, 2) + s.Substring(3, 2) + s.Substring(6, 2) + " " + s.Substring(9, 2) + ":" + s.Substring(12, 2) + ":00";
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


   
}
