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

    public List<float> GetDirections()
    {
        return new List<float> {this.UponDirection, this.UponRightDirection, this.RightDirection, this.DownRightDirection,
                                this.DownDirection, this.DownLeftDirection, this.LeftDirection, this.UponLeftDirection};
    }
}
