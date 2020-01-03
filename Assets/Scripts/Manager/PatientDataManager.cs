﻿/* ============================================================================== 
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
        Entry,      //入门
        Primary,    //初级
        General,    //一般
        Intermediate,  //中级
        Advanced    //高级
    }

    public enum DirectionType
    {
        UponDirection,         //上
        UponLeftDirection,     //左上
        UponRightDirection,    //右上
        DownDirection,         //下
        DownLeftDirection,     //左下
        DownRightDirection,    //右下
        LeftDirection,         //左
        RightDirection,        //右
        AnyDirection           //任意方向
    }


    //Read Only
    //[Header("TrainingPlan")]
    public DifficultyType PlanDifficulty { get; private set; } = DifficultyType.Entry;
    public DifficultyType TrainingDifficulty => PlanDifficulty;
    public long GameCount { get; private set; } = 10;
    public long PlanCount { get; private set; } = 10;
    public DirectionType PlanDirection { get; private set; } = DirectionType.UponDirection;
    public DirectionType TrainingDirection => PlanDirection;
    public long PlanTime { get; private set; } = 20;
    public long TrainingTime => PlanTime;
    public long IsEvaluated { get; private set; } = 1;  //1-评估


    public float LaunchSpeed { get; private set; } = 3.0f;
    public float MaxBallSpeed { get; private set; } = 10f;
    public float MinBallSpeed { get; private set; } = 10f;


    //[Header("GameData")]
    public long SuccessCount { get; private set; } = 0;
    public long FinishCount { get; private set; } = 0;
    public DateTime TrainingStartTime { get; private set; }
    public DateTime TrainingEndTime { get; private set; }

    //[Header("PatientRecord")]
    public long TrainingID { get; private set; } = 0;
    public long MaxSuccessCount { get; private set; } = 0;
    public float[] MaxDirection { get; private set; } = {0f, 0f, 0f, 0f, 0f, 0f, 0f};

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

    // set PatientName,PatientID
    public void SetUserMessage(long PatientID, string PatientName,string PatientSex)
    {
        this.PatientID = PatientID;
        this.PatientName = PatientName;
        this.PatientSex = PatientSex;
    }

    // set PlanDifficulty, GameCount, PlanCount, LaunchSpeed, MaxBallSpeed, MinBallSpeed
    public void SetTrainingPlan(DifficultyType PlanDifficulty, long GameCount, long PlanCount, DirectionType PlanDirection, long PlanTime)
    {
        this.PlanDifficulty = PlanDifficulty;
        this.GameCount = GameCount;
        this.PlanCount = PlanCount;
        this.PlanDirection = PlanDirection;
        this.PlanTime = PlanTime;
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

    // set LaunchSpeed
    public void SetLaunchSpeed(float LaunchSpeed)
    {
        this.LaunchSpeed = LaunchSpeed;
    }

    // set MaxBallSpeed
    public void SetMaxBallSpeed(float MaxBallSpeed)
    {
        this.MaxBallSpeed = MaxBallSpeed;
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


    // set PlanDifficulty
    public void SetPlanDifficulty(DifficultyType PlanDifficulty)
    {
        this.PlanDifficulty = PlanDifficulty;
    }

    //set LimbTips
    public void SetSoccerTrackTipss(bool SoccerTrackTips)
    {
        this.SoccerTrackTips = SoccerTrackTips;
    }

    // set WordTips
    public void SetWordTips(bool WordTips)
    {
        this.WordTips = WordTips;
    }

    // set bgmVolume
    public void SetbgmVolume(float Volume)
    {
        this.bgmVolume = Volume;
    }

    // set bgsVolume
    public void SetbgsVolume(float Volume)
    {
        this.bgsVolume = Volume;
    }

    // set seVolume
    public void SetseVolume(float Volume)
    {
        this.seVolume = Volume;
    }

    // reset game data when restart game
    public void ResetGameData()
    {
        this.SuccessCount = 0;
        this.FinishCount = 0;
        this.SoccerTrackTips = true;
        this.WordTips = true;
        SetTrainingStartTime();
        SetTrainingEndTime();
    }

    public void InitGameData()
    {
        this.SuccessCount = 0;
        this.FinishCount = 0;
        this.SoccerTrackTips = true;
        this.WordTips = true;
        SetTrainingStartTime();
        SetTrainingEndTime();
    }

    // DifficultyType to String
    public static string DifficultyType2Str(DifficultyType PlanDifficulty)
    {
        switch (PlanDifficulty)
        {
            case DifficultyType.Entry:
                return "入门";
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
            case "入门":
                return DifficultyType.Entry;
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

    public static DirectionType Str2DirectionType(string str)
    {
        switch (str)
        {
            case "上":
                return DirectionType.UponDirection;
            case "左上":
                return DirectionType.UponLeftDirection;
            case "右上":
                return DirectionType.UponRightDirection;
            case "下":
                return DirectionType.DownDirection;
            case "左下":
                return DirectionType.DownLeftDirection;
            case "右下":
                return DirectionType.DownRightDirection;
            case "左":
                return DirectionType.LeftDirection;
            case "右":
                return DirectionType.RightDirection;
            case "任意方向":
                return DirectionType.AnyDirection;
            default:
                return DirectionType.UponDirection;
        }
    }

    public static string DirectionType2Str(DirectionType PlanDirection)
    {
        switch (PlanDirection)
        {
            case DirectionType.UponDirection:
                return "上";
            case DirectionType.UponLeftDirection:
                return "左上";
            case DirectionType.UponRightDirection:
                return "右上";
            case DirectionType.DownDirection:
                return "下";
            case DirectionType.DownLeftDirection:
                return "左下";
            case DirectionType.DownRightDirection:
                return "右下";
            case DirectionType.LeftDirection:
                return "左";
            case DirectionType.RightDirection:
                return "右";
            case DirectionType.AnyDirection:
                return "任意方向";
            default:
                return "上";
        }
    }


}
