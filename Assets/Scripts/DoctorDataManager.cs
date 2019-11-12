using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoctorDataManager : MonoBehaviour {

    // Singleton instance holder
    public static DoctorDataManager instance = null;

    public enum DifficultyType
    {
        Primary,    //初级
        General,    //一般
        Intermediate,  //中级
        Advanced    //高级
    }

    //Read Only
    //[Header("TrainingPlan")]
    public DifficultyType PlanDifficulty { get; private set; } = DifficultyType.Primary;
    public DifficultyType TrainingDifficulty => PlanDifficulty;
    public long GameCount { get; private set; } = 10;
    public long PlanCount { get; private set; } = 10;


    //[Header("GameData")]
    public long SuccessCount { get; private set; } = 0;
    public long FinishCount { get; private set; } = 0;
    public DateTime TrainingStartTime { get; private set; }
    public DateTime TrainingEndTime { get; private set; }


    //[Header("PatientRecord")]
    public long TrainingID { get; private set; } = 0;
    public long MaxSuccessCount { get; private set; } = 0;


    //[Header("PatientMessage")]
    public Patient patient = new Patient();
    public Patient TempPatient = new Patient();

    // PatientID, PatientName, PatientPassword, DoctorID, PatientAge, PatientSex, PatientHeight, PatientWeight
    public List<Patient> Patients = new List<Patient>();

    public Doctor doctor = new Doctor();


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("@DataManager: Singleton created.");

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    
    // set PlanDifficulty, GameCount, PlanCount
    public void SetTrainingPlan(DifficultyType PlanDifficulty, long GameCount, long PlanCount)
    {
        this.PlanDifficulty = PlanDifficulty;
        this.GameCount = GameCount;
        this.PlanCount = PlanCount;
    }

    // set TrainingID, Max_SuccessCount
    public void SetTrainingID(long TrainingID)
    {
        this.TrainingID = TrainingID;
    }

    // set MaxSuccessCount
    public void SetMaxSuccessCount(long MaxSuccessCount)
    {
        this.MaxSuccessCount = MaxSuccessCount;
    }

    // set TrainingStartTime
    public void SetTrainingStartTime()
    {
        this.TrainingStartTime = DateTime.Now;
    }

    // set TrainingEndTime
    public void SetTrainingEndTime()
    {
        this.TrainingEndTime = DateTime.Now;
    }

    // set PlanCount
    public void SetPlanCount(long PlanCount)
    {
        this.PlanCount = PlanCount;
    }

    // set FinishCount
    public void SetFinishCount(long FinishCount)
    {
        this.FinishCount = FinishCount;
    }

    // set FinishCount
    public void SetSuccessCount(long SuccessCount)
    {
        this.SuccessCount = SuccessCount;
    }


    //set PlanDifficulty
    public void SetPlanDifficulty(DifficultyType PlanDifficulty)
    {
        this.PlanDifficulty = PlanDifficulty;
    }

    // DifficultyType to String
    public static string DifficultyType2Str(DifficultyType PlanDifficulty)
    {
        switch (PlanDifficulty)
        {
            case DifficultyType.Primary:
                return "Primary";
            case DifficultyType.General:
                return "General";
            case DifficultyType.Intermediate:
                return "Intermediate";
            case DifficultyType.Advanced:
                return "Advanced";
            default:
                return "Primary";
        }
    }

    // String to DifficultyType
    public static DifficultyType Str2DifficultyType(string str)
    {
        switch (str)
        {
            case "Primary":
                return DifficultyType.Primary;
            case "General":
                return DifficultyType.General;
            case "Intermediate":
                return DifficultyType.Intermediate;
            case "Advanced":
                return DifficultyType.Advanced;
            default:
                return DifficultyType.Primary;
        }
    }


    // DifficultyType to Chinese
    public static string Difficulty2Chinese(DifficultyType PlanDifficulty)
    {
        switch (PlanDifficulty)
        {
            case DifficultyType.Primary:
                return "初级";
            case DifficultyType.General:
                return "一般";
            case DifficultyType.Intermediate:
                return "中级";
            case DifficultyType.Advanced:
                return "高级";
            default:
                return "初级";
        }
    }
}
