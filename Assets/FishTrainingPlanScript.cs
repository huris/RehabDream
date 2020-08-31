using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishTrainingPlanScript : MonoBehaviour
{

    public Dropdown TrainingDirection;
    public InputField TrainingDuration;

    public GameObject PlanMakingSuccess;
    public GameObject PlanMakingFail;
    //public GameObject NoEvaluationData;

    //public Toggle PatientInfoManagerToggle;

    public Text PlanMakingButtonText;

    public GameObject TrainingStart;

    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {

        PlanMakingButtonText = transform.Find("PlanMakingButton/Text").GetComponent<Text>();

        PlanMakingSuccess = transform.Find("PlanMakingSuccess").gameObject;
        PlanMakingSuccess.SetActive(false);

        PlanMakingFail = transform.Find("PlanMakingFail").gameObject;
        PlanMakingFail.SetActive(false);


        if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
        {
            DoctorDataManager.instance.doctor.patient.fishTrainingPlan = DoctorDatabaseManager.instance.ReadPatientFishTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID);
            if (DoctorDataManager.instance.doctor.patient.fishTrainingPlan != null) DoctorDataManager.instance.doctor.patient.SetFishPlanIsMaking(true);
            else DoctorDataManager.instance.doctor.patient.SetFishPlanIsMaking(false);

            if (DoctorDataManager.instance.doctor.patient.FishPlanIsMaking)
            {
                TrainingDirection.value = (int)DoctorDataManager.instance.doctor.patient.fishTrainingPlan.TrainingDirection;
                TrainingDuration.text = DoctorDataManager.instance.doctor.patient.fishTrainingPlan.TrainingDuration.ToString() + "分钟";

                TrainingStart.SetActive(true);

                PlanMakingButtonText.text = "修  改";
            }
            else
            {
                TrainingStart.SetActive(false);

                TrainingDirection.value = TrainingDirection.options.Count - 1;
                TrainingDuration.text = "";

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
    void Update()
    {
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

        //print(TrainingDirection.value + "   " + TrainingDuration.text);

        PlanMakingFail.SetActive(false);
        PlanMakingSuccess.SetActive(false);

        try
        {
            //print(TrainingDirection.value + "   " + TrainingDuration.text);

            FishTrainingPlan fishTrainingPlan = new FishTrainingPlan();
            fishTrainingPlan.SetTrainingPlan(TrainingDirection.value, TrainingDuration.text == "" ? 20 : long.Parse(TrainingDuration.text));

            DoctorDatabaseManager.DatabaseReturn RETURN;  // 返回修改训练计划结果

            if((TrainingDirection.value == TrainingDirection.options.Count - 1) || (long.Parse(TrainingDuration.text) < 0))
            {
                RETURN = DoctorDatabaseManager.DatabaseReturn.Fail;
            }
            else if (DoctorDataManager.instance.doctor.patient.FishPlanIsMaking)
            {
                RETURN = DoctorDatabaseManager.instance.ModifyPatientFishTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID, fishTrainingPlan);
            }
            else
            {
                RETURN = DoctorDatabaseManager.instance.MakePatientFishTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID, fishTrainingPlan);
            }

            if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
            {
                //DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetPlanIsMaking(true);

                DoctorDataManager.instance.doctor.patient.fishTrainingPlan = fishTrainingPlan;
                //DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetTrainingPlan(DifficultInt2String[PlanDifficult.value], DirectionInt2String[PlanDirection.value], PlanTime.text == "" ? 20 : long.Parse(PlanTime.text));

                DoctorDataManager.instance.doctor.patient.SetFishPlanIsMaking(true);

                PlanMakingSuccess.SetActive(true);

                TrainingStart.SetActive(true);

                //StartCoroutine(DelayTime(3));
            }
            else
            {
                PlanMakingFail.SetActive(true);
            }
        }
        catch (Exception e)
        {
            PlanMakingFail.SetActive(true);
        }

    }
}
