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

		private float Distance;		// 重心与原点的距离


		// Use this for initialization
		void Start()
		{

		}

		void OnEnable()
		{
			if (DoctorDataManager.instance.patient.trainingPlays.Count > 0)
			{
				DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters = DoctorDatabaseManager.instance.ReadGravityCenterRecord(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count-1].TrainingID);
				GravityCenterCount = DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters.Count;
				//print(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[0].TrainingID);
			}

			GravityCenterChart = transform.Find("GravityCenterChart").GetComponent<LineChart>();
			if (GravityCenterChart == null) GravityCenterChart = transform.Find("GravityCenterChart").gameObject.AddComponent<LineChart>();

			GravityCenterChart.RemoveData();

			GravityCenterSerie = GravityCenterChart.AddSerie(SerieType.Line, "重心");

			GravityCenterSerie.lineType = LineType.Smooth;
			GravityCenterSerie.lineStyle.width = 2;
			GravityCenterSerie.lineStyle.opacity = 0.8f;
			GravityCenterSerie.symbol.size = 4;
			GravityCenterSerie.symbol.selectedSize = 5;

			for (int i = 0; i < 30; i++)
			{
				//chart.AddXAxisData("x" + (i + 1)); 
				print(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[i].Coordinate);
				GravityCenterChart.AddData(0, 1000 * Vector3.Distance(DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[i].Coordinate, DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].gravityCenters[0].Coordinate));
			}

		}



		// Update is called once per frame
		void Update()
		{

		}
	}
}