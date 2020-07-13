using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScaleActionInitScript : MonoBehaviour
{

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
    public Scrollbar ScaleActionScrolbar;

    public Text ScaleActionText;

    public Dictionary<int, int> ScaleActionID2Num = new Dictionary<int, int>();

    public int ScaleActionNum = 0;

    public InputField FindActionInputField;

    public GameObject ModifyButton;

    public GameObject ActionModify;

    public GameObject ActionResource;

    public int ModifyActionID;
    //public Transform ActionModifyTransform;
    public Text ActionModifyTitle;
    public Image FrontViewImage;
    public Image SideViewImage;
    public List<Toggle> JointToggles;
    public List<Toggle> PersonToggles;
    public InputField ActionModifyName;
    public Text ActionModifyID;
    public InputField ActionModifyCreateTime;
    public Text ActionModifyJointNum;
    public InputField ActionModifyDescription;

    public Dictionary<int, int> toggleJointId2Index;
    public Dictionary<int, int> toggleIndex2JointId;

    public Dictionary<int, int> ActionID2Index;

    public int UserID;

    public bool IsUserSave;

    public GameObject NoActionNum;

    public bool ScaleSelectValueChangedFirst;

    public GameObject NoEvaluateData;

    public GameObject LoadScene;

    void OnEnable()
    {
        ScaleSelectValueChangedFirst = true;

        toggleJointId2Index = new Dictionary<int, int>() { {2, 0}, {4, 1}, {8, 2}, {5, 3}, {9, 4}, {12, 5}, {16, 6}, {13, 7}, {17, 8}, {14, 9}, {18, 10} };
        toggleIndex2JointId = new Dictionary<int, int>() { {0, 2}, {1, 4}, {2, 8}, {3, 5}, {4, 9}, {5, 12}, {6, 16}, {7, 13}, {8, 17}, {9, 14}, {10, 18} };

        ActionID2Index = new Dictionary<int, int>();

        ScaleID = new List<int>(DATA.TrainingProgramIDToName.Keys);

        ScaleType2Int.Clear();
        ScaleInt2Type.Clear();
        ListScaleEvaluation.Clear();
        ScaleSelect.ClearOptions();

        for (int i = 0; i < ScaleID.Count; i++)
        {
            ScaleType2Int.Add(ScaleID[i], i);
            ScaleInt2Type.Add(i, ScaleID[i]);
            ListScaleEvaluation.Add(DATA.TrainingProgramIDToName[ScaleID[i]]);
        }
        ScaleType2Int.Add(ScaleID.Count, ScaleID.Count);
        ScaleInt2Type.Add(ScaleID.Count, ScaleID.Count);
        ListScaleEvaluation.Add("自定义");

        ScaleSelect.AddOptions(ListScaleEvaluation);

        UserID = -1;
        for (int i = 0; i < DoctorDataManager.instance.users.Count; i++)
        {
            if (DoctorDataManager.instance.doctor.patient.PatientID == DoctorDataManager.instance.users[i].ID)
            {
                UserID = i;
                break;
            }
        }

        if(UserID == -1)
        {
            ScaleSelect.value = 0;
            ActionSpeedDropdown.value = 2;
            
            for(int i = 0; i < DATA.TrainingProgramIDToActionIDs[ScaleInt2Type[ScaleSelect.value]].Count; i++)
            {
                ScaleActionID2Num[DATA.TrainingProgramIDToActionIDs[ScaleInt2Type[ScaleSelect.value]][i]] = 6;
            }
        }
        else
        {
            ScaleSelect.value = ScaleType2Int[DoctorDataManager.instance.users[UserID].trainingTypeId];
            ActionSpeedDropdown.value = DoctorDataManager.instance.users[UserID].level.wallSpeed - 7;

            ScaleActionID2Num = DoctorDataManager.instance.users[UserID].level.actionRates;
        }

        // 1.生成所有动作

        //// 所有动作的item赋值
        //      if (this.transform.GetChild(4).GetChild(0).GetChild(0).childCount > DoctorDataManager.instance.Actions.Count)   // 如果数目大于训练数据，说明足够存储了，需要把之后的几个给设置未激活
        //      {
        //          for (int i = this.transform.GetChild(4).GetChild(0).GetChild(0).childCount - 1; i >= DoctorDataManager.instance.Actions.Count; i--)
        //          {
        //              this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
        //              //Destroy(this.transform.GetChild(i).gameObject);
        //          }
        //      }
        //      else   // 否则说明数目不够，需要再生成几个预制体
        //      {


        int ScaleActionChildNum = 0;
        int AllActionChildNum = 0;

        for (int i = this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; i < DoctorDataManager.instance.Actions.Count; i++)
        {
            ActionPrefab = Resources.Load("Prefabs/ActionImageItem") as GameObject;

            //print(DoctorDataManager.instance.Actions[i].id + "    " + i);
            ActionID2Index.Add(DoctorDataManager.instance.Actions[i].id, i);

            if (ScaleActionID2Num.ContainsKey(DoctorDataManager.instance.Actions[i].id)) 
            {
                Instantiate(ActionPrefab).transform.SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));

                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(0).GetComponent<Toggle>().group = this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<ToggleGroup>();
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                    GameObject obj = EventSystem.current.currentSelectedGameObject;
                    ModifyActionID = int.Parse(obj.transform.parent.name);

                    if (obj.transform.GetComponent<Toggle>().isOn)
                    {
                        ModifyButton.SetActive(true);
                    }
                    else
                    {
                        ModifyButtonOnClick();
                        ModifyButton.SetActive(false);
                    }
                });

                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(3).GetComponent<InputField>().text = ScaleActionID2Num[DoctorDataManager.instance.Actions[i].id].ToString();
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(3).GetComponent<InputField>().onEndEdit.AddListener(delegate
                {
                    GameObject obj = EventSystem.current.currentSelectedGameObject;

                    if (int.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) == 0)
                    {
                        if (obj.transform.parent.parent.parent.parent.name == "ScaleAction")
                        {
                            int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

                            for (int z = TempID; z < obj.transform.parent.parent.childCount; z++)
                            {
                                obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = z.ToString();
                            }

                            obj.transform.parent.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                            obj.transform.parent.GetChild(7).GetComponent<InputField>().text = obj.transform.parent.parent.childCount.ToString();
                        }
                        else
                        {
                            obj.transform.parent.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                        }
                    }
                    else
                    {
                        if (obj.transform.parent.parent.parent.parent.name == "AllAction")
                        {
                            int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

                            for (int z = TempID; z < obj.transform.parent.parent.childCount; z++)
                            {
                                obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = z.ToString();
                            }

                            obj.transform.parent.SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                            obj.transform.parent.GetChild(7).GetComponent<InputField>().text = obj.transform.parent.parent.childCount.ToString();

                            ScaleActionScrolbar.value = 0;
                        }
                        else
                        {
                            obj.transform.parent.SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                        }
                    }

                    AdjustActionSetLayout();

                    ScaleActionText.text = "量表动作（" + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount + "）";
                    AllActionText.text = "所有动作（" + this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + "）";

                    int TempScaleActionNum = 0;

                    for (int z = 0; z < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; z++)
                    {
                        TempScaleActionNum += int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(z).GetChild(3).GetComponent<InputField>().text);
                    }

                    ActionNumText.text = TempScaleActionNum.ToString();

                });

                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(7).GetComponent<InputField>().text = (ScaleActionChildNum + 1).ToString();
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(7).GetComponent<InputField>().onEndEdit.AddListener(delegate
                {
                    GameObject obj = EventSystem.current.currentSelectedGameObject;

                    int TempID = int.Parse(obj.GetComponent<InputField>().text);

                    obj.transform.parent.SetSiblingIndex(TempID - 1);

                    for(int z = 0; z < obj.transform.parent.parent.childCount; z++)
                    {
                        obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = (z + 1).ToString();
                    }

                });


                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).name = DoctorDataManager.instance.Actions[i].id.ToString();   // 重新命名为0,1,2,3,4...

                //this.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(QuerySingleTrainingButtonOnClick);  // 查询身体状况

                Image image = this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(0).gameObject.GetComponent<Image>();

                // 速度很慢
                StartCoroutine(new Utils().Load(image, Environment.CurrentDirectory + DoctorDataManager.instance.Actions[i].filename));

                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(1).GetComponent<Text>().text = DoctorDataManager.instance.Actions[i].name;
                if (DoctorDataManager.instance.Actions[i].name.Length == 6)
                {
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(1).GetComponent<Text>().fontSize = 13;
                }
                else if (DoctorDataManager.instance.Actions[i].name.Length > 6)
                {
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(1).GetComponent<Text>().fontSize = 12;
                }

                string CreatTime = DoctorDataManager.instance.Actions[i].createTime;
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(2).GetComponent<Text>().text = CreatTime.Substring(0, 4) + CreatTime.Substring(5, 2) + CreatTime.Substring(8, 2);


                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(4).GetComponent<Button>().onClick.AddListener(ActionUponArrowOnClick);
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(5).GetComponent<Button>().onClick.AddListener(ActionDownArrowOnClick);

                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(8).GetComponent<Button>().onClick.AddListener(ActionLeftArrowOnClick);
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(ScaleActionChildNum).GetChild(9).GetComponent<Button>().onClick.AddListener(ActionRightArrowOnClick);

                ScaleActionChildNum++;

            }
            else
            {
                Instantiate(ActionPrefab).transform.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));

                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(0).GetComponent<Toggle>().group = this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<ToggleGroup>();
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                    GameObject obj = EventSystem.current.currentSelectedGameObject;
                    ModifyActionID = int.Parse(obj.transform.parent.name);

                    if (obj.transform.GetComponent<Toggle>().isOn)
                    {
                        ModifyButton.SetActive(true);
                    }
                    else
                    {
                        ModifyButtonOnClick();
                        ModifyButton.SetActive(false);
                    }
                });

                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(3).GetComponent<InputField>().text = "0";
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(3).GetComponent<InputField>().onEndEdit.AddListener(delegate{

                    GameObject obj = EventSystem.current.currentSelectedGameObject;

                    if (int.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) == 0)
                    {
                        if (obj.transform.parent.parent.parent.parent.name == "ScaleAction")
                        {
                            int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

                            for (int z = TempID; z < obj.transform.parent.parent.childCount; z++)
                            {
                                obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = z.ToString();
                            }

                            obj.transform.parent.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                            obj.transform.parent.GetChild(7).GetComponent<InputField>().text = obj.transform.parent.parent.childCount.ToString();
                        }
                        else
                        {
                            obj.transform.parent.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                        }
                    }
                    else
                    {
                        if (obj.transform.parent.parent.parent.parent.name == "AllAction")
                        {
                            int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

                            for (int z = TempID; z < obj.transform.parent.parent.childCount; z++)
                            {
                                obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = z.ToString();
                            }

                            obj.transform.parent.SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                            obj.transform.parent.GetChild(7).GetComponent<InputField>().text = obj.transform.parent.parent.childCount.ToString();

                            ScaleActionScrolbar.value = 0;
                        }
                        else
                        {
                            obj.transform.parent.SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                        }
                    }

                    AdjustActionSetLayout();

                    ScaleActionText.text = "量表动作（" + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount + "）";
                    AllActionText.text = "所有动作（" + this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + "）";

                    int TempScaleActionNum = 0;

                    for (int z = 0; z < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; z++)
                    {
                        TempScaleActionNum += int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(z).GetChild(3).GetComponent<InputField>().text);
                    }

                    ActionNumText.text = TempScaleActionNum.ToString();
                });


                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(7).GetComponent<InputField>().text = (AllActionChildNum + 1).ToString();
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(7).GetComponent<InputField>().onEndEdit.AddListener(delegate
                {
                    GameObject obj = EventSystem.current.currentSelectedGameObject;
                    
                    int TempID = int.Parse(obj.GetComponent<InputField>().text);

                    obj.transform.parent.SetSiblingIndex(TempID - 1);

                    for (int z = 0; z < obj.transform.parent.parent.childCount; z++)
                    {
                        obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = (z + 1).ToString();
                    }
                });

                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).name = DoctorDataManager.instance.Actions[i].id.ToString();   // 重新命名为0,1,2,3,4...

                //this.transform.GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(QuerySingleTrainingButtonOnClick);  // 查询身体状况

                Image image = this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(0).gameObject.GetComponent<Image>();

                // 速度很慢
                StartCoroutine(new Utils().Load(image, Environment.CurrentDirectory + DoctorDataManager.instance.Actions[i].filename));

                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(1).GetComponent<Text>().text = DoctorDataManager.instance.Actions[i].name;
                if (DoctorDataManager.instance.Actions[i].name.Length == 6)
                {
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(1).GetComponent<Text>().fontSize = 13;
                }
                else if (DoctorDataManager.instance.Actions[i].name.Length > 6)
                {
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(1).GetComponent<Text>().fontSize = 12;
                }

                string CreatTime = DoctorDataManager.instance.Actions[i].createTime;
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(2).GetComponent<Text>().text = CreatTime.Substring(0, 4) + CreatTime.Substring(5, 2) + CreatTime.Substring(8, 2);


                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(4).GetComponent<Button>().onClick.AddListener(ActionUponArrowOnClick);
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(5).GetComponent<Button>().onClick.AddListener(ActionDownArrowOnClick);

                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(8).GetComponent<Button>().onClick.AddListener(ActionLeftArrowOnClick);
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(AllActionChildNum).GetChild(9).GetComponent<Button>().onClick.AddListener(ActionRightArrowOnClick);

                AllActionChildNum++;
            }

            //this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);  // 要设置激活状态

        }
        //}

        //      //List<int> ActionID = new List<int>(DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overview.actionDatas.Keys);
        //      // 在将列表中的内容放入
        //      for (int i = 0; i < DoctorDataManager.instance.Actions.Count; i++)
        //      {

        //}


        AdjustActionSetLayout();

        // 计算动作个数
        int ScaleActionNum = 0;

        for(int i = 0; i < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; i++)
        {
            ScaleActionNum += int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text);
        }

        ScaleActionText.text = "量表动作（" + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount + "）";
        AllActionText.text = "所有动作（" + this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + "）";

        ActionNumText.text = ScaleActionNum.ToString();

        FindActionInputField.GetComponent<InputField>().onValueChanged.AddListener(delegate
        {
            int TempAllActionNum = 0;
            for(int i = 0; i < this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; i++)
            {
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Toggle>().isOn = false;

                if (FindActionInputField.GetComponent<InputField>().text == "")
                {
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
                    TempAllActionNum++;
                }
                else if (this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text.IndexOf(FindActionInputField.GetComponent<InputField>().text) >= 0)   // 存在所需的文字
                {
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
                    TempAllActionNum++;
                }
                else
                {
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
            }

            if (TempAllActionNum <= 14)
            {
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f);
            }
            else
            {
                //print(this.transform.childCount);
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f + ((TempAllActionNum - 15) / 7 + 1) * 185f);
            }
        });

        for (int i = 0; i < JointToggles.Count; i++)
        {
            JointToggles[i].onValueChanged.AddListener(delegate
            {
                int TempJointNum = 0;
                for(int j = 0; j < JointToggles.Count; j++)
                {
                    if (JointToggles[j].isOn)
                    {
                        TempJointNum++;
                    }
                }

                ActionModifyJointNum.text = TempJointNum.ToString();
            });
        }

        IsUserSave = false;

        LoadScene.SetActive(false);
    }


    public void AdjustActionSetLayout()
    {
        // 设置布局
        if (this.transform.GetChild(3).GetChild(0).GetChild(0).childCount <= 7)
        {
            this.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1115.7f, 180f);
        }
        else
        {
            //print(this.transform.childCount);
            this.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1115.7f, 180f + (this.transform.GetChild(3).GetChild(0).GetChild(0).childCount - 1) / 7 * 185f);
        }

        if (this.transform.GetChild(4).GetChild(0).GetChild(0).childCount <= 14)
        {
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f);
        }
        else
        {
            //print(this.transform.childCount);
            this.transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1129.8f, 364.8f + ((this.transform.GetChild(4).GetChild(0).GetChild(0).childCount - 15) / 7 + 1) * 185f);
        }
    }


    public void ActionUponArrowOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;

        if (int.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) < 99)
        {
            if (int.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) == 0)
            {

                if (obj.transform.parent.parent.parent.parent.name == "AllAction")
                {
                    int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

                    for (int z = TempID; z < obj.transform.parent.parent.childCount; z++)
                    {
                        obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = z.ToString();
                    }

                    obj.transform.parent.SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                    obj.transform.parent.GetChild(7).GetComponent<InputField>().text = obj.transform.parent.parent.childCount.ToString();

                    ScaleActionScrolbar.value = 0;
                }
                else
                {
                    obj.transform.parent.SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                }

                ScaleActionText.text = "量表动作（" + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount + "）";
                AllActionText.text = "所有动作（" + this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + "）";

                AdjustActionSetLayout();
                ScaleActionScrolbar.value = 0;
            }

            obj.transform.parent.GetChild(3).GetComponent<InputField>().text = (long.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) + 1).ToString();

            ActionNumText.text = (long.Parse(ActionNumText.text) + 1).ToString();
        }
    }

    public void ActionDownArrowOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;

        if (int.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) > 0)
        {
            if (int.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) == 1)
            {
                if (obj.transform.parent.parent.parent.parent.name == "ScaleAction")
                {
                    int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

                    for (int z = TempID; z < obj.transform.parent.parent.childCount; z++)
                    {
                        obj.transform.parent.parent.GetChild(z).GetChild(7).GetComponent<InputField>().text = z.ToString();
                    }

                    obj.transform.parent.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                    obj.transform.parent.GetChild(7).GetComponent<InputField>().text = obj.transform.parent.parent.childCount.ToString();
                }
                else
                {
                    obj.transform.parent.SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                }

                ScaleActionText.text = "量表动作（" + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount + "）";
                AllActionText.text = "所有动作（" + this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + "）";

                AdjustActionSetLayout();

            }

            obj.transform.parent.GetChild(3).GetComponent<InputField>().text = (long.Parse(obj.transform.parent.GetChild(3).GetComponent<InputField>().text) - 1).ToString();

            ActionNumText.text = (long.Parse(ActionNumText.text) - 1).ToString();
        }
    }

    public void ActionLeftArrowOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;

        int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

        if(TempID > 1)
        {
            obj.transform.parent.SetSiblingIndex(TempID - 2);
            obj.transform.parent.GetChild(7).GetComponent<InputField>().text = (TempID - 1).ToString();
            obj.transform.parent.parent.GetChild(TempID - 1).GetChild(7).GetComponent<InputField>().text = TempID.ToString();
        }
    }

    public void ActionRightArrowOnClick()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;

        int TempID = int.Parse(obj.transform.parent.GetChild(7).GetComponent<InputField>().text);

        if (TempID < obj.transform.parent.parent.childCount)
        {
            obj.transform.parent.SetSiblingIndex(TempID);
            obj.transform.parent.GetChild(7).GetComponent<InputField>().text = (TempID + 1).ToString();
            obj.transform.parent.parent.GetChild(TempID - 1).GetChild(7).GetComponent<InputField>().text = TempID.ToString();
        }
    }

    public void ModifyButtonOnClick()
    {
        //ModifyActionID = -9999; // 首先令ModifyActionID为一个不可能的值，然后进行查找

        ////ActionModifyTransform = null;
        //for (int i = 0; i < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; i++)
        //{
        //    if (this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Toggle>().isOn)
        //    {
        //        ModifyActionID = int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name);
        //        break;
        //    }
        //}

        //if(ModifyActionID == -9999)
        //{
        //    for (int i = 0; i < this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; i++)
        //    {
        //        if (this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Toggle>().isOn)
        //        {
        //            ModifyActionID = int.Parse(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name);
        //            break;
        //        }
        //    }
        //}

        ActionModify.SetActive(true);

        //for(int i = 0; i < DoctorDataManager.instance.Actions.Count; i++)
        //{
        //    if(DoctorDataManager.instance.Actions[i].id == ModifyActionID)
        //    {
        //        ModifyActionID = i;
        //        break;
        //    }
        //}

        //print(ModifyActionID);
        //print(ActionID2Index[ModifyActionID]);


        ActionModifyTitle.text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].name;
        StartCoroutine(new Utils().Load(FrontViewImage, Environment.CurrentDirectory + DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].filename));
        StartCoroutine(new Utils().Load(SideViewImage, Environment.CurrentDirectory + DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].sideFilename));

        ActionModifyName.text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].name;
        ActionModifyID.text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].id.ToString();
        ActionModifyCreateTime.text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].createTime;
        //ActionModifyJointNum.text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].checkJoints.Count.ToString();
        ActionModifyJointNum.text = "0";
        ActionModifyDescription.text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].describe;
        //print("!!!!!!");
        //print(JointToggles.Count + "   " + PersonToggles.Count);

        for (int i = 0; i < JointToggles.Count; i++)
        {
            //print(i + "!!!!");
            JointToggles[i].isOn = false;
            //print(i + "!!!!");
            PersonToggles[i].isOn = false;
        }
        
        //print("!!!!!!@@@1");

        //print(DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].checkJoints.Count);
        for (int i = 0; i < DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].checkJoints.Count; i++)
        {
            //print(DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].checkJoints[i]);

            //print(toggleJointId2Index[DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].checkJoints[i]]);

            JointToggles[toggleJointId2Index[DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].checkJoints[i]]].isOn = true;
        }

    }


    public void ActionModifySaveButtonOnClick()
    {
        List<int> TempActionJointsId = new List<int>();
        for(int i = 0; i < JointToggles.Count; i++)
        {
            if (JointToggles[i].isOn)
            {
                TempActionJointsId.Add(toggleIndex2JointId[i]);
            }
        }

        DoctorDatabaseManager.instance.ModifyWallEvaluationActionInfo(ModifyActionID, ActionModifyName.text, ActionModifyCreateTime.text, ActionModifyDescription.text, TempActionJointsId);

        DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].name = ActionModifyName.text;
        DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].createTime = ActionModifyCreateTime.text;
        DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].describe = ActionModifyDescription.text;
        DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].checkJoints = TempActionJointsId;

        for(int i = 0; i < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; i++)
        {
            if(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name == ModifyActionID.ToString())
            {
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].name;

                string CreatTime = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].createTime;
                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = CreatTime.Substring(0, 4) + CreatTime.Substring(5, 2) + CreatTime.Substring(8, 2);
                
                break;
            }
        }

        for (int i = 0; i < this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; i++)
        {
            if (this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name == ModifyActionID.ToString())
            {
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].name;

                string CreatTime = DoctorDataManager.instance.Actions[ActionID2Index[ModifyActionID]].createTime;
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = CreatTime.Substring(0, 4) + CreatTime.Substring(5, 2) + CreatTime.Substring(8, 2);
                
                break;
            }
        }

        ActionModifyExitButtonOnClick();
    }

    public void ActionModifyExitButtonOnClick()
    {
        ActionModify.SetActive(false);
    }



    //public Level(int ws, string st, int td, bool wr, int an, Dictionary<int, int> ar)
    //{
    //    wallSpeed = ws;
    //    StartTime = st;
    //    trainingDays = td;
    //    isWallRandom = wr;
    //    actionNum = an;
    //    actionRates = new Dictionary<int, int>();
    //    foreach (var item in ar)
    //    {
    //        actionRates.Add(item.Key, item.Value);
    //    }
    //}


    public void EvaluateButtonOnClick()
    {
        //// TODO: 判断动作数是否有

        //print(ActionNumText);

        if(ActionNumText.text == "0")
        {
            NoActionNum.SetActive(true);
        }
        else
        {
            SaveButtonOnClick();

            PatientDataManager.instance.SetPatientID(DoctorDataManager.instance.doctor.patient.PatientID);

            LoadScene.SetActive(true);

            SceneManager.LoadScene("08-WallEvaluation");
        }
    }

    public void SaveButtonOnClick()
    {

        Dictionary<int, int> TempSingleActionNum = new Dictionary<int, int>();

        for(int i = 0; i < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; i++)
        {
            TempSingleActionNum.Add(int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name), int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text));
        }

        Level TempLevel = new Level(ActionSpeedDropdown.value + 7, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), 100000, false, int.Parse(ActionNumText.text), TempSingleActionNum);


        //print(UserID);

        if(UserID == -1)
        {
            DoctorDatabaseManager.instance.InsertWallEvaluationScaleInfo(DoctorDataManager.instance.doctor.patient.PatientID,
                                                                         DoctorDataManager.instance.doctor.patient.PatientName,
                                                                         DoctorDataManager.instance.doctor.patient.PatientSex,
                                                                         DoctorDataManager.instance.doctor.patient.PatientAge,
                                                                         DoctorDataManager.instance.doctor.patient.PatientWeight,
                                                                          ScaleInt2Type[ScaleSelect.value],
                                                                          "123456",
                                                                          JsonHelper.SerializeObject(TempLevel));
        }
        else
        {
            DoctorDatabaseManager.instance.ModifyWallEvaluationScaleInfo(DoctorDataManager.instance.doctor.patient.PatientID, ScaleInt2Type[ScaleSelect.value], JsonHelper.SerializeObject(TempLevel));
        }

        IsUserSave = true;

        ExitButtonOnClick();
    }

    // TODO: 量表改变，动作库改变
    public void ScaleValueChanged()
    {
        if (ScaleSelectValueChangedFirst &&　(UserID != -1))
        {
            ScaleSelectValueChangedFirst = false;
        }
        else
        {
            ScaleActionID2Num = new Dictionary<int, int>();

            // 说明是自定义的量表，上面没有动作
            if (ScaleSelect.value != ScaleSelect.options.Count - 1)
            {
                for (int i = 0; i < DATA.TrainingProgramIDToActionIDs[ScaleInt2Type[ScaleSelect.value]].Count; i++)
                {
                    ScaleActionID2Num[DATA.TrainingProgramIDToActionIDs[ScaleInt2Type[ScaleSelect.value]][i]] = 6;
                }
            }

            //print(ScaleActionID2Num.Keys.Count);

            for (int i = 0; i < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; i++)
            {
                //print(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name);
                if (ScaleActionID2Num.ContainsKey(int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name)) && (ScaleActionID2Num[int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name)] > 0))
                {
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = (ScaleActionID2Num[int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name)]).ToString();
                }
                else
                {
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = "0";
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                    i--;
                }
            }

            for (int i = 0; i < this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; i++)
            {
                //print(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name);

                if (ScaleActionID2Num.ContainsKey(int.Parse(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name)) && (ScaleActionID2Num[int.Parse(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name)] > 0))
                {
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = (ScaleActionID2Num[int.Parse(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name)]).ToString();
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                    i--;
                }
            }




            int TempScaleActionNum = 0;

            for (int z = 0; z < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; z++)
            {
                TempScaleActionNum += int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(z).GetChild(3).GetComponent<InputField>().text);

                this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(z).GetChild(7).GetComponent<InputField>().text = (z + 1).ToString();
            }

            for (int z = 0; z < this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; z++)
            {
                this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(z).GetChild(7).GetComponent<InputField>().text = (z + 1).ToString();
            }

            ActionNumText.text = TempScaleActionNum.ToString();

            ScaleActionText.text = "量表动作（" + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount + "）";
            AllActionText.text = "所有动作（" + this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + "）";

            AdjustActionSetLayout();

        }
    }

    public void ExitButtonOnClick()
    {
        if(IsUserSave == false)
        {
            if (UserID == -1)
            {
                ScaleSelect.value = 0;
                ActionSpeedDropdown.value = 2;

                for (int i = 0; i < DATA.TrainingProgramIDToActionIDs[ScaleInt2Type[ScaleSelect.value]].Count; i++)
                {
                    ScaleActionID2Num[DATA.TrainingProgramIDToActionIDs[ScaleInt2Type[ScaleSelect.value]][i]] = 6;
                }
            }
            else
            {
                ScaleSelect.value = ScaleType2Int[DoctorDataManager.instance.users[UserID].trainingTypeId];
                ActionSpeedDropdown.value = DoctorDataManager.instance.users[UserID].level.wallSpeed - 7;

                ScaleActionID2Num = DoctorDataManager.instance.users[UserID].level.actionRates;
            }

            for (int i = 0; i < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; i++)
            {
                if (ScaleActionID2Num.ContainsKey(int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name)) && (ScaleActionID2Num[int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name)] > 0))
                {
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = (ScaleActionID2Num[int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).name)]).ToString();
                }
                else
                {
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = "0";
                    this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i).SetParent(this.transform.GetChild(4).GetChild(0).GetChild(0));
                    i--;
                }
            }

            for (int i = 0; i < this.transform.GetChild(4).GetChild(0).GetChild(0).childCount; i++)
            {
                if (ScaleActionID2Num.ContainsKey(int.Parse(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name)) && (ScaleActionID2Num[int.Parse(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name)] > 0))
                {
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<InputField>().text = (ScaleActionID2Num[int.Parse(this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).name)]).ToString();
                    this.transform.GetChild(4).GetChild(0).GetChild(0).GetChild(i).SetParent(this.transform.GetChild(3).GetChild(0).GetChild(0));
                    i--;
                }
            }


            int TempScaleActionNum = 0;

            for (int z = 0; z < this.transform.GetChild(3).GetChild(0).GetChild(0).childCount; z++)
            {
                TempScaleActionNum += int.Parse(this.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(z).GetChild(3).GetComponent<InputField>().text);
            }

            ActionNumText.text = TempScaleActionNum.ToString();

            ScaleActionText.text = "量表动作（" + this.transform.GetChild(3).GetChild(0).GetChild(0).childCount + "）";
            AllActionText.text = "所有动作（" + this.transform.GetChild(4).GetChild(0).GetChild(0).childCount + "）";

            AdjustActionSetLayout();
        }

        ActionResource.SetActive(false);

        if(DoctorDataManager.instance.doctor.patient.WallEvaluations.Count == 0)
        {
            NoEvaluateData.SetActive(true);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
