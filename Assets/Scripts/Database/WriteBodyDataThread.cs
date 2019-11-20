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
    private float _LeftArmAngle;
    private float _RightArmAngle;
    private float _LeftLegAngle;
    private float _RightLegAngle;
    private DateTime _Time;
    private Thread _WriteDatabaseThread;

    public WriteBodyDataThread(
        long TrainingID,
        Vector3 GraivtyCenter, 
        float LeftArmAngle, 
        float RightArmAngle, 
        float LeftLegAngle,
        float RightLegAngle,
        DateTime Time)
    {
        _TrainingID = TrainingID;
        _GraivtyCenter = GraivtyCenter;
        _LeftArmAngle = LeftArmAngle;
        _RightArmAngle = RightArmAngle;
        _LeftLegAngle = LeftLegAngle;
        _RightLegAngle = RightLegAngle;
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
       
        DatabaseManager.instance.WriteGravityCenter(
            _TrainingID,
            _GraivtyCenter.ToString().Replace("(", "").Replace(")", ""),
            _Time.ToString("yyyyMMdd HH:mm:ss")
        );
        

    }

    // write angles to database
    private void WriteAngles()
    {
        DatabaseManager.instance.WriteAngles(
            _TrainingID,
            _LeftArmAngle,
            _RightArmAngle,
            _LeftLegAngle,
            _RightLegAngle,
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
