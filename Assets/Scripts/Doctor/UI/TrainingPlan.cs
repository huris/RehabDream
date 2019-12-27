using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPlanResult{

    //  初级 一般 中级 高级
    //Read Only
    //[Header("TrainingPlan")]

    public bool PlanIsMaking { get; private set; } = false;
    public string PlanDifficulty { get; private set; } = "未制定计划";
    public long GameCount { get; private set; } = 0;
    public long PlanCount { get; private set; } = 0;

    // set PlanDifficulty, GameCount, PlanCount
    public void SetTrainingPlan(string PlanDifficulty, long GameCount, long PlanCount)
    {
        this.PlanDifficulty = PlanDifficulty;
        this.GameCount = GameCount;
        this.PlanCount = PlanCount;
    }

    public void SetPlanIsMaking(bool PlanIsMaking)
    {
        this.PlanIsMaking = PlanIsMaking; 
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
