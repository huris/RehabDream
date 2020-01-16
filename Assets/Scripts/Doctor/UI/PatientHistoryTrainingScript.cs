using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientHistoryTrainingScript : MonoBehaviour {

    public InputField StartTimeYear;
    public InputField StartTimeMonth;
    public InputField StartTimeDay;
    public InputField EndTimeYear;
    public InputField EndTimeMonth;
    public InputField EndTimeDay;

    public GameObject TrainingPlayList;

    // Use this for initialization
    void Start () 
    {
    
    }

    void OnEnable()
    {
        StartTimeYear = transform.Find("StartTime/Year/InputField").GetComponent<InputField>();
        StartTimeMonth = transform.Find("StartTime/Month/InputField").GetComponent<InputField>();
        StartTimeDay = transform.Find("StartTime/Day/InputField").GetComponent<InputField>();

        EndTimeYear = transform.Find("EndTime/Year/InputField").GetComponent<InputField>();
        EndTimeMonth = transform.Find("EndTime/Month/InputField").GetComponent<InputField>();
        EndTimeDay = transform.Find("EndTime/Day/InputField").GetComponent<InputField>();

        if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
        {
            int FirstTrainingPlay = 0;
            int LastTrainingPlay = DoctorDataManager.instance.doctor.patient.TrainingPlays.Count - 1;

            StartTimeYear.text = DoctorDataManager.instance.doctor.patient.TrainingPlays[FirstTrainingPlay].TrainingStartTime.Substring(0,4);
            StartTimeMonth.text = DoctorDataManager.instance.doctor.patient.TrainingPlays[FirstTrainingPlay].TrainingStartTime.Substring(4, 2);
            StartTimeDay.text = DoctorDataManager.instance.doctor.patient.TrainingPlays[FirstTrainingPlay].TrainingStartTime.Substring(6, 2);

            EndTimeYear.text = DoctorDataManager.instance.doctor.patient.TrainingPlays[LastTrainingPlay].TrainingStartTime.Substring(0, 4);
            EndTimeMonth.text = DoctorDataManager.instance.doctor.patient.TrainingPlays[LastTrainingPlay].TrainingStartTime.Substring(4, 2);
            EndTimeDay.text = DoctorDataManager.instance.doctor.patient.TrainingPlays[LastTrainingPlay].TrainingStartTime.Substring(6, 2);
        }

        TrainingPlayList = transform.Find("TrainingData/TrainingDataBG/TrainingPlayList").gameObject;
    }

    // Update is called once per frame
    void Update () 
    {
		
	}

    public void HistoryTrainingDataQueryButtonOnClick()
    {
        if (StartTimeMonth.text.Length < 2) StartTimeMonth.text = "0" + StartTimeMonth.text;
        if (StartTimeDay.text.Length < 2) StartTimeDay.text = "0" + StartTimeDay.text;
        if (EndTimeMonth.text.Length < 2) EndTimeMonth.text = "0" + EndTimeMonth.text;
        if (EndTimeDay.text.Length < 2) EndTimeDay.text = "0" + EndTimeDay.text;

        string StartTime = StartTimeYear.text + StartTimeMonth.text + StartTimeDay.text + " 00:00:00"; 
        string EndTime = EndTimeYear.text + EndTimeMonth.text + EndTimeDay.text + " 99:99:99";

        //print(StartTimeYear.text+"@@@"+ StartTimeMonth.text+"@@@" + StartTimeDay.text+"@@@");
        //print(StartTime);

        //print(EndTime);

        DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientQueryHistoryRecord(DoctorDataManager.instance.doctor.patient.PatientID, StartTime, EndTime, 0);

        TrainingPlayList.SetActive(false);

        TrainingPlayList.SetActive(true);
    }

    public void DisplayAllTrainingButtonOnClick()
    {
        DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);

        TrainingPlayList.SetActive(false);

        TrainingPlayList.SetActive(true);
    }

}
