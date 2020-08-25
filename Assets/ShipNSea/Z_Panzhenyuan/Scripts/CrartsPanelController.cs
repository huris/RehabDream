using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XCharts;
namespace ShipNSea 
{
	public class CrartsPanelController : MonoBehaviour
	{
		public Text successRateText;
		public Image successRate;
		public Text bestSuccessRateText;
		public Image bestSuccessRare;
		public Text getExpText;
		public Slider buffSlider;
		public Slider expSlider;
		public Text levelText;
		public Text trainTime;
		private float disTemp = 0f;
		private UnityAction upDateEvent;
		public GameObject gameControllerGO;
		private GameController gameController;
		public GameObject reportPanelGO;
		[Header("XCharts组件对象")]
		public LineChart lineChart;
		public RadarChart raderChart;
		public BarChart barChart;
		// Use this for initialization
		void Start()
		{
			buffSlider.value = (float)UserLevenController.exp / (float)UserLevenController.levelE;
			gameController = gameControllerGO.GetComponent<GameController>();
		}

		// Update is called once per frame
		void Update()
		{
			if (upDateEvent != null)
			{
				upDateEvent.Invoke();
			}
		}

		public void DataLoad()
		{
			print(FishFlock.catchFishCount / SpawnRange.fishCount);
			upDateEvent += () => {
				RateAnimation(successRate, successRateText, FishFlock.catchFishCount / (float)SpawnRange.fishCount);
			};
			upDateEvent += () =>
			{
				RateAnimation(bestSuccessRare, bestSuccessRateText, GameController._currentTime / (float)gameController.totalTime);
			};
			upDateEvent += () =>
			{
				LevelBarAnimation(expSlider, buffSlider);
			};
			upDateEvent += () =>
			{
				DistanceShowAnimation();
			};
			LineChartShow();
			BarChartShow();
			LevelBarShow();
			//RadarChartShow();
			Invoke("ShowReportPanel", 8f);
		}

		void RateAnimation(Image rare, Text text, float succParam)
		{
			text.text = (rare.fillAmount * 100).ToString(".#") + "%";
			if (rare.fillAmount >= succParam * 0.999)
			{
				rare.fillAmount = succParam;
				return;
			}
			rare.fillAmount = Mathf.Lerp(rare.fillAmount, succParam, Time.deltaTime * 3);
		}
		void LevelBarAnimation(Slider slider, Slider buffSlider)
		{
			if (slider.value >= buffSlider.value * 0.999)
			{
				slider.value = buffSlider.value;
				return;
			}
			slider.value = Mathf.Lerp(slider.value, buffSlider.value, Time.deltaTime * 3);
		}
		void DistanceShowAnimation()
		{
			disTemp = Mathf.Lerp(disTemp, DataCollection.dis, Time.deltaTime);
			trainTime.text = "运动总路程:" + Mathf.Round(disTemp);
		}


		//XChart展示数据
		/// <summary>
		///LineChart重心偏移角度
		/// </summary>
		void LineChartShow()
		{
			lineChart.title.text = "重心偏移角度(S)";
			lineChart.yAxis0.minMaxType = Axis.AxisMinMaxType.Default;
			lineChart.RemoveData();
			lineChart.ClearData();

			Serie serie = new Serie();
			serie = lineChart.AddSerie(SerieType.Line, "Line");
			serie.lineType = LineType.Smooth;
			serie.symbol.type = SerieSymbolType.Circle;
			serie.symbol.size = 3f;
			serie.symbol.forceShowLast = true;
			serie.areaStyle.show = true;
			serie.areaStyle.opacity = 0.5f;
			for (int i = 0; i < DataCollection.gAngleList.Count; i++)
			{
				lineChart.AddXAxisData((i + 1) + "(S)");
				lineChart.AddData(0, i + 1, DataCollection.gAngleList[i]);
			}
		}



		/// <summary>
		/// 每次成功捕鱼  花费时间
		/// </summary>
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



		/// <summary>
		///用于显示当前等级与经验信息与获得经验值
		/// </summary>
		void LevelBarShow()
		{
			getExpText.text = "获得经验值:" + FishFlock.catchFishCount * 100;
			//当前等级
			levelText.text = UserLevenController.levelStr;
		}



		/// <summary>
		/// 用于雷达图的显示
		/// 完成度: (float)Math.Round((gameController._currentTime / (float)gameController.totalTime),1)训练时长比
		/// 敏捷度: 2000 / (路程/捕鱼数)
		/// 运动量: (总路程)/(totalTime*系数)
		/// 熟练度: 当前经验值/最大经验值
		/// 成功率: FishFlock.catchFishCount/(float)SpawnRange.fishCount
		/// </summary>
		void RadarChartShow()
		{
			raderChart.RemoveRadar();
			raderChart.RemoveData();
			raderChart.title.text = "RadarChart - 雷达图";
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
			raderChart.AddData(0, new List<float> {
			//完成度
			Mathf.Clamp((float)Math.Round((GameController._currentTime / (float)gameController.totalTime),1)*100,0,100), 
			//敏捷度
			Mathf.Clamp(3000/(DataCollection.dis/FishFlock.catchFishCount)*50,0,100),
			//运动量
			Mathf.Clamp(DataCollection.dis/(gameController.totalTime*300f)*100,0,100) ,
			//熟练度
			Mathf.Clamp(UserLevenController.exp/10000f*100,0,100), 
			//成功率
			Mathf.Clamp(FishFlock.catchFishCount/(float)SpawnRange.fishCount*100,0,100)
		});
		}

		void ShowReportPanel()
		{
			reportPanelGO.SetActive(true);
		}
	}
}

