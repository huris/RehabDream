using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PatientItemScript : MonoBehaviour
{

    public GameObject Prefab;

    public Scrollbar PatientListScrollBar;

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientAdd;
    public GameObject PatienPhysicalConditionsQuery;
    public GameObject PatientSymptomModify;
    public GameObject PatientModify;

    public Toggle TrainingConditionQueryToggle;
    public Toggle TrainingPlanMakingToggle;
    public Toggle PatientEvaluationToggle;

    public GameObject PatientInfoDelete;
    public Text PatientInfoDeleteText;

    public GameObject PatientPlanDelete;
    public Text PatientPlanDeleteText;

    public GameObject PatientHaveNoPlan;
    public Text PatientHaveNoPlanText;

    public GameObject NoPatient;

    public GameObject PatentInfoManagerUI;

    public Image PatientAddImage;
    public Image PatientAllImage;
    public Image PatientQueryImage;

    public Toggle PatientInfoEvaluateItem;
    public Toggle TrainingCoditionQueryItem;

    

    void OnEnable()
    {
        //print(DoctorDatabaseManager.instance.ReadMaxEvaluationID());

        NoPatient = transform.parent.parent.Find("NoPatient").gameObject;

        PatientAddImage = transform.parent.parent.Find("PatientAddButton").GetComponent<Image>();
        PatientAddImage.color = Color.white;

        PatientAllImage = transform.parent.parent.Find("PatientAllButton").GetComponent<Image>();
        PatientAllImage.color = Color.white;

        PatientQueryImage = transform.parent.parent.Find("PatientQueryButton").GetComponent<Image>();
        PatientQueryImage.color = Color.white;

        if (DoctorDataManager.instance.doctor.Patients != null && DoctorDataManager.instance.doctor.Patients.Count > 0)
        {
            NoPatient.SetActive(false);
            // print(DoctorDataManager.instance.Patients.Count);
            //DoctorDataManager.instance.doctor.Patients = DoctorDataManager.instance.doctor.Patients.OrderBy(s => s.PatientPinyin).ToList();
            //DoctorDataManager.instance.Doctors = DoctorDataManager.instance.Doctors.OrderBy(s => s.DoctorPinyin).ToList();

            if (this.transform.childCount > DoctorDataManager.instance.doctor.Patients.Count)   // 如果数目大于患者，说明足够存储了，需要把之后的几个给设置未激活
            {
                for (int i = this.transform.childCount - 1; i >= DoctorDataManager.instance.doctor.Patients.Count; i--)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                    // Destroy(this.transform.GetChild(i).gameObject);
                }
            }
            else   // 否则说明数目不够，需要再生成几个预制体
            {
                for (int i = this.transform.childCount; i < DoctorDataManager.instance.doctor.Patients.Count; i++)
                {
                    Prefab = Resources.Load("Prefabs/PatientItem") as GameObject;
                    Instantiate(Prefab).transform.SetParent(this.transform);


                    // 为button添加监听函数
                    this.transform.GetChild(i).GetChild(0).GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(PhysicalConditionsQueryButtonOnClick);  // 查询身体状况
                                                                                                                                                                      //this.transform.GetChild(i).GetChild(0).GetChild(4).GetChild(1).GetComponent<Button>().onClick.AddListener(PhysicalConditionsEvaluateButtonOnClick);  // 评估身体状况
                                                                                                                                                                      //this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Button>().onClick.AddListener(PatientStartTraining);    // 修改患者密码                                                                                                                                              //this.transform.GetChild(i).GetChild(0).GetChild(6).GetChild(2).GetComponent<Button>().onClick.AddListener(TrainingPlanDeleteButtonOnClick);

                    //this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().ClearOptions();

                    this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().AddOptions(new List<string> { "动作姿势", "Bobath", "评估类型" });
                    this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().value = this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().options.Count - 1;

                    this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate
                    {
                        GameObject obj = EventSystem.current.currentSelectedGameObject;
                        //print(obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().value);

                        if (obj.transform.parent.name == "Content" && obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().value < obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().options.Count - 1)
                        {
                            DoctorDataManager.instance.doctor.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.parent.parent.parent.name));

                            DoctorDataManager.instance.EvaluationType = obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().value;

                            StartCoroutine(EvaluateDelayTime(0.15f));   // 经测试，0.15秒刚好使dropdownlist消失
                            //print(DoctorDataManager.instance.EvaluationType);

                            //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
                            //DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.parent.name);
                            //obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().value = obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().options.Count - 1;

                            //obj.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.Find("FunctionManager/PatientInfoEvaluateItem").GetComponent<Toggle>().isOn = true;
                        }
                    });
                   
                    this.transform.GetChild(i).GetChild(0).GetChild(6).gameObject.GetComponent<Dropdown>().AddOptions(new List<string> { "足球守门", "重心捕鱼", "训练类型" });
                    this.transform.GetChild(i).GetChild(0).GetChild(6).gameObject.GetComponent<Dropdown>().value = this.transform.GetChild(i).GetChild(0).GetChild(6).gameObject.GetComponent<Dropdown>().options.Count - 1;

                    this.transform.GetChild(i).GetChild(0).GetChild(6).gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate {
                        GameObject obj = EventSystem.current.currentSelectedGameObject;

                        if (obj.transform.parent.name == "Content" && obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().value < obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().options.Count - 1)
                        {
                            DoctorDataManager.instance.doctor.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.parent.parent.parent.name));

                            DoctorDataManager.instance.TrainingType = obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().value;

                            //print(DoctorDataManager.instance.EvaluationType);

                            StartCoroutine(TrainingDelayTime(0.15f));   // 经测试，0.15秒刚好使dropdownlist消失

                            //obj.transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.Find("FunctionManager/TrainingCoditionQueryItem").GetComponent<Toggle>().isOn = true;

                            //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
                            //DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.parent.name);
                            //obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().value = obj.transform.parent.parent.parent.parent.gameObject.GetComponent<Dropdown>().options.Count - 1;

                        }
                    });

                    this.transform.GetChild(i).GetChild(0).GetChild(8).gameObject.GetComponent<Button>().onClick.AddListener(PatientModifyButtonOnClick);  // 查询身体状况
                    this.transform.GetChild(i).GetChild(0).GetChild(9).gameObject.GetComponent<Button>().onClick.AddListener(PatientDeleteButtonOnClick);  // 查询身体状况

                }
            }
            // 在将列表中的内容放入
            for (int i = 0; i < DoctorDataManager.instance.doctor.Patients.Count; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);  // 要设置激活状态

                this.transform.GetChild(i).name = i.ToString();   // 重新命名使得之后可以调用button不同的方法

                this.transform.GetChild(i).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.Patients[i].PatientName;
                this.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.Patients[i].PatientID.ToString();
                this.transform.GetChild(i).GetChild(0).GetChild(2).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.Patients[i].PatientSex;
                this.transform.GetChild(i).GetChild(0).GetChild(3).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.Patients[i].PatientAge.ToString();

                //if (this.transform.GetChild(i).GetChild(0).GetChild(5).childCount == 4)
                //{
                //GameObject.Destroy(this.transform.GetChild(i).GetChild(0).GetChild(5).GetChild(3));
                this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().value = this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().options.Count - 1;
                this.transform.GetChild(i).GetChild(0).GetChild(6).gameObject.GetComponent<Dropdown>().value = this.transform.GetChild(i).GetChild(0).GetChild(6).gameObject.GetComponent<Dropdown>().options.Count - 1;
                //this.transform.GetChild(i).GetChild(0).GetChild(5).gameObject.GetComponent<Dropdown>().value = 1;
                //}

                //Doctor doctor = DoctorDatabaseManager.instance.ReadDoctorIDInfo(DoctorDataManager.instance.doctor.Patients[i].PatientDoctorID);
                this.transform.GetChild(i).GetChild(0).GetChild(7).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.PatientDoctorName;

            }

            //name = childGame.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
            //childGame.GetChild(0).GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(OnButtonClick);

            ////获取到父物体，设置为父物体的子物体
            if (DoctorDataManager.instance.doctor.Patients.Count <= 7)
            {
                this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1622f, 495.3f);
            }
            else
            {
                this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1622f, 495.3f + (DoctorDataManager.instance.doctor.Patients.Count - 7) * 70.2f);
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
            PatientSymptomModify = transform.parent.parent.Find("PatientSymptomModify").gameObject;
            PatientSymptomModify.SetActive(false);
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

            PatentInfoManagerUI = transform.parent.parent.gameObject;
        }
        else
        {
            NoPatient.SetActive(true);
        }

    }

    void Start()
    {
        //if (DoctorDataManager.instance.Patients.Count > 0)
        //{
        //    //print(DoctorDataManager.instance.Patients.Count);
        //    DoctorDataManager.instance.SetPatientCompleteInformation(0);
        //    //print(DoctorDataManager.instance.patient.PatientName);
        //}
    }

    void Update()
    {

    }

    IEnumerator EvaluateDelayTime(float time)
    {
        yield return new WaitForSeconds(time);
        PatientInfoEvaluateItem.isOn = true;
    }

    IEnumerator TrainingDelayTime(float time)
    {
        yield return new WaitForSeconds(time);
        TrainingCoditionQueryItem.isOn = true;
    }

    void PatientStartTraining()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        DoctorDataManager.instance.doctor.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.name));
        //DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.name);

        DoctorDataManager.instance.doctor.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(DoctorDataManager.instance.doctor.patient.PatientID);
        if (DoctorDataManager.instance.doctor.patient.trainingPlan != null) DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(true);
        else DoctorDataManager.instance.doctor.patient.SetPlanIsMaking(false);

        if (DoctorDataManager.instance.doctor.patient.PlanIsMaking)
        {
            //print(DoctorDataManager.instance.patient.PatientID);
            //print(DoctorDataManager.instance.patient.PatientName);
            //print(DoctorDataManager.instance.patient.PatientSex);

            PatientDataManager.instance.SetUserMessage(DoctorDataManager.instance.doctor.patient.PatientID, DoctorDataManager.instance.doctor.patient.PatientName, DoctorDataManager.instance.doctor.patient.PatientSex);
            //PatientDataManager.instance.SetTrainingPlan(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.patient.trainingPlan.PlanDifficulty), DoctorDataManager.instance.patient.trainingPlan.GameCount, DoctorDataManager.instance.patient.trainingPlan.PlanCount);

            //TrainingPlay trainingPlay = new TrainingPlay();
            //trainingPlay.SetTrainingID(DoctorDatabaseManager.instance.ReadPatientRecordCount(0) + DoctorDatabaseManager.instance.ReadPatientRecordCount(1));

            //DoctorDataManager.instance.doctor.patient.TrainingPlays.Add(trainingPlay);

            TrainingPlay trainingPlay = new TrainingPlay();
            trainingPlay.SetTrainingID(DoctorDatabaseManager.instance.ReadPatientRecordCount(0));
            trainingPlay.SetTrainingDifficulty(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty);

            //print("!!!!!");

            if (DoctorDataManager.instance.doctor.patient.Evaluations == null)
            {
                DoctorDataManager.instance.doctor.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

                if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
                {
                    DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);
                }
            }

            PatientDataManager.instance.SetTrainingData(trainingPlay, DoctorDataManager.instance.doctor.patient.trainingPlan, DoctorDataManager.instance.doctor.patient.Evaluations.Last().soccerDistance, DoctorDataManager.instance.doctor.patient.MaxSuccessCount);

            //PatientDataManager.instance.SetTrainingID(trainingPlay.TrainingID);
            //PatientDataManager.instance.SetMaxSuccessCount(DoctorDataManager.instance.doctor.patient.MaxSuccessCount);
            //PatientDataManager.instance.SetPlanDifficulty(PatientDataManager.Str2DifficultyType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDifficulty));
            //PatientDataManager.instance.SetPlanCount(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanCount);
            //PatientDataManager.instance.SetPlanDirection(PatientDataManager.Str2DirectionType(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanDirection));
            //PatientDataManager.instance.SetPlanTime(DoctorDataManager.instance.doctor.patient.trainingPlan.PlanTime);
            //PatientDataManager.instance.SetIsEvaluated(0);

            DoctorDataManager.instance.FunctionManager = 3; // 返回的时候进入训练状况查询界面
            SceneManager.LoadScene("06-Game");  // 如果登录成功,则进入医生管理界面
        }
        else
        {
            PatientHaveNoPlanText.text = "该患者（" + DoctorDataManager.instance.doctor.patient.PatientName + "）未制定训练计划\n请先制定计划后开始训练";
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

    void PhysicalConditionsQueryButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.name)];
        DoctorDataManager.instance.doctor.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        //DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.name);


        PatientQuery.SetActive(false);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
        PatientAdd.SetActive(false);
        PatienPhysicalConditionsQuery.SetActive(true);
    }

    void TrainingConditionQueryButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        DoctorDataManager.instance.doctor.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        //DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        TrainingConditionQueryToggle = transform.parent.parent.parent.parent.Find("FunctionManager/TrainingCoditionQueryItem").GetComponent<Toggle>();
        TrainingConditionQueryToggle.isOn = true;
    }

    void PhysicalConditionsEvaluateButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        DoctorDataManager.instance.doctor.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        //DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        PatientEvaluationToggle = transform.parent.parent.parent.parent.Find("FunctionManager/PatientInfoEvaluateItem").GetComponent<Toggle>();
        PatientEvaluationToggle.isOn = true;
    }

    void TrainingPlanMakingButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        DoctorDataManager.instance.doctor.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        //DoctorDataManager.instance.PatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        TrainingPlanMakingToggle = transform.parent.parent.parent.parent.Find("FunctionManager/TrainingPlanMakingItem").GetComponent<Toggle>();
        TrainingPlanMakingToggle.isOn = true;

    }

    void TrainingPlanDeleteButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.doctor.TempPatient = DoctorDataManager.instance.doctor.Patients[int.Parse(obj.transform.parent.parent.parent.name)];
        DoctorDataManager.instance.doctor.SetTempPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        // DoctorDataManager.instance.SetPatientCompleteInformation(int.Parse(obj.transform.parent.parent.parent.name));
        //DoctorDataManager.instance.TempPatientIndex = int.Parse(obj.transform.parent.parent.parent.name);

        PatientHaveNoPlanText.text = "该患者（" + DoctorDataManager.instance.doctor.TempPatient.PatientName + "）未制定训练计划";
        PatientPlanDeleteText.text = "是否删除患者（" + DoctorDataManager.instance.doctor.TempPatient.PatientName + "）训练计划?";

        if (DoctorDataManager.instance.doctor.TempPatient.PlanIsMaking) PatientPlanDelete.SetActive(true);
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
        DoctorDatabaseManager.instance.DeletePatientTrainingPlan(DoctorDataManager.instance.doctor.TempPatient.PatientID);
        DoctorDataManager.instance.doctor.TempPatient.SetPlanIsMaking(false);

        //DoctorDataManager.instance.doctor.Patients[DoctorDataManager.instance.doctor.TempPatientIndex].trainingPlan.SetPlanIsMaking(false);

        //if (DoctorDataManager.instance.doctor.patient.PatientID == DoctorDataManager.instance.doctor.TempPatient.PatientID) {
        //    DoctorDataManager.instance.doctor.patient = DoctorDataManager.instance.doctor.TempPatient;
        //    DoctorDataManager.instance.doctor.PatientIndex = DoctorDataManager.instance.doctor.TempPatientIndex;
        //}

        PatientPlanDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }



    void PatientModifyButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        DoctorDataManager.instance.doctor.SetTempPatientCompleteInformation(int.Parse(obj.transform.parent.parent.name));
        //DoctorDataManager.instance.TempPatientIndex = int.Parse(obj.transform.parent.parent.name);

        //print(DoctorDataManager.instance.TempPatient.PatientSymptom);

        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
        PatientModify.SetActive(true);

    }

    void PatientDeleteButtonOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        DoctorDataManager.instance.doctor.SetTempPatientCompleteInformation(int.Parse(obj.transform.parent.parent.name));

        PatientInfoDeleteText.text = "是否删除患者（" + DoctorDataManager.instance.doctor.TempPatient.PatientName + "）信息?";

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
        DoctorDatabaseManager.instance.PatientDelete(DoctorDataManager.instance.doctor.TempPatient.PatientID);

        DoctorDataManager.instance.doctor.Patients.Remove(DoctorDataManager.instance.doctor.TempPatient);

        if (DoctorDataManager.instance.doctor.PatientIndex == DoctorDataManager.instance.doctor.TempPatientIndex)
        {
            if (DoctorDataManager.instance.doctor.Patients.Count > 0)
            {
                DoctorDataManager.instance.doctor.SetPatientCompleteInformation(0);
            }
            else
            {
                PatentInfoManagerUI.SetActive(false);
                PatentInfoManagerUI.SetActive(true);
            }
        }

        if (DoctorDataManager.instance.doctor.Patients.Count == 0)
        {
            DoctorDataManager.instance.doctor.patient = null;
            //print("!!!!!");
        }


        PatientInfoDelete.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
