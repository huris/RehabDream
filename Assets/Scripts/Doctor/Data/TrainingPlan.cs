using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPlan{

    //  初级 一般 中级 高级
    //Read Only
    //[Header("TrainingPlan")]

    public string PlanDifficulty { get; private set; } = "Difficulty";
    public long GameCount { get; private set; } = 0;
    public long PlanCount { get; private set; } = 0;
    public string PlanDirection { get; private set; } = "All";
    public long PlanTime { get; private set; } = 20;  // 默认训练时间为20分钟

    public TrainingPlan() { }
    public TrainingPlan(string PlanDifficulty, long GameCount, long PlanCount, string PlanDirection, long PlanTime)
    {
        this.PlanDifficulty = PlanDifficulty;
        this.GameCount = GameCount;
        this.PlanCount = PlanCount;
        this.PlanDirection = PlanDirection;
        this.PlanTime = PlanTime;
    }

    // set PlanDifficulty, GameCount, PlanCount
    public void SetTrainingPlan(string PlanDifficulty, long GameCount, long PlanCount)
    {
        this.PlanDifficulty = PlanDifficulty;
        this.GameCount = GameCount;
        this.PlanCount = PlanCount;
    }

    public void SetTrainingPlan(string PlanDifficulty, string PlanDirection, long PlanTime)
    {
        this.PlanDifficulty = PlanDifficulty;
        this.PlanDirection = PlanDirection;
        this.PlanTime = PlanTime;
    }

    public void SetCompleteTrainingPlan(string PlanDifficulty, long GameCount, long PlanCount, string PlanDirection, long PlanTime)
    {
        this.PlanDifficulty = PlanDifficulty;
        this.GameCount = GameCount;
        this.PlanCount = PlanCount;
        this.PlanDirection = PlanDirection;
        this.PlanTime = PlanTime;
    }


    // set PlanCount
    public void SetPlanCount(long PlanCount)
    {
        this.PlanCount = PlanCount;
    }

    //set PlanDifficulty
    public void SetPlanDifficulty(string PlanDifficulty)
    {
        this.PlanDifficulty = PlanDifficulty;
    }
}
