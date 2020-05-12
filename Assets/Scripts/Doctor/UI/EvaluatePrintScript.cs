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
        public static float SideCoefficient = 70f;   // 距离修正系数

        public ConvexHull NowConvexHull;
        public ConvexHull LastConvexHull;
        public VectorLine ConvexHullLine;
        public VectorLine ConvexHullArea;
        public VectorLine LastConvexHullLine;
        public VectorLine LastConvexHullArea;
        public VectorLine LastNowConvexHullArea;
        public bool IsDrawNowConvexHull;
        public bool IsDrawLastConvexHull;

        public List<Vector2> NowConvexHullPoints;
        public List<Vector2> LastConvexHullPoints;

        public BarChart SoccerBar;


        void OnEnable()
        {
            if (DoctorDataManager.instance.doctor.patient.Evaluations == null)
            {
                DoctorDataManager.instance.doctor.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

                if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
                {
                    DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);
                }
            }

            if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
            {
                SingleEvaluation = DoctorDataManager.instance.doctor.patient.EvaluationIndex;
                evaluation = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation];

                NowSoccerDistance = evaluation.soccerDistance;

                // Title
                //string PatientNameBlock = "";
                //for (int z = 0; z < DoctorDataManager.instance.doctor.patient.PatientName.Length; z++)
                //{
                //    PatientNameBlock += DoctorDataManager.instance.doctor.patient.PatientName[z] + "  ";
                //}

                List<string> sequence = new List<string>();
                sequence.Add("st");
                sequence.Add("nd");
                sequence.Add("rd");
                sequence.Add("th");

                EvaluationTitle = transform.Find("EvaluationTitle").GetComponent<Text>();
                EvaluationTitle.text = DoctorDataManager.instance.doctor.patient.PatientName + "\'s " + (SingleEvaluation + 1).ToString() + sequence[SingleEvaluation] + " Evaluation Report";

                // Information
                InformationPatientID = transform.Find("Information/PatientInfo/ID/PatientID").GetComponent<Text>();
                InformationPatientID.text = DoctorDataManager.instance.doctor.patient.PatientID.ToString();

                InformationPatientName = transform.Find("Information/PatientInfo/Name/PatientName").GetComponent<Text>();
                InformationPatientName.text = DoctorDataManager.instance.doctor.patient.PatientName;

                InformationPatientSex = transform.Find("Information/PatientInfo/Sex/PatientSex").GetComponent<Text>();
                InformationPatientSex.text = DoctorDataManager.instance.doctor.patient.PatientSex;

                InformationPatientAge = transform.Find("Information/PatientInfo/Age/PatientAge").GetComponent<Text>();
                InformationPatientAge.text = DoctorDataManager.instance.doctor.patient.PatientAge.ToString();

                InformationPatientHeight = transform.Find("Information/PatientInfo/Height/PatientHeight").GetComponent<Text>();
                if (DoctorDataManager.instance.doctor.patient.PatientHeight == -1)
                {
                    InformationPatientHeight.text = "NULL";
                }
                else
                {
                    InformationPatientHeight.text = DoctorDataManager.instance.doctor.patient.PatientHeight.ToString() + " CM";
                }

                InformationPatientWeight = transform.Find("Information/PatientInfo/Weight/PatientWeight").GetComponent<Text>();
                if (DoctorDataManager.instance.doctor.patient.PatientWeight == -1)
                {
                    InformationPatientWeight.text = "NULL";
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
                EvaluationScore.text = evaluation.EvaluationScore.ToString("0.00");

                EvaluationDuration = transform.Find("Evaluation/EvaluationInfo/Duration/EvaluationDuration").GetComponent<Text>();
                EvaluationDuration.text = (long.Parse(evaluation.EvaluationEndTime.Substring(9, 2)) * 3600 + long.Parse(evaluation.EvaluationEndTime.Substring(12, 2)) * 60 + long.Parse(evaluation.EvaluationEndTime.Substring(15, 2))
                                           - long.Parse(evaluation.EvaluationStartTime.Substring(9, 2)) * 3600 - long.Parse(evaluation.EvaluationStartTime.Substring(12, 2)) * 60 - long.Parse(evaluation.EvaluationStartTime.Substring(15, 2))).ToString() + " 秒";

                EvaluationRank = transform.Find("Evaluation/EvaluationInfo/Rank/EvaluationRank").GetComponent<Text>();
                float TrainingEvaluationRate = evaluation.EvaluationScore;
                if (TrainingEvaluationRate >= 80f) { EvaluationRank.text = "Level 1"; }
                else if (TrainingEvaluationRate >= 70f) { EvaluationRank.text = "Level 2"; }
                else if (TrainingEvaluationRate >= 60f) { EvaluationRank.text = "Level 3"; }
                else if (TrainingEvaluationRate >= 50f) { EvaluationRank.text = "Level 4"; }
                else { EvaluationRank.text = "Level 5"; }

                EvaluationResult = transform.Find("Evaluation/EvaluationInfo/Result/EvaluationResult").GetComponent<Text>();
                EvaluationResult.text = "Normal";

                List<Tuple<float, string>> HemiPos = new List<Tuple<float, string>>();  // 偏瘫方位

                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.UponSoccerDistance, "U"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.UponRightSoccerDistance, "UR"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.RightSoccerDistance, "R"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.DownRightSoccerDistance, "DR"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.DownSoccerDistance, "D"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.DownLeftSoccerDistance, "DL"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.LeftSoccerDistance, "L"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.UponLeftSoccerDistance, "UL"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.FrontSoccerDistance, "F"));
                HemiPos.Add(new Tuple<float, string>(evaluation.soccerDistance.BehindSoccerDistance, "B"));

                HemiPos.Sort();  //升序

                int HemiPosCount = 0;
                if (HemiPos[0].Item1 < 0.75f)
                {
                    EvaluationResult.text = HemiPos[0].Item2;
                }
                for (int i = 1; i < 4 && HemiPos[i].Item1 < 0.75f; i++)
                {
                    HemiPosCount++;

                    EvaluationResult.text += "|" + HemiPos[i].Item2;
                }

                //print(HemiPosCount);
                //EvaluationResult.transform.parent.localScale = new Vector2(80f + 20 * HemiPosCount, 22.9f);

                EvaluationTime = transform.Find("Evaluation/EvaluationInfo/Time/EvaluationTime").GetComponent<Text>();
                EvaluationTime.text = evaluation.EvaluationStartTime;


                // Chart
                // 初始化对比结果
                for (int m = 0; m < 13; m++)
                {
                    for (int n = 1; n < 5; n++)
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


                if (DoctorDataManager.instance.doctor.patient.PatientSex == "Male")
                {
                    ManImage.SetActive(true); ManSideImage.SetActive(true);
                    WomanImage.SetActive(false); WomanSideImage.SetActive(false);

                    //WidthPixel = 100;
                    HeightPixel = 58;

                    ModelGravity = new Vector2(476.5f, 672.5f);

                    SideModelGravity = new Vector2(595f, 672.5f);
                }
                else
                {
                    ManImage.SetActive(false); ManSideImage.SetActive(false);
                    WomanImage.SetActive(true); WomanSideImage.SetActive(true);

                    //WidthPixel = 80;
                    HeightPixel = 58;

                    ModelGravity = new Vector2(476.5f, 682.5f);
                    SideModelGravity = new Vector2(595f, 682.5f);
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

                // 写入足球得分和位移数据
                DrawSoccerData();

            }

        }


        public void DrawSoccerData()
        {
            SoccerBar = transform.Find("Chart/BarChart").gameObject.GetComponent<BarChart>();
            if (SoccerBar == null) SoccerBar = transform.Find("Chart/BarChart").gameObject.AddComponent<BarChart>();

            // 写入数据
            SoccerBar.series.UpdateData(0, 0, NowSoccerDistance.UponSoccerDistance);
            SoccerBar.series.UpdateData(0, 1, NowSoccerDistance.UponRightSoccerDistance);
            SoccerBar.series.UpdateData(0, 2, NowSoccerDistance.RightSoccerDistance);
            SoccerBar.series.UpdateData(0, 3, NowSoccerDistance.DownRightSoccerDistance);
            SoccerBar.series.UpdateData(0, 4, NowSoccerDistance.DownSoccerDistance);
            SoccerBar.series.UpdateData(0, 5, NowSoccerDistance.DownLeftSoccerDistance);
            SoccerBar.series.UpdateData(0, 6, NowSoccerDistance.LeftSoccerDistance);
            SoccerBar.series.UpdateData(0, 7, NowSoccerDistance.UponLeftSoccerDistance);
            SoccerBar.series.UpdateData(0, 8, NowSoccerDistance.FrontSoccerDistance);
            SoccerBar.series.UpdateData(0, 9, NowSoccerDistance.BehindSoccerDistance);

            SoccerBar.series.UpdateData(1, 0, 1.0f * NowSoccerDistance.UponSoccerScore / NowSoccerDistance.UponSoccerTime);
            SoccerBar.series.UpdateData(1, 1, 1.0f * NowSoccerDistance.UponRightSoccerScore / NowSoccerDistance.UponRightSoccerTime);
            SoccerBar.series.UpdateData(1, 2, 1.0f * NowSoccerDistance.RightSoccerScore / NowSoccerDistance.RightSoccerTime);
            SoccerBar.series.UpdateData(1, 3, 1.0f * NowSoccerDistance.DownRightSoccerScore / NowSoccerDistance.DownRightSoccerTime);
            SoccerBar.series.UpdateData(1, 4, 1.0f * NowSoccerDistance.DownSoccerScore / NowSoccerDistance.DownSoccerTime);
            SoccerBar.series.UpdateData(1, 5, 1.0f * NowSoccerDistance.DownLeftSoccerScore / NowSoccerDistance.DownLeftSoccerTime);
            SoccerBar.series.UpdateData(1, 6, 1.0f * NowSoccerDistance.LeftSoccerScore / NowSoccerDistance.LeftSoccerTime);
            SoccerBar.series.UpdateData(1, 7, 1.0f * NowSoccerDistance.UponLeftSoccerScore / NowSoccerDistance.UponLeftSoccerTime);
            SoccerBar.series.UpdateData(1, 8, 1.0f * NowSoccerDistance.FrontSoccerScore / NowSoccerDistance.FrontSoccerTime);
            SoccerBar.series.UpdateData(1, 9, 1.0f * NowSoccerDistance.BehindSoccerScore / NowSoccerDistance.BehindSoccerTime);

            SoccerBar.RefreshChart();

            SetResultDataText(NowSoccerDistance.UponSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.UponSoccerScore / NowSoccerDistance.UponSoccerTime).ToString("0.0000"), 3, 2);
            SetResultDataText(NowSoccerDistance.UponRightSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.UponRightSoccerScore / NowSoccerDistance.UponRightSoccerTime).ToString("0.0000"), 4, 2);
            SetResultDataText(NowSoccerDistance.RightSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.RightSoccerScore / NowSoccerDistance.RightSoccerTime).ToString("0.0000"), 5, 2);
            SetResultDataText(NowSoccerDistance.DownRightSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.DownRightSoccerScore / NowSoccerDistance.DownRightSoccerTime).ToString("0.0000"), 6, 2);
            SetResultDataText(NowSoccerDistance.DownSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.DownSoccerScore / NowSoccerDistance.DownSoccerTime).ToString("0.0000"), 7, 2);
            SetResultDataText(NowSoccerDistance.DownLeftSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.DownLeftSoccerScore / NowSoccerDistance.DownLeftSoccerTime).ToString("0.0000"), 8, 2);
            SetResultDataText(NowSoccerDistance.LeftSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.LeftSoccerScore / NowSoccerDistance.LeftSoccerTime).ToString("0.0000"), 9, 2);
            SetResultDataText(NowSoccerDistance.UponLeftSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.UponLeftSoccerScore / NowSoccerDistance.UponLeftSoccerTime).ToString("0.0000"), 10, 2);
            SetResultDataText(NowSoccerDistance.FrontSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.FrontSoccerScore / NowSoccerDistance.FrontSoccerTime).ToString("0.0000"), 11, 2);
            SetResultDataText(NowSoccerDistance.BehindSoccerDistance.ToString("0.0000") + " | " + (1.0f * NowSoccerDistance.BehindSoccerScore / NowSoccerDistance.BehindSoccerTime).ToString("0.0000"), 12, 2);

            if (SingleEvaluation > 0)
            {
                SetResultDataText(LastSoccerDistance.UponSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.UponSoccerScore / LastSoccerDistance.UponSoccerTime).ToString("0.0000"), 3, 1);
                SetResultDataText(LastSoccerDistance.UponRightSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.UponRightSoccerScore / LastSoccerDistance.UponRightSoccerTime).ToString("0.0000"), 4, 1);
                SetResultDataText(LastSoccerDistance.RightSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.RightSoccerScore / LastSoccerDistance.RightSoccerTime).ToString("0.0000"), 5, 1);
                SetResultDataText(LastSoccerDistance.DownRightSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.DownRightSoccerScore / LastSoccerDistance.DownRightSoccerTime).ToString("0.0000"), 6, 1);
                SetResultDataText(LastSoccerDistance.DownSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.DownSoccerScore / LastSoccerDistance.DownSoccerTime).ToString("0.0000"), 7, 1);
                SetResultDataText(LastSoccerDistance.DownLeftSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.DownLeftSoccerScore / LastSoccerDistance.DownLeftSoccerTime).ToString("0.0000"), 8, 1);
                SetResultDataText(LastSoccerDistance.LeftSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.LeftSoccerScore / LastSoccerDistance.LeftSoccerTime).ToString("0.0000"), 9, 1);
                SetResultDataText(LastSoccerDistance.UponLeftSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.UponLeftSoccerScore / LastSoccerDistance.UponLeftSoccerTime).ToString("0.0000"), 10, 1);
                SetResultDataText(LastSoccerDistance.FrontSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.FrontSoccerScore / LastSoccerDistance.FrontSoccerTime).ToString("0.0000"), 11, 1);
                SetResultDataText(LastSoccerDistance.BehindSoccerDistance.ToString("0.0000") + " | " + (1.0f * LastSoccerDistance.BehindSoccerScore / LastSoccerDistance.BehindSoccerTime).ToString("0.0000"), 12, 1);

                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.UponSoccerDistance, 1.0f * LastSoccerDistance.UponSoccerScore / LastSoccerDistance.UponSoccerTime),
                               new Vector2(NowSoccerDistance.UponSoccerDistance, 1.0f * NowSoccerDistance.UponSoccerScore / NowSoccerDistance.UponSoccerTime), 4), 3, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.UponRightSoccerDistance, 1.0f * LastSoccerDistance.UponRightSoccerScore / LastSoccerDistance.UponRightSoccerTime),
                                new Vector2(NowSoccerDistance.UponRightSoccerDistance, 1.0f * NowSoccerDistance.UponRightSoccerScore / NowSoccerDistance.UponRightSoccerTime), 4), 4, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.RightSoccerDistance, 1.0f * LastSoccerDistance.RightSoccerScore / LastSoccerDistance.RightSoccerTime),
                                new Vector2(NowSoccerDistance.RightSoccerDistance, 1.0f * NowSoccerDistance.RightSoccerScore / NowSoccerDistance.RightSoccerTime), 4), 5, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.DownRightSoccerDistance, 1.0f * LastSoccerDistance.DownRightSoccerScore / LastSoccerDistance.DownRightSoccerTime),
                                new Vector2(NowSoccerDistance.DownRightSoccerDistance, 1.0f * NowSoccerDistance.DownRightSoccerScore / NowSoccerDistance.DownRightSoccerTime), 4), 6, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.DownSoccerDistance, 1.0f * LastSoccerDistance.DownSoccerScore / LastSoccerDistance.DownSoccerTime),
                                new Vector2(NowSoccerDistance.DownSoccerDistance, 1.0f * NowSoccerDistance.DownSoccerScore / NowSoccerDistance.DownSoccerTime), 4), 7, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.DownLeftSoccerDistance, 1.0f * LastSoccerDistance.DownLeftSoccerScore / LastSoccerDistance.DownLeftSoccerTime),
                                new Vector2(NowSoccerDistance.DownLeftSoccerDistance, 1.0f * NowSoccerDistance.DownLeftSoccerScore / NowSoccerDistance.DownLeftSoccerTime), 4), 8, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.LeftSoccerDistance, 1.0f * LastSoccerDistance.LeftSoccerScore / LastSoccerDistance.LeftSoccerTime),
                                new Vector2(NowSoccerDistance.LeftSoccerDistance, 1.0f * NowSoccerDistance.LeftSoccerScore / NowSoccerDistance.LeftSoccerTime), 4), 9, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.UponLeftSoccerDistance, 1.0f * LastSoccerDistance.UponLeftSoccerScore / LastSoccerDistance.UponLeftSoccerTime),
                                new Vector2(NowSoccerDistance.UponLeftSoccerDistance, 1.0f * NowSoccerDistance.UponLeftSoccerScore / NowSoccerDistance.UponLeftSoccerTime), 4), 10, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.FrontSoccerDistance, 1.0f * LastSoccerDistance.FrontSoccerScore / LastSoccerDistance.FrontSoccerTime),
                                new Vector2(NowSoccerDistance.FrontSoccerDistance, 1.0f * NowSoccerDistance.FrontSoccerScore / NowSoccerDistance.FrontSoccerTime), 4), 11, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.BehindSoccerDistance, 1.0f * LastSoccerDistance.BehindSoccerScore / LastSoccerDistance.BehindSoccerTime),
                                new Vector2(NowSoccerDistance.BehindSoccerDistance, 1.0f * NowSoccerDistance.BehindSoccerScore / NowSoccerDistance.BehindSoccerTime), 4), 12, 3);

                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.UponSoccerDistance, 1.0f * LastSoccerDistance.UponSoccerScore / LastSoccerDistance.UponSoccerTime),
                                new Vector2(NowSoccerDistance.UponSoccerDistance, 1.0f * NowSoccerDistance.UponSoccerScore / NowSoccerDistance.UponSoccerTime), 2), 3, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.UponRightSoccerDistance, 1.0f * LastSoccerDistance.UponRightSoccerScore / LastSoccerDistance.UponRightSoccerTime),
                                new Vector2(NowSoccerDistance.UponRightSoccerDistance, 1.0f * NowSoccerDistance.UponRightSoccerScore / NowSoccerDistance.UponRightSoccerTime), 2), 4, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.RightSoccerDistance, 1.0f * LastSoccerDistance.RightSoccerScore / LastSoccerDistance.RightSoccerTime),
                                new Vector2(NowSoccerDistance.RightSoccerDistance, 1.0f * NowSoccerDistance.RightSoccerScore / NowSoccerDistance.RightSoccerTime), 2), 5, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.DownRightSoccerDistance, 1.0f * LastSoccerDistance.DownRightSoccerScore / LastSoccerDistance.DownRightSoccerTime),
                                new Vector2(NowSoccerDistance.DownRightSoccerDistance, 1.0f * NowSoccerDistance.DownRightSoccerScore / NowSoccerDistance.DownRightSoccerTime), 2), 6, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.DownSoccerDistance, 1.0f * LastSoccerDistance.DownSoccerScore / LastSoccerDistance.DownSoccerTime),
                                new Vector2(NowSoccerDistance.DownSoccerDistance, 1.0f * NowSoccerDistance.DownSoccerScore / NowSoccerDistance.DownSoccerTime), 2), 7, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.DownLeftSoccerDistance, 1.0f * LastSoccerDistance.DownLeftSoccerScore / LastSoccerDistance.DownLeftSoccerTime),
                                new Vector2(NowSoccerDistance.DownLeftSoccerDistance, 1.0f * NowSoccerDistance.DownLeftSoccerScore / NowSoccerDistance.DownLeftSoccerTime), 2), 8, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.LeftSoccerDistance, 1.0f * LastSoccerDistance.LeftSoccerScore / LastSoccerDistance.LeftSoccerTime),
                                new Vector2(NowSoccerDistance.LeftSoccerDistance, 1.0f * NowSoccerDistance.LeftSoccerScore / NowSoccerDistance.LeftSoccerTime), 2), 9, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.UponLeftSoccerDistance, 1.0f * LastSoccerDistance.UponLeftSoccerScore / LastSoccerDistance.UponLeftSoccerTime),
                                new Vector2(NowSoccerDistance.UponLeftSoccerDistance, 1.0f * NowSoccerDistance.UponLeftSoccerScore / NowSoccerDistance.UponLeftSoccerTime), 2), 10, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.FrontSoccerDistance, 1.0f * LastSoccerDistance.FrontSoccerScore / LastSoccerDistance.FrontSoccerTime),
                                new Vector2(NowSoccerDistance.FrontSoccerDistance, 1.0f * NowSoccerDistance.FrontSoccerScore / NowSoccerDistance.FrontSoccerTime), 2), 11, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastSoccerDistance.BehindSoccerDistance, 1.0f * LastSoccerDistance.BehindSoccerScore / LastSoccerDistance.BehindSoccerTime),
                                new Vector2(NowSoccerDistance.BehindSoccerDistance, 1.0f * NowSoccerDistance.BehindSoccerScore / NowSoccerDistance.BehindSoccerTime), 2), 12, 4);

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
            ConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
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

                if (SingleEvaluation > 0)
                {
                    List<Point> tempPoints = new List<Point>();

                    foreach (var point in LastEvaluationPoints)
                    {
                        tempPoints.Add(new Point(point.x, point.y));
                    }

                    LastConvexHull = new ConvexHull(tempPoints);

                    // 画凸包圈
                    LastConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
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
                    SetResultDataText(GetEvaluationResult(LastConvexHull.ConvexHullArea, NowConvexHull.ConvexHullArea, 4), 0, 3);
                    SetResultDataText(GetEvaluationResult(LastConvexHull.ConvexHullArea, NowConvexHull.ConvexHullArea, 2), 0, 4);


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

                LastNowConvexHullArea = new VectorLine("ConvexHullLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
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

            }
        }

        public void DrawSideLine()
        {
            float HalfVerticalOffset = 30f;

            SideLine = new VectorLine("SideLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            SideLine.smoothColor = false;   // 设置平滑颜色
            SideLine.smoothWidth = false;   // 设置平滑宽度
            SideLine.color = SideLineColor;  // 设置颜色

            NowFrontLine = new VectorLine("NowFrontLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            NowFrontLine.smoothColor = false;   // 设置平滑颜色
            NowFrontLine.smoothWidth = false;   // 设置平滑宽度
            NowFrontLine.color = ConvexHullLineColor;  // 设置颜色
            NowFrontLine.points2.Add(new Vector2(SideModelGravity.x + NowSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y + HalfVerticalOffset));
            NowFrontLine.points2.Add(new Vector2(SideModelGravity.x + NowSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y - HalfVerticalOffset));
            NowFrontLine.Draw();
            SetResultDataText(NowSoccerDistance.FrontSoccerDistance.ToString("0.0000"), 1, 2);

            NowBehindLine = new VectorLine("NowBehindLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            NowBehindLine.smoothColor = false;   // 设置平滑颜色
            NowBehindLine.smoothWidth = false;   // 设置平滑宽度
            NowBehindLine.color = ConvexHullLineColor;  // 设置颜色
            NowBehindLine.points2.Add(new Vector2(SideModelGravity.x - NowSoccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y + HalfVerticalOffset));
            NowBehindLine.points2.Add(new Vector2(SideModelGravity.x - NowSoccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y - HalfVerticalOffset));
            NowBehindLine.Draw();
            SetResultDataText(NowSoccerDistance.BehindSoccerDistance.ToString("0.0000"), 2, 2);

            float FrontX = SideModelGravity.x + NowSoccerDistance.FrontSoccerDistance * SideCoefficient;
            float BehindX = SideModelGravity.x - NowSoccerDistance.BehindSoccerDistance * SideCoefficient;

            if (SingleEvaluation > 0)
            {
                LastFrontLine = new VectorLine("LastFrontLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
                LastFrontLine.smoothColor = false;   // 设置平滑颜色
                LastFrontLine.smoothWidth = false;   // 设置平滑宽度
                LastFrontLine.color = LastConvexHullLineColor;  // 设置颜色
                LastFrontLine.points2.Add(new Vector2(SideModelGravity.x + LastSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y + HalfVerticalOffset));
                LastFrontLine.points2.Add(new Vector2(SideModelGravity.x + LastSoccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y - HalfVerticalOffset));
                LastFrontLine.Draw();
                SetResultDataText(LastSoccerDistance.FrontSoccerDistance.ToString("0.0000"), 1, 1);

                LastBehindLine = new VectorLine("LastBehindLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
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

            if (Mathf.Abs(Diff) < 1e-5)
            {
                return "<color=blue>" + "Around" + "</color>";
            }
            else if (Diff < 0)
            {
                return "<color=red>- " + GetColorOriginString(Last, -Diff, bits) + "</color>";
            }
            else if (Diff > 0)
            {
                return "<color=green>+ " + GetColorOriginString(Last, Diff, bits) + "</color>";
            }
            else
            {
                return "-";
            }
        }

        public string GetColorOriginString(float Last, float Diff, int bits)
        {
            string ResultString = "";

            if (bits == 2)
            {
                ResultString = (Diff / Last * 100).ToString("0.00") + "%";
            }
            else if (bits == 4)
            {
                ResultString = (Diff).ToString("0.0000");
            }
            return ResultString;
        }

        public void RemoveLines()
        {
            VectorLine.Destroy(ref NowFrontLine);
            VectorLine.Destroy(ref NowBehindLine);
            VectorLine.Destroy(ref LastFrontLine);
            VectorLine.Destroy(ref LastBehindLine);
            VectorLine.Destroy(ref SideLine);

            VectorLine.Destroy(ref ConvexHullLine);
            VectorLine.Destroy(ref ConvexHullArea);
            VectorLine.Destroy(ref LastConvexHullLine);
            VectorLine.Destroy(ref LastConvexHullArea);
            VectorLine.Destroy(ref LastNowConvexHullArea);
        }
        void Update()
        {

        }
    }
}