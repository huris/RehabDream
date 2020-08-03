using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

namespace XCharts
{
	[DisallowMultipleComponent]
	public class EvaluationChartType : MonoBehaviour
	{

		public Toggle SoccerChartToggle;
		public Toggle GCTrackToggle;

		public Image SoccerChartImage;
		public Image GCTrackImage;

		public Text SoccerChartText;
		public Text GCTrackText;

		public GameObject SoccerChart;
		public GameObject GCTrack;

		public int SingleEvaluation;

		// SoccerChart
		public BarChart SoccerBar;
		public Serie DistanceSerie, TimeSerie;


		// GCLine
		public VectorLine GCLine;
		public VectorLine GCLineComplete;

		public Vector2 GravityDiff;

		public List<Point> tempGCPoints;  // 临时用于画凸包的点集
		public List<Point> tempGCPointsComplete;
		public ConvexHull GCconvexHull;   // 新建一个凸包

		public VectorLine GCConvexHullLine;   // 凸包线
		public VectorLine GCConvexHullArea;   // 画凸包透明面积
		public bool GCConvexHullIsDraw;
		public Color[] GCConvexHullColors;    // 凸包顶点颜色
		public Color ConvexHullLineColor;

		public GameObject RedArrow;

		public Vector2 RadarPos;

		public bool TrackIsDraw;

		public Text TrackFastText;

		public IEnumerator DrawNoDirect;

		public Image OffsetArrangeImage;
		public Text OffsetArrangeText;
		public Toggle OffsetArrange;

		// Use this for initialization

		void OnEnable()
		{
			if(DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
			{
				SoccerChartToggle.isOn = true;

				SingleEvaluation = DoctorDataManager.instance.doctor.patient.EvaluationIndex;

				RadarPos = new Vector2(801f, 154f);

				TrackIsDraw = false;

				DrawNoDirect = DrawGCTrack();
			}
		}

		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (GCTrackToggle.isOn)
				{
					TrackFastText.color = new Color32(110, 173, 220, 0);

					StopCoroutine(DrawNoDirect);
					DrawGCTrackComplete();

					//DrawColorFistTrack();
					//StopCoroutine("DrawColorFistLine");
				}
			}
		}

		public void OffsetArrangeToggleIsOn()
		{
			if (OffsetArrange.isOn)
			{
				OffsetArrangeImage.color = new Color32(85, 170, 173, 255);
				OffsetArrangeText.color = new Color32(255, 255, 255, 255);

				StartCoroutine(DrawGCConvexHull());
			}
			else
			{
				OffsetArrangeImage.color = new Color32(233, 239, 244, 255);
				OffsetArrangeText.color = new Color32(165, 165, 165, 255);

				VectorLine.Destroy(ref GCConvexHullLine);
				VectorLine.Destroy(ref GCConvexHullArea);
			}
		}

