/* ============================================================================== 
* ClassName：AvatarCaculator(改自原GravityCenter类) 
* Author：ChenShuwei 
* CreateDate：2019/11/15 14:44:35 
* Version: 1.0
* ==============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GravityCenterSegment
{ //一个单独肢体的信息

    public string name;

    [Tooltip("肢体远端定标点位置。")]
    public HumanBodyBones boneD;

    [Tooltip("肢体近端定标点位置。")]
    public HumanBodyBones boneP;

    [Tooltip("节段重心的位置百分比。")]
    [Range(0f, 1f)]
    public float com = 0.5f;

    [Tooltip("身体节段重量占全身重量的百分比。")]
    [Range(0f, 1f)]
    public float mi;

    [Tooltip("Customizable segment weight. Always set to 1 if unsure.")]
    [Range(0f, 1f)]
    public float weight = 1f;

}

[System.Serializable]
public class GravityCenterProfile
{
    public string name;
    public List<GravityCenterSegment> segments; //所有肢体的信息
}

[RequireComponent(typeof(Animator))]
public class AvatarCaculator : MonoBehaviour
{
    private Animator _Animator;
    public List<GravityCenterProfile> profiles;
    public int usingProfile = 0;
    public bool visualize = true;
    public bool fakeGravityCenter = false;

    //public bool IsMale = true;

    // mass of each segment / total mass
    private static float[] MaleMi = 
        {
        0.0862f,    //Head
        0.1682f,    //upperbody
        0.2723f,    //lowerbody
        0.1419f,    //thigh
        0.0367f,    //shank
        0.0243f,    //arm
        0.0125f,    //forearm
        0.0064f,    //hand
        0.0148f     //foot
    };

    private static float[] FealeMi =
        {
        0.0820f,    //Head
        0.1635f,    //upperbody
        0.2748f,    //lowerbody
        0.1410f,    //thigh
        0.0443f,    //shank
        0.0266f,    //arm
        0.0114f,    //forearm
        0.0042f,    //hand
        0.0124f     //foot
    };

    // distance from gravity center to upperpoint / whole length
    private static float[] MaleLCS =
       {
        0.469f,    //Head
        0.536f,    //upperbody
        0.403f,    //lowerbody
        0.453f,    //thigh
        0.393f,    //shank
        0.478f,    //arm
        0.424f,    //forearm
        0.366f,    //hand
        0.438f     //foot
    };

    private static float[] FealeLCS =
       {
        0.473f,    //Head
        0.493f,    //upperbody
        0.446f,    //lowerbody
        0.442f,    //thigh
        0.425f,    //shank
        0.467f,    //arm
        0.453f,    //forearm
        0.349f,    //hand
        0.445f     //foot
    };

    void OnDrawGizmos()
    {//物体被选中，空闲等一些状态调用
        if (visualize)
        {
            Vector3 point = GetGravityCenter();
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(point, 0.1f);     //在重心位置画球
        }
    }

    private void Start()
    {
        this._Animator = GetComponent<Animator>(); ;
    }


    public Vector3 GetGravityCenter()       //return Gravity Center
    {


        if (fakeGravityCenter)
        {
            return _Animator.GetBoneTransform(HumanBodyBones.Neck).position; //取脖子位置 
        }



        if (usingProfile > profiles.Count)
        {        //如果没有一套肢体信息
            return Vector3.zero;
        }

        GravityCenterProfile profile = profiles[usingProfile];
        Vector3 com = Vector3.zero;
        foreach (GravityCenterSegment segment in profile.segments)
        {

            Transform transformD = _Animator.GetBoneTransform(segment.boneD);
            Transform transformP = _Animator.GetBoneTransform(segment.boneP);

            com += (((transformD.position - transformP.position) * segment.com + transformP.position) * segment.mi * segment.weight);
        }
        com /= profile.segments.Count;

        return com;
    }

    // 新增节段法计算人体重心
    //Calculate Gravity Center using Avatar
    public Vector3 CalculateGravityCenter(bool IsMale) 
    {

        Vector3 HeadTop;    //can't get headTop from avatar
        Vector3 Head = _Animator.GetBoneTransform(HumanBodyBones.Head).position;
        Vector3 Neck= _Animator.GetBoneTransform(HumanBodyBones.Neck).position;
        Vector3 Chest = _Animator.GetBoneTransform(HumanBodyBones.Chest).position;
        Vector3 RightUpperArm = _Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
        Vector3 LeftUpperArm = _Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
        Vector3 RightLowerArm = _Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position;
        Vector3 LeftLowerArm = _Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position;
        Vector3 RightHand = _Animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        Vector3 LeftHand = _Animator.GetBoneTransform(HumanBodyBones.LeftHand).position;
        Vector3 RightMiddleDistal = _Animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).position;
        Vector3 LeftMiddleDistal = _Animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).position;
        Vector3 Hips = _Animator.GetBoneTransform(HumanBodyBones.Hips).position; //hips
        Vector3 RightUpperLeg = _Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
        Vector3 LeftUpperLeg = _Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
        Vector3 RightLowerLeg = _Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
        Vector3 LeftLowerLeg = _Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position;
        Vector3 RightFoot = _Animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
        Vector3 LeftFoot = _Animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        Vector3 RightToes = _Animator.GetBoneTransform(HumanBodyBones.RightToes).position;
        Vector3 LeftToes = _Animator.GetBoneTransform(HumanBodyBones.LeftToes).position;


        float[] Mi;
        float[] LCS;
        Vector3 com = Vector3.zero;
        if (IsMale)
        {
            Mi = MaleMi;
            LCS = MaleLCS;

        }
        else
        {
            Mi = FealeMi;
            LCS = FealeLCS;
        }

        com = Head * Mi[0]  //head
            + Center(Neck, Chest, LCS[1]) * Mi[1]   //upper body
            + Center(Chest, Hips, LCS[2]) * Mi[2]   //Lower body
            + Center(LeftUpperLeg, LeftLowerLeg, LCS[3]) * Mi[3]    //left thigh
            + Center(RightUpperLeg, RightLowerLeg, LCS[3]) * Mi[3]    //right thigh
            + Center(LeftLowerLeg, LeftFoot, LCS[4]) * Mi[4]    //left shank
            + Center(RightLowerLeg, RightFoot, LCS[4]) * Mi[4]    //right shank
            + Center(LeftUpperArm, LeftLowerArm, LCS[5]) * Mi[5]    //left arm
            + Center(RightUpperArm, RightLowerArm, LCS[5]) * Mi[5]    //right arm
            + Center(LeftLowerArm, LeftHand, LCS[6]) * Mi[6]    //left forearm
            + Center(RightLowerArm, RightHand, LCS[6]) * Mi[6]    //right aforerm
            + Center(LeftHand, LeftMiddleDistal, LCS[7]) * Mi[7]    //left hand
            + Center(RightHand, RightMiddleDistal, LCS[7]) * Mi[7]    //right hand
            + Center(LeftFoot, LeftToes, LCS[8]) * Mi[8]    //left foot
            + Center(RightFoot, RightToes, LCS[8]) * Mi[8];   //right foot


        return com;
    }


    // return gravity center of segment
    private Vector3 Center(Vector3 up, Vector3 low,float LCS)
    {

        return up - LCS * (up - low);


    }

    // caculator LeftArmAngle
    public float LeftArmAngle()
    {

        Vector3 LeftArmVector = _Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position -
                            _Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
        Vector3 BodyVector= _Animator.GetBoneTransform(HumanBodyBones.Hips).position-
                            _Animator.GetBoneTransform(HumanBodyBones.Chest).position;

        float Angle = Vector3.SignedAngle(BodyVector, LeftArmVector, new Vector3(1, 0, 0));
        if (Angle < 0)
        {
            Angle += 360;
        }
        return Angle;
    }

    public float RightArmAngle()
    {

        Vector3 RightArmVector = _Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position -
                            _Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
        Vector3 BodyVector = _Animator.GetBoneTransform(HumanBodyBones.Hips).position -
                            _Animator.GetBoneTransform(HumanBodyBones.Chest).position;

        float Angle = Vector3.SignedAngle(RightArmVector, BodyVector, new Vector3(1, 0, 0));
        if (Angle < 0)
        {
            Angle += 360;
        }
        return Angle;
    }

    public float LeftLegAngle()
    {

        Vector3 LeftLegVector = _Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position -
                            _Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
        Vector3 BodyVector = _Animator.GetBoneTransform(HumanBodyBones.Hips).position -
                            _Animator.GetBoneTransform(HumanBodyBones.Chest).position;

        float Angle = Vector3.SignedAngle(BodyVector, LeftLegVector, new Vector3(1, 0, 0));
        if (Angle < 0)
        {
            Angle += 360;
        }
        return Angle;
    }

    public float RightLegAngle()
    {

        Vector3 RightLegVector = _Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position -
                            _Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
        Vector3 BodyVector = _Animator.GetBoneTransform(HumanBodyBones.Hips).position -
                            _Animator.GetBoneTransform(HumanBodyBones.Chest).position;

        float Angle = Vector3.SignedAngle(RightLegVector, BodyVector, new Vector3(1, 0, 0));
        if (Angle < 0)
        {
            Angle += 360;
        }
        return Angle;
    }
}
