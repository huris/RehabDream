using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseUtil  {
    static SQLiteHelper sql;
    static SqliteDataReader reader;
    public static void InitDataBase()
    {
        sql = new SQLiteHelper("data source=" + DATA.databasePath);
    }
    public static List<PreAction> LoadPreActions()
    {
        InitDataBase();
        List<PreAction> list = new List<PreAction>();
        try
        {
            reader = sql.ReadFullTable("preactions");
            while (reader.Read())
            {
                PreAction action = new PreAction() ;
                action.id = reader.GetInt32(0);
                action.filename = reader.GetString(1);
                action.sideFilename = reader.GetString(2);
                action.actionData = JsonHelper.DeserializeJsonToObject<ActionData>(reader.GetString(3));
                list.Add(action);
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("preactions table is not exsits!");
        }
        sql.CloseConnection();
        return list;
    }
    // 读取全部动作
    public static List<Action> LoadActions()
    {
        InitDataBase();
        List<Action> list = new List<Action>();
        try
        {
            reader = sql.ReadFullTable("actions");
            while (reader.Read())
            {
                Action action = new Action();
                action.id = reader.GetInt32(0);
                action.name = reader.GetString(1);
                action.describe = reader.GetString(2);
                action.createTime = reader.GetString(3);
                action.filename = reader.GetString(4);
                action.sideFilename = reader.GetString(5);
                action.gameFilename = reader.GetString(6);
                action.checkJoints = JsonHelper.DeserializeJsonToObject<List<int>>(reader.GetString(7));
                action.actionData = JsonHelper.DeserializeJsonToObject<ActionData>(reader.GetString(8));
                list.Add(action);
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("actions table is not exsits!");
        }
        sql.CloseConnection();
        return list;
    }
    public static List<WallDoctor> LoadDoctorInfo()
    {
        InitDataBase();
        List<WallDoctor> list = new List<WallDoctor>();
        try
        {
            reader = sql.ReadFullTable("doctorinfo");
            while (reader.Read())
            {
                WallDoctor doctor = new WallDoctor();
                doctor.id = reader.GetInt32(0);
                doctor.name = reader.GetString(1);
                doctor.pwd = reader.GetString(2);
                list.Add(doctor);
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("doctorinfo table is not exsits!");
        }
        sql.CloseConnection();
        return list;
    }
    public static List<User> LoadUserInfo()
    {
        InitDataBase();
        List<User> list = new List<User>();
        try
        {
            reader = sql.ReadFullTable("userinfo");
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string userName = reader.GetString(1);
                string sex = reader.GetString(2);
                int age = reader.GetInt32(3);
                int weight = reader.GetInt32(4);
                int trainingTypeId = reader.GetInt32(5);
                string pwd = reader.GetString(6);
                string level = reader.GetString(7);
                list.Add(new User(id, userName, sex, age, weight, trainingTypeId, pwd, JsonHelper.DeserializeJsonToObject<Level>(level)));
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("userinfo table is not exsits!");
        }
        sql.CloseConnection();
        return list;
    }
    public static void LoadTrainingType()
    {
        InitDataBase();
        try
        {
            reader = sql.ReadFullTable("trainingtype");
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string actions = reader.GetString(2);
                DATA.TrainingProgramIDToName.Add(id, name);
                DATA.TrainingProgramIDToActionIDs.Add(id, JsonHelper.DeserializeJsonToObject<List<int>>(actions));
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.LogWarning("trainingtype table is not null!");
            sql.InsertValues("trainingtype", new string[] { "" + 0, "'暂无训练方案'", "'[]'" });
        }
        sql.CloseConnection();
    }
}
