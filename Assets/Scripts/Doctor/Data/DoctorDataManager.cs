using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DoctorDataManager : MonoBehaviour {

    // Singleton instance holder
    public static DoctorDataManager instance = null;

    public Doctor doctor = null;
    public List<Doctor>Doctors = null;
    public List<Tuple<long, string> > DoctorsIDAndName = null;   // 返回所有医生的姓名,item1是pinyin，item2是中文
    public List<Action> Actions = null; // 所有动作
    public List<User> users = null;  // 穿墙系统的患者


    // 医生端UI界面子窗口选择
    // 0:患者信息管理(默认)，1:患者状况评估，2:训练任务制定，3:训练状况查询
    public int FunctionManager = 0;    

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

    public bool DoctorLoginCheck(string DoctorID, string DoctorPassword)
    {
        if (DoctorID == "root")
        {
            // 如果管理员账号不存在，则创建一个
            if (DoctorDatabaseManager.instance.CheckRoot() == DoctorDatabaseManager.DatabaseReturn.Success)
            {
                this.doctor = new Doctor(12345, DoctorDatabaseManager.instance.MD5Encrypt("root"), "root");
                DoctorDatabaseManager.instance.DoctorRegister(this.doctor);
            }

            if (DoctorDatabaseManager.instance.DoctorNameLogin(DoctorID, DoctorPassword) == DoctorDatabaseManager.DatabaseReturn.Success)
            {

                //print("成功");
                this.doctor = DoctorDatabaseManager.instance.ReadDoctorNameInfo(DoctorID);

                //DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
                this.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();

                this.DoctorsIDAndName = DoctorDatabaseManager.instance.ReadAllDoctorIDAndName();

                //foreach(var item in DoctorDataManager.instance.Patients)
                //{
                //    print(item.PatientPinyin);
                //}
                return true;
                //SceneManager.LoadScene("03-DoctorUI");  // 如果登录成功,则进入医生管理界面
            }
            else  // 如果账号密码不正确,则提示
            {
                return false;
                //ErrorInformation.SetActive(true);
            }


        }
        // 判断是否存在该用户且账号密码正确
        else if (DoctorID[0] >= '0' && DoctorID[0] <= '9')    // 如果为数字
        {
            if (DoctorDatabaseManager.instance.DoctorIDLogin(long.Parse(DoctorID), DoctorPassword) == DoctorDatabaseManager.DatabaseReturn.Success)
            {
                //print("成功");
                this.doctor = DoctorDatabaseManager.instance.ReadDoctorIDInfo(long.Parse(DoctorID));
                this.DoctorsIDAndName = DoctorDatabaseManager.instance.ReadAllDoctorIDAndName();

                //this.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();

                //DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
                //DoctorDataManager.instance.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();


                //foreach(var item in DoctorDataManager.instance.Patients)
                //{
                //    print(item.PatientPinyin);
                //}
                return true;
                //SceneManager.LoadScene("03-DoctorUI");  // 如果登录成功,则进入医生管理界面
            }
            else  // 如果账号密码不正确,则提示
            {
                return false;
                //ErrorInformation.SetActive(true);
            }
        }
        else    // 否则为输入姓名
        {
            if (DoctorDatabaseManager.instance.DoctorNameLogin(DoctorID, DoctorPassword) == DoctorDatabaseManager.DatabaseReturn.Success)
            {

                //print("成功");
                //DoctorDataManager.instance.doctor = DoctorDatabaseManager.instance.ReadDoctorNameInfo(DoctorID.text);
                this.doctor = DoctorDatabaseManager.instance.ReadDoctorNameInfo(DoctorID);
                this.DoctorsIDAndName = DoctorDatabaseManager.instance.ReadAllDoctorIDAndName();

                //this.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();
                //print("成功");
                //DoctorDataManager.instance.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID);
                //DoctorDataManager.instance.Doctors = DoctorDatabaseManager.instance.ReadAllDoctorInformation();


                //foreach(var item in DoctorDataManager.instance.Patients)
                //{
                //    print(item.PatientPinyin);
                //}
                return true;
                //SceneManager.LoadScene("03-DoctorUI");  // 如果登录成功,则进入医生管理界面
            }
            else  // 如果账号密码不正确,则提示
            {
                return false;
                //ErrorInformation.SetActive(true);
            }
        }
    }


}
