using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XCharts;

namespace ShipNSea 
{
	public class ReportPanelController : MonoBehaviour
	{
		public Text trainTimeText;
		public Text catchFishCountText;
		public Text distanceText;
		public Text gotExpText;
		public LineChart lineChart;
		public BarChart barChart;
		public RadarChart raderChart;
		public Text adiviceText;
		public Button closeButton;
		public Button printButton;
		public GameObject gameControllerGO;
		private GameController gameController;
		private ScreenShot2Pdf shot2Pdf;
		//adviceCoefficient
		private float completion;
		private float agility;
		private float exercise;
		private float proficiency;
		private float sucessRate;

		public static UnityAction closeBtnFun;
		// Use this for initialization
		void OnEnable()
		{
			gameController = gameControllerGO.GetComponent<GameController>();
			//原本是GoBackToIntro
			closeButton.onClick.AddListener(closeBtnFun);
			trainTimeText.text = "训练时间:" + Mathf.Round(GameController._currentTime);
			catchFishCountText.text = "捕获鱼总数:" + FishFlock.catchFishCount;
			distanceText.text = "移动总路程:" + Mathf.Round(DataCollection.dis);
			gotExpText.text = "获得经验值:" + FishFlock.catchFishCount * 100;
			LineChartShow();
			BarChartShow();
			RadarChartShow();
			AdviceShowFunc();
		}
		void Start()
		{
			gameController = gameControllerGO.GetComponent<GameController>();
			shot2Pdf = GetComponent<ScreenShot2Pdf>();
			MapDetectionController.mapOccupyDis.Clear();
			//closeButton.onClick.AddListener(() => { MapDetectionController.mapOccupyDis.Clear(); SceneManager.LoadScene("Intro"); });
			printButton.onClick.AddListener(() => {
				shot2Pdf.saveImg();
				//ChangeAllColor();
			});
		}
		public void LineChartShow()
		{
			lineChart.title.text = "重心偏移角度(S)";
			lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Default;
			lineChart.RemoveData();
			lineChart.ClearData();

			Serie serie = new Serie();
			serie = lineChart.AddSerie(SerieType.Line, "Line");
			serie.lineType = LineType.Smooth;
			serie.areaStyle.show = true;
			serie.areaStyle.opacity = 0.5f;
			serie.areaStyle.toColor = new Color(244, 240, 230);
			//serie.label.show = true;
			//serie.label.border = false;
			serie.label.color = Color.black;
			serie.symbol.type = SerieSymbolType.Circle;
			serie.symbol.size = 3f;
			serie.symbol.forceShowLast = true;
			for (int i = 0; i < DataCollection.gAngleList.Count; i++)
			{
				lineChart.AddXAxisData((i + 1) + "(S)");
				lineChart.AddData(0, i + 1, (float)Math.Round((double)DataCollection.gAngleList[i], 1));
			}
		}
		void BarChartShow()
		{
			barChart.title.text = "每次成功花费时间(S)";
			barChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Default;
			barChart.ClearData();
			barChart.RemoveData();
			Serie serie = new Serie();
			serie = barChart.AddSerie(SerieType.Bar, "Bar1");
			for (int i = 0; i < FishFlock.fishFlockTimeList.Count; i++)
			{
				barChart.AddXAxisData("第" + (i + 1) + "条");
				barChart.AddData(0, FishFlock.fishFlockTimeList[i]);
			}
		}
		void RadarChartShow()
		{
			raderChart.RemoveRadar();
			raderChart.RemoveData();
			raderChart.title.text = "";
			raderChart.title.subText = "";

			raderChart.legend.show = true;
			raderChart.legend.location.align = Location.Align.TopLeft;
			raderChart.legend.location.top = 60;
			raderChart.legend.location.left = 2;
			raderChart.legend.itemWidth = 70;
			raderChart.legend.itemHeight = 20;
			raderChart.legend.orient = Orient.Vertical;

			raderChart.AddRadar(Radar.Shape.Polygon, new Vector2(0.5f, 0.4f), 0.4f);
			raderChart.AddIndicator(0, "完成度", 0, 100);
			raderChart.AddIndicator(0, "敏捷度", 0, 100);
			raderChart.AddIndicator(0, "运动量", 0, 100);
			raderChart.AddIndicator(0, "熟练度", 0, 100);
			raderChart.AddIndicator(0, "成功率", 0, 100);
			Serie serie = new Serie();
			serie = raderChart.AddSerie(SerieType.Radar, "test");
			serie.radarIndex = 0;
			completion = Mathf.Clamp((float)Math.Round((GameController._currentTime / (float)gameController.totalTime), 1) * 100, 0, 100);
			agility = Mathf.Clamp(BeingAtk.staminaPoint, 0, 100);
			exercise = Mathf.Clamp(DataCollection.dis / (gameController.totalTime * 200f) * 100, 0, 100);
			proficiency = Mathf.Clamp((UserLevenController.exp / 10000f + FishFlock.catchFishCount / (float)SpawnRange.fishCount) * 50, 0, 100);
			sucessRate = Mathf.Clamp(FishFlock.catchFishCount / (float)SpawnRange.fishCount * 100, 0, 100);
			raderChart.AddData(0, new List<float> {
			//完成度
			completion, 
			//敏捷度
			agility,
			//运动量
			exercise,
			//熟练度
			proficiency, 
			//成功率
			sucessRate
		});
		}
		void GoBackToIntro()
		{
			//数据初始化
			FishFlock.catchFishCount = 0;
			FishFlock.fishFlockTimeList.Clear();
			SpawnRange.fishCount = 0;
			DataCollection.dis = 0;
			DataCollection.gAngleList.Clear();
			SceneManager.LoadScene("Intro");
		}

		void AdviceShowFunc()
		{
			float dege = (float)Math.Round((completion + agility + exercise + proficiency + sucessRate) / 5, 1);
			StringBuilder stringBuilder = new StringBuilder("本次得分");
			if (CompareFunc(dege, 60))
			{
				stringBuilder.Append(dege.ToString()).Append(",得分不太理想,加油完成训练");
			}
			else if (CompareFunc(dege, 80))
			{
				stringBuilder.Append(dege.ToString()).Append(",得分良好,继续保持啊");
			}
			else if (CompareFunc(dege, 100))
			{
				stringBuilder.Append(dege.ToString()).Append(",得分优秀,您太优秀了");
			}
			adiviceText.text = stringBuilder.ToString();
		}
		bool CompareFunc(float value, float comp)
		{
			if (value <= comp)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public void ChangeAllColor()
		{
			var images = this.GetComponentsInChildren<Image>();
			foreach (var item in images)
			{
				item.color = Color.white;
			}
		}
	}
}

