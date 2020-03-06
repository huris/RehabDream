using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class EvaluateInitScript : MonoBehaviour
    {


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



        // Use this for initialization
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
            EvaluationToggle = transform.parent.parent.parent.Find("Report/Evaluation").GetComponent<Toggle>();

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
                int SingleEvaluation = DoctorDataManager.instance.doctor.patient.EvaluationIndex;

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

                if (TrainingEvaluationRate >= 0.95f) { Rank1.SetActive(true); EvaluationRank.text = "1 级"; }
                else if (TrainingEvaluationRate >= 0.90f) { Rank2.SetActive(true); EvaluationRank.text = "2 级"; }
                else if (TrainingEvaluationRate >= 0.80f) { Rank3.SetActive(true); EvaluationRank.text = "3 级"; }
                else if (TrainingEvaluationRate >= 0.70f) { Rank4.SetActive(true); EvaluationRank.text = "4 级"; }
                else { Rank5.SetActive(true); EvaluationRank.text = "5 级"; }

                EvaluationScore.text = TrainingEvaluationRate.ToString("0.00");

                EvaluationStartTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationStartTime;
                EvaluationEndTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].EvaluationEndTime;

                // 计算有训练时长
                EvaluationTime.text = (long.Parse(EvaluationEndTime.text.Substring(9, 2)) * 3600 + long.Parse(EvaluationEndTime.text.Substring(12, 2)) * 60 + long.Parse(EvaluationEndTime.text.Substring(15, 2))
                                           - long.Parse(EvaluationStartTime.text.Substring(9, 2)) * 3600 - long.Parse(EvaluationStartTime.text.Substring(12, 2)) * 60 - long.Parse(EvaluationStartTime.text.Substring(15, 2))).ToString() + "秒";

                // SpeedRadar
                List<Point> tempPoints = new List<Point>();
                // 获取最后一个元素的点
                DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].Points.ForEach(i => tempPoints.Add(i));  // 将所有的点复制给temppoints用于画凸包图


                // 绘制速度轨迹雷达图
                float WidthRate, HeightRate, Rate;  // 求长宽比值,做一下缩放
                float MaxX, MinX, MaxY, MinY;   // 求所有点的最大最小X, 最大最小Y


                for(int i = 0; i < tempPoints.Count; i++)
                {

                }


                //RadarArea = new List<float>();
                //RadarIncreaseRate = new List<float>();

                //RadarAreaText = transform.Find("RadarArea").GetComponent<Text>();

                //WhiteLine = "";

                //DirectionRadarChart = transform.Find("RadarChart").GetComponent<RadarChart>();
                //if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("RadarChart").gameObject.AddComponent<RadarChart>();
                ////DirectionRadarChart.RemoveRadar();
                ////DirectionRadarChart.RemoveData();
                //DirectionRadarChart.ClearData();

                ////DirectionRadarChart.title.text = "";
                ////DirectionRadarChart.title.subText = "";
                ////DirectionRadarChart.title.textStyle.fontSize = 30;
                ////DirectionRadarChart.title.textStyle.fontStyle = FontStyle.Bold;
                ////DirectionRadarChart.title.location.top = 0;

                ////DirectionRadarChart.legend.show = false;
                ////DirectionRadarChart.legend.location.align = Location.Align.TopLeft;
                ////DirectionRadarChart.legend.location.top = 60;
                ////DirectionRadarChart.legend.location.left = 2;
                ////DirectionRadarChart.legend.itemWidth = 70;
                ////DirectionRadarChart.legend.itemHeight = 20;
                ////DirectionRadarChart.legend.orient = Orient.Vertical;

                ////DirectionRadarChart.AddRadar(Radar.Shape.Polygon, new Vector2(0.5f, 0.45f), 0.35f);
                ////DirectionRadarChart.AddIndicator(0, "正上", 0, 180);
                ////DirectionRadarChart.AddIndicator(0, "右上", 0, 180);
                ////DirectionRadarChart.AddIndicator(0, "正右", 0, 180);
                ////DirectionRadarChart.AddIndicator(0, "右下", 0, 180);
                ////DirectionRadarChart.AddIndicator(0, "正下", 0, 180);
                ////DirectionRadarChart.AddIndicator(0, "左下", 0, 180);
                ////DirectionRadarChart.AddIndicator(0, "正左", 0, 180);
                ////DirectionRadarChart.AddIndicator(0, "左上", 0, 180);

                ////DirectionRadarChart.radars[0].lineStyle.width = 1.7f;
                ////DirectionRadarChart.radars[0].lineStyle.opacity = 0.66f;

                ////DirectionRadarChart.radars[0].splitArea.color = new List<Color>();
                ////Color color = new Color();
                ////color.r = 55;
                ////color.g = 139;
                ////color.b = 146;

                ////DirectionRadarChart.radars[0].splitArea.color.Add(color);
                //////{ new Color(55, 139, 146), new Color(91, 193, 197), new Color(149, 221, 226), new Color(214, 241, 243), new Color(238, 251, 251) };
                ////print(DirectionRadarChart.radars[0].splitArea.color.Count);
                ////DirectionRadarChart.radars[0].indicatorGap = 10;

                ////serie = DirectionRadarChart.AddSerie(SerieType.Radar);
                ////serie.radarIndex = 0;
                ////serie.symbol.type = SerieSymbolType.Circle;
                ////serie.symbol.size = 4;
                ////serie.symbol.selectedSize = 5;
                ////serie.symbol.color = Color.red;

                ////serie.lineStyle.color = Color.red;
                ////serie.lineStyle.width = 2;

                //if(DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
                //{
                //    if (DoctorDataManager.instance.doctor.patient.Evaluations.Count > 6) WhiteLine = "\n";
                //    else WhiteLine = "\n\n";

                //    //print(DoctorDataManager.instance.patient.Evaluations.Count+"!!!!");

                //    for (int i = 0; i < DoctorDataManager.instance.doctor.patient.Evaluations.Count; i++)
                //    {
                //        //DoctorDataManager.instance.patient.Evaluations[i].direction = DoctorDatabaseManager.instance.ReadDirectionRecord(DoctorDataManager.instance.patient.Evaluations[i].TrainingID);

                //        if (i == DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1)
                //        {
                //            DirectionRadarChart.AddData(0, DoctorDataManager.instance.doctor.patient.Evaluations[i].direction.GetDirections(), "第" + (i + 1).ToString() + "次");
                //        }

                //        // print(DoctorDataManager.instance.patient.Evaluations[i].direction.UponDirection+"+++++");

                //        RadarArea.Add(DoctorDataManager.instance.doctor.patient.Evaluations[i].direction.DirectionRadarArea);

                //        if (i == 0)
                //        {
                //            RadarAreaText.text = "（1）雷达图面积: " + RadarArea[i].ToString("0.00");
                //        }
                //        else
                //        {
                //            //print(RadarArea[i] + "####");
                //            //print(RadarArea[i-1] + "####");
                //            //print((RadarArea[i] - RadarArea[i - 1]) / RadarArea[i - 1] + "@@@@@");

                //            RadarIncreaseRate.Add((RadarArea[i] - RadarArea[i - 1]) / RadarArea[i - 1]);

                //            //print(RadarIncreaseRate[i] + "@@@@");

                //            RadarAreaText.text += WhiteLine;
                //            RadarAreaText.text += "（" + (i + 1).ToString() + "）雷达图面积: " + RadarArea[i].ToString("0.00");
                //            if (RadarIncreaseRate[i - 1] < 0) RadarAreaText.text += "  <color=blue>" + (RadarIncreaseRate[i - 1] * 100).ToString("0.00") + "%  Down</color>";
                //            else if (RadarIncreaseRate[i - 1] == 0) RadarAreaText.text += "  <color=green>" + (RadarIncreaseRate[i - 1] * 100).ToString("0.00") + "%  Equal</color>";
                //            else RadarAreaText.text += "  <color=red>" + (RadarIncreaseRate[i - 1] * 100).ToString("0.00") + "%  Up</color>";
                //        }
                //    }
                //}



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

        public void EvaluationChange()
        {
            // 刷新评估界面
            DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1 - EvaluationSelect.value);

            this.OnEnable();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EvaluateButtonOnclick()
        {
            //print(DoctorDataManager.instance.patient.PatientID);
            //print(DoctorDataManager.instance.patient.PatientName);
            //print(DoctorDataManager.instance.patient.PatientSex);

            //PatientDataManager.instance.SetUserMessage(DoctorDataManager.instance.doctor.patient.PatientID, DoctorDataManager.instance.doctor.patient.PatientName, DoctorDataManager.instance.doctor.patient.PatientSex);
            ////PatientDataManager.instance.SetTrainingPlan(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.patient.trainingPlan.PlanDifficulty), DoctorDataManager.instance.patient.trainingPlan.GameCount, DoctorDataManager.instance.patient.trainingPlan.PlanCount);

            //Evaluation evaluation = new Evaluation();
            //evaluation.SetEvaluationID(DoctorDatabaseManager.instance.ReadPatientEvaluationCount());
            ////print(DoctorDataManager.instance.doctor.patient.Evaluations.Count);
            //if(DoctorDataManager.instance.doctor.patient.Evaluations == null)
            //{
            //    DoctorDataManager.instance.doctor.patient.Evaluations = new List<Evaluation>();
            //}
            //DoctorDataManager.instance.doctor.patient.Evaluations.Add(evaluation);

            //PatientDataManager.instance.SetTrainingID(evaluation.TrainingID);
            //PatientDataManager.instance.SetMaxSuccessCount(DoctorDataManager.instance.doctor.patient.MaxSuccessCount);
            //PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty));
            //PatientDataManager.instance.SetPlanCount(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanCount);
            //PatientDataManager.instance.SetPlanDirection(PatientDataManager.Str2DirectionType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDirection));
            //PatientDataManager.instance.SetPlanTime(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanTime);
            //PatientDataManager.instance.SetIsEvaluated(1);

            DoctorDataManager.instance.FunctionManager = 1;  // 返回的时候进入患者状况评估界面
            SceneManager.LoadScene("05-RadarTest");
        }

        public void ReadReportButtonOnclick()
        {
            Report.SetActive(true);
            EvaluationToggle.isOn = true;
        }

    }
}

