using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vectrosity;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class EvaluateInitScript : MonoBehaviour
    {
        // 当前的评估
        public int SingleEvaluation;

        // DataBG
        public GameObject NoEvaluateData;

        public Text EvaluateTime;
        public Text EvaluateButtonText;

        public GameObject ReadReportButton;

        public GameObject Report;
        public Toggle EvaluationToggle;
        //public GameObject SaveButton;

        public Sequence seq;
        public Image EvaluationButtonImage;

        public Dropdown EvaluationSelect;
        public Dictionary<string, int> EvaluationString2Int = new Dictionary<string, int>();
        public Dictionary<int, string> EvaluationInt2String = new Dictionary<int, string>();
        public List<string> ListEvaluationTime = new List<string>();


        // EvaluationInfo
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


        // SpeedRadar
        public Text RadarAreaText;

        public List<float> RadarArea;
        public List<float> RadarIncreaseRate;

        public string WhiteLine;

        public VectorLine ColorFistLineTrack;   // 彩色手势线
        public VectorLine ColorFistLineReality;   // 彩色手势线
        public VectorLine ConvexHullLine;   // 凸包线
        public VectorLine ConvexHullArea;   // 凸包区域填充
        public bool TrackIsOver;    // 判断轨迹是否画完

        public GameObject ManImage; // 男患者
        public GameObject ManSideImage; // 男患者侧身
        public GameObject WomanImage;   // 女患者
        public GameObject WomanSideImage;   // 女患者侧身

        public Toggle ContexHullToggle;
        //public Toggle TrackToggle;
        //public Toggle SoccerballToggle;
        public Toggle RealityToggle;

        public bool TrackIsDraw;

        // 绘制速度轨迹雷达图
        // 根据患者的身高来确定速度曲线或者凸包的缩小比例（默认为男性175cm，女性160cm）
        // 男模型: 身高段：425像素，肩宽：100像素，重心坐标（1430, 340）
        // 女模型: 身高段：425像素，肩宽： 80像素，重心坐标（1430，360）
        // 经测试，只需算身高段即可，不需要算肩宽

        public List<Point> EvaluationPoints;
        public ConvexHull LastConvexHull;   // 上次凸包
        public ConvexHull NowConvexHull;    // 本次凸包

        // 先平移,后放缩
        public Vector2 ModelGravity;   // 模型重心坐标
        public Vector2 SideModelGravity;    // 模型侧身重心坐标
        public Vector2 GravityDiff;    // 重心偏移
        public float HeightPixel;    // 身高段

        public Color[] ConvexHullColors;
        public Color ConvexHullAreaColor;
        public Color ConvexHullLineColor;

        public SoccerDistance tempSoccerDistance;

        // 显示本次和上次的凸包雷达图
        public Toggle LastConvexHullToggle;
        public Toggle NowConvexHullToggle;
        
        public Color LastConvexHullAreaColor;
        public Color LastConvexHullLineColor;
        public Color SideLineColor;

        public Color LastNowOverlappingColor;   // 两次重叠的颜色

        public VectorLine LastConvexHullLine;   // 凸包线
        public VectorLine LastConvexHullArea;   // 凸包区域填充

        public List<Point> LastEvaluationPoints;

        public List<Vector2> LastNowOverlappingPoints;

        public VectorLine LastNowConvexHullArea;   // 交集区域填充

        List<Vector2> LastConvexHullPoints; // 上次区域填充的交点
        List<Vector2> NowConvexHullPoints; // 本次区域填充的交点

        public Text LastConvexHullText; // 上次凸包文字 
        public Text NowConvexHullText;  // 本次凸包文字

        public VectorLine NowFrontLine; // 本次向前倾的线
        public VectorLine NowBehindLine;    //本次向后倾的线
        public VectorLine LastFrontLine; // 本次向前倾的线
        public VectorLine LastBehindLine;    //本次向后倾的线
        public VectorLine SideLine; // 侧身直线
        public static float SideCoefficient = 157f;   // 距离修正系数

        public Text TrackFastText;

        public float FrontX, BehindX;  // SideLine 前后大小



        // SoccerSpeedAndTime
        public BarChart SoccerBar;
        public Serie DistanceSerie, TimeSerie;


        void Start()
        {

        }

        void OnEnable()
        {

            // DataBG
            NoEvaluateData = transform.Find("NoEvaluateData").gameObject;
            EvaluateTime = transform.Find("DataBG/EvaluateTime").GetComponent<Text>();
            EvaluateButtonText = transform.Find("DataBG/EvaluateButton/Text").GetComponent<Text>();
            ReadReportButton = transform.Find("DataBG/ReadReportButton").gameObject;

            Report = transform.parent.parent.parent.Find("Report").gameObject;
            EvaluationToggle = transform.parent.parent.parent.Find("Report/ReportToggle/EvaluationToggle").GetComponent<Toggle>();

            EvaluationSelect = transform.Find("DataBG/EvaluationSelect").GetComponent<Dropdown>();
            //SaveButton = transform.Find("DataBG/SaveButton").gameObject;


            // EvaluationInfo
            Rank1.SetActive(false);
            Rank2.SetActive(false);
            Rank3.SetActive(false);
            Rank4.SetActive(false);
            Rank5.SetActive(false);

            if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
            {
                SingleEvaluation = DoctorDataManager.instance.doctor.patient.EvaluationIndex;

                // DataBG
                NoEvaluateData.SetActive(false);

                ReadReportButton.SetActive(true);

                Report.SetActive(false);
                //SaveButton.SetActive(true);

                //print(LastEvaluation.TrainingStartTime);
                EvaluateTime.text = "第" + (DoctorDataManager.instance.doctor.patient.EvaluationIndex + 1).ToString() + "次评估时间：" + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.EvaluationIndex].EvaluationStartTime;
                EvaluateButtonText.text = "再次评估";

                EvaluationButtonImage.color = Color.white;

                EvaluationSelect.gameObject.SetActive(true);
                EvaluationString2Int.Clear();
                EvaluationInt2String.Clear();
                ListEvaluationTime.Clear();
                EvaluationSelect.ClearOptions();

                for (int i = DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1; i >= 0; i--)
                {
                    string tempEvaluationTime = DoctorDataManager.instance.doctor.patient.Evaluations[i].EvaluationStartTime;
                    ListEvaluationTime.Add("第" + (i + 1).ToString() + "次 | " + tempEvaluationTime.Substring(4, 2) + "." + tempEvaluationTime.Substring(6, 2));
                }

                EvaluationSelect.AddOptions(ListEvaluationTime);
                EvaluationSelect.value = DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1 - DoctorDataManager.instance.doctor.patient.EvaluationIndex;


                // EvaluationInfo

                float TrainingEvaluationRate = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationScore;

                if (TrainingEvaluationRate >= 95f) { Rank1.SetActive(true); EvaluationRank.text = "1 级"; }
                else if (TrainingEvaluationRate >= 90f) { Rank2.SetActive(true); EvaluationRank.text = "2 级"; }
                else if (TrainingEvaluationRate >= 80f) { Rank3.SetActive(true); EvaluationRank.text = "3 级"; }
                else if (TrainingEvaluationRate >= 70f) { Rank4.SetActive(true); EvaluationRank.text = "4 级"; }
                else { Rank5.SetActive(true); EvaluationRank.text = "5 级"; }

                EvaluationScore.text = TrainingEvaluationRate.ToString("0.00");

                EvaluationStartTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationStartTime;
                EvaluationEndTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationEndTime;

                // 计算有效训练时长
                EvaluationTime.text = (long.Parse(EvaluationEndTime.text.Substring(9, 2)) * 3600 + long.Parse(EvaluationEndTime.text.Substring(12, 2)) * 60 + long.Parse(EvaluationEndTime.text.Substring(15, 2))
                                           - long.Parse(EvaluationStartTime.text.Substring(9, 2)) * 3600 - long.Parse(EvaluationStartTime.text.Substring(12, 2)) * 60 - long.Parse(EvaluationStartTime.text.Substring(15, 2))).ToString() + " 秒";



                // SpeedRadar
                EvaluationPoints = new List<Point>();
                foreach (var point in DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].Points)
                {
                    EvaluationPoints.Add(new Point(point.x, point.y));
                }

                tempSoccerDistance = new SoccerDistance(DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance);

                if (DoctorDataManager.instance.doctor.patient.PatientSex == "男")
                {
                    ManImage.SetActive(true); ManSideImage.SetActive(true);
                    WomanImage.SetActive(false); WomanSideImage.SetActive(false);

                    //WidthPixel = 100;
                    HeightPixel = 120;

                    ModelGravity = new Vector2(1270, 400);

                    SideModelGravity = new Vector2(1635, 400);
                }
                else
                {
                    ManImage.SetActive(false); ManSideImage.SetActive(false);
                    WomanImage.SetActive(true); WomanSideImage.SetActive(true);

                    //WidthPixel = 80;
                    HeightPixel = 120;

                    ModelGravity = new Vector2(1270, 410);
                    SideModelGravity = new Vector2(1635, 410);
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


                    LastConvexHullToggle.isOn = false;

                    LastConvexHullToggle.gameObject.SetActive(true);
                    NowConvexHullToggle.gameObject.SetActive(true);

                    NowConvexHullToggle.transform.localPosition = new Vector3(-78.5f, -109.23f, 0);

                }
                else
                {
                    LastConvexHullToggle.isOn = false;

                    LastConvexHullToggle.gameObject.SetActive(false);
                    NowConvexHullToggle.gameObject.SetActive(true);

                    NowConvexHullToggle.transform.localPosition = new Vector3(-78.5f, -90f, 0);


                }

                // 默认画凸包图

                ConvexHullLineColor = new Color32(255, 0, 0, 255);
                ConvexHullAreaColor = new Color32(255, 0, 0, 40);

                LastConvexHullLineColor = new Color32(0, 255, 0, 255);
                LastConvexHullAreaColor = new Color32(0, 255, 0, 40);

                LastNowOverlappingColor = new Color32(255, 255, 0, 15);

                SideLineColor = new Color32(255, 140, 5, 255);

                LastConvexHullText.text = "上次评估";
                NowConvexHullText.text = "本次评估";

                //// 画侧身直线
                //DrawSideLine();

                // 画凸包图
                ContexHullToggle.isOn = true;
                //DrawContexHullToggleChange();


                // SoccerSpeedAndTime

                SoccerBar = transform.Find("SoccerBar/BarChart").gameObject.GetComponent<BarChart>();
                if (SoccerBar == null) SoccerBar = transform.Find("SoccerBar/BarChart").gameObject.AddComponent<BarChart>();

                // 写入数据
                SoccerBar.series.UpdateData(0, 0, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerDistance);
                SoccerBar.series.UpdateData(0, 1, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerDistance);
                SoccerBar.series.UpdateData(0, 2, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerDistance);
                SoccerBar.series.UpdateData(0, 3, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerDistance);
                SoccerBar.series.UpdateData(0, 4, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerDistance);
                SoccerBar.series.UpdateData(0, 5, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerDistance);
                SoccerBar.series.UpdateData(0, 6, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerDistance);
                SoccerBar.series.UpdateData(0, 7, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerDistance);
                SoccerBar.series.UpdateData(0, 8, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance);
                SoccerBar.series.UpdateData(0, 9, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance);
                
                SoccerBar.series.UpdateData(1, 0, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerTime);
                SoccerBar.series.UpdateData(1, 1, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerTime);
                SoccerBar.series.UpdateData(1, 2, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerTime);
                SoccerBar.series.UpdateData(1, 3, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerTime);
                SoccerBar.series.UpdateData(1, 4, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerTime);
                SoccerBar.series.UpdateData(1, 5, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerTime);
                SoccerBar.series.UpdateData(1, 6, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerTime);
                SoccerBar.series.UpdateData(1, 7, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerTime);
                SoccerBar.series.UpdateData(1, 8, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerTime);
                SoccerBar.series.UpdateData(1, 9, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerTime);

                SoccerBar.RefreshChart();

            }
            else
            {
                NoEvaluateData.SetActive(true);
                ReadReportButton.SetActive(false);
                Report.SetActive(false);

                EvaluationSelect.gameObject.SetActive(false);
                //SaveButton.SetActive(false);

                EvaluateTime.text = "点击右侧按钮对患者进行状况评估";
                EvaluateButtonText.text = "状况评估";

                Tweener t1 = EvaluationButtonImage.DOColor(new Color(60 / 255, 255 / 255, 60 / 255), 0.8f);
                Tweener t2 = EvaluationButtonImage.DOColor(Color.white, 0.8f);
                seq = DOTween.Sequence();
                seq.Append(t1);
                seq.Append(t2);
                seq.SetLoops(-1);
            }

        }

        public void DrawSideLine()
        {
            VectorLine.Destroy(ref SideLine);

            SideLine = new VectorLine("SideLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            SideLine.smoothColor = false;   // 设置平滑颜色
            SideLine.smoothWidth = false;   // 设置平滑宽度
            SideLine.color = SideLineColor;  // 设置颜色

            if (NowConvexHullToggle.isOn && LastConvexHullToggle.isOn)
            {
                FrontX = SideModelGravity.x + DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient;
                BehindX = SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient;

                BehindX = Mathf.Min(BehindX, SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient);
                BehindX = Mathf.Min(BehindX, SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient);
            }
            else if (NowConvexHullToggle.isOn)
            {
                FrontX = SideModelGravity.x + DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient;
                BehindX = SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient;
            }
            else if (LastConvexHullToggle.isOn)
            {
                FrontX = SideModelGravity.x + DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient;
                BehindX = SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient;
            }

            //print(FrontX + " " + SideModelGravity.y + " " + BehindX);

            SideLine.points2.Add(new Vector2(FrontX, SideModelGravity.y));
            SideLine.points2.Add(new Vector2(BehindX, SideModelGravity.y));
            SideLine.Draw();

        }


        public void DrawContexHullToggleChange()
        {
            if (ContexHullToggle.isOn)
            {
                TrackFastText.color = new Color32(99, 212, 189, 0);

                if (NowConvexHullToggle.isOn == true) NowConvexHullToggle.isOn = false;

                NowConvexHullToggle.isOn = true;
                //LastConvexHullToggle.isOn = true;
                
            }
            else
            {
                LastConvexHullToggle.isOn = false;
                NowConvexHullToggle.isOn = false;

                //LastConvexHullToggle.gameObject.SetActive(false);
                //NowConvexHullToggle.gameObject.SetActive(false);
            }
        }

        public void DrawNowSideLine()
        {
            //VectorLine.Destroy(ref NowFrontLine);
            //VectorLine.Destroy(ref NowBehindLine);

            NowFrontLine = new VectorLine("NowFrontLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            NowFrontLine.smoothColor = false;   // 设置平滑颜色
            NowFrontLine.smoothWidth = false;   // 设置平滑宽度
            NowFrontLine.color = ConvexHullLineColor;  // 设置颜色
            NowFrontLine.points2.Add(new Vector2(SideModelGravity.x + DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y + 50));
            NowFrontLine.points2.Add(new Vector2(SideModelGravity.x + DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y - 50));
            NowFrontLine.Draw();

            NowBehindLine = new VectorLine("NowBehindLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            NowBehindLine.smoothColor = false;   // 设置平滑颜色
            NowBehindLine.smoothWidth = false;   // 设置平滑宽度
            NowBehindLine.color = ConvexHullLineColor;  // 设置颜色
            NowBehindLine.points2.Add(new Vector2(SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y + 50));
            NowBehindLine.points2.Add(new Vector2(SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y - 50));
            NowBehindLine.Draw();

            DrawSideLine();

        }



        public void DrawNowContexHullToggleChange()
        {
            if (NowConvexHullToggle.isOn && ContexHullToggle.isOn)
            {
                DrawNowSideLine();
                DrawContexHull();
            }
            else
            {
                //NowConvexHullToggle.isOn = false;

                VectorLine.Destroy(ref ConvexHullLine);
                VectorLine.Destroy(ref ConvexHullArea);

                VectorLine.Destroy(ref LastNowConvexHullArea);

                if (LastConvexHullToggle.isOn)
                {
                    DrawSideLine();
                }
                else
                {
                    VectorLine.Destroy(ref SideLine);
                }

                VectorLine.Destroy(ref NowFrontLine);
                VectorLine.Destroy(ref NowBehindLine);
            }
        }

        public void DrawLastSideLine()
        {
            //VectorLine.Destroy(ref LastFrontLine);
            //VectorLine.Destroy(ref LastBehindLine);

            LastFrontLine = new VectorLine("LastFrontLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            LastFrontLine.smoothColor = false;   // 设置平滑颜色
            LastFrontLine.smoothWidth = false;   // 设置平滑宽度
            LastFrontLine.color = LastConvexHullLineColor;  // 设置颜色
            LastFrontLine.points2.Add(new Vector2(SideModelGravity.x + DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y + 50));
            LastFrontLine.points2.Add(new Vector2(SideModelGravity.x + DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient, SideModelGravity.y - 50));
            LastFrontLine.Draw();

            LastBehindLine = new VectorLine("LastBehindLine", new List<Vector2>(), 6.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            LastBehindLine.smoothColor = false;   // 设置平滑颜色
            LastBehindLine.smoothWidth = false;   // 设置平滑宽度
            LastBehindLine.color = LastConvexHullLineColor;  // 设置颜色
            LastBehindLine.points2.Add(new Vector2(SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y + 50));
            LastBehindLine.points2.Add(new Vector2(SideModelGravity.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient, SideModelGravity.y - 50));
            LastBehindLine.Draw();

            DrawSideLine();
        }



        public void DrawLastContexHullToggleChange()
        {
            if (LastConvexHullToggle.isOn && ContexHullToggle.isOn)
            {
                DrawLastSideLine();
                DrawLastContexHull();
            }
            else
            {
                //LastConvexHullToggle.isOn = false;

                VectorLine.Destroy(ref LastConvexHullLine);
                VectorLine.Destroy(ref LastConvexHullArea);

                VectorLine.Destroy(ref LastNowConvexHullArea);

                if (NowConvexHullToggle.isOn)
                {
                    DrawSideLine();
                }
                else
                {
                    VectorLine.Destroy(ref SideLine);
                }
                VectorLine.Destroy(ref LastFrontLine);
                VectorLine.Destroy(ref LastBehindLine);
            }
        }

        public void DrawLastContexHull()
        {
            if (LastEvaluationPoints == null) return;
            //if (ConvexHullLine != null) return;

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


            int MinX = Mathf.FloorToInt(LastConvexHull.ConvexHullSet[0].x), MaxX = Mathf.CeilToInt(LastConvexHull.ConvexHullSet[0].x);   // 凸包的最大最小X
            int MinY = Mathf.FloorToInt(LastConvexHull.ConvexHullSet[0].y), MaxY = Mathf.CeilToInt(LastConvexHull.ConvexHullSet[0].y);   // 凸包的最大最小Y


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

            // 多算几个单位
            StartCoroutine(DrawLastConvexHullArea(MinX - 10, MaxX + 10, MinY - 10, MaxY + 10));
        }

        IEnumerator DrawLastConvexHullArea(int MinX, int MaxX, int MinY, int MaxY)
        {
            yield return new WaitForEndOfFrame();

            if (ContexHullToggle.isOn)
            {
                //if (ConvexHullArea.points2 != 0 )
                //{

                //if (ConvexHullArea == null)
                //{
                // 画透明区域
                LastConvexHullArea = new VectorLine("ConvexHullLine", new List<Vector2>(), 1f, Vectrosity.LineType.Continuous, Joins.Weld);
                LastConvexHullArea.smoothColor = false;   // 设置平滑颜色
                LastConvexHullArea.smoothWidth = false;   // 设置平滑宽度
                LastConvexHullArea.color = LastConvexHullAreaColor;

                Texture2D m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
                m_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
                //m_texture.Apply();

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
                //}
                //}

                LastConvexHullText.text = "上次评估:雷达图(" + (LastConvexHull.ConvexHullArea / SideCoefficient / SideCoefficient).ToString("0.00") + ")";
                LastConvexHullText.text += ",前倾(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00") + ")";
                LastConvexHullText.text += ",后仰(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00") + ")";

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
                    if (RadarAreaIncreaseRate < 0) NowConvexHullText.text += "<color=blue>↓" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (RadarAreaIncreaseRate == 0) NowConvexHullText.text += "<color=green>↔" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += "<color=red>↑" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";

                    NowConvexHullText.text += ",前倾(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00");
                    float FrontIncreaseRate = (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient
                                                - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient)
                                                / (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient);
                    if (FrontIncreaseRate < 0) NowConvexHullText.text += "<color=blue>↓" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (FrontIncreaseRate == 0) NowConvexHullText.text += "<color=green>↔" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += "<color=red>↑" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";

                    NowConvexHullText.text += ",后仰(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00");
                    float BehindIncreaseRate = (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient
                                                - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient)
                                                / (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient);
                    if (BehindIncreaseRate < 0) NowConvexHullText.text += "<color=blue>↓" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (BehindIncreaseRate == 0) NowConvexHullText.text += "<color=green>↔" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += "<color=red>↑" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";
                }
            }
            else
            {
                VectorLine.Destroy(ref LastConvexHullLine);
                VectorLine.Destroy(ref LastConvexHullArea);
            }
        }





        public void DrawContexHull()
        {
            if (EvaluationPoints == null) return;
            //if (ConvexHullLine != null) return;

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

            // 多算几个单位
            StartCoroutine(DrawConvexHullArea(MinX - 10, MaxX + 10, MinY - 10, MaxY + 10));
        }

        IEnumerator DrawConvexHullArea(int MinX, int MaxX, int MinY, int MaxY)
        {
            yield return new WaitForEndOfFrame();

            if (ContexHullToggle.isOn)
            {                
                //if (ConvexHullArea.points2 != 0 )
                //{

                //if (ConvexHullArea == null)
                //{
                    // 画透明区域
                ConvexHullArea = new VectorLine("ConvexHullLine", new List<Vector2>(), 1f, Vectrosity.LineType.Continuous, Joins.Weld);
                ConvexHullArea.smoothColor = false;   // 设置平滑颜色
                ConvexHullArea.smoothWidth = false;   // 设置平滑宽度
                ConvexHullArea.color = ConvexHullAreaColor;

                Texture2D m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
                m_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
                //m_texture.Apply();

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
                //}
                //}
                NowConvexHullText.text = "本次评估:雷达图(" + (NowConvexHull.ConvexHullArea / SideCoefficient / SideCoefficient).ToString("0.00") + ")";
                NowConvexHullText.text += ",前倾(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00") + ")";
                NowConvexHullText.text += ",后仰(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00") + ")";

                if (LastConvexHullToggle.isOn)
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
                            if(x2 <= x4)
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
                    if (RadarAreaIncreaseRate < 0) NowConvexHullText.text += "<color=blue>↓" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (RadarAreaIncreaseRate == 0) NowConvexHullText.text += "<color=green>↔" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += "<color=red>↑" + (RadarAreaIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";

                    NowConvexHullText.text += ",前倾(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00");
                    float FrontIncreaseRate = (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance * SideCoefficient
                                                - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient)
                                                / (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.FrontSoccerDistance * SideCoefficient);
                    if (FrontIncreaseRate < 0) NowConvexHullText.text += "<color=blue>↓" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (FrontIncreaseRate == 0) NowConvexHullText.text += "<color=green>↔" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += "<color=red>↑" + (FrontIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";

                    NowConvexHullText.text += ",后仰(" + (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient / SideCoefficient).ToString("0.00");
                    float BehindIncreaseRate = (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance * SideCoefficient
                                                - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient)
                                                / (DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation - 1].soccerDistance.BehindSoccerDistance * SideCoefficient);
                    if (BehindIncreaseRate < 0) NowConvexHullText.text += "<color=blue>↓" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else if (BehindIncreaseRate == 0) NowConvexHullText.text += "<color=green>↔" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    else NowConvexHullText.text += "<color=red>↑" + (BehindIncreaseRate * 100).ToString("0.00") + "%</color>";
                    NowConvexHullText.text += ")";
                }

            }
            else
            {
                VectorLine.Destroy(ref ConvexHullLine);
                VectorLine.Destroy(ref ConvexHullArea);
            }
        }




        //public void DrawTrackToggleChange()
        //{
        //    if (TrackToggle.isOn)
        //    {
        //        DrawColorFistTrack();
        //    }
        //    else
        //    {
        //        VectorLine.Destroy(ref ColorFistLineTrack);
        //    }
        //}


        public void DrawColorFistTrack()
        {
            if(TrackIsDraw || (EvaluationPoints == null))
            {
                return;
            }

            TrackIsDraw = true;

            List<Point> tempPoints = new List<Point>();

            foreach (var point in EvaluationPoints)
            {
                tempPoints.Add(new Point(point.x, point.y));
            }

            ColorFistLineTrack = new VectorLine("ColorFistLine", new List<Vector2>(), 5.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            ColorFistLineTrack.smoothColor = false;   // 设置平滑颜色
            ColorFistLineTrack.smoothWidth = false;   // 设置平滑宽度
            //ColorFistLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

            int DeltaBase = 0, DeltaColorR = 0, DeltaColorG = 0;

            ColorFistLineTrack.points2.Add(new Vector2(tempPoints[0].x, tempPoints[0].y));

            for (int i = 1; i < tempPoints.Count; i++)
            {
                //ColorFistLine.Draw();

                ColorFistLineTrack.points2.Add(new Vector2(tempPoints[i].x, tempPoints[i].y));

                DeltaBase = (int)(Point.PointsDistance(tempPoints[i - 1], tempPoints[i]) * 7);

                if (DeltaBase <= 0) { DeltaColorR = 0; DeltaColorG = 0; }
                else if (DeltaBase > 0 && DeltaBase <= 255) { DeltaColorR = DeltaBase; DeltaColorG = 0; }
                else if (DeltaBase > 255 && DeltaBase <= 510) { DeltaColorR = 255; DeltaColorG = DeltaBase - 255; }
                else if (DeltaBase > 510) { DeltaColorR = 255; DeltaColorG = 255; }

                ColorFistLineTrack.SetColor(new Color32((Byte)DeltaColorR, (Byte)(255 - DeltaColorG), 0, (Byte)255), i - 1);
                //ColorFistLine.SetWidth(7.0f * LastNowDis / 20, Points.Count - 2);
                //yield return new WaitForSeconds(0.01f);
            }
            ColorFistLineTrack.Draw();
            
        }

        public void DrawRealityToggleChange()
        {
            if (RealityToggle.isOn)
            {
                TrackIsDraw = false;
                TrackIsOver = false;

                TrackFastText.color = new Color32(99, 212,189, 255);

                StartCoroutine(DrawColorFistLine());
            }
            else
            {
                //ColorFistLineReality.points2.Clear();
                TrackIsOver = true;

                if (ColorFistLineReality != null)
                {
                    VectorLine.Destroy(ref ColorFistLineReality);
                }

                if (ColorFistLineTrack != null)
                {
                    VectorLine.Destroy(ref ColorFistLineTrack);
                }
            }
        }


        IEnumerator DrawColorFistLine()
        {

            List<Point> tempPoints = new List<Point>();


            foreach (var point in EvaluationPoints)
            {
                tempPoints.Add(new Point(point.x, point.y));
            }


            ColorFistLineReality = new VectorLine("ColorFistLine", new List<Vector2>(), 5.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            ColorFistLineReality.smoothColor = false;   // 设置平滑颜色
            ColorFistLineReality.smoothWidth = false;   // 设置平滑宽度
            ColorFistLineReality.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

            int DeltaBase = 0, DeltaColorR = 0, DeltaColorG = 0;

            ColorFistLineReality.points2.Add(new Vector2(tempPoints[0].x, tempPoints[0].y));

            for (int i = 1; i < tempPoints.Count; i++)
            {

                if (TrackIsOver)
                {
                    VectorLine.Destroy(ref ColorFistLineReality);
                    VectorLine.Destroy(ref ColorFistLineTrack);
                    break;
                }

                //ColorFistLine.Draw();

                ColorFistLineReality.points2.Add(new Vector2(tempPoints[i].x, tempPoints[i].y));

                DeltaBase = (int)(Point.PointsDistance(tempPoints[i - 1], tempPoints[i]) * 7);

                if (DeltaBase <= 0) { DeltaColorR = 0; DeltaColorG = 0; }
                else if (DeltaBase > 0 && DeltaBase <= 255) { DeltaColorR = DeltaBase; DeltaColorG = 0; }
                else if (DeltaBase > 255 && DeltaBase <= 510) { DeltaColorR = 255; DeltaColorG = DeltaBase - 255; }
                else if (DeltaBase > 510) { DeltaColorR = 255; DeltaColorG = 255; }

                ColorFistLineReality.SetColor(new Color32((Byte)DeltaColorR, (Byte)(255 - DeltaColorG), 0, (Byte)255), i - 1);
                //ColorFistLine.SetWidth(7.0f * LastNowDis / 20, Points.Count - 2);
                ColorFistLineReality.Draw();
                
                yield return new WaitForSeconds(0.01f);
            }

            
        }

        //public void DrawSoccerballToggleChange()
        //{
        //    if (SoccerballToggle.isOn)
        //    {
        //        DrawSoccerball();
        //    }
        //    else
        //    {
        //        transform.GetChild(2).GetChild(4).gameObject.SetActive(false);
        //    }
        //}

        //public void DrawSoccerball()
        //{
        //    transform.GetChild(2).GetChild(4).gameObject.SetActive(true);

        //    print(Kinect2UIPosition(tempSoccerDistance.UponSoccer));

        //    //transform.GetChild(2).GetChild(4).GetChild(0).position = tempSoccerDistance.UponSoccer;
        //    //transform.GetChild(2).GetChild(4).GetChild(1).position = tempSoccerDistance.UponRightSoccer;
        //    //transform.GetChild(2).GetChild(4).GetChild(2).position = tempSoccerDistance.RightSoccer;
        //    //transform.GetChild(2).GetChild(4).GetChild(3).position = tempSoccerDistance.DownRightSoccer;
        //    //transform.GetChild(2).GetChild(4).GetChild(4).position = tempSoccerDistance.DownSoccer;
        //    //transform.GetChild(2).GetChild(4).GetChild(5).position = tempSoccerDistance.DownLeftSoccer;
        //    //transform.GetChild(2).GetChild(4).GetChild(6).position = tempSoccerDistance.LeftSoccer;
        //    //transform.GetChild(2).GetChild(4).GetChild(7).position = tempSoccerDistance.UponLeftSoccer;

        //}


        public void EvaluationChange()
        {
            // 刷新评估界面
            DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1 - EvaluationSelect.value);

            RemoveLine();

            OnEnable();
        }

        public void RemoveLine()
        {
            VectorLine.Destroy(ref NowFrontLine);
            VectorLine.Destroy(ref NowBehindLine);
            VectorLine.Destroy(ref LastFrontLine);
            VectorLine.Destroy(ref LastBehindLine);
            VectorLine.Destroy(ref SideLine);

            ContexHullToggle.isOn = false;
            RealityToggle.isOn = false;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (RealityToggle.isOn)
                {
                    TrackFastText.color = new Color32(99, 212, 189, 0);
                    DrawColorFistTrack();
                    //StopCoroutine("DrawColorFistLine");
                }
            }

        }
        

        public void EvaluateButtonOnclick()
        {

            DoctorDataManager.instance.FunctionManager = 1;  // 返回的时候进入患者状况评估界面
            
            RemoveLine();
            
            SceneManager.LoadScene("05-RadarTest");
        }

        public void ReadReportButtonOnclick()
        {
            RemoveLine();

            Report.SetActive(true);
            EvaluationToggle.isOn = true;
        }

    }
}

