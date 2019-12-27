using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor{

    //[Header("DoctorMessage")]
    public long DoctorID { get; private set; } = 0;
    public string DoctorName { get; private set; } = "DoctorName";
    public string DoctorPassword { get; private set; } = "123456";
    public string DoctorPinyin { get; private set; } = "DoctorPinyin";


    // set DoctorID, DoctorName, DoctorPassword
    public void SetDoctorMessage(long DoctorID, string DoctorPassword, string DoctorName)
    {
        this.DoctorID = DoctorID;
        this.DoctorPassword = DoctorPassword;
        this.DoctorName = DoctorName;
    }

    public void SetDoctorPinyin(string DoctorPinyin)
    {
        this.DoctorPinyin = DoctorPinyin;
    }

    public void SetDoctorPassword(string DoctorPassword)
    {
        this.DoctorPassword = DoctorPassword;
    }
}
