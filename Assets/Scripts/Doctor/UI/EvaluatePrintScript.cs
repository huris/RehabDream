using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Vectrosity;
using System.Collections;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class EvaluatePrintScript : MonoBehaviour
    {

        public int SingleEvaluation;
        public Evaluation evaluation;
        public SoccerDistance NowSoccerDistance;
        public SoccerDistance LastSoccerDistance;

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
        public Text EvaluationResult;
        public Text EvaluationTime;

        // Chart
        public List<Point> EvaluationPoints;
        public List<Point> LastEvaluationPoints;

        public GameObject ManImage; // 男患者
        public GameObject ManSideImage; // 男患者侧身
        public GameObject WomanImage;   // 女患者
        public GameObject WomanSideImage;   // 女患者侧身

        // 先平移,后放缩
        public Vector2 ModelGravity;   // 模型重心坐标
        public Vector2 SideModelGravity;    // 模型侧身重心坐标
        public Vector2 GravityDiff;    // 重心偏移
        public float HeightPixel;   // 模型身高像素

        public Color32 ConvexHullLineColor;
        public Color32 ConvexHullAreaColor;
        public Color32 LastConvexHullLineColor;
        public Color32 LastConvexHullAreaColor;
        public Color32 LastNowOverlappingColor;
        public Color32 SideLineColor;
        public Color[] ConvexHullColors;

        public VectorLine NowFrontLine; // 本次向前倾的线
        public VectorLine NowBehindLine;    //本次向后倾的线
        public VectorLine LastFrontLine; // 本次向前倾的线
        public VectorLine LastBehindLine;    //本次向后倾的线
        public VectorLine SideLine; // 侧身直线
        public static float SideCoefficient = 80f;   // 距离修正系数

        public ConvexHull NowConvexHull;
        public ConvexHull LastConvexHull;
        public VectorLine ConvexHullLine;
        public VectorLine ConvexHullArea;
        public VectorLine LastConvexHullLine;
        public VectorLine LastConvexHullArea;
        public bool IsDrawNowConvexHull;
        public bool IsDrawLastConvexHull;

        public List<Vector2> NowConvexHullPoints;
        public List<Vector2> LastConvexHullPoints;



        void OnEnable()
        {
            if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
            {
                SingleEvaluation = DoctorDataManager.instance.doctor.patient.EvaluationIndex;
                evaluation = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation];

                NowSoccerDistance = evaluation.soccerDistance;

                // Title
                string PatientNameBlock = "";
                for(int z = 0; z < DoctorDataManager.instance.doctor.patient.PatientName.Length; z++)
                {
                    PatientNameBlock += DoctorDataManager.instance.doctor.patient.PatientName[z] + "  ";
                }
                EvaluationTitle = transform.Find("EvaluationTitle").GetComponent<Text>();
                EvaluationTitle.text = PatientNameBlock + "第  " + (SingleEvaluation + 1).ToString() + "  次  评  估  报  告  表";

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
                // 初始化对比结果
                for(int m = 0; m < 13; m++)
                {
                    for(int n = 1; n < 5; n++)
                    {
                        SetResultDataText("-", m, n);
                    }
                }

                EvaluationPoints = new List<Point>();
                foreach (var point in DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].Points)
                {
                    EvaluationPoints.Add(new Point(point.x, point.y));
                }

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
                    LastSoccerDistance = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance;

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

                //默认画凸包图

                ConvexHullLineColor = new Color32(255, 0, 0, 255);
                ConvexHullAreaColor = new Color32(255, 0, 0, 40);

                LastConvexHullLineColor = new Color32(0, 255, 0, 255);
                LastConvexHullAreaColor = new Color32(0, 255, 0, 40);

                LastNowOverlappingColor = new Color32(255, 255, 0, 15);

                SideLineColor = new Color32(255, 140, 5, 255);

                // 画侧身直线
                DrawSideLine();

                // 画凸包图
                DrawContexHull();


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

        public void DrawContexHull()
        {
            // 先画这次的凸包图

            List<Point> tempPoints = new List<Point>();

            foreach (var point in EvaluationPoints)
            {
                tempPoints.Add(new Point(point.x, point.y));
            }

            NowConvexHull = new ConvexHull(tempPoints);

            // 画凸包圈
            ConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 5.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            ConvexHullLine.smoothColor = false;   // 设置平滑颜色
            ConvexHullLine.smoothWidth = false;   // 设置平滑宽度
            //ConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed
            ConvexHullLine.color = ConvexHullLineColor;  // 设置颜色

            int MinX = Mathf.FloorToInt(NowConvexHull.ConvexHullSet[0].x), MaxX = Mathf.CeilToInt(NowConvexHull.ConvexHullSet[0].x);   // 凸包的最大最小X
            int MinY = Mathf.FloorToInt(NowConvexHull.ConvexHullSet[0].y), MaxY = Mathf.CeilToInt(NowConvexHull.ConvexHullSet[0].y);   // 凸包的最大最小Y

            // 先把初始点存入画图函数
            ConvexHullLine.points2.Add(new Vector2(NowConvexHull.ConvexHullSet[0].x, NowConvexHull.ConvexHullSet[0].y));
            NowConvexHull.ConvexHullArea = 0f;   // 令凸包面积初始为0

            for (int i = 1; i < NowConvexHull.ConvexHullNum; i++)
            {
                ConvexHullLine.points2.Add(new Vector2(NowConvexHull.ConvexHullSet[i].x, NowConvexHull.ConvexHullSet[i].y));
                //ConvexHullLine.SetColor(ConvexHullLineColor);  // 设置颜色

                if (i < NowConvexHull.ConvexHullNum - 1)
                {
                    NowConvexHull.ConvexHullArea += Math.Abs(ConvexHull.isLeft(NowConvexHull.ConvexHullSet[0], NowConvexHull.ConvexHullSet[i], NowConvexHull.ConvexHullSet[i + 1]));
                }

                if (MinX > Mathf.FloorToInt(NowConvexHull.ConvexHullSet[i].x)) MinX = Mathf.FloorToInt(NowConvexHull.ConvexHullSet[i].x);
                if (MaxX < Mathf.CeilToInt(NowConvexHull.ConvexHullSet[i].x)) MaxX = Mathf.CeilToInt(NowConvexHull.ConvexHullSet[i].x);
                if (MinY > Mathf.FloorToInt(NowConvexHull.ConvexHullSet[i].y)) MinY = Mathf.FloorToInt(NowConvexHull.ConvexHullSet[i].y);
                if (MaxY < Mathf.CeilToInt(NowConvexHull.ConvexHullSet[i].y)) MaxY = Mathf.CeilToInt(NowConvexHull.ConvexHullSet[i].y);


                //ConvexHullLine.Draw();
                //yield return new WaitForSeconds(0.15f);
            }

            //button.transform.GetChild(0).GetComponent<Text>().text = (ConvexHullArea / 2).ToString("0.00");// 最后求出来的面积要除以2

            ConvexHullLine.points2.Add(new Vector2(NowConvexHull.ConvexHullSet[0].x, NowConvexHull.ConvexHullSet[0].y));
            //ConvexHullLine.SetColor(Color.blue);  // 设置颜色

            //ColorFistLine.Draw();
            ConvexHullLine.Draw();

            SetResultDataText(NowConvexHull.ConvexHullArea.ToString("0.0000"), 0, 2);

            IsDrawNowConvexHull = false;
            // 多算几个单位
            StartCoroutine(DrawConvexHullArea(MinX - 10, MaxX + 10, MinY - 10, MaxY + 10));
        }

        IEnumerator DrawConvexHullArea(int MinX, int MaxX, int MinY, int MaxY)
        {
            yield return new WaitForEndOfFrame();

            if (!IsDrawNowConvexHull)
            {
                ConvexHullArea = new VectorLine("ConvexHullLine", new List<Vector2>(), 1f, Vectrosity.LineType.Continuous, Joins.Weld);
                ConvexHullArea.smoothColor = false;   // 设置平滑颜色
                ConvexHullArea.smoothWidth = false;   // 设置平滑宽度
                ConvexHullArea.color = ConvexHullAreaColor;

                Texture2D m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
                m_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);

                ConvexHullColors = m_texture.GetPixels(MinX, MinY, MaxX - MinX, MaxY - MinY);

                MaxY = MaxY - MinY - 1;

                int x, y;

                NowConvexHullPoints = new List<Vector2>();

                for (int i = 0; i < MaxY; i++)
                {
                    x = i * (MaxX - MinX); y = (i + 1) * (MaxX - MinX);

                    while ((x < y) && (ConvexHullColors[x] != ConvexHullLineColor)) x++;    // 查找左边的凸包边界
                    while ((x < y) && (ConvexHullColors[y] != ConvexHullLineColor)) y--;    // 查找右边的凸包边界

                    if (x != y)
                    {
                        ConvexHullArea.points2.Add(new Vector2(MinX + x - i * (MaxX - MinX), MinY + i));
                        ConvexHullArea.points2.Add(new Vector2(MinX + y - i * (MaxX - MinX), MinY + i));

                        NowConvexHullPoints.Add(new Vector2(MinX + x - i * (MaxX - MinX), MinY + i));
                        NowConvexHullPoints.Add(new Vector2(MinX + y - i * (MaxX - MinX), MinY + i));
                    }
                }
                ConvexHullArea.Draw();

                IsDrawNowConvexHull = true;

                if(SingleEvaluation > 0)
                {
                    List<Point> tempPoints = new List<Point>();

                    foreach (var point in LastEvaluationPoints)
                    {
                        tempPoints.Add(new Point(point.x, point.y));
                    }

                    LastConvexHull = new ConvexHull(tempPoints);

                    // 画凸包圈
                    LastConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 5.0f, Vectrosity.LineType.Continuous, Joins.Weld);
                    LastConvexHullLine.smoothColor = false;   // 设置平滑颜色
                    LastConvexHullLine.smoothWidth = false;   // 设置平滑宽度
                                                              //ConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed
                    LastConvexHullLine.color = LastConvexHullLineColor;  // 设置颜色

                    MinX = Mathf.FloorToInt(LastConvexHull.ConvexHullSet[0].x); MaxX = Mathf.CeilToInt(LastConvexHull.ConvexHullSet[0].x);   // 凸包的最大最小X
                    MinY = Mathf.FloorToInt(LastConvexHull.ConvexHullSet[0].y); MaxY = Mathf.CeilToInt(LastConvexHull.ConvexHullSet[0].y);   // 凸包的最大最小Y

                    // 先把初始点存入画图函数
                    LastConvexHullLine.points2.Add(new Vector2(LastConvexHull.ConvexHullSet[0].x, LastConvexHull.ConvexHullSet[0].y));
                    LastConvexHull.ConvexHullArea = 0f;   // 令凸包面积初始为0

                    for (int i = 1; i < LastConvexHull.ConvexHullNum; i++)
                    {
                        LastConvexHullLine.points2.Add(new Vector2(LastConvexHull.ConvexHullSet[i].x, LastConvexHull.ConvexHullSet[i].y));
                        //ConvexHullLine.SetColor(ConvexHullLineColor);  // 设置颜色

                        if (i < LastConvexHull.ConvexHullNum - 1)
                        {
                            LastConvexHull.ConvexHullArea += Math.Abs(ConvexHull.isLeft(LastConvexHull.ConvexHullSet[0], LastConvexHull.ConvexHullSet[i], LastConvexHull.ConvexHullSet[i + 1]));
                        }

                        if (MinX > Mathf.FloorToInt(LastConvexHull.ConvexHullSet[i].x)) MinX = Mathf.FloorToInt(LastConvexHull.ConvexHullSet[i].x);
                        if (MaxX < Mathf.CeilToInt(LastConvexHull.ConvexHullSet[i].x)) MaxX = Mathf.CeilToInt(LastConvexHull.ConvexHullSet[i].x);
                        if (MinY > Mathf.FloorToInt(LastConvexHull.ConvexHullSet[i].y)) MinY = Mathf.FloorToInt(LastConvexHull.ConvexHullSet[i].y);
                        if (MaxY < Mathf.CeilToInt(LastConvexHull.ConvexHullSet[i].y)) MaxY = Mathf.CeilToInt(LastConvexHull.ConvexHullSet[i].y);


                        //ConvexHullLine.Draw();
                        //yield return new WaitForSeconds(0.15f);
                    }

                    //button.transform.GetChild(0).GetComponent<Text>().text = (ConvexHullArea / 2).ToString("0.00");// 最后求出来的面积要除以2

                    LastConvexHullLine.points2.Add(new Vector2(LastConvexHull.ConvexHullSet[0].x, LastConvexHull.ConvexHullSet[0].y));
                    //ConvexHullLine.SetColor(Color.blue);  // 设置颜色

                    //ColorFistLine.Draw();
                    LastConvexHullLine.Draw();

                    SetResultDataText(LastConvexHull.ConvexHullArea.ToString("0.0000"), 0, 1);

                    SetResultDataText(GetEvaluationResult(NowSoccerDistance.BehindSoccerDistance, LastSoccerDistance.BehindSoccerDistance, 4), 2, 3);


                    IsDrawLastConvexHull = false;
                    // 多算几个单位
                    StartCoroutine(DrawLastConvexHullArea(MinX - 10, MaxX + 10, MinY - 10, MaxY + 10));
                }
            }      
        }

        IEnumerator DrawLastConvexHullArea(int MinX, int MaxX, int MinY, int MaxY)
        {
            yield return new WaitForEndOfFrame();

            if (!IsDrawLastConvexHull)
            {

                LastConvexHullArea = new VectorLine("ConvexHullLine", new List<Vector2>(), 1f, Vectrosity.LineType.Continuous, Joins.Weld);
                LastConvexHullArea.smoothColor = false;   // 设置平滑颜色
                LastConvexHullArea.smoothWidth = false;   // 设置平滑宽度
                LastConvexHullArea.color = LastConvexHullAreaColor;

                Texture2D m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
                m_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);

                ConvexHullColors = m_texture.GetPixels(MinX, MinY, MaxX - MinX, MaxY - MinY);

                MaxY = MaxY - MinY - 1;

                int x, y;

                LastConvexHullPoints = new List<Vector2>();

                for (int i = 0; i < MaxY; i++)
                {
                    x = i * (MaxX - MinX); y = (i + 1) * (MaxX - MinX);

                    while ((x < y) && (ConvexHullColors[x] != LastConvexHullLineColor)) x++;    // 查找左边的凸包边界
                    while ((x < y) && (ConvexHullColors[y] != LastConvexHullLineColor)) y--;    // 查找右边的凸包边界

                    if (x != y)
                    {
                        LastConvexHullArea.points2.Add(new Vector2(MinX + x - i * (MaxX - MinX), MinY + i));
                        LastConvexHullArea.points2.Add(new Vector2(MinX + y - i * (MaxX - MinX), MinY + i));

                        LastConvexHullPoints.Add(new Vector2(MinX + x - i * (MaxX - MinX), MinY + i));
                        LastConvexHullPoints.Add(new Vector2(MinX + y - i * (MaxX - MinX), MinY + i));
                    }
                }
                LastConvexHullArea.Draw();

                IsDrawLastConvexHull = true;







                if (NowConvexHullToggle.isOn)
                {

                    LastNowConvexHullArea = new VectorLine("ConvexHullLine", new List<Vector2>(), 5.0f, Vectrosity.LineType.Continuous, Joins.Weld);
                    LastNowConvexHullArea.smoothColor = false;   // 设置平滑颜色
                    LastNowConvexHullArea.smoothWidth = false;   // 设置平滑宽度
                                                                 //ConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed
                    LastNowConvexHullArea.color = LastNowOverlappingColor;  // 设置颜色

                    int LastInx = 0, NowInx = 0;
                    while ((LastInx < LastConvexHullPoints.Count) && (LastConvexHullPoints[LastInx].y < NowConvexHullPoints[0].y)) LastInx++;
                    while ((NowInx < NowConvexHullPoints.Count) && (NowConvexHullPoints[NowInx].y < LastConvexHullPoints[0].y)) NowInx++;
                    float x1, x2, x3, x4;
                    while ((LastInx < LastConvexHullPoints.Count) && (NowInx < NowConvexHullPoints.Count))
                    {
                        x1 = LastConvexHullPoints[LastInx++].x;
                        x2 = LastConvexHullPoints[LastInx++].x;
                        x3 = NowConvexHullPoints[NowInx++].x;
                        x4 = NowConvexHullPoints[NowInx++].x;

                        if (x1 <= x3)
                        {
                            if (x2 <= x4)
                            {
                                LastNowConvexHullArea.points2.Add(new Vector2(x2, NowConvexHullPoints[NowInx - 1].y));
                                LastNowConvexHullArea.points2.Add(new Vector2(x3, NowConvexHullPoints[NowInx - 1].y));
                            }
                            else
                            {
                                LastNowConvexHullArea.points2.Add(new Vector2(x3, NowConvexHullPoints[NowInx - 1].y));
                                LastNowConvexHullArea.points2.Add(new Vector2(x4, NowConvexHullPoints[NowInx - 1].y));
                            }
                        }
                        else
                        {
                            if (x2 <= x4)
                            {
                                LastNowConvexHullArea.points2.Add(new Vector2(x1, NowConvexHullPoints[NowInx - 1].y));
                                LastNowConvexHullArea.points2.Add(new Vector2(x2, NowConvexHullPoints[NowInx - 1].y));
                            }
                            else
                            {
                                LastNowConvexHullArea.points2.Add(new Vector2(x1, NowConvexHullPoints[NowInx - 1].y));
                                LastNowConvexHullArea.points2.Add(new Vector2(x4, NowConvexHullPoints[NowInx - 1].y));
                            }
                        }
                    }

                    LastNowConvexHullArea.Draw();

                    NowConvexHullText.text = "本次评估:雷达图(" + (NowConvexHull.ConvexHullArea / SideCoefficient / SideCoefficient).ToString("0.00");

                    float RadarAreaIncreaseRate = (NowConvexHull.ConvexHullArea - LastConvexHull.ConvexHullArea) / LastConvexHull.ConvexHullArea;
                    if (RadarAreaIncreaseRate < 0) NowConvexHullText.text += " <color=red>-" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (RadarAreaIncreaseRate == 0) NowConvexHullText.text += " <color=blue>=" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += " <color=green>+" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";

                    NowConvexHullText.text += ",前倾(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00");
                    float FrontIncreaseRate = (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient
                                                - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient)
                                                / (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient);
                    if (FrontIncreaseRate < 0) NowConvexHullText.text += " <color=red>-" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (FrontIncreaseRate == 0) NowConvexHullText.text += " <color=blue>=" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += " <color=green>+" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";

                    NowConvexHullText.text += ",后仰(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00");
                    float BehindIncreaseRate = (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient
                                                - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient)
                                                / (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient);
                    if (BehindIncreaseRate < 0) NowConvexHullText.text += " <color=red>-" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (BehindIncreaseRate == 0) NowConvexHullText.text += " <color=blue>=" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += " <color=green>+" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";
                }
            }
        }












        public void DrawSideLine()
        {
            float HalfVerticalOffset = 30f;

            SideLine = new VectorLine("SideLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            SideLine.smoothColor = false;   // 设置平滑颜色
            SideLine.smoothWidth = false;   // 设置平滑宽度
            SideLine.color = SideLineColor;  // 设置颜色

            NowFrontLine = new VectorLine("NowFrontLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            NowFrontLine.smoothColor = false;   // 设置平滑颜色
            NowFrontLine.smoothWidth = false;   // 设置平滑宽度
            NowFrontLine.color = ConvexHullLineColor;  // 设置颜色
            NowFrontLine.points2.Add(new Vector2(SideModelGravity.x + NowSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y + HalfVerticalOffset));
            NowFrontLine.points2.Add(new Vector2(SideModelGravity.x + NowSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y - HalfVerticalOffset));
            NowFrontLine.Draw();
            SetResultDataText(NowSoccerDistance.FrontSoccerDistance.ToString("0.0000"), 1, 2);

            NowBehindLine = new VectorLine("NowBehindLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            NowBehindLine.smoothColor = false;   // 设置平滑颜色
            NowBehindLine.smoothWidth = false;   // 设置平滑宽度
            NowBehindLine.color = ConvexHullLineColor;  // 设置颜色
            NowBehindLine.points2.Add(new Vector2(SideModelGravity.x - NowSoccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y + HalfVerticalOffset));
            NowBehindLine.points2.Add(new Vector2(SideModelGravity.x - NowSoccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y - HalfVerticalOffset));
            NowBehindLine.Draw();
            SetResultDataText(NowSoccerDistance.BehindSoccerDistance.ToString("0.0000"), 2, 2);

            float FrontX = SideModelGravity.x + NowSoccerDistance.FrontSoccerDistance * SideCoefficient;
            float BehindX = SideModelGravity.x - NowSoccerDistance.BehindSoccerDistance * SideCoefficient;

            if(SingleEvaluation > 0)
            {
                LastFrontLine = new VectorLine("LastFrontLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
                LastFrontLine.smoothColor = false;   // 设置平滑颜色
                LastFrontLine.smoothWidth = false;   // 设置平滑宽度
                LastFrontLine.color = LastConvexHullLineColor;  // 设置颜色
                LastFrontLine.points2.Add(new Vector2(SideModelGravity.x + LastSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y + HalfVerticalOffset));
                LastFrontLine.points2.Add(new Vector2(SideModelGravity.x + LastSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y - HalfVerticalOffset));
                LastFrontLine.Draw();
                SetResultDataText(LastSoccerDistance.FrontSoccerDistance.ToString("0.0000"), 1, 1);

                LastBehindLine = new VectorLine("LastBehindLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
                LastBehindLine.smoothColor = false;   // 设置平滑颜色
                LastBehindLine.smoothWidth = false;   // 设置平滑宽度
                LastBehindLine.color = LastConvexHullLineColor;  // 设置颜色
                LastBehindLine.points2.Add(new Vector2(SideModelGravity.x - LastSoccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y + HalfVerticalOffset));
                LastBehindLine.points2.Add(new Vector2(SideModelGravity.x - LastSoccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y - HalfVerticalOffset));
                LastBehindLine.Draw();
                SetResultDataText(LastSoccerDistance.BehindSoccerDistance.ToString("0.0000"), 2, 1);

                FrontX = Mathf.Max(FrontX, SideModelGravity.x + LastSoccerDistance.FrontSoccerDistance * SideCoefficient);
                BehindX = Mathf.Min(BehindX, SideModelGravity.x - LastSoccerDistance.BehindSoccerDistance * SideCoefficient);

                SetResultDataText(GetEvaluationResult(LastSoccerDistance.FrontSoccerDistance, NowSoccerDistance.FrontSoccerDistance, 4), 1, 3);
                SetResultDataText(GetEvaluationResult(LastSoccerDistance.FrontSoccerDistance, NowSoccerDistance.FrontSoccerDistance, 2), 1, 4);
                
                SetResultDataText(GetEvaluationResult(LastSoccerDistance.BehindSoccerDistance, NowSoccerDistance.BehindSoccerDistance, 4), 2, 3);
                SetResultDataText(GetEvaluationResult(LastSoccerDistance.BehindSoccerDistance, NowSoccerDistance.BehindSoccerDistance, 2), 2, 4);
            }

            SideLine.points2.Add(new Vector2(FrontX, SideModelGravity.y));
            SideLine.points2.Add(new Vector2(BehindX, SideModelGravity.y));
            SideLine.Draw();
        }




        // 修改结果对比的数据
        public void SetResultDataText(string DataText, int i, int j)
        {
            this.transform.GetChild(4).GetChild(3).GetChild(2).GetChild(i).GetChild(j).GetComponent<Text>().text = DataText;
        }
        public string GetEvaluationResult(float Last, float Now, int bits)
        {
            return ChangeColorString(Last, Now, bits);
        }
        public string GetEvaluationResult(Vector2 Last, Vector2 Now, int bits)
        {
            return ChangeColorString(Last.x, Now.x, bits) + " | " + ChangeColorString(Last.y, Now.y, bits);
        }

        public string ChangeColorString(float Last, float Now, int bits)
        {
            float Diff = Now - Last;

            if(Mathf.Abs(Diff) < 1e-5)
            {
                return "<color=blue>" + "基本无变化" + "</color>";
            }
            else if (Diff < 0)
            {
                return "<color=red>- " + GetColorOriginString(Last, -Diff, bits) + "</color>";
            }
            else if(Diff > 0)
            {
                return "<color=green>+ " + GetColorOriginString(Last, Diff, bits)+ "</color>";
            }
            else
            {
                return "-";
            }
        }

        public string GetColorOriginString(float Last, float Diff, int bits)
        {
            string ResultString = "";

            if(bits == 2)
            {
                ResultString = (Diff / Last * 100).ToString("0.00") + "%";    
            }
            else if(bits == 4)
            {
                ResultString = (Diff).ToString("0.0000");
            }
            return ResultString;
        }

        void Update()
        {

        }
    }
}