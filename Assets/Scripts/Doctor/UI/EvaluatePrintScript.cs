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

        // Chart
        public List<Point> EvaluationPoints;
        public List<Point> LastEvaluationPoints;
        public int SingleEvaluation;

        public SoccerDistance tempSoccerDistance;

        public GameObject ManImage; // 男患者
        public GameObject ManSideImage; // 男患者侧身
        public GameObject WomanImage;   // 女患者
        public GameObject WomanSideImage;   // 女患者侧身

        // 先平移,后放缩
        public Vector2 ModelGravity;   // 模型重心坐标
        public Vector2 SideModelGravity;    // 模型侧身重心坐标
        public Vector2 GravityDiff;    // 重心偏移
        public float HeightPixel;   // 模型身高像素


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
                EvaluationResult.text = "各方位正常";

                List<Tuple<float, string> > HemiPos = new List<Tuple<float, string> >();  // 偏瘫方位

                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.UponSoccerDistance, "正上"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.UponRightSoccerDistance, "右上"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.RightSoccerDistance, "正右"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.DownRightSoccerDistance, "右下"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.DownSoccerDistance, "正下"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.DownLeftSoccerDistance, "左下"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.LeftSoccerDistance, "正左"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.UponLeftSoccerDistance, "左上"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.FrontSoccerDistance, "正前"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.BehindSoccerDistance, "正后"));

                HemiPos.Sort();  //升序

                int HemiPosCount = 0;
                if (HemiPos[0].Item1 < 0.75f)
                {
                    EvaluationResult.text = HemiPos[0].Item2;
                }
                for(int i = 1; i<4 && HemiPos[i].Item1 < 0.75f; i++)
                {
                    HemiPosCount++;

                    EvaluationResult.text += "|" + HemiPos[i].Item2;
                }

                EvaluationResult.transform.localScale = new Vector2(80f + 20 * HemiPosCount, 22.9f);

                EvaluationTime = transform.Find("Evaluation/EvaluationInfo/Time/EvaluationTime").GetComponent<Text>();
                EvaluationTime.text = evaluation.EvaluationStartTime;


                // Chart
                SingleEvaluation = DoctorDataManager.instance.doctor.patient.EvaluationIndex;
                EvaluationPoints = new List<Point>();
                foreach (var point in DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].Points)
                {
                    EvaluationPoints.Add(new Point(point.x, point.y));
                }

                tempSoccerDistance = new SoccerDistance(DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance);

                ManImage = transform.Find("Chart/RadarChart/ManImage").gameObject; // 男患者
                ManSideImage = transform.Find("Chart/RadarChart/ManSideImage").gameObject;  // 男患者侧身
                WomanImage = transform.Find("Chart/RadarChart/WomanImage").gameObject; ;   // 女患者
                WomanSideImage = transform.Find("Chart/RadarChart/WomanSideImage").gameObject; ;   // 女患者侧身



                if (DoctorDataManager.instance.doctor.patient.PatientSex == "男")
                {
                    ManImage.SetActive(true); ManSideImage.SetActive(true);
                    WomanImage.SetActive(false); WomanSideImage.SetActive(false);

                    //WidthPixel = 100;
                    HeightPixel = 32;

                    ModelGravity = new Vector2(476.5f, 672.5f);

                    SideModelGravity = new Vector2(615f, 672.5f);
                }
                else
                {
                    ManImage.SetActive(false); ManSideImage.SetActive(false);
                    WomanImage.SetActive(true); WomanSideImage.SetActive(true);

                    //WidthPixel = 80;
                    HeightPixel = 32;

                    ModelGravity = new Vector2(476.5f, 682.5f);
                    SideModelGravity = new Vector2(615f, 682.5f);
                }

                GravityDiff = new Vector2(ModelGravity.x - EvaluationPoints[0].x, ModelGravity.y - EvaluationPoints[0].y);
                EvaluationPoints[0] = new Point(ModelGravity);

                for (int i = 1; i < EvaluationPoints.Count; i++)
                {
                    EvaluationPoints[i].x += GravityDiff.x;
                    EvaluationPoints[i].y += GravityDiff.y;

                    //tempPoints[i].x = tempPoints[0].x + (tempPoints[i].x - tempPoints[0].x) * WidthPixel / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationWidth;
                    EvaluationPoints[i].x = EvaluationPoints[0].x + (EvaluationPoints[i].x - EvaluationPoints[0].x) * HeightPixel / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationHeight;
                    EvaluationPoints[i].y = EvaluationPoints[0].y + (EvaluationPoints[i].y - EvaluationPoints[0].y) * HeightPixel / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationHeight;
                }


                if (SingleEvaluation > 0)
                {
                    LastEvaluationPoints = new List<Point>();
                    foreach (var point in DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].Points)
                    {
                        LastEvaluationPoints.Add(new Point(point.x, point.y));
                    }

                    GravityDiff = new Vector2(ModelGravity.x - LastEvaluationPoints[0].x, ModelGravity.y - LastEvaluationPoints[0].y);
                    LastEvaluationPoints[0] = new Point(ModelGravity);

                    for (int i = 1; i < LastEvaluationPoints.Count; i++)
                    {
                        LastEvaluationPoints[i].x += GravityDiff.x;
                        LastEvaluationPoints[i].y += GravityDiff.y;

                        //tempPoints[i].x = tempPoints[0].x + (tempPoints[i].x - tempPoints[0].x) * WidthPixel / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationWidth;
                        LastEvaluationPoints[i].x = LastEvaluationPoints[0].x + (LastEvaluationPoints[i].x - LastEvaluationPoints[0].x) * HeightPixel / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].EvaluationHeight;
                        LastEvaluationPoints[i].y = LastEvaluationPoints[0].y + (LastEvaluationPoints[i].y - LastEvaluationPoints[0].y) * HeightPixel / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].EvaluationHeight;
                    }
                }

                // 默认画凸包图

                //ConvexHullLineColor = new Color32(255, 0, 0, 255);
                //ConvexHullAreaColor = new Color32(255, 0, 0, 40);

                //LastConvexHullLineColor = new Color32(0, 255, 0, 255);
                //LastConvexHullAreaColor = new Color32(0, 255, 0, 40);

                //LastNowOverlappingColor = new Color32(255, 255, 0, 15);

                //SideLineColor = new Color32(255, 140, 5, 255);

                //LastConvexHullText.text = "上次评估";
                //NowConvexHullText.text = "本次评估";

                ////// 画侧身直线
                ////DrawSideLine();

                //// 画凸包图
                //ContexHullToggle.isOn = true;
                ////DrawContexHullToggleChange();


                //// SoccerSpeedAndTime

                //SoccerBar = transform.Find("SoccerBar/BarChart").gameObject.GetComponent<BarChart>();
                //if (SoccerBar == null) SoccerBar = transform.Find("SoccerBar/BarChart").gameObject.AddComponent<BarChart>();

                //// 写入数据
                //SoccerBar.series.UpdateData(0, 0, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerDistance);
                //SoccerBar.series.UpdateData(0, 1, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerDistance);
                //SoccerBar.series.UpdateData(0, 2, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerDistance);
                //SoccerBar.series.UpdateData(0, 3, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerDistance);
                //SoccerBar.series.UpdateData(0, 4, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerDistance);
                //SoccerBar.series.UpdateData(0, 5, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerDistance);
                //SoccerBar.series.UpdateData(0, 6, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerDistance);
                //SoccerBar.series.UpdateData(0, 7, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerDistance);
                //SoccerBar.series.UpdateData(0, 8, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance);
                //SoccerBar.series.UpdateData(0, 9, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance);

                //SoccerBar.series.UpdateData(1, 0, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerTime);
                //SoccerBar.series.UpdateData(1, 1, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerTime);
                //SoccerBar.series.UpdateData(1, 2, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerTime);
                //SoccerBar.series.UpdateData(1, 3, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerTime);
                //SoccerBar.series.UpdateData(1, 4, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerTime);
                //SoccerBar.series.UpdateData(1, 5, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerTime);
                //SoccerBar.series.UpdateData(1, 6, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerTime);
                //SoccerBar.series.UpdateData(1, 7, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerTime);
                //SoccerBar.series.UpdateData(1, 8, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerTime);
                //SoccerBar.series.UpdateData(1, 9, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerTime);

                //SoccerBar.RefreshChart();




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