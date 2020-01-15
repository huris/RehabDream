using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction
{
    //public long TrainingID { get; private set; } = 0;
    public float UponDirection { get; private set; } = 0;
    public float UponRightDirection { get; private set; } = 0;
    public float RightDirection { get; private set; } = 0;
    public float DownRightDirection { get; private set; } = 0;
    public float DownDirection { get; private set; } = 0;
    public float DownLeftDirection { get; private set; } = 0;
    public float LeftDirection { get; private set; } = 0;
    public float UponLeftDirection { get; private set; } = 0;
    public float DirectionRadarArea { get; private set; } = 0f;
    public List<float> DirectionAreaRate { get; private set; } = null;

    public Direction(float UponDirection, float UponRightDirection, float RightDirection, float DownRightDirection,
        float DownDirection, float DownLeftDirection, float LeftDirection, float UponLeftDirection)
    {
        this.UponDirection = UponDirection;
        this.UponRightDirection = UponRightDirection;
        this.RightDirection = RightDirection;
        this.DownRightDirection = DownRightDirection;
        this.DownDirection = DownDirection;
        this.DownLeftDirection = DownLeftDirection;
        this.LeftDirection = LeftDirection;
        this.UponLeftDirection = UponLeftDirection;

        this.DirectionRadarArea = this.GetRadarArea();
        this.DirectionAreaRate = this.GetDirectionAreaRate();
    }

    public void SetCompleteDirections(float UponDirection, float UponRightDirection, float RightDirection, float DownRightDirection,
        float DownDirection, float DownLeftDirection, float LeftDirection, float UponLeftDirection)
    {
        this.UponDirection = UponDirection;
        this.UponRightDirection = UponRightDirection;
        this.RightDirection = RightDirection;
        this.DownRightDirection = DownRightDirection;
        this.DownDirection = DownDirection;
        this.DownLeftDirection = DownLeftDirection;
        this.LeftDirection = LeftDirection;
        this.UponLeftDirection = UponLeftDirection;

        this.DirectionRadarArea = this.GetRadarArea();
        this.DirectionAreaRate = this.GetDirectionAreaRate();
    }

    public List<float> GetDirections()
    {
        return new List<float> {this.UponDirection, this.UponRightDirection, this.RightDirection, this.DownRightDirection,
                                this.DownDirection, this.DownLeftDirection, this.LeftDirection, this.UponLeftDirection};
    }

    public float[] GetDirectionsArray()
    {
        return new float[8]{this.UponDirection, this.UponRightDirection, this.RightDirection, this.DownRightDirection,
                                this.DownDirection, this.DownLeftDirection, this.LeftDirection, this.UponLeftDirection};
    }

    public float GetRadarArea()
    {
        return 1.0f / 2 * (Mathf.Sqrt(2) / 2) * (this.UponDirection * this.UponRightDirection + this.UponRightDirection * this.RightDirection +
           this.RightDirection * this.DownRightDirection + this.DownRightDirection * this.DownDirection +
           this.DownDirection * this.DownLeftDirection + this.DownLeftDirection * this.LeftDirection +
           this.LeftDirection * this.UponLeftDirection + this.UponLeftDirection * this.UponDirection);
    }

    public List<float> GetDirectionAreaRate()
    {
        return new List<float>{ this.UponDirection/this.DirectionRadarArea, this.UponRightDirection/this.DirectionRadarArea, 
                                this.RightDirection/this.DirectionRadarArea, this.DownRightDirection/this.DirectionRadarArea,
                                this.DownDirection/this.DirectionRadarArea, this.DownLeftDirection/this.DirectionRadarArea,
                                this.LeftDirection/this.DirectionRadarArea, this.UponLeftDirection/this.DirectionRadarArea};
    }
}
