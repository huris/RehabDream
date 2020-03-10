using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerDistance
{
    public Vector3 UponSoccer { get;  set; } = new Vector3(0, 0, 0);
    public Vector3 UponRightSoccer { get;  set; } = new Vector3(0, 0, 0);
    public Vector3 RightSoccer { get;  set; } = new Vector3(0, 0, 0);
    public Vector3 DownRightSoccer { get;  set; } = new Vector3(0, 0, 0);
    public Vector3 DownSoccer { get;  set; } = new Vector3(0, 0, 0);
    public Vector3 DownLeftSoccer { get;  set; } = new Vector3(0, 0, 0);
    public Vector3 LeftSoccer { get;  set; } = new Vector3(0, 0, 0);
    public Vector3 UponLeftSoccer { get;  set; } = new Vector3(0, 0, 0);
    public float CenterSoccerMin { get;  set; } = 3.5f;
    public float CenterSoccerMax { get; set; } = 3.5f;


    public SoccerDistance() { }
    public SoccerDistance(Vector3 UponSoccer, Vector3 UponRightSoccer, Vector3 RightSoccer, Vector3 DownRightSoccer,
        Vector3 DownSoccer, Vector3 DownLeftSoccer, Vector3 LeftSoccer, Vector3 UponLeftSoccer, float CenterSoccerMin,
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

    public SoccerDistance(SoccerDistance soccerDistance)
    {
        this.UponSoccer = soccerDistance.UponSoccer;
        this.UponRightSoccer = soccerDistance.UponRightSoccer;
        this.RightSoccer = soccerDistance.RightSoccer;
        this.DownRightSoccer = soccerDistance.DownRightSoccer;
        this.DownSoccer = soccerDistance.DownSoccer;
        this.DownLeftSoccer = soccerDistance.DownLeftSoccer;
        this.LeftSoccer = soccerDistance.LeftSoccer;
        this.UponLeftSoccer = soccerDistance.UponLeftSoccer;

        this.CenterSoccerMin = soccerDistance.CenterSoccerMin;
        this.CenterSoccerMax = soccerDistance.CenterSoccerMax;
    }

    public void SetCompleteSoccerDistance(Vector3 UponSoccer, Vector3 UponRightSoccer, Vector3 RightSoccer, Vector3 DownRightSoccer,
        Vector3 DownSoccer, Vector3 DownLeftSoccer, Vector3 LeftSoccer, Vector3 UponLeftSoccer, float CenterSoccerMin,
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
        return new float[10]{this.UponSoccer.magnitude, this.UponRightSoccer.magnitude, this.RightSoccer.magnitude, this.DownRightSoccer.magnitude,
                                this.DownSoccer.magnitude, this.DownLeftSoccer.magnitude, this.LeftSoccer.magnitude, this.UponLeftSoccer.magnitude,
                                    this.CenterSoccerMin, this.CenterSoccerMax};
    }

    public float GetCenterSoccerAreaDiff()
    {
        return (this.CenterSoccerMax - this.CenterSoccerMin) * (this.CenterSoccerMax - this.CenterSoccerMin) * Mathf.PI;
    }
}
