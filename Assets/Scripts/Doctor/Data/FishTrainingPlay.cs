using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTrainingPlay
{
    public long TrainingID { get; private set; } = 0;
    public string TrainingStartTime { get; private set; } = "00000000 00:00:00";
    public string TrainingEndTime { get; private set; } = "00000000 00:00:00";
    public long PlanDuration { get; private set; } = 0;
    public long TrainingDirection { get; private set; } = 0;      // 方位偏向： 0:两侧一致,1:左侧重点,2:右侧重点
    public long Bonus { get; private set; } = 0;    // 奖金
    public long StaticFishSuccessCount { get; private set; } = 0;   // 静态捕获鱼
    public long StaticFishAllCount { get; private set; } = 0;   // 静态总鱼
    public long DynamicFishSuccessCount { get; private set; } = 0;   // 动态捕获鱼
    public long DynamicFishAllCount { get; private set; } = 0;   // 动态总鱼
    
    public List<float> FishCaptureTime = new List<float>(); // 捕鱼花费时长
    public long Experience { get; private set; } = 0;   // 至此经验值
    public long Distance { get; private set; } = 0; // 路程

    public List<float> GCAngles = new List<float>();

    //public List<GravityCenter> gravityCenters = null;   // 患者重心变化
    public float TrainingScore { get; private set; } = 0.0f; // 训练得分


    public void SetFishTrainingPlay(long Bonus, long StaticFishSuccessCount, long StaticFishAllCount,
         long DynamicFishSuccessCount, long DynamicFishAllCount, List<float> FishCaptureTime,
         long Experience, long Distance, List<float> GCAngles, float TrainingScore)
    {
        this.Bonus = Bonus;
        this.StaticFishSuccessCount = StaticFishSuccessCount;
        this.StaticFishAllCount = StaticFishAllCount;
        this.DynamicFishSuccessCount = DynamicFishSuccessCount;
        this.DynamicFishAllCount = DynamicFishAllCount;
        this.FishCaptureTime = FishCaptureTime;
        this.Experience = Experience;
        this.Distance = Distance;
        this.GCAngles = GCAngles;
        this.TrainingScore = TrainingScore;
        //this.gravityCenters = DoctorDatabaseManager.instance.ReadFishGravityCenterRecord(this.TrainingID);
    }

    public void SetPlanDuration(long PlanDuration)
    {
        this.PlanDuration = PlanDuration;
    }

    public void SetTrainingDirection(long TrainingDirection)
    {
        this.TrainingDirection = TrainingDirection;
    }

    public void SetTrainingStartTime(string TrainingStartTime)
    {
        this.TrainingStartTime = TrainingStartTime;
    }

    public void SetTrainingEndTime(string TrainingEndTime)
    {
        this.TrainingEndTime = TrainingEndTime;
    }


    public FishTrainingPlay() { this.TrainingID = DoctorDatabaseManager.instance.ReadPatientFishRecordCount(); }

    public FishTrainingPlay(long TrainingID, string TrainingStartTime, string TrainingEndTime, long PlanDuration,
        long TrainingDirection, long Bonus, long StaticFishSuccessCount, long StaticFishAllCount,
        long DynamicFishSuccessCount, long DynamicFishAllCount, List<float> FishCaptureTime,
        long Experience, long Distance, List<float> GCAngles, float TrainingScore)
    {
        this.TrainingID = TrainingID;
        this.TrainingStartTime = TrainingStartTime;
        this.TrainingEndTime = TrainingEndTime;
        this.PlanDuration = PlanDuration;
        this.TrainingDirection = TrainingDirection;
        this.Bonus = Bonus;
        this.StaticFishSuccessCount = StaticFishSuccessCount;
        this.StaticFishAllCount = StaticFishAllCount;
        this.DynamicFishSuccessCount = DynamicFishSuccessCount;
        this.DynamicFishAllCount = DynamicFishAllCount;
        this.FishCaptureTime = FishCaptureTime;
        this.Experience = Experience;
        this.Distance = Distance;
        this.GCAngles = GCAngles;
        this.TrainingScore = TrainingScore;

        //this.gravityCenters = DoctorDatabaseManager.instance.ReadFishGravityCenterRecord(this.TrainingID);
    }


    public void SetCompleteTrainingPlay(long TrainingID, string TrainingStartTime, string TrainingEndTime,
        long PlanDuration, long TrainingDirection, long Bonus, long StaticFishSuccessCount, long StaticFishAllCount,
        long DynamicFishSuccessCount, long DynamicFishAllCount, List<float> FishCaptureTime,
        long Experience, long Distance, List<float> GCAngles, float TrainingScore)
    {
        this.TrainingID = TrainingID;
        this.TrainingStartTime = TrainingStartTime;
        this.TrainingEndTime = TrainingEndTime;
        this.PlanDuration = PlanDuration;
        this.TrainingDirection = TrainingDirection;
        this.Bonus = Bonus;
        this.StaticFishSuccessCount = StaticFishSuccessCount;
        this.StaticFishAllCount = StaticFishAllCount;
        this.DynamicFishSuccessCount = DynamicFishSuccessCount;
        this.DynamicFishAllCount = DynamicFishAllCount;
        this.FishCaptureTime = FishCaptureTime;
        this.Experience = Experience;
        this.Distance = Distance;
        this.GCAngles = GCAngles;
        this.TrainingScore = TrainingScore;

        //this.gravityCenters = DoctorDatabaseManager.instance.ReadFishGravityCenterRecord(this.TrainingID);
    }
}
