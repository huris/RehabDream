 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingPlay
{
    public long TrainingID { get; private set; } = 0;
    public string TrainingStartTime { get; private set; } = "00000000 00:00:00";
    public string TrainingEndTime { get; private set; } = "00000000 00:00:00";
    public string TrainingDifficulty { get; private set; } = "初级";
    public long SuccessCount { get; private set; } = 0;
    public long GameCount { get; private set; } = 0;
    public string TrainingDirection { get; private set; } = "全方位";
    public long TrainingTime { get; private set; } = 20;
    public float EvaluationScore { get; private set; } = 0.0f;

    public List<Angle> angles = new List<Angle>();      // 患者角度

    public List<GravityCenter> gravityCenters = new List<GravityCenter>();   // 患者重心变化

    public Direction direction = new Direction();  // 患者8个方向的最大值

    public void SetEvaluationScore()
    {
        this.EvaluationScore = this.direction.GetRadarArea();
    }

    public void SetCompleteTrainingPlay(long TrainingID, string TrainingStartTime, string TrainingEndTime, string TrainingDifficulty, 
        long SuccessCount, long GameCount, string TrainingDirection, long TrainingTime)
    {
        this.TrainingID = TrainingID;
        this.TrainingStartTime = TrainingStartTime;
        this.TrainingEndTime = TrainingEndTime;
        this.TrainingDifficulty = TrainingDifficulty;
        this.SuccessCount = SuccessCount;
        this.GameCount = GameCount;
        this.TrainingDirection = TrainingDirection;
        this.TrainingTime = TrainingTime;
    }

    // set TrainingID, Max_SuccessCount
    public void SetTrainingID(long TrainingID)
    {
        this.TrainingID = TrainingID;
    }

    // set FinishCount
    public void SetGameCount(long GameCount)
    {
        this.GameCount = GameCount;
    }

    // set FinishCount
    public void SetSuccessCount(long SuccessCount)
    {
        this.SuccessCount = SuccessCount;
    }
}
