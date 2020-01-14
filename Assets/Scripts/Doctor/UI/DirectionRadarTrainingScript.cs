using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class DirectionRadarTrainingScript : MonoBehaviour
    {
        public RadarChart DirectionRadarChart; // 雷达图
        public Serie serie, serie1;

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

            RadarArea = new List<float>();
            RadarIncreaseRate = new List<float>();

            RadarAreaText = transform.Find("RadarArea").GetComponent<Text>();

            WhiteLine = "";

            DirectionRadarChart = transform.Find("RadarChart").GetComponent<RadarChart>();
            if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("RadarChart").gameObject.AddComponent<RadarChart>();
            //DirectionRadarChart.RemoveRadar();
            //DirectionRadarChart.RemoveData();
            DirectionRadarChart.ClearData();

            //DirectionRadarChart.title.text = "";
            //DirectionRadarChart.title.subText = "";
            //DirectionRadarChart.title.textStyle.fontSize = 30;
            //DirectionRadarChart.title.textStyle.fontStyle = FontStyle.Bold;
            //DirectionRadarChart.title.location.top = 0;

            //DirectionRadarChart.legend.show = false;
            //DirectionRadarChart.legend.location.align = Location.Align.TopLeft;
            //DirectionRadarChart.legend.location.top = 60;
            //DirectionRadarChart.legend.location.left = 2;
            //DirectionRadarChart.legend.itemWidth = 70;
            //DirectionRadarChart.legend.itemHeight = 20;
            //DirectionRadarChart.legend.orient = Orient.Vertical;

            //DirectionRadarChart.AddRadar(Radar.Shape.Polygon, new Vector2(0.5f, 0.4f), 0.4f);
            //DirectionRadarChart.AddIndicator(0, "正上", 0, 180);
            //DirectionRadarChart.AddIndicator(0, "右上", 0, 180);
            //DirectionRadarChart.AddIndicator(0, "正右", 0, 180);
            //DirectionRadarChart.AddIndicator(0, "右下", 0, 180);
            //DirectionRadarChart.AddIndicator(0, "正下", 0, 180);
            //DirectionRadarChart.AddIndicator(0, "左下", 0, 180);
            //DirectionRadarChart.AddIndicator(0, "正左", 0, 180);
            //DirectionRadarChart.AddIndicator(0, "左上", 0, 180);

            //serie = DirectionRadarChart.AddSerie(SerieType.Pie);
            //serie.radarIndex = 0;
            //serie.symbol.type = SerieSymbolType.Circle;
            //serie.symbol.size = 3;
            //serie.symbol.selectedSize = 4;
            //serie.symbol.color = Color.blue;

            //serie.lineStyle.color = Color.red;
            //serie.lineStyle.width = 1;

            if(DoctorDataManager.instance.doctor.patient.trainingPlays != null && DoctorDataManager.instance.doctor.patient.trainingPlays.Count > 0)
            {
                if (DoctorDataManager.instance.doctor.patient.trainingPlays.Count > 6) WhiteLine = "\n";
                else WhiteLine = "\n\n";

                //print(DoctorDataManager.instance.patient.Evaluations.Count+"!!!!");

                for (int i = 0; i < DoctorDataManager.instance.doctor.patient.trainingPlays.Count; i++)
                {
                    //DoctorDataManager.instance.patient.trainingPlays[i].direction = DoctorDatabaseManager.instance.ReadDirectionRecord(DoctorDataManager.instance.patient.trainingPlays[i].TrainingID);

                    if (i == DoctorDataManager.instance.doctor.patient.trainingPlays.Count - 1)
                    {
                        DirectionRadarChart.AddData(0, DoctorDataManager.instance.doctor.patient.trainingPlays[i].direction.GetDirections(), "第" + (i + 1).ToString() + "次");
                    }
                    // print(DoctorDataManager.instance.patient.Evaluations[i].direction.UponDirection+"+++++");

                    RadarArea.Add(DoctorDataManager.instance.doctor.patient.trainingPlays[i].direction.GetRadarArea());

                    if (i == 0)
                    {
                        RadarAreaText.text = "（1）雷达图面积: " + RadarArea[i].ToString("0.00");
                    }
                    else
                    {
                        //print(RadarArea[i] + "####");
                        //print(RadarArea[i-1] + "####");
                        //print((RadarArea[i] - RadarArea[i - 1]) / RadarArea[i - 1] + "@@@@@");

                        RadarIncreaseRate.Add((RadarArea[i] - RadarArea[i - 1]) / RadarArea[i - 1]);

                        //print(RadarIncreaseRate[i] + "@@@@");

                        RadarAreaText.text += WhiteLine;
                        RadarAreaText.text += "（" + (i + 1).ToString() + "）雷达图面积: " + RadarArea[i].ToString("0.00");
                        if (RadarIncreaseRate[i - 1] < 0) RadarAreaText.text += "  <color=blue>" + RadarIncreaseRate[i - 1].ToString("0.00") + "%  Down</color>";
                        else if (RadarIncreaseRate[i - 1] == 0) RadarAreaText.text += "  <color=green>" + RadarIncreaseRate[i - 1].ToString("0.00") + "%  Equal</color>";
                        else RadarAreaText.text += "  <color=red>" + RadarIncreaseRate[i - 1].ToString("0.00") + "%  Up</color>";
                    }
                }

            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

