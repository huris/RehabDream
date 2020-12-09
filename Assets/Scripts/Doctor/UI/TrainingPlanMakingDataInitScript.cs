using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingPlanMakingDataInitScript : MonoBehaviour {

    public Dropdown PlanDifficult;
    public Dropdown PlanDirection;
    public InputField PlanTime;

    public GameObject PlanMakingSuccess;
    public GameObject PlanMakingFail;
    public GameObject PlanDeleteSuccess;
    public GameObject PlanDeleteFail;
    //public GameObject NoEvaluationData;

    //public Toggle PatientInfoManagerToggle;

    public Text PlanMakingButtonText;

    public Dictionary<string, int> DifficultString2Int;
    public Dictionary<int, string> DifficultInt2String;

    public Dictionary<string, int> DirectionString2Int;
    public Dictionary<int, string> DirectionInt2String;

    public EventSystem system;

    private Selectable SelecInput;   // 当前焦点所处的Input
    private Selectable NextInput;   // 目标Input

    public GameObject PatientInfo;

    public GameObject TrainingStart;

    // Use this for initialization
    void Start() {

    }

    void OnEnable()
    {
        PlanDifficult = transform.Find("PlanDifficult/Dropdown").GetComponent<Dropdown>();
        PlanDirection = transform.Find("PlanDirection/Dropdown").GetComponent<Dropdown>();
        PlanTime = transform.Find("PlanTime/InputField").GetComponent<InputField>();

        PlanMakingButtonText = transform.Find("PlanMakingButton/Text").GetComponent<Text>();
      
        PlanMakingSuccess = transform.Find("PlanMakingSuccess").gameObject;
        PlanMakingSuccess.SetActive(false);

        PlanMakingFail = transform.Find("PlanMakingFail").gameObject;
        PlanMakingFail.SetActive(false);

        PlanDeleteSuccess = transform.Find("PlanDeleteSuccess").gameObject;
        PlanDeleteSuccess.SetActive(false);

        PlanDeleteFail = transform.Find("PlanDeleteFail").gameObject;
        PlanDeleteFail.SetActive(false);

        //PatientInfoManagerToggle = transform.parent.parent.parent.parent.parent.Find("FunctionManager/PatentInfoManagerItem").GetComponent<Toggle>();

        //NoEvaluationData = transform.parent.Find("NoEvaluationData").gameObject;

        if (DoctorDataManager.instance.doctor.patient.Evaluations == null){
            DoctorDataManager.instance.doctor.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

            if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
            {
                DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);
            }
        }

        if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
        {
            //NoEvaluationData.SetActive(false);

            DifficultString2Int = new Dictionary<string, int>();
            DifficultString2Int.Add("请选择难度", 0);
            DifficultString2Int.Add("入门", 1);
            DifficultString2Int.Add("初级", 2);
            DifficultString2Int.Add("一般", 3);
            DifficultString2Int.Add("中级", 4);
            DifficultString2Int.Add("高级", 5);

            DifficultInt2String = new Dictionary<int, string>();
            DifficultInt2String.Add(0, "请选择难度");
            DifficultInt2String.Add(1, "入门");
            DifficultInt2String.Add(2, "初级");
            DifficultInt2String.Add(3, "一般");
            DifficultInt2String.Add(4, "中级");
            DifficultInt2String.Add(5, "高级");

            DirectionString2Int = new Dictionary<string, int>();
            DirectionString2Int.Add("请选择方向", 0);
            DirectionString2Int.Add("全方位", 1);
            DirectionString2Int.Add("正上方", 2);
            DirectionString2Int.Add("右上方", 3);
            DirectionString2Int.Add("正右方", 4);
            DirectionString2Int.Add("右下方", 5);
            DirectionString2Int.Add("正下方", 6);
            DirectionString2Int.Add("左下方", 7);
            DirectionString2Int.Add("正左方", 8);
            DirectionString2Int.Add("左上方", 9);

            DirectionInt2String = new Dictionary<int, string>();
            DirectionInt2String.Add(0, "请选择方向");
            DirectionInt2String.Add(1, "全方位");
            DirectionInt2String.Add(2, "正上方");
            DirectionInt2String.Add(3, "右上方");
            DirectionInt2String.Add(4, "正右方");
            DirectionInt2String.Add(5, "右下方");
            DirectionInt2String.Add(6, "正下方");
            DirectionInt2String.Add(7, "左下方");
            DirectionInt2String.Add(8, "正左方");
            DirectionInt2String.Add(9, "左上方");

            DoctorDataManager.instance.doctor.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID);
            if (DoctorDataManager.instance.doctor.patient.trainingPlan != null) DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(true);
            else DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(false);

            if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
            {
                PlanDifficult.value = DifficultString2Int[DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty];
                PlanDirection.value = DirectionString2Int[DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDirection];
                PlanTime.text = DoctorDataManager.instance.doctor.patient.trainingPlan.PlanTime.ToString();

                TrainingStart.SetActive(true);

                PlanMakingButtonText.text = "修  改";
            }
            else
            {
                TrainingStart.SetActive(false);


                PlanDifficult.value = 0;
                PlanDirection.value = 0;
                PlanTime.text = "";

                PlanMakingButtonText.text = "制  定";
            }

        }
        else
        {
            //NoEvaluationData.SetActive(true);
        }

        //system = EventSystem.current;       // 获取当前的事件

    }

    // Update is called once per frame
    void Update() {
        ////在Update内监听Tap键的按下
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    //是否聚焦Input
        //    if (system.currentSelectedGameObject != null)
        //    {
        //        //获取当前选中的Input
        //        SelecInput = system.currentSelectedGameObject.GetComponent<Selectable>();
        //        //监听Shift
        //        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        //        {
        //            //Shift按下则选择出去上方的Input
        //            NextInput = SelecInput.FindSelectableOnUp();
        //            //上边没有找左边的
        //            if (NextInput == null) NextInput = SelecInput.FindSelectableOnLeft();
        //        }
        //        else
        //        {
        //            //没按shift就找下边的Input
        //            NextInput = SelecInput.FindSelectableOnDown();
        //            //或者右边的
        //            if (NextInput == null) NextInput = SelecInput.FindSelectableOnRight();
        //        }
        //    }

        //    //下一个Input不空的话就聚焦
        //    if (NextInput != null) NextInput.Select();
        //}

        //// 按回车键进行登录
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    TrainingPlanMakingButtonOnClick();
        //}
    }

    public void TrainingPlanMakingButtonOnClick()
    {
        PlanMakingFail.SetActive(false);
        PlanMakingSuccess.SetActive(false);
        PlanDeleteSuccess.SetActive(false);
        PlanDeleteFail.SetActive(false);

        try
        {
            TrainingPlan trainingPlan = new TrainingPlan();
            trainingPlan.SetTrainingPlan(DifficultInt2String[PlanDifficult.value], DirectionInt2String[PlanDirection.value], PlanTime.text == ""?20:long.Parse(PlanTime.text));

            DoctorDatabaseManager.DatabaseReturn RETURN;  // 返回修改训练计划结果

            if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
            {
                RETURN = DoctorDatabaseManager.instance.ModifyPatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID, trainingPlan);
            }
            else
            {
                RETURN = DoctorDatabaseManager.instance.MakePatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID, trainingPlan);
                Debug.Log(RETURN);
            }

            if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
            {
                //DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetPlanIsMaking(true);

                DoctorDataManager.instance.doctor.patient.trainingPlan = trainingPlan;
                //DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetTrainingPlan(DifficultInt2String[PlanDifficult.value], DirectionInt2String[PlanDirection.value], PlanTime.text == "" ? 20 : long.Parse(PlanTime.text));
                
                DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(true);

                PatientInfo.SetActive(false);
                PatientInfo.SetActive(true);

                PlanMakingSuccess.SetActive(true);

                TrainingStart.SetActive(true);

                //StartCoroutine(DelayTime(3));
            }
            else
            {
                PlanMakingFail.SetActive(true);
            }
        }
        catch(Exception e)
        {
            PlanMakingFail.SetActive(true);
        }

    }

    public void TrainingPlanDeleteButtonOnClick()
    {
        PlanMakingFail.SetActive(false);
        PlanMakingSuccess.SetActive(false);
        PlanDeleteSuccess.SetActive(false);
        PlanDeleteFail.SetActive(false);

        try
        {
            if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
            {
                DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.DeletePatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID);

                if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
                {
                    DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(false);
                    //DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetPlanIsMaking(false);

                    PatientInfo.SetActive(false);
                    PatientInfo.SetActive(true);

                    PlanDeleteSuccess.SetActive(true);

                    //StartCoroutine(DelayTime(3));
                }
                else
                {
                    PlanMakingFail.SetActive(true);
                }
            }
            else
            {
                PlanDeleteFail.SetActive(true);
            }
        }
        catch (Exception e)
        {
            PlanDeleteFail.SetActive(true);
        }

        
    }


    //IEnumerator DelayTime(int time)
    //{
    //    yield return new WaitForSeconds(time);

    //    PatientInfoManagerToggle.isOn = true;
    //}

}
