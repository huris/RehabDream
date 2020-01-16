using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class TrainingFeedbackInitScript : MonoBehaviour
    {
        private LineChart RadarAreaChart;   // 雷达图趋势图
        private Serie RadarAreaSerie;

        private LineChart SuccessRateChart;   // 接球率趋势图
        private Serie SuccessRateSerie;

        private LineChart DirectionsChart;   // 各方向极限距离趋势图
        private Serie UponSerie;
        private Serie UponRightSerie;
        private Serie RightSerie;
        private Serie DownRightSerie;
        private Serie DownSerie;
        private Serie DownLeftSerie;
        private Serie LeftSerie;
        private Serie UponLeftSerie;

        private int TrainingCount;

        void OnEnable()
        {
            if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
            {
                //DoctorDataManager.instance.patient.TrainingPlays[DoctorDataManager.instance.patient.TrainingPlays.Count - 1].angles = DoctorDatabaseManager.instance.ReadAngleRecord(DoctorDataManager.instance.patient.TrainingPlays[DoctorDataManager.instance.patient.TrainingPlays.Count - 1].TrainingID);

                TrainingCount = DoctorDataManager.instance.doctor.patient.TrainingPlays.Count;
                //TrainingCount = 10;

                RadarAreaChart = transform.Find("RadarAreaChart").gameObject.GetComponent<LineChart>();
                if (RadarAreaChart == null) RadarAreaChart = transform.Find("RadarAreaChart").gameObject.AddComponent<LineChart>();

                RadarAreaChart.themeInfo.theme = Theme.Light;
                //chart.themeInfo.tooltipBackgroundColor = Color.white;
                //chart.themeInfo.backgroundColor = Color.grey;

                RadarAreaChart.title.show = true;
                RadarAreaChart.title.text = "雷 达 面 积 趋 势 图";
                RadarAreaChart.title.textFontSize = 20;
                RadarAreaChart.title.textStyle.fontStyle = FontStyle.Bold;
                RadarAreaChart.title.location.top = 2;

                //chart.title.subText = "前30s";
                //chart.title.subTextFontSize = 18;

                RadarAreaChart.legend.show = false;
                //RadarAreaChart.legend.location.align = Location.Align.TopRight;
                //RadarAreaChart.legend.location.top = 2;
                //RadarAreaChart.legend.location.right = 55;
                //RadarAreaChart.legend.orient = Orient.Horizonal;  // 图例显示方向
                //RadarAreaChart.legend.itemGap = 0;       // `图例之间的距离
                //RadarAreaChart.legend.itemWidth = 25;
                //RadarAreaChart.legend.itemHeight = 25;

                RadarAreaChart.tooltip.show = true;
                RadarAreaChart.tooltip.type = Tooltip.Type.Line;
                RadarAreaChart.tooltip.titleFormatter = "          第{b}次        ";
                RadarAreaChart.tooltip.itemFormatter = "雷达图面积为{c}";


                RadarAreaChart.xAxis0.show = true;
                RadarAreaChart.xAxis0.type = XAxis.AxisType.Category;
                RadarAreaChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
                RadarAreaChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
                RadarAreaChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                RadarAreaChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
                RadarAreaChart.xAxis0.axisLine.symbolWidth = 10;
                RadarAreaChart.xAxis0.axisLine.symbolHeight = 15;
                RadarAreaChart.xAxis0.axisLine.symbolOffset = 0;
                RadarAreaChart.xAxis0.axisLine.symbolDent = 3;
                RadarAreaChart.xAxis0.axisName.show = true;  // 坐标轴名称
                RadarAreaChart.xAxis0.axisName.name = "时间(次)";
                RadarAreaChart.xAxis0.axisName.location = AxisName.Location.Middle;
                RadarAreaChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
                RadarAreaChart.xAxis0.axisName.rotate = 0;
                RadarAreaChart.xAxis0.axisName.color = Color.black;
                RadarAreaChart.xAxis0.axisName.fontSize = 15;
                RadarAreaChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
                RadarAreaChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                RadarAreaChart.xAxis0.axisLabel.fontSize = 12;
                RadarAreaChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                RadarAreaChart.xAxis0.showSplitLine = true;
                RadarAreaChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
                RadarAreaChart.xAxis0.splitArea.show = true;
                RadarAreaChart.xAxis0.boundaryGap = false;

                RadarAreaChart.yAxis0.show = true;
                RadarAreaChart.yAxis0.type = YAxis.AxisType.Value;
                RadarAreaChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
                RadarAreaChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
                RadarAreaChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                RadarAreaChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
                RadarAreaChart.yAxis0.axisLine.symbolWidth = 10;
                RadarAreaChart.yAxis0.axisLine.symbolHeight = 15;
                RadarAreaChart.yAxis0.axisLine.symbolOffset = 0;
                RadarAreaChart.yAxis0.axisLine.symbolDent = 3;
                RadarAreaChart.yAxis0.axisName.show = true;  // 坐标轴名称
                RadarAreaChart.yAxis0.axisName.name = "面积(平方米)";
                RadarAreaChart.yAxis0.axisName.location = AxisName.Location.Middle;
                RadarAreaChart.yAxis0.axisName.offset = new Vector2(45f, 40f);
                RadarAreaChart.yAxis0.axisName.rotate = 90;
                RadarAreaChart.yAxis0.axisName.color = Color.black;
                RadarAreaChart.yAxis0.axisName.fontSize = 15;
                RadarAreaChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
                RadarAreaChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                RadarAreaChart.yAxis0.axisLabel.fontSize = 12;
                RadarAreaChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                RadarAreaChart.yAxis0.showSplitLine = true;
                RadarAreaChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
                RadarAreaChart.yAxis0.splitArea.show = true;
                RadarAreaChart.yAxis0.axisLabel.formatter = "{value:f1}";

                RadarAreaChart.RemoveData();
                RadarAreaSerie = RadarAreaChart.AddSerie(SerieType.Line, "面积");//添加折线图
                //RightSerie = RadarAreaChart.AddSerie(SerieType.Line, "右");//添加折线

                RadarAreaSerie.areaStyle.show = true;
                RadarAreaSerie.areaStyle.opacity = 0.4f;
                RadarAreaSerie.areaStyle.toColor = Color.white;

                RadarAreaSerie.symbol.type = SerieSymbolType.None;
                //RightSerie.symbol.type = SerieSymbolType.None;

                RadarAreaChart.grid.left = 60;
                RadarAreaChart.grid.right = 20;
                RadarAreaChart.grid.top = 30;
                RadarAreaChart.grid.bottom = 35;

                RadarAreaChart.dataZoom.enable = true;
                RadarAreaChart.dataZoom.supportInside = true;
                RadarAreaChart.dataZoom.start = 0;
                RadarAreaChart.dataZoom.end = 100;
                RadarAreaChart.dataZoom.minShowNum = 30;


                SuccessRateChart = transform.Find("SuccessRateChart").gameObject.GetComponent<LineChart>();
                if (SuccessRateChart == null) SuccessRateChart = transform.Find("SuccessRateChart").gameObject.AddComponent<LineChart>();

                SuccessRateChart.themeInfo.theme = Theme.Light;
                //chart.themeInfo.tooltipBackgroundColor = Color.white;
                //chart.themeInfo.backgroundColor = Color.grey;

                SuccessRateChart.title.show = true;
                SuccessRateChart.title.text = "成 功 接 球 率 趋 势 图";
                SuccessRateChart.title.textFontSize = 20;
                SuccessRateChart.title.textStyle.fontStyle = FontStyle.Bold;
                SuccessRateChart.title.location.top = 2;

                //chart.title.subText = "前30s";
                //chart.title.subTextFontSize = 18;

                SuccessRateChart.legend.show = false;
                //SuccessRateChart.legend.location.align = Location.Align.TopRight;
                //SuccessRateChart.legend.location.top = 2;
                //SuccessRateChart.legend.location.right = 55;
                //SuccessRateChart.legend.orient = Orient.Horizonal;  // 图例显示方向
                //SuccessRateChart.legend.itemGap = 0;       // `图例之间的距离
                //SuccessRateChart.legend.itemWidth = 25;
                //SuccessRateChart.legend.itemHeight = 25;

                SuccessRateChart.tooltip.show = true;
                SuccessRateChart.tooltip.type = Tooltip.Type.Line;
                SuccessRateChart.tooltip.titleFormatter = "          第{b}次        ";
                SuccessRateChart.tooltip.itemFormatter = "成功接球率为{c}%";


                SuccessRateChart.xAxis0.show = true;
                SuccessRateChart.xAxis0.type = XAxis.AxisType.Category;
                SuccessRateChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
                SuccessRateChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
                SuccessRateChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                SuccessRateChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
                SuccessRateChart.xAxis0.axisLine.symbolWidth = 10;
                SuccessRateChart.xAxis0.axisLine.symbolHeight = 15;
                SuccessRateChart.xAxis0.axisLine.symbolOffset = 0;
                SuccessRateChart.xAxis0.axisLine.symbolDent = 3;
                SuccessRateChart.xAxis0.axisName.show = true;  // 坐标轴名称
                SuccessRateChart.xAxis0.axisName.name = "时间(次)";
                SuccessRateChart.xAxis0.axisName.location = AxisName.Location.Middle;
                SuccessRateChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
                SuccessRateChart.xAxis0.axisName.rotate = 0;
                SuccessRateChart.xAxis0.axisName.color = Color.black;
                SuccessRateChart.xAxis0.axisName.fontSize = 15;
                SuccessRateChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
                SuccessRateChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                SuccessRateChart.xAxis0.axisLabel.fontSize = 12;
                SuccessRateChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                SuccessRateChart.xAxis0.showSplitLine = true;
                SuccessRateChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
                SuccessRateChart.xAxis0.splitArea.show = true;
                SuccessRateChart.xAxis0.boundaryGap = false;

                SuccessRateChart.yAxis0.show = true;
                SuccessRateChart.yAxis0.type = YAxis.AxisType.Value;
                SuccessRateChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
                SuccessRateChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
                SuccessRateChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                SuccessRateChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
                SuccessRateChart.yAxis0.axisLine.symbolWidth = 10;
                SuccessRateChart.yAxis0.axisLine.symbolHeight = 15;
                SuccessRateChart.yAxis0.axisLine.symbolOffset = 0;
                SuccessRateChart.yAxis0.axisLine.symbolDent = 3;
                SuccessRateChart.yAxis0.axisName.show = true;  // 坐标轴名称
                SuccessRateChart.yAxis0.axisName.name = "成功接球率(%)";
                SuccessRateChart.yAxis0.axisName.location = AxisName.Location.Middle;
                SuccessRateChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
                SuccessRateChart.yAxis0.axisName.rotate = 90;
                SuccessRateChart.yAxis0.axisName.color = Color.black;
                SuccessRateChart.yAxis0.axisName.fontSize = 15;
                SuccessRateChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
                SuccessRateChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                SuccessRateChart.yAxis0.axisLabel.fontSize = 12;
                SuccessRateChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                SuccessRateChart.yAxis0.showSplitLine = true;
                SuccessRateChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
                SuccessRateChart.yAxis0.splitArea.show = true;
                SuccessRateChart.yAxis0.axisLabel.formatter = "{value:f1}";

                SuccessRateChart.RemoveData();
                SuccessRateSerie = SuccessRateChart.AddSerie(SerieType.Line, "接球率");//添加折线图

                SuccessRateSerie.areaStyle.show = true;
                SuccessRateSerie.areaStyle.opacity = 0.4f;
                SuccessRateSerie.areaStyle.toColor = Color.white;

                SuccessRateSerie.symbol.type = SerieSymbolType.None;

                SuccessRateChart.grid.left = 60;
                SuccessRateChart.grid.right = 20;
                SuccessRateChart.grid.top = 30;
                SuccessRateChart.grid.bottom = 35;

                SuccessRateChart.dataZoom.enable = true;
                SuccessRateChart.dataZoom.supportInside = true;
                SuccessRateChart.dataZoom.start = 0;
                SuccessRateChart.dataZoom.end = 100;
                SuccessRateChart.dataZoom.minShowNum = 30;


                DirectionsChart = transform.Find("DirectionsChart").gameObject.GetComponent<LineChart>();
                if (DirectionsChart == null) DirectionsChart = transform.Find("DirectionsChart").gameObject.AddComponent<LineChart>();

                DirectionsChart.themeInfo.theme = Theme.Light;
                //chart.themeInfo.tooltipBackgroundColor = Color.white;
                //chart.themeInfo.backgroundColor = Color.grey;

                DirectionsChart.title.show = true;
                DirectionsChart.title.text = "方 位 距 离 趋 势 图";
                DirectionsChart.title.textFontSize = 20;
                DirectionsChart.title.textStyle.fontStyle = FontStyle.Bold;
                DirectionsChart.title.location.top = 2;

                //chart.title.subText = "前30s";
                //chart.title.subTextFontSize = 18;

                DirectionsChart.legend.show = false;
                //DirectionsChart.legend.location.align = Location.Align.TopRight;
                //DirectionsChart.legend.location.top = 2;
                //DirectionsChart.legend.location.right = 32;
                //DirectionsChart.legend.orient = Orient.Horizonal;  // 图例显示方向
                //DirectionsChart.legend.itemGap = 0;       // `图例之间的距离
                //DirectionsChart.legend.itemWidth = 35;
                //DirectionsChart.legend.itemHeight = 25;

                DirectionsChart.tooltip.show = true;
                DirectionsChart.tooltip.type = Tooltip.Type.Line;
                DirectionsChart.tooltip.titleFormatter = "       第{b}次      ";
                DirectionsChart.tooltip.itemFormatter = "{a}极限距离为{c}";


                DirectionsChart.xAxis0.show = true;
                DirectionsChart.xAxis0.type = XAxis.AxisType.Category;
                DirectionsChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
                DirectionsChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
                DirectionsChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                DirectionsChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
                DirectionsChart.xAxis0.axisLine.symbolWidth = 10;
                DirectionsChart.xAxis0.axisLine.symbolHeight = 15;
                DirectionsChart.xAxis0.axisLine.symbolOffset = 0;
                DirectionsChart.xAxis0.axisLine.symbolDent = 3;
                DirectionsChart.xAxis0.axisName.show = true;  // 坐标轴名称
                DirectionsChart.xAxis0.axisName.name = "时间(秒)";
                DirectionsChart.xAxis0.axisName.location = AxisName.Location.Middle;
                DirectionsChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
                DirectionsChart.xAxis0.axisName.rotate = 0;
                DirectionsChart.xAxis0.axisName.color = Color.black;
                DirectionsChart.xAxis0.axisName.fontSize = 15;
                DirectionsChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
                DirectionsChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                DirectionsChart.xAxis0.axisLabel.fontSize = 12;
                DirectionsChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                DirectionsChart.xAxis0.showSplitLine = true;
                DirectionsChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
                DirectionsChart.xAxis0.splitArea.show = true;
                DirectionsChart.xAxis0.boundaryGap = false;

                DirectionsChart.yAxis0.show = true;
                DirectionsChart.yAxis0.type = YAxis.AxisType.Value;
                DirectionsChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
                DirectionsChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
                DirectionsChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
                DirectionsChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
                DirectionsChart.yAxis0.axisLine.symbolWidth = 10;
                DirectionsChart.yAxis0.axisLine.symbolHeight = 15;
                DirectionsChart.yAxis0.axisLine.symbolOffset = 0;
                DirectionsChart.yAxis0.axisLine.symbolDent = 3;
                DirectionsChart.yAxis0.axisName.show = true;  // 坐标轴名称
                DirectionsChart.yAxis0.axisName.name = "距离(米)";
                DirectionsChart.yAxis0.axisName.location = AxisName.Location.Middle;
                DirectionsChart.yAxis0.axisName.offset = new Vector2(45f, 30f);
                DirectionsChart.yAxis0.axisName.rotate = 90;
                DirectionsChart.yAxis0.axisName.color = Color.black;
                DirectionsChart.yAxis0.axisName.fontSize = 15;
                DirectionsChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
                DirectionsChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
                DirectionsChart.yAxis0.axisLabel.fontSize = 12;
                DirectionsChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
                DirectionsChart.yAxis0.showSplitLine = true;
                DirectionsChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
                DirectionsChart.yAxis0.splitArea.show = true;
                DirectionsChart.yAxis0.axisLabel.formatter = "{value:f1}";

                DirectionsChart.RemoveData();
                UponSerie = DirectionsChart.AddSerie(SerieType.Line, "正上");//添加折线图
                UponRightSerie = DirectionsChart.AddSerie(SerieType.Line, "右上");//添加折线图
                RightSerie = DirectionsChart.AddSerie(SerieType.Line, "正右");//添加折线图
                DownRightSerie = DirectionsChart.AddSerie(SerieType.Line, "右下");//添加折线图
                DownSerie = DirectionsChart.AddSerie(SerieType.Line, "正下");//添加折线图
                DownLeftSerie = DirectionsChart.AddSerie(SerieType.Line, "左下");//添加折线图
                LeftSerie = DirectionsChart.AddSerie(SerieType.Line, "正左");//添加折线图
                UponLeftSerie = DirectionsChart.AddSerie(SerieType.Line, "左上");//添加折线图

                UponSerie.symbol.type = SerieSymbolType.None;
                UponRightSerie.symbol.type = SerieSymbolType.None;
                RightSerie.symbol.type = SerieSymbolType.None;
                DownRightSerie.symbol.type = SerieSymbolType.None;
                DownSerie.symbol.type = SerieSymbolType.None;
                DownLeftSerie.symbol.type = SerieSymbolType.None;
                LeftSerie.symbol.type = SerieSymbolType.None;
                UponLeftSerie.symbol.type = SerieSymbolType.None;
                //RightSerie.symbol.type = SerieSymbolType.None;

                DirectionsChart.grid.left = 60;
                DirectionsChart.grid.right = 20;
                DirectionsChart.grid.top = 30;
                DirectionsChart.grid.bottom = 35;

                DirectionsChart.dataZoom.enable = true;
                DirectionsChart.dataZoom.supportInside = true;
                DirectionsChart.dataZoom.start = 0;
                DirectionsChart.dataZoom.end = 100;
                DirectionsChart.dataZoom.minShowNum = 30;

                for (int i = 0; i < TrainingCount; i++)
                {
                    //print(AngleCount);
                    //print(DoctorDataManager.instance.patient.TrainingPlays[DoctorDataManager.instance.patient.TrainingPlays.Count - 1].angles[i].TrainingID);
                    //chart.AddXAxisData("x" + (i + 1));
                    //print(TrainingCount);

                    RadarAreaChart.AddXAxisData((i + 1).ToString("0"));
                    RadarAreaChart.AddData(0, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.DirectionRadarArea);

                    //print(DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.GetRadarArea()+"!!!");
                    //RadarAreaChart.AddData(0, i);

                    SuccessRateChart.AddXAxisData((i + 1).ToString("0"));
                    float TrainingSuccessRate = 100.0f * DoctorDataManager.instance.doctor.patient.TrainingPlays[i].SuccessCount / DoctorDataManager.instance.doctor.patient.TrainingPlays[i].GameCount;
                    SuccessRateChart.AddData(0, TrainingSuccessRate);

                    //DirectionsChart.AddXAxisData((i + 1).ToString("0"));
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.UponDirection);
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.UponRightDirection);
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.RightDirection);
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.DownRightDirection);
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.DownDirection);
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.DownLeftDirection);
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.LeftDirection);
                    //DirectionsChart.AddData(0, DoctorDataManager.instance.patient.TrainingPlays[i].direction.UponLeftDirection);

                }
            }
        }



        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}