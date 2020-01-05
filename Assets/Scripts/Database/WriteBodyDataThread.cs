/* ============================================================================== 
* ClassName：WriteBodyDataThread 
* Author：ChenShuwei 
* CreateDate：2019/11/16 10:29:11 
* Version: 1.0
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class WriteBodyDataThread
{
    //暂时存储要插入数据库的值
    private long _TrainingID;
    private Vector3 _GraivtyCenter;
    private float[] _Angles = new float[17];
    private DateTime _Time;
    private Thread _WriteDatabaseThread;

    public WriteBodyDataThread(
        long TrainingID,
        Vector3 GraivtyCenter,
        float LeftArmAngle,
        float RightArmAngle,
        float LeftLegAngle,
        float RightLegAngle,
        float LeftElbowAngle,
        float RightElbowAngle,
        float LeftKneeAngle,
        float RightKneeAngle,
        float LeftAnkleAngle,
        float RightAnkleAngle,
        float LeftHipAngle,
        float RightHipAngle,
        float HipAngle,
        float LeftSideAngle,
        float RightSideAngle,
        float UponSideAngle,
        float DownSideAngle,
        DateTime Time)
    {
        _TrainingID = TrainingID;
        _GraivtyCenter = GraivtyCenter;
        _Angles[0] = LeftArmAngle;
        _Angles[1] = RightArmAngle;
        _Angles[2] = LeftLegAngle;
        _Angles[3] = RightLegAngle;
        _Angles[4] = LeftElbowAngle;
        _Angles[5] = RightElbowAngle;
        _Angles[6] = LeftKneeAngle;
        _Angles[7] = RightKneeAngle;
        _Angles[8] = LeftAnkleAngle;
        _Angles[9] = RightAnkleAngle;
        _Angles[10] = LeftHipAngle;
        _Angles[11] = RightHipAngle;
        _Angles[12] = HipAngle;
        _Angles[13] = LeftSideAngle;
        _Angles[14] = RightSideAngle;
        _Angles[15] = UponSideAngle;
        _Angles[16] = DownSideAngle;
        _Time = Time;
        _WriteDatabaseThread = new Thread(WriteDatabase);
        //Debug.Log("@WriteDatabaseThread: WriteDatabaseThread Init");
    }

    public void StartThread()
    {
        _WriteDatabaseThread.Start();
        //Debug.Log("@WriteDatabaseThread: WriteDatabaseThread Start");
    }

    private void WriteDatabase()
    {
        WriteGravityCenter();
        WriteAngles();
        //Debug.Log("@WriteDatabaseThread: WriteDatabaseThread Over");

        //FreeMemory();
    }

    // write Gravity Center
    private void WriteGravityCenter()
    {

        PatientDatabaseManager.instance.WriteGravityCenter(
            _TrainingID,
            _GraivtyCenter.ToString().Replace("(", "").Replace(")", ""),
            _Time.ToString("yyyyMMdd HH:mm:ss")
        );


    }

    // write angles to database
    private void WriteAngles()
    {
        PatientDatabaseManager.instance.WriteAngles(
            _TrainingID,
            _Angles,
            _Time.ToString("yyyyMMdd HH:mm:ss")
        );

    }

    // 调用可能反而性能下降
    private void FreeMemory()
    {
        _WriteDatabaseThread = null;
        GC.Collect();
    }


}
