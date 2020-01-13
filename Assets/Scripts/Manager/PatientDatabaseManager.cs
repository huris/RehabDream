/* ============================================================================== 
* ClassName：DatabaseManager 
* Author：ChenShuwei 
* CreateDate：2019/10/20 10:32:12 
* Version: 1.0
* ==============================================================================*/

using UnityEngine;
using Mono.Data.Sqlite;
using System;
using System.IO;

public class PatientDatabaseManager : MonoBehaviour
{

    // Singleton instance holder
    public static PatientDatabaseManager instance = null;

    private string UserFolderPath = "Data/";
    private string PatientInfoTableName = "PatientInfo";
    private string PatientRecordTableName = "PatientRecord";
    private string GravityCenterTableName = "GravityCenter";
    private string AnglesTableName = "Angles";

    private string DoctorInfoTableName = "DoctorInfo";
    private string TrainingPlanTableName = "TrainingPlan";
    private string DirectionsTableName = "Directions";
    // .db is in data/

    public enum DatabaseReturn
    {
        NullInput,
        Success,
        Fail,
        AlreadyExist,
        Exception
    }

    private SQLiteHelper DoctorDatabase { get { return DoctorDatabaseManager.instance.GetDoctorDatabase(); } }     //DoctorDatabase
    private SQLiteHelper PatientDatabase { get { return DoctorDatabaseManager.instance.GetPatientDatabase(); } }    //PatientDatabase


    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            Debug.Log("@DatabaseManager: Singleton created.");

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        //CheckDataFolder();
        //ConnectPatientDatabase();
        //ConnectDoctorDatabase();
        Debug.Log("@DatabaseManager: DatabaseManager Start.");
    }

    //call when quit application, close connection
    void OnApplicationQuit()
    {
        PatientDatabase?.CloseConnection();
        DoctorDatabase?.CloseConnection();
        Debug.Log("@DatabaseManager: Close Connection.");
    }



    //check login
    public DatabaseReturn Login(long PatientID, string PatientPassword) //login
    {

        SqliteDataReader reader;    //sql读取器

        try
        {
            reader = PatientDatabase.ReadTable(
                PatientInfoTableName,
                new String[] {
                    "*"
                    },

                new String[] {
                    "PatientID",
                    "PatientPassword"
                    },

                new String[] {
                    "=",
                    "="
                    },

                new String[] {
                    PatientID.ToString(),
                    AddSingleQuotes(SQLiteHelper.MD5Encrypt(PatientPassword))   //md5加密
                    }
                );

            if (reader.Read() && reader.HasRows)
            {       //存在用户、密码正确
                Debug.Log("@DatabaseManager: Login Success");
                return DatabaseReturn.Success;
            }
            else
            {       //不存在用户或密码不正确
                Debug.Log("@DatabaseManager: Login Fail");
                return DatabaseReturn.Fail;
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Login SqliteException");
            PatientDatabase.CloseConnection();
            return DatabaseReturn.Exception;
        }


    }

    //Read PatientName
    public string ReadPatientName(long PatientID)
    {
        string PatientName = "PatientName";
        SqliteDataReader reader;    //sql读取器

        try
        {
            reader = PatientDatabase.ReadTable(
                PatientInfoTableName,
                new String[] {
                    "PatientName"
                    },

                new String[] {
                    "PatientID",
                    },

                new String[] {
                    "=",
                    },

                new String[] {
                    PatientID.ToString(),
                    }
                );


            if (reader.Read() && reader.HasRows && reader[0] != null && reader[0] != DBNull.Value)
            {
                PatientName = reader[0].ToString();
                Debug.Log("@DatabaseManager: Read PatientName Success");
            }
            else
            {
                Debug.Log("@DatabaseManager: Read PatientName Fail");
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Read PatientName SqliteException");
            PatientDatabase.CloseConnection();
        }

        return PatientName;
    }

    // read patientsex
    public string ReadPatientSex(long PatientID)
    {
        string PatientSex = "男";
        SqliteDataReader reader;    //sql读取器

        try
        {
            reader = PatientDatabase.ReadTable(
                PatientInfoTableName,
                new String[] {
                    "PatientSex"
                    },

                new String[] {
                    "PatientID",
                    },

                new String[] {
                    "=",
                    },

                new String[] {
                    PatientID.ToString(),
                    }
                );


            if (reader.Read() && reader.HasRows && reader[0] != null && reader[0] != DBNull.Value)
            {
                PatientSex = reader[0].ToString();
                Debug.Log("@DatabaseManager: Read PatientSex Success");
            }
            else
            {
                Debug.Log("@DatabaseManager: Read PatientSex Fail");
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Read PatientSex SqliteException");
            PatientDatabase.CloseConnection();
        }

        return PatientSex;
    }


    //check register
    public DatabaseReturn Register(long PatientID, string PatientName, string PatientSymptom)  //Register
    {
        if (PatientID.ToString().Equals("") || PatientSymptom.Equals("") || PatientName.Equals(""))   //input Null
        {
            Debug.Log("@DatabaseManager: Register NullInput");
            return DatabaseReturn.NullInput;
        }

        try
        {
            SqliteDataReader reader = PatientDatabase.ReadTable(
                PatientInfoTableName,
                new String[] {
                    "*"
                    },

                new String[] {
                    "PatientID",
                    },

                new String[] {
                    "=",
                    },

                new String[] {
                    PatientID.ToString(),
                    }
                );


            if (reader.Read() && reader.HasRows)
            {
                Debug.Log("@DatabaseManager: PatientAccount AlreadyExist");
                return DatabaseReturn.AlreadyExist;
            }


            PatientDatabase.InsertValues(
                PatientInfoTableName, //table name
                new String[] {
                    PatientID.ToString(),
                    AddSingleQuotes(PatientName),
                    AddSingleQuotes(SQLiteHelper.MD5Encrypt(PatientSymptom)),  //md5加密
                    0.ToString(),
                    0.ToString(),
                    "NULL",
                    0.ToString(),
                    0.ToString(),
                }
            );
            Debug.Log("@DatabaseManager: Register PatientAccount Success");
            return DatabaseReturn.Success;
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Register SqliteException");
            PatientDatabase.CloseConnection();
            return DatabaseReturn.Exception;
        }

    }


    //Generate TrainingID
    //default return:1
    public long GenerateTrainingID()
    {
        SqliteDataReader reader;    //sql读取器
        long MaxTrainingID = 0;

        try
        {
            //read Max_TrainingID
            reader = PatientDatabase.ReadTable(
                PatientRecordTableName,
                new String[] {
                    "MAX(TrainingID)",
                },
                new String[] { },
                new String[] { },
                new String[] { }
                );

            // 当返回0行时，reader.Read()==null,当返回行但值为空时,reader[0] == DBNull.Value
            if (reader.Read() && reader.HasRows && reader[0] != null && reader[0] != DBNull.Value)
            {
                //此时返回值只有一行而且列名为MAX(TrainingID) 而非 TrainingID
                MaxTrainingID = long.Parse(reader[0].ToString());
                Debug.Log("@DatabaseManager: Generate TrainingID Success-" + MaxTrainingID);
            }
            else
            {
                Debug.Log("@DatabaseManager: Generate TrainingID NULL");
            }



        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Generate MaxTrainingID SqliteException");

        }

        long TrainingID = MaxTrainingID + 1;


        return TrainingID;
    }


    //read patient's game record
    //return MaxSuccessCount
    //default return:0
    public long ReadMaxSuccessCount(long PatientID, string TrainingDifficulty)
    {

        SqliteDataReader reader;    //sql读取器
        long MaxSuccessCount = 0;

        try
        {

            //read Max_SuccessCount
            reader = PatientDatabase.ReadTable(
                PatientRecordTableName,
                new String[] {
                    "MAX(SuccessCount)",
                },
                new String[] {
                    "PatientID",
                    "TrainingDifficulty"
                },

                new String[] {
                    "=",
                    "="
                },
                new String[] {
                    PatientID.ToString(),
                    AddSingleQuotes(TrainingDifficulty),
                }
            );

            //Debug.Log(reader.Read());
            //Debug.Log(reader.HasRows);
            //Debug.Log(reader[0] != null);
            //Debug.Log(reader[0] != DBNull.Value);

            if (reader.Read() && reader.HasRows && reader[0] != null && reader[0] != DBNull.Value)
            {
                MaxSuccessCount = reader.GetInt64(reader.GetOrdinal("MAX(SuccessCount)"));
                Debug.Log("@DatabaseManager: Read MaxSuccessCount Success-" + MaxSuccessCount);
            }
            else
            {
                Debug.Log("@DatabaseManager: Read MaxSuccessCount NULL");
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Read MaxSuccessCount SqliteException");

        }

        return MaxSuccessCount;
    }

    // read MaxDirection by PatientID
    public float[] ReadEvaluateDirection(long PatientID)
    {
        SqliteDataReader reader;    //sql读取器
        long TrainingID = 0;
        float[] EvaluateDirection = {0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f};    //default value

        try
        {
            reader = PatientDatabase.ReadTable(
                PatientRecordTableName,
                new string[] {
                    "*"
                    },

                new string[] {
                    "PatientID",
                    "IsEvaluated"
                    },

                new string[] {
                    "=",
                    "="
                    },

                new string[] {
                    PatientID.ToString(),
                    1.ToString()
                }
                );

            if (reader.Read() && reader.HasRows)
            {   //存在训练记录
                TrainingID = reader.GetInt64(reader.GetOrdinal("TrainingID"));
            }
            else
            {
                Debug.Log("@DatabaseManager: Read IsEvaluated TrainingID NULL");
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Read IsEvaluated TrainingID SqliteException");
            PatientDatabase.CloseConnection();

        }

        try
        {
            reader = PatientDatabase.ReadTable(
                DirectionsTableName,
                new string[] {
                    "*"
                    },

                new string[] {
                    "TrainingID",
                    },

                new string[] {
                    "=",
                    },

                new string[] {
                    TrainingID.ToString(),
                }
                );

            if (reader.Read() && reader.HasRows)
            {   //存在训练记录
                EvaluateDirection[0] = reader.GetFloat(reader.GetOrdinal("UponDirection"));
                EvaluateDirection[1] = reader.GetFloat(reader.GetOrdinal("UponLeftDirection"));
                EvaluateDirection[2] = reader.GetFloat(reader.GetOrdinal("UponRightDirection"));
                EvaluateDirection[3] = reader.GetFloat(reader.GetOrdinal("DownDirection"));
                EvaluateDirection[4] = reader.GetFloat(reader.GetOrdinal("DownLeftDirection"));
                EvaluateDirection[5] = reader.GetFloat(reader.GetOrdinal("DownRightDirection"));
                EvaluateDirection[6] = reader.GetFloat(reader.GetOrdinal("LeftDirection"));
                EvaluateDirection[7] = reader.GetFloat(reader.GetOrdinal("RightDirection"));
            }
            else
            {
                Debug.Log("@DatabaseManager: Read MaxDirection NULL");
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Read MaxDirection SqliteException");
            PatientDatabase.CloseConnection();

        }

        return EvaluateDirection;
    }

    //write patient record
    public DatabaseReturn WritePatientRecord(long TrainingID, long PatientID, string TrainingStartTime, string TrainingEndTime, string TrainingDifficulty, long GameCount, long SuccessCount, string TrainingDirection, long TrainingTime, long IsEvaluated, float EvaluationSocre)
    {

        //try
        //{

        //write TrainingID-TrainingStartTime-TrainingEndTime-TrainingDifficulty-GameCount-SuccessCount to PatientRecord
        PatientDatabase.InsertValues(
            PatientRecordTableName, //table name
            new string[] {
                    TrainingID.ToString(),
                    PatientID.ToString(),
                    AddSingleQuotes(TrainingStartTime),
                    AddSingleQuotes(TrainingEndTime),
                    AddSingleQuotes(TrainingDifficulty),
                    GameCount.ToString(),
                    SuccessCount.ToString(),
                    AddSingleQuotes(TrainingDirection),
                    TrainingTime.ToString(),
                    IsEvaluated.ToString(),
                    EvaluationSocre.ToString()
            }
        );

        Debug.Log("@DatabaseManager: Write PatientRecord Success");
        return DatabaseReturn.Success;
        //}
        //catch (SqliteException e)
        //{
            Debug.Log("@DatabaseManager: Write PatientRecord SqliteException");
            this.PatientDatabase.CloseConnection();
            return DatabaseReturn.Exception;
        //}
    }

    //write patient record
    public DatabaseReturn WriteMaxDirection(long TrainingID, float[] MaxDirections)
    {

        try
        {

                PatientDatabase.InsertValues(
                DirectionsTableName, //table name
                new string[] {
                    TrainingID.ToString(),
                    MaxDirections[0].ToString(),
                    MaxDirections[1].ToString(),
                    MaxDirections[2].ToString(),
                    MaxDirections[3].ToString(),
                    MaxDirections[4].ToString(),
                    MaxDirections[5].ToString(),
                    MaxDirections[6].ToString(),
                    MaxDirections[7].ToString()
                }
            );

            Debug.Log("@DatabaseManager: Write MaxDirection Success");
            return DatabaseReturn.Success;
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Write MaxDirection SqliteException");
            this.PatientDatabase.CloseConnection();
            return DatabaseReturn.Exception;
        }
    }

    // read TrainingPlan for patient(PatientID)
    // return: PlanDifficulty GameCount PlanCount
    // default return: "Primary",10,10
    public TrainingPlanResult ReadTrainingPlan(long PatientID)
    {
        SqliteDataReader reader;    //sql读取器
        TrainingPlanResult result = new TrainingPlanResult();

        try
        {
            reader = DoctorDatabase.ReadTable(
                TrainingPlanTableName,
                new String[] {
                    "*"
                    },

                new String[] {
                    "PatientID",
                    },

                new String[] {
                    "=",
                    },

                new String[] {
                    PatientID.ToString(),
                    }
                );

            if (reader.Read() && reader.HasRows)
            {   //存在用户训练任务


                result.PlanDifficulty = reader.GetString(reader.GetOrdinal("PlanDifficulty"));
                result.GameCount = reader.GetInt64(reader.GetOrdinal("GameCount"));
                result.PlanCount = reader.GetInt64(reader.GetOrdinal("PlanCount"));
                result.PlanDirection = reader.GetString(reader.GetOrdinal("PlanDirection"));
                result.PlanTime = reader.GetInt64(reader.GetOrdinal("PlanTime"));


                Debug.Log("@DatabaseManager:Read TrainingPlan Success " +result.PlanDifficulty + " " + result.GameCount.ToString() + " " + result.PlanCount.ToString());

            }
            else
            {
                Debug.Log("@DatabaseManager: Read TrainingPlan NULL");
            }
        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Read TrainingPlan SqliteException");
            DoctorDatabase.CloseConnection();

        }

        return result;
    }

    // finish one mission
    public DatabaseReturn UpdateTrainingPlan(long PatientID, string PlanDifficulty, long NewPlanCount)
    {
        try
        {

            //UPDATE TrainingPlan SET PlanCount=NewPlanCount WHERE PatientID=PatientID AND PlanDifficulty=PlanDifficulty
            DoctorDatabase.UpdateValuesAND(
                TrainingPlanTableName,
                new string[] { "PlanCount" },
                new string[] { NewPlanCount.ToString() },
                new string[] { "PatientID", "PlanDifficulty" },
                new string[] { "=", "=" },
                new string[] { PatientID.ToString(), AddSingleQuotes(PlanDifficulty) }
                );

            // DELETE FROM TrainingPlan WHRER PlanCount=0
            DoctorDatabase.DeleteValuesAND(
                TrainingPlanTableName,
                new string[] { "PlanCount" },
                new string[] { "<=" },
                new string[] { 0.ToString() }
                );


            Debug.Log("@DatabaseManager:Update TrainingPlan Success");
            return DatabaseReturn.Success;

        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Update TrainingPlan SqliteException");
            DoctorDatabase.CloseConnection();
            return DatabaseReturn.Exception;
        }
    }



    //write gravity center to GravityCenter table
    public DatabaseReturn WriteGravityCenter(long TrainingID, string Coordinate, string Time)
    {
        try
        {

            PatientDatabase.InsertValues(
                GravityCenterTableName,
                new string[] {
                    TrainingID.ToString(),      //INTEGER TrainingID
                    AddSingleQuotes(Coordinate),//TEXT Coordinate
                    AddSingleQuotes(Time)       //TEXT Time
                }
            );


            //Debug.Log("@DatabaseManager: Write GravityCenter");
            return DatabaseReturn.Success;

        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Write GravityCenter SqliteException");
            PatientDatabase.CloseConnection();
            return DatabaseReturn.Exception;
        }
    }

    //write Angles to Angles table
    public DatabaseReturn WriteAngles(long TrainingID, float[] Angles, string Time)
    {
        try
        {

            PatientDatabase.InsertValues(
                AnglesTableName,
                new string[] {
                    TrainingID.ToString(),      //FLOAT TrainingID
                    Angles[0].ToString(),    //FLOAT LeftArmAngle
                    Angles[1].ToString(),    //FLOAT RightArmAngle
                    Angles[2].ToString(),    //FLOAT LeftLegAngle
                    Angles[3].ToString(),    //FLOAT RightLegAngle
                    Angles[4].ToString(),   //FLOAT LeftElbowAngle
                    Angles[5].ToString(),   //FLOAT RightElbowAngle
                    Angles[6].ToString(),   //FLOAT LeftKneeAngle
                    Angles[7].ToString(),   //FLOAT RightKneeAngle
                    Angles[8].ToString(),   //FLOAT LeftAnkleAngle
                    Angles[9].ToString(),   //FLOAT RightAnkleAngle
                    Angles[10].ToString(),   //FLOAT LeftHipAngle
                    Angles[11].ToString(),   //FLOAT RightHipAngle
                    Angles[12].ToString(),  //FLOAT HipAngle
                    Angles[13].ToString(),  //FLOAT LeftSideAngle
                    Angles[14].ToString(),  //FLOAT RightSideAngle
                    Angles[15].ToString(),  //UponSideAngle
                    Angles[16].ToString(),  //DownSideAngle
                    AddSingleQuotes(Time)       //STRING Time
                }
            );


            //Debug.Log("@DatabaseManager: Write Angles");
            return DatabaseReturn.Success;

        }
        catch (SqliteException e)
        {
            Debug.Log("@DatabaseManager: Write Angles SqliteException");
            PatientDatabase.CloseConnection();
            return DatabaseReturn.Exception;
        }
    }


    // create UserFolderPath/
    private void CheckDataFolder()
    {
        if (UserFolderPath.Equals(""))
        {
            return;
        }

        if (!Directory.Exists(UserFolderPath))
        {
            Directory.CreateDirectory(UserFolderPath);
        }
    }


    // add single quotes to s
    public string AddSingleQuotes(string s)
    {
        return "'" + s + "'";
    }

    // TrainingPlan in db
    public class TrainingPlanResult
    {
        public string PlanDifficulty = "初级";
        public long GameCount = 10;
        public long PlanCount = 10;
        public string PlanDirection = "上";
        public long PlanTime = 20;
    }


    #region 此处已移至DoctorDatabaseManager

    ////connect  PatientDatabase.db
    //connect DoctorDatabase.db
    //private void ConnectDoctorDatabase()
    //{
    //    string DbPath = UserFolderPath + "DoctorDatabase.db";
    //    this.DoctorDatabase = new SQLiteHelper("Data Source=" + DbPath); //connect DoctorDatabase.db

    //    //check DoctorInfoTable
    //    if (!this.DoctorDatabase.IsTableExists(DoctorInfoTableName))  //check DoctorInfoTableName table
    //    {
    //        this.DoctorDatabase.CreateTable(
    //            DoctorInfoTableName,   //table name
    //            new String[] {
    //                "DoctorID",
    //                "DoctorName",
    //                "DoctorPassword",
    //                "" },

    //            new String[] {
    //                "INTEGER UNIQUE NOT NULL",
    //                "TEXT NOT NULL",
    //                "TEXT NOT NULL",
    //                "PRIMARY KEY(DoctorID)" }
    //            );
    //        Debug.Log("@DatabaseManager: Create DoctorInfoTableName");
    //    }

    //    //check TrainingPlanTable
    //    if (!this.DoctorDatabase.IsTableExists(TrainingPlanTableName))  //check TrainingPlanTable table
    //    {
    //        this.DoctorDatabase.CreateTable(
    //            TrainingPlanTableName,   //table name
    //            new String[] {
    //                "PatientID",
    //                "PlanDifficulty",
    //                "GameCount",
    //                "PlanCount",
    //                "PlanDirection",
    //                "PlanTime",
    //                ""},

    //            new String[] {
    //                "INTEGER UNIQUE NOT NULL",
    //                "TEXT NOT NULL",
    //                "INTEGER UNIQUE NOT NULL",
    //                "INTEGER UNIQUE NOT NULL",
    //                "TEXT NOT NULL",
    //                "INTEGER NOT NULL",
    //                "PRIMARY KEY(PatientID)" }
    //            );
    //        Debug.Log("@DatabaseManager: Create TrainingPlanTable");
    //    }


    //    Debug.Log("@DatabaseManager: Connect DoctorDatabase.db");
    //}

    ////connect PatientDatabase.db
    //private void ConnectPatientDatabase()
    //{
    //    string DbPath = UserFolderPath + "PatientDatabase.db";
    //    this.PatientDatabase = new SQLiteHelper("Data Source=" + DbPath); //connect PatientDatabase.db

    //    //check PatientInfoTable
    //    if (!this.PatientDatabase.IsTableExists(PatientInfoTableName))  //check PatientInfoTableName table
    //    {
    //        this.PatientDatabase.CreateTable(
    //            PatientInfoTableName,   //table name
    //            new String[] {
    //                "PatientID",
    //                "PatientName",
    //                "PatientSymptom",
    //                "DoctorID",
    //                "PatientAge",
    //                "PatientSex",
    //                "PatientHeight",
    //                "PatientWeight",
    //                "" },

    //            new String[] {
    //                "INTEGER UNIQUE NOT NULL",
    //                "TEXT NOT NULL",
    //                "TEXT NOT NULL",
    //                "INTEGER NOT NULL",
    //                "INTEGER NOT NULL",
    //                "TEXT NOT NULL",
    //                "INTEGER",
    //                "INTEGER",
    //                "PRIMARY KEY(PatientID)" }
    //            );
    //        Debug.Log("@DatabaseManager: Create PatientInfoTable");
    //    }

    //    //check PatientRecordTable
    //    if (!this.PatientDatabase.IsTableExists(PatientRecordTableName))  //check PatientInfoTableName table
    //    {
    //        this.PatientDatabase.CreateTable(
    //            PatientRecordTableName,   //table name
    //            new String[] {
    //                "TrainingID",
    //                "PatientID",
    //                "TrainingStartTime",
    //                "TrainingEndTime",
    //                "TrainingDifficulty",
    //                "GameCount",
    //                "SuccessCount",
    //                "TrainingDirection",
    //                "TrainingTime",
    //                "IsEvaluated",
    //                "" },

    //            new String[] {
    //                "INTEGER UNIQUE NOT NULL",
    //                "INTEGER NOT NULL",
    //                "TEXT NOT NULL",
    //                "TEXT NOT NULL",
    //                "TEXT NOT NULL",
    //                "INTEGER NOT NULL",
    //                "INTEGER NOT NULL",
    //                "TEXT NOT NULL",
    //                "INTEGER NOT NULL",
    //                "INTEGER NOT NULL",
    //                "PRIMARY KEY(TrainingID)" }
    //            );
    //        Debug.Log("@DatabaseManager: Create PatientRecordTable");
    //    }

    //    //check GravityCenterTable
    //    if (!this.PatientDatabase.IsTableExists(GravityCenterTableName))  //check PatientInfoTableName table
    //    {
    //        this.PatientDatabase.CreateTable(
    //            GravityCenterTableName,   //table name
    //            new String[] {
    //                "TrainingID",
    //                "Coordinate",
    //                "Time" },

    //            new String[] {
    //                "INTEGER NOT NULL",
    //                "TEXT NOT NULL",
    //                "TEXT NOT NULL" }
    //            );
    //        Debug.Log("@DatabaseManager: Create GravityCenterTable");
    //    }

    //    //check GravityCenterTable
    //    if (!this.PatientDatabase.IsTableExists(AnglesTableName))  //check PatientInfoTableName table
    //    {
    //        this.PatientDatabase.CreateTable(
    //            AnglesTableName,   //table name
    //            new String[] {
    //                "TrainingID",
    //                "LeftArmAngle",
    //                "RightArmAngle",
    //                "LeftLegAngle",
    //                "RightLegAngle",
    //                "LeftElbowAngle",
    //                "RightElbowAngle",
    //                "LeftKneeAngle",
    //                "RightKneeAngle",
    //                "LeftAnkleAngle",
    //                "RightAnkleAngle",
    //                "LeftHipAngle",
    //                "RightHipAngle",
    //                "HipAngle",
    //                "LeftSideAngle",
    //                "RightSideAngle",
    //                "UponSideAngle",
    //                "DownSideAngle",
    //                "Time" },

    //            new String[] {
    //                "INTEGER NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "TEXT NOT NULL" }
    //            );
    //        Debug.Log("@DatabaseManager: Create AnglesTable");
    //    }

    //    //check GravityCenterTable
    //    if (!this.PatientDatabase.IsTableExists(DirectionsTableName))  //check PatientInfoTableName table
    //    {
    //        this.PatientDatabase.CreateTable(
    //            DirectionsTableName,   //table name
    //            new String[] {
    //                "TrainingID",
    //                "UponDirection",
    //                "UponLeftDirection",
    //                "UponRightDirection",
    //                "DownDirection",
    //                "DownLeftDirection",
    //                "DownRightDirection",
    //                "LeftDirection",
    //                "RightDirection",
    //                 },

    //            new String[] {
    //                "INTEGER NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL",
    //                "FLOAT NOT NULL"
    //                }
    //            );
    //        Debug.Log("@DatabaseManager: Create DirectionsTable");
    //    }

    //    Debug.Log("@DatabaseManager: Connect PatientAccount.db");
    //}

    #endregion
}
