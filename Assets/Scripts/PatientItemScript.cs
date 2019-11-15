using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    void OnEnable()
    {
        // print(DoctorDataManager.instance.Patients.Count);

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
            this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Button>().onClick.AddListener(PatientPasswordModifyButtonOnClick);    // 修改患者密码
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

        PatientQuery = transform.parent.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
        PatientAdd = transform.parent.parent.Find("PatientAdd").gameObject;
        PatienPhysicalConditionsQuery = transform.parent.parent.Find("PatienPhysicalConditionsQuery").gameObject;
        PatientPasswordModify = transform.parent.parent.Find("PatientPasswordModify").gameObject;
        PatientModify = transform.parent.parent.Find("PatientModify").gameObject;

        PatientInfoDelete = transform.parent.parent.Find("PatientInfoDelete").gameObject;
        PatientInfoDeleteText = transform.parent.parent.Find("PatientInfoDelete/Text").GetComponent<Text>();

        PatientPlanDelete = transform.parent.parent.Find("PatientPlanDelete").gameObject;
        PatientPlanDeleteText = transform.parent.parent.Find("PatientPlanDelete/Text").GetComponent<Text>();
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

    void PatientPasswordModifyButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.name)];
        DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.name));
        DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.name);

        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
        PatientPasswordModify.SetActive(true);
    }

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

        //DoctorDataManager.instance.TempPatient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        DoctorDataManager.instance.TempPatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        PatientInfoDeleteText.text = "是否删除病人（" + DoctorDataManager.instance.TempPatient.PatientName + "）训练计划?";

        PatientPlanDelete.SetActive(true);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);

    }

    public void TrainingPlanDeleteExitButtonOnClick()
    {
        PatientPlanDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }

    public void TrainingPlanDeleteYesButtonOnclick()
    {
        DoctorDatabaseManager.instance.DeletePatientTrainingPlan(DoctorDataManager.instance.TempPatient.PatientID);

        DoctorDataManager.instance.Patients[DoctorDataManager.instance.TempPatientIndex].trainingPlan.SetPlanIsMaking(false);

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

        PatientInfoDeleteText.text = "是否删除病人（"+ DoctorDataManager.instance.TempPatient.PatientName+ "）信息?";

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

        PatientInfoDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}



