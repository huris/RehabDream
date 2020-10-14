using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameDatas : MonoBehaviour {
    private void Awake()
    {

        // 读取所有动作列表、所有用户信息
        DATA.actionList = DataBaseUtil.LoadActions();
        DATA.preActionList = DataBaseUtil.LoadPreActions();
        GameData.user_info = DataBaseUtil.LoadUserInfo();
        Debug.Log("@LoadGameDatas: Read Info");
    }
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // 系统组合后使用GetUserInfo传递单个用户信息,该用户默认为用户列表第一位
    public void GetWallGameUserInfo(int PatientId, string PatientName, string PatientSex, int PatientAge, int PatientWeight, int trainingTypeId, 
        int WallSpeed, string StartTime, int TrainingDays, bool IsWallRandom, int ActionNum, Dictionary<int,int> ActionRate)
    {
        //WallSpeed 墙运动的速度,以秒为单位，表示墙到人需要的时间
        //StartTime 治疗计划开始的时间
        //TrainingDays 疗程
        //IsWallRandom 墙是否随机生成
        //ActionRate 一个字典，元素<i,j>表示动作i重复j次
        //ActionNum 动作总次数、墙的总数

        Level level = new Level(WallSpeed, StartTime, TrainingDays, IsWallRandom, ActionNum, ActionRate);
        User user = new User(PatientId, PatientName, PatientSex, PatientAge, PatientWeight, trainingTypeId, "", level);
        GameData.user_info[0] = user;
    }
}
