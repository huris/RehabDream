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
            if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
            {

                RadarArea = new List<float>();
                RadarIncreaseRate = new List<float>();

                RadarAreaText = transform.Find("RadarArea").GetComponent<Text>();

                WhiteLine = "";

                DirectionRadarChart = transform.Find("RadarChart").GetComponent<RadarChart>();
                if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("RadarChart").gameObject.AddComponent<RadarChart>();

                if (DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 6) WhiteLine = "\n";
                else WhiteLine = "\n\n";

                //print(DoctorDataManager.instance.patient.Evaluations.Count+"!!!!");


                //print(DoctorDataManager.instance.doctor.patient.TrainingPlayIndex+"@@@@@");

                RadarAreaText.text = "";


                for (int i = 0; i < DoctorDataManager.instance.doctor.patient.TrainingPlays.Count; i++)
                {
                    //DoctorDataManager.instance.patient.TrainingPlays[i].direction = DoctorDatabaseManager.instance.ReadDirectionRecord(DoctorDataManager.instance.patient.TrainingPlays[i].TrainingID);

                    if (i == DoctorDataManager.instance.doctor.patient.TrainingPlayIndex)
                    {
                        DirectionRadarChart.UpdateData(0, 0, 0, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.UponDirection);
                        DirectionRadarChart.UpdateData(0, 0, 1, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.UponRightDirection);
                        DirectionRadarChart.UpdateData(0, 0, 2, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.RightDirection);
                        DirectionRadarChart.UpdateData(0, 0, 3, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.DownRightDirection);
                        DirectionRadarChart.UpdateData(0, 0, 4, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.DownDirection);
                        DirectionRadarChart.UpdateData(0, 0, 5, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.DownLeftDirection);
                        DirectionRadarChart.UpdateData(0, 0, 6, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.LeftDirection);
                        DirectionRadarChart.UpdateData(0, 0, 7, DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.UponLeftDirection);

                        DirectionRadarChart.RefreshChart();

                        if (i != 0) RadarAreaText.text += WhiteLine;
                        RadarAreaText.text += "本次面积: ";
                    }
                    else
                    {
                        RadarAreaText.text += WhiteLine;
                        RadarAreaText.text += "第(" + (i + 1).ToString() + ")次面积: ";
                    }
                    // print(DoctorDataManager.instance.patient.Evaluations[i].direction.UponDirection+"+++++");

                    RadarArea.Add(DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.DirectionRadarArea);

                    if (i == 0)
                    {
                        RadarAreaText.text += RadarArea[i].ToString("0.00");
                    }
                    else
                    {
                        RadarIncreaseRate.Add((RadarArea[i] - RadarArea[i - 1]) / RadarArea[i - 1]);

                        RadarAreaText.text += RadarArea[i].ToString("0.00");
                        if (RadarIncreaseRate[i - 1] < 0) RadarAreaText.text += "  <color=red>-" + RadarIncreaseRate[i - 1].ToString("0.00") + "%</color>";
                        else if (RadarIncreaseRate[i - 1] == 0) RadarAreaText.text += "  <color=blue>=" + RadarIncreaseRate[i - 1].ToString("0.00") + "%</color>";
                        else RadarAreaText.text += "  <color=green>+" + RadarIncreaseRate[i - 1].ToString("0.00") + "%</color>";
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

