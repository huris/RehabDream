using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTrainingPlan
{

    //  初级 一般 中级 高级
    //Read Only
    //[Header("TrainingPlan")]

    public long TrainingDirection { get; private set; } = 0;
    public long TrainingDuration { get; private set; } = 20;  // 默认训练时间为20分钟

    public FishTrainingPlan() { }
    public FishTrainingPlan(long TrainingDirection,long TrainingDuration)
    {
        this.TrainingDirection = TrainingDirection;
        this.TrainingDuration = TrainingDuration;
    }

    // set PlanDifficulty, GameCount, PlanCount
    public void SetTrainingPlan(long TrainingDirection, long TrainingDuration)
    {
        this.TrainingDirection = TrainingDirection;
        this.TrainingDuration = TrainingDuration;
    }
}

