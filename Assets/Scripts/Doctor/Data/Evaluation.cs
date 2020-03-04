 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Evaluation
{
    public long EvaluationID { get; private set; } = 0;
    public string EvaluationStartTime { get; private set; } = "00000000 00:00:00";
    public string EvaluationEndTime { get; private set; } = "00000000 00:00:00";

    public List<Point> Points = new List<Point>();  // 数据点的集合 

    public SoccerDistance soccerDistance = null;  // 四周8个方向足球位移的最大值和中间足球的最大最小面积

    public float EvaluationScore { get; private set; } = 0.0f;

    public void SetEvaluationScore()    // 求评估分数
    {
        this.EvaluationScore = 0.0f;
    }

    public void SetEvaluationStartTime(string EvaluationStartTime)
    {
        this.EvaluationStartTime = EvaluationStartTime;
    }

    public void SetEvaluationEndTime(string EvaluationEndTime)
    {
        this.EvaluationEndTime = EvaluationEndTime;
    }

    public Evaluation() { soccerDistance = new SoccerDistance(); }

    public Evaluation(long EvaluationID, string EvaluationStartTime, string EvaluationEndTime)
    {
        this.EvaluationID = EvaluationID;
        this.EvaluationStartTime = EvaluationStartTime;
        this.EvaluationEndTime = EvaluationEndTime;

        this.soccerDistance = DoctorDatabaseManager.instance.ReadEvaluationSoccerDistanceRecord(this.EvaluationID);
        this.Points = DoctorDatabaseManager.instance.ReadEvaluationPointsRecord(this.EvaluationID);

        this.SetEvaluationScore();
    }

    // set EvaluationID, Max_SuccessCount
    public void SetEvaluationID(long EvaluationID)
    {
        this.EvaluationID = EvaluationID;
    }
}
