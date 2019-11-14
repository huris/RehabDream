using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingPlayItemScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void OnEnable()
    {
        if (this.transform.childCount > DoctorDataManager.instance.patient.Train)   // 如果数目大于患者，说明足够存储了，需要把之后的几个给设置未激活
        {
            for (int i = this.transform.childCount - 1; i >= DoctorDataManager.instance.Patients.Count; i--)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
                // Destroy(this.transform.GetChild(i).gameObject);
            }
        }
        else   // 否则说明数目不够，需要再生成几个预制体
        {
            for (int i = this.transform.childCount; i < DoctorDataManager.instance.Patients.Count; i++)
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
