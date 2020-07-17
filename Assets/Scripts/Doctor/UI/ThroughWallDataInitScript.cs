using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ThroughWallDataInitScript : MonoBehaviour {

    public List<GameObject> Ranks;

    public Text WallEvaluationScore;
    public Text LastWallEvaluationScore;

    public Text WallEvaluationPassScore;
    public Text WallEvaluationAccuracyScore;
    public Text WallEvaluationDuration;
    public Text WallEvaluationActionNum;
    public Text WallEvaluationCompliance;

    public Text WallEvaluationStartTime;

    public Dropdown ScaleSelect;
    public Dropdown NumberSelect;

    public Dictionary<int, int> ScaleType2Int = new Dictionary<int, int>();
    public Dictionary<int, int> ScaleInt2Type = new Dictionary<int, int>();
    public List<string> ListScaleEvaluation = new List<string>();

    public Dictionary<int, int> NumberID2Int = new Dictionary<int, int>();
    public Dictionary<int, int> NumberInt2ID = new Dictionary<int, int>();
    public List<string> ListNumberEvaluation = new List<string>();

    public GameObject NoEvaluateData;

    public GameObject ActionResource;

    public GameObject Report;
    public Toggle EvaluationToggle;
    public Toggle WallEvaluateToggle;

    public GameObject LoadScene;
    void OnEnable()
	{

        //print(DoctorDataManager.instance.doctor.patient.WallEvaluations.Count);

        if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
        {
            NoEvaluateData.SetActive(false);

            int WallEvaluationIndex = DoctorDataManager.instance.doctor.patient.WallEvaluationIndex;

            ScaleType2Int.Clear();
            ScaleInt2Type.Clear();
            ListScaleEvaluation.Clear();
            ScaleSelect.ClearOptions();

            // 写一下量表选择

            int NumID = 0;
            for(int i = 0; i < DoctorDataManager.instance.doctor.patient.WallEvaluations.Count; i++)
            {
                if(ScaleType2Int.ContainsKey(DoctorDataManager.instance.doctor.patient.WallEvaluations[i].type) == false)
                {
                    ScaleType2Int.Add(DoctorDataManager.instance.doctor.patient.WallEvaluations[i].type, NumID);
                    ScaleInt2Type.Add(NumID, DoctorDataManager.instance.doctor.patient.WallEvaluations[i].type);
                    NumID++;

                    ListScaleEvaluation.Add(DATA.TrainingProgramIDToName[DoctorDataManager.instance.doctor.patient.WallEvaluations[i].type]);
                }
            }

            ScaleSelect.AddOptions(ListScaleEvaluation);

            ScaleSelect.value = ScaleType2Int[DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].type];
            
            if(ScaleSelect.value == 0)
            {
                ScaleChange();
            }
            
        }
        else
        {
            NoEvaluateData.SetActive(true);
        }

        LoadScene.SetActive(false);

    }

    public void ScaleChange()
    {

        NumberID2Int.Clear();
        NumberInt2ID.Clear();
        ListNumberEvaluation.Clear();
        NumberSelect.ClearOptions();

        //print("!!!!!");

        int NumIndex = 0;
        for (int i = 0; i < DoctorDataManager.instance.doctor.patient.WallEvaluations.Count; i++)
        {
            if(DoctorDataManager.instance.doctor.patient.WallEvaluations[i].type == ScaleInt2Type[ScaleSelect.value])
            {
                //print(i+"   "+NumIndex);
                NumberID2Int.Add(i, NumIndex);
                NumberInt2ID.Add(NumIndex, i);
                NumIndex++;

                ListNumberEvaluation.Add("第" + NumIndex.ToString() + "次 | " + DoctorDataManager.instance.doctor.patient.WallEvaluations[i].startTime.Substring(3, 2) + "." + DoctorDataManager.instance.doctor.patient.WallEvaluations[i].startTime.Substring(6, 2));
            }
        }

        //print(ListNumberEvaluation.Count);
        NumberSelect.AddOptions(ListNumberEvaluation);

        //print(NumIndex);
        if (NumberSelect.value == NumIndex - 1)
        {
            NumberChange();
        }
        else
        {
            //NumberSelect.RefreshShownValue();
            NumberSelect.value = NumIndex - 1;
        }
    }

    public void NumberChange()
    {
        DoctorDataManager.instance.doctor.patient.SetWallEvaluationIndex(NumberInt2ID[NumberSelect.value]);

        int WallEvaluationIndex = NumberInt2ID[NumberSelect.value];

        float rate = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.passScore / 100f;

        foreach (var item in Ranks)
        {
            item.SetActive(false);
        }
        // 根据准确率显示评分: S A B C D
        if (rate >= 0.9 && rate <= 1)//S
        {
            Ranks[0].SetActive(true);
        }
        else if (rate >= 0.8 && rate < 0.9)//A
        {
            Ranks[1].SetActive(true);
        }
        else if (rate >= 0.7 && rate < 0.8)//B
        {
            Ranks[2].SetActive(true);
        }
        else if (rate >= 0.6 && rate < 0.7)//C
        {
            Ranks[3].SetActive(true);
        }
        else //D
        {
            Ranks[4].SetActive(true);
        }

        WallEvaluationScore.text = "评分: " + DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.score.ToString();
        
        if(DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.lastScore == -1)
        {
            LastWallEvaluationScore.text = "";
        }
        else
        {
            LastWallEvaluationScore.text = "上次评分: " + DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.lastScore.ToString();
        }

        WallEvaluationPassScore.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.passScore.ToString() + "%";
        WallEvaluationAccuracyScore.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.accuracyScore.ToString() + "%";
        WallEvaluationDuration.text = (DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.duration * 60).ToString() + "s";
        WallEvaluationActionNum.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.actionNum.ToString() + "个";
        WallEvaluationCompliance.text = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].overrall.compliance.ToString();

        string s = DoctorDataManager.instance.doctor.patient.WallEvaluations[WallEvaluationIndex].startTime;
        WallEvaluationStartTime.text = "20" + s.Substring(0, 2) + s.Substring(3, 2) + s.Substring(6, 2) + " " + s.Substring(9, 2) + ":" + s.Substring(12, 2) + ":00";
    }

    public void ActionSelectButtonOnClick()
    {
        ActionResource.SetActive(true);
    }

    public void NoEvaluateDataButtonOnClick()
    {
        NoEvaluateData.SetActive(false);
        ActionSelectButtonOnClick();
    }

    public void WallEvaluateButtonOnclick()
    {

        PatientDataManager.instance.SetPatientID(DoctorDataManager.instance.doctor.patient.PatientID);

        LoadScene.SetActive(true);

        DoctorDataManager.instance.FunctionManager = 1;
        DoctorDataManager.instance.EvaluationType = 0;

        SceneManager.LoadScene("08-WallEvaluation");
    }

    public void ReadWallEvaluateReportButtonOnclick()
    {
        Report.SetActive(true);

        EvaluationToggle.isOn = true;
        WallEvaluateToggle.isOn = true;
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


   
}
