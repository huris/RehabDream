using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerDistance
{
    public float UponSoccerDistance { get;  set; } = 0.0f;
    public float UponRightSoccerDistance { get;  set; } = 0.0f;
    public float RightSoccerDistance { get;  set; } = 0.0f;
    public float DownRightSoccerDistance { get;  set; } = 0.0f;
    public float DownSoccerDistance { get;  set; } = 0.0f;
    public float DownLeftSoccerDistance { get;  set; } = 0.0f;
    public float LeftSoccerDistance { get;  set; } = 0.0f;
    public float UponLeftSoccerDistance { get;  set; } = 0.0f;
    public float FrontSoccerDistance { get;  set; } = 0.0f;
    public float BehindSoccerDistance { get; set; } = 0.0f;
    public long UponSoccerScore { get; set; } = 0;
    public long UponRightSoccerScore { get; set; } = 0;
    public long RightSoccerScore { get; set; } = 0;
    public long DownRightSoccerScore { get; set; } = 0;
    public long DownSoccerScore { get; set; } = 0;
    public long DownLeftSoccerScore { get; set; } = 0;
    public long LeftSoccerScore { get; set; } = 0;
    public long UponLeftSoccerScore { get; set; } = 0;
    public long FrontSoccerScore { get; set; } = 0;
    public long BehindSoccerScore { get; set; } = 0;
    public long UponSoccerTime { get; set; } = 0;
    public long UponRightSoccerTime { get; set; } = 0;
    public long RightSoccerTime { get; set; } = 0;
    public long DownRightSoccerTime { get; set; } = 0;
    public long DownSoccerTime { get; set; } = 0;
    public long DownLeftSoccerTime { get; set; } = 0;
    public long LeftSoccerTime { get; set; } = 0;
    public long UponLeftSoccerTime { get; set; } = 0;
    public long FrontSoccerTime { get; set; } = 0;
    public long BehindSoccerTime { get; set; } = 0;


    public SoccerDistance() { }
    public SoccerDistance(float UponSoccerDistance, float UponRightSoccerDistance, float RightSoccerDistance, float DownRightSoccerDistance,
        float DownSoccerDistance, float DownLeftSoccerDistance, float LeftSoccerDistance, float UponLeftSoccerDistance, 
        float FrontSoccerDistance, float BehindSoccerDistance,
        long UponSoccerScore, long UponRightSoccerScore, long RightSoccerScore, long DownRightSoccerScore,
        long DownSoccerScore, long DownLeftSoccerScore, long LeftSoccerScore, long UponLeftSoccerScore,
        long FrontSoccerScore, long BehindSoccerScore,
        long UponSoccerTime, long UponRightSoccerTime, long RightSoccerTime, long DownRightSoccerTime,
        long DownSoccerTime, long DownLeftSoccerTime, long LeftSoccerTime, long UponLeftSoccerTime,
        long FrontSoccerTime, long BehindSoccerTime)
    {
        this.UponSoccerDistance = UponSoccerDistance;
        this.UponRightSoccerDistance = UponRightSoccerDistance;
        this.RightSoccerDistance = RightSoccerDistance;
        this.DownRightSoccerDistance = DownRightSoccerDistance;
        this.DownSoccerDistance = DownSoccerDistance;
        this.DownLeftSoccerDistance = DownLeftSoccerDistance;
        this.LeftSoccerDistance = LeftSoccerDistance;
        this.UponLeftSoccerDistance = UponLeftSoccerDistance;
        this.FrontSoccerDistance = FrontSoccerDistance;
        this.BehindSoccerDistance = BehindSoccerDistance;

        this.UponSoccerScore = UponSoccerScore;
        this.UponRightSoccerScore = UponRightSoccerScore;
        this.RightSoccerScore = RightSoccerScore;
        this.DownRightSoccerScore = DownRightSoccerScore;
        this.DownSoccerScore = DownSoccerScore;
        this.DownLeftSoccerScore = DownLeftSoccerScore;
        this.LeftSoccerScore = LeftSoccerScore;
        this.UponLeftSoccerScore = UponLeftSoccerScore;
        this.FrontSoccerScore = FrontSoccerScore;
        this.BehindSoccerScore = BehindSoccerScore;

        this.UponSoccerTime = UponSoccerTime;
        this.UponRightSoccerTime = UponRightSoccerTime;
        this.RightSoccerTime = RightSoccerTime;
        this.DownRightSoccerTime = DownRightSoccerTime;
        this.DownSoccerTime = DownSoccerTime;
        this.DownLeftSoccerTime = DownLeftSoccerTime;
        this.LeftSoccerTime = LeftSoccerTime;
        this.UponLeftSoccerTime = UponLeftSoccerTime;
        this.FrontSoccerTime = FrontSoccerTime;
        this.BehindSoccerTime = BehindSoccerTime;

    }

    public SoccerDistance(SoccerDistance soccerDistance)
    {
        this.UponSoccerDistance = soccerDistance.UponSoccerDistance;
        this.UponRightSoccerDistance = soccerDistance.UponRightSoccerDistance;
        this.RightSoccerDistance = soccerDistance.RightSoccerDistance;
        this.DownRightSoccerDistance = soccerDistance.DownRightSoccerDistance;
        this.DownSoccerDistance = soccerDistance.DownSoccerDistance;
        this.DownLeftSoccerDistance = soccerDistance.DownLeftSoccerDistance;
        this.LeftSoccerDistance = soccerDistance.LeftSoccerDistance;
        this.UponLeftSoccerDistance = soccerDistance.UponLeftSoccerDistance;
        this.FrontSoccerDistance = soccerDistance.FrontSoccerDistance;
        this.BehindSoccerDistance = soccerDistance.BehindSoccerDistance;

        this.UponSoccerScore = soccerDistance.UponSoccerScore;
        this.UponRightSoccerScore = soccerDistance.UponRightSoccerScore;
        this.RightSoccerScore = soccerDistance.RightSoccerScore;
        this.DownRightSoccerScore = soccerDistance.DownRightSoccerScore;
        this.DownSoccerScore = soccerDistance.DownSoccerScore;
        this.DownLeftSoccerScore = soccerDistance.DownLeftSoccerScore;
        this.LeftSoccerScore = soccerDistance.LeftSoccerScore;
        this.UponLeftSoccerScore = soccerDistance.UponLeftSoccerScore;
        this.FrontSoccerScore = soccerDistance.FrontSoccerScore;
        this.BehindSoccerScore = soccerDistance.BehindSoccerScore;

        this.UponSoccerTime = soccerDistance.UponSoccerTime;
        this.UponRightSoccerTime = soccerDistance.UponRightSoccerTime;
        this.RightSoccerTime = soccerDistance.RightSoccerTime;
        this.DownRightSoccerTime = soccerDistance.DownRightSoccerTime;
        this.DownSoccerTime = soccerDistance.DownSoccerTime;
        this.DownLeftSoccerTime = soccerDistance.DownLeftSoccerTime;
        this.LeftSoccerTime = soccerDistance.LeftSoccerTime;
        this.UponLeftSoccerTime = soccerDistance.UponLeftSoccerTime;
        this.FrontSoccerTime = soccerDistance.FrontSoccerTime;
        this.BehindSoccerTime = soccerDistance.BehindSoccerTime;
    }

    public void SetCompleteSoccerDistance(float UponSoccerDistance, float UponRightSoccerDistance, float RightSoccerDistance, float DownRightSoccerDistance,
        float DownSoccerDistance, float DownLeftSoccerDistance, float LeftSoccerDistance, float UponLeftSoccerDistance,
        float FrontSoccerDistance, float BehindSoccerDistance,
        long UponSoccerTime, long UponRightSoccerTime, long RightSoccerTime, long DownRightSoccerTime,
        long DownSoccerTime, long DownLeftSoccerTime, long LeftSoccerTime, long UponLeftSoccerTime,
        long FrontSoccerTime, long BehindSoccerTime)
    {
        this.UponSoccerDistance = UponSoccerDistance;
        this.UponRightSoccerDistance = UponRightSoccerDistance;
        this.RightSoccerDistance = RightSoccerDistance;
        this.DownRightSoccerDistance = DownRightSoccerDistance;
        this.DownSoccerDistance = DownSoccerDistance;
        this.DownLeftSoccerDistance = DownLeftSoccerDistance;
        this.LeftSoccerDistance = LeftSoccerDistance;
        this.UponLeftSoccerDistance = UponLeftSoccerDistance;
        this.FrontSoccerDistance = FrontSoccerDistance;
        this.BehindSoccerDistance = BehindSoccerDistance;

        this.UponSoccerScore = UponSoccerScore;
        this.UponRightSoccerScore = UponRightSoccerScore;
        this.RightSoccerScore = RightSoccerScore;
        this.DownRightSoccerScore = DownRightSoccerScore;
        this.DownSoccerScore = DownSoccerScore;
        this.DownLeftSoccerScore = DownLeftSoccerScore;
        this.LeftSoccerScore = LeftSoccerScore;
        this.UponLeftSoccerScore = UponLeftSoccerScore;
        this.FrontSoccerScore = FrontSoccerScore;
        this.BehindSoccerScore = BehindSoccerScore;

        this.UponSoccerTime = UponSoccerTime;
        this.UponRightSoccerTime = UponRightSoccerTime;
        this.RightSoccerTime = RightSoccerTime;
        this.DownRightSoccerTime = DownRightSoccerTime;
        this.DownSoccerTime = DownSoccerTime;
        this.DownLeftSoccerTime = DownLeftSoccerTime;
        this.LeftSoccerTime = LeftSoccerTime;
        this.UponLeftSoccerTime = UponLeftSoccerTime;
        this.FrontSoccerTime = FrontSoccerTime;
        this.BehindSoccerTime = BehindSoccerTime;
    }

    public float SumScore()
    {
        float a1 = 5f;
        float a2 = 6f;
        float a3 = 7f;

        //return a1 * this.UponSoccerDistance * this.UponSoccerScore / this.UponSoccerTime 
        //     + a1 * this.UponRightSoccerDistance * this.UponRightSoccerScore / this.UponRightSoccerTime
        //     + a1 * this.RightSoccerDistance * this.RightSoccerScore / this.RightSoccerTime 
        //     + a1 * this.DownRightSoccerDistance * this.DownRightSoccerScore / this.DownRightSoccerTime
        //     + a1 * this.DownSoccerDistance * this.DownSoccerScore / this.DownSoccerTime 
        //     + a1 * this.DownLeftSoccerDistance * this.DownLeftSoccerScore / this.DownLeftSoccerTime
        //     + a1 * this.LeftSoccerDistance * this.LeftSoccerScore / this.LeftSoccerTime 
        //     + a1 * this.UponLeftSoccerDistance * this.UponLeftSoccerScore / this.UponLeftSoccerTime
        //     + a2 * this.FrontSoccerDistance * this.FrontSoccerScore / this.FrontSoccerTime 
        //     + a3 * this.BehindSoccerDistance * this.BehindSoccerScore / this.BehindSoccerTime;


        return this.UponSoccerScore +
             this.UponRightSoccerScore 
             +  this.RightSoccerScore 
             +  this.DownRightSoccerScore 
             +  this.DownSoccerScore 
             +  this.DownLeftSoccerScore 
             +  this.LeftSoccerScore 
             +  this.UponLeftSoccerScore 
             +  this.FrontSoccerScore 
             +  this.BehindSoccerScore ;
    }

    //public float[] GetMaxSoccerDistances()
    //{
    //    return new float[10]{this.UponSoccer.magnitude, this.UponRightSoccer.magnitude, this.RightSoccer.magnitude, this.DownRightSoccer.magnitude,
    //                            this.DownSoccer.magnitude, this.DownLeftSoccer.magnitude, this.LeftSoccer.magnitude, this.UponLeftSoccer.magnitude,
    //                                this.CenterSoccerMin, this.CenterSoccerMax};
    //}

    //public float GetCenterSoccerAreaDiff()
    //{
    //    return (this.CenterSoccerMax - this.CenterSoccerMin) * (this.CenterSoccerMax - this.CenterSoccerMin) * Mathf.PI;
    //}
}
