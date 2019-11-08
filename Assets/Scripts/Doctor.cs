using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor{

    //[Header("DoctorMessage")]
    public long DoctorID { get; private set; } = 0;
    public string DoctorName { get; private set; } = "DoctorName";
    public string DoctorPassword { get; private set; } = "123456";


    // set DoctorID, DoctorName, DoctorPassword
    public void SetDoctorMessage(long DoctorID, string DoctorPassword, string DoctorName)
    {
        this.DoctorID = DoctorID;
        this.DoctorPassword = DoctorPassword;
        this.DoctorName = DoctorName;
    }
}
