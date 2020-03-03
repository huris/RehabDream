using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerDistance
{
    public float UponSoccer { get;  set; } = 0;
    public float UponRightSoccer { get;  set; } = 0;
    public float RightSoccer { get;  set; } = 0;
    public float DownRightSoccer { get;  set; } = 0;
    public float DownSoccer { get;  set; } = 0;
    public float DownLeftSoccer { get;  set; } = 0;
    public float LeftSoccer { get;  set; } = 0;
    public float UponLeftSoccer { get;  set; } = 0;
    public float CenterSoccerMin { get;  set; } = 0f;
    public float CenterSoccerMax { get;  set; } = 0f;


    public SoccerDistance() { }
    public SoccerDistance(float UponSoccer, float UponRightSoccer, float RightSoccer, float DownRightSoccer,
        float DownSoccer, float DownLeftSoccer, float LeftSoccer, float UponLeftSoccer, float CenterSoccerMin,
        float CenterSoccerMax)
    {
        this.UponSoccer = UponSoccer;
        this.UponRightSoccer = UponRightSoccer;
        this.RightSoccer = RightSoccer;
        this.DownRightSoccer = DownRightSoccer;
        this.DownSoccer = DownSoccer;
        this.DownLeftSoccer = DownLeftSoccer;
        this.LeftSoccer = LeftSoccer;
        this.UponLeftSoccer = UponLeftSoccer;

        this.CenterSoccerMin = CenterSoccerMin;
        this.CenterSoccerMax = CenterSoccerMax;
    }

    public void SetCompleteSoccerDistance(float UponSoccer, float UponRightSoccer, float RightSoccer, float DownRightSoccer,
        float DownSoccer, float DownLeftSoccer, float LeftSoccer, float UponLeftSoccer, float CenterSoccerMin,
        float CenterSoccerMax)
    {
        this.UponSoccer = UponSoccer;
        this.UponRightSoccer = UponRightSoccer;
        this.RightSoccer = RightSoccer;
        this.DownRightSoccer = DownRightSoccer;
        this.DownSoccer = DownSoccer;
        this.DownLeftSoccer = DownLeftSoccer;
        this.LeftSoccer = LeftSoccer;
        this.UponLeftSoccer = UponLeftSoccer;

        this.CenterSoccerMin = CenterSoccerMin;
        this.CenterSoccerMax = CenterSoccerMax;
    }

    public float[] GetMaxSoccerDistances()
    {
        return new float[10]{this.UponSoccer, this.UponRightSoccer, this.RightSoccer, this.DownRightSoccer,
                                this.DownSoccer, this.DownLeftSoccer, this.LeftSoccer, this.UponLeftSoccer,
                                    this.CenterSoccerMin, this.CenterSoccerMax};
    }

    public float GetCenterSoccerAreaDiff()
    {
        return (this.CenterSoccerMax - this.CenterSoccerMin) * (this.CenterSoccerMax - this.CenterSoccerMin) * Mathf.PI;
    }
}
