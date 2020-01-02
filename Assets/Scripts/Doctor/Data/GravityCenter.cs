using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCenter{

    //public long TrainingID { get; private set; } = 0;
    public Vector3 Coordinate { get; private set; } = new Vector3(0f,0f,0f);
    public string Time { get; private set; } = "00000000 00:00:00";

    public void SetCoordinate(Vector3 Coordinate)
    {
        this.Coordinate = Coordinate;
    }

    //public void SetCompleteGravityCenter(long TrainingID, Vector3 Coordinate, string Time)
    //{
    //    this.TrainingID = TrainingID;
    //    this.Coordinate = Coordinate;
    //    this.Time = Time;
    //}

    public void SetCompleteGravityCenter(Vector3 Coordinate, string Time)
    {
        this.Coordinate = Coordinate;
        this.Time = Time;
    }
} 
