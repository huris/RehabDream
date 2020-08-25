﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientQueryButtonScript : MonoBehaviour {

    public GameObject PatientQuery;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientAdd;
    public GameObject PatientModify;

    public InputField PatientName;
    public string PatientSex;
    public InputField PatientAge;
    public Toggle Man;
    public Toggle Woman;

    public Image PatientAddImage;
    public Image PatientAllImage;
    public Image PatientQueryImage;

    public Sequence seq;

    // Use this for initialization
    void OnEnable () {
        PatientQuery = transform.parent.Find("PatientQuery").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;
        PatientAdd = transform.parent.Find("PatientAdd").gameObject;
        PatientModify = transform.parent.Find("PatientModify").gameObject;

        PatientName = transform.parent.Find("PatientQuery/QueryPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.parent.Find("PatientQuery/QueryPatientAge/InputField").GetComponent<InputField>();
        Man = transform.parent.Find("PatientQuery/QueryPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.parent.Find("PatientQuery/QueryPatientSex/Woman").GetComponent<Toggle>();

        PatientName.text = "";
        PatientAge.text = "";
        Man.isOn = false;
        Woman.isOn = false;

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

    public void PatientQueryButtonOnclick()
    {
        if(DoctorDataManager.instance.doctor.Patients == null || DoctorDataManager.instance.doctor.Patients.Count == 0)
        {
            PatientQueryImage.color = Color.white;
            PatientAllImage.color = Color.white;

            PatientQuery.SetActive(false);
            PatientAdd.SetActive(false);
            PatientModify.SetActive(false);

            PatientInfo.SetActive(true);
            PatientListBG.SetActive(true);

            Tweener t1 = PatientAddImage.DOColor(new Color(60 / 255, 255 / 255, 60 / 255), 0.5f);
            Tweener t2 = PatientAddImage.DOColor(Color.white, 0.5f);
            seq = DOTween.Sequence();
            seq.Append(t1);
            seq.Append(t2);
            seq.SetLoops(3);
        }
        else
        {
            PatientQuery.SetActive(true);
            PatientInfo.SetActive(false);
            PatientListBG.SetActive(false);
            PatientAdd.SetActive(false);
            PatientModify.SetActive(false);

            PatientAddImage.color = Color.white;
            PatientAllImage.color = Color.white;

            PatientQueryImage.color = new Color(60 / 255, 255 / 255, 60 / 255);
        }
    }

}
