using NPinyin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patient
{
    public string PatientName { get; private set; } = "PatientName";
    public long PatientID { get; private set; } = 0;
    public long PatientDoctorID { get; private set; } = 0;
    public string PatientDoctorName { get; private set; } = "PatientDoctorName";
    public string PatientSymptom { get; private set; } = "Symptom";
    public long PatientAge { get; private set; } = 0;
    public string PatientSex { get; private set; } = "男";
    public long PatientHeight { get; private set; } = 0;
    public long PatientWeight { get; private set; } = 0;
    public string PatientPinyin { get; private set; } = "";
    public long MaxSuccessCount { get; private set; } = 0;
    public bool PlanIsMaking { get; private set; } = false;

    public TrainingPlan trainingPlan = null;      // 患者训练计划
    public List<TrainingPlay> trainingPlays = null;   // 患者训练列表
    public List<TrainingPlay> Evaluations = null;   // 患者评估列表

    public Patient() { }

    public Patient(long PatientID, string PatientName, string PatientSymptom, long PatientDoctorID, string PatientDoctorName, long PatientAge, string PatientSex, long PatientHeight, long PatientWeight)
    {
        this.PatientID = PatientID;
        this.PatientName = PatientName;
        this.PatientSymptom = PatientSymptom;
        this.PatientDoctorID = PatientDoctorID;
        this.PatientDoctorName = PatientDoctorName;
        this.PatientAge = PatientAge;
        this.PatientSex = PatientSex;
        this.PatientHeight = PatientHeight;
        this.PatientWeight = PatientWeight;
        this.PatientPinyin = Pinyin.GetPinyin(PatientName);
    }

    public void SetPatientData()
    {
        if (this.trainingPlan == null) this.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(this.PatientID);
        else this.PlanIsMaking = true;

        if(this.trainingPlays == null) this.trainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(this.PatientID, 0);
        else
        {
            foreach (var item in this.trainingPlays)
            {
                if (this.MaxSuccessCount < item.SuccessCount)
                {
                    this.MaxSuccessCount = item.SuccessCount;
                }
            }
        }

        if(this.Evaluations == null) this.Evaluations = DoctorDatabaseManager.instance.ReadPatientRecord(this.PatientID, 1);        
    }

    public void SetPatientPinyin(string PatientPinyin)
    {
        this.PatientPinyin = PatientPinyin;
    }

    //set PatientName,PatientID
    public void SetPatientMessage(long PatientID, string PatientName)
    {
        this.PatientID = PatientID;
        this.PatientName = PatientName;
    }

    public void setPatientCompleteMessage(long PatientID, string PatientName, string PatientSymptom, long PatientDoctorID, string PatientDoctorName,long PatientAge, string PatientSex, long PatientHeight, long PatientWeight){
        this.PatientID = PatientID;
        this.PatientName = PatientName;
        this.PatientSymptom = PatientSymptom;
        this.PatientDoctorID = PatientDoctorID;
        this.PatientDoctorName = PatientDoctorName;
        this.PatientAge = PatientAge;
        this.PatientSex = PatientSex;
        this.PatientHeight = PatientHeight;
        this.PatientWeight = PatientWeight;
        this.PatientPinyin = Pinyin.GetPinyin(PatientName);
    }
    public void setPatientCompleteMessage(Patient patient)
    {
        this.PatientID = patient.PatientID;
        this.PatientName = patient.PatientName;
        this.PatientSymptom = patient.PatientSymptom;
        this.PatientDoctorID = patient.PatientDoctorID;
        this.PatientDoctorName = patient.PatientDoctorName;
        this.PatientAge = patient.PatientAge;
        this.PatientSex = patient.PatientSex;
        this.PatientHeight = patient.PatientHeight;
        this.PatientWeight = patient.PatientWeight;
        this.PatientPinyin = Pinyin.GetPinyin(PatientName);
        this.MaxSuccessCount = patient.MaxSuccessCount;

        this.trainingPlan = patient.trainingPlan;
        this.trainingPlays = patient.trainingPlays;
        this.Evaluations = patient.Evaluations;

        this.PlanIsMaking = patient.PlanIsMaking;
    }

    public void setPatientSymptom(string PatientSymptom)
    {
        this.PatientSymptom = PatientSymptom;
    }

    public void ModifyPatientInfo(string PatientName, string PatientSex, long PatientAge, long PatientHeight, long PatientWeight)
    {
        this.PatientName = PatientName;
        this.PatientSex = PatientSex;
        this.PatientAge = PatientAge;
        this.PatientHeight = PatientHeight;
        this.PatientWeight = PatientWeight;
    }

    public void ModifyPatientInfo(string PatientName, string PatientSex, long PatientAge, long PatientHeight, long PatientWeight, string PatientSymptom)
    {
        this.PatientName = PatientName;
        this.PatientSex = PatientSex;
        this.PatientAge = PatientAge;
        this.PatientHeight = PatientHeight;
        this.PatientWeight = PatientWeight;
        this.PatientSymptom = PatientSymptom;
    }
    public void ModifyPatientInfo(string PatientName, string PatientSex, long PatientAge, long PatientHeight, long PatientWeight, string PatientSymptom, long PatientDoctorID)
    {
        this.PatientName = PatientName;
        this.PatientSex = PatientSex;
        this.PatientAge = PatientAge;
        this.PatientHeight = PatientHeight;
        this.PatientWeight = PatientWeight;
        this.PatientSymptom = PatientSymptom;
        this.PatientDoctorID = PatientDoctorID;
    }

    public void SetPlanIsMaking(bool PlanIsMaking)
    {
        this.PlanIsMaking = PlanIsMaking;
    }
}
