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
