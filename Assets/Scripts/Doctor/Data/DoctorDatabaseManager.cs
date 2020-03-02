using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class DoctorDatabaseManager : MonoBehaviour
{
    /// <summary>
    /// 医生端
    /// 0. 数据库表 DoctorInfo
    ///    DoctorID, INTEGER, UNIQUE NOT NULL, PRIMARYKEY, 医生ID
    ///    DoctorName, TEXT, NOT NULL, 医生登录账号
    ///    DoctorPassword, TEXT, NOT NULL, 医生登录密码
    /// 1. 数据库表 TrainingPlan
    ///    PatientID, INTEGER, UNIQUE NOT NULL, PRIMARYKEY, 患者ID
    ///    PlanDifficulty, TEXT, NOT NULL, 训练方案难度（Primary, General, Intermediate, Advanced）
    ///    GameCount, INTEGER, NOT NULL, 一次游戏需要完成多少次扑救
    ///    PlanCount, INTEGER, NOT NULL, 训练次数或疗程, 每完成一次游戏减一
    /// </summary>


    // Singleton DoctorInstance holder
    public static DoctorDatabaseManager instance = null;
    public static string UserFolderPath = "Data/Database/";
    // .db is in data/

    private string PatientInfoTableName = "PatientInfo";
    private string PatientRecordTableName = "PatientRecord";
    private string GravityCenterTableName = "GravityCenter";
    private string AnglesTableName = "Angles";
    private string DirectionsTableName = "Directions";
    private string PatientEvaluationTableName = "PatientEvaluation";
    private string EvaluationSoccerTableName = "EvaluationSoccer";
    private string EvaluationPointsTableName = "EvaluationPoints";


    private string DoctorInfoTableName = "DoctorInfo";
    private string TrainingPlanTableName = "TrainingPlan";

    public enum DatabaseReturn
    {
        NullInput,
        Success,
        Fail,
        AlreadyExist,
    }

    private SQLiteHelper DoctorDatabase = null;    // DoctorDatabase 医生端数据库
    private SQLiteHelper PatientDatabase = null;    // PatientDatabase 患者端数据库

    public SQLiteHelper GetDoctorDatabase()
    {
        return DoctorDatabase;
    }

    public SQLiteHelper GetPatientDatabase()
    {
        return PatientDatabase;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("@DataManager: Singleton created.");

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

    }

    // Use this for initialization
    void Start()
    {
        CheckDataFolder();
        ConnectDoctorDatabase();
        ConnectPatientDatabase();

        // 医生信息测试 OK
        //DoctorRegister(123456, "HuBen", "123456");    // 注册医生测试 OK
        //DoctorDelete(123456);   // 删除医生测试 OK
        //DoctorModify(123456, "Hello", "123456789");  // 修改医生测试 OK
        //DoctorLogin(123456, "123456");   // 医生登录测试 OK

        // 患者信息测试 OK
        //PatientRegister(123, "HuBen", "123456", 123456, 23, "男", 176, 65);   // 注册患者测试 OK
        //PatientDelete(123);   // 删除医患者测试 OK
        //PatientModify(1234, "HuBen", "12346", 123456, 25, "男", 175, 65);   // 修改患者测试 OK

        // 患者训练计划 OK
        //MakePatientTrainingPlan(1234, "Primary", 20, 30);   // 制定患者训练计划 OK
        //ModifyPatientTrainingPlan(1234, "General", 50, 30);  // 修改患者训练计划 OK
        //DeletePatientTrainingPlan(1234);  // 删除患者训练计划 OK

        // 读取患者全部记录 OK
        // 插入患者记录语句 OK
        //PatientDatabase.InsertValues("PatientRecord", //table name
        //                                new String[] {"127", "1234", AddSingleQuotes("20191027 14:59:39"), AddSingleQuotes("20191027 14:59:39"),
        //                                               AddSingleQuotes("Primary"), "32", "24"}
        //                                );
        // 测试读取患者记录 OK
        //List<Tuple<long, string, string, string, long, long>> result = ReadPatientRecord(1234);
        //Debug.Log(result.Count);
        //foreach(var item in result)
        //{
        //    Debug.Log(item);
        //}

        // 数据格式：20191027 14:19:39
        //Debug.Log(DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));

        // 测试读取患者最后一条记录 OK 
        //Tuple<long, string, string, string, long, long> LastResult = ReadLastPatientRecord(1234);
        //Debug.Log(LastResult);

        // 测试三维坐标获取 OK
        // 插入三维坐标 OK
        //PatientDatabase.InsertValues("GravityCenter", //table name
        //                                new String[] {"127", AddSingleQuotes("4.0,4.0,4.0"), AddSingleQuotes("20191027 14:59:45")}
        //                                );

        // 获取患者三维坐标 OK
        //List<Tuple<long, Vector3, string>> result = ReadGravityCenterRecord(1234);

        //foreach(var item in result)
        //{
        //    Debug.Log(item.Item2.x);  // 获取x坐标
        //}

        Debug.Log("@UserManager: Database Connected.");
    }
    

    //call when quit application, close connection
    //void OnApplicationQuit()

    void OnDestroy()
    {
        DoctorDatabase?.CloseConnection();
        PatientDatabase?.CloseConnection();
        Debug.Log("@UserManager: Database Disconnected.");
    }

    //connect DoctorDatabase.db
    private void ConnectDoctorDatabase()
    {
        string DbPath = UserFolderPath + "DoctorDatabase.db";
        this.DoctorDatabase = new SQLiteHelper("Data Source=" + DbPath); //connect DoctorDatabase.db

        //check DoctorInfoTable
        if (!this.DoctorDatabase.IsTableExists(DoctorInfoTableName))  //check DoctorInfoTableName table
        {
            this.DoctorDatabase.CreateTable(
                DoctorInfoTableName,   //table name
                new String[] {
                    "DoctorID",
                    "DoctorName",
                    "DoctorPassword",
                    "" },

                new String[] {
                    "INTEGER UNIQUE NOT NULL",
                    "TEXT NOT NULL",
                    "TEXT NOT NULL",
                    "PRIMARY KEY(DoctorID)" }
                );
            Debug.Log("@DatabaseManager: Create DoctorInfoTableName");
        }

        //check TrainingPlanTable
        if (!this.DoctorDatabase.IsTableExists(TrainingPlanTableName))  //check TrainingPlanTable table
        {
            this.DoctorDatabase.CreateTable(
                TrainingPlanTableName,   //table name
                new String[] {
                    "PatientID",
                    "PlanDifficulty",
                    "GameCount",
                    "PlanCount",
                    "PlanDirection",
                    "PlanTime",
                    ""},

                new String[] {
                    "INTEGER UNIQUE NOT NULL",
                    "TEXT NOT NULL",
                    "INTEGER UNIQUE NOT NULL",
                    "INTEGER UNIQUE NOT NULL",
                    "TEXT NOT NULL",
                    "INTEGER NOT NULL",
                    "PRIMARY KEY(PatientID)" }
                );
            Debug.Log("@DatabaseManager: Create TrainingPlanTable");
        }


        Debug.Log("@DatabaseManager: Connect DoctorDatabase.db");
    }

    //connect PatientDatabase.db
    private void ConnectPatientDatabase()
    {
        string DbPath = UserFolderPath + "PatientDatabase.db";
        this.PatientDatabase = new SQLiteHelper("Data Source=" + DbPath); //connect PatientDatabase.db

        //check PatientInfoTable
        if (!this.PatientDatabase.IsTableExists(PatientInfoTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                PatientInfoTableName,   //table name
                new String[] {
                    "PatientID",
                    "PatientName",
                    "PatientSymptom",
                    "DoctorID",
                    "PatientAge",
                    "PatientSex",
                    "PatientHeight",
                    "PatientWeight",
                    "" },

                new String[] {
                    "INTEGER UNIQUE NOT NULL",
                    "TEXT NOT NULL",
                    "TEXT NOT NULL",
                    "INTEGER NOT NULL",
                    "INTEGER NOT NULL",
                    "TEXT NOT NULL",
                    "INTEGER",
                    "INTEGER",
                    "PRIMARY KEY(PatientID)" }
                );
            Debug.Log("@DatabaseManager: Create PatientInfoTable");
        }

        //check PatientRecordTable
        if (!this.PatientDatabase.IsTableExists(PatientRecordTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                PatientRecordTableName,   //table name
                new String[] {
                    "TrainingID",
                    "PatientID",
                    "TrainingStartTime",
                    "TrainingEndTime",
                    "TrainingDifficulty",
                    "GameCount",
                    "SuccessCount",
                    "TrainingDirection",
                    "TrainingTime",
                    "IsEvaluated",
                    "EvaluationScore",
                    "" },

                new String[] {
                    "INTEGER UNIQUE NOT NULL",
                    "INTEGER NOT NULL",
                    "TEXT NOT NULL",
                    "TEXT NOT NULL",
                    "TEXT NOT NULL",
                    "INTEGER NOT NULL",
                    "INTEGER NOT NULL",
                    "TEXT NOT NULL",
                    "INTEGER NOT NULL",
                    "INTEGER NOT NULL",
                    "FLOAT NOT NULL",
                    "PRIMARY KEY(TrainingID)" }
                );
            Debug.Log("@DatabaseManager: Create PatientRecordTable");
        }

        //check GravityCenterTable
        if (!this.PatientDatabase.IsTableExists(GravityCenterTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                GravityCenterTableName,   //table name
                new String[] {
                    "TrainingID",
                    "Coordinate",
                    "Time" },

                new String[] {
                    "INTEGER NOT NULL",
                    "TEXT NOT NULL",
                    "TEXT NOT NULL" }
                );
            Debug.Log("@DatabaseManager: Create GravityCenterTable");
        }

        //check GravityCenterTable
        if (!this.PatientDatabase.IsTableExists(AnglesTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                AnglesTableName,   //table name
                new String[] {
                    "TrainingID",
                    "LeftArmAngle",
                    "RightArmAngle",
                    "LeftLegAngle",
                    "RightLegAngle",
                    "LeftElbowAngle",
                    "RightElbowAngle",
                    "LeftKneeAngle",
                    "RightKneeAngle",
                    "LeftAnkleAngle",
                    "RightAnkleAngle",
                    "LeftHipAngle",
                    "RightHipAngle",
                    "HipAngle",
                    "LeftSideAngle",
                    "RightSideAngle",
                    "UponSideAngle",
                    "DownSideAngle",
                    "Time" },

                new String[] {
                    "INTEGER NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "TEXT NOT NULL" }
                );
            Debug.Log("@DatabaseManager: Create AnglesTable");
        }

        //check GravityCenterTable
        if (!this.PatientDatabase.IsTableExists(DirectionsTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                DirectionsTableName,   //table name
                new String[] {
                    "TrainingID",
                    "UponDirection",
                    "UponRightDirection",
                    "RightDirection",
                    "DownRightDirection",
                    "DownDirection",
                    "DownLeftDirection",
                    "LeftDirection",
                    "UponLeftDirection",
                     },

                new String[] {
                    "INTEGER NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL"
                    }
                );
            Debug.Log("@DatabaseManager: Create DirectionsTable");
        }

        //check GravityCenterTable
        if (!this.PatientDatabase.IsTableExists(PatientEvaluationTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                DirectionsTableName,   //table name
                new String[] {
                    "EvaluationID",
                    "PatientID",
                    "EvaluationStartTime",
                    "EvaluationEndTime",
                    ""},

                new String[] {
                    "INTEGER NOT NULL",
                    "INTEGER NOT NULL",
                    "TEXT NOT NULL",
                    "TEXT NOT NULL",
                    "PRIMARY KEY(EvaluationID)" 
                    }
                );
            Debug.Log("@DatabaseManager: Create PatientEvaluatiuon");
        }

        //check GravityCenterTable
        if (!this.PatientDatabase.IsTableExists(EvaluationSoccerTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                DirectionsTableName,   //table name
                new String[] {
                    "EvaluationID",
                    "UponSoccer",
                    "UponRightSoccer",
                    "RightSoccer",
                    "DownRightSoccer",
                    "DownSoccer",
                    "DownLeftSoccer",
                    "LeftSoccer",
                    "UponLeftSoccer",
                    "CenterSoccerMax",
                    "CenterSoccerMin",
                    ""},

                new String[] {
                    "INTEGER NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",                    
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "PRIMARY KEY(EvaluationID)"
                    }
                );
            Debug.Log("@DatabaseManager: Create EvaluationSoccer");
        }

        //check GravityCenterTable
        if (!this.PatientDatabase.IsTableExists(EvaluationPointsTableName))  //check PatientInfoTableName table
        {
            this.PatientDatabase.CreateTable(
                DirectionsTableName,   //table name
                new String[] {
                    "EvaluationID",
                    "PointX",
                    "PointY",
                    ""},

                new String[] {
                    "INTEGER NOT NULL",
                    "FLOAT NOT NULL",
                    "FLOAT NOT NULL",
                    "PRIMARY KEY(EvaluationID)"
                    }
                );
            Debug.Log("@DatabaseManager: Create EvaluationPoints");
        }

        Debug.Log("@DatabaseManager: Connect PatientAccount.db");
    }

    // check login
    public DatabaseReturn DoctorIDLogin(long DoctorID, string DoctorPassword) // DoctorLogin
    {
        if (DoctorID <= 0)   // input Null
        {
            Debug.Log("@UserManager: Login DoctorInfo NullInput");
            return DatabaseReturn.NullInput;
        }

        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM DoctorInfo where DoctorID=" + DoctorID.ToString() + " and DoctorPassword=" + AddSingleQuotes(MD5Encrypt(DoctorPassword));

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                // 用户名存在且密码正确
                Debug.Log("@UserManager: Login DoctorDatabase Success");
                return DatabaseReturn.Success;
            }
            else
            {
                // 不存在用户或密码不正确
                Debug.Log("@UserManager: Login DoctorDatabase Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Login DoctorDatabase SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    public DatabaseReturn DoctorNameLogin(string DoctorName, string DoctorPassword) // DoctorLogin
    {
        if (DoctorName == "")   // input Null
        {
            Debug.Log("@UserManager: Login DoctorInfo NullInput");
            return DatabaseReturn.NullInput;
        }

        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM DoctorInfo where DoctorName=" + AddSingleQuotes(DoctorName) + " and DoctorPassword=" + AddSingleQuotes(MD5Encrypt(DoctorPassword));

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                // 用户名存在且密码正确
                Debug.Log("@UserManager: Login DoctorDatabase Success");
                return DatabaseReturn.Success;
            }
            else
            {
                // 不存在用户或密码不正确
                Debug.Log("@UserManager: Login DoctorDatabase Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Login DoctorDatabase SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // Modify Doctor Information
    public DatabaseReturn DoctorModify(Doctor doctor)
    {
        if (doctor.DoctorID <= 0)   // input Null
        {
            Debug.Log("@UserManager: Modify DoctorInfo NullInput");
            return DatabaseReturn.NullInput;
        }

        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM DoctorInfo where DoctorID=" + doctor.DoctorID.ToString();

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                // 用户名存在
                QueryString = "UPDATE DoctorInfo SET DoctorName=" + AddSingleQuotes(doctor.DoctorName) + " , DoctorPassword=" + AddSingleQuotes(doctor.DoctorPassword) + " where DoctorID=" + doctor.DoctorID.ToString();
                DoctorDatabase.ExecuteQuery(QueryString);

                Debug.Log("@UserManager: Modify DoctorDatabase Success");
                return DatabaseReturn.Success;
            }
            else
            {
                // 用户名不存在
                Debug.Log("@UserManager: Modify DoctorDatabase Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Modify DoctorDatabase SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }


    // Delete DoctorInfo
    public DatabaseReturn DoctorDelete(long DoctorID)  // Delete
    {
        if (DoctorID <= 0)   // input Null
        {
            Debug.Log("@UserManager: Delete DoctorInfo NullInput");
            return DatabaseReturn.NullInput;
        }

        try
        {
            SqliteDataReader reader;    //sql读取器
            string QueryString = "SELECT * FROM DoctorInfo where DoctorID=" + DoctorID.ToString();

            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                QueryString = "DELETE FROM DoctorInfo where DoctorID=" + DoctorID.ToString();
                DoctorDatabase.ExecuteQuery(QueryString);
            }

            Debug.Log("@UserManager: Delete DoctorInfo Success");
            return DatabaseReturn.Success;
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Delete DoctorInfo SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // check doctor register
    public DatabaseReturn DoctorRegister(Doctor doctor)  // Register
    {
        if (doctor.DoctorID <= 0 || doctor.DoctorName == "" || doctor.DoctorPassword == "")   // input Null
        {
            Debug.Log("@UserManager: Register DoctorInfo NullInput");
            return DatabaseReturn.NullInput;
        }

        try
        {
            DoctorDatabase.InsertValues("DoctorInfo", //table name
                                        new String[] { doctor.DoctorID.ToString(), AddSingleQuotes(doctor.DoctorName), AddSingleQuotes(doctor.DoctorPassword) }
                                        );

            Debug.Log("@UserManager: Register DoctorInfo Success");
            return DatabaseReturn.Success;
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Register DoctorInfo SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // check DoctorInfo
    public DatabaseReturn CheckDoctor(long DoctorID) // DoctorInfo
    {
        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM DoctorInfo where DoctorID=" + DoctorID.ToString();

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                // 患者用户名存在
                Debug.Log("@UserManager: DoctorInfo Existence!");
                return DatabaseReturn.Fail;
            }
            else
            {
                // 患者用户名不存在
                Debug.Log("@UserManager: DoctorInfo will be created!");
                return DatabaseReturn.Success;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: DoctorInfo SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    public DatabaseReturn CheckRoot() // 特判是否存在root账户
    {
        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM DoctorInfo where DoctorName='root'";

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                // 患者用户名存在
                Debug.Log("@UserManager: DoctorInfo Existence!");
                return DatabaseReturn.Fail;
            }
            else
            {
                // 患者用户名不存在
                Debug.Log("@UserManager: DoctorInfo will be created!");
                return DatabaseReturn.Success;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: DoctorInfo SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }


    // read DoctorInformation
    public Doctor ReadDoctorIDInfo(long DoctorID)
    {
        SqliteDataReader reader;    //sql读取器
        Doctor result = null; //返回值
        string QueryString = "SELECT * FROM DoctorInfo where DoctorID=" + DoctorID.ToString();

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在医生信息
                result = new Doctor(reader.GetInt64(reader.GetOrdinal("DoctorID")),
                reader.GetString(reader.GetOrdinal("DoctorPassword")),
                reader.GetString(reader.GetOrdinal("DoctorName")));

                Debug.Log("@UserManager:Read DoctorInformation Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read DoctorInformation Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read DoctorInformation SqliteException");
            DoctorDatabase?.CloseConnection();
            return result;
        }
    }

    // read DoctorInformation
    public Doctor ReadDoctorNameInfo(string DoctorName)
    {
        SqliteDataReader reader;    //sql读取器
        Doctor result = null; //返回值
        string QueryString = "SELECT * FROM DoctorInfo where DoctorName=" + AddSingleQuotes(DoctorName);

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在医生信息
                result = new Doctor(reader.GetInt64(reader.GetOrdinal("DoctorID")),
                reader.GetString(reader.GetOrdinal("DoctorPassword")),
                reader.GetString(reader.GetOrdinal("DoctorName")));

               // print("@@@@");

                Debug.Log("@UserManager:Read DoctorInformation Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read DoctorInformation Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read DoctorInformation SqliteException");
            DoctorDatabase?.CloseConnection();
            return result;
        }
    }

    // 读取所有医生信息
    public List<Doctor> ReadAllDoctorInformation()
    {
        SqliteDataReader reader;    //sql读取器
        List<Doctor> result = new List<Doctor>(); //返回值
        string QueryString = "SELECT * FROM DoctorInfo";

        //ORDER BY convert(name using gbk)
        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<Doctor>();
                //存在用户训练任务
                do
                {
                    var res = new Doctor(
                       reader.GetInt64(reader.GetOrdinal("DoctorID")),
                       reader.GetString(reader.GetOrdinal("DoctorPassword")),
                       reader.GetString(reader.GetOrdinal("DoctorName"))
                       );

                    if(res.DoctorName != "root")
                    {
                        result.Add(res);
                    }

                } while (reader.Read());

                result = result.OrderBy(s => s.DoctorPinyin).ToList();

                Debug.Log("@UserManager:Read DoctorInfo Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read DoctorInfo Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read DoctorInfo SqliteException");
            DoctorDatabase?.CloseConnection();
            return result;
        }
    }

    // read Patient's Training Plan
    public TrainingPlan ReadPatientTrainingPlan(long PatientID)
    {

        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM TrainingPlan where PatientID=" + PatientID.ToString();

        TrainingPlan trainingPlan = null;

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                trainingPlan = new TrainingPlan(
                    reader.GetString(reader.GetOrdinal("PlanDifficulty")),
                    reader.GetInt64(reader.GetOrdinal("GameCount")),
                    reader.GetInt64(reader.GetOrdinal("PlanCount")),
                    reader.GetString(reader.GetOrdinal("PlanDirection")),
                    reader.GetInt64(reader.GetOrdinal("PlanTime")));

                //trainingPlan.SetPlanIsMaking(true);

                return trainingPlan;
            }
            else
            {
                //trainingPlan.SetPlanIsMaking(false);
                return trainingPlan;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: MakePatientTrainingPlan SqliteException");
            DoctorDatabase?.CloseConnection();

            //trainingPlan.SetPlanIsMaking(false);
            return trainingPlan;
        }
    }



    // make Patient's Training Plan
    public DatabaseReturn MakePatientTrainingPlan(long PatientID, TrainingPlan trainingPlan)
    {
        if (PatientID <= 0)   // input Null
        {
            Debug.Log("@UserManager: Make Patient's Training Plan NullInput");
            return DatabaseReturn.NullInput;
        }

        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM PatientInfo where PatientID=" + PatientID.ToString();

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                //write PatientID-PlanDifficulty-GameCount-PlanCount to DoctorDataBase.db
                DoctorDatabase.InsertValues("TrainingPlan", //table name
                                            new String[] {
                                                PatientID.ToString(),
                                                AddSingleQuotes(trainingPlan.PlanDifficulty),
                                                trainingPlan.GameCount.ToString(),
                                                trainingPlan.PlanCount.ToString(),
                                                AddSingleQuotes(trainingPlan.PlanDirection),
                                                trainingPlan.PlanTime.ToString()
                                                }
                                            );
                Debug.Log("@UserManager: MakePatientTrainingPlan Success");
                return DatabaseReturn.Success;
            }
            else
            {
                Debug.Log("@UserManager: MakePatientTrainingPlan Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: MakePatientTrainingPlan SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // modify Patient's Training Plan
    public DatabaseReturn ModifyPatientTrainingPlan(long PatientID, TrainingPlan trainingPlan)
    {
        if (PatientID <= 0)   // input Null
        {
            Debug.Log("@UserManager: Modify Patient's Training Plan NullInput");
            return DatabaseReturn.NullInput;
        }

        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM TrainingPlan where PatientID=" + PatientID.ToString();

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                QueryString = "UPDATE TrainingPlan SET PlanDifficulty=" + AddSingleQuotes(trainingPlan.PlanDifficulty) +
                    " , GameCount=" + trainingPlan.GameCount.ToString() + " , PlanCount=" + trainingPlan.PlanCount.ToString() +
                    " , PlanDirection=" + AddSingleQuotes(trainingPlan.PlanDirection) + " , PlanTime=" + trainingPlan.PlanTime.ToString() +
                    " where PatientID=" + PatientID.ToString();
                DoctorDatabase.ExecuteQuery(QueryString);

                Debug.Log("@UserManager: Modify Patient TrainingPlan Success");
                return DatabaseReturn.Success;
            }
            else
            {
                Debug.Log("@UserManager: Modify Patient TrainingPlan Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Modify Patient TrainingPlan SqliteException");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // Delete Patient's Training Plan
    public DatabaseReturn DeletePatientTrainingPlan(long PatientID)  // Delete
    {
        if (PatientID <= 0)   // input Null
        {
            Debug.Log("@UserManager: Delete Patient's Training Plan NullInput");
            return DatabaseReturn.NullInput;
        }

        try
        {
            SqliteDataReader reader;    //sql读取器
            string QueryString = "SELECT * FROM TrainingPlan where PatientID=" + PatientID.ToString();

            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                QueryString = "DELETE FROM TrainingPlan where PatientID=" + PatientID.ToString();
                DoctorDatabase.ExecuteQuery(QueryString);
            }

            Debug.Log("@UserManager: Delete Patient's Training Plan Success");
            return DatabaseReturn.Success;
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Delete Patient's Training Plan Fail");
            DoctorDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }


    // check PatientInfo
    public DatabaseReturn CheckPatient(long PatientID) // Check PatientInfo
    {
        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM PatientInfo where PatientID=" + PatientID.ToString();

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                // 患者用户名存在
                Debug.Log("@UserManager: PatientInfo Existence!");
                return DatabaseReturn.Fail;
            }
            else
            {
                // 患者用户名不存在
                Debug.Log("@UserManager: PatientInfo will be created!");
                return DatabaseReturn.Success;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: PatientDatabase SqliteException");
            PatientDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    //public void DeleteCS(long PatientID)
    //{
    //    SqliteDataReader reader;    //sql读取器
    //    string QueryString = "DELETE FROM TrainingPlan where PatientID=" + PatientID.ToString();

    //    reader = DoctorDatabase.ExecuteQuery(QueryString);


    //}


    // Delete PatientInfo
    public DatabaseReturn PatientDelete(long PatientID)  // Delete
    {
        if (PatientID <= 0)   // input Null
        {
            Debug.Log("@UserManager: Delete PatientInfo NullInput");
            return DatabaseReturn.NullInput;
        }

        try
        {
            SqliteDataReader reader;    //sql读取器
            string QueryString = "SELECT * FROM PatientInfo where PatientID=" + PatientID.ToString();

            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                // 删除患者信息
                QueryString = "DELETE FROM PatientInfo where PatientID=" + PatientID.ToString();
                PatientDatabase.ExecuteQuery(QueryString);

                // 删除患者重心数据
                QueryString = "DELETE FROM GravityCenter where exists(select TrainingID from PatientRecord where PatientRecord.TrainingID=GravityCenter.TrainingID and PatientRecord.PatientID=" + PatientID.ToString() + ")";
                PatientDatabase.ExecuteQuery(QueryString);

                // 删除患者训练记录
                QueryString = "DELETE FROM PatientRecord where PatientID=" + PatientID.ToString();
                PatientDatabase.ExecuteQuery(QueryString);

                // 删除患者训练计划
                QueryString = "DELETE FROM TrainingPlan where PatientID=" + PatientID.ToString();
                DoctorDatabase.ExecuteQuery(QueryString);
            }

            Debug.Log("@UserManager: Delete PatientInfo Success");
            return DatabaseReturn.Success;
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Delete PatientInfo SqliteException");
            PatientDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // check patient register
    public DatabaseReturn PatientRegister(Patient patient)
    {
        if (patient.PatientID <= 0 || patient.PatientName == "" || patient.PatientSymptom == "" ||
            patient.PatientDoctorID <= 0 || patient.PatientAge < 0 || patient.PatientSex == "")   // input Null
        {
            Debug.Log("@UserManager: Register PatientInfo NullInput");
            return DatabaseReturn.NullInput;
        }

        try
        {
            SqliteDataReader reader;    //sql读取器
            string QueryString = "SELECT * FROM DoctorInfo where DoctorID=" + patient.PatientDoctorID.ToString();

            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                PatientDatabase.InsertValues("PatientInfo", //table name
                                   new String[] { patient.PatientID.ToString(), AddSingleQuotes(patient.PatientName), AddSingleQuotes(patient.PatientSymptom),
                                                  patient.PatientDoctorID.ToString(), patient.PatientAge.ToString(),AddSingleQuotes(patient.PatientSex),patient.PatientHeight.ToString(),patient.PatientWeight.ToString() }
                                   );

                Debug.Log("@UserManager: Register PatientInfo Success");
                return DatabaseReturn.Success;
            }
            else
            {
                Debug.Log("@UserManager: Register PatientInfo Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Register PatientInfo SqliteException");
            PatientDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // Modify Patient Information
    public DatabaseReturn PatientModify(Patient patient)
    {
        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM PatientInfo where PatientID=" + patient.PatientID.ToString();

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                QueryString = "SELECT * FROM DoctorInfo where DoctorID=" + patient.PatientDoctorID.ToString();

                reader = DoctorDatabase.ExecuteQuery(QueryString);
                reader.Read();

                if (reader.HasRows)
                {
                    // 用户名存在
                    QueryString = "UPDATE PatientInfo SET PatientName=" + AddSingleQuotes(patient.PatientName) + " , PatientSymptom=" +
                    AddSingleQuotes(patient.PatientSymptom) + " , DoctorID=" + patient.PatientDoctorID.ToString() + " , PatientAge=" +
                    patient.PatientAge.ToString() + " , PatientSex=" + AddSingleQuotes(patient.PatientSex) + " , PatientHeight=" + patient.PatientHeight.ToString() +
                    " , PatientWeight=" + patient.PatientWeight.ToString() + " where PatientID=" + patient.PatientID.ToString();
                    PatientDatabase.ExecuteQuery(QueryString);

                    Debug.Log("@UserManager: Modify PatientDatabase Success");
                    return DatabaseReturn.Success;
                }
                else
                {
                    // DoctorID不存在
                    Debug.Log("@UserManager: Modify PatientDatabase Fail");
                    return DatabaseReturn.Fail;
                }
            }
            else
            {
                // 用户名不存在
                Debug.Log("@UserManager: Modify PatientDatabase Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Modify PatientDatabase SqliteException");
            PatientDatabase?.CloseConnection();
            return DatabaseReturn.Fail;
        }
    }

    // Read Doctor Patient Information
    // PatientID, PatientName, PatientPassword, DoctorID, PatientAge, PatientSex, PatientHeight, PatientWeight
    public List<Patient> ReadDoctorPatientInformation(long DoctorID, string DoctorName)
    {
        SqliteDataReader reader;    //sql读取器
        List<Patient> result = new List<Patient>(); //返回值
        string QueryString = "";
        
        if(DoctorID == 12345)   // 如果为root账户，则显示所有患者的信息
        {
            QueryString = "SELECT * FROM PatientInfo";
        }
        else    // 否则显示对应医生的信息
        {
            QueryString = "SELECT * FROM PatientInfo WHERE DoctorID=" + DoctorID.ToString();
        }

        //print(QueryString);
            
        //ORDER BY convert(name using gbk)
        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<Patient>();
                //存在用户训练任务
                do
                {
                    var res = new Patient(
                       reader.GetInt64(reader.GetOrdinal("PatientID")),
                       reader.GetString(reader.GetOrdinal("PatientName")),
                       reader.GetString(reader.GetOrdinal("PatientSymptom")),
                       reader.GetInt64(reader.GetOrdinal("DoctorID")),
                       DoctorName,
                       reader.GetInt64(reader.GetOrdinal("PatientAge")),
                       reader.GetString(reader.GetOrdinal("PatientSex")),
                       reader.GetInt64(reader.GetOrdinal("PatientHeight")),
                       reader.GetInt64(reader.GetOrdinal("PatientWeight"))
                       );

                    //print(res.PatientName);
                    //res.SetPatientPinyin(Pinyin.GetPinyin(res.PatientName));
                    //res.trainingPlan = this.ReadPatientTrainingPlan(res.PatientID);
                    //res.TrainingPlays = this.ReadPatientRecord(res.PatientID, 0);
                    //res.Evaluations = this.ReadPatientRecord(res.PatientID, 1);
                    result.Add(res);
                } while (reader.Read());
                result = result.OrderBy(s => s.PatientPinyin).ToList();
                
                Debug.Log("@UserManager:Read Doctor PatientInfo Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read Doctor PatientInfo Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read Doctor PatientInfo SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }


    // Query Patient Information
    public List<Patient> PatientQueryInformation(string PatientName, string PatientSex, long PatientAge, long DoctorID, string DoctorName)
    {
        //print("!!!!!");
        SqliteDataReader reader;    //sql读取器
        List<Patient> result = new List<Patient>(); //返回值
        string QueryString = "SELECT * FROM PatientInfo where DoctorID="+DoctorID.ToString();

        if (PatientName != "") QueryString += " and PatientName=" + AddSingleQuotes(PatientName);
        if (PatientSex != "") QueryString += " and PatientSex=" + AddSingleQuotes(PatientSex);
        if (PatientAge != 0) QueryString += " and PatientAge=" + PatientAge.ToString();

        QueryString += " ORDER BY PatientName ASC";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<Patient>();
                //存在用户训练任务
                do
                {
                    var res = new Patient(
                       reader.GetInt64(reader.GetOrdinal("PatientID")),
                       reader.GetString(reader.GetOrdinal("PatientName")),
                       reader.GetString(reader.GetOrdinal("PatientSymptom")),
                       reader.GetInt64(reader.GetOrdinal("DoctorID")),
                       DoctorName,
                       reader.GetInt64(reader.GetOrdinal("PatientAge")),
                       reader.GetString(reader.GetOrdinal("PatientSex")),
                       reader.GetInt64(reader.GetOrdinal("PatientHeight")),
                       reader.GetInt64(reader.GetOrdinal("PatientWeight"))
                       );
                    //res.SetPatientPinyin(Pinyin.GetPinyin(res.PatientName));
                    //res.trainingPlan = this.ReadPatientTrainingPlan(res.PatientID);
                    //res.TrainingPlays = this.ReadPatientRecord(res.PatientID, 0);
                    //res.Evaluations = this.ReadPatientRecord(res.PatientID, 1);
                    result.Add(res);
                } while (reader.Read());
                result = result.OrderBy(s => s.PatientPinyin).ToList();
                Debug.Log("@UserManager:Query PatientInfo Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Query PatientInfo Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Query PatientInfo SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    //Query Patient Information
    public List<Patient> PatientQueryInformation(string PatientName, long PatientDoctorID, string PatientDoctorName)
    {
        //print("!!!!!");
        SqliteDataReader reader;    //sql读取器
        List<Patient> result = new List<Patient>(); //返回值
        string QueryString = "";

        if(PatientName == "")   // 如果未填患者信息
        {
            if(PatientDoctorID == -1)
            {
                QueryString = "SELECT * FROM PatientInfo"; 
            }
            else
            {
                QueryString = "SELECT * FROM PatientInfo WHERE DoctorID = " + PatientDoctorID.ToString();
            }
        }
        else
        {
            if (PatientDoctorID == -1)  // 如果未选择医生
            {
                // 如果为输入患者病历号
                if (PatientName[0] >= '0' && PatientName[0] <= '9')
                {
                    QueryString = "SELECT * FROM PatientInfo WHERE PatientID = " + PatientName;
                }
                else   // 否则输入的是患者的姓名
                {
                    QueryString = "SELECT * FROM PatientInfo WHERE PatientName = " + AddSingleQuotes(PatientName);
                }
            }
            else
            {
                // 如果为输入患者病历号
                if (PatientName[0] >= '0' && PatientName[0] <= '9')
                {
                    QueryString = "SELECT * FROM PatientInfo WHERE PatientID = " + PatientName + " AND DoctorID = " + PatientDoctorID.ToString();
                }
                else   // 否则输入的是患者的姓名
                {
                    QueryString = "SELECT * FROM PatientInfo WHERE PatientName = " + AddSingleQuotes(PatientName) + " AND DoctorID = " + PatientDoctorID.ToString();

                }
            }
        }

        QueryString += " ORDER BY PatientName ASC";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<Patient>();
                //存在用户训练任务
                do
                {
                    var res = new Patient(
                       reader.GetInt64(reader.GetOrdinal("PatientID")),
                       reader.GetString(reader.GetOrdinal("PatientName")),
                       reader.GetString(reader.GetOrdinal("PatientSymptom")),
                       reader.GetInt64(reader.GetOrdinal("DoctorID")),
                       PatientDoctorName,
                       reader.GetInt64(reader.GetOrdinal("PatientAge")),
                       reader.GetString(reader.GetOrdinal("PatientSex")),
                       reader.GetInt64(reader.GetOrdinal("PatientHeight")),
                       reader.GetInt64(reader.GetOrdinal("PatientWeight"))
                       );
                    //res.SetPatientPinyin(Pinyin.GetPinyin(res.PatientName));
                    //res.trainingPlan = this.ReadPatientTrainingPlan(res.PatientID);
                    //res.TrainingPlays = this.ReadPatientRecord(res.PatientID, 0);
                    //res.Evaluations = this.ReadPatientRecord(res.PatientID, 1);
                    result.Add(res);
                } while (reader.Read());
                result = result.OrderBy(s => s.PatientPinyin).ToList();
                Debug.Log("@UserManager:Query PatientInfo Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Query PatientInfo Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Query PatientInfo SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    // read PatientRecord
    public List<TrainingPlay> ReadPatientQueryHistoryRecord(long PatientID, string StartTime, string EndTime, long IsEvaluated)
    {
        SqliteDataReader reader;    //sql读取器
        List<TrainingPlay> result = new List<TrainingPlay>(); //返回值
        string QueryString = "SELECT * FROM PatientRecord where PatientID=" + PatientID.ToString() + " and TrainingStartTime >= " + AddSingleQuotes(StartTime) + " and TrainingEndTime <= " + AddSingleQuotes(EndTime) + " and IsEvaluated = "+ IsEvaluated.ToString() +" order by TrainingEndTime";
        //string QueryString = "SELECT * FROM PatientRecord where PatientID=" + PatientID.ToString() +  " order by TrainingEndTime";

        //print(QueryString);
        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<TrainingPlay>();
                //存在用户训练任务
                do
                {
                    var res = new TrainingPlay(
                       reader.GetInt64(reader.GetOrdinal("TrainingID")),
                       reader.GetString(reader.GetOrdinal("TrainingStartTime")),
                       reader.GetString(reader.GetOrdinal("TrainingEndTime")),
                       reader.GetString(reader.GetOrdinal("TrainingDifficulty")),
                       reader.GetInt64(reader.GetOrdinal("SuccessCount")),
                       reader.GetInt64(reader.GetOrdinal("GameCount")),
                       reader.GetString(reader.GetOrdinal("TrainingDirection")),
                       reader.GetInt64(reader.GetOrdinal("TrainingTime"))
                       );
                    //res.angles = this.ReadAngleRecord(res.TrainingID);
                    //res.direction = this.ReadDirectionRecord(res.TrainingID);
                    //res.gravityCenters = this.ReadGravityCenterRecord(res.TrainingID);
                    result.Add(res);
                } while (reader.Read());

                Debug.Log("@UserManager:Read PatientRecord Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read PatientRecord Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read PatientRecord SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    // read PatientRecord
    public List<TrainingPlay> ReadPatientRecord(long PatientID, long IsEvaluated)
    {
        SqliteDataReader reader;    //sql读取器
        List<TrainingPlay> result = new List<TrainingPlay>(); //返回值
        string QueryString = "SELECT * FROM PatientRecord where PatientID=" + PatientID.ToString() + " and IsEvaluated = " + IsEvaluated.ToString() + " order by TrainingEndTime";

       //print(QueryString);

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<TrainingPlay>();
                //存在用户训练任务
                do
                {
                    var res = new TrainingPlay(
                       reader.GetInt64(reader.GetOrdinal("TrainingID")),
                       reader.GetString(reader.GetOrdinal("TrainingStartTime")),
                       reader.GetString(reader.GetOrdinal("TrainingEndTime")),
                       reader.GetString(reader.GetOrdinal("TrainingDifficulty")),
                       reader.GetInt64(reader.GetOrdinal("SuccessCount")),
                       reader.GetInt64(reader.GetOrdinal("GameCount")),
                       reader.GetString(reader.GetOrdinal("TrainingDirection")),
                       reader.GetInt64(reader.GetOrdinal("TrainingTime"))
                       );
                    //res.angles = this.ReadAngleRecord(res.TrainingID);
                    //res.direction = this.ReadDirectionRecord(res.TrainingID);
                    //res.gravityCenters = this.ReadGravityCenterRecord(res.TrainingID);
                    result.Add(res);
                } while (reader.Read());

                Debug.Log("@UserManager:Read PatientRecord Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read PatientRecord Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read PatientRecord SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    public long ReadPatientRecordCount(long IsEvaluated)   // 返回训练或者评估的数目
    {
        SqliteDataReader reader;    //sql读取器
        //List<TrainingPlay> result = null; //返回值
        string QueryString = "SELECT * FROM PatientRecord where IsEvaluated = " + IsEvaluated.ToString();

        long PatientRecordCount = 0;
        //print(QueryString);

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                do
                {
                    PatientRecordCount++;
                    //var res = new TrainingPlay(
                    //   reader.GetInt64(reader.GetOrdinal("TrainingID")),
                    //   reader.GetString(reader.GetOrdinal("TrainingStartTime")),
                    //   reader.GetString(reader.GetOrdinal("TrainingEndTime")),
                    //   reader.GetString(reader.GetOrdinal("TrainingDifficulty")),
                    //   reader.GetInt64(reader.GetOrdinal("SuccessCount")),
                    //   reader.GetInt64(reader.GetOrdinal("GameCount")),
                    //   reader.GetString(reader.GetOrdinal("TrainingDirection")),
                    //   reader.GetInt64(reader.GetOrdinal("TrainingTime"))
                    //   );
                    ////res.angles = this.ReadAngleRecord(res.TrainingID);
                    ////res.direction = this.ReadDirectionRecord(res.TrainingID);
                    ////res.gravityCenters = this.ReadGravityCenterRecord(res.TrainingID);
                    //result.Add(res);
                } while (reader.Read());

                //Debug.Log("@UserManager:Read PatientRecord Success" + result);
                return PatientRecordCount;
            }
            else
            {
                //Debug.Log("@UserManager: Read PatientRecord Fail");
                return PatientRecordCount;
            }
        }
        catch (SqliteException e)
        {
            //Debug.Log("@UserManager: Read PatientRecord SqliteException");
            PatientDatabase?.CloseConnection();
            return PatientRecordCount;
        }
    }

    // read LastPatientRecord
    public TrainingPlay ReadLastPatientRecord(long PatientID, long IsEvaluated)
    {
        SqliteDataReader reader;    //sql读取器
        TrainingPlay result = null; //返回值
        string QueryString = "SELECT * FROM PatientRecord where PatientID=" + PatientID.ToString() + " and IsEvaluated = "+IsEvaluated.ToString()+" order by TrainingEndTime desc limit 1";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                result = new TrainingPlay(
                reader.GetInt64(reader.GetOrdinal("TrainingID")),
                reader.GetString(reader.GetOrdinal("TrainingStartTime")),
                reader.GetString(reader.GetOrdinal("TrainingEndTime")),
                reader.GetString(reader.GetOrdinal("TrainingDifficulty")),
                reader.GetInt64(reader.GetOrdinal("SuccessCount")),
                reader.GetInt64(reader.GetOrdinal("GameCount")),
                reader.GetString(reader.GetOrdinal("TrainingDirection")),
                reader.GetInt64(reader.GetOrdinal("TrainingTime"))
                );
                //result.angles = this.ReadAngleRecord(result.TrainingID);
                //result.direction = this.ReadDirectionRecord(result.TrainingID);
                //result.gravityCenters = this.ReadGravityCenterRecord(result.TrainingID);
                Debug.Log("@UserManager:Read LastPatientRecord Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read LastPatientRecord Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read LastPatientRecord SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    // read GravityCenterRecord
    public List<GravityCenter> ReadGravityCenterRecord(long TrainingID)
    {
        SqliteDataReader reader;    //sql读取器
        List<GravityCenter> result = new List<GravityCenter>(); //返回值
        string QueryString = "SELECT * FROM GravityCenter where TrainingID=" + TrainingID.ToString() + "  order by Time";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<GravityCenter>();
                //存在用户训练任务
                do
                {
                    string Coordinate = reader.GetString(reader.GetOrdinal("Coordinate"));
                    string[] XYZ = Coordinate.Split(',');
                    Vector3 CoordinateVector3 = new Vector3(Convert.ToSingle(XYZ[0]), Convert.ToSingle(XYZ[1]), Convert.ToSingle(XYZ[2]));

                    var res = new GravityCenter(
                    //reader.GetInt64(reader.GetOrdinal("TrainingID")),
                    CoordinateVector3,
                    reader.GetString(reader.GetOrdinal("Time"))
                    );
                    result.Add(res);
                } while (reader.Read());

                Debug.Log("@UserManager:Read GravityCenterRecord Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read GravityCenterRecord Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read GravityCenterRecord SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    // read Angle
    public List<Angle> ReadAngleRecord(long TrainingID)
    {
        SqliteDataReader reader;    //sql读取器
        List<Angle> result = new List<Angle>(); //返回值
        string QueryString = "SELECT * FROM Angles where TrainingID=" + TrainingID.ToString() + " order by Time";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //result = new List<Angle>();
                //存在用户训练任务
                do
                {
                    var res = new Angle(
                    //reader.GetInt64(reader.GetOrdinal("TrainingID")),
                    reader.GetFloat(reader.GetOrdinal("LeftArmAngle")),
                    reader.GetFloat(reader.GetOrdinal("RightArmAngle")),
                    reader.GetFloat(reader.GetOrdinal("LeftLegAngle")),
                    reader.GetFloat(reader.GetOrdinal("RightLegAngle")),
                    reader.GetFloat(reader.GetOrdinal("LeftElbowAngle")),
                    reader.GetFloat(reader.GetOrdinal("RightElbowAngle")),
                    reader.GetFloat(reader.GetOrdinal("LeftKneeAngle")),
                    reader.GetFloat(reader.GetOrdinal("RightKneeAngle")),
                    reader.GetFloat(reader.GetOrdinal("LeftHipAngle")),
                    reader.GetFloat(reader.GetOrdinal("RightHipAngle")),
                    reader.GetFloat(reader.GetOrdinal("HipAngle")),
                    reader.GetFloat(reader.GetOrdinal("LeftAnkleAngle")),
                    reader.GetFloat(reader.GetOrdinal("RightAnkleAngle")),
                    reader.GetFloat(reader.GetOrdinal("LeftSideAngle")),
                    reader.GetFloat(reader.GetOrdinal("RightSideAngle")),
                    reader.GetFloat(reader.GetOrdinal("UponSideAngle")),
                    reader.GetFloat(reader.GetOrdinal("DownSideAngle")),
                    reader.GetString(reader.GetOrdinal("Time"))
                    );
                    result.Add(res);
                } while (reader.Read());

                Debug.Log("@UserManager:Read AnglesRecord Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read AnglesRecord Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read AnglesRecord SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    // read Angle
    public Direction ReadDirectionRecord(long TrainingID)
    {
        SqliteDataReader reader;    //sql读取器
        Direction result = null; //返回值
        string QueryString = "SELECT * FROM Directions where TrainingID=" + TrainingID.ToString();

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                result = new Direction(
                //reader.GetInt64(reader.GetOrdinal("TrainingID")),
                reader.GetFloat(reader.GetOrdinal("UponDirection")),
                reader.GetFloat(reader.GetOrdinal("UponRightDirection")),
                reader.GetFloat(reader.GetOrdinal("RightDirection")),
                reader.GetFloat(reader.GetOrdinal("DownRightDirection")),
                reader.GetFloat(reader.GetOrdinal("DownDirection")),
                reader.GetFloat(reader.GetOrdinal("DownLeftDirection")),
                reader.GetFloat(reader.GetOrdinal("LeftDirection")),
                reader.GetFloat(reader.GetOrdinal("UponLeftDirection")));

                Debug.Log("@UserManager:Read DirectionRecord Success" + result);
                return result;
            }
            else
            {
                Debug.Log("@UserManager: Read DirectionRecord Fail");
                return result;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: Read DirectionRecord SqliteException");
            PatientDatabase?.CloseConnection();
            return result;
        }
    }

    // create Data/
    private void CheckDataFolder()
    {
        if (!Directory.Exists(UserFolderPath))
        {
            Directory.CreateDirectory(UserFolderPath);
        }
    }

    // add single quotes to string
    public string AddSingleQuotes(string s)
    {
        return "'" + s + "'";
    }

    /// <summary>
    /// 用MD5加密字符串
    /// </summary>
    /// <param name="password">待加密的字符串</param>
    /// <returns></returns>
    public string MD5Encrypt(string password)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes;
        hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));
        }
        return tmp.ToString();
    }
}
