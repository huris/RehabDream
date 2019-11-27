/* ============================================================================== 
* ClassName：DataManager 
* Author：ChenShuwei 
* CreateDate：2019/10/20 10:35:23
* Version: 1.0
* ==============================================================================*/

using System;
using System.Collections.Generic;
using UnityEngine;

// 跨场景数据、UI数据存在此处
public class PatientDataManager : MonoBehaviour{
    // Singleton instance holder
    public static PatientDataManager instance = null;


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

    //[Header("MusicSetting")]
    public float bgmVolume { get; private set; } = 0.5f;
    public float bgsVolume { get; private set; } = 0.5f;
    public float seVolume { get; private set; } = 0.8f;

    //[Header("TipsSetting")]
    public bool SoccerTrackTips { get; private set; } = true;
    public bool WordTips { get; private set; } = true;

    //[Header("UserMessage")]
    public string PatientName { get; private set; } = "PatientName";
    public long PatientID { get; private set; } = 0;
    public string PatientSex { get; private set; } = "男";



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

    //set PatientName,PatientID
    public void SetUserMessage(long PatientID, string PatientName,string PatientSex)
    {
        this.PatientID = PatientID;
        this.PatientName = PatientName;
        this.PatientSex = PatientSex;
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

    //set LimbTips
    public void SetSoccerTrackTipss(bool SoccerTrackTips)
    {
        this.SoccerTrackTips = SoccerTrackTips;
    }

    //set WordTips
    public void SetWordTips(bool WordTips)
    {
        this.WordTips = WordTips;
    }

    //set bgmVolume
    public void SetbgmVolume(float Volume)
    {
        this.bgmVolume = Volume;
    }

    //set bgsVolume
    public void SetbgsVolume(float Volume)
    {
        this.bgsVolume = Volume;
    }

    //set seVolume
    public void SetseVolume(float Volume)
    {
        this.seVolume = Volume;
    }

    //reset game data when restart game
    public void ResetGameData()
    {
        this.SuccessCount = 0;
        this.FinishCount = 0;
        SetTrainingStartTime();
        SetTrainingEndTime();
    }

    // DifficultyType to String
    public static string DifficultyType2Str(DifficultyType PlanDifficulty)
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

    // String to DifficultyType
    public static DifficultyType Str2DifficultyType(string str)
    {
        switch (str)
        {
            case "初级":
                return DifficultyType.Primary;
            case "一般":
                return DifficultyType.General;
            case "中级":
                return DifficultyType.Intermediate;
            case "高级":
                return DifficultyType.Advanced;
            default:
                return DifficultyType.Primary;
        }
    }


    
}
