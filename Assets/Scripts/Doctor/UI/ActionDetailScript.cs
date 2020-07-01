using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace XCharts
{
    [DisallowMultipleComponent]
    public class ActionDetailScript : MonoBehaviour {

        public Dictionary<int, int> toggleIndexTojointId;
        public GameObject jointCheckBox;
        public Sprite NotChecked;
        public Sprite Checked;
        public int current_jointId = -1;

        public LineChart ActionPassRateChart;
        public LineChart ActionDeviationChart;

        public int WallEvaluationIndex;

        public List<Toggle> Toggles;

        public void JointToggleChange()
        {
            WallEvaluationIndex = DoctorDataManager.instance.doctor.patient.WallEvaluationIndex;

            List<int> ActionID = new List<int>(DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Keys);

            toggleIndexTojointId = new Dictionary<int, int>() { { 0, 2 }, { 1, 4 }, { 2, 8 }, { 3, 5 }, { 4, 9 }, { 5, 12 }, { 6, 16 }, { 7, 13 }, { 8, 17 }, { 9, 14 }, { 10, 18 } };

            int ToggleIndex = -1;   // 找到那个关节改变了

            for(int i = 0; i < Toggles.Count; i++)
            {
                if (Toggles[i].isOn)
                {
                    ToggleIndex = i;
                    break;
                }
            }

            if(ToggleIndex == -1)
            {
                ActionPassRateChart.xAxis0.ClearData();
                ActionPassRateChart.series.list[0].ClearData();
                ActionPassRateChart.title.subText = "<color=#4BABDCFF>所有动作均未涉及该关节</color>";
            }
            else
            {

            }

            //ActionPassRateChart.title.subText = "整体平均" + " <color=(55,162,218)>" + DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas.. + "%</color>"; ;

        }



        // Use this for initialization
        void Start()
        {
            JointToggleChange();
        }
        // Update is called once per frame
        void Update() {

        }
    }
}