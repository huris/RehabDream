using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XCharts
{
	[DisallowMultipleComponent]
	public class DirectionRadarChartScript : MonoBehaviour
	{
		public RadarChart DirectionRadarChart; // 雷达图
		public Serie serie, serie1;

		// Use this for initialization
		void Start()
		{

		}

		void OnEnable()
		{
            DirectionRadarChart = transform.Find("RadarChart").GetComponent<RadarChart>();
            if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("RadarChart").gameObject.AddComponent<RadarChart>();
            DirectionRadarChart.RemoveRadar();
            DirectionRadarChart.RemoveData();

            DirectionRadarChart.title.text = "";
            //DirectionRadarChart.title.subText = "";
            //DirectionRadarChart.title.textStyle.fontSize = 30;
            //DirectionRadarChart.title.textStyle.fontStyle = FontStyle.Bold;
            //DirectionRadarChart.title.location.top = 0;

            DirectionRadarChart.legend.show = true;
            DirectionRadarChart.legend.location.align = Location.Align.TopLeft;
            DirectionRadarChart.legend.location.top = 60;
            DirectionRadarChart.legend.location.left = 2;
            DirectionRadarChart.legend.itemWidth = 70;
            DirectionRadarChart.legend.itemHeight = 20;
            DirectionRadarChart.legend.orient = Orient.Vertical;

            DirectionRadarChart.AddRadar(Radar.Shape.Polygon, new Vector2(0.5f, 0.4f), 0.4f);
            DirectionRadarChart.AddIndicator(0, "正上", 0, 180);
            DirectionRadarChart.AddIndicator(0, "右上", 0, 180);
            DirectionRadarChart.AddIndicator(0, "正右", 0, 180);
            DirectionRadarChart.AddIndicator(0, "右下", 0, 180);
            DirectionRadarChart.AddIndicator(0, "正下", 0, 180);
            DirectionRadarChart.AddIndicator(0, "左下", 0, 180);
            DirectionRadarChart.AddIndicator(0, "正左", 0, 180);
            DirectionRadarChart.AddIndicator(0, "左上", 0, 180);

            serie = DirectionRadarChart.AddSerie(SerieType.Pie);
            serie.radarIndex = 0;

            for (int i = 0; i < DoctorDataManager.instance.patient.Evaluations.Count; i++)
            {
                DirectionRadarChart.AddData(0, DoctorDataManager.instance.patient.Evaluations[i].direction.GetDirections(), "第" + (i + 1).ToString() + "次");
            } 
        }

		// Update is called once per frame
		void Update()
		{

		}
	}
}
