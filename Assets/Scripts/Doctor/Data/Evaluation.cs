 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Evaluation
{
    public long EvaluationID { get; private set; } = 0;
    //public float EvaluationWidth { get; private set; } = 0.0f;
    public float EvaluationRatio { get; private set; } = 1.0f;
    public float EvaluationHeight { get; private set; } = 500.0f;
    public string EvaluationStartTime { get; private set; } = "00000000 00:00:00";
    public string EvaluationEndTime { get; private set; } = "00000000 00:00:00";

    public List<Point> Points = new List<Point>();  // 数据点的集合 

    public List<GravityCenter> GravityCenters = new List<GravityCenter>();  // 重心点集合

    public SoccerDistance soccerDistance = new SoccerDistance();  // 四周8个方向足球位移的最大值和中间足球的最大最小面积

    public float EvaluationScore { get; set; } = 0.0f;


    //public void SetEvaluationWidth(float EvaluationWidth)    // 求肩宽
    //{
    //    this.EvaluationWidth = EvaluationWidth;
    //}
    public void SetEvaluationRatio(float EvaluationRatio)
    {
        this.EvaluationRatio = EvaluationRatio;
    }


    public void SetEvaluationHeight(float EvaluationHeight)    // 求身高段
    {
        this.EvaluationHeight = EvaluationHeight;
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

    public Evaluation(long EvaluationID, float EvaluationRatio, float EvaluationHeight, string EvaluationStartTime, string EvaluationEndTime, float EvaluationScore)
    {
        this.EvaluationID = EvaluationID;
        //this.EvaluationWidth = EvaluationWidth;
        this.EvaluationRatio = EvaluationRatio;
        this.EvaluationHeight = EvaluationHeight;
        this.EvaluationStartTime = EvaluationStartTime;
        this.EvaluationEndTime = EvaluationEndTime;
        this.EvaluationScore = EvaluationScore;

        this.soccerDistance = DoctorDatabaseManager.instance.ReadEvaluationSoccerDistanceRecord(this.EvaluationID);
        this.Points = DoctorDatabaseManager.instance.ReadEvaluationPointsRecord(this.EvaluationID);
        this.GravityCenters = DoctorDatabaseManager.instance.ReadEvaluationGravityCenterRecord(this.EvaluationID);

        //this.SetEvaluationScore();
    }

    // set EvaluationID, Max_SuccessCount
    public void SetEvaluationID(long EvaluationID)
    {
        this.EvaluationID = EvaluationID;
    }
}
