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

        private LineChart LegChart;    // 腿与躯干
        private Serie LeftLegSerie;
        private Serie RightLegSerie;

        private LineChart KneeChart;    // 膝关节
        private Serie LeftKneeSerie;
        private Serie RightKneeSerie;

        private LineChart AnkleChart;    // 踝关节
        private Serie LeftAnkleSerie;
        private Serie RightAnkleSerie;


        // Use this for initialization
        void Start()
        {

        }

        void OnEnable()
        {
            //if (DoctorDataManager.instance.patient.trainingPlays.Count > 0)
            //{
            //    DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles = DoctorDatabaseManager.instance.ReadAngleRecord(DoctorDataManager.instance.patient.PatientID);
            //}

            ArmChart = transform.Find("ArmChart").GetComponent<LineChart>();
            if (ArmChart == null) ArmChart = transform.Find("ArmChart").gameObject.AddComponent<LineChart>();
            
            ArmChart.RemoveData();
            
            LeftArmSerie = ArmChart.AddSerie(SerieType.Line, "左");
            RightArmSerie = ArmChart.AddSerie(SerieType.Line, "右");

            LeftArmSerie.lineType = LineType.Smooth;
            LeftArmSerie.lineStyle.width = 2;
            LeftArmSerie.lineStyle.opacity = 0.8f;
            LeftArmSerie.symbol.size = 4;
            LeftArmSerie.symbol.selectedSize = 5;

            RightArmSerie.lineType = LineType.Smooth;
            RightArmSerie.lineStyle.width = 2;
            RightArmSerie.lineStyle.opacity = 0.8f;
            RightArmSerie.symbol.size = 4;
            RightArmSerie.symbol.selectedSize = 5;
            

            ElbowChart = transform.Find("ElbowChart").GetComponent<LineChart>();
            if (ElbowChart == null) ElbowChart = transform.Find("ElbowChart").gameObject.AddComponent<LineChart>();

            ElbowChart.RemoveData();

            LeftElbowSerie = ElbowChart.AddSerie(SerieType.Line, "左");
            RightElbowSerie = ElbowChart.AddSerie(SerieType.Line, "右");

            LeftElbowSerie.lineType = LineType.Smooth;
            LeftElbowSerie.lineStyle.width = 2;
            LeftElbowSerie.lineStyle.opacity = 0.8f;
            LeftElbowSerie.symbol.size = 4;
            LeftElbowSerie.symbol.selectedSize = 5;

            RightElbowSerie.lineType = LineType.Smooth;
            RightElbowSerie.lineStyle.width = 2;
            RightElbowSerie.lineStyle.opacity = 0.8f;
            RightElbowSerie.symbol.size = 4;
            RightElbowSerie.symbol.selectedSize = 5;


            LegChart = transform.Find("LegChart").GetComponent<LineChart>();
            if (LegChart == null) LegChart = transform.Find("LegChart").gameObject.AddComponent<LineChart>();

            LegChart.RemoveData();

            LeftLegSerie = LegChart.AddSerie(SerieType.Line, "左");
            RightLegSerie = LegChart.AddSerie(SerieType.Line, "右");

            LeftLegSerie.lineType = LineType.Smooth;
            LeftLegSerie.lineStyle.width = 2;
            LeftLegSerie.lineStyle.opacity = 0.8f;
            LeftLegSerie.symbol.size = 4;
            LeftLegSerie.symbol.selectedSize = 5;

            RightLegSerie.lineType = LineType.Smooth;
            RightLegSerie.lineStyle.width = 2;
            RightLegSerie.lineStyle.opacity = 0.8f;
            RightLegSerie.symbol.size = 4;
            RightLegSerie.symbol.selectedSize = 5;


            HipChart = transform.Find("HipChart").GetComponent<LineChart>();
            if (HipChart == null) HipChart = transform.Find("HipChart").gameObject.AddComponent<LineChart>();

            HipChart.RemoveData();

            LeftHipSerie = HipChart.AddSerie(SerieType.Line, "左");
            RightHipSerie = HipChart.AddSerie(SerieType.Line, "右");

            LeftHipSerie.lineType = LineType.Smooth;
            LeftHipSerie.lineStyle.width = 2;
            LeftHipSerie.lineStyle.opacity = 0.8f;
            LeftHipSerie.symbol.size = 4;
            LeftHipSerie.symbol.selectedSize = 5;

            RightHipSerie.lineType = LineType.Smooth;
            RightHipSerie.lineStyle.width = 2;
            RightHipSerie.lineStyle.opacity = 0.8f;
            RightHipSerie.symbol.size = 4;
            RightHipSerie.symbol.selectedSize = 5;


            KneeChart = transform.Find("KneeChart").GetComponent<LineChart>();
            if (KneeChart == null) KneeChart = transform.Find("KneeChart").gameObject.AddComponent<LineChart>();

            KneeChart.RemoveData();

            LeftKneeSerie = KneeChart.AddSerie(SerieType.Line, "左");
            RightKneeSerie = KneeChart.AddSerie(SerieType.Line, "右");

            LeftKneeSerie.lineType = LineType.Smooth;
            LeftKneeSerie.lineStyle.width = 2;
            LeftKneeSerie.lineStyle.opacity = 0.8f;
            LeftKneeSerie.symbol.size = 4;
            LeftKneeSerie.symbol.selectedSize = 5;

            RightKneeSerie.lineType = LineType.Smooth;
            RightKneeSerie.lineStyle.width = 2;
            RightKneeSerie.lineStyle.opacity = 0.8f;
            RightKneeSerie.symbol.size = 4;
            RightKneeSerie.symbol.selectedSize = 5;


            AnkleChart = transform.Find("AnkleChart").GetComponent<LineChart>();
            if (AnkleChart == null) AnkleChart = transform.Find("AnkleChart").gameObject.AddComponent<LineChart>();

            AnkleChart.RemoveData();

            LeftAnkleSerie = AnkleChart.AddSerie(SerieType.Line, "左");
            RightAnkleSerie = AnkleChart.AddSerie(SerieType.Line, "右");

            LeftAnkleSerie.lineType = LineType.Smooth;
            LeftAnkleSerie.lineStyle.width = 2;
            LeftAnkleSerie.lineStyle.opacity = 0.8f;
            LeftAnkleSerie.symbol.size = 4;
            LeftAnkleSerie.symbol.selectedSize = 5;

            RightAnkleSerie.lineType = LineType.Smooth;
            RightAnkleSerie.lineStyle.width = 2;
            RightAnkleSerie.lineStyle.opacity = 0.8f;
            RightAnkleSerie.symbol.size = 4;
            RightAnkleSerie.symbol.selectedSize = 5;

            // DoctorDataManager.instance.patient.trainingPlays[DoctorDataManager.instance.patient.trainingPlays.Count - 1].angles.Count
            for (int i = 0; i < 20; i++)
            {
                //chart.AddXAxisData("x" + (i + 1));
                ArmChart.AddData(0, UnityEngine.Random.Range(30, 90));
                ArmChart.AddData(1, UnityEngine.Random.Range(30, 90));

                ElbowChart.AddData(0, UnityEngine.Random.Range(30, 90));
                ElbowChart.AddData(1, UnityEngine.Random.Range(30, 90));

                LegChart.AddData(0, UnityEngine.Random.Range(30, 90));
                LegChart.AddData(1, UnityEngine.Random.Range(30, 90));

                HipChart.AddData(0, UnityEngine.Random.Range(30, 90));
                HipChart.AddData(1, UnityEngine.Random.Range(30, 90));

                KneeChart.AddData(0, UnityEngine.Random.Range(30, 90));
                KneeChart.AddData(1, UnityEngine.Random.Range(30, 90));

                AnkleChart.AddData(0, UnityEngine.Random.Range(30, 90));
                AnkleChart.AddData(1, UnityEngine.Random.Range(30, 90));
            }



        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}