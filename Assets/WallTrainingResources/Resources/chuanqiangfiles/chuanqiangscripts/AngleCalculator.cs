
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleCalculator : MonoBehaviour {
    float time;
    public List<Vector3> actionPos;
    public string actionName;
    public int actionId;
    public bool SaveToDatabse=true;
    ActionData actionData;
    bool AngleLessThan90 = false;
    // Use this for initialization
    void Start () {
        time = 0;
        actionData = new ActionData();
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    #region 以下是各关节点检测指标的实现算法
    public Dictionary<int,float> GetAllMethodAngle(List<Vector3> pos,List<int> checkJoints)
    {
        actionPos = pos;
        Dictionary<int, float> angle = new Dictionary<int, float>();
        for (int i = 0; i < checkJoints.Count; i++)
        {
            int joint = checkJoints[i];
            if (DATA.JointCheckMethod.ContainsKey(joint))
            {
                List<int> checkMethods = DATA.JointCheckMethod[joint];
                for (int j = 0; j < checkMethods.Count; j++)
                {
                    angle.Add(checkMethods[j], GetMethodAgle(checkMethods[j]));
                }
            }
        }
        string output = "动作：" + actionName+ "  各方法夹角(方法id，夹角)：";
        foreach (var item in angle)
        {
            output += "(" + item.Key + "," + item.Value + ")";
        }
        print(output);
        return angle;
    }
    public float GetMethodAgle(int methodId)
    {
        if (actionPos == null || actionPos.Count == 0) return 0;
        switch (methodId)
        {
            case 1:
                return GetSagittalRootAngle(V(20, 0), V(4, 8), V(4, 5));
                break;
            case 2:
                return GetCoronalRootAngle(V(20, 0), V(4, 8), V(4, 5));
                break;
            case 3:
                return GetVectorAngle(V(5, 6), V(20, 0));
                break;
            case 4:
                return GetVectorAngle(V(4,5), V(4,8));
                break;
            case 5:
                return GetSagittalRootAngle(V(20, 0), V(4, 8), V(8, 9));
                break;
            case 6:
                return GetCoronalRootAngle(V(20, 0), V(4, 8), V(8, 9));
                break;
            case 7:
                return GetVectorAngle(V(9, 10), V(20, 0));
                break;
            case 8:
                return GetVectorAngle(V(8, 9), V(8, 4));
                break;
            case 9:
                return GetVectorAngle(V(5,4), V(5, 6));
                break;
            case 10:
                return GetVectorAngle(V(9,8), V(9, 10));
                break;
            case 11:
                return GetSagittalRootAngle(V(20, 0), V(12, 16), V(12, 13));
                break;
            case 12:
                return GetCoronalRootAngle(V(20, 0), V(12, 16), V(12, 13));
                break;
            case 13:
                return GetVectorAngle(V(13,14), V(12,16));
                break;
            case 14:
                return GetSagittalRootAngle(V(20, 0), V(12, 16), V(16, 17));
                break;
            case 15:
                return GetCoronalRootAngle(V(20, 0), V(12, 16), V(16, 17));
                break;
            case 16:
                return GetVectorAngle(V(17,18), V(16,12));
                break;
            case 17:
                return GetVectorAngle(V(13,12), V(13, 14));
                break;
            case 18:
                return GetVectorAngle(V(17,16), V(17, 18));
                break;
            case 19:
                return GetVectorAngle(V(14, 15), V(14, 13));
                break;
            case 20:
                return GetVectorAngle(V(14, 15), V(12, 16));
                break;
            case 21:
                return GetVectorAngle(V(18, 19), V(18, 17));
                break;
            case 22:
                return GetVectorAngle(V(18,19), V(16,12));
                break;
            case 23:
                return GetSagittalVerticalAngle(V(20, 0), V(4, 8), V(2, 3));
                break;
            case 24:
                return GetCoronalVerticalAngle(V(20, 0), V(4, 8), V(2, 3));
                break;
            default:
                return 0;

        }
        return 0;
    }
    Vector3 V(int startpoint,int endpoint)
    {
        return actionPos[endpoint] - actionPos[startpoint];
    }
    /// <summary>
    /// 计算向量与冠状面内基向量的夹角
    /// </summary>
    /// <param name="rootVec1" 构成冠状面的基向量1
    /// <param name="rootVec2" 构成冠状面的基向量2
    /// <param name="Vec3" 待计算的向量
    /// <returns></returns>
    float GetCoronalRootAngle(Vector3 rootVec1, Vector3 rootVec2,Vector3 Vec3)
    {
        Vector3 verticalVec= rootVec2;
        Vector3 tmpVec=new Vector3();
        Vector3.OrthoNormalize(ref rootVec1, ref verticalVec,ref tmpVec);
        Vector3 projection = Vector3.ProjectOnPlane(Vec3, tmpVec);
        float angle= Vector3.Angle(projection, rootVec1);
        angle = (float)Math.Round(angle, 2);
        if (AngleLessThan90 && angle > 90) angle = 180 - angle; 
        return angle;
    }
    /// <summary>
    /// 计算向量与矢状面内基向量的夹角
    /// </summary>
    /// <param name="rootVec1" 构成矢状面的基向量1
    /// <param name="rootVec2" 构成矢状面的基向量2
    /// <param name="Vec3" 待计算的向量
    /// <returns></returns>
    float GetSagittalRootAngle(Vector3 rootVec1, Vector3 rootVec2, Vector3 Vec3)
    {
        Vector3 verticalVec = rootVec2;
        Vector3.OrthoNormalize(ref rootVec1, ref verticalVec);
        Vector3 projection = Vector3.ProjectOnPlane(Vec3, verticalVec);
        float angle = Vector3.Angle(projection, rootVec1);
        angle = (float)Math.Round(angle, 2);
        if (AngleLessThan90 && angle > 90) angle = 180 - angle;
        return angle;
    }
    /// <summary>
    /// 直接计算两个向量夹角
    /// </summary>
    /// <param name="Vec1"></param>
    /// <param name="Vec2"></param>
    /// <returns></returns>
    float GetVectorAngle(Vector3 Vec1, Vector3 Vec2)
    {
        float angle = Vector3.Angle(Vec1, Vec2);
        angle = (float)Math.Round(angle, 2);
        if (AngleLessThan90 && angle > 90) angle = 180 - angle;
        return angle;
    }
    /// <summary>
    /// 计算向量与冠状面的垂直向量的夹角
    /// </summary>
    /// <param name="rootVec1"></param>
    /// <param name="rootVec2"></param>
    /// <param name="Vec3"></param>
    /// <returns></returns>
    float GetCoronalVerticalAngle(Vector3 rootVec1, Vector3 rootVec2, Vector3 Vec3)
    {
        Vector3 verticalVec = rootVec2;
        Vector3 tmpVec = new Vector3();
        Vector3.OrthoNormalize(ref rootVec1, ref verticalVec, ref tmpVec);
        float angle = Vector3.Angle(Vec3, tmpVec);
        angle = (float)Math.Round(angle, 2);
        if (AngleLessThan90 && angle > 90) angle = 180 - angle;
        return angle;
    }
    /// <summary>
    /// 计算向量与矢状面的垂直向量的夹角
    /// </summary>
    /// <param name="rootVec1"></param>
    /// <param name="rootVec2"></param>
    /// <param name="Vec3"></param>
    /// <returns></returns>
    float GetSagittalVerticalAngle(Vector3 rootVec1, Vector3 rootVec2, Vector3 Vec3)
    {
        Vector3 verticalVec = rootVec2;
        Vector3.OrthoNormalize(ref rootVec1, ref verticalVec);
        float angle = Vector3.Angle(Vec3, verticalVec);
        angle = (float)Math.Round(angle, 2);
        if (AngleLessThan90 && angle > 90) angle = 180 - angle;
        return angle;
    }
    ///// <summary>
    ///// 计算与横断面夹角
    ///// </summary>
    ///// <param name="rootVec1" 构成横断面的基向量1
    ///// <param name="rootVec2" 构成横断面的基向量2
    ///// <param name="Vec3" 待计算的向量
    ///// <returns></returns>
    //float GetTransverseAngle(Vector3 rootVec1, Vector3 rootVec2, Vector3 Vec3)
    //{
    //    Vector3 verticalVec = rootVec1;
    //    Vector3.OrthoNormalize(ref rootVec2, ref verticalVec);
    //    Vector3 projection = Vector3.ProjectOnPlane(Vec3, verticalVec);
    //    float angle = Vector3.Angle(projection, Vec3);
    //    angle = (float)Math.Round(angle, 2);
    //    if (AngleLessThan90 && angle > 90) angle = 180 - angle;
    //    return angle;
    //}
    ///// <summary>
    ///// 计算与冠状面夹角
    ///// </summary>
    ///// <param name="rootVec1" 构成冠状面的基向量1
    ///// <param name="rootVec2" 构成冠状面的基向量2
    ///// <param name="Vec3" 待计算的向量
    ///// <returns></returns>
    //float GetCoronalAngle(Vector3 rootVec1, Vector3 rootVec2, Vector3 Vec3)
    //{
    //    Vector3 verticalVec = rootVec2;
    //    Vector3 tmpVec = new Vector3();
    //    Vector3.OrthoNormalize(ref rootVec1, ref verticalVec,ref tmpVec);
    //    Vector3 projection = Vector3.ProjectOnPlane(Vec3, tmpVec);
    //    float angle = Vector3.Angle(Vec3, projection);
    //    angle = (float)Math.Round(angle, 2);
    //    if (AngleLessThan90 && angle > 90) angle = 180 - angle;
    //    return angle;
    //}
    ///// <summary>
    ///// 计算与矢状面夹角
    ///// </summary>
    ///// <param name="rootVec1" 构成矢状面的基向量1
    ///// <param name="rootVec2" 构成矢状面的基向量2
    ///// <param name="Vec3" 待计算的向量
    ///// <returns></returns>
    //float GetSagittalAngle(Vector3 rootVec1, Vector3 rootVec2, Vector3 Vec3)
    //{
    //    Vector3 verticalVec = rootVec2;
    //    Vector3.OrthoNormalize(ref rootVec1, ref verticalVec);
    //    Vector3 projection = Vector3.ProjectOnPlane(Vec3, verticalVec);
    //    float angle = Vector3.Angle(Vec3, projection);
    //    angle = (float)Math.Round(angle, 2);
    //    if (AngleLessThan90 && angle > 90) angle = 180 - angle;
    //    return angle;
    //}

    #endregion
    #region 两个动作的匹配检测
    /// <summary>
    /// 返回关节对应检测方法的角度差
    /// </summary>
    /// <param name="joint"></param>
    /// <param name="method"></param>
    /// <param name="current_pos"></param>
    /// <param name="standord_pos"></param>
    /// <returns></returns>
    public MethodData MethodAngle(int method,List<Vector3> position)
    {
        actionPos = position;
        float angle = GetMethodAgle(method);
        return new MethodData(angle, 0, 0);
    }
    public ActionData GetActionData(List<Vector3> position)
    {
        List<int> checkJoints = DATA.defaultMatchingCheckJoints;        //读取默认需要检查的关节点
        ActionData ad = new ActionData();
        for (int i = 0; i < checkJoints.Count; i++)     //对需要检查的关节点进行逐个检查
        {

            int joint = checkJoints[i];
            if (DATA.JointCheckMethod.ContainsKey(joint))       //该关节点是否有检测方法
            {
                JointData jointData = new JointData();
                List<int> checkMethods = DATA.JointCheckMethod[joint];  //读取该关节点对应的检测方法
                for (int j = 0; j < checkMethods.Count; j++)        //逐个判断使用对应的方法进行检测是否有效
                {

                    int method = checkMethods[j];
#region 对于特殊的检测方法（肩髋部位的屈曲和外展），需要根据夹角判定动作与检测方法的相关性，相关度过低则不检测该方法
                    if (method == 1)//左肩关节屈曲检测，和矢状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 verticalVec = V(4, 8);
                        Vector3.OrthoNormalize(ref vec1, ref verticalVec);
                        float angle = Vector3.Angle(V(4, 5), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)       //夹角过小时，该检测指标无效
                        {
                            jointData.invalidateMethods.Add(method);        //该方法检测指标无效
                        }
                    }
                    else if (method == 2)//左肩关节外展检测，和冠状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 vec2 = V(4, 8);
                        Vector3 verticalVec = new Vector3();
                        Vector3.OrthoNormalize(ref vec1, ref vec2, ref verticalVec);
                        float angle = Vector3.Angle(V(4, 5), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)
                        {
                            jointData.invalidateMethods.Add(method);
                        }
                    }
                    else if (method == 5)//右肩关节屈曲检测，和矢状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 verticalVec = V(4, 8);
                        Vector3.OrthoNormalize(ref vec1, ref verticalVec);
                        float angle = Vector3.Angle(V(8, 9), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)
                        {
                            jointData.invalidateMethods.Add(method);
                        }
                    }
                    else if (method == 6)//右肩关节外展检测，和冠状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 vec2 = V(4, 8);
                        Vector3 verticalVec = new Vector3();
                        Vector3.OrthoNormalize(ref vec1, ref vec2, ref verticalVec);
                        float angle = Vector3.Angle(V(8, 9), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)
                        {
                            jointData.invalidateMethods.Add(method);
                        }
                    }
                    else if (method == 11)//左髋关节屈曲检测，和矢状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 verticalVec = V(12, 16);
                        Vector3.OrthoNormalize(ref vec1, ref verticalVec);
                        float angle = Vector3.Angle(V(12,13), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)
                        {
                            jointData.invalidateMethods.Add(method);
                        }
                    }
                    else if (method == 12)//左髋关节外展检测，和冠状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 vec2 = V(12, 16);
                        Vector3 verticalVec = new Vector3();
                        Vector3.OrthoNormalize(ref vec1, ref vec2, ref verticalVec);
                        float angle = Vector3.Angle(V(12,13), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)
                        {
                            jointData.invalidateMethods.Add(method);
                        }
                    }
                    else if (method == 14)//右髋关节屈曲检测，和矢状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 verticalVec = V(12, 16);
                        Vector3.OrthoNormalize(ref vec1, ref verticalVec);
                        float angle = Vector3.Angle(V(16,17), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)
                        {
                            jointData.invalidateMethods.Add(method);
                        }
                    }
                    else if (method == 15)//右髋关节外展检测，和冠状面垂直向量夹角过小时，该检测指标无效，不放入字典中
                    {
                        actionPos = position;
                        Vector3 vec1 = V(20, 0);
                        Vector3 vec2 = V(12, 16);
                        Vector3 verticalVec = new Vector3();
                        Vector3.OrthoNormalize(ref vec1, ref vec2, ref verticalVec);
                        float angle = Vector3.Angle(V(16,17), verticalVec);
                        if (angle > 90) angle = 180 - angle;
                        if (angle < DATA.ShoulderInvalidateAngle)
                        {
                            jointData.invalidateMethods.Add(method);
                        }
                    }
