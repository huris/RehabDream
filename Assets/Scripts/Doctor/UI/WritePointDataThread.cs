using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class WritePointDataThread
{
  


    //暂时存储要插入数据库的值
    //private long _EvaluationID;
    //private Point _point;
    //private Vector3 _BobathGC;
    //private DateTime _dateTime;
    private Thread _WriteDatabaseThread;
    private List<string> SQLStringList;
    public WritePointDataThread(List<string> SQLStringList)
    {
        this.SQLStringList = SQLStringList;

        _WriteDatabaseThread = new Thread(WriteDatabase);
    }
    //public WritePointDataThread(
    //    long EvaluationID,
    //    Point point,
    //    Vector3 BobathGC,
    //    DateTime dateTime
    //    )
    //{
    //    _EvaluationID = EvaluationID;
    //    _point = point;
    //    _BobathGC = BobathGC;
    //    _dateTime = dateTime;
    //    _WriteDatabaseThread = new Thread(WriteDatabase);
    //    //Debug.Log("@WriteDatabaseThread: WriteDatabaseThread Init");
    //}

    public void StartThread()
    {
        _WriteDatabaseThread.Start();
        //Debug.Log("@WriteDatabaseThread: WriteDatabaseThread Start");
    }

    private void WriteDatabase()
    {

        PatientDatabaseManager.instance.ExecuteSqlTran(SQLStringList);

        //WritePoint();


        //WriteGCData();
        //WriteAngles();
        //Debug.Log("@WriteDatabaseThread: WriteDatabaseThread Over");

        //FreeMemory();
    }

    // write Gravity Center

    //private void WriteGCData()
    //{
    //    PatientDatabaseManager.instance.WriteBobathGravityCenter(
    //        _EvaluationID,
    //        _BobathGC,
    //        _dateTime.ToString("yyyyMMdd HH:mm:ss")
    //    );
    //}
    //private void WritePoint()
    //{
    //    //Debug.Log(_EvaluationID+" "+_point.x + " " + _point.y);
    //    PatientDatabaseManager.instance.WritePoint(
    //        _EvaluationID,
    //        _point,
    //        _dateTime.ToString("yyyyMMdd HH:mm:ss")
    //    );


    //}

    //// write angles to database
    //private void WriteAngles()
    //{
    //    PatientDatabaseManager.instance.WriteAngles(
    //        _TrainingID,
    //        _Angles,
    //        _Time.ToString("yyyyMMdd HH:mm:ss")
    //    );

    //}

    // 调用可能反而性能下降
    private void FreeMemory()
    {
        _WriteDatabaseThread = null;
        GC.Collect();
    }

   
}
