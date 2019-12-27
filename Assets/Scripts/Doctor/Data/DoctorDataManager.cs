using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoctorDataManager : MonoBehaviour {

    // Singleton instance holder
    public static DoctorDataManager instance = null;

    //[Header("PatientMessage")]
    public Patient patient = new Patient();
    public int PatientIndex = 0;

    public Patient TempPatient = new Patient();
    public int TempPatientIndex = 0;

    // PatientID, PatientName, PatientPassword, DoctorID, PatientAge, PatientSex, PatientHeight, PatientWeight
    public List<Patient> Patients = new List<Patient>();

    public Doctor doctor = new Doctor();
    public List<Doctor>Doctors = new List<Doctor>();


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

    // 对单个患者进行赋值
    public void SetPatientCompleteInformation(int PatientIndex)
    {
        this.patient = this.Patients[PatientIndex];
 
        this.PatientIndex = PatientIndex;

        this.patient.trainingPlan = DoctorDatabaseManager.instance.ReadPatientTrainingPlan(this.patient.PatientID);
        this.patient.trainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(this.patient.PatientID);
    }

}
