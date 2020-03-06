using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class DirectionRadarChartScript : MonoBehaviour
    {
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
            List<Point> tempPoints = new List<Point>();
            // 获取最后一个元素的点
            DoctorDataManager.instance.doctor.patient.Evaluations.Last().Points.ForEach(i => tempPoints.Add(i));  // 将所有的点复制给temppoints用于画凸包图


            // 绘制速度轨迹雷达图
            float WidthRate, HeightRate, Rate;  // 求长宽比值,做一下缩放
            float MaxX, MinX, MaxY, MinY;   // 求所有点的最大最小X, 最大最小Y
        
                
            //for(int i=0;i<)


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

        // Update is called once per frame
        void Update()
        {

        }
    }
}
