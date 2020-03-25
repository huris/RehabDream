using NPinyin;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Doctor{

    //[Header("DoctorMessage")]
    public long DoctorID { get; private set; } = 0;
    public string DoctorName { get; private set; } = "DoctorName";
    public string DoctorPassword { get; private set; } = "123456";
    public string DoctorPinyin { get; private set; } = "DoctorPinyin";

    //[Header("PatientMessage")]
    public Patient patient = null;
    public int PatientIndex = -1;

    public Patient TempPatient = null;
    public int TempPatientIndex = -1;

    // PatientID, PatientName, PatientPassword, DoctorID, PatientAge, PatientSex, PatientHeight, PatientWeight
    public List<Patient> Patients = null;


    public Doctor() { }

    public Doctor(long DoctorID, string DoctorPassword, string DoctorName)
    {
        this.DoctorID = DoctorID;
        this.DoctorPassword = DoctorPassword;
        this.DoctorName = DoctorName;
        this.DoctorPinyin = Pinyin.GetPinyin(DoctorName);

        this.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(this.DoctorID, this.DoctorName);

        if (this.Patients != null && this.Patients.Count > 0)
        {
            //this.Patients[0].SetPatientData();
            this.patient = this.Patients[0];
            this.TempPatient = this.patient;
        }
    }

    // set DoctorID, DoctorName, DoctorPassword
    public void SetDoctorMessage(long DoctorID, string DoctorPassword, string DoctorName)
    {
        this.DoctorID = DoctorID;
        this.DoctorPassword = DoctorPassword;
        this.DoctorName = DoctorName;
        this.DoctorPinyin = Pinyin.GetPinyin(DoctorName);

        this.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(this.DoctorID, this.DoctorName);

        if (this.Patients != null && this.Patients.Count > 0)
        {
            //this.Patients[0].SetPatientData();
            this.patient = this.Patients[0];
            this.TempPatient = this.patient;
        }
    }

    public void SetDoctorPinyin(string DoctorPinyin)
    {
        this.DoctorPinyin = DoctorPinyin;
    }

    public void SetDoctorPassword(string DoctorPassword)
    {
        this.DoctorPassword = DoctorPassword;
    }

    // 对单个患者进行赋值
    public void SetPatientCompleteInformation(int PatientIndex)
    {
        //this.Patients[PatientIndex].SetPatientData();
        
        this.patient = this.Patients[PatientIndex];

        this.PatientIndex = PatientIndex;

        //this.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(this.patient.PatientID);
        //this.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(this.patient.PatientID, 0);
        //this.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientRecord(this.patient.PatientID, 1);
    }

    public void SetTempPatientCompleteInformation(int TempPatientIndex)
    {
        //this.Patients[TempPatientIndex].SetPatientData();

        this.TempPatient = this.Patients[TempPatientIndex];

        this.TempPatientIndex = TempPatientIndex;

        //this.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(this.patient.PatientID);
        //this.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(this.patient.PatientID, 0);
        //this.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientRecord(this.patient.PatientID, 1);
    }
}
