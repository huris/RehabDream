using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Vectrosity;
using System.Collections;
using System.Linq;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class FishTrainingPrintScript : MonoBehaviour
    {

        public int SingleTrainingPlay;
        public FishTrainingPlay fishTrainingPlay;
        public int LastTrainingPlay;
        public FishTrainingPlay LastFishTrainingPlay;

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
        public Text TrainingScore;
        public Text TrainingRank;
        public long RealityTrainingDuration;
        public Text TrainingDuration;
        public Text TrainingDirection;
        public List<string> DirectionText = new List<string> { "两侧一致", "左侧重点", "右侧重点" };
        public Text TrainingSuccessRate;
        public Text TrainingTime;

        // Chart
        public RadarChart DirectionRadarChart;
        public LineChart GCAnglesChart;


        public Dropdown FirstItem;
        public Dropdown SecondItem;

        void OnEnable()
        {
            if (DoctorDataManager.instance.doctor.patient.FishTrainingPlays == null)
            {
                DoctorDataManager.instance.doctor.patient.FishTrainingPlays = DoctorDatabaseManager.instance.ReadPatientFishTrainings(DoctorDataManager.instance.doctor.patient.PatientID);
                if (DoctorDataManager.instance.doctor.patient.FishTrainingPlays != null && DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count > 0)
                {
                    DoctorDataManager.instance.doctor.patient.SetFishTrainingPlayIndex(DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count - 1);
                }
            }

            if (DoctorDataManager.instance.doctor.patient.FishTrainingPlays != null && DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count > 0)
            {
                SingleTrainingPlay = SecondItem.value;
                fishTrainingPlay = DoctorDataManager.instance.doctor.patient.FishTrainingPlays[SingleTrainingPlay];
                LastTrainingPlay = FirstItem.value;
                LastFishTrainingPlay = DoctorDataManager.instance.doctor.patient.FishTrainingPlays[LastTrainingPlay];

                // Title
                string PatientNameBlock = "";
                for (int z = 0; z < DoctorDataManager.instance.doctor.patient.PatientName.Length; z++)
                {
                    PatientNameBlock += DoctorDataManager.instance.doctor.patient.PatientName[z] + "  ";
                }
                TrainingTitle = transform.Find("TrainingTitle").GetComponent<Text>();
                TrainingTitle.text = PatientNameBlock + "第  " + (SingleTrainingPlay + 1).ToString() + "  次  重  心  捕  鱼  训  练  报  告  表";

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
                TrainingScore = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Score/TrainingScore").GetComponent<Text>();
                TrainingScore.text = fishTrainingPlay.TrainingScore.ToString();

                TrainingRank = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Rank/TrainingRank").GetComponent<Text>();

                float TrainingEvaluationRate = fishTrainingPlay.TrainingScore / (
                    fishTrainingPlay.StaticFishAllCount * 100 +
                    fishTrainingPlay.DynamicFishAllCount * 150);

                if (TrainingEvaluationRate >= 0.95f) { TrainingRank.text = "S 级"; }
                else if (TrainingEvaluationRate >= 0.85f) { TrainingRank.text = "A 级"; }
                else if (TrainingEvaluationRate >= 0.75f) { TrainingRank.text = "B 级"; }
                else if (TrainingEvaluationRate >= 0.65f) { TrainingRank.text = "C 级"; }
                else if (TrainingEvaluationRate >= 0.55f) { TrainingRank.text = "D 级"; }
                else { TrainingRank.text = "E 级"; }

                TrainingDuration = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Duration/TrainingDuration").GetComponent<Text>();
                RealityTrainingDuration = long.Parse(fishTrainingPlay.TrainingEndTime.Substring(9, 2)) * 3600 + long.Parse(fishTrainingPlay.TrainingEndTime.Substring(12, 2)) * 60 + long.Parse(fishTrainingPlay.TrainingEndTime.Substring(15, 2))
                                           - long.Parse(fishTrainingPlay.TrainingStartTime.Substring(9, 2)) * 3600 - long.Parse(fishTrainingPlay.TrainingStartTime.Substring(12, 2)) * 60 - long.Parse(fishTrainingPlay.TrainingStartTime.Substring(15, 2));
                TrainingDuration.text = RealityTrainingDuration.ToString() + "秒";

                TrainingDirection = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Direction/TrainingDirection").GetComponent<Text>();
                TrainingDirection.text = DirectionText[(int)fishTrainingPlay.TrainingDirection];

                TrainingSuccessRate = transform.Find("TrainingFeedback/TrainingFeedbackInfo/SuccessRate/TrainingSuccessRate").GetComponent<Text>();
                TrainingSuccessRate.text = (fishTrainingPlay.StaticFishSuccessCount + fishTrainingPlay.DynamicFishSuccessCount).ToString() + "/" 
                    + (fishTrainingPlay.StaticFishAllCount + fishTrainingPlay.DynamicFishAllCount).ToString();

                TrainingTime = transform.Find("TrainingFeedback/TrainingFeedbackInfo/Time/TrainingTime").GetComponent<Text>();
                TrainingTime.text = fishTrainingPlay.TrainingStartTime;

                // Chart
                DrawRadarChart();
                DrawGravityCenterOffset();


                //    // 初始化对比结果
                //    for (int m = 0; m < 19; m++)
                //    {
                //        for (int n = 1; n < 5; n++)
                //        {
                //            SetResultDataText("-", m, n);
                //        }
                //    }

                //    DrawRadarChart();
                //    WriteAngleData();

            }

        }

        public void DrawRadarChart()
        {
            DirectionRadarChart = transform.Find("Chart/RadarChart").GetComponent<RadarChart>();
            if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("Chart/RadarChart").gameObject.AddComponent<RadarChart>();

            DirectionRadarChart.UpdateData(0, 0, 0, Mathf.Min(1f, 1.0f * RealityTrainingDuration / fishTrainingPlay.PlanDuration));
            DirectionRadarChart.UpdateData(0, 0, 1, Mathf.Min(1f, 5.0f / fishTrainingPlay.FishCaptureTime.Average()));
            DirectionRadarChart.UpdateData(0, 0, 2, Mathf.Min(1f, 1.0f * fishTrainingPlay.Distance / (RealityTrainingDuration * 40)));
            long HistoryCaptureCount = 0;
            long HistoryAllCount = 0;
            for (int i = 0; i < DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Count; i++)
            {
                HistoryCaptureCount += DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].StaticFishSuccessCount + DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].DynamicFishSuccessCount;
                HistoryAllCount += DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].StaticFishAllCount + DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].DynamicFishAllCount;
            }
            DirectionRadarChart.UpdateData(0, 0, 3, 1.0f * HistoryCaptureCount / HistoryAllCount);
            DirectionRadarChart.UpdateData(0, 0, 4, 1.0f * (fishTrainingPlay.StaticFishSuccessCount + fishTrainingPlay.DynamicFishSuccessCount) / (
                fishTrainingPlay.StaticFishAllCount + fishTrainingPlay.DynamicFishAllCount));
            DirectionRadarChart.RefreshChart();
        }

        public void DrawGravityCenterOffset()
        {
            while (GCAnglesChart.series.list[0].data.Count > fishTrainingPlay.GCAngles.Count)
            {

                GCAnglesChart.series.list[0].data.RemoveAt(GCAnglesChart.series.list[0].data.Count - 1);
                GCAnglesChart.xAxis0.data.RemoveAt(GCAnglesChart.xAxis0.data.Count - 1);
            }

            while (GCAnglesChart.series.list[0].data.Count < fishTrainingPlay.GCAngles.Count)
            {
                GCAnglesChart.series.list[0].AddYData(0f);
                GCAnglesChart.xAxis0.data.Add("A" + (GCAnglesChart.xAxis0.data.Count + 1).ToString());
            }

            for (int i = 0; i < fishTrainingPlay.GCAngles.Count; i++)
            {
                GCAnglesChart.series.UpdateData(0, i, fishTrainingPlay.GCAngles[i]);
            }

            GCAnglesChart.title.text = "整体平均" + fishTrainingPlay.GCAngles.Average().ToString("0.00") + "度, 最大偏移"
                + fishTrainingPlay.GCAngles.Max().ToString("0.00") + "度";

            GCAnglesChart.RefreshChart();
        }

        public void WriteAngleData()
        {
            //float MinLeftSideAngle = trainingPlay.angles[0].LeftSideAngle;
            //float MinRightSideAngle = trainingPlay.angles[0].RightSideAngle;
            //float MinUponSideAngle = trainingPlay.angles[0].UponSideAngle;
            //float MinDownSideAngle = trainingPlay.angles[0].DownSideAngle;
            //float MinLeftArmAngle = trainingPlay.angles[0].LeftArmAngle;
            //float MinRightArmAngle = trainingPlay.angles[0].RightArmAngle;
            //float MinLeftElbowAngle = trainingPlay.angles[0].LeftElbowAngle;
            //float MinRightElbowAngle = trainingPlay.angles[0].RightElbowAngle;
            //float MinLeftLegAngle = trainingPlay.angles[0].LeftLegAngle;
            //float MinRightLegAngle = trainingPlay.angles[0].RightLegAngle;
            //float MinLeftHipAngle = trainingPlay.angles[0].LeftHipAngle;
            //float MinRightHipAngle = trainingPlay.angles[0].RightHipAngle;
            //float MinHipAngle = trainingPlay.angles[0].HipAngle;
            //float MinLeftKneeAngle = trainingPlay.angles[0].LeftKneeAngle;
            //float MinRightKneeAngle = trainingPlay.angles[0].RightKneeAngle;
            //float MinLeftAnkleAngle = trainingPlay.angles[0].LeftAnkleAngle;
            //float MinRightAnkleAngle = trainingPlay.angles[0].RightAnkleAngle;

            //float MaxLeftSideAngle = trainingPlay.angles[0].LeftSideAngle;
            //float MaxRightSideAngle = trainingPlay.angles[0].RightSideAngle;
            //float MaxUponSideAngle = trainingPlay.angles[0].UponSideAngle;
            //float MaxDownSideAngle = trainingPlay.angles[0].DownSideAngle;
            //float MaxLeftArmAngle = trainingPlay.angles[0].LeftArmAngle;
            //float MaxRightArmAngle = trainingPlay.angles[0].RightArmAngle;
            //float MaxLeftElbowAngle = trainingPlay.angles[0].LeftElbowAngle;
            //float MaxRightElbowAngle = trainingPlay.angles[0].RightElbowAngle;
            //float MaxLeftLegAngle = trainingPlay.angles[0].LeftLegAngle;
            //float MaxRightLegAngle = trainingPlay.angles[0].RightLegAngle;
            //float MaxLeftHipAngle = trainingPlay.angles[0].LeftHipAngle;
            //float MaxRightHipAngle = trainingPlay.angles[0].RightHipAngle;
            //float MaxHipAngle = trainingPlay.angles[0].HipAngle;
            //float MaxLeftKneeAngle = trainingPlay.angles[0].LeftKneeAngle;
            //float MaxRightKneeAngle = trainingPlay.angles[0].RightKneeAngle;
            //float MaxLeftAnkleAngle = trainingPlay.angles[0].LeftAnkleAngle;
            //float MaxRightAnkleAngle = trainingPlay.angles[0].RightAnkleAngle;

            //for (int i = 1; i < trainingPlay.angles.Count; i++)
            //{
            //    MinLeftSideAngle = Math.Min(MinLeftSideAngle, trainingPlay.angles[i].LeftSideAngle);
            //    MinRightSideAngle = Math.Min(MinRightSideAngle, trainingPlay.angles[i].RightSideAngle);
            //    MinUponSideAngle = Math.Min(MinUponSideAngle, trainingPlay.angles[i].UponSideAngle);
            //    MinDownSideAngle = Math.Min(MinDownSideAngle, trainingPlay.angles[i].DownSideAngle);
            //    MinLeftArmAngle = Math.Min(MinLeftArmAngle, trainingPlay.angles[i].LeftArmAngle);
            //    MinRightArmAngle = Math.Min(MinRightArmAngle, trainingPlay.angles[i].RightArmAngle);
            //    MinLeftElbowAngle = Math.Min(MinLeftElbowAngle, trainingPlay.angles[i].LeftElbowAngle);
            //    MinRightElbowAngle = Math.Min(MinRightElbowAngle, trainingPlay.angles[i].RightElbowAngle);
            //    MinLeftLegAngle = Math.Min(MinLeftLegAngle, trainingPlay.angles[i].LeftLegAngle);
            //    MinRightLegAngle = Math.Min(MinRightLegAngle, trainingPlay.angles[i].RightLegAngle);
            //    MinLeftHipAngle = Math.Min(MinLeftHipAngle, trainingPlay.angles[i].LeftHipAngle);
            //    MinRightHipAngle = Math.Min(MinRightHipAngle, trainingPlay.angles[i].RightHipAngle);
            //    MinHipAngle = Math.Min(MinHipAngle, trainingPlay.angles[i].HipAngle);
            //    MinLeftKneeAngle = Math.Min(MinLeftKneeAngle, trainingPlay.angles[i].LeftKneeAngle);
            //    MinRightKneeAngle = Math.Min(MinRightKneeAngle, trainingPlay.angles[i].RightKneeAngle);
            //    MinLeftAnkleAngle = Math.Min(MinLeftAnkleAngle, trainingPlay.angles[i].LeftAnkleAngle);
            //    MinRightAnkleAngle = Math.Min(MinRightAnkleAngle, trainingPlay.angles[i].RightAnkleAngle);

            //    MaxLeftSideAngle = Math.Max(MaxLeftSideAngle, trainingPlay.angles[i].LeftSideAngle);
            //    MaxRightSideAngle = Math.Max(MaxRightSideAngle, trainingPlay.angles[i].RightSideAngle);
            //    MaxUponSideAngle = Math.Max(MaxUponSideAngle, trainingPlay.angles[i].UponSideAngle);
            //    MaxDownSideAngle = Math.Max(MaxDownSideAngle, trainingPlay.angles[i].DownSideAngle);
            //    MaxLeftArmAngle = Math.Max(MaxLeftArmAngle, trainingPlay.angles[i].LeftArmAngle);
            //    MaxRightArmAngle = Math.Max(MaxRightArmAngle, trainingPlay.angles[i].RightArmAngle);
            //    MaxLeftElbowAngle = Math.Max(MaxLeftElbowAngle, trainingPlay.angles[i].LeftElbowAngle);
            //    MaxRightElbowAngle = Math.Max(MaxRightElbowAngle, trainingPlay.angles[i].RightElbowAngle);
            //    MaxLeftLegAngle = Math.Max(MaxLeftLegAngle, trainingPlay.angles[i].LeftLegAngle);
            //    MaxRightLegAngle = Math.Max(MaxRightLegAngle, trainingPlay.angles[i].RightLegAngle);
            //    MaxLeftHipAngle = Math.Max(MaxLeftHipAngle, trainingPlay.angles[i].LeftHipAngle);
            //    MaxRightHipAngle = Math.Max(MaxRightHipAngle, trainingPlay.angles[i].RightHipAngle);
            //    MaxHipAngle = Math.Max(MaxHipAngle, trainingPlay.angles[i].HipAngle);
            //    MaxLeftKneeAngle = Math.Max(MaxLeftKneeAngle, trainingPlay.angles[i].LeftKneeAngle);
            //    MaxRightKneeAngle = Math.Max(MaxRightKneeAngle, trainingPlay.angles[i].RightKneeAngle);
            //    MaxLeftAnkleAngle = Math.Max(MaxLeftAnkleAngle, trainingPlay.angles[i].LeftAnkleAngle);
            //    MaxRightAnkleAngle = Math.Max(MaxRightAnkleAngle, trainingPlay.angles[i].RightAnkleAngle);
            //}

            //SetResultDataText(MinLeftSideAngle.ToString("0.00") + " | " + MaxLeftSideAngle.ToString("0.00"), 2, 2);
            //SetResultDataText(MinRightSideAngle.ToString("0.00") + " | " + MaxRightSideAngle.ToString("0.00"), 3, 2);
            //SetResultDataText(MinUponSideAngle.ToString("0.00") + " | " + MaxUponSideAngle.ToString("0.00"), 4, 2);
            //SetResultDataText(MinDownSideAngle.ToString("0.00") + " | " + MaxDownSideAngle.ToString("0.00"), 5, 2);
            //SetResultDataText(MinLeftArmAngle.ToString("0.00") + " | " + MaxLeftArmAngle.ToString("0.00"), 6, 2);
            //SetResultDataText(MinRightArmAngle.ToString("0.00") + " | " + MaxRightArmAngle.ToString("0.00"), 7, 2);
            //SetResultDataText(MinLeftElbowAngle.ToString("0.00") + " | " + MaxLeftElbowAngle.ToString("0.00"), 8, 2);
            //SetResultDataText(MinRightElbowAngle.ToString("0.00") + " | " + MaxRightElbowAngle.ToString("0.00"), 9, 2);
            //SetResultDataText(MinLeftLegAngle.ToString("0.00") + " | " + MaxLeftLegAngle.ToString("0.00"), 10, 2);
            //SetResultDataText(MinRightLegAngle.ToString("0.00") + " | " + MaxRightLegAngle.ToString("0.00"), 11, 2);
            //SetResultDataText(MinLeftHipAngle.ToString("0.00") + " | " + MaxLeftHipAngle.ToString("0.00"), 12, 2);
            //SetResultDataText(MinRightHipAngle.ToString("0.00") + " | " + MaxRightHipAngle.ToString("0.00"), 13, 2);
            //SetResultDataText(MinHipAngle.ToString("0.00") + " | " + MaxHipAngle.ToString("0.00"), 14, 2);
            //SetResultDataText(MinLeftKneeAngle.ToString("0.00") + " | " + MaxLeftKneeAngle.ToString("0.00"), 15, 2);
            //SetResultDataText(MinRightKneeAngle.ToString("0.00") + " | " + MaxRightKneeAngle.ToString("0.00"), 16, 2);
            //SetResultDataText(MinLeftAnkleAngle.ToString("0.00") + " | " + MaxLeftAnkleAngle.ToString("0.00"), 17, 2);
            //SetResultDataText(MinRightAnkleAngle.ToString("0.00") + " | " + MaxRightAnkleAngle.ToString("0.00"), 18, 2);

            //if (LastSingleTrainingPlay != SingleTrainingPlay)
            //{
            //    float LastMinLeftSideAngle = LastTrainingPlay.angles[0].LeftSideAngle;
            //    float LastMinRightSideAngle = LastTrainingPlay.angles[0].RightSideAngle;
            //    float LastMinUponSideAngle = LastTrainingPlay.angles[0].UponSideAngle;
            //    float LastMinDownSideAngle = LastTrainingPlay.angles[0].DownSideAngle;
            //    float LastMinLeftArmAngle = LastTrainingPlay.angles[0].LeftArmAngle;
            //    float LastMinRightArmAngle = LastTrainingPlay.angles[0].RightArmAngle;
            //    float LastMinLeftElbowAngle = LastTrainingPlay.angles[0].LeftElbowAngle;
            //    float LastMinRightElbowAngle = LastTrainingPlay.angles[0].RightElbowAngle;
            //    float LastMinLeftLegAngle = LastTrainingPlay.angles[0].LeftLegAngle;
            //    float LastMinRightLegAngle = LastTrainingPlay.angles[0].RightLegAngle;
            //    float LastMinLeftHipAngle = LastTrainingPlay.angles[0].LeftHipAngle;
            //    float LastMinRightHipAngle = LastTrainingPlay.angles[0].RightHipAngle;
            //    float LastMinHipAngle = LastTrainingPlay.angles[0].HipAngle;
            //    float LastMinLeftKneeAngle = LastTrainingPlay.angles[0].LeftKneeAngle;
            //    float LastMinRightKneeAngle = LastTrainingPlay.angles[0].RightKneeAngle;
            //    float LastMinLeftAnkleAngle = LastTrainingPlay.angles[0].LeftAnkleAngle;
            //    float LastMinRightAnkleAngle = LastTrainingPlay.angles[0].RightAnkleAngle;

            //    float LastMaxLeftSideAngle = LastTrainingPlay.angles[0].LeftSideAngle;
            //    float LastMaxRightSideAngle = LastTrainingPlay.angles[0].RightSideAngle;
            //    float LastMaxUponSideAngle = LastTrainingPlay.angles[0].UponSideAngle;
            //    float LastMaxDownSideAngle = LastTrainingPlay.angles[0].DownSideAngle;
            //    float LastMaxLeftArmAngle = LastTrainingPlay.angles[0].LeftArmAngle;
            //    float LastMaxRightArmAngle = LastTrainingPlay.angles[0].RightArmAngle;
            //    float LastMaxLeftElbowAngle = LastTrainingPlay.angles[0].LeftElbowAngle;
            //    float LastMaxRightElbowAngle = LastTrainingPlay.angles[0].RightElbowAngle;
            //    float LastMaxLeftLegAngle = LastTrainingPlay.angles[0].LeftLegAngle;
            //    float LastMaxRightLegAngle = LastTrainingPlay.angles[0].RightLegAngle;
            //    float LastMaxLeftHipAngle = LastTrainingPlay.angles[0].LeftHipAngle;
            //    float LastMaxRightHipAngle = LastTrainingPlay.angles[0].RightHipAngle;
            //    float LastMaxHipAngle = LastTrainingPlay.angles[0].HipAngle;
            //    float LastMaxLeftKneeAngle = LastTrainingPlay.angles[0].LeftKneeAngle;
            //    float LastMaxRightKneeAngle = LastTrainingPlay.angles[0].RightKneeAngle;
            //    float LastMaxLeftAnkleAngle = LastTrainingPlay.angles[0].LeftAnkleAngle;
            //    float LastMaxRightAnkleAngle = LastTrainingPlay.angles[0].RightAnkleAngle;

            //    for (int i = 1; i < LastTrainingPlay.angles.Count; i++)
            //    {
            //        MinLeftSideAngle = Math.Min(MinLeftSideAngle, LastTrainingPlay.angles[i].LeftSideAngle);
            //        MinRightSideAngle = Math.Min(MinRightSideAngle, LastTrainingPlay.angles[i].RightSideAngle);
            //        MinUponSideAngle = Math.Min(MinUponSideAngle, LastTrainingPlay.angles[i].UponSideAngle);
            //        MinDownSideAngle = Math.Min(MinDownSideAngle, LastTrainingPlay.angles[i].DownSideAngle);
            //        MinLeftArmAngle = Math.Min(MinLeftArmAngle, LastTrainingPlay.angles[i].LeftArmAngle);
            //        MinRightArmAngle = Math.Min(MinRightArmAngle, LastTrainingPlay.angles[i].RightArmAngle);
            //        MinLeftElbowAngle = Math.Min(MinLeftElbowAngle, LastTrainingPlay.angles[i].LeftElbowAngle);
            //        MinRightElbowAngle = Math.Min(MinRightElbowAngle, LastTrainingPlay.angles[i].RightElbowAngle);
            //        MinLeftLegAngle = Math.Min(MinLeftLegAngle, LastTrainingPlay.angles[i].LeftLegAngle);
            //        MinRightLegAngle = Math.Min(MinRightLegAngle, LastTrainingPlay.angles[i].RightLegAngle);
            //        MinLeftHipAngle = Math.Min(MinLeftHipAngle, LastTrainingPlay.angles[i].LeftHipAngle);
            //        MinRightHipAngle = Math.Min(MinRightHipAngle, LastTrainingPlay.angles[i].RightHipAngle);
            //        MinHipAngle = Math.Min(MinHipAngle, LastTrainingPlay.angles[i].HipAngle);
            //        MinLeftKneeAngle = Math.Min(MinLeftKneeAngle, LastTrainingPlay.angles[i].LeftKneeAngle);
            //        MinRightKneeAngle = Math.Min(MinRightKneeAngle, LastTrainingPlay.angles[i].RightKneeAngle);
            //        MinLeftAnkleAngle = Math.Min(MinLeftAnkleAngle, LastTrainingPlay.angles[i].LeftAnkleAngle);
            //        MinRightAnkleAngle = Math.Min(MinRightAnkleAngle, LastTrainingPlay.angles[i].RightAnkleAngle);

            //        MaxLeftSideAngle = Math.Max(MaxLeftSideAngle, LastTrainingPlay.angles[i].LeftSideAngle);
            //        MaxRightSideAngle = Math.Max(MaxRightSideAngle, LastTrainingPlay.angles[i].RightSideAngle);
            //        MaxUponSideAngle = Math.Max(MaxUponSideAngle, LastTrainingPlay.angles[i].UponSideAngle);
            //        MaxDownSideAngle = Math.Max(MaxDownSideAngle, LastTrainingPlay.angles[i].DownSideAngle);
            //        MaxLeftArmAngle = Math.Max(MaxLeftArmAngle, LastTrainingPlay.angles[i].LeftArmAngle);
            //        MaxRightArmAngle = Math.Max(MaxRightArmAngle, LastTrainingPlay.angles[i].RightArmAngle);
            //        MaxLeftElbowAngle = Math.Max(MaxLeftElbowAngle, LastTrainingPlay.angles[i].LeftElbowAngle);
            //        MaxRightElbowAngle = Math.Max(MaxRightElbowAngle, LastTrainingPlay.angles[i].RightElbowAngle);
            //        MaxLeftLegAngle = Math.Max(MaxLeftLegAngle, LastTrainingPlay.angles[i].LeftLegAngle);
            //        MaxRightLegAngle = Math.Max(MaxRightLegAngle, LastTrainingPlay.angles[i].RightLegAngle);
            //        MaxLeftHipAngle = Math.Max(MaxLeftHipAngle, LastTrainingPlay.angles[i].LeftHipAngle);
            //        MaxRightHipAngle = Math.Max(MaxRightHipAngle, LastTrainingPlay.angles[i].RightHipAngle);
            //        MaxHipAngle = Math.Max(MaxHipAngle, LastTrainingPlay.angles[i].HipAngle);
            //        MaxLeftKneeAngle = Math.Max(MaxLeftKneeAngle, LastTrainingPlay.angles[i].LeftKneeAngle);
            //        MaxRightKneeAngle = Math.Max(MaxRightKneeAngle, LastTrainingPlay.angles[i].RightKneeAngle);
            //        MaxLeftAnkleAngle = Math.Max(MaxLeftAnkleAngle, LastTrainingPlay.angles[i].LeftAnkleAngle);
            //        MaxRightAnkleAngle = Math.Max(MaxRightAnkleAngle, LastTrainingPlay.angles[i].RightAnkleAngle);
            //    }

            //    SetResultDataText(LastMinLeftSideAngle.ToString("0.00") + " | " + LastMaxLeftSideAngle.ToString("0.00"), 2, 1);
            //    SetResultDataText(LastMinRightSideAngle.ToString("0.00") + " | " + LastMaxRightSideAngle.ToString("0.00"), 3, 1);
            //    SetResultDataText(LastMinUponSideAngle.ToString("0.00") + " | " + LastMaxUponSideAngle.ToString("0.00"), 4, 1);
            //    SetResultDataText(LastMinDownSideAngle.ToString("0.00") + " | " + LastMaxDownSideAngle.ToString("0.00"), 5, 1);
            //    SetResultDataText(LastMinLeftArmAngle.ToString("0.00") + " | " + LastMaxLeftArmAngle.ToString("0.00"), 6, 1);
            //    SetResultDataText(LastMinRightArmAngle.ToString("0.00") + " | " + LastMaxRightArmAngle.ToString("0.00"), 7, 1);
            //    SetResultDataText(LastMinLeftElbowAngle.ToString("0.00") + " | " + LastMaxLeftElbowAngle.ToString("0.00"), 8, 1);
            //    SetResultDataText(LastMinRightElbowAngle.ToString("0.00") + " | " + LastMaxRightElbowAngle.ToString("0.00"), 9, 1);
            //    SetResultDataText(LastMinLeftLegAngle.ToString("0.00") + " | " + LastMaxLeftLegAngle.ToString("0.00"), 10, 1);
            //    SetResultDataText(LastMinRightLegAngle.ToString("0.00") + " | " + LastMaxRightLegAngle.ToString("0.00"), 11, 1);
            //    SetResultDataText(LastMinLeftHipAngle.ToString("0.00") + " | " + LastMaxLeftHipAngle.ToString("0.00"), 12, 1);
            //    SetResultDataText(LastMinRightHipAngle.ToString("0.00") + " | " + LastMaxRightHipAngle.ToString("0.00"), 13, 1);
            //    SetResultDataText(LastMinHipAngle.ToString("0.00") + " | " + LastMaxHipAngle.ToString("0.00"), 14, 1);
            //    SetResultDataText(LastMinLeftKneeAngle.ToString("0.00") + " | " + LastMaxLeftKneeAngle.ToString("0.00"), 15, 1);
            //    SetResultDataText(LastMinRightKneeAngle.ToString("0.00") + " | " + LastMaxRightKneeAngle.ToString("0.00"), 16, 1);
            //    SetResultDataText(LastMinLeftAnkleAngle.ToString("0.00") + " | " + LastMaxLeftAnkleAngle.ToString("0.00"), 17, 1);
            //    SetResultDataText(LastMinRightAnkleAngle.ToString("0.00") + " | " + LastMaxRightAnkleAngle.ToString("0.00"), 18, 1);

            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftSideAngle, LastMaxLeftSideAngle), new Vector2(MinLeftSideAngle, MaxLeftSideAngle), 4), 2, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightSideAngle, LastMaxRightSideAngle), new Vector2(MinRightSideAngle, MaxRightSideAngle), 4), 3, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinUponSideAngle, LastMaxUponSideAngle), new Vector2(MinUponSideAngle, MaxUponSideAngle), 4), 4, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinDownSideAngle, LastMaxDownSideAngle), new Vector2(MinDownSideAngle, MaxDownSideAngle), 4), 5, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftArmAngle, LastMaxLeftArmAngle), new Vector2(MinLeftArmAngle, MaxLeftArmAngle), 4), 6, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightArmAngle, LastMaxRightArmAngle), new Vector2(MinRightArmAngle, MaxRightArmAngle), 4), 7, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftElbowAngle, LastMaxLeftElbowAngle), new Vector2(MinLeftElbowAngle, MaxLeftElbowAngle), 4), 8, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightElbowAngle, LastMaxRightElbowAngle), new Vector2(MinRightElbowAngle, MaxRightElbowAngle), 4), 9, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftLegAngle, LastMaxLeftLegAngle), new Vector2(MinLeftLegAngle, MaxLeftLegAngle), 4), 10, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightLegAngle, LastMaxRightLegAngle), new Vector2(MinRightLegAngle, MaxRightLegAngle), 4), 11, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftHipAngle, LastMaxLeftHipAngle), new Vector2(MinLeftHipAngle, MaxLeftHipAngle), 4), 12, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightHipAngle, LastMaxRightHipAngle), new Vector2(MinRightHipAngle, MaxRightHipAngle), 4), 13, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinHipAngle, LastMaxHipAngle), new Vector2(MinHipAngle, MaxHipAngle), 4), 14, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftKneeAngle, LastMaxLeftKneeAngle), new Vector2(MinLeftKneeAngle, MaxLeftKneeAngle), 4), 15, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightKneeAngle, LastMaxRightKneeAngle), new Vector2(MinRightKneeAngle, MaxRightKneeAngle), 4), 16, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftAnkleAngle, LastMaxLeftAnkleAngle), new Vector2(MinLeftAnkleAngle, MaxLeftAnkleAngle), 4), 17, 3);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightAnkleAngle, LastMaxRightAnkleAngle), new Vector2(MinRightAnkleAngle, MaxRightAnkleAngle), 4), 18, 3);

            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftSideAngle, LastMaxLeftSideAngle), new Vector2(MinLeftSideAngle, MaxLeftSideAngle), 2), 2, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightSideAngle, LastMaxRightSideAngle), new Vector2(MinRightSideAngle, MaxRightSideAngle), 2), 3, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinUponSideAngle, LastMaxUponSideAngle), new Vector2(MinUponSideAngle, MaxUponSideAngle), 2), 4, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinDownSideAngle, LastMaxDownSideAngle), new Vector2(MinDownSideAngle, MaxDownSideAngle), 2), 5, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftArmAngle, LastMaxLeftArmAngle), new Vector2(MinLeftArmAngle, MaxLeftArmAngle), 2), 6, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightArmAngle, LastMaxRightArmAngle), new Vector2(MinRightArmAngle, MaxRightArmAngle), 2), 7, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftElbowAngle, LastMaxLeftElbowAngle), new Vector2(MinLeftElbowAngle, MaxLeftElbowAngle), 2), 8, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightElbowAngle, LastMaxRightElbowAngle), new Vector2(MinRightElbowAngle, MaxRightElbowAngle), 2), 9, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftLegAngle, LastMaxLeftLegAngle), new Vector2(MinLeftLegAngle, MaxLeftLegAngle), 2), 10, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightLegAngle, LastMaxRightLegAngle), new Vector2(MinRightLegAngle, MaxRightLegAngle), 2), 11, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftHipAngle, LastMaxLeftHipAngle), new Vector2(MinLeftHipAngle, MaxLeftHipAngle), 2), 12, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightHipAngle, LastMaxRightHipAngle), new Vector2(MinRightHipAngle, MaxRightHipAngle), 2), 13, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinHipAngle, LastMaxHipAngle), new Vector2(MinHipAngle, MaxHipAngle), 2), 14, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftKneeAngle, LastMaxLeftKneeAngle), new Vector2(MinLeftKneeAngle, MaxLeftKneeAngle), 2), 15, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightKneeAngle, LastMaxRightKneeAngle), new Vector2(MinRightKneeAngle, MaxRightKneeAngle), 2), 16, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinLeftAnkleAngle, LastMaxLeftAnkleAngle), new Vector2(MinLeftAnkleAngle, MaxLeftAnkleAngle), 2), 17, 4);
            //    SetResultDataText(GetEvaluationResult(new Vector2(LastMinRightAnkleAngle, LastMaxRightAnkleAngle), new Vector2(MinRightAnkleAngle, MaxRightAnkleAngle), 2), 18, 4);
            //}
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
                return "<color=blue>" + "基本无变化" + "</color>";
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
