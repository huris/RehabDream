using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PositionCalculator
{
  
    public Action NowAction;    //正在检测的动作
    public List<Vector3> Joints;//所有关节的坐标
    private delegate int CheckMethod(List<Vector3> SpecialJoints);
    private CheckMethod _check = null;      //使用的检测方法
    private const int _FeetTogetherID = 79;     //数据库中双足站立为79号动作（从1开始）
    private const int _ArmForward = 80;         //数据库中双上肢前伸为80号动作


    public PositionCalculator(Action NowAction, List<Vector3> Joints)
    {
        this.NowAction = NowAction;
        this.Joints = Joints;

        InitCheckMethod();
    }


    // 根据ActionID选择检测方法
    private void InitCheckMethod()
    {
        switch (NowAction.id)   //不同动作不同检测方法
        {
            case _FeetTogetherID:
                this._check = new CheckMethod(CheckFeetTogether);
                break;
            case _ArmForward:
                this._check = new CheckMethod(CheckArmForward);
                break;
            default:
                this._check = new CheckMethod((x) => 100);   //不属于特殊动作则不检测，输出100分
                break;

        }
    }


    public int CheckPosition()
    {
        int Score =  _check(this.Joints);


        return Score2Result(Score);
    }

    // 双足并拢站立的检测
    private int CheckFeetTogether(List<Vector3> Joints)
    {

        int FootRight = (int)KinectInterop.JointType.FootRight;
        int FootLeft = (int)KinectInterop.JointType.FootLeft;
        int ShoulderRight = (int)KinectInterop.JointType.ShoulderRight;
        int ShoulderLeft = (int)KinectInterop.JointType.ShoulderLeft;


        float MinDistance = 0.1f;   //经测量，双足并拢时，双足间距约为0.1m
        float MaxDistance = 0.3f;   //经测量，双足自然站立时，双足间距约为0.3m
        float ShoulderDistance = (Joints[ShoulderRight] - Joints[ShoulderLeft]).magnitude;  //肩距

        MaxDistance = ShoulderDistance > MaxDistance ? ShoulderDistance : MaxDistance;  //取肩距和0.3中最大的值

        Debug.Log((Joints[FootRight] - Joints[FootLeft]).magnitude);

        // 训练中双足的实际距离
        float ActualDistance = (Joints[FootRight] - Joints[FootLeft]).magnitude;
        Debug.Log((MaxDistance - ActualDistance) / (MaxDistance - MinDistance));
        if(ActualDistance < MinDistance)
        {
            return 100; //满分100
        }
        else if (ActualDistance < MaxDistance)
        {
            return (int)(100 * (MaxDistance - ActualDistance) / (MaxDistance - MinDistance));
        }
        else
        {
            return 0;
        }
    }

    //站立双上肢前伸的检测
    private int CheckArmForward(List<Vector3> Joints)
    {
        int ShoulderRight = (int)KinectInterop.JointType.ShoulderRight;
        int ShoulderLeft = (int)KinectInterop.JointType.ShoulderLeft;
        int HandRight = (int)KinectInterop.JointType.HandRight;
        int HandLeft = (int)KinectInterop.JointType.HandLeft;

        float HandDistance = 0.3f;   //经测量，双上肢前伸时，双手间距约为0.3m
        float ShoulderDistance = (Joints[ShoulderRight] - Joints[ShoulderLeft]).magnitude;  //肩距

        HandDistance = ShoulderDistance > HandDistance ? ShoulderDistance : HandDistance;  //取肩距和0.3中最小的值

        Debug.Log((Joints[ShoulderRight] - Joints[ShoulderLeft]).magnitude);
        Debug.Log((Joints[HandRight] - Joints[HandLeft]).magnitude);

        // 训练中双手的实际距离
        float ActualDistance = (Joints[HandRight] - Joints[HandLeft]).magnitude;


        float Offset = Mathf.Abs(ActualDistance - HandDistance);
        float Percent = Mathf.Clamp( 1 - (Offset / HandDistance), 0f, 1.0f);   //限制取值范围
        Debug.Log(Percent);

        return (int)(100 * Percent);
    }


    // 从分数（百分制）转换为评级
    private int Score2Result(int Score)
    {
        if (Score >= DATA.ActionMatchThreshold["PERFECT"])       //最低准确率大于0.9 * 100
        {
            return -3;
        }
        else if (Score >= DATA.ActionMatchThreshold["GREAT"])     //最低准确率大于0.8 * 100
        {
            return -2;
        }
        else if (Score > DATA.ActionMatchThreshold["GOOD"])        //最低准确率大于0.7 * 100
        {
            return -1;
        }
        else if (NowAction.id == _FeetTogetherID)
        {
            return 14;  //左脚不标准
        }
        else //if (NowAction.id == _ArmForward)
        {
            return 34;  //肩关节不标准
        }
    }
}