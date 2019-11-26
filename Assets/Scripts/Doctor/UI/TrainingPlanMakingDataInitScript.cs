using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingPlanMakingDataInitScript : MonoBehaviour {

    public Dropdown TrainingDifficult;
    public InputField GameCount;
    public InputField PlanCount;

    public GameObject PlanMakingSuccess;
    public GameObject PlanMakingFail;
    public GameObject PlanDeleteSuccess;
    public GameObject PlanDeleteFail;

    public Toggle PatientInfoManagerToggle;

    public Text PlanMakingButtonText;

    public Dictionary<string, int> DifficultString2Int;
    public Dictionary<int, string> DifficultInt2String;


    // Use this for initialization
    void Start() {

    }

    void OnEnable()
    {
        TrainingDifficult = transform.Find("TrainingDifficult/Dropdown").GetComponent<Dropdown>();
        GameCount = transform.Find("GameCount/InputField").GetComponent<InputField>();
        PlanCount = transform.Find("PlanCount/InputField").GetComponent<InputField>();

        PlanMakingButtonText = transform.Find("PlanMakingButton/Text").GetComponent<Text>();
      
        PlanMakingSuccess = transform.Find("PlanMakingSuccess").gameObject;
        PlanMakingSuccess.SetActive(false);

        PlanMakingFail = transform.Find("PlanMakingFail").gameObject;
        PlanMakingFail.SetActive(false);

        PlanDeleteSuccess = transform.Find("PlanDeleteSuccess").gameObject;
        PlanDeleteSuccess.SetActive(false);

        PlanDeleteFail = transform.Find("PlanDeleteFail").gameObject;
        PlanDeleteFail.SetActive(false);

        PatientInfoManagerToggle = transform.parent.parent.parent.Find("FunctionManager/PatentInfoManagerItem").GetComponent<Toggle>();

        DifficultString2Int = new Dictionary<string, int>();
        DifficultString2Int.Add("请选择难度", 0);
        DifficultString2Int.Add("初级", 1);
        DifficultString2Int.Add("一般", 2);
        DifficultString2Int.Add("中级", 3);
        DifficultString2Int.Add("高级", 4);

        DifficultInt2String = new Dictionary<int, string>();
        DifficultInt2String.Add(0, "请选择难度");
        DifficultInt2String.Add(1, "初级");
        DifficultInt2String.Add(2, "一般");
        DifficultInt2String.Add(3, "中级");
        DifficultInt2String.Add(4, "高级");


        if (DoctorDataManager.instance.patient.trainingPlan.PlanIsMaking)
        {
            TrainingDifficult.value = DifficultString2Int[DoctorDataManager.instance.patient.trainingPlan.PlanDifficulty];
            GameCount.text = DoctorDataManager.instance.patient.trainingPlan.GameCount.ToString();
            PlanCount.text = DoctorDataManager.instance.patient.trainingPlan.PlanCount.ToString();

            PlanMakingButtonText.text = "修改计划";
        }
        else
        {
            TrainingDifficult.value = 0;
            GameCount.text = "";
            PlanCount.text = "";

            PlanMakingButtonText.text = "制定计划";
        }

    }

    // Update is called once per frame
    void Update() {

    }

    public void TrainingPlanMakingButtonOnClick()
    {
        PlanMakingFail.SetActive(false);
        PlanMakingSuccess.SetActive(false);
        PlanDeleteSuccess.SetActive(false);
        PlanDeleteFail.SetActive(false);

        TrainingPlan trainingPlan = new TrainingPlan();
        trainingPlan.SetTrainingPlan(DifficultInt2String[TrainingDifficult.value], long.Parse(GameCount.text), long.Parse(PlanCount.text));

        DoctorDatabaseManager.DatabaseReturn RETURN;  // 返回修改训练计划结果

        if (DoctorDataManager.instance.patient.trainingPlan.PlanIsMaking)
        {
            RETURN = DoctorDatabaseManager.instance.ModifyPatientTrainingPlan(DoctorDataManager.instance.patient.PatientID, trainingPlan);
        }
        else
        {
            RETURN = DoctorDatabaseManager.instance.MakePatientTrainingPlan(DoctorDataManager.instance.patient.PatientID, trainingPlan);
        }

        if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
        {
            DoctorDataManager.instance.patient.trainingPlan.SetPlanIsMaking(true);
            DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetPlanIsMaking(true);

            DoctorDataManager.instance.patient.trainingPlan.SetTrainingPlan(DifficultInt2String[TrainingDifficult.value], long.Parse(GameCount.text), long.Parse(PlanCount.text));
            DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetTrainingPlan(DifficultInt2String[TrainingDifficult.value], long.Parse(GameCount.text), long.Parse(PlanCount.text));

            PlanMakingSuccess.SetActive(true);

            StartCoroutine(DelayTime(3));
        }
        else
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

        if (DoctorDataManager.instance.patient.trainingPlan.PlanIsMaking)
        {
            DoctorDatabaseManager.DatabaseReturn RETURN = DoctorDatabaseManager.instance.DeletePatientTrainingPlan(DoctorDataManager.instance.patient.PatientID);

            if (RETURN == DoctorDatabaseManager.DatabaseReturn.Success)
            {
                DoctorDataManager.instance.patient.trainingPlan.SetPlanIsMaking(false);
                DoctorDataManager.instance.Patients[DoctorDataManager.instance.PatientIndex].trainingPlan.SetPlanIsMaking(false);

                PlanDeleteSuccess.SetActive(true);

                StartCoroutine(DelayTime(3));
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


    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        PatientInfoManagerToggle.isOn = true;
    }

}
