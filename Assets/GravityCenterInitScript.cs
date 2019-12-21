using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XCharts
{
	[DisallowMultipleComponent]

	public class GravityCenterInitScript : MonoBehaviour
	{

		private LineChart GravityCenterChart;   // 重心图
		private Serie GravityCenterSerie;

		private long GravityCenterCount;   // 重心点

		// Use this for initialization
		void Start()
		{
            int a;
		}

		void OnEnable()
		{
			if (DoctorDataManager.instance.patient.trainingPlays.Count > 0)
			{
				DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters = DoctorDatabaseManager.instance.ReadGravityCenterRecord(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count-1].TrainingID);
				GravityCenterCount = DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters.Count;
                //print(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[0].TrainingID);
                GravityCenterChart = transform.Find("GravityCenterChart").GetComponent<LineChart>();
                if (GravityCenterChart == null) GravityCenterChart = transform.Find("GravityCenterChart").gameObject.AddComponent<LineChart>();

                GravityCenterChart.themeInfo.theme = Theme.Light;
                //chart.themeInfo.tooltipBackgroundColor = Color.white;
                //chart.themeInfo.backgroundColor = Color.grey;

                GravityCenterChart.title.show = true;
                GravityCenterChart.title.text = "与 初 始 点 距 离";
                GravityCenterChart.title.textFontSize = 20;
                GravityCenterChart.title.location.top = 13;

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
                GravityCenterChart.tooltip.formatter = "   第{b}秒   \n重心距离为{c}";

                GravityCenterChart.xAxis0.show = true;
                GravityCenterChart.xAxis0.type = XAxis.AxisType.Category;
                GravityCenterChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
                GravityCenterChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
                GravityCenterChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                GravityCenterChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
                GravityCenterChart.xAxis0.axisLine.symbolWidth = 10;
                GravityCenterChart.xAxis0.axisLine.symbolHeight = 15;
                GravityCenterChart.xAxis0.axisLine.symbolOffset = 0;
                GravityCenterChart.xAxis0.axisLine.symbolDent = 3;
                GravityCenterChart.xAxis0.axisName.show = true;  // 坐标轴名称
                GravityCenterChart.xAxis0.axisName.name = "时间（秒）";
                GravityCenterChart.xAxis0.axisName.location = AxisName.Location.Middle;
                GravityCenterChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
                GravityCenterChart.xAxis0.axisName.rotate = 0;
                GravityCenterChart.xAxis0.axisName.color = Color.black;
                GravityCenterChart.xAxis0.axisName.fontSize = 15;
                GravityCenterChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
                GravityCenterChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                GravityCenterChart.xAxis0.axisLabel.fontSize = 12;
                GravityCenterChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                GravityCenterChart.xAxis0.showSplitLine = true;
                GravityCenterChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
                GravityCenterChart.xAxis0.splitArea.show = true;

                GravityCenterChart.yAxis0.show = true;
                GravityCenterChart.yAxis0.type = YAxis.AxisType.Value;
                GravityCenterChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
                GravityCenterChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
                GravityCenterChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                GravityCenterChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
                GravityCenterChart.yAxis0.axisLine.symbolWidth = 10;
                GravityCenterChart.yAxis0.axisLine.symbolHeight = 15;
                GravityCenterChart.yAxis0.axisLine.symbolOffset = 0;
                GravityCenterChart.yAxis0.axisLine.symbolDent = 3;
                GravityCenterChart.yAxis0.axisName.show = true;  // 坐标轴名称
                GravityCenterChart.yAxis0.axisName.name = "距离（米）";
                GravityCenterChart.yAxis0.axisName.location = AxisName.Location.Middle;
                GravityCenterChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
                GravityCenterChart.yAxis0.axisName.rotate = 90;
                GravityCenterChart.yAxis0.axisName.color = Color.black;
                GravityCenterChart.yAxis0.axisName.fontSize = 15;
                GravityCenterChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
                GravityCenterChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                GravityCenterChart.yAxis0.axisLabel.fontSize = 12;
                GravityCenterChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                GravityCenterChart.yAxis0.showSplitLine = true;
                GravityCenterChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
                GravityCenterChart.yAxis0.splitArea.show = true;

                GravityCenterChart.RemoveData();
                GravityCenterSerie = GravityCenterChart.AddSerie(SerieType.Line, "重心");//添加折线图

                GravityCenterSerie.symbol.type = SerieSymbolType.None;

                GravityCenterChart.grid.left = 75;
                GravityCenterChart.grid.right = 20;
                GravityCenterChart.grid.top = 50;
                GravityCenterChart.grid.bottom = 25;

                GravityCenterChart.dataZoom.enable = true;
                GravityCenterChart.dataZoom.supportInside = true;
                GravityCenterChart.dataZoom.start = 0;
                GravityCenterChart.dataZoom.end = 100;

                for (int i = 0; i < GravityCenterCount; i++)
                {
                    //print(GravityCenterCount);
                    //chart.AddXAxisData("x" + (i + 1)); 
                    // print(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[i].Coordinate);
                    GravityCenterChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                    GravityCenterChart.AddData(0, 1000 * Vector3.Distance(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[i].Coordinate, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[0].Coordinate));
                }
            }

			

		}



		// Update is called once per frame
		void Update()
		{

		}
	}
}