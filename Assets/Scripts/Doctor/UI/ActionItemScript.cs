using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionItemScript : MonoBehaviour {


    public GameObject Prefab;

    public Scrollbar TrainingPlayListScrollBar;

    // Use this for initialization
    void Start()
    {

    }

    public void ActionItemInit()
    {
        if (DoctorDataManager.instance.doctor.patient.WallEvaluations == null)
        {
            DoctorDataManager.instance.doctor.patient.WallEvaluations = DoctorDatabaseManager.instance.ReadPatientWallEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

            if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
            {
                DoctorDataManager.instance.doctor.patient.SetWallEvaluationIndex(DoctorDataManager.instance.doctor.patient.WallEvaluations.Count - 1);
            }
        }

        if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
        {
            int WallEvaluationIndex = DoctorDataManager.instance.doctor.patient.WallEvaluationIndex;

            if (this.transform.childCount > DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Count)   // 如果数目大于训练数据，说明足够存储了，需要把之后的几个给设置未激活
            {
                for (int i = this.transform.childCount - 1; i >= DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Count; i--)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                    //Destroy(this.transform.GetChild(i).gameObject);
                }
            }
            else   // 否则说明数目不够，需要再生成几个预制体
            {
                for (int i = this.transform.childCount; i < DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Count; i++)
                {
                    Prefab = Resources.Load("Prefabs/ActionItem") as GameObject;
                    Instantiate(Prefab).transform.SetParent(this.transform);
                }
            }

            List<int> ActionID = new List<int>(DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Keys);
            // 在将列表中的内容放入
            for (int i = 0; i < DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Count; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);  // 要设置激活状态

                this.transform.GetChild(i).name = i.ToString();   // 重新命名为0,1,2,3,4...

                //this.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(QuerySingleTrainingButtonOnClick);  // 查询身体状况

                this.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().text = "A" + (i + 1).ToString();

                for (int z = 0; z < DoctorDataManager.instance.Actions.Count; z++)
                {
                    if (DoctorDataManager.instance.Actions[z].id == ActionID[i])
                    {
                        this.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Actions[z].name;
                        this.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Actions[z].id.ToString();
                        this.transform.GetChild(i).GetChild(3).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas[ActionID[i]].accuracy.ToString() + " %";
                        this.transform.GetChild(i).GetChild(4).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas[ActionID[i]].passPercent.ToString() + " %";
                        break;
                    }
                }
                //this.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingDifficulty;
                //this.transform.GetChild(i).GetChild(3).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingDirection;
                //this.transform.GetChild(i).GetChild(4).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingTime.ToString();

                //float TrainingSuccessRate = 100.0f * DoctorDataManager.instance.doctor.patient.TrainingPlays[i].SuccessCount / DoctorDataManager.instance.doctor.patient.TrainingPlays[i].GameCount;
                //this.transform.GetChild(i).GetChild(5).gameObject.GetComponent<Text>().text = TrainingSuccessRate.ToString("0.00") + "%";

                //this.transform.GetChild(i).GetChild(6).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.DirectionRadarArea.ToString("0.00");

                //this.transform.GetChild(i).GetChild(7).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].direction.GetDirectionsArray()[Directions.value].ToString();


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

            if (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Count <= 5)
            {
                this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(840.2f, 183.98f);
            }
            else
            {
                //print(this.transform.childCount);
                this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(840.2f, 183.98f + (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Count - 5) * 42f);
            }

            TrainingPlayListScrollBar = transform.parent.Find("Scrollbar").GetComponent<Scrollbar>();
            TrainingPlayListScrollBar.value = 1;

        }
    }

    void OnEnable()
    {
        ActionItemInit();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

