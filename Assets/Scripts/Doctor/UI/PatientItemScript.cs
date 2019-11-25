using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PatientItemScript : MonoBehaviour {

    public GameObject Prefab;

    public Scrollbar PatientListScrollBar;

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientAdd;
    public GameObject PatienPhysicalConditionsQuery;
    public GameObject PatientPasswordModify;
    public GameObject PatientModify;

    public Toggle TrainingConditionQueryToggle;
    public Toggle TrainingPlanMakingToggle;

    public GameObject PatientInfoDelete;
    public Text PatientInfoDeleteText;

    public GameObject PatientPlanDelete;
    public Text PatientPlanDeleteText;

    public GameObject PatientHaveNoPlan;
    public Text PatientHaveNoPlanText;

    void OnEnable()
    {
        // print(DoctorDataManager.instance.Patients.Count);
        DoctorDataManager.instance.Patients = DoctorDataManager.instance.Patients.OrderBy(s => s.PatientPinyin).ToList();

        if(this.transform.childCount > DoctorDataManager.instance.Patients.Count)   // 如果数目大于患者，说明足够存储了，需要把之后的几个给设置未激活
        {
            for (int i=this.transform.childCount-1;i>= DoctorDataManager.instance.Patients.Count; i--)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
                // Destroy(this.transform.GetChild(i).gameObject);
            }
        }
        else   // 否则说明数目不够，需要再生成几个预制体
        {
            for(int i=this.transform.childCount;i< DoctorDataManager.instance.Patients.Count; i++)
            {
                Prefab = Resources.Load("Prefabs/PatientItem") as GameObject;
                Instantiate(Prefab).transform.SetParent(this.transform);
            }
        }
        // 在将列表中的内容放入
        for (int i = 0; i < DoctorDataManager.instance.Patients.Count; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);  // 要设置激活状态

            this.transform.GetChild(i).name = i.ToString();   // 重新命名使得之后可以调用button不同的方法
            
            this.transform.GetChild(i).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientName;
            this.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientSex;
            this.transform.GetChild(i).GetChild(0).GetChild(2).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientAge.ToString();
            this.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientID.ToString();

            // 为button添加监听函数
            this.transform.GetChild(i).GetChild(0).GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(PhysicalConditionsQueryButtonOnClick);  // 查询身体状况
            this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Button>().onClick.AddListener(PatientStartTraining);    // 修改患者密码
            this.transform.GetChild(i).GetChild(0).GetChild(6).GetChild(0).GetComponent<Button>().onClick.AddListener(TrainingConditionQueryButtonOnClick);
            this.transform.GetChild(i).GetChild(0).GetChild(6).GetChild(1).GetComponent<Button>().onClick.AddListener(TrainingPlanMakingButtonOnClick);
            this.transform.GetChild(i).GetChild(0).GetChild(6).GetChild(2).GetComponent<Button>().onClick.AddListener(TrainingPlanDeleteButtonOnClick);
            this.transform.GetChild(i).GetChild(0).GetChild(7).gameObject.GetComponent<Button>().onClick.AddListener(PatientModifyButtonOnClick);  // 查询身体状况
            this.transform.GetChild(i).GetChild(0).GetChild(8).gameObject.GetComponent<Button>().onClick.AddListener(PatientDeleteButtonOnClick);  // 查询身体状况

        }

        //name = childGame.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        //childGame.GetChild(0).GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(OnButtonClick);

        ////获取到父物体，设置为父物体的子物体
        if (DoctorDataManager.instance.Patients.Count <= 7)
        {
            this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1622f, 495.3f);
        }
        else
        {
            this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1622f, 495.3f + (this.transform.childCount - 7) * 70.2f);
        }

        PatientListScrollBar = transform.parent.Find("Scrollbar").GetComponent<Scrollbar>();
        PatientListScrollBar.value = 1;

        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientInfo.SetActive(true);
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
        PatientListBG.SetActive(true);

        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;
        PatientQuery.SetActive(false);
        PatientAdd = transform.parent.parent.Find("PatientAdd").gameObject;
        PatientAdd.SetActive(false);
        PatienPhysicalConditionsQuery = transform.parent.parent.Find("PatienPhysicalConditionsQuery").gameObject;
        PatienPhysicalConditionsQuery.SetActive(false);
        PatientPasswordModify = transform.parent.parent.Find("PatientPasswordModify").gameObject;
        PatientPasswordModify.SetActive(false);
        PatientModify = transform.parent.parent.Find("PatientModify").gameObject;
        PatientModify.SetActive(false);

        PatientInfoDelete = transform.parent.parent.Find("PatientInfoDelete").gameObject;
        PatientInfoDelete.SetActive(false);
        PatientInfoDeleteText = transform.parent.parent.Find("PatientInfoDelete/Text").GetComponent<Text>();

        PatientPlanDelete = transform.parent.parent.Find("PatientPlanDelete").gameObject;
        PatientPlanDelete.SetActive(false);
        PatientPlanDeleteText = transform.parent.parent.Find("PatientPlanDelete/Text").GetComponent<Text>();

        PatientHaveNoPlan = transform.parent.parent.Find("PatientHaveNoPlan").gameObject;
        PatientHaveNoPlan.SetActive(false);
        PatientHaveNoPlanText = transform.parent.parent.Find("PatientHaveNoPlan/Text").GetComponent<Text>();
    }

    void Start()
    {
        if (DoctorDataManager.instance.Patients.Count != 0)
        {
            //print(DoctorDataManager.instance.Patients.Count);
            DoctorDataManager.instance.SetPatientCompleteInformation(0);
            //print(DoctorDataManager.instance.patient.PatientName);
        }
    }

	void Update () {
      
    }
    void PhysicalConditionsQueryButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.name)];
        DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.name));
        DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.name);



        PatientQuery.SetActive(false);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
        PatientAdd.SetActive(false);
        PatienPhysicalConditionsQuery.SetActive(true);
    }

    void PatientStartTraining()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.name));
        DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.name);

        if (DoctorDataManager.instance.patient.trainingPlan.PlanIsMaking)
        {
            //print(DoctorDataManager.instance.patient.PatientID);
            //print(DoctorDataManager.instance.patient.PatientName);
            //print(DoctorDataManager.instance.patient.PatientSex);
            
            PatientDataManager.instance.SetUserMessage(DoctorDataManager.instance.patient.PatientID, DoctorDataManager.instance.patient.PatientName, DoctorDataManager.instance.patient.PatientSex);
            PatientDataManager.instance.SetTrainingPlan(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.patient.trainingPlan.PlanDifficulty), DoctorDataManager.instance.patient.trainingPlan.GameCount, DoctorDataManager.instance.patient.trainingPlan.PlanCount);

            TrainingPlay trainingPlay = new TrainingPlay();
            trainingPlay.SetTrainingID(DoctorDataManager.instance.patient.trainingPlays.Count + 1);

            DoctorDataManager.instance.patient.trainingPlays.Add(trainingPlay);

            PatientDataManager.instance.SetTrainingID(trainingPlay.TrainingID);
            PatientDataManager.instance.SetMaxSuccessCount(DoctorDataManager.instance.patient.MaxSuccessCount);
            SceneManager.LoadScene("Game");  // 如果登录成功,则进入医生管理界面
        }
        else
        {
            PatientHaveNoPlanText.text = "该患者（" + DoctorDataManager.instance.patient.PatientName + "）未制定训练计划\n请先制定计划后开始训练";
            PatientHaveNoPlan.SetActive(true);
        }
    }


    //void PatientPasswordModifyButtonOnClick()
    //{
    //    GameObject obj = EventSystem.current.currentSelectedGameObject;
    //    // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

    //    //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.name)];
    //    DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.name));
    //    DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.name);

    //    PatientInfo.SetActive(false);
    //    PatientListBG.SetActive(false);
    //    PatientPasswordModify.SetActive(true);
    //}

    void TrainingConditionQueryButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        TrainingConditionQueryToggle = transform.parent.parent.parent.parent.Find("FunctionManager/TrainingCoditionQueryItem").GetComponent<Toggle>();
        TrainingConditionQueryToggle.isOn = true;
    }

    void TrainingPlanMakingButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        TrainingPlanMakingToggle = transform.parent.parent.parent.parent.Find("FunctionManager/TrainingPlanMakingItem").GetComponent<Toggle>();
        TrainingPlanMakingToggle.isOn = true;

    }

    void TrainingPlanDeleteButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        DoctorDataManager.instance.TempPatient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        // DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        DoctorDataManager.instance.TempPatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        PatientHaveNoPlanText.text = "该患者（" + DoctorDataManager.instance.TempPatient.PatientName + "）未制定训练计划";
        PatientPlanDeleteText.text = "是否删除患者（" + DoctorDataManager.instance.TempPatient.PatientName + "）训练计划?";

        if (DoctorDataManager.instance.TempPatient.trainingPlan.PlanIsMaking) PatientPlanDelete.SetActive(true);
        else PatientHaveNoPlan.SetActive(true);

        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);

    }

    public void PatientHaveNoPlanTextButtonOnClick()
    {
        PatientHaveNoPlan.SetActive(false);
        PatientPlanDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }

    public void TrainingPlanDeleteExitButtonOnClick()
    {
        PatientHaveNoPlan.SetActive(false);
        PatientPlanDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }

    public void TrainingPlanDeleteYesButtonOnclick()
    {
        DoctorDatabaseManager.instance.DeletePatientTrainingPlan(DoctorDataManager.instance.TempPatient.PatientID);

        DoctorDataManager.instance.Patients[DoctorDataManager.instance.TempPatientIndex].trainingPlan.SetPlanIsMaking(false);

        if (DoctorDataManager.instance.patient.PatientID == DoctorDataManager.instance.TempPatient.PatientID) {
            DoctorDataManager.instance.patient = DoctorDataManager.instance.TempPatient;
            DoctorDataManager.instance.PatientIndex = DoctorDataManager.instance.TempPatientIndex;
        }

        PatientPlanDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }



    void PatientModifyButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        DoctorDataManager.instance.TempPatient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.name)];
        DoctorDataManager.instance.TempPatientIndex = int.Parse(obj.transform.parent.parent.name);

        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
        PatientModify.SetActive(true);

    }

    void PatientDeleteButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        DoctorDataManager.instance.TempPatient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.name)];

        PatientInfoDeleteText.text = "是否删除患者（"+ DoctorDataManager.instance.TempPatient.PatientName+ "）信息?";

        PatientInfoDelete.SetActive(true);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
    }

    public void PatientDeleteExitButtonOnClick()
    {
        PatientInfoDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }

    public void PatientDeleteYesButtonOnclick()
    {
        DoctorDatabaseManager.instance.PatientDelete(DoctorDataManager.instance.TempPatient.PatientID);

        DoctorDataManager.instance.Patients.Remove(DoctorDataManager.instance.TempPatient);

        if (DoctorDataManager.instance.patient.PatientID == DoctorDataManager.instance.TempPatient.PatientID)
        {
            DoctorDataManager.instance.SetPatientCompleteInformation(0);
        }

        PatientInfoDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}



