using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angle
{
    //public long TrainingID { get; private set; } = 0;
    public float LeftArmAngle { get; private set; } = 0;
    public float RightArmAngle { get; private set; } = 0;

    public float LeftLegAngle { get; private set; } = 0;
    public float RightLegAngle { get; private set; } = 0;

    public float LeftElbowAngle { get; private set; } = 0;
    public float RightElbowAngle { get; private set; } = 0;

    public float LeftHipAngle { get; private set; } = 0;
    public float RightHipAngle { get; private set; } = 0;

    public float HipAngle { get; private set; } = 0;

    public float LeftKneeAngle { get; private set; } = 0;
    public float RightKneeAngle { get; private set; } = 0;

    public float LeftAnkleAngle { get; private set; } = 0;
    public float RightAnkleAngle { get; private set; } = 0;
    public float LeftSideAngle { get; private set; } = 0;
    public float RightSideAngle { get; private set; } = 0;
    public float UponSideAngle { get; private set; } = 0;
    public float DownSideAngle { get; private set; } = 0;

    public string time { get; private set; } = "00000000 00:00:00";

    //public void SetCompleteAngles(long TrainingID, float LeftArmAngle, float RightArmAngle, float LeftLegAngle, float RightLegAngle,
    //                              float LeftElbowAngle, float RightElbowAngle, float LeftKneeAngle, float RightKneeAngle, float LeftHipAngle,
    //                              float RightHipAngle, float HipAngle, float LeftAnkleAngle, float RightAnkleAngle,
    //                              float LeftSideAngle, float RightSideAngle, float UponSideAngle, float DownSideAngle,
    //                              string time)
    //{
    //    this.TrainingID = TrainingID;
    //    this.LeftArmAngle = LeftArmAngle;
    //    this.RightArmAngle = RightArmAngle;
    //    this.LeftLegAngle = LeftLegAngle;
    //    this.RightLegAngle = RightLegAngle;
    //    this.LeftElbowAngle = LeftElbowAngle;
    //    this.RightElbowAngle = RightElbowAngle;
    //    this.LeftKneeAngle = LeftKneeAngle;
    //    this.RightKneeAngle = RightKneeAngle;
    //    this.LeftHipAngle = LeftHipAngle;
    //    this.RightHipAngle = RightHipAngle;
    //    this.HipAngle = HipAngle;
    //    this.LeftAnkleAngle = LeftAnkleAngle;
    //    this.RightAnkleAngle = RightAnkleAngle;
    //    this.LeftSideAngle = LeftSideAngle;
    //    this.RightSideAngle = RightSideAngle;
    //    this.UponSideAngle = UponSideAngle;
    //    this.DownSideAngle = DownSideAngle;
    //    this.time = time;
    //}

    public void SetCompleteAngles(float LeftArmAngle, float RightArmAngle, float LeftLegAngle, float RightLegAngle,
                              float LeftElbowAngle, float RightElbowAngle, float LeftKneeAngle, float RightKneeAngle, float LeftHipAngle,
                              float RightHipAngle, float HipAngle, float LeftAnkleAngle, float RightAnkleAngle,
                              float LeftSideAngle, float RightSideAngle, float UponSideAngle, float DownSideAngle,
                              string time)
    {
        this.LeftArmAngle = LeftArmAngle;
        this.RightArmAngle = RightArmAngle;
        this.LeftLegAngle = LeftLegAngle;
        this.RightLegAngle = RightLegAngle;
        this.LeftElbowAngle = LeftElbowAngle;
        this.RightElbowAngle = RightElbowAngle;
        this.LeftKneeAngle = LeftKneeAngle;
        this.RightKneeAngle = RightKneeAngle;
        this.LeftHipAngle = LeftHipAngle;
        this.RightHipAngle = RightHipAngle;
        this.HipAngle = HipAngle;
        this.LeftAnkleAngle = LeftAnkleAngle;
        this.RightAnkleAngle = RightAnkleAngle;
        this.LeftSideAngle = LeftSideAngle;
        this.RightSideAngle = RightSideAngle;
        this.UponSideAngle = UponSideAngle;
        this.DownSideAngle = DownSideAngle;
        this.time = time;
    }
}
