using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Vectrosity;
using System.Collections;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class TrainingPrintScript : MonoBehaviour
    {

        public int SingleTrainingPlay;
        public TrainingPlay trainingPlay;
        public TrainingPlay LastTrainingPlay;

        // Title
        public Text TrainingTitle;

        // Information
        public Text InformationPatientID;
        public Text InformationPatientName;
        public Text InformationPatientSex;
        public Text InformationPatientAge;
        public Text InformationPatientHeight;
        public Text InformationPatientWeight;
        public Text InformationPatientSymptom;
        public Text InformationPatientDoctor;

        // Training
        public Text TrainingDifficulty;
        public Text TrainingRank;
        public Text TrainingDuration;
        public Text TrainingDirection;
        public Text TrainingSuccessRate;
        public Text TrainingTime;

        // Chart
        RadarChart DirectionRadarChart;
        public long GravityCenterCount;
        LineChart GravityCenterChart;
        Serie GravityCenterSerie;

        void OnEnable()
        {
            if (DoctorDataManager.instance.doctor.patient.TrainingPlays == null)
            {
                DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);
                if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
                {
                    foreach (var item in DoctorDataManager.instance.doctor.patient.TrainingPlays)
                    {
                        if (DoctorDataManager.instance.doctor.patient.MaxSuccessCount < item.SuccessCount)
                        {
                            DoctorDataManager.instance.doctor.patient.SetMaxSuccessCount(item.SuccessCount);
                        }
                    }

                    DoctorDataManager.instance.doctor.patient.SetTrainingPlayIndex(DoctorDataManager.instance.doctor.patient.TrainingPlays.Count - 1);
                    //print(DoctorDataManager.instance.doctor.patient.TrainingPlayIndex);
                }
            }
            
            if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
            {
                SingleTrainingPlay = DoctorDataManager.instance.doctor.patient.TrainingPlayIndex;
                trainingPlay = DoctorDataManager.instance.doctor.patient.TrainingPlays[SingleTrainingPlay];

                if (SingleTrainingPlay > 0)
                {
                    LastTrainingPlay = DoctorDataManager.instance.doctor.patient.TrainingPlays[SingleTrainingPlay - 1];
                }

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

                TrainingTitle = transform.Find("TrainingTitle").GetComponent<Text>();
                TrainingTitle.text = DoctorDataManager.instance.doctor.patient.PatientName + "\'s" + (SingleTrainingPlay + 1).ToString() + sequence[SingleTrainingPlay] + "Training Report";

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


                // Training
                TrainingDifficulty = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Difficulty/TrainingDifficulty").GetComponent<Text>();
                TrainingDifficulty.text = trainingPlay.TrainingDifficulty;

                TrainingRank = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Rank/TrainingRank").GetComponent<Text>();
                float TrainingEvaluationRate = 1.0f * trainingPlay.SuccessCount / trainingPlay.GameCount;
                if (TrainingEvaluationRate >= 0.8f) { TrainingRank.text = "S 级"; }
                else if (TrainingEvaluationRate >= 0.7f) { TrainingRank.text = "A 级"; }
                else if (TrainingEvaluationRate >= 0.6f) { TrainingRank.text = "B 级"; }
                else if (TrainingEvaluationRate >= 0.5f) { TrainingRank.text = "C 级"; }
                else if (TrainingEvaluationRate >= 0.4f) { TrainingRank.text = "D 级"; }
                else { TrainingRank.text = "E 级"; }


                TrainingDuration = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Duration/TrainingDuration").GetComponent<Text>();
                TrainingDuration.text = (long.Parse(trainingPlay.TrainingEndTime.Substring(9, 2)) * 3600 + long.Parse(trainingPlay.TrainingEndTime.Substring(12, 2)) * 60 + long.Parse(trainingPlay.TrainingEndTime.Substring(15, 2))
                                           - long.Parse(trainingPlay.TrainingStartTime.Substring(9, 2)) * 3600 - long.Parse(trainingPlay.TrainingStartTime.Substring(12, 2)) * 60 - long.Parse(trainingPlay.TrainingStartTime.Substring(15, 2))).ToString() + " 秒";

                TrainingDirection = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Direction/TrainingDirection").GetComponent<Text>();
                TrainingDirection.text = trainingPlay.TrainingDirection;

                TrainingSuccessRate = transform.Find("TrainingFeedback/TrainingFeedbackInfo/SuccessRate/TrainingSuccessRate").GetComponent<Text>();
                TrainingSuccessRate.text = trainingPlay.SuccessCount.ToString() + "/" + trainingPlay.GameCount.ToString();

                TrainingTime = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Time/TrainingTime").GetComponent<Text>();
                TrainingTime.text = trainingPlay.TrainingStartTime;


                // Chart
                // 初始化对比结果
                for (int m = 0; m < 19; m++)
                {
                    for (int n = 1; n < 5; n++)
                    {
                        SetResultDataText("-", m, n);
                    }
                }

                DrawRadarChart();
                DrawGravityCenterOffset();
                WriteAngleData();

            }

        }

        public void DrawRadarChart()
        {
            DirectionRadarChart = transform.Find("Chart/RadarChart").GetComponent<RadarChart>();
            if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("Chart/RadarChart").gameObject.AddComponent<RadarChart>();

            DirectionRadarChart.UpdateData(0, 0, 0, trainingPlay.direction.UponDirection);
            DirectionRadarChart.UpdateData(0, 0, 1, trainingPlay.direction.UponRightDirection);
            DirectionRadarChart.UpdateData(0, 0, 2, trainingPlay.direction.RightDirection);
            DirectionRadarChart.UpdateData(0, 0, 3, trainingPlay.direction.DownRightDirection);
            DirectionRadarChart.UpdateData(0, 0, 4, trainingPlay.direction.DownDirection);
            DirectionRadarChart.UpdateData(0, 0, 5, trainingPlay.direction.DownLeftDirection);
            DirectionRadarChart.UpdateData(0, 0, 6, trainingPlay.direction.LeftDirection);
            DirectionRadarChart.UpdateData(0, 0, 7, trainingPlay.direction.UponLeftDirection);

            DirectionRadarChart.RefreshChart();

            SetResultDataText(trainingPlay.direction.DirectionRadarArea.ToString("0.0000"), 0, 2);

            if (SingleTrainingPlay > 0)
            {
                SetResultDataText(LastTrainingPlay.direction.DirectionRadarArea.ToString("0.0000"), 0, 1);

                SetResultDataText(GetEvaluationResult(LastTrainingPlay.direction.DirectionRadarArea, trainingPlay.direction.DirectionRadarArea, 4), 0, 3);
                SetResultDataText(GetEvaluationResult(LastTrainingPlay.direction.DirectionRadarArea, trainingPlay.direction.DirectionRadarArea, 2), 0, 4);
            }
        }

        public void DrawGravityCenterOffset()
        {
            GravityCenterCount = trainingPlay.gravityCenters.Count;
            GravityCenterChart = transform.Find("Chart/GravityCenterChart").GetComponent<LineChart>();
            if (GravityCenterChart == null) GravityCenterChart = transform.Find("Chart/GravityCenterChart").gameObject.AddComponent<LineChart>();

            GravityCenterChart.themeInfo.theme = Theme.Light;
            //chart.themeInfo.tooltipBackgroundColor = Color.white;
            //chart.themeInfo.backgroundColor = Color.grey;

            GravityCenterChart.title.show = true;
            GravityCenterChart.title.text = "Gravity Center";
            GravityCenterChart.title.textStyle.fontSize = 14;
            GravityCenterChart.title.textStyle.fontStyle = FontStyle.Bold;
            GravityCenterChart.title.location.top = 22;
            GravityCenterChart.title.subText = "Origin Offset Distance";

            //chart.title.subText = "前30s";
            //chart.title.subTextFontSize = 18;

            GravityCenterChart.legend.show = false;
            GravityCenterChart.legend.location.align = Location.Align.TopRight;
            GravityCenterChart.legend.location.top = 2;
            GravityCenterChart.legend.location.right = 55;
            GravityCenterChart.legend.orient = Orient.Horizonal;  // 图例显示方向
            GravityCenterChart.legend.itemGap = 0;       // `图例之间的距离
            GravityCenterChart.legend.itemWidth = 25;
            GravityCenterChart.legend.itemHeight = 25;

            GravityCenterChart.tooltip.show = true;
            GravityCenterChart.tooltip.type = Tooltip.Type.Line;
            GravityCenterChart.tooltip.titleFormatter = "   第{b}秒   ";
            GravityCenterChart.tooltip.itemFormatter = "重心距离为{c}";

            GravityCenterChart.xAxis0.show = true;
            GravityCenterChart.xAxis0.type = XAxis.AxisType.Category;
            GravityCenterChart.xAxis0.splitNumber = 8;   // 把数据分成多少份
            GravityCenterChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
            GravityCenterChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            GravityCenterChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
            GravityCenterChart.xAxis0.axisLine.symbolWidth = 10;
            GravityCenterChart.xAxis0.axisLine.symbolHeight = 15;
            GravityCenterChart.xAxis0.axisLine.symbolOffset = 0;
            GravityCenterChart.xAxis0.axisLine.symbolDent = 3;
            GravityCenterChart.xAxis0.axisName.show = true;  // 坐标轴名称
            GravityCenterChart.xAxis0.axisName.name = "Time";
            GravityCenterChart.xAxis0.axisName.location = AxisName.Location.Middle;
            GravityCenterChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
            GravityCenterChart.xAxis0.axisName.rotate = 0;
            GravityCenterChart.xAxis0.axisName.color = Color.black;
            GravityCenterChart.xAxis0.axisName.fontSize = 13;
            GravityCenterChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
            GravityCenterChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            GravityCenterChart.xAxis0.axisLabel.fontSize = 12;
            GravityCenterChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            GravityCenterChart.xAxis0.splitLine.show = true;
            //GravityCenterChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
            GravityCenterChart.xAxis0.splitArea.show = true;
            GravityCenterChart.xAxis0.boundaryGap = false;

            GravityCenterChart.yAxis0.show = true;
            GravityCenterChart.yAxis0.type = YAxis.AxisType.Value;
            GravityCenterChart.yAxis0.splitNumber = 8;   // 把数据分成多少份
            GravityCenterChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
            GravityCenterChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            GravityCenterChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
            GravityCenterChart.yAxis0.axisLine.symbolWidth = 10;
            GravityCenterChart.yAxis0.axisLine.symbolHeight = 15;
            GravityCenterChart.yAxis0.axisLine.symbolOffset = 0;
            GravityCenterChart.yAxis0.axisLine.symbolDent = 3;
            GravityCenterChart.yAxis0.axisName.show = true;  // 坐标轴名称
            GravityCenterChart.yAxis0.axisName.name = "Distance";
            GravityCenterChart.yAxis0.axisName.location = AxisName.Location.Middle;
            GravityCenterChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
            GravityCenterChart.yAxis0.axisName.rotate = 90;
            GravityCenterChart.yAxis0.axisName.color = Color.black;
            GravityCenterChart.yAxis0.axisName.fontSize = 13;
            GravityCenterChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
            GravityCenterChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            GravityCenterChart.yAxis0.axisLabel.fontSize = 12;
            GravityCenterChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            GravityCenterChart.yAxis0.splitLine.show = true;
            //GravityCenterChart.yAxis0.splitLine.lineStyle.show = true;
            GravityCenterChart.yAxis0.splitArea.show = true;
            GravityCenterChart.yAxis0.axisLabel.formatter = "{value:f1}";

            GravityCenterChart.RemoveData();
            GravityCenterSerie = GravityCenterChart.AddSerie(SerieType.Line, "重心");//添加折线图

            GravityCenterSerie.areaStyle.show = true;
            GravityCenterSerie.areaStyle.opacity = 0.4f;
            GravityCenterSerie.areaStyle.toColor = Color.white;


            GravityCenterSerie.symbol.type = SerieSymbolType.None;

            GravityCenterChart.grid.left = 50;
            GravityCenterChart.grid.right = 20;
            GravityCenterChart.grid.top = 50;
            GravityCenterChart.grid.bottom = 25;

            GravityCenterChart.dataZoom.enable = true;
            GravityCenterChart.dataZoom.supportInside = true;
            GravityCenterChart.dataZoom.start = 0;
            GravityCenterChart.dataZoom.end = 100;
            GravityCenterChart.dataZoom.minShowNum = 30;

            float MaxGravityCenter = 0f;

            float tempDistance;

            for (int i = 0; i < GravityCenterCount; i++)
            {
                tempDistance = 1000 * Vector3.Distance(trainingPlay.gravityCenters[i].Coordinate, trainingPlay.gravityCenters[0].Coordinate);

                GravityCenterChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                GravityCenterChart.AddData(0, tempDistance);

                MaxGravityCenter = Math.Max(MaxGravityCenter, tempDistance);
            }

            SetResultDataText(MaxGravityCenter.ToString("0.0000"), 1, 2);

            if (SingleTrainingPlay > 0)
            {
                float LastMaxGravityCenter = 0f;

                for (int i = 0; i < LastTrainingPlay.gravityCenters.Count; i++)
                {
                    tempDistance = 1000 * Vector3.Distance(LastTrainingPlay.gravityCenters[i].Coordinate, LastTrainingPlay.gravityCenters[0].Coordinate);

                    LastMaxGravityCenter = Math.Max(LastMaxGravityCenter, tempDistance);
                }


                SetResultDataText(LastMaxGravityCenter.ToString("0.0000"), 1, 1);

                SetResultDataText(GetEvaluationResult(LastMaxGravityCenter, MaxGravityCenter, 4), 1, 3);
                SetResultDataText(GetEvaluationResult(LastMaxGravityCenter, MaxGravityCenter, 2), 1, 4);
            }
        }

        public void WriteAngleData()
        {
            float MinLeftSideAngle = trainingPlay.angles[0].LeftSideAngle;
            float MinRightSideAngle = trainingPlay.angles[0].RightSideAngle;
            float MinUponSideAngle = trainingPlay.angles[0].UponSideAngle;
            float MinDownSideAngle = trainingPlay.angles[0].DownSideAngle;
            float MinLeftArmAngle = trainingPlay.angles[0].LeftArmAngle;
            float MinRightArmAngle = trainingPlay.angles[0].RightArmAngle;
            float MinLeftElbowAngle = trainingPlay.angles[0].LeftElbowAngle;
            float MinRightElbowAngle = trainingPlay.angles[0].RightElbowAngle;
            float MinLeftLegAngle = trainingPlay.angles[0].LeftLegAngle;
            float MinRightLegAngle = trainingPlay.angles[0].RightLegAngle;
            float MinLeftHipAngle = trainingPlay.angles[0].LeftHipAngle;
            float MinRightHipAngle = trainingPlay.angles[0].RightHipAngle;
            float MinHipAngle = trainingPlay.angles[0].HipAngle;
            float MinLeftKneeAngle = trainingPlay.angles[0].LeftKneeAngle;
            float MinRightKneeAngle = trainingPlay.angles[0].RightKneeAngle;
            float MinLeftAnkleAngle = trainingPlay.angles[0].LeftAnkleAngle;
            float MinRightAnkleAngle = trainingPlay.angles[0].RightAnkleAngle;

            float MaxLeftSideAngle = trainingPlay.angles[0].LeftSideAngle;
            float MaxRightSideAngle = trainingPlay.angles[0].RightSideAngle;
            float MaxUponSideAngle = trainingPlay.angles[0].UponSideAngle;
            float MaxDownSideAngle = trainingPlay.angles[0].DownSideAngle;
            float MaxLeftArmAngle = trainingPlay.angles[0].LeftArmAngle;
            float MaxRightArmAngle = trainingPlay.angles[0].RightArmAngle;
            float MaxLeftElbowAngle = trainingPlay.angles[0].LeftElbowAngle;
            float MaxRightElbowAngle = trainingPlay.angles[0].RightElbowAngle;
            float MaxLeftLegAngle = trainingPlay.angles[0].LeftLegAngle;
            float MaxRightLegAngle = trainingPlay.angles[0].RightLegAngle;
            float MaxLeftHipAngle = trainingPlay.angles[0].LeftHipAngle;
            float MaxRightHipAngle = trainingPlay.angles[0].RightHipAngle;
            float MaxHipAngle = trainingPlay.angles[0].HipAngle;
            float MaxLeftKneeAngle = trainingPlay.angles[0].LeftKneeAngle;
            float MaxRightKneeAngle = trainingPlay.angles[0].RightKneeAngle;
            float MaxLeftAnkleAngle = trainingPlay.angles[0].LeftAnkleAngle;
            float MaxRightAnkleAngle = trainingPlay.angles[0].RightAnkleAngle;

            for (int i = 1; i < trainingPlay.angles.Count; i++)
            {
                MinLeftSideAngle = Math.Min(MinLeftSideAngle, trainingPlay.angles[i].LeftSideAngle);
                MinRightSideAngle = Math.Min(MinRightSideAngle, trainingPlay.angles[i].RightSideAngle);
                MinUponSideAngle = Math.Min(MinUponSideAngle, trainingPlay.angles[i].UponSideAngle);
                MinDownSideAngle = Math.Min(MinDownSideAngle, trainingPlay.angles[i].DownSideAngle);
                MinLeftArmAngle = Math.Min(MinLeftArmAngle, trainingPlay.angles[i].LeftArmAngle);
                MinRightArmAngle = Math.Min(MinRightArmAngle, trainingPlay.angles[i].RightArmAngle);
                MinLeftElbowAngle = Math.Min(MinLeftElbowAngle, trainingPlay.angles[i].LeftElbowAngle);
                MinRightElbowAngle = Math.Min(MinRightElbowAngle, trainingPlay.angles[i].RightElbowAngle);
                MinLeftLegAngle = Math.Min(MinLeftLegAngle, trainingPlay.angles[i].LeftLegAngle);
                MinRightLegAngle = Math.Min(MinRightLegAngle, trainingPlay.angles[i].RightLegAngle);
                MinLeftHipAngle = Math.Min(MinLeftHipAngle, trainingPlay.angles[i].LeftHipAngle);
                MinRightHipAngle = Math.Min(MinRightHipAngle, trainingPlay.angles[i].RightHipAngle);
                MinHipAngle = Math.Min(MinHipAngle, trainingPlay.angles[i].HipAngle);
                MinLeftKneeAngle = Math.Min(MinLeftKneeAngle, trainingPlay.angles[i].LeftKneeAngle);
                MinRightKneeAngle = Math.Min(MinRightKneeAngle, trainingPlay.angles[i].RightKneeAngle);
                MinLeftAnkleAngle = Math.Min(MinLeftAnkleAngle, trainingPlay.angles[i].LeftAnkleAngle);
                MinRightAnkleAngle = Math.Min(MinRightAnkleAngle, trainingPlay.angles[i].RightAnkleAngle);

                MaxLeftSideAngle = Math.Max(MaxLeftSideAngle, trainingPlay.angles[i].LeftSideAngle);
                MaxRightSideAngle = Math.Max(MaxRightSideAngle, trainingPlay.angles[i].RightSideAngle);
                MaxUponSideAngle = Math.Max(MaxUponSideAngle, trainingPlay.angles[i].UponSideAngle);
                MaxDownSideAngle = Math.Max(MaxDownSideAngle, trainingPlay.angles[i].DownSideAngle);
                MaxLeftArmAngle = Math.Max(MaxLeftArmAngle, trainingPlay.angles[i].LeftArmAngle);
                MaxRightArmAngle = Math.Max(MaxRightArmAngle, trainingPlay.angles[i].RightArmAngle);
                MaxLeftElbowAngle = Math.Max(MaxLeftElbowAngle, trainingPlay.angles[i].LeftElbowAngle);
                MaxRightElbowAngle = Math.Max(MaxRightElbowAngle, trainingPlay.angles[i].RightElbowAngle);
                MaxLeftLegAngle = Math.Max(MaxLeftLegAngle, trainingPlay.angles[i].LeftLegAngle);
                MaxRightLegAngle = Math.Max(MaxRightLegAngle, trainingPlay.angles[i].RightLegAngle);
                MaxLeftHipAngle = Math.Max(MaxLeftHipAngle, trainingPlay.angles[i].LeftHipAngle);
                MaxRightHipAngle = Math.Max(MaxRightHipAngle, trainingPlay.angles[i].RightHipAngle);
                MaxHipAngle = Math.Max(MaxHipAngle, trainingPlay.angles[i].HipAngle);
                MaxLeftKneeAngle = Math.Max(MaxLeftKneeAngle, trainingPlay.angles[i].LeftKneeAngle);
                MaxRightKneeAngle = Math.Max(MaxRightKneeAngle, trainingPlay.angles[i].RightKneeAngle);
                MaxLeftAnkleAngle = Math.Max(MaxLeftAnkleAngle, trainingPlay.angles[i].LeftAnkleAngle);
                MaxRightAnkleAngle = Math.Max(MaxRightAnkleAngle, trainingPlay.angles[i].RightAnkleAngle);
            }

            SetResultDataText(MinLeftSideAngle.ToString("0.00") + " | " + MaxLeftSideAngle.ToString("0.00"), 2, 2);
            SetResultDataText(MinRightSideAngle.ToString("0.00") + " | " + MaxRightSideAngle.ToString("0.00"), 3, 2);
            SetResultDataText(MinUponSideAngle.ToString("0.00") + " | " + MaxUponSideAngle.ToString("0.00"), 4, 2);
            SetResultDataText(MinDownSideAngle.ToString("0.00") + " | " + MaxDownSideAngle.ToString("0.00"), 5, 2);
            SetResultDataText(MinLeftArmAngle.ToString("0.00") + " | " + MaxLeftArmAngle.ToString("0.00"), 6, 2);
            SetResultDataText(MinRightArmAngle.ToString("0.00") + " | " + MaxRightArmAngle.ToString("0.00"), 7, 2);
            SetResultDataText(MinLeftElbowAngle.ToString("0.00") + " | " + MaxLeftElbowAngle.ToString("0.00"), 8, 2);
            SetResultDataText(MinRightElbowAngle.ToString("0.00") + " | " + MaxRightElbowAngle.ToString("0.00"), 9, 2);
            SetResultDataText(MinLeftLegAngle.ToString("0.00") + " | " + MaxLeftLegAngle.ToString("0.00"), 10, 2);
            SetResultDataText(MinRightLegAngle.ToString("0.00") + " | " + MaxRightLegAngle.ToString("0.00"), 11, 2);
            SetResultDataText(MinLeftHipAngle.ToString("0.00") + " | " + MaxLeftHipAngle.ToString("0.00"), 12, 2);
            SetResultDataText(MinRightHipAngle.ToString("0.00") + " | " + MaxRightHipAngle.ToString("0.00"), 13, 2);
            SetResultDataText(MinHipAngle.ToString("0.00") + " | " + MaxHipAngle.ToString("0.00"), 14, 2);
            SetResultDataText(MinLeftKneeAngle.ToString("0.00") + " | " + MaxLeftKneeAngle.ToString("0.00"), 15, 2);
            SetResultDataText(MinRightKneeAngle.ToString("0.00") + " | " + MaxRightKneeAngle.ToString("0.00"), 16, 2);
            SetResultDataText(MinLeftAnkleAngle.ToString("0.00") + " | " + MaxLeftAnkleAngle.ToString("0.00"), 17, 2);
            SetResultDataText(MinRightAnkleAngle.ToString("0.00") + " | " + MaxRightAnkleAngle.ToString("0.00"), 18, 2);

            if(SingleTrainingPlay > 0)
            {
                float LastMinLeftSideAngle = LastTrainingPlay.angles[0].LeftSideAngle;
                float LastMinRightSideAngle = LastTrainingPlay.angles[0].RightSideAngle;
                float LastMinUponSideAngle = LastTrainingPlay.angles[0].UponSideAngle;
                float LastMinDownSideAngle = LastTrainingPlay.angles[0].DownSideAngle;
                float LastMinLeftArmAngle = LastTrainingPlay.angles[0].LeftArmAngle;
                float LastMinRightArmAngle = LastTrainingPlay.angles[0].RightArmAngle;
                float LastMinLeftElbowAngle = LastTrainingPlay.angles[0].LeftElbowAngle;
                float LastMinRightElbowAngle = LastTrainingPlay.angles[0].RightElbowAngle;
                float LastMinLeftLegAngle = LastTrainingPlay.angles[0].LeftLegAngle;
                float LastMinRightLegAngle = LastTrainingPlay.angles[0].RightLegAngle;
                float LastMinLeftHipAngle = LastTrainingPlay.angles[0].LeftHipAngle;
                float LastMinRightHipAngle = LastTrainingPlay.angles[0].RightHipAngle;
                float LastMinHipAngle = LastTrainingPlay.angles[0].HipAngle;
                float LastMinLeftKneeAngle = LastTrainingPlay.angles[0].LeftKneeAngle;
                float LastMinRightKneeAngle = LastTrainingPlay.angles[0].RightKneeAngle;
                float LastMinLeftAnkleAngle = LastTrainingPlay.angles[0].LeftAnkleAngle;
                float LastMinRightAnkleAngle = LastTrainingPlay.angles[0].RightAnkleAngle;
                      
                float LastMaxLeftSideAngle = LastTrainingPlay.angles[0].LeftSideAngle;
                float LastMaxRightSideAngle = LastTrainingPlay.angles[0].RightSideAngle;
                float LastMaxUponSideAngle = LastTrainingPlay.angles[0].UponSideAngle;
                float LastMaxDownSideAngle = LastTrainingPlay.angles[0].DownSideAngle;
                float LastMaxLeftArmAngle = LastTrainingPlay.angles[0].LeftArmAngle;
                float LastMaxRightArmAngle = LastTrainingPlay.angles[0].RightArmAngle;
                float LastMaxLeftElbowAngle = LastTrainingPlay.angles[0].LeftElbowAngle;
                float LastMaxRightElbowAngle = LastTrainingPlay.angles[0].RightElbowAngle;
                float LastMaxLeftLegAngle = LastTrainingPlay.angles[0].LeftLegAngle;
                float LastMaxRightLegAngle = LastTrainingPlay.angles[0].RightLegAngle;
                float LastMaxLeftHipAngle = LastTrainingPlay.angles[0].LeftHipAngle;
                float LastMaxRightHipAngle = LastTrainingPlay.angles[0].RightHipAngle;
                float LastMaxHipAngle = LastTrainingPlay.angles[0].HipAngle;
                float LastMaxLeftKneeAngle = LastTrainingPlay.angles[0].LeftKneeAngle;
                float LastMaxRightKneeAngle = LastTrainingPlay.angles[0].RightKneeAngle;
                float LastMaxLeftAnkleAngle = LastTrainingPlay.angles[0].LeftAnkleAngle;
                float LastMaxRightAnkleAngle = LastTrainingPlay.angles[0].RightAnkleAngle;

                for (int i = 1; i < LastTrainingPlay.angles.Count; i++)
                {
                    MinLeftSideAngle = Math.Min(MinLeftSideAngle, LastTrainingPlay.angles[i].LeftSideAngle);
                    MinRightSideAngle = Math.Min(MinRightSideAngle, LastTrainingPlay.angles[i].RightSideAngle);
                    MinUponSideAngle = Math.Min(MinUponSideAngle, LastTrainingPlay.angles[i].UponSideAngle);
                    MinDownSideAngle = Math.Min(MinDownSideAngle, LastTrainingPlay.angles[i].DownSideAngle);
                    MinLeftArmAngle = Math.Min(MinLeftArmAngle, LastTrainingPlay.angles[i].LeftArmAngle);
                    MinRightArmAngle = Math.Min(MinRightArmAngle, LastTrainingPlay.angles[i].RightArmAngle);
                    MinLeftElbowAngle = Math.Min(MinLeftElbowAngle, LastTrainingPlay.angles[i].LeftElbowAngle);
                    MinRightElbowAngle = Math.Min(MinRightElbowAngle, LastTrainingPlay.angles[i].RightElbowAngle);
                    MinLeftLegAngle = Math.Min(MinLeftLegAngle, LastTrainingPlay.angles[i].LeftLegAngle);
                    MinRightLegAngle = Math.Min(MinRightLegAngle, LastTrainingPlay.angles[i].RightLegAngle);
                    MinLeftHipAngle = Math.Min(MinLeftHipAngle, LastTrainingPlay.angles[i].LeftHipAngle);
                    MinRightHipAngle = Math.Min(MinRightHipAngle, LastTrainingPlay.angles[i].RightHipAngle);
                    MinHipAngle = Math.Min(MinHipAngle, LastTrainingPlay.angles[i].HipAngle);
                    MinLeftKneeAngle = Math.Min(MinLeftKneeAngle, LastTrainingPlay.angles[i].LeftKneeAngle);
                    MinRightKneeAngle = Math.Min(MinRightKneeAngle, LastTrainingPlay.angles[i].RightKneeAngle);
                    MinLeftAnkleAngle = Math.Min(MinLeftAnkleAngle, LastTrainingPlay.angles[i].LeftAnkleAngle);
                    MinRightAnkleAngle = Math.Min(MinRightAnkleAngle, LastTrainingPlay.angles[i].RightAnkleAngle);

                    MaxLeftSideAngle = Math.Max(MaxLeftSideAngle, LastTrainingPlay.angles[i].LeftSideAngle);
                    MaxRightSideAngle = Math.Max(MaxRightSideAngle, LastTrainingPlay.angles[i].RightSideAngle);
                    MaxUponSideAngle = Math.Max(MaxUponSideAngle, LastTrainingPlay.angles[i].UponSideAngle);
                    MaxDownSideAngle = Math.Max(MaxDownSideAngle, LastTrainingPlay.angles[i].DownSideAngle);
                    MaxLeftArmAngle = Math.Max(MaxLeftArmAngle, LastTrainingPlay.angles[i].LeftArmAngle);
                    MaxRightArmAngle = Math.Max(MaxRightArmAngle, LastTrainingPlay.angles[i].RightArmAngle);
                    MaxLeftElbowAngle = Math.Max(MaxLeftElbowAngle, LastTrainingPlay.angles[i].LeftElbowAngle);
                    MaxRightElbowAngle = Math.Max(MaxRightElbowAngle, LastTrainingPlay.angles[i].RightElbowAngle);
                    MaxLeftLegAngle = Math.Max(MaxLeftLegAngle, LastTrainingPlay.angles[i].LeftLegAngle);
                    MaxRightLegAngle = Math.Max(MaxRightLegAngle, LastTrainingPlay.angles[i].RightLegAngle);
                    MaxLeftHipAngle = Math.Max(MaxLeftHipAngle, LastTrainingPlay.angles[i].LeftHipAngle);
                    MaxRightHipAngle = Math.Max(MaxRightHipAngle, LastTrainingPlay.angles[i].RightHipAngle);
                    MaxHipAngle = Math.Max(MaxHipAngle, LastTrainingPlay.angles[i].HipAngle);
                    MaxLeftKneeAngle = Math.Max(MaxLeftKneeAngle, LastTrainingPlay.angles[i].LeftKneeAngle);
                    MaxRightKneeAngle = Math.Max(MaxRightKneeAngle, LastTrainingPlay.angles[i].RightKneeAngle);
                    MaxLeftAnkleAngle = Math.Max(MaxLeftAnkleAngle, LastTrainingPlay.angles[i].LeftAnkleAngle);
                    MaxRightAnkleAngle = Math.Max(MaxRightAnkleAngle, LastTrainingPlay.angles[i].RightAnkleAngle);
                }

                SetResultDataText(LastMinLeftSideAngle.ToString("0.00") + " | " + LastMaxLeftSideAngle.ToString("0.00"), 2, 1);
                SetResultDataText(LastMinRightSideAngle.ToString("0.00") + " | " + LastMaxRightSideAngle.ToString("0.00"), 3, 1);
                SetResultDataText(LastMinUponSideAngle.ToString("0.00") + " | " + LastMaxUponSideAngle.ToString("0.00"), 4, 1);
                SetResultDataText(LastMinDownSideAngle.ToString("0.00") + " | " + LastMaxDownSideAngle.ToString("0.00"), 5, 1);
                SetResultDataText(LastMinLeftArmAngle.ToString("0.00") + " | " + LastMaxLeftArmAngle.ToString("0.00"), 6, 1);
                SetResultDataText(LastMinRightArmAngle.ToString("0.00") + " | " + LastMaxRightArmAngle.ToString("0.00"), 7, 1);
                SetResultDataText(LastMinLeftElbowAngle.ToString("0.00") + " | " + LastMaxLeftElbowAngle.ToString("0.00"), 8, 1);
                SetResultDataText(LastMinRightElbowAngle.ToString("0.00") + " | " + LastMaxRightElbowAngle.ToString("0.00"), 9, 1);
                SetResultDataText(LastMinLeftLegAngle.ToString("0.00") + " | " + LastMaxLeftLegAngle.ToString("0.00"), 10, 1);
                SetResultDataText(LastMinRightLegAngle.ToString("0.00") + " | " + LastMaxRightLegAngle.ToString("0.00"), 11, 1);
                SetResultDataText(LastMinLeftHipAngle.ToString("0.00") + " | " + LastMaxLeftHipAngle.ToString("0.00"), 12, 1);
                SetResultDataText(LastMinRightHipAngle.ToString("0.00") + " | " + LastMaxRightHipAngle.ToString("0.00"), 13, 1);
                SetResultDataText(LastMinHipAngle.ToString("0.00") + " | " + LastMaxHipAngle.ToString("0.00"), 14, 1);
                SetResultDataText(LastMinLeftKneeAngle.ToString("0.00") + " | " + LastMaxLeftKneeAngle.ToString("0.00"), 15, 1);
                SetResultDataText(LastMinRightKneeAngle.ToString("0.00") + " | " + LastMaxRightKneeAngle.ToString("0.00"), 16, 1);
                SetResultDataText(LastMinLeftAnkleAngle.ToString("0.00") + " | " + LastMaxLeftAnkleAngle.ToString("0.00"), 17, 1);
                SetResultDataText(LastMinRightAnkleAngle.ToString("0.00") + " | " + LastMaxRightAnkleAngle.ToString("0.00"), 18, 1);

                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftSideAngle, LastMaxLeftSideAngle), new Vector2(MinLeftSideAngle, MaxLeftSideAngle), 4), 2, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightSideAngle,LastMaxRightSideAngle), new Vector2(MinRightSideAngle, MaxRightSideAngle), 4), 3, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinUponSideAngle,LastMaxUponSideAngle), new Vector2(MinUponSideAngle, MaxUponSideAngle), 4), 4, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinDownSideAngle,LastMaxDownSideAngle), new Vector2(MinDownSideAngle, MaxDownSideAngle), 4), 5, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftArmAngle,LastMaxLeftArmAngle), new Vector2(MinLeftArmAngle, MaxLeftArmAngle), 4), 6, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightArmAngle,LastMaxRightArmAngle), new Vector2(MinRightArmAngle, MaxRightArmAngle), 4), 7, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftElbowAngle,LastMaxLeftElbowAngle), new Vector2(MinLeftElbowAngle, MaxLeftElbowAngle), 4), 8, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightElbowAngle,LastMaxRightElbowAngle), new Vector2(MinRightElbowAngle, MaxRightElbowAngle), 4), 9, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftLegAngle,LastMaxLeftLegAngle), new Vector2(MinLeftLegAngle, MaxLeftLegAngle), 4), 10, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightLegAngle,LastMaxRightLegAngle), new Vector2(MinRightLegAngle, MaxRightLegAngle), 4), 11, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftHipAngle,LastMaxLeftHipAngle), new Vector2(MinLeftHipAngle, MaxLeftHipAngle), 4), 12, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightHipAngle,LastMaxRightHipAngle), new Vector2(MinRightHipAngle, MaxRightHipAngle), 4), 13, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinHipAngle,LastMaxHipAngle), new Vector2(MinHipAngle, MaxHipAngle), 4), 14, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftKneeAngle,LastMaxLeftKneeAngle), new Vector2(MinLeftKneeAngle, MaxLeftKneeAngle), 4), 15, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightKneeAngle,LastMaxRightKneeAngle), new Vector2(MinRightKneeAngle, MaxRightKneeAngle), 4), 16, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftAnkleAngle,LastMaxLeftAnkleAngle), new Vector2(MinLeftAnkleAngle, MaxLeftAnkleAngle), 4), 17, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightAnkleAngle, LastMaxRightAnkleAngle), new Vector2(MinRightAnkleAngle, MaxRightAnkleAngle), 4), 18, 3);

                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftSideAngle, LastMaxLeftSideAngle), new Vector2(MinLeftSideAngle, MaxLeftSideAngle), 2), 2, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightSideAngle, LastMaxRightSideAngle), new Vector2(MinRightSideAngle, MaxRightSideAngle), 2), 3, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinUponSideAngle, LastMaxUponSideAngle), new Vector2(MinUponSideAngle, MaxUponSideAngle), 2), 4, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinDownSideAngle, LastMaxDownSideAngle), new Vector2(MinDownSideAngle, MaxDownSideAngle), 2), 5, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftArmAngle, LastMaxLeftArmAngle), new Vector2(MinLeftArmAngle, MaxLeftArmAngle), 2), 6, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightArmAngle, LastMaxRightArmAngle), new Vector2(MinRightArmAngle, MaxRightArmAngle), 2), 7, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftElbowAngle, LastMaxLeftElbowAngle), new Vector2(MinLeftElbowAngle, MaxLeftElbowAngle), 2), 8, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightElbowAngle, LastMaxRightElbowAngle), new Vector2(MinRightElbowAngle, MaxRightElbowAngle), 2), 9, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftLegAngle, LastMaxLeftLegAngle), new Vector2(MinLeftLegAngle, MaxLeftLegAngle), 2), 10, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightLegAngle, LastMaxRightLegAngle), new Vector2(MinRightLegAngle, MaxRightLegAngle), 2), 11, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftHipAngle, LastMaxLeftHipAngle), new Vector2(MinLeftHipAngle, MaxLeftHipAngle), 2), 12, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightHipAngle, LastMaxRightHipAngle), new Vector2(MinRightHipAngle, MaxRightHipAngle), 2), 13, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinHipAngle, LastMaxHipAngle), new Vector2(MinHipAngle, MaxHipAngle), 2), 14, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftKneeAngle, LastMaxLeftKneeAngle), new Vector2(MinLeftKneeAngle, MaxLeftKneeAngle), 2), 15, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightKneeAngle, LastMaxRightKneeAngle), new Vector2(MinRightKneeAngle, MaxRightKneeAngle), 2), 16, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftAnkleAngle, LastMaxLeftAnkleAngle), new Vector2(MinLeftAnkleAngle, MaxLeftAnkleAngle), 2), 17, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightAnkleAngle, LastMaxRightAnkleAngle), new Vector2(MinRightAnkleAngle, MaxRightAnkleAngle), 2), 18, 4);
            }
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
                ResultString = (Diff).ToString("0.000");
            }
            return ResultString;
        }

        void Update()
        {

        }
    }
}