/* ============================================================================== 
* ClassName：DoctorDatabaseManager 
* Author：Hu Ben 
* CreateDate：2019/10/22 08:20:12 
* Version: 0.01
* ==============================================================================*/

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.IO;

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
    public static string UserFolderPath = "Data/";
    // .db is in data/

    public enum DatabaseReturn
    {
        NullInput,
        Success,
        Fail,
        AlreadyExist,
    }

    private SQLiteHelper DoctorDatabase = null;    // DoctorDatabase 医生端数据库
    private SQLiteHelper PatientDatabase = null;    // PatientDatabase 患者端数据库

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
        this.DoctorDatabase = new SQLiteHelper("Data Source=" + DbPath); // connect DoctorDatabase.db

        if (!this.DoctorDatabase.IsTableExists("DoctorInfo"))
        {
            this.DoctorDatabase.CreateTable(
                "DoctorInfo",   //table name
                new String[] { "DoctorID", "DoctorName", "DoctorPassword", "" },
                new String[] { "INTEGER UNIQUE NOT NULL", "TEXT NOT NULL", "TEXT NOT NULL", "PRIMARY KEY(DoctorID)" }
                );
            // this.DoctorDatabase.ExecuteQuery("CREATE TABLE 'DoctorInfo' ( DoctorID INTEGER UNIQUE NOT NULL, DoctorName TEXT NOT NULL, DoctorPassword TEXT NOT NULL, PRIMARY KEY(DoctorID) )");
            // create table DoctorID-DoctorName-DoctorPassword ->DoctorDatabase.db/DoctorInfo
        }
        if (!this.DoctorDatabase.IsTableExists("TrainingPlan"))
        {
            this.DoctorDatabase.CreateTable(
                "TrainingPlan",   //table name
                new String[] { "PatientID", "PlanDifficulty", "GameCount", "PlanCount", "" },
                new String[] { "INTEGER UNIQUE NOT NULL", "TEXT NOT NULL", "INTEGER NOT NULL", "INTEGER NOT NULL", "PRIMARY KEY(PatientID)" }
                );
            // this.DoctorDatabase.ExecuteQuery("CREATE TABLE 'TrainingPlan' ( PatientID INTEGER UNIQUE NOT NULL, PlanDifficulty TEXT NOT NULL, GameCount INTEGER NOT NULL, PlanCount INTEGER NOT NULL, PRIMARY KEY(PatientID) )");
            // create table PatientID-PlanDifficulty-GameCount-PlanCount ->DoctorDatabase.db/TrainingPlan
        }
        Debug.Log("@UserManager: Connect DoctorDatabase.db");
    }

    //connect PatientDatabase.db
    private void ConnectPatientDatabase()
    {
        string DbPath = UserFolderPath + "PatientDatabase.db";
        this.PatientDatabase = new SQLiteHelper("Data Source=" + DbPath); // connect PatientDatabase.db

        //if (!this.DoctorDatabase.IsTableExists("DoctorInfo"))
        //{
        //    this.DoctorDatabase.CreateTable(
        //        "DoctorInfo",   //table name
        //        new String[] { "DoctorID", "DoctorName", "DoctorPassword", "" },
        //        new String[] { "INTEGER UNIQUE NOT NULL", "TEXT NOT NULL", "TEXT NOT NULL", "PRIMARY KEY(DoctorID)" }
        //        );
        //    // this.DoctorDatabase.ExecuteQuery("CREATE TABLE 'DoctorInfo' ( DoctorID INTEGER UNIQUE NOT NULL, DoctorName TEXT NOT NULL, DoctorPassword TEXT NOT NULL, PRIMARY KEY(DoctorID) )");
        //    // create table DoctorID-DoctorName-DoctorPassword ->DoctorDatabase.db/DoctorInfo
        //}
        //if (!this.DoctorDatabase.IsTableExists("TrainingPlan"))
        //{
        //    this.DoctorDatabase.CreateTable(
        //        "TrainingPlan",   //table name
        //        new String[] { "PatientID", "PlanDifficulty", "GameCount", "PlanCount", "" },
        //        new String[] { "INTEGER UNIQUE NOT NULL", "TEXT NOT NULL", "INTEGER NOT NULL", "INTEGER NOT NULL", "PRIMARY KEY(PatientID)" }
        //        );
        //    // this.DoctorDatabase.ExecuteQuery("CREATE TABLE 'TrainingPlan' ( PatientID INTEGER UNIQUE NOT NULL, PlanDifficulty TEXT NOT NULL, GameCount INTEGER NOT NULL, PlanCount INTEGER NOT NULL, PRIMARY KEY(PatientID) )");
        //    // create table PatientID-PlanDifficulty-GameCount-PlanCount ->DoctorDatabase.db/TrainingPlan
        //}
        Debug.Log("@UserManager: Connect DoctorDatabase.db");
    }

    // check login
    public DatabaseReturn DoctorLogin(long DoctorID, string DoctorPassword) // DoctorLogin
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


    // read DoctorInformation
    public Doctor ReadDoctorInfo(long DoctorID)
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
                result = new Doctor();
                result.SetDoctorMessage(reader.GetInt64(reader.GetOrdinal("DoctorID")),
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

    // read Patient's Training Plan
    public TrainingPlan ReadPatientTrainingPlan(long PatientID)
    {

        SqliteDataReader reader;    //sql读取器
        string QueryString = "SELECT * FROM TrainingPlan where PatientID=" + PatientID.ToString();

        TrainingPlan trainingPlan = new TrainingPlan();

        try
        {
            reader = DoctorDatabase.ExecuteQuery(QueryString);
            reader.Read();

            if (reader.HasRows)
            {
                trainingPlan.SetTrainingPlan(
                    reader.GetString(reader.GetOrdinal("PlanDifficulty")),
                    reader.GetInt64(reader.GetOrdinal("GameCount")),
                    reader.GetInt64(reader.GetOrdinal("PlanCount")));

                trainingPlan.SetPlanIsMaking(true);

                return trainingPlan;
            }
            else
            {
                trainingPlan.SetPlanIsMaking(false);
                return trainingPlan;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@UserManager: MakePatientTrainingPlan SqliteException");
            DoctorDatabase?.CloseConnection();

            trainingPlan.SetPlanIsMaking(false);
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
                                                trainingPlan.PlanCount.ToString() }
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
        if (patient.PatientID <= 0 || patient.PatientName == "" || patient.PatientPassword == "" ||
            patient.PatientDoctorID <= 0 || patient.PatientAge < 0 || patient.PatientSex == "" ||
            patient.PatientHeight < 0 || patient.PatientWeight < 0)   // input Null
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
                                   new String[] { patient.PatientID.ToString(), AddSingleQuotes(patient.PatientName), AddSingleQuotes(patient.PatientPassword),
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
                    QueryString = "UPDATE PatientInfo SET PatientName=" + AddSingleQuotes(patient.PatientName) + " , PatientPassword=" +
                    AddSingleQuotes(patient.PatientPassword) + " , DoctorID=" + patient.PatientDoctorID.ToString() + " , PatientAge=" +
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
    public List<Patient> ReadDoctorPatientInformation(long DoctorID)
    {
        SqliteDataReader reader;    //sql读取器
        List<Patient> result = new List<Patient>(); //返回值
        string QueryString = "SELECT * FROM PatientInfo WHERE DoctorID=" + DoctorID.ToString() + " ORDER BY PatientID ASC";
        
        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                do
                {
                    var res = new Patient();
                    res.setPatientCompleteMessage(
                       reader.GetInt64(reader.GetOrdinal("PatientID")),
                       reader.GetString(reader.GetOrdinal("PatientName")),
                       reader.GetString(reader.GetOrdinal("PatientPassword")),
                       reader.GetInt64(reader.GetOrdinal("DoctorID")),
                       reader.GetInt64(reader.GetOrdinal("PatientAge")),
                       reader.GetString(reader.GetOrdinal("PatientSex")),
                       reader.GetInt64(reader.GetOrdinal("PatientHeight")),
                       reader.GetInt64(reader.GetOrdinal("PatientWeight"))
                       );
                    result.Add(res);
                } while (reader.Read());
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
    public List<Patient> PatientQueryInformation(string PatientName, string PatientSex, long PatientAge, long DoctorID)
    {
        //print("!!!!!");
        SqliteDataReader reader;    //sql读取器
        List<Patient> result = new List<Patient>(); //返回值
        string QueryString = "SELECT * FROM PatientInfo where DoctorID="+DoctorID.ToString();

        if (PatientName != "") QueryString += " and PatientName=" + AddSingleQuotes(PatientName);
        if (PatientSex != "") QueryString += " and PatientSex=" + AddSingleQuotes(PatientSex);
        if (PatientAge != 0) QueryString += " and PatientAge=" + PatientAge.ToString();

        QueryString += " ORDER BY PatientID ASC";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                do
                {
                    var res = new Patient();
                    res.setPatientCompleteMessage(
                       reader.GetInt64(reader.GetOrdinal("PatientID")),
                       reader.GetString(reader.GetOrdinal("PatientName")),
                       reader.GetString(reader.GetOrdinal("PatientPassword")),
                       reader.GetInt64(reader.GetOrdinal("DoctorID")),
                       reader.GetInt64(reader.GetOrdinal("PatientAge")),
                       reader.GetString(reader.GetOrdinal("PatientSex")),
                       reader.GetInt64(reader.GetOrdinal("PatientHeight")),
                       reader.GetInt64(reader.GetOrdinal("PatientWeight"))
                       );
                    result.Add(res);
                } while (reader.Read());

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
    public List<TrainingPlay> ReadPatientQueryHistoryRecord(long PatientID, string StartTime, string EndTime)
    {
        SqliteDataReader reader;    //sql读取器
        List<TrainingPlay> result = new List<TrainingPlay>(); //返回值
        string QueryString = "SELECT * FROM PatientRecord where PatientID=" + PatientID.ToString() + "and TrainingStartTime <= " + AddSingleQuotes(StartTime) + " and TrainingEndTime >= " + AddSingleQuotes(EndTime) +" order by TrainingEndTime";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                do
                {
                    var res = new TrainingPlay();
                    res.SetCompleteTrainingPlay(
                       reader.GetInt64(reader.GetOrdinal("TrainingID")),
                       reader.GetString(reader.GetOrdinal("TrainingStartTime")),
                       reader.GetString(reader.GetOrdinal("TrainingEndTime")),
                       reader.GetString(reader.GetOrdinal("TrainingDifficulty")),
                       reader.GetInt64(reader.GetOrdinal("SuccessCount")),
                       reader.GetInt64(reader.GetOrdinal("GameCount"))
                       );
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
    public List<TrainingPlay> ReadPatientRecord(long PatientID)
    {
        SqliteDataReader reader;    //sql读取器
        List<TrainingPlay> result = new List<TrainingPlay>(); //返回值
        string QueryString = "SELECT * FROM PatientRecord where PatientID=" + PatientID.ToString() + " order by TrainingEndTime";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                do
                {
                    var res = new TrainingPlay();                   
                    res.SetCompleteTrainingPlay(
                       reader.GetInt64(reader.GetOrdinal("TrainingID")),
                       reader.GetString(reader.GetOrdinal("TrainingStartTime")),
                       reader.GetString(reader.GetOrdinal("TrainingEndTime")),
                       reader.GetString(reader.GetOrdinal("TrainingDifficulty")),
                       reader.GetInt64(reader.GetOrdinal("SuccessCount")),
                       reader.GetInt64(reader.GetOrdinal("GameCount"))
                       );
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

    // read LastPatientRecord
    public TrainingPlay ReadLastPatientRecord(long PatientID)
    {
        SqliteDataReader reader;    //sql读取器
        TrainingPlay result = new TrainingPlay(); //返回值
        string QueryString = "SELECT * FROM PatientRecord where PatientID=" + PatientID.ToString() + " order by TrainingEndTime desc limit 1";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                result.SetCompleteTrainingPlay(
                reader.GetInt64(reader.GetOrdinal("TrainingID")),
                reader.GetString(reader.GetOrdinal("TrainingStartTime")),
                reader.GetString(reader.GetOrdinal("TrainingEndTime")),
                reader.GetString(reader.GetOrdinal("TrainingDifficulty")),
                reader.GetInt64(reader.GetOrdinal("SuccessCount")),
                reader.GetInt64(reader.GetOrdinal("GameCount"))
                );

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
    public List<GravityCenter> ReadGravityCenterRecord(long PatientID)
    {
        SqliteDataReader reader;    //sql读取器
        List<GravityCenter> result = new List<GravityCenter>(); //返回值
        string QueryString = "SELECT * FROM GravityCenter,PatientRecord where PatientID=" + PatientID.ToString() + " and GravityCenter.TrainingID=PatientRecord.TrainingID order by TrainingID";

        try
        {
            reader = PatientDatabase.ExecuteQuery(QueryString);
            reader.Read();
            if (reader.HasRows)
            {
                //存在用户训练任务
                do
                {
                    string Coordinate = reader.GetString(reader.GetOrdinal("Coordinate"));
                    string[] XYZ = Coordinate.Split(',');
                    Vector3 CoordinateVector3 = new Vector3(Convert.ToSingle(XYZ[0]), Convert.ToSingle(XYZ[1]), Convert.ToSingle(XYZ[2]));

                    var res = new GravityCenter();
                    res.SetCompleteGravityCenter(
                    reader.GetInt64(reader.GetOrdinal("TrainingID")),
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
