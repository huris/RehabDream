using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 记录坐标集,取前两个坐标,舍弃z轴,求凸包
// 求凸包使用Melkman算法
public class Point
{
    public float x { get; set; } = 0.0f;
    public float y { get; set; } = 0.0f;
    public Point(float _x = 0.0f, float _y = 0.0f)
    {
        this.x = _x;
        this.y = _y;
    }
    public Point(Vector2 point)
    {
        this.x = point.x;
        this.y = point.y;
    }

    public class CoordinateComparer : IComparer<Point>
    {
        //实现姓名升序
        public int Compare(Point x, Point y)
        {
            if (x.y.CompareTo(y.y) == 0) return (x.x.CompareTo(y.x));
            else return (x.y.CompareTo(y.y));
        }

    }
    public static float PointsDistance(Point A, Point B)
    {
        float Distance = Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y);
        return Distance;
    }
}