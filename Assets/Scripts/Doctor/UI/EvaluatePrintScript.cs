using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class EvaluatePrintScript : MonoBehaviour
    {
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
        public Text EvaluationResult;
        public Text EvaluationTime;

        // Directions
        public RadarChart DirectionRadarChart; // 雷达图
        public Serie serie, serie1;
        public Text RadarArea; // 雷达图面积
        public Text UponDirection;
        public Text UponRightDirection;
        public Text RightDirection;
        public Text DownRightDirection;
        public Text DownDirection;
        public Text DownLeftDirection;
        public Text LeftDirection;
        public Text UponLeftDirection;

        // Surroundings
        private LineChart SurroundingChart;   // 身体左右倾角
        //private Serie LeftSerie;
        //private Serie RightSerie;
        //private Serie FrontSerie;
        //private Serie BehindSerie;
        private long SurroundingCount;


        public int SingleEvaluationIndex;
        public Evaluation evaluation;

        void OnEnable()
        {
            if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
            {
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
                if(DoctorDataManager.instance.doctor.patient.PatientHeight == -1)
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
                SingleEvaluationIndex = DoctorDataManager.instance.doctor.patient.EvaluationIndex;
                evaluation = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluationIndex];

                EvaluationScore = transform.Find("Evaluation/EvaluationInfo/Score/EvaluationScore").GetComponent<Text>();
                EvaluationScore.text = evaluation.EvaluationScore.ToString("0.00") + " 分";
                
                EvaluationDuration = transform.Find("Evaluation/EvaluationInfo/Duration/EvaluationDuration").GetComponent<Text>();
                EvaluationDuration.text = (long.Parse(evaluation.EvaluationEndTime.Substring(9, 2)) * 3600 + long.Parse(evaluation.EvaluationEndTime.Substring(12, 2)) * 60 + long.Parse(evaluation.EvaluationEndTime.Substring(15, 2))
                                           - long.Parse(evaluation.EvaluationStartTime.Substring(9, 2)) * 3600 - long.Parse(evaluation.EvaluationStartTime.Substring(12, 2)) * 60 - long.Parse(evaluation.EvaluationStartTime.Substring(15, 2))).ToString() + " 秒";

                EvaluationRank = transform.Find("Evaluation/EvaluationInfo/Rank/EvaluationRank").GetComponent<Text>();
                float TrainingEvaluationRate = evaluation.EvaluationScore;
                if (TrainingEvaluationRate >= 0.95f) { EvaluationRank.text = "1 级"; }
                else if (TrainingEvaluationRate >= 0.90f) {EvaluationRank.text = "2 级"; }
                else if (TrainingEvaluationRate >= 0.80f) {EvaluationRank.text = "3 级"; }
                else if (TrainingEvaluationRate >= 0.70f) {EvaluationRank.text = "4 级"; }
                else {EvaluationRank.text = "5 级"; }

                EvaluationResult = transform.Find("Evaluation/EvaluationInfo/Result/EvaluationResult").GetComponent<Text>();
                List<string> HemiPos = new List<string>();  // 偏瘫方位

                //if(evaluation.soccerDistance.UponSoccerDistance)



                EvaluationTime = transform.Find("Evaluation/EvaluationInfo/Time/EvaluationTime").GetComponent<Text>();


                //    //if (DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].EvaluationScore == 0.0f)
                //    //{
                //    //    DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].SetEvaluationScore();
                //    //}

                //    EvaluationScore.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].EvaluationScore.ToString("0.00");
                //    EvaluationRank.text = "(           )";
                //    EvaluationSuccessCount.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].SuccessCount.ToString() + "/" +
                //        DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].GameCount.ToString() + "(" +
                //        (1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].SuccessCount /
                //         DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].GameCount * 100).ToString("0.00") + "%)";
                //    EvaluationTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].TrainingStartTime;
                //    //EvaluationResult.text

                //    DirectionRadarChart = transform.Find("Chart/Directions/RadarChart").GetComponent<RadarChart>();
                //    if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("Chart/Directions/RadarChart").gameObject.AddComponent<RadarChart>();
                //    DirectionRadarChart.ClearData();
                //    DirectionRadarChart.AddData(0, DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.GetDirections());

                //    RadarArea = transform.Find("Chart/Directions/RadarArea/Text").GetComponent<Text>();
                //    UponDirection = transform.Find("Chart/Directions/DirectionFrame/UponDirection/Upon/Text").GetComponent<Text>();
                //    UponRightDirection = transform.Find("Chart/Directions/DirectionFrame/UponRightDirection/UponRight/Text").GetComponent<Text>();
                //    RightDirection = transform.Find("Chart/Directions/DirectionFrame/RightDirection/Right/Text").GetComponent<Text>();
                //    DownRightDirection = transform.Find("Chart/Directions/DirectionFrame/DownRightDirection/DownRight/Text").GetComponent<Text>();
                //    DownDirection = transform.Find("Chart/Directions/DirectionFrame/DownDirection/Down/Text").GetComponent<Text>();
                //    DownLeftDirection = transform.Find("Chart/Directions/DirectionFrame/DownLeftDirection/DownLeft/Text").GetComponent<Text>();
                //    LeftDirection = transform.Find("Chart/Directions/DirectionFrame/LeftDirection/Left/Text").GetComponent<Text>();
                //    UponLeftDirection = transform.Find("Chart/Directions/DirectionFrame/UponLeftDirection/UponLeft/Text").GetComponent<Text>();

                //    RadarArea.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionRadarArea.ToString("0.00");
                //    UponDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[0].ToString("0.0") + "%";
                //    UponRightDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[1].ToString("0.0") + "%";
                //    RightDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[2].ToString("0.0") + "%";
                //    DownRightDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[3].ToString("0.0") + "%";
                //    DownDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[4].ToString("0.0") + "%";
                //    DownLeftDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[5].ToString("0.0") + "%";
                //    LeftDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[6].ToString("0.0") + "%";
                //    UponLeftDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.DirectionAreaRate[7].ToString("0.0") + "%";

                //    if (DoctorDataManager.instance.doctor.patient.Evaluations.Count > 1)
                //    {
                //        RadarArea.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionRadarArea.ToString("0.00") + ")";
                //        UponDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[0].ToString("0.0") + "%" + ")";
                //        UponRightDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[1].ToString("0.0") + "%" + ")";
                //        RightDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[2].ToString("0.0") + "%" + ")";
                //        DownRightDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[3].ToString("0.0") + "%" + ")";
                //        DownDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[4].ToString("0.0") + "%" + ")";
                //        DownLeftDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[5].ToString("0.0") + "%" + ")";
                //        LeftDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[6].ToString("0.0") + "%" + ")";
                //        UponLeftDirection.text += "(" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 2].direction.DirectionAreaRate[7].ToString("0.0") + "%" + ")";
                //    }

                //    SurroundingCount = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].angles.Count;

                //    SurroundingChart = transform.Find("Chart/Surroundings/LineChart").gameObject.GetComponent<LineChart>();
                //    if (SurroundingChart == null) SurroundingChart = transform.Find("Chart/Surroundings/LineChart").gameObject.AddComponent<LineChart>();
                //    SurroundingChart.RemoveData();

                //    for (int i = 0; i < SurroundingCount; i++)
                //    {
                //        //print(AngleCount);
                //        //print(DoctorDataManager.instance.patient.TrainingPlays[DoctorDataManager.instance.patient.TrainingPlays.Count - 1].angles[i].TrainingID);
                //        //chart.AddXAxisData("x" + (i + 1));

                //        SurroundingChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                //        SurroundingChart.AddData(0, DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].angles[i].LeftSideAngle);
                //        SurroundingChart.AddData(1, DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].angles[i].RightSideAngle);
                //        SurroundingChart.AddData(2, DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].angles[i].UponSideAngle);
                //        SurroundingChart.AddData(3, DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].angles[i].DownSideAngle);
                //    }
            }

        }

        void Update()
        {

        }
    }
}