#endregion
                    MethodData methodData = new MethodData();
                    methodData = MethodAngle(method, position);     //记录method检测方法检测出的角度、所有关节点的坐标
                    jointData.methodDatas.Add(method, methodData);      //记录对同一关节使用不同检测方法对应的methodData、方法是否有效
                }
                ad.jointDatas.Add(joint, jointData);        //记录关节-jointData对
            }

        }
        return ad;
    }
    ///// <summary>
    ///// 返回角度差超过阈值的检测方法
    ///// </summary>
    ///// <param name="dValueDic"></param>
    ///// <returns></returns>
    //public Dictionary<int, float> MethodOutOfGOODThreshold(Dictionary<int, float> dValueDic)
    //{
    //    Dictionary<int, float> OutOfGOODThresholdMethodDic = new Dictionary<int, float>();
    //    foreach (var item in dValueDic)
    //    {
    //        if (Mathf.Abs(item.Value)> DATA.AngleTresholdsByMethod[item.Key-1])//此处减一只是因为method角标从1开始， DATA.AngleTresholdsByMethod角标从0开始
    //        {
    //            OutOfGOODThresholdMethodDic.Add(item.Key, item.Value);
    //        }
    //    }
    //    string output = "所有检测方法误差（方法ID,误差值）：";
    //    foreach (var item in dValueDic)
    //    {
    //        output += "(" + item.Key + "," + item.Value + ")";
    //    }
    //    print(output);
    //    output = "超过阈值检测方法误差及对应阈值（方法ID,误差值，误差阈值）：";
    //    foreach (var item in OutOfGOODThresholdMethodDic)
    //    {
    //        output += "(" + item.Key + "," + item.Value +","+ DATA.AngleTresholdsByMethod[item.Key - 1]+ ")";
    //    }
    //    print(output);
    //    return OutOfGOODThresholdMethodDic;
    //}
    //返回角度差超过阈值的关节,值为该关节检测方法中误差最大值
    public Dictionary<int, float> JointOutOfThreshold(Dictionary<int, float> methodDic)
    {
        Dictionary<int, float> OutOfThresholdJointDic = new Dictionary<int, float>();
        foreach (var item in methodDic)
        {
            int joint = DATA.JointOfMethod[item.Key];
            if (!OutOfThresholdJointDic.ContainsKey(joint))
            {
                OutOfThresholdJointDic.Add(joint, item.Value);
            }
            else
            {
                if (Mathf.Abs(OutOfThresholdJointDic[joint])<Mathf.Abs(item.Value))
                {
                    OutOfThresholdJointDic[joint] = item.Value;
                }
            }
        }
        return OutOfThresholdJointDic;
    }
    public int ActionMatching(ActionData currentData,Action standardAction,ref OneTrainingData trainingData)
    {
        #region 返回当前动作完成的质量，如果SaveToDatabse=true，记录当前动作的所有数据，后续保存到数据库中
        ActionData standardData = standardAction.actionData;
        List<int> checkJoints = standardAction.checkJoints;
        float lowestAccuracy = 100;
        int jointid = 0;
        ActionData data = new ActionData();
        List<int> wrong_joint = new List<int>();
        for (int i=0;i<checkJoints.Count;i++)       //遍历该动作需要检测的关节点
        {
            int joint = checkJoints[i];
            JointData jointData = new JointData();
            //if (!DATA.JointCheckMethod.ContainsKey(joint)) continue;
            
            for (int j=0;j<DATA.JointCheckMethod[joint].Count;j++)  //遍历需要检测的关节点对应的检测方法
            {
                int method = DATA.JointCheckMethod[joint][j];
                if (!standardData.jointDatas[joint].invalidateMethods.Contains(method))     //录制动作时可以指定不使用某些检测方法？？？
                {
                    float standardAngle = standardData.jointDatas[joint].methodDatas[method].currentAngle;
                    float currentAngle = currentData.jointDatas[joint].methodDatas[method].currentAngle;
                    //准确率算法
                    float accuracy;
                    float offsetAngle= currentAngle;
                    int methodMaxDValue = DATA.MaxValueOfMethod[method] - DATA.MinValueOfMethod[method];        //检测指标角度波动范围
                    if (currentAngle < DATA.MinValueOfMethod[method]) offsetAngle = DATA.MinValueOfMethod[method];
                    if (currentAngle > DATA.MaxValueOfMethod[method]) offsetAngle = DATA.MaxValueOfMethod[method];//超过夹角范围的奇异值用边界值代替
                    accuracy = (1 - Mathf.Abs(standardAngle - offsetAngle) / methodMaxDValue) * 100;
                    if (accuracy < 0) accuracy = 0;//买一手保险。。。。
                    if (accuracy < lowestAccuracy)
                    {
                        lowestAccuracy = accuracy;      //更新最低准确率及对应的关节id
                        jointid = joint;
                    }
                    if (accuracy < DATA.ActionMatchThreshold["GOOD"])       //准确度<0.7 * 100
                    {
                        wrong_joint.Add(joint);
                    }
                    MethodData methodData=new MethodData(currentAngle, standardAngle, accuracy);
                    jointData.methodDatas.Add(method, methodData);      //记录关节在该检测方法下的准确度
                }
                else
                {   //该检测方法不适用被排除
                    jointData.invalidateMethods.Add(method);
                }
                
            }
            data.jointDatas.Add(joint, jointData);      //记录关节-jointData对
        }
        data.actionId = standardAction.id;
        print("动作"+data.actionId + "最小准确率为" + lowestAccuracy+"节点id"+jointid);       //打印准确率最低的关节 和 最低的准确率
        if (SaveToDatabse)
        {
            trainingData.actionDatas.Add(data);     //记录当前动作的结果
        }
        if (lowestAccuracy>=DATA.ActionMatchThreshold["PERFECT"])       //最低准确率大于0.9 * 100
        {
            return -3;
        }
        else if (lowestAccuracy >= DATA.ActionMatchThreshold["GREAT"])     //最低准确率大于0.8 * 100
        {
            return -2;
        }
        else if (lowestAccuracy > DATA.ActionMatchThreshold["GOOD"])        //最低准确率大于0.7 * 100
        {
            return -1;
        }
        else
        {
            if (wrong_joint.Contains(jointid) && (wrong_joint.Contains(jointid + 4) || wrong_joint.Contains(jointid - 4)))
            {
                return jointid + 30;
            }
            return jointid;     //返回错误的关节
        }
        #endregion


    }
    #endregion
}
