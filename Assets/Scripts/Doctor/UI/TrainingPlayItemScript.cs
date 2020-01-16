using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainingPlayItemScript : MonoBehaviour {

    public GameObject Prefab;

    public Scrollbar TrainingPlayListScrollBar;

    public Dropdown Directions;
    List<string> directions = new List<string>();

    public Toggle LastTrainingToggle;
    // Use this for initialization
    void Start() {
        
    }
    
    void OnEnable()
    {
        LastTrainingToggle = transform.parent.parent.parent.parent.Find("DataBG/LastTrainingData").GetComponent<Toggle>();

        Directions = transform.parent.parent.Find("Directions").GetComponent<Dropdown>();
        Directions.ClearOptions();

        directions.Clear();
        directions.Add("正上");
        directions.Add("右上");
        directions.Add("正右");
        directions.Add("右下");
        directions.Add("正下");
        directions.Add("左下");
        directions.Add("正左");
        directions.Add("左上");

        Directions.AddOptions(directions);

        Directions.value = 0;

        //DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);

        if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
        {
            //print(DoctorDataManager.instance.patient.TrainingPlays.Count+"!!!!");

            if (this.transform.childCount > DoctorDataManager.instance.doctor.patient.TrainingPlays.Count)   // 如果数目大于训练数据，说明足够存储了，需要把之后的几个给设置未激活
            {
                for (int i = this.transform.childCount - 1; i >= DoctorDataManager.instance.doctor.patient.TrainingPlays.Count; i--)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                    // Destroy(this.transform.GetChild(i).gameObject);
                }
            }
            else   // 否则说明数目不够，需要再生成几个预制体
            {
                for (int i = this.transform.childCount; i < DoctorDataManager.instance.doctor.patient.TrainingPlays.Count; i++)
                {
                    Prefab = Resources.Load("Prefabs/TrainingPlay") as GameObject;
                    Instantiate(Prefab).transform.SetParent(this.transform);
                }
            }

            // 在将列表中的内容放入
            for (int i = 0; i < DoctorDataManager.instance.doctor.patient.TrainingPlays.Count; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);  // 要设置激活状态

                this.transform.GetChild(i).name = i.ToString();   // 重新命名为0,1,2,3,4...

                this.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(QuerySingleTrainingButtonOnClick);  // 查询身体状况

                this.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().text = (i+1).ToString();
                this.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingStartTime;
                this.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingDifficulty;
                this.transform.GetChild(i).GetChild(3).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingDirection;
                this.transform.GetChild(i).GetChild(4).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingTime.ToString();

                float TrainingSuccessRate = 100.0f * DoctorDataManager.instance.doctor.patient.TrainingPlays[i].SuccessCount / DoctorDataManager.instance.doctor.patient.TrainingPlays[i].GameCount;
                this.transform.GetChild(i).GetChild(5).gameObject.GetComponent<Text>().text = TrainingSuccessRate.ToString("0.00") + "%";

                this.transform.GetChild(i).GetChild(6).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.DirectionRadarArea.ToString("0.00");

                this.transform.GetChild(i).GetChild(7).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.GetDirectionsArray()[Directions.value].ToString();


                //string TrainingEvaluation = "";
                //double TrainingEvaluationRate = 1.0 * DoctorDataManager.instance.patient.TrainingPlays[i].SuccessCount / DoctorDataManager.instance.patient.TrainingPlays[i].GameCount;

                //if (TrainingEvaluationRate >= 0.95) TrainingEvaluation = "S";
                //else if (TrainingEvaluationRate >= 0.90) TrainingEvaluation = "A";
                //else if (TrainingEvaluationRate >= 0.80) TrainingEvaluation = "B";
                //else if (TrainingEvaluationRate >= 0.70) TrainingEvaluation = "C";
                //else if (TrainingEvaluationRate >= 0.60) TrainingEvaluation = "D";
                //else TrainingEvaluation = "E";

                //this.transform.GetChild(i).GetChild(4).gameObject.GetComponent<Text>().text = TrainingEvaluation;
            }

            if (DoctorDataManager.instance.doctor.patient.TrainingPlays.Count <= 7)
            {
                this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1222f, 270.36f);
            }
            else
            {
                this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1222f, 270.36f + (this.transform.childCount - 7) * 39.4f);
            }

            TrainingPlayListScrollBar = transform.parent.Find("Scrollbar").GetComponent<Scrollbar>();
            TrainingPlayListScrollBar.value = 1;

        }
    }

    public void QuerySingleTrainingButtonOnClick() {

        GameObject obj = EventSystem.current.currentSelectedGameObject;
        // print(obj.transform.parent.parent.name);  // obj.transform.parent.parent.name为当前按钮的编号

        //DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[int.Parse(obj.transform.parent.parent.name)];
        DoctorDataManager.instance.doctor.patient.SetTrainingPlayIndex(int.Parse(obj.transform.name));
        //TrainingPlay trainingPlay = DoctorDataManager.instance.doctor.patient.TrainingPlays[int.Parse(obj.transform.name)];
        //DoctorDataManager.instance.doctor.patient.TrainingPlays[int.Parse(obj.transform.name)] = DoctorDataManager.instance.doctor.patient.TrainingPlays[DoctorDataManager.instance.doctor.patient.TrainingPlays.Count - 1];
        //DoctorDataManager.instance.doctor.patient.TrainingPlays[DoctorDataManager.instance.doctor.patient.TrainingPlays.Count-1] = trainingPlay;

        LastTrainingToggle.isOn = true;
    }

    public void DirectionValueChange()
    {
        for (int i = 0; i < DoctorDataManager.instance.doctor.patient.TrainingPlays.Count; i++)
        {
            this.transform.GetChild(i).GetChild(7).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.GetDirectionsArray()[Directions.value].ToString();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
