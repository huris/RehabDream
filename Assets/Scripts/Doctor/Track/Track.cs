using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//显示足球弹道
public class Track : MonoBehaviour {

    public LineRenderer[] LineRendererList;    //lines of arrow
    public LineRenderer LeftLine;
    public LineRenderer RightLine;

    public int SegmentCount = 30;  //count of lines
    public float SegmentWidth = 0.05f; //
    public Color StartColor = Color.red;
    public Color EndColor = Color.yellow;
    public Material LineMaterial;

    private float HeighDivideWidth = 829 / 631;
    public float gap = 0.2f;
    public Vector3[] Positions;


    // Use this for initialization
    void Start () {
        InitTrack();
        //GenerateArrow(new Vector3 (1,1,1),new Vector3(13,1,2),2,-9.8f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}



    public void InitTrack()
    {
        this.LineRendererList = new LineRenderer[SegmentCount];
        for (int i = 0; i < SegmentCount; i++)
        {
            GameObject ArrowObject = new GameObject("Arrow_" + i);
            ArrowObject.transform.SetParent(gameObject.transform);

            //添加LineRenderer组件
            LineRenderer lineRenderer = ArrowObject.AddComponent<LineRenderer>();

            //设置材质
            lineRenderer.material = LineMaterial;
            //设置起始颜色
            //lineRenderer.startColor = StartColor;
            //设置结束颜色
            //lineRenderer.endColor = EndColor;
            //设置起始宽度
            lineRenderer.startWidth= SegmentWidth;
            //设置结束宽度
            lineRenderer.endWidth = SegmentWidth;
            //加入列表
            LineRendererList[i] = lineRenderer;
        }

        GameObject LeftArrowObject = new GameObject("Arrow_Left");
        LeftArrowObject.transform.SetParent(gameObject.transform);

        //添加LineRenderer组件
        LeftLine = LeftArrowObject.AddComponent<LineRenderer>();
        LeftLine.material = LineMaterial;
        LeftLine.startWidth = SegmentWidth;
        LeftLine.startWidth = SegmentWidth;

        GameObject RightArrowObject = new GameObject("Arrow_Right");
        RightArrowObject.transform.SetParent(gameObject.transform);

        //添加LineRenderer组件
        RightLine = RightArrowObject.AddComponent<LineRenderer>();
        RightLine.material = LineMaterial;
        RightLine.startWidth = SegmentWidth;
        LeftLine.startWidth = SegmentWidth;
    }


    //设定Arrow的坐标
    public void GenerateTrack(Vector3 StartPoint, Vector3 ControlPoint, Vector3 EndPoint)
    {

        //需要在射门前提前计算轨道
        this.Positions = BezierUtils.GetBeizerList(StartPoint, ControlPoint, EndPoint, SegmentCount + 1);

        //一个LineRenderer需要两个坐标
        //两个LineRenderer之间首尾相接
        //所以需要LineRendererList.Length+1个坐标
        for (int i = 0; i < SegmentCount; i++)
        {
            Vector3 Start = this.Positions[i];
            this.LineRendererList[i].SetPosition(0, Start); //设置起始点

            //使各个LineRenderer间保持一定距离，不要相交
            Vector3 delta = this.Positions[i + 1] - this.Positions[i];
            Vector3 change;
            change.x = delta.x * gap;
            change.y = delta.y * gap;
            change.z = delta.z * gap;
            Vector3 End = this.Positions[i + 1] - change;
            this.LineRendererList[i].SetPosition(1, End);   //设置终点

            if((i+1) == SegmentCount)   // 箭镞的两翼
            {
                float Offset =  (End - Start).sqrMagnitude/2;
                Vector3 LeftStart = new Vector3(End.x - 4 *Offset, End.y + Offset, End.z - 2*Offset);
                Vector3 RightStart = new Vector3(End.x - 4 * Offset, End.y + Offset, End.z + 2*Offset);
                Vector3 LeftEnd = new Vector3(End.x , End.y , End.z);
                Vector3 RightEnd = new Vector3(End.x , End.y , End.z);

                this.LeftLine.SetPosition(0, LeftStart);
                this.LeftLine.SetPosition(1, LeftEnd);

                this.RightLine.SetPosition(0, RightStart);
                this.RightLine.SetPosition(1, RightEnd);
            }
        }
    }
}
