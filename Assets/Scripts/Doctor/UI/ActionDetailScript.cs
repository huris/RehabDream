using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace XCharts
{
    [DisallowMultipleComponent]
    public class ActionDetailScript : MonoBehaviour
    {

        public Dictionary<int, int> toggleIndexTojointId;
        public GameObject jointCheckBox;
        public Sprite NotChecked;
        public Sprite Checked;
        public int current_jointId = -1;

        public LineChart ActionPassRateChart;
        public LineChart ActionDeviationChart;

        public int WallEvaluationIndex;

        public List<Toggle> PeopleToggles;
        public List<Toggle> AngleToggles;
        public List<Toggle> MethodToggles;

        public int JointToggleIndex;   // 找到哪个关节改变了
        public int MethodToggleIndex;   // 找到哪个方法变了

        public static List<int> defaultCheckJoints = new List<int>() { 0, 2, 3, 4, 5, 6, 8, 9, 10, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        public static List<int> defaultMatchingCheckJoints = new List<int>() { 2, 4, 5, 8, 9, 12, 13, 14, 16, 17, 18 };
        public static Dictionary<string, float> ActionMatchThreshold = new Dictionary<string, float>() { { "PERFECT", 90F }, { "GREAT", 80F }, { "GOOD", 70F } };
        //24个关节夹角检测方法
        public static Dictionary<int, List<int>> JointCheckMethod = new Dictionary<int, List<int>>() {
        { 2, new List<int>() { 23,24 } },
        { 4, new List<int>() { 1, 2, 3, 4 } },
        { 5, new List<int>() { 9 } },
        { 12, new List<int>() { 11,12,13 } },
        { 13, new List<int>() { 17 } },
        { 14, new List<int>() { 20 } },//21号检测方法不可靠，暂时去掉
        { 8, new List<int>() { 5,6,7,8 } },
        { 9, new List<int>() { 10 } },
        { 16, new List<int>() { 14,15,16 } },
        { 17, new List<int>() { 18 } },
        { 18, new List<int>() { 22 } }};//21号检测方法不可靠，暂时去掉

        public static List<int> JointOfMethod = new List<int>() { 0, 4, 4, 4, 4, 8, 8, 8, 8, 5, 9, 12, 12, 12, 16, 16, 16, 13, 17, 14, 14, 18, 18, 2, 2 };
        public static Dictionary<int, int> MaxValueOfMethod = new Dictionary<int, int>() {
            { 1,180}, { 2, 180 }, { 3, 180 }, { 4, 180 }, { 5, 180 },
            { 6,180}, { 7, 180 }, { 8, 180 }, { 9, 180 }, { 10, 180 },
            { 11,90}, { 12, 90 }, { 13, 150 }, { 14, 90 }, { 15, 90 },
            { 16,150}, { 17, 180 }, { 18, 180 }, { 19, 180 }, { 20, 180 },
            { 21,180}, { 22, 180 }, { 23, 135 }, { 24, 135 }};
        public static Dictionary<int, int> MinValueOfMethod = new Dictionary<int, int>() {
            { 1,0}, { 2, 0 }, { 3, 0 }, { 4, 20 }, { 5, 0 },
            { 6,0}, { 7, 0 }, { 8, 20 }, { 9, 25 }, { 10, 25 },
            { 11,0}, { 12, 0 }, { 13, 60 }, { 14, 0 }, { 15, 0 },
            { 16,60}, { 17, 50 }, { 18, 50 }, { 19, 60 }, { 20, 0 },
            { 21,60}, { 22, 0 }, { 23, 45 }, { 24, 45 }};
        public static float ShoulderInvalidateAngle = 20;
        public static int ToBeImprovedJointNum = 3;
        public static float KinectErrorAngle = 10;
        public static Dictionary<int, List<int>> TrainingTypeToJoint = new Dictionary<int, List<int>>() {//训练类型（类型id,对应包含的关节id）
            { 0,new List<int>() { 4,5} },
            { 1,new List<int>() { 8,9} },
            { 2,new List<int>() { 4} },
            { 3,new List<int>() { 8} },
            { 4,new List<int>() { 13,12} },
            { 5,new List<int>() { 12} },
            { 6,new List<int>() { 16,17} },
            { 7,new List<int>() { 16} },
            };
        public static Dictionary<int, string> TrainingTypeIDToName = new Dictionary<int, string>() {//训练类型（类型id,对应包含的关节id）
            { 0,"左小臂截肢" },
            { 1,"右小臂截肢" },
            { 2,"左大臂截肢" },
            { 3,"右大臂截肢" },
            { 4,"左小腿截肢" },
            { 5,"左大腿截肢" },
            { 6,"右小腿截肢" },
            { 7,"右大腿截肢" },
            };
        public static List<int> wallSpeed = new List<int>() { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        public static List<int> trainingDays = new List<int>() { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
        public static Dictionary<int, string> methodIDToName = new Dictionary<int, string>()
        {
            { 1,"屈曲伸展"},
            { 2,"外展内收"},
            { 3,"内旋外旋"},
            { 4,"水平收展"},
            { 5,"屈曲伸展"},
            { 6,"外展内收"},
            { 7,"内旋外旋"},
            { 8,"水平收展"},
            { 9,"屈曲伸展"},
            { 10,"屈曲伸展"},
            { 11,"屈曲伸展"},
            { 12,"外展内收"},
            { 13,"内旋外旋"},
            { 14,"屈曲伸展"},
            { 15,"外展内收"},
            { 16,"内旋外旋"},
            { 17,"屈曲伸展"},
            { 18,"屈曲伸展"},
            { 19,"背屈跖屈"},
            { 20,"内翻外翻"},
            { 21,"背屈跖屈"},
            { 22,"内翻外翻"},
            { 23,"左右侧屈"},
            { 24,"屈曲伸展"},
        };
        public static Dictionary<int, string> checkjointIDToName = new Dictionary<int, string>()
        {
            {2,"颈关节" },
            {4,"左肩关节" },
            {5,"左肘关节" },
            {8,"右肩关节" },
            {9,"右肘关节" },
            {12,"左髋关节" },
            {13,"左膝关节" },
            {14,"左踝关节" },
            {16,"右髋关节" },
            {17,"右膝关节" },
            {18,"右踝关节" },
        };

        public List<int> ActionID;

        public void JointToggleChange()
        {
            //DoctorDataManager.instance.doctor.patient.SetWallEvaluationIndex(1);

            JointToggleIndex = -1;

            for (int i = 0; i < AngleToggles.Count; i++)
            {
                if (AngleToggles[i].isOn)
                {
                    JointToggleIndex = i;
                    break;
                }
            }

            //MethodToggles[0].isOn = true;

            while (ActionPassRateChart.series.list[0].data.Count > ActionID.Count)
            {
                ActionPassRateChart.series.list[0].data.RemoveAt(ActionPassRateChart.series.list[0].data.Count - 1);
                ActionPassRateChart.xAxis0.data.RemoveAt(ActionPassRateChart.xAxis0.data.Count - 1);
            }

            while (ActionPassRateChart.series.list[0].data.Count < ActionID.Count)
            {
                ActionPassRateChart.series.list[0].AddYData(-1f);
                ActionPassRateChart.xAxis0.data.Add("A" + (ActionPassRateChart.xAxis0.data.Count + 1).ToString());
            }

            //if (JointToggleIndex == -1)
            //{
            //    ActionPassRateChart.title.subText = "动作组均未涉及该关节,请选择其他关节";
            //    for (int i = 0; i < ActionPassRateChart.series.list[0].data.Count; i++)
            //    {
            //        ActionPassRateChart.series.UpdateData(0, i, -1f);
            //    }

            //    ActionDeviationChart.title.subText = "动作组均未涉及该关节,请选择其他关节";
            //    for (int i = 0; i < ActionDeviationChart.series.list[0].data.Count; i++)
            //    {
            //        ActionDeviationChart.series.UpdateData(0, i, -1f);
            //    }

            //    for (int i = 0; i < 4; i++)
            //    {
            //        MethodToggles[i].gameObject.SetActive(false);
            //    }
            //}
            //else
            //{
            //for (int i = 0; i < 4; i++)
            //{
            //    MethodToggles[i].gameObject.SetActive(false);
            //}

            if (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].passPercentScore == -1)
            {
                ActionPassRateChart.title.subText = "动作组均未涉及该关节,请选择其他关节";

                for (int i = 0; i < ActionPassRateChart.series.list[0].data.Count; i++)
                {
                    ActionPassRateChart.series.UpdateData(0, i, -1f);
                }

                ActionDeviationChart.title.subText = "动作组均未涉及该关节,请选择其他关节";
                for (int i = 0; i < ActionDeviationChart.series.list[0].data.Count; i++)
                {
                    ActionDeviationChart.series.UpdateData(0, i, -1f);
                }
            }
            else
            {
                ActionPassRateChart.title.subText = checkjointIDToName[toggleIndexTojointId[JointToggleIndex]] + " 整体通过率: " + " <color=#4BABDCFF>" + DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].passPercentScore + "%</color>";
            }


            for (int i = 0; i < ActionID.Count; i++)
            {
                ActionPassRateChart.series.UpdateData(0, i, DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].passPercent[ActionID[i]]);
            }

            for (int i = 0; i < 4; i++)
            {
                MethodToggles[i].isOn = false;

                if (i < JointCheckMethod[toggleIndexTojointId[JointToggleIndex]].Count)
                {
                    MethodToggles[i].gameObject.SetActive(true);
                    MethodToggles[i].transform.Find("Label").GetComponent<Text>().text = methodIDToName[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][i]];
                }
                else
                {
                    MethodToggles[i].gameObject.SetActive(false);
                }
            }


            //ActionDeviationChart.title.subText = "请选择检测方法后查看";
            //for (int i = 0; i < ActionDeviationChart.series.list[0].data.Count; i++)
            //{
            //    ActionDeviationChart.series.UpdateData(0, i, -1f);
            //}

            MethodToggles[0].isOn = true;
            MethodToggleChange();

            //}
        }

        public void MethodToggleChange()
        {
            MethodToggleIndex = -1;
            for (int i = 0; i < 4; i++)
            {
                if (MethodToggles[i].isOn)
                {
                    MethodToggleIndex = i;
                    break;
                }
            }

            //if(MethodToggleIndex == -1)
            //{
            //    ActionDeviationChart.title.subText = "请选择检测方法后查看";
            //    for (int i = 0; i < ActionDeviationChart.series.list[0].data.Count; i++)
            //    {
            //        ActionDeviationChart.series.UpdateData(0, i, -1f);
            //    }
            //}
            //else
            //{
            if (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].methodDatas[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][MethodToggleIndex]].eps == -1)
            {
                ActionDeviationChart.title.subText = "动作组均未涉及该关节,请选择其他关节";

                for (int i = 0; i < ActionDeviationChart.series.list[0].data.Count; i++)
                {
                    ActionDeviationChart.series.UpdateData(0, i, -1f);
                }

                for (int i = 0; i < 4; i++)
                {
                    MethodToggles[i].gameObject.SetActive(false);
                }
            }
            else
            {
                ActionDeviationChart.title.subText = methodIDToName[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][MethodToggleIndex]] + " 整体误差率: " + " <color=#C23531FF>" + DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].methodDatas[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][MethodToggleIndex]].eps + "%</color>";
            }

            while (ActionDeviationChart.series.list[0].data.Count > DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].methodDatas[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][MethodToggleIndex]].ep.Count)
            {

                ActionDeviationChart.series.list[0].data.RemoveAt(ActionDeviationChart.series.list[0].data.Count - 1);
                ActionDeviationChart.xAxis0.data.RemoveAt(ActionDeviationChart.xAxis0.data.Count - 1);
            }

            while (ActionDeviationChart.series.list[0].data.Count < DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].methodDatas[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][MethodToggleIndex]].ep.Count)
            {
                ActionDeviationChart.series.list[0].AddYData(-1f);
                ActionDeviationChart.xAxis0.data.Add("G" + (ActionDeviationChart.xAxis0.data.Count + 1).ToString());
            }

            for (int i = 0; i < DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].methodDatas[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][MethodToggleIndex]].ep.Count; i++)
            {
                ActionDeviationChart.series.UpdateData(0, i, DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[JointToggleIndex]].methodDatas[JointCheckMethod[toggleIndexTojointId[JointToggleIndex]][MethodToggleIndex]].ep[i]);
            }
            //}
        }

        public void ActionDetailInit()
        {
            if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
            {
                WallEvaluationIndex = DoctorDataManager.instance.doctor.patient.WallEvaluationIndex;

                ActionID = new List<int>(DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Keys);

                toggleIndexTojointId = new Dictionary<int, int>() { { 0, 2 }, { 1, 4 }, { 2, 8 }, { 3, 5 }, { 4, 9 }, { 5, 12 }, { 6, 16 }, { 7, 13 }, { 8, 17 }, { 9, 14 }, { 10, 18 } };

                int FirstAngleID = 0;

                for (int i = 0; i < AngleToggles.Count; i++)
                { 
                    if (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[i]].passPercentScore != -1)
                    {
                        FirstAngleID = i;
                        AngleToggles[FirstAngleID].isOn = true;
                        break;
                    }

                }

                for (int i = 0; i < AngleToggles.Count; i++)
                {
                    PeopleToggles[i].isOn = false;
                    AngleToggles[i].isOn = false;

                    if (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].detail.jointDatas[toggleIndexTojointId[i]].passPercentScore == -1)
                    {
                        PeopleToggles[i].gameObject.SetActive(false);
                        AngleToggles[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        PeopleToggles[i].gameObject.SetActive(true);
                        AngleToggles[i].gameObject.SetActive(true);
                    }
                }


                JointToggleChange();
            }
        }

        // Use this for initialization
        void Start()
        {
            ActionDetailInit();
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}