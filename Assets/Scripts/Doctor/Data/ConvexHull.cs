using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

// 记录坐标集,取前两个坐标,舍弃z轴,求凸包
// 求凸包使用Melkman算法
public class ConvexHull
{
    public Point[] pointArray;  //坐标数组
    public int PointNum;
    public float ConvexHullArea = 0f;  // 凸包面积
    public Point[] TwoTable; // 数组索引，双向表
    public Point[] ConvexHullSet;  // 凸包集
    public int ConvexHullNum;  // 凸包点的个数

    public ConvexHull(){}

    public ConvexHull(List<Point> Points)
    {
        Points.Sort(new Point.CoordinateComparer());   // 对点排序一下才能用凸包算法

        this.pointArray = new Point[Points.Count];
        
        Points.Add(new Point(0.0f, 0.0f));    // 加入一个点方便下面的循环计算
        
        this.PointNum = 0;

        //print(Points.Count);
        for (int i = 0; i < Points.Count - 1; i++)
        {

            //print(Points[i].x+" "+Points[i].y+" "+i);
            // 去掉一些重复的点
            if (Points[i].x == Points[i + 1].x && Points[i].y == Points[i + 1].y)
            {
                Points.RemoveAt(i + 1);
                i--;
            }
            else
            {
                // 记录不重复的点
                this.pointArray[this.PointNum++] = Points[i];
            }
        }

        this.ConvexHullNum = ConvexHullMelkman(this.pointArray, this.PointNum);

    }

    // isLeft(): test if a point is Left|On|Right of an infinite line.
    //    Input:  three points P0, P1, and P2
    //    Return: >0 for P2 left of the line through P0 and P1
    //            =0 for P2 on the line
    //            <0 for P2 right of the line
    //    See: Algorithm 1 on Area of Triangles
    public static float isLeft(Point P0, Point P1, Point P2)
    {
        //print(P0.x + " " + P0.y +" "+P1.x+ " " + P1.y + " " + P2.x+" "+P2.y);
        return (P1.x - P0.x) * (P2.y - P0.y) - (P2.x - P0.x) * (P1.y - P0.y);
    }

    // ConvexHullMelkman(): Melkman's 2D simple polyline O(n) convex hull algorithm
    //    Input:  P[] = array of 2D vertex points for a simple polyline
    //            n   = the number of points in V[]
    //    Output: H[] = output convex hull array of vertices (max is n)
    //    Return: h   = the number of points in H[]
    public int ConvexHullMelkman(Point[] P, int n)
    {
        // initialize a deque D[] from bottom to top so that the
        // 1st three vertices of P[] are a ccw triangle
        TwoTable = new Point[2 * n + 1];
        ConvexHullSet = new Point[n];

        int bot = n - 2, top = bot + 3;    // initial bottom and top deque indices
        TwoTable[bot] = TwoTable[top] = P[2];        // 3rd vertex is at both bot and top
        if (isLeft(P[0], P[1], P[2]) > 0)
        {
            TwoTable[bot + 1] = P[0];
            TwoTable[bot + 2] = P[1];           // ccw vertices are: 2,0,1,2
        }
        else
        {
            TwoTable[bot + 1] = P[1];
            TwoTable[bot + 2] = P[0];           // ccw vertices are: 2,1,0,2
        }

        // compute the hull on the deque D[]
        for (int i = 3; i < n; i++)
        {
            //print(i + "!!!!!");
            // process the rest of vertices
            // test if next vertex is inside the deque hull
            if ((isLeft(TwoTable[bot], TwoTable[bot + 1], P[i]) > 0) &&
                (isLeft(TwoTable[top - 1], TwoTable[top], P[i]) > 0))
                continue;         // skip an interior vertex

            // incrementally add an exterior vertex to the deque hull
            // get the rightmost tangent at the deque bot
            while (isLeft(TwoTable[bot], TwoTable[bot + 1], P[i]) <= 0)
                ++bot;                 // remove bot of deque
            TwoTable[--bot] = P[i];           // insert P[i] at bot of deque

            // get the leftmost tangent at the deque top
            while (isLeft(TwoTable[top - 1], TwoTable[top], P[i]) <= 0)
                --top;                 // pop top of deque
            TwoTable[++top] = P[i];           // push P[i] onto top of deque
        }
        //print("!!!!\n");
        // transcribe deque D[] to the output hull array H[]
        int h;        // hull vertex counter
        for (h = 0; h <= (top - bot); h++)
        {
            ConvexHullSet[h] = TwoTable[bot + h];
        }

        return h - 1;
    }

}