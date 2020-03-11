using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public ConvexHull convexHull;   // 新建一个凸包

        // 先平移,后放缩
        public Vector2 ModelGravity;   // 模型重心坐标
        public Vector2 GravityDiff;    // 重心偏移
        public float HeightPixel;    // 身高段

        public Color[] ConvexHullColors;
        public Color ConvexHullAreaColor;
        public Color ConvexHullLineColor;

        public SoccerDistance tempSoccerDistance;


        //public Canvas canvas;
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
                EvaluationPoints = new List<Point>();
                foreach (var point in DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].Points)
                {
                    EvaluationPoints.Add(new Point(point.x, point.y));
                }

                tempSoccerDistance = new SoccerDistance(DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance);

                if (DoctorDataManager.instance.doctor.patient.PatientSex == "女")
                {
                    ManImage.SetActive(true); ManSideImage.SetActive(true);
                    WomanImage.SetActive(false); WomanSideImage.SetActive(false);

                    //WidthPixel = 100;
                    HeightPixel = 120;

                    ModelGravity = new Vector2(1260, 400);
                }
                else
                {
                    ManImage.SetActive(false); ManSideImage.SetActive(false);
                    WomanImage.SetActive(true); WomanSideImage.SetActive(true);

                    //WidthPixel = 80;
                    HeightPixel = 120;

                    ModelGravity = new Vector2(1260, 410);
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

                // 默认画凸包图

                ConvexHullLineColor = Color.red;
                ConvexHullAreaColor = new Color32(255,0,0,40);

                ContexHullToggle.isOn = true;
                //DrawContexHullToggleChange();





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

        public void DrawContexHullToggleChange()
        {
            if (ContexHullToggle.isOn)
            {
                DrawContexHull();
            }
            else
            {
                VectorLine.Destroy(ref ConvexHullLine);
                VectorLine.Destroy(ref ConvexHullArea);
            }
        }

        public void DrawContexHull()
        {
            if (EvaluationPoints == null) return;

            List<Point> tempPoints = new List<Point>();

            foreach (var point in EvaluationPoints)
            {
                tempPoints.Add(new Point(point.x, point.y));
            }

            convexHull = new ConvexHull(tempPoints);

            // 画凸包圈
            ConvexHullLine = new VectorLine("ConvexHullLine", new List<Vector2>(), 5.0f, Vectrosity.LineType.Continuous, Joins.Weld);
            ConvexHullLine.smoothColor = false;   // 设置平滑颜色
            ConvexHullLine.smoothWidth = false;   // 设置平滑宽度
            //ConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed
            ConvexHullLine.color = ConvexHullLineColor;  // 设置颜色


            int MinX = Mathf.FloorToInt(convexHull.ConvexHullSet[0].x), MaxX = Mathf.CeilToInt(convexHull.ConvexHullSet[0].x);   // 凸包的最大最小X
            int MinY = Mathf.FloorToInt(convexHull.ConvexHullSet[0].y), MaxY = Mathf.CeilToInt(convexHull.ConvexHullSet[0].y);   // 凸包的最大最小Y


            // 先把初始点存入画图函数
            ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[0].x, convexHull.ConvexHullSet[0].y));
            convexHull.ConvexHullArea = 0f;   // 令凸包面积初始为0

            for (int i = 1; i < convexHull.ConvexHullNum; i++)
            {
                ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[i].x, convexHull.ConvexHullSet[i].y));
                //ConvexHullLine.SetColor(ConvexHullLineColor);  // 设置颜色

                if (i < convexHull.ConvexHullNum - 1)
                {
                    convexHull.ConvexHullArea += Math.Abs(ConvexHull.isLeft(convexHull.ConvexHullSet[0], convexHull.ConvexHullSet[i], convexHull.ConvexHullSet[i + 1]));
                }

                if (MinX > Mathf.FloorToInt(convexHull.ConvexHullSet[i].x)) MinX = Mathf.FloorToInt(convexHull.ConvexHullSet[i].x);
                if (MaxX < Mathf.CeilToInt(convexHull.ConvexHullSet[i].x)) MaxX = Mathf.CeilToInt(convexHull.ConvexHullSet[i].x);
                if (MinY > Mathf.FloorToInt(convexHull.ConvexHullSet[i].y)) MinY = Mathf.FloorToInt(convexHull.ConvexHullSet[i].y);
                if (MaxY < Mathf.CeilToInt(convexHull.ConvexHullSet[i].y)) MaxY = Mathf.CeilToInt(convexHull.ConvexHullSet[i].y);


                //ConvexHullLine.Draw();
                //yield return new WaitForSeconds(0.15f);
            }

            //button.transform.GetChild(0).GetComponent<Text>().text = (ConvexHullArea / 2).ToString("0.00");// 最后求出来的面积要除以2

            ConvexHullLine.points2.Add(new Vector2(convexHull.ConvexHullSet[0].x, convexHull.ConvexHullSet[0].y));
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
                    for (int i = 0; i < MaxY; i++)
                    {
                        x = i * (MaxX - MinX); y = (i + 1) * (MaxX - MinX);

                        while ((x < y) && (ConvexHullColors[x] != ConvexHullLineColor)) x++;    // 查找左边的凸包边界
                        while ((x < y) && (ConvexHullColors[y] != ConvexHullLineColor)) y--;    // 查找右边的凸包边界

                        if (x != y)
                        {
                            ConvexHullArea.points2.Add(new Vector2(MinX + x - i * (MaxX - MinX), MinY + i));
                            ConvexHullArea.points2.Add(new Vector2(MinX + y - i * (MaxX - MinX), MinY + i));
                        }
                    }
                    ConvexHullArea.Draw();
                //}
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
            //VectorLine.Destroy(ref ConvexHullLine);
            //VectorLine.Destroy(ref ConvexHullArea);

            //VectorLine.Destroy(ref ColorFistLineTrack);
            //VectorLine.Destroy(ref ColorFistLineReality);

            //transform.GetChild(2).GetChild(4).gameObject.SetActive(false);
            ContexHullToggle.isOn = false;
            //TrackToggle.isOn = false;
            //SoccerballToggle.isOn = false;
            RealityToggle.isOn = false;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (RealityToggle.isOn)
                {
                    DrawColorFistTrack();
                    //StopCoroutine("DrawColorFistLine");
                }
            }

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
            
            RemoveLine();
            
            SceneManager.LoadScene("05-RadarTest");
        }

        public void ReadReportButtonOnclick()
        {
            Report.SetActive(true);
            EvaluationToggle.isOn = true;
        }

    }
}

