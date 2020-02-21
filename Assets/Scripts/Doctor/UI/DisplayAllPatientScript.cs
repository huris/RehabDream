﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAllPatientScript : MonoBehaviour {

    public GameObject PatientList;  // 让PatientList重新激活一下，刷新界面
    
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    public GameObject PatientQuery;
    public GameObject PatientAdd;
    public GameObject PatientModify;

    public Image PatientAddImage;
    public Image PatientAllImage;
    public Image PatientQueryImage;

    public Sequence seq;

    // Use this for initialization
    void Start () {
        PatientList = transform.parent.Find("PatientListBG/PatientList").gameObject;

        PatientQuery = transform.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;

        PatientAdd = transform.parent.Find("PatientAdd").gameObject;
        PatientModify = transform.parent.Find("PatientModify").gameObject;

        PatientAddImage = transform.parent.Find("PatientAddButton").GetComponent<Image>();
        PatientAddImage.color = Color.white;

        PatientAllImage = transform.parent.Find("PatientAllButton").GetComponent<Image>();
        PatientAllImage.color = Color.white;

        PatientQueryImage = transform.parent.Find("PatientQueryButton").GetComponent<Image>();
        PatientQueryImage.color = Color.white;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayAllPatientButonOnClick(){

        DoctorDataManager.instance.doctor.Patients = DoctorDatabaseManager.instance.ReadDoctorPatientInformation(DoctorDataManager.instance.doctor.DoctorID, DoctorDataManager.instance.doctor.DoctorName);

        if (DoctorDataManager.instance.doctor.Patients == null || DoctorDataManager.instance.doctor.Patients.Count == 0)
        {
            PatientAllImage.color = Color.white;
            PatientQueryImage.color = Color.white;

            Tweener t1 = PatientAddImage.DOColor(new Color(60 / 255, 255 / 255, 60 / 255), 0.5f);
            Tweener t2 = PatientAddImage.DOColor(Color.white, 0.5f);
            seq = DOTween.Sequence();
            seq.Append(t1);
            seq.Append(t2);
            seq.SetLoops(-1);
        }
        else
        {
            PatientAddImage.color = Color.white;
            PatientQueryImage.color = Color.white;

            PatientAllImage.color = new Color(60 / 255, 255 / 255, 60 / 255);

        }

        PatientList.SetActive(false);
        PatientList.SetActive(true);

        PatientQuery.SetActive(false);
        PatientAdd.SetActive(false);
        PatientModify.SetActive(false);

        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
