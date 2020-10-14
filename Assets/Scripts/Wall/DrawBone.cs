using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawBone : MonoBehaviour
{
    int CircleNodeNum;
    int LineRadius = 5;
    int EdgeLength = 10;
    //private Texture2D jointTexture;
    //private Texture2D horizontalTexture;
    //private Texture2D verticalTexture;
    Texture2D m_texture;
    Color lineColor;
    int TextureWidth;
    int TextureHeight;
    public List<Vector3> actionPos;
    public List<KinectInterop.TrackingState> trackingState;
    RectTransform m_rect;
    Vector3 offset;
    Vector3 spinePos;
    public float scale = 0.5f;
    int compressTimes = 2;
    private void Start()
    {
        lineColor = Color.red;
        actionPos = new List<Vector3>();
        m_rect = gameObject.GetComponent<RectTransform>();
        TextureWidth = (int)m_rect.sizeDelta.x/compressTimes;
        TextureHeight = (int)m_rect.sizeDelta.y/compressTimes;
        m_texture = new Texture2D(TextureWidth, TextureHeight);
        offset = new Vector3();
        trackingState = new List<KinectInterop.TrackingState>();
        spinePos = new Vector3(m_texture.width/2,m_texture.height/2);
        
    }
    private void OnPostRender()
    {
        
    }
    private void OnGUI()
    {

        if (actionPos.Count>0)
        {
            if (trackingState[0]== KinectInterop.TrackingState.NotTracked)
            {//如果spinebase没有别检测到，则不画出骨骼图
                return;
            }
            ClearTexture();
            DrawJoints();
            DrawBones();
            Rect rect = new Rect(m_rect.position.x,Screen.height- m_rect.position.y, m_rect.sizeDelta.x, -m_rect.sizeDelta.y);
            GUI.DrawTexture(rect, m_texture);

        }
    }
    void ClearTexture()
    {
        m_texture = new Texture2D(TextureWidth, TextureHeight);
    }
    private void DrawJoints()
    {
        for (int i = 0; i < KinectManager.Instance.GetJointCount(); i++)
        {

            KinectInterop.TrackingState ts = trackingState[i];
            Color color = (ts == KinectInterop.TrackingState.Tracked) ? Color.red :
                (ts == KinectInterop.TrackingState.Inferred) ? Color.yellow : Color.blue;
            if (ts != KinectInterop.TrackingState.NotTracked
                                                            && i != 7
                                                            && i != 21
                                                            && i != 22
                                                            && i != 11
                                                            && i != 23
                                                            && i != 24//手部关节点不检测，也不绘出
                                                               )
            {
                Vector3 v3 = TransToScreenPos(actionPos[i]);
                if (i == 0)
                {//spinebase
                    offset = spinePos - v3;
                }
                DrawTextureNode(v3 + offset, EdgeLength, color);
            }

        }
        m_texture.Apply();

    }
    void DrawTextureNode(Vector3 centerPos,int edgeLength,Color color)
    {
        for (int i=0;i<edgeLength;i++)
        {
            for (int j=0;j<edgeLength;j++)
            {
                m_texture.SetPixel((int)centerPos.x - edgeLength / 2+i, (int)centerPos.y - edgeLength / 2+j, color);
            }
        }
    }
    void DrawTextureVericalLine(Vector3 startPos, int Length, Color color)
    {
        for (int i = 0; i < Length; i++)
        {
            m_texture.SetPixel((int)startPos.x , (int)startPos.y+i, color);
        }
    }
    void DrawTextureHorizontalLine(Vector3 startPos, int Length, Color color)
    {
        for (int i = 0; i < Length; i++)
        {
            m_texture.SetPixel((int)startPos.x+i, (int)startPos.y, color);

        }
    }
    Vector2 TransToScreenPos(Vector3 pos)
    {
        Vector3 posJoint = pos;
        Vector2 posDepth = KinectManager.Instance.MapSpacePointToDepthCoords(posJoint);
        posDepth = posDepth *scale;
        return posDepth; 
        //ushort depthValue = KinectManager.Instance.GetDepthForPixel((int)posDepth.x, (int)posDepth.y);
        //if (depthValue > 0)
        //{
        //    Vector2 posColor = KinectManager.Instance.MapDepthPointToColorCoords(posDepth, depthValue);
        //    float xNorm = (float)posColor.x / KinectManager.Instance.GetColorImageWidth();
        //    float yNorm = (float)posColor.y / KinectManager.Instance.GetColorImageWidth();
        //    Vector3 v3 = Camera.main.ViewportToScreenPoint(new Vector3(xNorm, yNorm, 0));
        //    float times = Screen.width / TextureWidth / scale;
        //    v3 = v3 / times;
        //    return v3;
        //}
        //else return Vector3.zero;
    }
    private void DrawLine(float x1, float y1, float x2, float y2)
    {
        if (x2 < x1)
        {
            DrawLine(x2, y2, x1, y1);
            return;
        }
        float k = (y2 - y1) / (x2 - x1);
        if (Mathf.Abs(k) > 1.0)
        {
            if (y1 < y2)
            {
                for (int y = (int)y1; y <= y2; y++)
                {
                    int x = (int)(x1 - LineRadius / 2 + (y - (int)y1) / k);
                    DrawTextureHorizontalLine(new Vector3(x, y, 0)+offset, LineRadius, lineColor);
                }
            }
            else
            {
                for (int y = (int)y2; y <= y1; y++)
                {
                    int x = (int)(x2 - LineRadius / 2 + (y - (int)y2) / k);
                    DrawTextureHorizontalLine(new Vector3(x, y, 0)+offset, LineRadius, lineColor);
                }
            }
            return;
        }
        for (int x = (int)x1; x <= x2; x++)
        {
            int y = (int)(y1 + LineRadius / 2 + (x - (int)x1) * k);
            DrawTextureVericalLine(new Vector3(x, y, 0)+offset, LineRadius, lineColor);
        }
    }
    private void DrawBones()
    {
        DrawBoneBetweenJoint(KinectInterop.JointType.Head, KinectInterop.JointType.Neck);
        DrawBoneBetweenJoint(KinectInterop.JointType.Neck, KinectInterop.JointType.SpineShoulder);
        DrawBoneBetweenJoint(KinectInterop.JointType.SpineShoulder, KinectInterop.JointType.ShoulderRight);
        DrawBoneBetweenJoint(KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.ElbowRight);
        DrawBoneBetweenJoint(KinectInterop.JointType.ElbowRight, KinectInterop.JointType.WristRight);
        //DrawBoneBetweenJoint(KinectInterop.JointType.WristRight, KinectInterop.JointType.HandRight);
        //DrawBoneBetweenJoint(KinectInterop.JointType.HandRight, KinectInterop.JointType.ThumbRight);
        //DrawBoneBetweenJoint(KinectInterop.JointType.HandRight, KinectInterop.JointType.HandTipRight);
        DrawBoneBetweenJoint(KinectInterop.JointType.SpineShoulder, KinectInterop.JointType.ShoulderLeft);
        DrawBoneBetweenJoint(KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.ElbowLeft);
        DrawBoneBetweenJoint(KinectInterop.JointType.ElbowLeft, KinectInterop.JointType.WristLeft);
        //DrawBoneBetweenJoint(KinectInterop.JointType.WristLeft, KinectInterop.JointType.HandLeft);
        //DrawBoneBetweenJoint(KinectInterop.JointType.HandLeft, KinectInterop.JointType.ThumbLeft);
        //DrawBoneBetweenJoint(KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandTipLeft);
        DrawBoneBetweenJoint(KinectInterop.JointType.SpineShoulder, KinectInterop.JointType.SpineMid);
        DrawBoneBetweenJoint(KinectInterop.JointType.SpineMid, KinectInterop.JointType.SpineBase);
        DrawBoneBetweenJoint(KinectInterop.JointType.SpineBase, KinectInterop.JointType.HipRight);
        DrawBoneBetweenJoint(KinectInterop.JointType.HipRight, KinectInterop.JointType.KneeRight);
        DrawBoneBetweenJoint(KinectInterop.JointType.KneeRight, KinectInterop.JointType.AnkleRight);
        DrawBoneBetweenJoint(KinectInterop.JointType.AnkleRight, KinectInterop.JointType.FootRight);
        DrawBoneBetweenJoint(KinectInterop.JointType.SpineBase, KinectInterop.JointType.HipLeft);
        DrawBoneBetweenJoint(KinectInterop.JointType.HipLeft, KinectInterop.JointType.KneeLeft);
        DrawBoneBetweenJoint(KinectInterop.JointType.KneeLeft, KinectInterop.JointType.AnkleLeft);
        DrawBoneBetweenJoint(KinectInterop.JointType.AnkleLeft, KinectInterop.JointType.FootLeft);
        m_texture.Apply();
    }
    private void DrawBoneBetweenJoint(KinectInterop.JointType jointType1, KinectInterop.JointType jointType2)
    {
        int joint1 = KinectManager.Instance.GetJointIndex(jointType1);
        int joint2 = KinectManager.Instance.GetJointIndex(jointType2);
        if (trackingState[joint1]==KinectInterop.TrackingState.Tracked&& trackingState[joint2] == KinectInterop.TrackingState.Tracked)
        {
            Vector3 v1 = TransToScreenPos(actionPos[joint1]);
            Vector3 v2 = TransToScreenPos(actionPos[joint2]);
            DrawLine(v1.x, v1.y, v2.x, v2.y);
        }
        
    }
}