		public void SoccerChartToggleIsOn()
		{
			if (SoccerChartToggle.isOn)
			{
				RemoveGCLine();

				SoccerChartImage.color = new Color32(88, 181, 140, 255);
				GCTrackImage.color = new Color32(233, 239, 244, 255);

				SoccerChartText.color = new Color32(255, 255, 255, 255);
				GCTrackText.color = new Color32(165, 165, 165, 255);

				GCTrack.SetActive(false);
				SoccerChart.SetActive(true);

				// SoccerSpeedAndTime

				SoccerBar = transform.parent.Find("BarChart").gameObject.GetComponent<BarChart>();
				if (SoccerBar == null) SoccerBar = transform.parent.Find("BarChart").gameObject.AddComponent<BarChart>();

				// 写入数据
				SoccerBar.series.UpdateData(0, 0, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerDistance);
				SoccerBar.series.UpdateData(0, 1, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerDistance);
				SoccerBar.series.UpdateData(0, 2, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerDistance);
				SoccerBar.series.UpdateData(0, 3, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerDistance);
				SoccerBar.series.UpdateData(0, 4, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerDistance);
				SoccerBar.series.UpdateData(0, 5, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerDistance);
				SoccerBar.series.UpdateData(0, 6, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerDistance);
				SoccerBar.series.UpdateData(0, 7, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerDistance);
				SoccerBar.series.UpdateData(0, 8, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerDistance);
				SoccerBar.series.UpdateData(0, 9, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerDistance);

				SoccerBar.series.UpdateData(1, 0, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponSoccerTime);
				SoccerBar.series.UpdateData(1, 1, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponRightSoccerTime);
				SoccerBar.series.UpdateData(1, 2, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.RightSoccerTime);
				SoccerBar.series.UpdateData(1, 3, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownRightSoccerTime);
				SoccerBar.series.UpdateData(1, 4, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownSoccerTime);
				SoccerBar.series.UpdateData(1, 5, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.DownLeftSoccerTime);
				SoccerBar.series.UpdateData(1, 6, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.LeftSoccerTime);
				SoccerBar.series.UpdateData(1, 7, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.UponLeftSoccerTime);
				SoccerBar.series.UpdateData(1, 8, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.FrontSoccerTime);
				SoccerBar.series.UpdateData(1, 9, 1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerScore / DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].soccerDistance.BehindSoccerTime);

				SoccerBar.RefreshChart();
			}
		}

		public void GCTrackToggleIsOn()
		{
			if (GCTrackToggle.isOn)
			{
				SoccerChartImage.color = new Color32(233, 239, 244, 255);
				GCTrackImage.color = new Color32(88, 181, 140, 255);

				SoccerChartText.color = new Color32(165, 165, 165, 255);
				GCTrackText.color = new Color32(255, 255, 255, 255);

				SoccerChart.SetActive(false);
				GCTrack.SetActive(true);

				OffsetArrange.isOn = false;

				StartDrawGCTrack();

			}
		}

		public void StartDrawGCTrack()
		{
			if(TrackIsDraw || DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters.Count == 0)
			{
				return;
			}


			TrackIsDraw = true;

			TrackFastText.color = new Color32(110, 173, 220, 255);

			StartCoroutine(DrawNoDirect);
		}

		public void DrawGCTrackComplete()
		{

			GCLineComplete = new VectorLine("GCLineComplete", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
			GCLineComplete.smoothColor = false;   // 设置平滑颜色
			GCLineComplete.smoothWidth = false;   // 设置平滑宽度
			//GCLineComplete.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

			tempGCPointsComplete = new List<Point>();

			//GravityDiff = new Vector2(RadarPos.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[0].Coordinate.x, RadarPos.y - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[0].Coordinate.y);

			tempGCPointsComplete.Add(new Point(RadarPos.x, RadarPos.y));
			GCLineComplete.points2.Add(new Vector2(RadarPos.x, RadarPos.y));

			for (int i = 1; i < DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters.Count; i++)
			{
				tempGCPointsComplete.Add(new Point(DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[i].Coordinate.x, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[i].Coordinate.y));
			}

			for (int i = 1; i < tempGCPointsComplete.Count; i++)
			{
				tempGCPointsComplete[i].x += GravityDiff.x;
				tempGCPointsComplete[i].y += GravityDiff.y;

				tempGCPointsComplete[i].x = GCLineComplete.points2[0].x + (tempGCPointsComplete[i].x - GCLineComplete.points2[0].x) * 250f / 344f;
				tempGCPointsComplete[i].y = GCLineComplete.points2[0].y + (tempGCPointsComplete[i].y - GCLineComplete.points2[0].y) * 250f / 344f;

				GCLineComplete.points2.Add(new Vector2(tempGCPointsComplete[i].x, tempGCPointsComplete[i].y));

				int DeltaColorR = 0, DeltaColorG = 0;

				int DeltaBase = (int)((GCLineComplete.points2[GCLineComplete.points2.Count - 2] - GCLineComplete.points2[GCLineComplete.points2.Count - 1]).magnitude * 344f / 250f * 40f);

				if (DeltaBase <= 0) { DeltaColorR = 0; DeltaColorG = 0; }
				else if (DeltaBase > 0 && DeltaBase <= 255) { DeltaColorR = DeltaBase; DeltaColorG = 0; }
				else if (DeltaBase > 255 && DeltaBase <= 510) { DeltaColorR = 255; DeltaColorG = DeltaBase - 255; }
				else if (DeltaBase > 510) { DeltaColorR = 255; DeltaColorG = 255; }

				GCLineComplete.SetColor(new Color32((Byte)DeltaColorR, (Byte)(255 - DeltaColorG), 0, (Byte)255), GCLineComplete.points2.Count - 2);
			}
			RedArrow.transform.position = new Vector3(tempGCPointsComplete[tempGCPointsComplete.Count - 1].x, tempGCPointsComplete[tempGCPointsComplete.Count - 1].y, 0f);

			GCLineComplete.Draw();
		}


		IEnumerator DrawGCTrack()
		{
			GCLine = new VectorLine("GCLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);
			GCLine.smoothColor = false;   // 设置平滑颜色
			GCLine.smoothWidth = false;   // 设置平滑宽度
			GCLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

			tempGCPoints = new List<Point>();

			GravityDiff = new Vector2(RadarPos.x - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[0].Coordinate.x, RadarPos.y - DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[0].Coordinate.y);

			tempGCPoints.Add(new Point(RadarPos.x, RadarPos.y));
			GCLine.points2.Add(new Vector2(RadarPos.x, RadarPos.y));

			for (int i = 1; i < DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters.Count; i++)
			{
				tempGCPoints.Add(new Point(DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[i].Coordinate.x, DoctorDataManager.instance.doctor.patient.Evaluations[SingleEvaluation].GravityCenters[i].Coordinate.y));
			}
			//print(tempGCPoints[1].x + tempGCPoints[1].y);

			//for (int i = 0; i < 20; i++)
			//{
			//	print(tempGCPoints[i].x + " !!! " + tempGCPoints[i].y);
			//}

			for (int i = 1; i < tempGCPoints.Count; i++)
			{
				tempGCPoints[i].x += GravityDiff.x;
				tempGCPoints[i].y += GravityDiff.y;

				tempGCPoints[i].x = GCLine.points2[0].x + (tempGCPoints[i].x - GCLine.points2[0].x) * 250f / 344f;
				tempGCPoints[i].y = GCLine.points2[0].y + (tempGCPoints[i].y - GCLine.points2[0].y) * 250f / 344f;

				//if(i < 20)
				//{
				//	print(tempGCPoints[i].x + "  " + tempGCPoints[i].y);
				//}

				GCLine.points2.Add(new Vector2(tempGCPoints[i].x, tempGCPoints[i].y));

				int DeltaColorR = 0, DeltaColorG = 0;

				int DeltaBase = (int)((GCLine.points2[GCLine.points2.Count - 2] - GCLine.points2[GCLine.points2.Count - 1]).magnitude * 344f / 250f * 40f);

				if (DeltaBase <= 0) { DeltaColorR = 0; DeltaColorG = 0; }
				else if (DeltaBase > 0 && DeltaBase <= 255) { DeltaColorR = DeltaBase; DeltaColorG = 0; }
				else if (DeltaBase > 255 && DeltaBase <= 510) { DeltaColorR = 255; DeltaColorG = DeltaBase - 255; }
				else if (DeltaBase > 510) { DeltaColorR = 255; DeltaColorG = 255; }

				GCLine.SetColor(new Color32((Byte)DeltaColorR, (Byte)(255 - DeltaColorG), 0, (Byte)255), GCLine.points2.Count - 2);

				GCLine.Draw();
				RedArrow.transform.position = new Vector3(tempGCPoints[i].x, tempGCPoints[i].y, 0f);

				yield return new WaitForSeconds(0.01f);
			}

			TrackFastText.color = new Color32(110, 173, 220, 0);

		}

		IEnumerator DrawGCConvexHull()
		{

			GCconvexHull = new ConvexHull(tempGCPoints);

			//print(tempGCPoints);

			// 画凸包圈
			GCConvexHullLine = new VectorLine("GCConvexHullLine", new List<Vector2>(), 2.0f, Vectrosity.LineType.Continuous, Joins.Weld);

			GCConvexHullLine.smoothColor = false;   // 设置平滑颜色
			GCConvexHullLine.smoothWidth = false;   // 设置平滑宽度
			GCConvexHullLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

			ConvexHullLineColor = new Color32((Byte)0, (Byte)191, (Byte)255, (Byte)255);

			GCConvexHullLine.color = ConvexHullLineColor;

			int MinX = Mathf.FloorToInt(GCconvexHull.ConvexHullSet[0].x), MaxX = Mathf.CeilToInt(GCconvexHull.ConvexHullSet[0].x);   // 凸包的最大最小X
			int MinY = Mathf.FloorToInt(GCconvexHull.ConvexHullSet[0].y), MaxY = Mathf.CeilToInt(GCconvexHull.ConvexHullSet[0].y);   // 凸包的最大最小Y

			// 先把初始点存入画图函数
			GCConvexHullLine.points2.Add(new Vector2(GCconvexHull.ConvexHullSet[0].x, GCconvexHull.ConvexHullSet[0].y));
			GCconvexHull.ConvexHullArea = 0f;   // 令凸包面积初始为0

			for (int i = 1; i < GCconvexHull.ConvexHullNum; i++)
			{
				//print("!!!!!");
				if (GCconvexHull.ConvexHullSet[i].x > 1919) GCconvexHull.ConvexHullSet[i].x = 1919;
				else if (GCconvexHull.ConvexHullSet[i].x < 0) GCconvexHull.ConvexHullSet[i].x = 0;

				if (GCconvexHull.ConvexHullSet[i].y > 1079) GCconvexHull.ConvexHullSet[i].y = 1079;
				else if (GCconvexHull.ConvexHullSet[i].y < 0) GCconvexHull.ConvexHullSet[i].y = 0;

				GCConvexHullLine.points2.Add(new Vector2(GCconvexHull.ConvexHullSet[i].x, GCconvexHull.ConvexHullSet[i].y));
				//ConvexHullLine.SetColor(ConvexHullLineColor);  // 设置颜色

				if (i < GCconvexHull.ConvexHullNum - 1)
				{
					GCconvexHull.ConvexHullArea += Math.Abs(ConvexHull.isLeft(GCconvexHull.ConvexHullSet[0], GCconvexHull.ConvexHullSet[i], GCconvexHull.ConvexHullSet[i + 1]));
				}

				if (MinX > Mathf.FloorToInt(GCconvexHull.ConvexHullSet[i].x)) MinX = Mathf.FloorToInt(GCconvexHull.ConvexHullSet[i].x);
				if (MaxX < Mathf.CeilToInt(GCconvexHull.ConvexHullSet[i].x)) MaxX = Mathf.CeilToInt(GCconvexHull.ConvexHullSet[i].x);
				if (MinY > Mathf.FloorToInt(GCconvexHull.ConvexHullSet[i].y)) MinY = Mathf.FloorToInt(GCconvexHull.ConvexHullSet[i].y);
				if (MaxY < Mathf.CeilToInt(GCconvexHull.ConvexHullSet[i].y)) MaxY = Mathf.CeilToInt(GCconvexHull.ConvexHullSet[i].y);

				GCConvexHullLine.Draw();
				yield return new WaitForSeconds(0.15f);
			}

			//button.transform.GetChild(0).GetComponent<Text>().text = (ConvexHullArea / 2).ToString("0.00");// 最后求出来的面积要除以2

			GCConvexHullLine.points2.Add(new Vector2(GCconvexHull.ConvexHullSet[0].x, GCconvexHull.ConvexHullSet[0].y));
			//ConvexHullLine.SetColor(Color.blue);  // 设置颜色
			//ConvexHullLine.SetColor(ConvexHullLineColor);  // 设置颜色
			GCConvexHullLine.Draw();

			StartCoroutine(DrawGCConvexHullArea(MinX - 2, MaxX + 2, MinY - 2, MaxY + 2));
		}

		IEnumerator DrawGCConvexHullArea(int MinX, int MaxX, int MinY, int MaxY)
		{
			yield return new WaitForEndOfFrame();

			if (!GCConvexHullIsDraw)
			{
				//if (ConvexHullArea.points2 != 0 )
				//{
				// 画透明区域

				GCConvexHullIsDraw = true;

				Color32 ConvexHullAreaColor = new Color32((Byte)0, (Byte)191, (Byte)255, (Byte)40);

				GCConvexHullArea = new VectorLine("GCConvexHullLine", new List<Vector2>(), 1f, Vectrosity.LineType.Continuous, Joins.Weld);
				GCConvexHullArea.smoothColor = false;   // 设置平滑颜色
				GCConvexHullArea.smoothWidth = false;   // 设置平滑宽度
				GCConvexHullArea.color = ConvexHullAreaColor;

				Texture2D m_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
				m_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
				//m_texture.Apply();

				MinX = Math.Max(0, MinX);
				MinY = Math.Max(0, MinY);

				GCConvexHullColors = m_texture.GetPixels(MinX, MinY, Math.Min(MaxX - MinX, 1919 - MinX), Math.Min(MaxY - MinY, 1079 - MinY));

				MaxY = MaxY - MinY - 1;

				int x, y;
				for (int i = 0; i < MaxY; i++)
				{
					x = i * (MaxX - MinX); y = (i + 1) * (MaxX - MinX);

					while ((x < y) && (GCConvexHullColors[x] != ConvexHullLineColor)) x++;    // 查找左边的凸包边界
					while ((x < y) && (GCConvexHullColors[y] != ConvexHullLineColor)) y--;    // 查找右边的凸包边界

					if (x != y)
					{
						GCConvexHullArea.points2.Add(new Vector2(MinX + x - i * (MaxX - MinX), MinY + i));
						GCConvexHullArea.points2.Add(new Vector2(MinX + y - i * (MaxX - MinX), MinY + i));
					}
				}
				GCConvexHullArea.Draw();
				//}
			}
		}

		public void RemoveGCLine()
		{
			VectorLine.Destroy(ref GCLine);
			VectorLine.Destroy(ref GCConvexHullLine);
			VectorLine.Destroy(ref GCConvexHullArea);
			VectorLine.Destroy(ref GCLineComplete);


			TrackIsDraw = false;
		}
	}
}