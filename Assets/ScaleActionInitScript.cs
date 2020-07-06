using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScaleActionInitScript : MonoBehaviour {

	public Dropdown ScaleSelect;

	public Dictionary<int, int> ScaleType2Int = new Dictionary<int, int>();
	public Dictionary<int, int> ScaleInt2Type = new Dictionary<int, int>();
	public List<string> ListScaleEvaluation = new List<string>();

	public List<int> ScaleID;

	public Text ActionNumText;

	public Dropdown ActionSpeedDropdown;

    public GameObject Prefab;

    void OnEnable()
	{
		ScaleID = new List<int>(DATA.TrainingProgramIDToName.Keys);

		ScaleType2Int.Clear();
		ScaleInt2Type.Clear();
		ListScaleEvaluation.Clear();
		ScaleSelect.ClearOptions();

		for(int i = 0; i < ScaleID.Count; i++)
		{
			ScaleType2Int.Add(ScaleID[i], i);
			ScaleInt2Type.Add(i, ScaleID[i]);
			ListScaleEvaluation.Add(DATA.TrainingProgramIDToName[ScaleID[i]]);
		}

		ListScaleEvaluation.Add("自定义");

		ScaleSelect.AddOptions(ListScaleEvaluation);

		int UserID = -1;
		for(int i = 0; i < DoctorDataManager.instance.users.Count; i++)
		{
			if(DoctorDataManager.instance.doctor.patient.PatientID == DoctorDataManager.instance.users[i].ID)
			{
				UserID = i;
				break;
			}
		}

       
        if (this.transform.GetChild(4).GetChild(0).GetChild(0).childCount > DATA.actionList.Count)   // 如果数目大于训练数据，说明足够存储了，需要把之后的几个给设置未激活
        {
            for (int i = this.transform.GetChild(4).GetChild(0).GetChild(0).childCount - 1; i >= DATA.actionList.Count; i--)
            {
                this.transform.GetChild(i).GetChild(4).GetChild(0).GetChild(0).gameObject.SetActive(false);
                //Destroy(this.transform.GetChild(i).gameObject);
            }
        }
        else   // 否则说明数目不够，需要再生成几个预制体
        {
            for (int i = this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; i < DATA.actionList.Count; i++)
            {
                Prefab = Resources.Load("Prefabs/ActionImageItem") as GameObject;
                Instantiate(Prefab).transform.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
            }
        }

        //List<int> ActionID = new List<int>(DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Keys);
        // 在将列表中的内容放入
        for (int i = 0; i < DATA.actionList.Count; i++)
        {
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);  // 要设置激活状态

            this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name = i.ToString();   // 重新命名为0,1,2,3,4...

            //this.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(QuerySingleTrainingButtonOnClick);  // 查询身体状况

            Image image = this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(0).gameObject.GetComponent<Image>();

            StartCoroutine(new Utils().Load(image, Environment.CurrentDirectory + DATA.actionList[i].filename));

            this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = DATA.actionList[i].name;
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = DATA.actionList[i].createTime;
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = "1";

        }

        if (DATA.actionList.Count <= 14)
        {
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f);
        }
        else
        {
            //print(this.transform.childCount);
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f + ((DATA.actionList.Count - 14) / 7 + 1) * 176f);
        }

		if(UserID == -1)   // 说明该患者尚未制定评估方案
		{
			ScaleSelect.value = 0;
			ActionNumText.text = "0";
			ActionSpeedDropdown.value = 0;



		}
		else
		{

		}

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
