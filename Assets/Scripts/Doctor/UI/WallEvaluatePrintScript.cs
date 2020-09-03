using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Vectrosity;
using System.Collections;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class WallEvaluatePrintScript : MonoBehaviour
    {

        public int WallEvaluationIndex;
        public OneTrainingData WallEvaluation;


        // Title
        public Text EvaluationTitle;

        // Information
        public Text InformationPatientID;
        public Text InformationPatientName;
        public Text InformationPatientSex;
        public Text InformationPatientAge;
        public Text InformationPatientHeight;
        public Text InformationPatientWeight;
        public Text InformationPatientSymptom;
        public Text InformationPatientDoctor;

        // Evaluation
        public Text EvaluationScore;
        public Text EvaluationDuration;
        public Text EvaluationRank;
        public Text EvaluationPassScore;
        public Text EvaluationAccuracyScore;
        public Text EvaluationTime;

        //// Chart
        public GameObject ActionFeedback;
        public GameObject JointFeedback;

        public Dictionary<int, int> toggleIndexTojointId;

        public Dropdown SecondItem;

        void OnEnable()
        {
            if (DoctorDataManager.instance.doctor.patient.WallEvaluations == null)
            {
                DoctorDataManager.instance.doctor.patient.WallEvaluations = DoctorDatabaseManager.instance.ReadPatientWallEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

                if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
                {
                    DoctorDataManager.instance.doctor.patient.SetWallEvaluationIndex(DoctorDataManager.instance.doctor.patient.WallEvaluations.Count - 1);
                }
            }

            if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
            {
                WallEvaluationIndex = SecondItem.value;
                WallEvaluation = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex];

                // Title
                string PatientNameBlock = "";
                for (int z = 0; z < DoctorDataManager.instance.doctor.patient.PatientName.Length; z++)
                {
                    PatientNameBlock += DoctorDataManager.instance.doctor.patient.PatientName[z] + "  ";
                }
                EvaluationTitle = transform.Find("EvaluationTitle").GetComponent<Text>();
                EvaluationTitle.text = PatientNameBlock + "第  " + (WallEvaluationIndex + 1).ToString() + "  次  动  作  姿  势  评  估  报  告  表";

                // Information
                InformationPatientID = transform.Find("Information/PatientInfo/ID/PatientID").GetComponent<Text>();
                InformationPatientID.text = DoctorDataManager.instance.doctor.patient.PatientID.ToString();

                InformationPatientName = transform.Find("Information/PatientInfo/Name/PatientName").GetComponent<Text>();
                InformationPatientName.text = DoctorDataManager.instance.doctor.patient.PatientName;

                InformationPatientSex = transform.Find("Information/PatientInfo/Sex/PatientSex").GetComponent<Text>();
                InformationPatientSex.text = DoctorDataManager.instance.doctor.patient.PatientSex;

                InformationPatientAge = transform.Find("Information/PatientInfo/Age/PatientAge").GetComponent<Text>();
                InformationPatientAge.text = DoctorDataManager.instance.doctor.patient.PatientAge.ToString() + " 岁";

                InformationPatientHeight = transform.Find("Information/PatientInfo/Height/PatientHeight").GetComponent<Text>();
                if (DoctorDataManager.instance.doctor.patient.PatientHeight == -1)
                {
                    InformationPatientHeight.text = "未填写";
                }
                else
                {
                    InformationPatientHeight.text = DoctorDataManager.instance.doctor.patient.PatientHeight.ToString() + " CM";
                }

                InformationPatientWeight = transform.Find("Information/PatientInfo/Weight/PatientWeight").GetComponent<Text>();
                if (DoctorDataManager.instance.doctor.patient.PatientWeight == -1)
                {
                    InformationPatientWeight.text = "未填写";
                }
                else
                {
                    InformationPatientWeight.text = DoctorDataManager.instance.doctor.patient.PatientWeight.ToString() + " KG";
                }

                InformationPatientSymptom = transform.Find("Information/PatientInfo/Symptom/PatientSymptom").GetComponent<Text>();
                InformationPatientSymptom.text = DoctorDataManager.instance.doctor.patient.PatientSymptom;

                InformationPatientDoctor = transform.Find("Information/PatientInfo/Doctor/PatientDoctor").GetComponent<Text>();
                InformationPatientDoctor.text = DoctorDataManager.instance.doctor.DoctorName;


                // Evaluation
                EvaluationScore = transform.Find("Evaluation/EvaluationInfo/Score/EvaluationScore").GetComponent<Text>();
                EvaluationScore.text = WallEvaluation.overrall.score.ToString("0.00") + " 分";

                EvaluationDuration = transform.Find("Evaluation/EvaluationInfo/Duration/EvaluationDuration").GetComponent<Text>();
                EvaluationDuration.text = (WallEvaluation.overrall.duration).ToString() + " 秒";

                EvaluationRank = transform.Find("Evaluation/EvaluationInfo/Rank/EvaluationRank").GetComponent<Text>();
                float rate = WallEvaluation.overrall.passScore / 100f;
                // 根据准确率显示评分: S A B C D
                if (rate >= 0.9 && rate <= 1)//S
                {
                    EvaluationRank.text = "S 级";
                }
                else if (rate >= 0.8 && rate < 0.9)//A
                {
                    EvaluationRank.text = "A 级";
                }
                else if (rate >= 0.7 && rate < 0.8)//B
                {
                    EvaluationRank.text = "B 级";
                }
                else if (rate >= 0.6 && rate < 0.7)//C
                {
                    EvaluationRank.text = "C 级";
                }
                else //D
                {
                    EvaluationRank.text = "D 级";
                }

                EvaluationPassScore = transform.Find("Evaluation/EvaluationInfo/PassScore/EvaluationPassScore").GetComponent<Text>();
                EvaluationPassScore.text = WallEvaluation.overrall.passScore.ToString("0.00") + "%";

                EvaluationAccuracyScore = transform.Find("Evaluation/EvaluationInfo/AccuracyScore/EvaluationAccuracyScore").GetComponent<Text>();
                EvaluationAccuracyScore.text = WallEvaluation.overrall.accuracyScore.ToString("0.00") + "%";

                EvaluationTime = transform.Find("Evaluation/EvaluationInfo/Time/EvaluationTime").GetComponent<Text>();
                string s = WallEvaluation.startTime;
                EvaluationTime.text = "20" + s.Substring(0, 2) + s.Substring(3, 2) + s.Substring(6, 2) + " " + s.Substring(9, 2) + ":" + s.Substring(12, 2) + ":00";


                // Chart
                // 初始化对比结果
                int ActionNum = Math.Min(WallEvaluation.overview.actionDatas.Count, 11);
                ActionFeedback.transform.localPosition = new Vector3(0f, 32f - (11 - ActionNum) * 5, 0f);
                JointFeedback.transform.localPosition = new Vector3(0f, -238f + (11 - ActionNum) * 7, 0f);

                List<int> ActionID = new List<int>(WallEvaluation.overview.actionDatas.Keys);

                for (int i = 0; i < ActionNum; i++)
                {
                    for (int z = 0; z < DoctorDataManager.instance.Actions.Count; z++)
                    {
                        if (DoctorDataManager.instance.Actions[z].id == ActionID[i])
                        {
                            this.transform.GetChild(4).GetChild(1).GetChild(i).GetChild(1).GetComponent<Text>().text = DoctorDataManager.instance.Actions[z].name;
                            this.transform.GetChild(4).GetChild(1).GetChild(i).GetChild(2).GetComponent<Text>().text = DoctorDataManager.instance.Actions[z].id.ToString();
                            this.transform.GetChild(4).GetChild(1).GetChild(i).GetChild(3).GetComponent<Text>().text = WallEvaluation.overview.actionDatas[ActionID[i]].accuracy.ToString() + " %";
                            this.transform.GetChild(4).GetChild(1).GetChild(i).GetChild(4).GetComponent<Text>().text = WallEvaluation.overview.actionDatas[ActionID[i]].passPercent.ToString() + " %";
                            break;
                        }
                    }
                }

                for(int i = ActionNum; i < 11; i++)
                {
                    this.transform.GetChild(4).GetChild(1).GetChild(i).gameObject.SetActive(false);
                }

                toggleIndexTojointId = new Dictionary<int, int>() { { 0, 2 }, { 1, 4 }, { 2, 8 }, { 3, 5 }, { 4, 9 }, { 5, 12 }, { 6, 16 }, { 7, 13 }, { 8, 17 }, { 9, 14 }, { 10, 18 } };

                float TempMethodEPS = 0f;
                for (int i = 0; i < 11; i++)
                {
                    this.transform.GetChild(4).GetChild(2).GetChild(i).GetChild(1).GetComponent<Text>().text = WallEvaluation.detail.jointDatas[toggleIndexTojointId[i]].passPercentScore.ToString("0.00") + "%";

                    TempMethodEPS = 0f;
                    foreach (var item in WallEvaluation.detail.jointDatas[toggleIndexTojointId[i]].methodDatas)
                    {
                        TempMethodEPS += item.Value.eps;
                    }

                    this.transform.GetChild(4).GetChild(2).GetChild(i).GetChild(2).GetComponent<Text>().text = (TempMethodEPS / WallEvaluation.detail.jointDatas[toggleIndexTojointId[i]].methodDatas.Count).ToString("0.00") + "%";
                }
            }

        }


       
        void Update()
        {

        }
    }
}