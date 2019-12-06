using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XCharts
{
    [DisallowMultipleComponent]
    public class JointFeedbackInit : MonoBehaviour
    {
        private LineChart ArmChart;   // 胳膊与躯干
        private Serie LeftArmSerie;
        private Serie RightArmSerie;

        private LineChart ElbowChart;   // 肘部弯曲
        private Serie LeftElbowSerie;
        private Serie RightElbowSerie;

        private LineChart HipChart;    // 髋关节
        private Serie LeftHipSerie;
        private Serie RightHipSerie;
        private Serie HipSerie;

        private LineChart LegChart;    // 腿与躯干
        private Serie LeftLegSerie;
        private Serie RightLegSerie;

        private LineChart KneeChart;    // 膝关节
        private Serie LeftKneeSerie;
        private Serie RightKneeSerie;

        private LineChart AnkleChart;    // 踝关节
        private Serie LeftAnkleSerie;
        private Serie RightAnkleSerie;

        private long AngleCount;

        // Use this for initialization
        void Start()
        {

        }

        void OnEnable()
        {
            if (DoctorDataManager.instance.patient.trainingPlays.Count > 0)
            {
                DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles = DoctorDatabaseManager.instance.ReadAngleRecord(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].TrainingID);
                AngleCount = DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles.Count;
            }


            //SetChart(ref ArmChart, "ArmChart", "胳 膊 与 躯 干", ref LeftArmSerie, ref RightArmSerie); ;
            //SetChart(ref ElbowChart, "ElbowChart", "肘 部 弯 曲", ref LeftElbowSerie, ref RightElbowSerie); ;
            //SetChart(ref LegChart, "LegChart", "腿 部 与 中 垂 线", ref LeftLegSerie, ref RightLegSerie); ;
            //SetChart(ref HipChart, "HipChart", "髋 关 节", ref LeftHipSerie, ref RightHipSerie); ;
            //HipSerie = HipChart.AddSerie(SerieType.Line, "跨部");//添加折线
            //HipSerie.symbol.type = SerieSymbolType.None;
            //SetChart(ref KneeChart, "KneeChart", "膝 关 节", ref LeftKneeSerie, ref RightKneeSerie); ;
            //SetChart(ref AnkleChart, "AnkleChart", "踝 关 节", ref LeftAnkleSerie, ref RightAnkleSerie); ;

            ArmChart = transform.Find("ArmChart").gameObject.GetComponent<LineChart>();
            if (ArmChart == null) ArmChart = transform.Find("ArmChart").gameObject.AddComponent<LineChart>();

            ArmChart.themeInfo.theme = Theme.Light;
            //chart.themeInfo.tooltipBackgroundColor = Color.white;
            //chart.themeInfo.backgroundColor = Color.grey;

            ArmChart.title.show = true;
            ArmChart.title.text = "胳 膊 与 躯 干";
            ArmChart.title.textFontSize = 20;
            ArmChart.title.location.top = 2;

            //chart.title.subText = "前30s";
            //chart.title.subTextFontSize = 18;

            ArmChart.legend.show = true;
            ArmChart.legend.location.align = Location.Align.TopRight;
            ArmChart.legend.location.top = 2;
            ArmChart.legend.location.right = 55;
            ArmChart.legend.orient = Orient.Horizonal;  // 图例显示方向
            ArmChart.legend.itemGap = 0;       // `图例之间的距离
            ArmChart.legend.itemWidth = 25;
            ArmChart.legend.itemHeight = 25;

            ArmChart.tooltip.show = true;
            ArmChart.tooltip.type = Tooltip.Type.Line;
            ArmChart.tooltip.formatter = "   第{b}秒   \n{a0}部为{c0}\n{a1}部为{c1}";

            ArmChart.xAxis0.show = true;
            ArmChart.xAxis0.type = XAxis.AxisType.Category;
            ArmChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
            ArmChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
            ArmChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            ArmChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
            ArmChart.xAxis0.axisLine.symbolWidth = 10;
            ArmChart.xAxis0.axisLine.symbolHeight = 15;
            ArmChart.xAxis0.axisLine.symbolOffset = 0;
            ArmChart.xAxis0.axisLine.symbolDent = 3;
            ArmChart.xAxis0.axisName.show = true;  // 坐标轴名称
            ArmChart.xAxis0.axisName.name = "时间（秒）";
            ArmChart.xAxis0.axisName.location = AxisName.Location.Middle;
            ArmChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
            ArmChart.xAxis0.axisName.rotate = 0;
            ArmChart.xAxis0.axisName.color = Color.black;
            ArmChart.xAxis0.axisName.fontSize = 15;
            ArmChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
            ArmChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            ArmChart.xAxis0.axisLabel.fontSize = 12;
            ArmChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            ArmChart.xAxis0.showSplitLine = true;
            ArmChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
            ArmChart.xAxis0.splitArea.show = true;

            ArmChart.yAxis0.show = true;
            ArmChart.yAxis0.type = YAxis.AxisType.Value;
            ArmChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
            ArmChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
            ArmChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            ArmChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
            ArmChart.yAxis0.axisLine.symbolWidth = 10;
            ArmChart.yAxis0.axisLine.symbolHeight = 15;
            ArmChart.yAxis0.axisLine.symbolOffset = 0;
            ArmChart.yAxis0.axisLine.symbolDent = 3;
            ArmChart.yAxis0.axisName.show = true;  // 坐标轴名称
            ArmChart.yAxis0.axisName.name = "夹角（度）";
            ArmChart.yAxis0.axisName.location = AxisName.Location.Middle;
            ArmChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
            ArmChart.yAxis0.axisName.rotate = 90;
            ArmChart.yAxis0.axisName.color = Color.black;
            ArmChart.yAxis0.axisName.fontSize = 15;
            ArmChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
            ArmChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            ArmChart.yAxis0.axisLabel.fontSize = 12;
            ArmChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            ArmChart.yAxis0.showSplitLine = true;
            ArmChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
            ArmChart.yAxis0.splitArea.show = true;

            ArmChart.RemoveData();
            LeftArmSerie = ArmChart.AddSerie(SerieType.Line, "左");//添加折线图
            RightArmSerie = ArmChart.AddSerie(SerieType.Line, "右");//添加折线

            LeftArmSerie.symbol.type = SerieSymbolType.None;
            RightArmSerie.symbol.type = SerieSymbolType.None;

            ArmChart.grid.left = 60;
            ArmChart.grid.right = 20;
            ArmChart.grid.top = 30;
            ArmChart.grid.bottom = 35;

            ArmChart.dataZoom.enable = true;
            ArmChart.dataZoom.supportInside = true;
            ArmChart.dataZoom.start = 0;
            ArmChart.dataZoom.end = 100;


            ElbowChart = transform.Find("ElbowChart").gameObject.GetComponent<LineChart>();
            if (ElbowChart == null) ElbowChart = transform.Find("ElbowChart").gameObject.AddComponent<LineChart>();

            ElbowChart.themeInfo.theme = Theme.Light;
            //chart.themeInfo.tooltipBackgroundColor = Color.white;
            //chart.themeInfo.backgroundColor = Color.grey;

            ElbowChart.title.show = true;
            ElbowChart.title.text = "肘 部 弯 曲";
            ElbowChart.title.textFontSize = 20;
            ElbowChart.title.location.top = 2;

            //chart.title.subText = "前30s";
            //chart.title.subTextFontSize = 18;

            ElbowChart.legend.show = true;
            ElbowChart.legend.location.align = Location.Align.TopRight;
            ElbowChart.legend.location.top = 2;
            ElbowChart.legend.location.right = 55;
            ElbowChart.legend.orient = Orient.Horizonal;  // 图例显示方向
            ElbowChart.legend.itemGap = 0;       // `图例之间的距离
            ElbowChart.legend.itemWidth = 25;
            ElbowChart.legend.itemHeight = 25;

            ElbowChart.tooltip.show = true;
            ElbowChart.tooltip.type = Tooltip.Type.Line;
            ElbowChart.tooltip.formatter = "   第{b}秒   \n{a0}部为{c0}\n{a1}部为{c1}";

            ElbowChart.xAxis0.show = true;
            ElbowChart.xAxis0.type = XAxis.AxisType.Category;
            ElbowChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
            ElbowChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
            ElbowChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            ElbowChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
            ElbowChart.xAxis0.axisLine.symbolWidth = 10;
            ElbowChart.xAxis0.axisLine.symbolHeight = 15;
            ElbowChart.xAxis0.axisLine.symbolOffset = 0;
            ElbowChart.xAxis0.axisLine.symbolDent = 3;
            ElbowChart.xAxis0.axisName.show = true;  // 坐标轴名称
            ElbowChart.xAxis0.axisName.name = "时间（秒）";
            ElbowChart.xAxis0.axisName.location = AxisName.Location.Middle;
            ElbowChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
            ElbowChart.xAxis0.axisName.rotate = 0;
            ElbowChart.xAxis0.axisName.color = Color.black;
            ElbowChart.xAxis0.axisName.fontSize = 15;
            ElbowChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
            ElbowChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            ElbowChart.xAxis0.axisLabel.fontSize = 12;
            ElbowChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            ElbowChart.xAxis0.showSplitLine = true;
            ElbowChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
            ElbowChart.xAxis0.splitArea.show = true;

            ElbowChart.yAxis0.show = true;
            ElbowChart.yAxis0.type = YAxis.AxisType.Value;
            ElbowChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
            ElbowChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
            ElbowChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            ElbowChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
            ElbowChart.yAxis0.axisLine.symbolWidth = 10;
            ElbowChart.yAxis0.axisLine.symbolHeight = 15;
            ElbowChart.yAxis0.axisLine.symbolOffset = 0;
            ElbowChart.yAxis0.axisLine.symbolDent = 3;
            ElbowChart.yAxis0.axisName.show = true;  // 坐标轴名称
            ElbowChart.yAxis0.axisName.name = "夹角（度）";
            ElbowChart.yAxis0.axisName.location = AxisName.Location.Middle;
            ElbowChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
            ElbowChart.yAxis0.axisName.rotate = 90;
            ElbowChart.yAxis0.axisName.color = Color.black;
            ElbowChart.yAxis0.axisName.fontSize = 15;
            ElbowChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
            ElbowChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            ElbowChart.yAxis0.axisLabel.fontSize = 12;
            ElbowChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            ElbowChart.yAxis0.showSplitLine = true;
            ElbowChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
            ElbowChart.yAxis0.splitArea.show = true;

            ElbowChart.RemoveData();
            LeftElbowSerie = ElbowChart.AddSerie(SerieType.Line, "左");//添加折线图
            RightElbowSerie = ElbowChart.AddSerie(SerieType.Line, "右");//添加折线

            LeftElbowSerie.symbol.type = SerieSymbolType.None;
            RightElbowSerie.symbol.type = SerieSymbolType.None;

            ElbowChart.grid.left = 60;
            ElbowChart.grid.right = 20;
            ElbowChart.grid.top = 30;
            ElbowChart.grid.bottom = 35;

            ElbowChart.dataZoom.enable = true;
            ElbowChart.dataZoom.supportInside = true;
            ElbowChart.dataZoom.start = 0;
            ElbowChart.dataZoom.end = 100;

            LegChart = transform.Find("LegChart").gameObject.GetComponent<LineChart>();
            if (LegChart == null) LegChart = transform.Find("LegChart").gameObject.AddComponent<LineChart>();

            LegChart.themeInfo.theme = Theme.Light;
            //chart.themeInfo.tooltipBackgroundColor = Color.white;
            //chart.themeInfo.backgroundColor = Color.grey;

            LegChart.title.show = true;
            LegChart.title.text = "腿 部 与 中 垂 线";
            LegChart.title.textFontSize = 20;
            LegChart.title.location.top = 2;

            //chart.title.subText = "前30s";
            //chart.title.subTextFontSize = 18;

            LegChart.legend.show = true;
            LegChart.legend.location.align = Location.Align.TopRight;
            LegChart.legend.location.top = 2;
            LegChart.legend.location.right = 55;
            LegChart.legend.orient = Orient.Horizonal;  // 图例显示方向
            LegChart.legend.itemGap = 0;       // `图例之间的距离
            LegChart.legend.itemWidth = 25;
            LegChart.legend.itemHeight = 25;

            LegChart.tooltip.show = true;
            LegChart.tooltip.type = Tooltip.Type.Line;
            LegChart.tooltip.formatter = "   第{b}秒   \n{a0}部为{c0}\n{a1}部为{c1}";

            LegChart.xAxis0.show = true;
            LegChart.xAxis0.type = XAxis.AxisType.Category;
            LegChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
            LegChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
            LegChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            LegChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
            LegChart.xAxis0.axisLine.symbolWidth = 10;
            LegChart.xAxis0.axisLine.symbolHeight = 15;
            LegChart.xAxis0.axisLine.symbolOffset = 0;
            LegChart.xAxis0.axisLine.symbolDent = 3;
            LegChart.xAxis0.axisName.show = true;  // 坐标轴名称
            LegChart.xAxis0.axisName.name = "时间（秒）";
            LegChart.xAxis0.axisName.location = AxisName.Location.Middle;
            LegChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
            LegChart.xAxis0.axisName.rotate = 0;
            LegChart.xAxis0.axisName.color = Color.black;
            LegChart.xAxis0.axisName.fontSize = 15;
            LegChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
            LegChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            LegChart.xAxis0.axisLabel.fontSize = 12;
            LegChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            LegChart.xAxis0.showSplitLine = true;
            LegChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
            LegChart.xAxis0.splitArea.show = true;

            LegChart.yAxis0.show = true;
            LegChart.yAxis0.type = YAxis.AxisType.Value;
            LegChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
            LegChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
            LegChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            LegChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
            LegChart.yAxis0.axisLine.symbolWidth = 10;
            LegChart.yAxis0.axisLine.symbolHeight = 15;
            LegChart.yAxis0.axisLine.symbolOffset = 0;
            LegChart.yAxis0.axisLine.symbolDent = 3;
            LegChart.yAxis0.axisName.show = true;  // 坐标轴名称
            LegChart.yAxis0.axisName.name = "夹角（度）";
            LegChart.yAxis0.axisName.location = AxisName.Location.Middle;
            LegChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
            LegChart.yAxis0.axisName.rotate = 90;
            LegChart.yAxis0.axisName.color = Color.black;
            LegChart.yAxis0.axisName.fontSize = 15;
            LegChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
            LegChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            LegChart.yAxis0.axisLabel.fontSize = 12;
            LegChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            LegChart.yAxis0.showSplitLine = true;
            LegChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
            LegChart.yAxis0.splitArea.show = true;

            LegChart.RemoveData();
            LeftLegSerie = LegChart.AddSerie(SerieType.Line, "左");//添加折线图
            RightLegSerie = LegChart.AddSerie(SerieType.Line, "右");//添加折线

            LeftLegSerie.symbol.type = SerieSymbolType.None;
            RightLegSerie.symbol.type = SerieSymbolType.None;

            LegChart.grid.left = 60;
            LegChart.grid.right = 20;
            LegChart.grid.top = 30;
            LegChart.grid.bottom = 35;

            LegChart.dataZoom.enable = true;
            LegChart.dataZoom.supportInside = true;
            LegChart.dataZoom.start = 0;
            LegChart.dataZoom.end = 100;

            HipChart = transform.Find("HipChart").gameObject.GetComponent<LineChart>();
            if (HipChart == null) HipChart = transform.Find("HipChart").gameObject.AddComponent<LineChart>();

            HipChart.themeInfo.theme = Theme.Light;
            //chart.themeInfo.tooltipBackgroundColor = Color.white;
            //chart.themeInfo.backgroundColor = Color.grey;

            HipChart.title.show = true;
            HipChart.title.text = "髋 关 节";
            HipChart.title.textFontSize = 20;
            HipChart.title.location.top = 2;

            //chart.title.subText = "前30s";
            //chart.title.subTextFontSize = 18;

            HipChart.legend.show = true;
            HipChart.legend.location.align = Location.Align.TopRight;
            HipChart.legend.location.top = 2;
            HipChart.legend.location.right = 32;
            HipChart.legend.orient = Orient.Horizonal;  // 图例显示方向
            HipChart.legend.itemGap = 0;       // `图例之间的距离
            HipChart.legend.itemWidth = 35;
            HipChart.legend.itemHeight = 25;

            HipChart.tooltip.show = true;
            HipChart.tooltip.type = Tooltip.Type.Line;
            HipChart.tooltip.formatter = "   第{b}秒   \n{a0}部为{c0}\n{a1}部为{c1}";

            HipChart.xAxis0.show = true;
            HipChart.xAxis0.type = XAxis.AxisType.Category;
            HipChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
            HipChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
            HipChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            HipChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
            HipChart.xAxis0.axisLine.symbolWidth = 10;
            HipChart.xAxis0.axisLine.symbolHeight = 15;
            HipChart.xAxis0.axisLine.symbolOffset = 0;
            HipChart.xAxis0.axisLine.symbolDent = 3;
            HipChart.xAxis0.axisName.show = true;  // 坐标轴名称
            HipChart.xAxis0.axisName.name = "时间（秒）";
            HipChart.xAxis0.axisName.location = AxisName.Location.Middle;
            HipChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
            HipChart.xAxis0.axisName.rotate = 0;
            HipChart.xAxis0.axisName.color = Color.black;
            HipChart.xAxis0.axisName.fontSize = 15;
            HipChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
            HipChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            HipChart.xAxis0.axisLabel.fontSize = 12;
            HipChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            HipChart.xAxis0.showSplitLine = true;
            HipChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
            HipChart.xAxis0.splitArea.show = true;

            HipChart.yAxis0.show = true;
            HipChart.yAxis0.type = YAxis.AxisType.Value;
            HipChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
            HipChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
            HipChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            HipChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
            HipChart.yAxis0.axisLine.symbolWidth = 10;
            HipChart.yAxis0.axisLine.symbolHeight = 15;
            HipChart.yAxis0.axisLine.symbolOffset = 0;
            HipChart.yAxis0.axisLine.symbolDent = 3;
            HipChart.yAxis0.axisName.show = true;  // 坐标轴名称
            HipChart.yAxis0.axisName.name = "夹角（度）";
            HipChart.yAxis0.axisName.location = AxisName.Location.Middle;
            HipChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
            HipChart.yAxis0.axisName.rotate = 90;
            HipChart.yAxis0.axisName.color = Color.black;
            HipChart.yAxis0.axisName.fontSize = 15;
            HipChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
            HipChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            HipChart.yAxis0.axisLabel.fontSize = 12;
            HipChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            HipChart.yAxis0.showSplitLine = true;
            HipChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
            HipChart.yAxis0.splitArea.show = true;

            HipChart.RemoveData();
            LeftHipSerie = HipChart.AddSerie(SerieType.Line, "左");//添加折线图
            RightHipSerie = HipChart.AddSerie(SerieType.Line, "右");//添加折线
            HipSerie = HipChart.AddSerie(SerieType.Line, "胯部");//添加折线

            LeftHipSerie.symbol.type = SerieSymbolType.None;
            RightHipSerie.symbol.type = SerieSymbolType.None;
            HipSerie.symbol.type = SerieSymbolType.None;

            HipChart.grid.left = 60;
            HipChart.grid.right = 20;
            HipChart.grid.top = 30;
            HipChart.grid.bottom = 35;

            HipChart.dataZoom.enable = true;
            HipChart.dataZoom.supportInside = true;
            HipChart.dataZoom.start = 0;
            HipChart.dataZoom.end = 100;

            KneeChart = transform.Find("KneeChart").gameObject.GetComponent<LineChart>();
            if (KneeChart == null) KneeChart = transform.Find("KneeChart").gameObject.AddComponent<LineChart>();

            KneeChart.themeInfo.theme = Theme.Light;
            //chart.themeInfo.tooltipBackgroundColor = Color.white;
            //chart.themeInfo.backgroundColor = Color.grey;

            KneeChart.title.show = true;
            KneeChart.title.text = "膝 关 节";
            KneeChart.title.textFontSize = 20;
            KneeChart.title.location.top = 2;

            //chart.title.subText = "前30s";
            //chart.title.subTextFontSize = 18;

            KneeChart.legend.show = true;
            KneeChart.legend.location.align = Location.Align.TopRight;
            KneeChart.legend.location.top = 2;
            KneeChart.legend.location.right = 55;
            KneeChart.legend.orient = Orient.Horizonal;  // 图例显示方向
            KneeChart.legend.itemGap = 0;       // `图例之间的距离
            KneeChart.legend.itemWidth = 25;
            KneeChart.legend.itemHeight = 25;

            KneeChart.tooltip.show = true;
            KneeChart.tooltip.type = Tooltip.Type.Line;
            KneeChart.tooltip.formatter = "   第{b}秒   \n{a0}部为{c0}\n{a1}部为{c1}";

            KneeChart.xAxis0.show = true;
            KneeChart.xAxis0.type = XAxis.AxisType.Category;
            KneeChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
            KneeChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
            KneeChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            KneeChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
            KneeChart.xAxis0.axisLine.symbolWidth = 10;
            KneeChart.xAxis0.axisLine.symbolHeight = 15;
            KneeChart.xAxis0.axisLine.symbolOffset = 0;
            KneeChart.xAxis0.axisLine.symbolDent = 3;
            KneeChart.xAxis0.axisName.show = true;  // 坐标轴名称
            KneeChart.xAxis0.axisName.name = "时间（秒）";
            KneeChart.xAxis0.axisName.location = AxisName.Location.Middle;
            KneeChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
            KneeChart.xAxis0.axisName.rotate = 0;
            KneeChart.xAxis0.axisName.color = Color.black;
            KneeChart.xAxis0.axisName.fontSize = 15;
            KneeChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
            KneeChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            KneeChart.xAxis0.axisLabel.fontSize = 12;
            KneeChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            KneeChart.xAxis0.showSplitLine = true;
            KneeChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
            KneeChart.xAxis0.splitArea.show = true;

            KneeChart.yAxis0.show = true;
            KneeChart.yAxis0.type = YAxis.AxisType.Value;
            KneeChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
            KneeChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
            KneeChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            KneeChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
            KneeChart.yAxis0.axisLine.symbolWidth = 10;
            KneeChart.yAxis0.axisLine.symbolHeight = 15;
            KneeChart.yAxis0.axisLine.symbolOffset = 0;
            KneeChart.yAxis0.axisLine.symbolDent = 3;
            KneeChart.yAxis0.axisName.show = true;  // 坐标轴名称
            KneeChart.yAxis0.axisName.name = "夹角（度）";
            KneeChart.yAxis0.axisName.location = AxisName.Location.Middle;
            KneeChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
            KneeChart.yAxis0.axisName.rotate = 90;
            KneeChart.yAxis0.axisName.color = Color.black;
            KneeChart.yAxis0.axisName.fontSize = 15;
            KneeChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
            KneeChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            KneeChart.yAxis0.axisLabel.fontSize = 12;
            KneeChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            KneeChart.yAxis0.showSplitLine = true;
            KneeChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
            KneeChart.yAxis0.splitArea.show = true;

            KneeChart.RemoveData();
            LeftKneeSerie = KneeChart.AddSerie(SerieType.Line, "左");//添加折线图
            RightKneeSerie = KneeChart.AddSerie(SerieType.Line, "右");//添加折线

            LeftKneeSerie.symbol.type = SerieSymbolType.None;
            RightKneeSerie.symbol.type = SerieSymbolType.None;

            KneeChart.grid.left = 60;
            KneeChart.grid.right = 20;
            KneeChart.grid.top = 30;
            KneeChart.grid.bottom = 35;

            KneeChart.dataZoom.enable = true;
            KneeChart.dataZoom.supportInside = true;
            KneeChart.dataZoom.start = 0;
            KneeChart.dataZoom.end = 100;


            AnkleChart = transform.Find("AnkleChart").gameObject.GetComponent<LineChart>();
            if (AnkleChart == null) AnkleChart = transform.Find("AnkleChart").gameObject.AddComponent<LineChart>();

            AnkleChart.themeInfo.theme = Theme.Light;
            //chart.themeInfo.tooltipBackgroundColor = Color.white;
            //chart.themeInfo.backgroundColor = Color.grey;

            AnkleChart.title.show = true;
            AnkleChart.title.text = "踝 关 节";
            AnkleChart.title.textFontSize = 20;
            AnkleChart.title.location.top = 2;

            //chart.title.subText = "前30s";
            //chart.title.subTextFontSize = 18;

            AnkleChart.legend.show = true;
            AnkleChart.legend.location.align = Location.Align.TopRight;
            AnkleChart.legend.location.top = 2;
            AnkleChart.legend.location.right = 55;
            AnkleChart.legend.orient = Orient.Horizonal;  // 图例显示方向
            AnkleChart.legend.itemGap = 0;       // `图例之间的距离
            AnkleChart.legend.itemWidth = 25;
            AnkleChart.legend.itemHeight = 25;

            AnkleChart.tooltip.show = true;
            AnkleChart.tooltip.type = Tooltip.Type.Line;
            AnkleChart.tooltip.formatter = "   第{b}秒   \n{a0}部为{c0}\n{a1}部为{c1}";

            AnkleChart.xAxis0.show = true;
            AnkleChart.xAxis0.type = XAxis.AxisType.Category;
            AnkleChart.xAxis0.splitNumber = 10;   // 把数据分成多少份
            AnkleChart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
            AnkleChart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            AnkleChart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
            AnkleChart.xAxis0.axisLine.symbolWidth = 10;
            AnkleChart.xAxis0.axisLine.symbolHeight = 15;
            AnkleChart.xAxis0.axisLine.symbolOffset = 0;
            AnkleChart.xAxis0.axisLine.symbolDent = 3;
            AnkleChart.xAxis0.axisName.show = true;  // 坐标轴名称
            AnkleChart.xAxis0.axisName.name = "时间（秒）";
            AnkleChart.xAxis0.axisName.location = AxisName.Location.Middle;
            AnkleChart.xAxis0.axisName.offset = new Vector2(0f, 25f);
            AnkleChart.xAxis0.axisName.rotate = 0;
            AnkleChart.xAxis0.axisName.color = Color.black;
            AnkleChart.xAxis0.axisName.fontSize = 15;
            AnkleChart.xAxis0.axisName.fontStyle = FontStyle.Normal;
            AnkleChart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            AnkleChart.xAxis0.axisLabel.fontSize = 12;
            AnkleChart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            AnkleChart.xAxis0.showSplitLine = true;
            AnkleChart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
            AnkleChart.xAxis0.splitArea.show = true;

            AnkleChart.yAxis0.show = true;
            AnkleChart.yAxis0.type = YAxis.AxisType.Value;
            AnkleChart.yAxis0.splitNumber = 10;   // 把数据分成多少份
            AnkleChart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
            AnkleChart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
            AnkleChart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
            AnkleChart.yAxis0.axisLine.symbolWidth = 10;
            AnkleChart.yAxis0.axisLine.symbolHeight = 15;
            AnkleChart.yAxis0.axisLine.symbolOffset = 0;
            AnkleChart.yAxis0.axisLine.symbolDent = 3;
            AnkleChart.yAxis0.axisName.show = true;  // 坐标轴名称
            AnkleChart.yAxis0.axisName.name = "夹角（度）";
            AnkleChart.yAxis0.axisName.location = AxisName.Location.Middle;
            AnkleChart.yAxis0.axisName.offset = new Vector2(45f, 50f);
            AnkleChart.yAxis0.axisName.rotate = 90;
            AnkleChart.yAxis0.axisName.color = Color.black;
            AnkleChart.yAxis0.axisName.fontSize = 15;
            AnkleChart.yAxis0.axisName.fontStyle = FontStyle.Normal;
            AnkleChart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
            AnkleChart.yAxis0.axisLabel.fontSize = 12;
            AnkleChart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
            AnkleChart.yAxis0.showSplitLine = true;
            AnkleChart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
            AnkleChart.yAxis0.splitArea.show = true;

            AnkleChart.RemoveData();
            LeftAnkleSerie = AnkleChart.AddSerie(SerieType.Line, "左");//添加折线图
            RightAnkleSerie = AnkleChart.AddSerie(SerieType.Line, "右");//添加折线

            LeftAnkleSerie.symbol.type = SerieSymbolType.None;
            RightAnkleSerie.symbol.type = SerieSymbolType.None;

            AnkleChart.grid.left = 60;
            AnkleChart.grid.right = 20;
            AnkleChart.grid.top = 30;
            AnkleChart.grid.bottom = 35;

            AnkleChart.dataZoom.enable = true;
            AnkleChart.dataZoom.supportInside = true;
            AnkleChart.dataZoom.start = 0;
            AnkleChart.dataZoom.end = 100;

            for (int i = 0; i < AngleCount; i++)
            {
                //print(AngleCount);
                //print(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].TrainingID);
                //chart.AddXAxisData("x" + (i + 1));
                ArmChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                ArmChart.AddData(0, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].LeftArmAngle);
                ArmChart.AddData(1, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].RightArmAngle);

                ElbowChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                ElbowChart.AddData(0, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].LeftElbowAngle);
                ElbowChart.AddData(1, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].RightElbowAngle);

                LegChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                LegChart.AddData(0, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].LeftLegAngle);
                LegChart.AddData(1, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].RightLegAngle);

                HipChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                HipChart.AddData(0, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].LeftHipAngle);
                HipChart.AddData(1, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].RightHipAngle);
                HipChart.AddData(2, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].HipAngle);

                KneeChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                KneeChart.AddData(0, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].LeftKneeAngle);
                KneeChart.AddData(1, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].RightKneeAngle);

                AnkleChart.AddXAxisData((i * 0.2f).ToString("0.0"));
                //print(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].LeftAnkleAngle);
                AnkleChart.AddData(0, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].LeftAnkleAngle);
                AnkleChart.AddData(1, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles[i].RightAnkleAngle);
            }
        }

        //public void SetChart(ref LineChart Chart, string ChartName, string TitleName, ref Serie LeftSerie, ref Serie RightSerie)
        //{
        //    Chart = transform.Find(ChartName).gameObject.GetComponent<LineChart>();
        //    if (Chart == null) Chart = transform.Find(ChartName).gameObject.AddComponent<LineChart>();

        //    Chart.themeInfo.theme = Theme.Light;
        //    //chart.themeInfo.tooltipBackgroundColor = Color.white;
        //    //chart.themeInfo.backgroundColor = Color.grey;

        //    Chart.title.show = true;
        //    Chart.title.text = TitleName;
        //    Chart.title.textFontSize = 20;
        //    Chart.title.location.top = 2;

        //    //chart.title.subText = "前30s";
        //    //chart.title.subTextFontSize = 18;

        //    Chart.legend.show = true;
        //    Chart.legend.location.align = Location.Align.TopRight;
        //    Chart.legend.location.top = 2;
        //    Chart.legend.location.right = 55;
        //    Chart.legend.orient = Orient.Horizonal;  // 图例显示方向
        //    Chart.legend.itemGap = 0;       // `图例之间的距离
        //    Chart.legend.itemWidth = 25;
        //    Chart.legend.itemHeight = 25;

        //    Chart.tooltip.show = true;
        //    Chart.tooltip.type = Tooltip.Type.Line;
        //    Chart.tooltip.formatter = "   第{b}秒   \n{a0}部为{c0}\n{a1}部为{c1}";

        //    Chart.xAxis0.show = true;
        //    Chart.xAxis0.type = XAxis.AxisType.Category;
        //    Chart.xAxis0.splitNumber = 10;   // 把数据分成多少份
        //    Chart.xAxis0.boundaryGap = true;   // 坐标轴两边是否留白
        //    Chart.xAxis0.axisLine.width = 1;    // 坐标轴轴线宽
        //    Chart.xAxis0.axisLine.symbol = true;    // 坐标轴箭头
        //    Chart.xAxis0.axisLine.symbolWidth = 10;
        //    Chart.xAxis0.axisLine.symbolHeight = 15;
        //    Chart.xAxis0.axisLine.symbolOffset = 0;
        //    Chart.xAxis0.axisLine.symbolDent = 3;
        //    Chart.xAxis0.axisName.show = true;  // 坐标轴名称
        //    Chart.xAxis0.axisName.name = "时间（秒）";
        //    Chart.xAxis0.axisName.location = AxisName.Location.Middle;
        //    Chart.xAxis0.axisName.offset = new Vector2(0f, 25f);
        //    Chart.xAxis0.axisName.rotate = 0;
        //    Chart.xAxis0.axisName.color = Color.black;
        //    Chart.xAxis0.axisName.fontSize = 15;
        //    Chart.xAxis0.axisName.fontStyle = FontStyle.Normal;
        //    Chart.xAxis0.axisTick.inside = true;    // 坐标轴是否朝内
        //    Chart.xAxis0.axisLabel.fontSize = 12;
        //    Chart.xAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
        //    Chart.xAxis0.showSplitLine = true;
        //    Chart.xAxis0.splitLineType = Axis.SplitLineType.Solid;
        //    Chart.xAxis0.splitArea.show = true;

        //    Chart.yAxis0.show = true;
        //    Chart.yAxis0.type = YAxis.AxisType.Value;
        //    Chart.yAxis0.splitNumber = 10;   // 把数据分成多少份
        //    Chart.yAxis0.boundaryGap = false;   // 坐标轴两边是否留白
        //    Chart.yAxis0.axisLine.width = 1;    // 坐标轴轴线宽
        //    Chart.yAxis0.axisLine.symbol = true;    // 坐标轴箭头
        //    Chart.yAxis0.axisLine.symbolWidth = 10;
        //    Chart.yAxis0.axisLine.symbolHeight = 15;
        //    Chart.yAxis0.axisLine.symbolOffset = 0;
        //    Chart.yAxis0.axisLine.symbolDent = 3;
        //    Chart.yAxis0.axisName.show = true;  // 坐标轴名称
        //    Chart.yAxis0.axisName.name = "夹角（度）";
        //    Chart.yAxis0.axisName.location = AxisName.Location.Middle;
        //    Chart.yAxis0.axisName.offset = new Vector2(45f, 50f);
        //    Chart.yAxis0.axisName.rotate = 90;
        //    Chart.yAxis0.axisName.color = Color.black;
        //    Chart.yAxis0.axisName.fontSize = 15;
        //    Chart.yAxis0.axisName.fontStyle = FontStyle.Normal;
        //    Chart.yAxis0.axisTick.inside = true;    // 坐标轴是否朝内
        //    Chart.yAxis0.axisLabel.fontSize = 12;
        //    Chart.yAxis0.axisLabel.margin = 4;  // 标签与轴线的距离
        //    Chart.yAxis0.showSplitLine = true;
        //    Chart.yAxis0.splitLineType = Axis.SplitLineType.Solid;
        //    Chart.yAxis0.splitArea.show = true;

        //    Chart.RemoveData();
        //    LeftSerie = Chart.AddSerie(SerieType.Line, "左");//添加折线图
        //    RightSerie = Chart.AddSerie(SerieType.Line, "右");//添加折线

        //    LeftSerie.symbol.type = SerieSymbolType.None;
        //    RightSerie.symbol.type = SerieSymbolType.None;

        //    Chart.grid.left = 60;
        //    Chart.grid.right = 20;
        //    Chart.grid.top = 30;
        //    Chart.grid.bottom = 30;

        //    Chart.dataZoom.enable = true;
        //    Chart.dataZoom.supportInside = true;
        //    Chart.dataZoom.start = 0;
        //    Chart.dataZoom.end = 100;
        //}

        // Update is called once per frame
        void Update()
        {

        }
    }
}