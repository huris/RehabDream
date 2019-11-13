using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patient {


    public string PatientName { get; private set; } = "PatientName";
    public long PatientID { get; private set; } = 0;
    public long PatientDoctorID { get; private set; } = 0;
    public string PatientPassword { get; private set; } = "123456";
    public long PatientAge { get; private set; } = 0;
    public string PatientSex { get; private set; } = "男";
    public long PatientHeight { get; private set; } = 0;
    public long PatientWeight { get; private set; } = 0;

    public TrainingPlan trainingPlan = new TrainingPlan();      // 患者训练计划
    public List<TrainingPlay> trainingPlays = new List<TrainingPlay>();   // 患者训练列表

    //set PatientName,PatientID
    public void SetPatientMessage(long PatientID, string PatientName)
    {
        this.PatientID = PatientID;
        this.PatientName = PatientName;
    }

    public void setPatientCompleteMessage(long PatientID, string PatientName, string PatientPassword, long PatientDoctorID, long PatientAge, string PatientSex, long PatientHeight, long PatientWeight){
        this.PatientID = PatientID;
        this.PatientName = PatientName;
        this.PatientPassword = PatientPassword;
        this.PatientDoctorID = PatientDoctorID;
        this.PatientAge = PatientAge;
        this.PatientSex = PatientSex;
        this.PatientHeight = PatientHeight;
        this.PatientWeight = PatientWeight;
    }

    public void setPatientPassword(string NewPassword)
    {
        this.PatientPassword = NewPassword;
    }

    public void ModifyPatientInfo(string PatientName, string PatientSex, long PatientAge, long PatientHeight, long PatientWeight)
    {
        this.PatientName = PatientName;
        this.PatientSex = PatientSex;
        this.PatientAge = PatientAge;
        this.PatientHeight = PatientHeight;
        this.PatientWeight = PatientWeight;
    }
}
