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
                WriteTrainingData();
            }
        }

        public void DrawRadarChart()
        {
            DirectionRadarChart = transform.Find("Chart/RadarChartResult").GetComponent<RadarChart>();
            if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("Chart/RadarChartResult").gameObject.AddComponent<RadarChart>();

            DirectionRadarChart.gameObject.SetActive(false);
            DirectionRadarChart.gameObject.SetActive(true);

            DirectionRadarChart.UpdateData(0, 0, 0, Mathf.Min(1f, 1.0f * RealityTrainingDuration / 60 / fishTrainingPlay.PlanDuration));
            DirectionRadarChart.UpdateData(0, 0, 1, Mathf.Min(1f, 5.0f / fishTrainingPlay.FishCaptureTime.Average()));
            DirectionRadarChart.UpdateData(0, 0, 2, Mathf.Min(1f, 1.0f * fishTrainingPlay.Distance / (RealityTrainingDuration * 40)));
            long HistoryCaptureCount = 0;
            long HistoryAllCount = 0;
            for (int i = 0; i <= SingleTrainingPlay; i++)
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

        public void WriteTrainingData()
        {
            // 初始化对比结果
            for (int m = 0; m < 9; m++)
            {
                for (int n = 1; n < 5; n++)
                {
                    SetResultDataText("-", m, n);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                SetResultDataText(DirectionRadarChart.series.list[0].data[0].data[i].ToString("0.000"), i, 2);
            }

            SetResultDataText(fishTrainingPlay.FishCaptureTime.Average().ToString("0.000"), 5, 2);
            SetResultDataText(fishTrainingPlay.GCAngles.Average().ToString("0.000") + " | " + fishTrainingPlay.GCAngles.Max().ToString("0.000"), 6, 2);
            SetResultDataText(fishTrainingPlay.Experience.ToString("0.000"), 7, 2);
            SetResultDataText(fishTrainingPlay.Distance.ToString("0.000"), 8, 2);

            if (LastTrainingPlay != SingleTrainingPlay)
            {
                long LastRealityTrainingDuration = long.Parse(LastFishTrainingPlay.TrainingEndTime.Substring(9, 2)) * 3600 + long.Parse(LastFishTrainingPlay.TrainingEndTime.Substring(12, 2)) * 60 + long.Parse(LastFishTrainingPlay.TrainingEndTime.Substring(15, 2))
                                          - long.Parse(LastFishTrainingPlay.TrainingStartTime.Substring(9, 2)) * 3600 - long.Parse(LastFishTrainingPlay.TrainingStartTime.Substring(12, 2)) * 60 - long.Parse(LastFishTrainingPlay.TrainingStartTime.Substring(15, 2));

                SetResultDataText(Mathf.Min(1f, 1.0f * LastRealityTrainingDuration / 60 / LastFishTrainingPlay.PlanDuration).ToString("0.000"), 0, 1);
                SetResultDataText(Mathf.Min(1f, 5.0f / LastFishTrainingPlay.FishCaptureTime.Average()).ToString("0.000"), 1, 1);
                SetResultDataText(Mathf.Min(1f, 1.0f * LastFishTrainingPlay.Distance / (LastRealityTrainingDuration * 40)).ToString("0.000"), 2, 1);

                long HistoryCaptureCount = 0;
                long HistoryAllCount = 0;
                for (int i = 0; i <= LastTrainingPlay; i++)
                {
                    HistoryCaptureCount += DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].StaticFishSuccessCount + DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].DynamicFishSuccessCount;
                    HistoryAllCount += DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].StaticFishAllCount + DoctorDataManager.instance.doctor.patient.FishTrainingPlays[i].DynamicFishAllCount;
                }
                SetResultDataText((1.0f * HistoryCaptureCount / HistoryAllCount).ToString("0.000"), 3, 1);


                SetResultDataText((1.0f * (LastFishTrainingPlay.StaticFishSuccessCount + LastFishTrainingPlay.DynamicFishSuccessCount) / (
                    LastFishTrainingPlay.StaticFishAllCount + LastFishTrainingPlay.DynamicFishAllCount)).ToString("0.000"), 4, 1);

                SetResultDataText(LastFishTrainingPlay.FishCaptureTime.Average().ToString("0.000"), 5, 1);
                SetResultDataText(LastFishTrainingPlay.GCAngles.Average().ToString("0.000") + " | " + LastFishTrainingPlay.GCAngles.Max().ToString("0.000"), 6, 1);
                SetResultDataText(LastFishTrainingPlay.Experience.ToString("0.000"), 7, 1);
                SetResultDataText(LastFishTrainingPlay.Distance.ToString("0.000"), 8, 1);


                print(1.0f * LastRealityTrainingDuration / 60 / LastFishTrainingPlay.PlanDuration);

                SetResultDataText(GetEvaluationResult(Mathf.Min(1f, 1.0f * LastRealityTrainingDuration / 60 / LastFishTrainingPlay.PlanDuration), DirectionRadarChart.series.list[0].data[0].data[0], 4), 0, 3);
                SetResultDataText(GetEvaluationResult(Mathf.Min(1f, 5.0f / LastFishTrainingPlay.FishCaptureTime.Average()), DirectionRadarChart.series.list[0].data[0].data[1], 4), 1, 3);
                SetResultDataText(GetEvaluationResult(Mathf.Min(1f, 1.0f * LastFishTrainingPlay.Distance / (LastRealityTrainingDuration * 40)), DirectionRadarChart.series.list[0].data[0].data[2], 4), 2, 3);
                SetResultDataText(GetEvaluationResult(1.0f * HistoryCaptureCount / HistoryAllCount, DirectionRadarChart.series.list[0].data[0].data[3], 4), 3, 3);
                SetResultDataText(GetEvaluationResult(1.0f * (LastFishTrainingPlay.StaticFishSuccessCount + LastFishTrainingPlay.DynamicFishSuccessCount) / (
                    LastFishTrainingPlay.StaticFishAllCount + LastFishTrainingPlay.DynamicFishAllCount), DirectionRadarChart.series.list[0].data[0].data[4], 4), 4, 3);

                SetResultDataText(GetEvaluationResult(LastFishTrainingPlay.FishCaptureTime.Average(), fishTrainingPlay.FishCaptureTime.Average(), 4), 5, 3);
                SetResultDataText(GetEvaluationResult(new Vector2(LastFishTrainingPlay.GCAngles.Average(), LastFishTrainingPlay.GCAngles.Max()), new Vector2(fishTrainingPlay.GCAngles.Average(), fishTrainingPlay.GCAngles.Max()), 4), 6, 3);
                SetResultDataText(GetEvaluationResult(LastFishTrainingPlay.Experience, fishTrainingPlay.Experience, 4), 7, 3);
                SetResultDataText(GetEvaluationResult(LastFishTrainingPlay.Distance, fishTrainingPlay.Distance, 4), 8, 3);


                SetResultDataText(GetEvaluationResult(Mathf.Min(1f, 1.0f * LastRealityTrainingDuration / 60 / LastFishTrainingPlay.PlanDuration), DirectionRadarChart.series.list[0].data[0].data[0], 2), 0, 4);
                SetResultDataText(GetEvaluationResult(Mathf.Min(1f, 5.0f / LastFishTrainingPlay.FishCaptureTime.Average()), DirectionRadarChart.series.list[0].data[0].data[1], 2), 1, 4);
                SetResultDataText(GetEvaluationResult(Mathf.Min(1f, 1.0f * LastFishTrainingPlay.Distance / (LastRealityTrainingDuration * 40)), DirectionRadarChart.series.list[0].data[0].data[2], 2), 2, 4);
                SetResultDataText(GetEvaluationResult(1.0f * HistoryCaptureCount / HistoryAllCount, DirectionRadarChart.series.list[0].data[0].data[3], 2), 3, 4);
                SetResultDataText(GetEvaluationResult(1.0f * (LastFishTrainingPlay.StaticFishSuccessCount + LastFishTrainingPlay.DynamicFishSuccessCount) / (
                    LastFishTrainingPlay.StaticFishAllCount + LastFishTrainingPlay.DynamicFishAllCount), DirectionRadarChart.series.list[0].data[0].data[4], 2), 4, 4);

                SetResultDataText(GetEvaluationResult(LastFishTrainingPlay.FishCaptureTime.Average(), fishTrainingPlay.FishCaptureTime.Average(), 2), 5, 4);
                SetResultDataText(GetEvaluationResult(new Vector2(LastFishTrainingPlay.GCAngles.Average(), LastFishTrainingPlay.GCAngles.Max()), new Vector2(fishTrainingPlay.GCAngles.Average(), fishTrainingPlay.GCAngles.Max()), 2), 6, 4);
                SetResultDataText(GetEvaluationResult(LastFishTrainingPlay.Experience, fishTrainingPlay.Experience, 2), 7, 4);
                SetResultDataText(GetEvaluationResult(LastFishTrainingPlay.Distance, fishTrainingPlay.Distance, 2), 8, 4);
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
