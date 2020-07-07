using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ScaleActionInitScript : MonoBehaviour {

	public Dropdown ScaleSelect;

	public Dictionary<int, int> ScaleType2Int = new Dictionary<int, int>();
	public Dictionary<int, int> ScaleInt2Type = new Dictionary<int, int>();
	public List<string> ListScaleEvaluation = new List<string>();

	public List<int> ScaleID;

	public Text ActionNumText;

	public Dropdown ActionSpeedDropdown;

    public GameObject ActionPrefab;

	public Text AllActionText;
	public Scrollbar AllActionScrollbar;

	public Text ScaleActionText;


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

		AllActionText.text = "所有动作（" + DoctorDataManager.instance.Actions.Count.ToString() + "）";

		// 所有动作的item赋值
        if (this.transform.GetChild(4).GetChild(0).GetChild(0).childCount > DoctorDataManager.instance.Actions.Count)   // 如果数目大于训练数据，说明足够存储了，需要把之后的几个给设置未激活
        {
            for (int i = this.transform.GetChild(4).GetChild(0).GetChild(0).childCount - 1; i >= DoctorDataManager.instance.Actions.Count; i--)
            {
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
                //Destroy(this.transform.GetChild(i).gameObject);
            }
        }
        else   // 否则说明数目不够，需要再生成几个预制体
        {
            for (int i = this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; i < DoctorDataManager.instance.Actions.Count; i++)
            {
				ActionPrefab = Resources.Load("Prefabs/ActionImageItem") as GameObject;
                Instantiate(ActionPrefab).transform.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
            }
        }

        //List<int> ActionID = new List<int>(DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Keys);
        // 在将列表中的内容放入
        for (int i = 0; i < DoctorDataManager.instance.Actions.Count; i++)
        {
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);  // 要设置激活状态

            this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name = i.ToString();   // 重新命名为0,1,2,3,4...

            //this.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(QuerySingleTrainingButtonOnClick);  // 查询身体状况

            Image image = this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(0).gameObject.GetComponent<Image>();

			// 速度很慢
			StartCoroutine(new Utils().Load(image, Environment.CurrentDirectory + DoctorDataManager.instance.Actions[i].filename));

			this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = DoctorDataManager.instance.Actions[i].name;
			if(DoctorDataManager.instance.Actions[i].name.Length == 6)
			{
				this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().fontSize = 13;
			}else if(DoctorDataManager.instance.Actions[i].name.Length > 6)
			{
				this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().fontSize = 12;
			}

			string CreatTime = DoctorDataManager.instance.Actions[i].createTime;
			this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = CreatTime.Substring(0,4) + CreatTime.Substring(5,2) + CreatTime.Substring(8,2);
            
			this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = "1";

			this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(4).GetComponent<Button>().onClick.AddListener(ActionUponArrowOnClick);
			this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(5).GetComponent<Button>().onClick.AddListener(ActionDownArrowOnClick);

		}

		if (DoctorDataManager.instance.Actions.Count <= 14)
        {
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f);
        }
        else
        {
            //print(this.transform.childCount);
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f + ((DoctorDataManager.instance.Actions.Count - 14) / 7 + 1) * 185f);
        }


		if (UserID == -1)   // 说明该患者尚未制定评估方案
		{
			ScaleSelect.value = 0;
			ActionNumText.text = "0";
			ActionSpeedDropdown.value = 0;

			ScaleActionText.text = "量表动作（" + DoctorDataManager.instance.Actions.Count.ToString() + "）"; ;


		}
		else
		{
			ScaleActionText.text = "量表动作（" + DoctorDataManager.instance.Actions.Count.ToString() + "）"; ;

		}

	}

	public void ActionUponArrowOnClick()
	{
		GameObject obj = EventSystem.current.currentSelectedGameObject;
	
		if(long.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) < 100)
		{
			obj.transform.parent.GetChild(3).GetComponent<InputField>().text = (long.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) + 1).ToString();

			ActionNumText.text = (long.Parse(ActionNumText.text) + 1).ToString();
		}
	}

	public void ActionDownArrowOnClick()
	{
		GameObject obj = EventSystem.current.currentSelectedGameObject;

		if (long.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) > 0)
		{
			obj.transform.parent.GetChild(3).GetComponent<InputField>().text = (long.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) - 1).ToString();

			ActionNumText.text = (long.Parse(ActionNumText.text) - 1).ToString();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
