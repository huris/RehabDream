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
    // 在kinect坐标系下，人面对Kinect时，人体的前方向、右方向、上方向
    public static Vector3 ManForward = new Vector3(0, 0, -1);
    public static Vector3 ManRight = new Vector3(-1, 0, 0);
    public static Vector3 ManUp = new Vector3(0, 1, 0);

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
        print("Get animator");
    }



    // 已废弃
    public Vector3 GetGravityCenter()       //return Gravity Center
    {

        //if (fakeGravityCenter)
        //{
        //    return _Animator.GetBoneTransform(HumanBodyBones.Neck).position; //取脖子位置 
        //}



        //if (usingProfile > profiles.Count)
        //{        //如果没有一套肢体信息
        //    return Vector3.zero;
        //}

        //GravityCenterProfile profile = profiles[usingProfile];
        //Vector3 com = Vector3.zero;
        //foreach (GravityCenterSegment segment in profile.segments)
        //{

        //    Transform transformD = _Animator.GetBoneTransform(segment.boneD);
        //    Transform transformP = _Animator.GetBoneTransform(segment.boneP);

        //    com += (((transformD.position - transformP.position) * segment.com + transformP.position) * segment.mi * segment.weight);
        //}
        //com /= profile.segments.Count;

        //return com;

        return Vector3.zero;
    }

    // 新增节段法计算人体重心
    //Calculate Gravity Center using Avatar
    public Vector3 CalculateGravityCenter(bool IsMale)
    {

        Vector3 HeadTop;    //can't get headTop from avatar
        Vector3 Head = _Animator.GetBoneTransform(HumanBodyBones.Head).position;
        Vector3 Neck = _Animator.GetBoneTransform(HumanBodyBones.Neck).position;
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
    private Vector3 Center(Vector3 up, Vector3 low, float LCS)
    {

        return up - LCS * (up - low);


    }

    // 肩关节外展（内收）
    public float LeftArmAbductionAngle()
    {
        // 人体胳膊向量
        Vector3 LeftArmVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position -
                                //_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
                            GetKinectJointPosition(KinectInterop.JointType.ElbowLeft) -
                            GetKinectJointPosition(KinectInterop.JointType.ShoulderLeft);

        // 人体冠状面法向量（向前）
        Vector3 CoronalNormal = GetCoronalNormal();

        // 投影到冠状面
        Vector3 LeftArmVectorP = Vector3.ProjectOnPlane(LeftArmVector, CoronalNormal);


        Vector3 BodyVector = //_Animator.GetBoneTransform(HumanBodyBones.Hips).position -
                             //_Animator.GetBoneTransform(HumanBodyBones.Chest).position;
                            GetKinectJointPosition(KinectInterop.JointType.SpineMid) -
                            GetKinectJointPosition(KinectInterop.JointType.SpineShoulder);

        if (Vector3.Dot(Vector3.Cross(LeftArmVectorP, BodyVector), CoronalNormal) > 0)
        {
            // 外展
            return CaculateAngle(BodyVector, LeftArmVectorP, 180);
        }
        else
        {
            // 内收
            return -1f * CaculateAngle(BodyVector, LeftArmVectorP, 180);
        }

    }

    public float RightArmAbductionAngle()
    {

        // 人体胳膊向量
        Vector3 RightArmVector = //_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position -
                                 //_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
                            GetKinectJointPosition(KinectInterop.JointType.ElbowRight) -
                            GetKinectJointPosition(KinectInterop.JointType.ShoulderRight);

        // 人体冠状面法向量（向前）
        Vector3 CoronalNormal = GetCoronalNormal();

        // 投影到冠状面
        Vector3 RightArmVectorP = Vector3.ProjectOnPlane(RightArmVector, CoronalNormal);


        Vector3 BodyVector = //_Animator.GetBoneTransform(HumanBodyBones.Hips).position -
                             //_Animator.GetBoneTransform(HumanBodyBones.Chest).position;
                            GetKinectJointPosition(KinectInterop.JointType.SpineMid) -
                            GetKinectJointPosition(KinectInterop.JointType.SpineShoulder);

        if (Vector3.Dot(Vector3.Cross(RightArmVectorP, BodyVector), CoronalNormal) < 0)
        {
            // 外展
            return CaculateAngle(BodyVector, RightArmVectorP, 180);
        }
        else
        {
            // 内收
            return -1f * CaculateAngle(BodyVector, RightArmVectorP, 180);
        }

    }

    // 左肩关节前屈（后伸）
    public float LeftArmFlexionAngle()
    {

        // 人体胳膊向量
        Vector3 LeftArmVector = //_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position -
                                //_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
                            GetKinectJointPosition(KinectInterop.JointType.ElbowLeft) -
                            GetKinectJointPosition(KinectInterop.JointType.ShoulderLeft);

        // 人体矢状面法向量（向左）
        Vector3 SagittalNormal = GetKinectJointPosition(KinectInterop.JointType.ShoulderLeft) - GetKinectJointPosition(KinectInterop.JointType.ShoulderRight);

        // 人体冠状面法向量（向前）
        Vector3 CoronalNormal = GetCoronalNormal();

        // 将胳膊向量投影到矢状面
        Vector3 LeftArmVectorP = Vector3.ProjectOnPlane(LeftArmVector, SagittalNormal);

        Vector3 BodyDownVector = GetKinectJointPosition(KinectInterop.JointType.SpineMid) - GetKinectJointPosition(KinectInterop.JointType.SpineShoulder);


        if (Vector3.Dot(LeftArmVectorP, CoronalNormal) > 0)
        {
            // 前屈
            return CaculateAngle(BodyDownVector, LeftArmVectorP, 180);
        }
        else
        {
            // 后伸
            return -1f * CaculateAngle(BodyDownVector, LeftArmVectorP, 180);
        }

    }


    // 右肩关节前屈（后伸）
    public float RightFlexionAngle()
    {

        // 人体胳膊向量
        Vector3 RightArmVector = //_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position -
                                 //_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
                            GetKinectJointPosition(KinectInterop.JointType.ElbowRight) -
                            GetKinectJointPosition(KinectInterop.JointType.ShoulderRight);

        // 人体矢状面法向量（向右）
        Vector3 SagittalNormal = GetKinectJointPosition(KinectInterop.JointType.ShoulderRight) - GetKinectJointPosition(KinectInterop.JointType.ShoulderLeft);

        // 人体冠状面法向量（向前）
        Vector3 CoronalNormal = GetCoronalNormal();

        // 将胳膊向量投影到矢状面
        Vector3 RightArmVectorP = Vector3.ProjectOnPlane(RightArmVector, SagittalNormal);

        Vector3 BodyDownVector = GetKinectJointPosition(KinectInterop.JointType.SpineMid) - GetKinectJointPosition(KinectInterop.JointType.SpineShoulder);


        if (Vector3.Dot(RightArmVectorP, CoronalNormal) > 0)
        {
            // 前屈
            return CaculateAngle(BodyDownVector, RightArmVectorP, 180);
        }
        else
        {
            // 后伸
            return -1f * CaculateAngle(BodyDownVector, RightArmVectorP, 180);
        }

    }

    // 左肘屈曲
    public float LeftElbowFlexionAngle()
    {
        Vector3 LeftForearmVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftHand).position -
                                    //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position;
                                    GetKinectJointPosition(KinectInterop.JointType.WristLeft) -
                                   GetKinectJointPosition(KinectInterop.JointType.ElbowLeft);

        Vector3 LeftArmVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position -
                                //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position;
                                GetKinectJointPosition(KinectInterop.JointType.ShoulderLeft) -
                               GetKinectJointPosition(KinectInterop.JointType.ElbowLeft);

        return CaculateAngle(LeftForearmVector, LeftArmVector, 180);

    }

    // 右肘屈曲
    public float RightElbowFlexionAngle()
    {
        Vector3 RightForearmVector = //_Animator.GetBoneTransform(HumanBodyBones.RightHand).position -
                                     //_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position;
                                    GetKinectJointPosition(KinectInterop.JointType.WristRight) -
                                   GetKinectJointPosition(KinectInterop.JointType.ElbowRight);

        Vector3 RightArmVector = //_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position -
                                 //_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position;
                                GetKinectJointPosition(KinectInterop.JointType.ShoulderRight) -
                               GetKinectJointPosition(KinectInterop.JointType.ElbowRight);

        return CaculateAngle(RightArmVector, RightForearmVector, 180);
    }

    // 左膝屈曲
    public float LeftKneeFlexionAngle()
    {
        Vector3 LeftLegVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position -
                                //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position;
                                GetKinectJointPosition(KinectInterop.JointType.HipLeft) -
                               GetKinectJointPosition(KinectInterop.JointType.KneeLeft);

        Vector3 LeftShankVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftFoot).position -
                                  //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position;
                                GetKinectJointPosition(KinectInterop.JointType.AnkleLeft) -
                               GetKinectJointPosition(KinectInterop.JointType.KneeLeft);

        return CaculateAngle(LeftLegVector, LeftShankVector, 180);
    }

    // 右膝屈曲
    public float RightKneeFlexionAngle()
    {
        Vector3 RightLegVector = //_Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position -
                                 //_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
                                GetKinectJointPosition(KinectInterop.JointType.HipRight) -
                               GetKinectJointPosition(KinectInterop.JointType.KneeRight);

        Vector3 RightShankVector = //_Animator.GetBoneTransform(HumanBodyBones.RightFoot).position -
                                   //_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
                                 GetKinectJointPosition(KinectInterop.JointType.AnkleRight) -
                               GetKinectJointPosition(KinectInterop.JointType.KneeRight);

        return CaculateAngle(RightLegVector, RightShankVector, 180);
    }

    // 左踝背屈（90-踝关节角度），或跖屈（踝关节角度-90）
    public float LeftAnkleAngle()
    {
        Vector3 LeftShankVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position -
                                  //_Animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                                GetKinectJointPosition(KinectInterop.JointType.KneeLeft) -
                               GetKinectJointPosition(KinectInterop.JointType.AnkleLeft);

        Vector3 LeftFootVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftToes).position -
                                 //_Animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                                GetKinectJointPosition(KinectInterop.JointType.FootLeft) -
                               GetKinectJointPosition(KinectInterop.JointType.AnkleLeft);

        return 90 - CaculateAngle(LeftShankVector, LeftFootVector, 180);
    }

    // 右踝关节角度
    public float RightAnkleAngle()
    {
        Vector3 RightShankVector = //_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position -
                                   //_Animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
                                GetKinectJointPosition(KinectInterop.JointType.KneeRight) -
                               GetKinectJointPosition(KinectInterop.JointType.AnkleRight);

        Vector3 RightFootVector = //_Animator.GetBoneTransform(HumanBodyBones.RightToes).position -
                                  //_Animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
                                GetKinectJointPosition(KinectInterop.JointType.FootRight) -
                               GetKinectJointPosition(KinectInterop.JointType.AnkleRight);

        return 90 - CaculateAngle(RightShankVector, RightFootVector, 180);
    }

    // 左髋外展
    public float LeftHipAngle()
    {

        // 人体大腿向量
        Vector3 LeftLegVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position -
                                //_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
                            GetKinectJointPosition(KinectInterop.JointType.HipLeft) -
                            GetKinectJointPosition(KinectInterop.JointType.KneeLeft);

        // 人体冠状面法向量（向前）
        Vector3 CoronalNormal = GetCoronalNormal();

        // 投影到冠状面
        Vector3 LeftLegVectorP = Vector3.ProjectOnPlane(LeftLegVector, CoronalNormal);


        Vector3 BodyVector = //_Animator.GetBoneTransform(HumanBodyBones.Hips).position -
                             //_Animator.GetBoneTransform(HumanBodyBones.Chest).position;
                            GetKinectJointPosition(KinectInterop.JointType.SpineBase) -
                            GetKinectJointPosition(KinectInterop.JointType.SpineShoulder);

        if (Vector3.Dot(Vector3.Cross(LeftLegVectorP, BodyVector), CoronalNormal) > 0)
        {
            // 外展
            return CaculateAngle(BodyVector, LeftLegVectorP, 180);
        }
        else
        {
            // 内收
            return CaculateAngle(BodyVector, LeftLegVectorP, 180);
        }

    }



    // 右髋外展
    public float RightHipAngle()
    {
        // 人体大腿向量
        Vector3 RightLegVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position -
                                //_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
                            GetKinectJointPosition(KinectInterop.JointType.HipRight) -
                            GetKinectJointPosition(KinectInterop.JointType.KneeRight);

        // 人体冠状面法向量（向前）
        Vector3 CoronalNormal = GetCoronalNormal();

        // 投影到冠状面
        Vector3 RightLegVectorP = Vector3.ProjectOnPlane(RightLegVector, CoronalNormal);


        Vector3 BodyVector = //_Animator.GetBoneTransform(HumanBodyBones.Hips).position -
                             //_Animator.GetBoneTransform(HumanBodyBones.Chest).position;
                            GetKinectJointPosition(KinectInterop.JointType.SpineBase) -
                            GetKinectJointPosition(KinectInterop.JointType.SpineShoulder);

        if (Vector3.Dot(Vector3.Cross(RightLegVectorP, BodyVector), CoronalNormal) < 0)
        {
            // 外展
            return CaculateAngle(BodyVector, RightLegVectorP, 180);
        }
        else
        {
            // 内收
            return CaculateAngle(BodyVector, RightLegVectorP, 180);
        }

    }


    // 胯部夹角
    public float HipAngle()
    {
        Vector3 RightLegVector = //_Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position -
                                 //_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
                                GetKinectJointPosition(KinectInterop.JointType.HipRight) -
                               GetKinectJointPosition(KinectInterop.JointType.KneeRight);

        Vector3 LeftLegVector = //_Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position -
                                //_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position;
                                GetKinectJointPosition(KinectInterop.JointType.HipLeft) -
                               GetKinectJointPosition(KinectInterop.JointType.KneeLeft);

        return CaculateAngle(LeftLegVector, RightLegVector, 180);
    }


    // 身体左倾夹角
    public float LeftSideAngle()
    {
        Vector3 SpineVector = _Animator.GetBoneTransform(HumanBodyBones.Spine).position -
                           _Animator.GetBoneTransform(HumanBodyBones.Hips).position;

        Vector3 HipLeftVector = _Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position -
                          _Animator.GetBoneTransform(HumanBodyBones.Hips).position;

        return CaculateAngle(SpineVector, HipLeftVector, 180);
    }


    // 身体右倾夹角
    public float RightSideAngle()
    {
        Vector3 SpineVector = _Animator.GetBoneTransform(HumanBodyBones.Spine).position -
                           _Animator.GetBoneTransform(HumanBodyBones.Hips).position;

        Vector3 HipRightVector = _Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position -
                          _Animator.GetBoneTransform(HumanBodyBones.Hips).position;

        return CaculateAngle(SpineVector, HipRightVector, 180);
    }


    // 身体左倾夹角
    public float UponSideAngle()
    {
        Vector3 SpineVector = _Animator.GetBoneTransform(HumanBodyBones.Spine).position -
                           _Animator.GetBoneTransform(HumanBodyBones.Hips).position;

        Vector3 Up = new Vector3(0, 1, 0);

        return CaculateAngle(SpineVector, Up, 180);
    }

    // 身体左倾夹角
    public float DownSideAngle()
    {
        Vector3 SpineVector = _Animator.GetBoneTransform(HumanBodyBones.Spine).position -
                           _Animator.GetBoneTransform(HumanBodyBones.Hips).position;

        Vector3 Down = new Vector3(0, -1, 0);

        return CaculateAngle(SpineVector, Down, 180);
    }


    //// 将动画骨骼 转换成 Kinect关节
    //public KinectInterop.JointType AnimatorBones2KinectJoints(HumanBodyBones Bone)
    //{

    //}


    // 获取对应的kinect关节点坐标
    public static Vector3 GetKinectJointPosition(KinectInterop.JointType joint)
    {
        return KinectManager.Instance.GetJointKinectPosition(KinectManager.Instance.GetPrimaryUserID(), (int)joint);
    }


    // Unity为左手系，故叉乘满足左手法则，以axis方向为叉乘后的正方向 
    public static float CaculateAngle(Vector3 from, Vector3 to, float range = 180)
    {

        float Angle = Vector3.Angle(from, to);
        if (Angle > range)
        {
            return 360 - Angle;
        }
        else
        {
            return Angle;
        }
    }

    // the point nearest to soccerball
    public HumanBodyBones NearestPoint(Vector3 SoccerballPosition)
    {
        HumanBodyBones[] array = new HumanBodyBones[]{
            HumanBodyBones.RightHand,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightShoulder,

            HumanBodyBones.LeftHand,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftShoulder,

            HumanBodyBones.RightToes,
            HumanBodyBones.RightFoot,
            HumanBodyBones.RightLowerLeg,
            HumanBodyBones.RightUpperLeg,

            HumanBodyBones.LeftToes,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.LeftUpperLeg
        };

        HumanBodyBones Nearest = HumanBodyBones.RightHand;
        float disance = (_Animator.GetBoneTransform(Nearest).position - SoccerballPosition).sqrMagnitude;
        foreach (HumanBodyBones point in array)
        {
            Vector3 position = _Animator.GetBoneTransform(point).position;
            float disance_now = (position - SoccerballPosition).sqrMagnitude;
            if (disance_now < disance)
            {
                disance = disance_now;
                Nearest = point;
            }
        }

        return Nearest;

    }

    // is soccerball close enough to Limb
    public bool CloseEnough(Vector3 SoccerballPosition, string Limb, float MixDis = 0.1f)
    {
        HumanBodyBones[] RightHandArray = new HumanBodyBones[]
        {
            HumanBodyBones.RightHand,
           // HumanBodyBones.RightLowerArm,
           // HumanBodyBones.RightUpperArm,
           // HumanBodyBones.RightShoulder
        };

        HumanBodyBones[] LeftHandArray = new HumanBodyBones[]
        {
            HumanBodyBones.LeftHand,
           // HumanBodyBones.LeftLowerArm,
           // HumanBodyBones.LeftUpperArm,
           // HumanBodyBones.LeftShoulder
        };

        HumanBodyBones[] RightFootArray = new HumanBodyBones[]
        {
            HumanBodyBones.RightToes,
            HumanBodyBones.RightFoot,
            HumanBodyBones.RightLowerLeg,
            HumanBodyBones.RightUpperLeg,
        };

        HumanBodyBones[] LeftFootArray = new HumanBodyBones[]{
            HumanBodyBones.LeftToes,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.LeftUpperLeg
        };

        HumanBodyBones[] Array;
        if (Limb.Equals("左手"))
        {
            Array = LeftHandArray;
        }
        else if (Limb.Equals("右手"))
        {
            Array = RightHandArray;
        }
        else if (Limb.Equals("左脚"))
        {
            Array = LeftFootArray;
        }
        else
        {
            Array = RightFootArray;
        }


        foreach (HumanBodyBones point in Array)
        {
            Vector3 position = _Animator.GetBoneTransform(point).position;
            float disance_now = (position - SoccerballPosition).sqrMagnitude;
            if (disance_now < MixDis)
            {
                return true;
            }
        }

        return false;
    }



    public string Point2Limb(HumanBodyBones Point)
    {
        if (Point == HumanBodyBones.RightHand || Point == HumanBodyBones.RightLowerArm || Point == HumanBodyBones.RightUpperArm || Point == HumanBodyBones.RightShoulder)
        {
            return "右手";
        }
        else if (Point == HumanBodyBones.LeftHand || Point == HumanBodyBones.LeftLowerArm || Point == HumanBodyBones.LeftUpperArm || Point == HumanBodyBones.LeftShoulder)
        {
            return "左手";
        }
        else if (Point == HumanBodyBones.LeftToes || Point == HumanBodyBones.LeftFoot || Point == HumanBodyBones.LeftLowerLeg || Point == HumanBodyBones.LeftUpperLeg)
        {
            return "左脚";
        }
        else
        {
            return "右脚";
        }
    }

    // position of left shoulder
    public Vector3 GetLeftShoulderPosition()
    {
        return _Animator.GetBoneTransform(HumanBodyBones.LeftShoulder).position;
    }

    // position of Right shoulder
    public Vector3 GetRightShoulderPosition()
    {
        return _Animator.GetBoneTransform(HumanBodyBones.RightShoulder).position;
    }

    // position of left upperarm
    public Vector3 GetLeftUpperArmPosition()
    {
        return _Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
    }

    // position of right upperarm
    public Vector3 GetRightUpperArmPosition()
    {
        return _Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
    }

    // position of spine
    public Vector3 GetSpinePosition()
    {
        return _Animator.GetBoneTransform(HumanBodyBones.Spine).position;
    }

    // InitDistance for evaluate
    public float InitDistance()
    {
        Vector3 temp = _Animator.GetBoneTransform(HumanBodyBones.LeftHand).position -
            _Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;

        float HandLength = temp.sqrMagnitude;

        Debug.Log(HandLength);
        return HandLength;
    }

    // 冠状面法向量
    public Vector3 GetCoronalNormal()
    {
        // 人体冠状面法向量（向前）
        Vector3 CoronalNormal = Vector3.Cross(
            GetKinectJointPosition(KinectInterop.JointType.ShoulderLeft) - GetKinectJointPosition(KinectInterop.JointType.ShoulderRight),
            GetKinectJointPosition(KinectInterop.JointType.SpineBase) - GetKinectJointPosition(KinectInterop.JointType.SpineShoulder)
            );

        return CoronalNormal;
    }

}
