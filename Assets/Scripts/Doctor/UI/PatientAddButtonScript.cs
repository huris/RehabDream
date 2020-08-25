using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PatientAddButtonScript : MonoBehaviour {

    public GameObject PatientAdd;
    public GameObject PatientInfo;
    public GameObject PatientListBG;
    public GameObject PatientModify;

    public GameObject NoPatient;

    public Image PatientAddImage;
    public Image PatientAllImage;
    public Image PatientQueryImage;

    public Sequence seq;
    //public int direction = 10;

    // Use this for initialization
    void OnEnable()
    {
        PatientAdd = transform.parent.Find("PatientAdd").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;
        PatientModify = transform.parent.Find("PatientModify").gameObject;

        NoPatient = transform.parent.Find("NoPatient").gameObject;

        PatientAddImage = transform.GetComponent<Image>();
        PatientAddImage.color = Color.white;

        PatientAllImage = transform.parent.Find("PatientAllButton").GetComponent<Image>();
        PatientAllImage.color = Color.white;

        PatientQueryImage = transform.parent.Find("PatientQueryButton").GetComponent<Image>();
        PatientQueryImage.color = Color.white;

        if (DoctorDataManager.instance.doctor.Patients == null || DoctorDataManager.instance.doctor.Patients.Count == 0)
        {
            Tweener t1 = PatientAddImage.DOColor(new Color(60 / 255, 255 / 255, 60 / 255), 0.5f);
            Tweener t2 = PatientAddImage.DOColor(Color.white, 0.5f);
            seq = DOTween.Sequence();
            seq.Append(t1);
            seq.Append(t2);
            seq.SetLoops(3);
            //PatientAddImage.DOColor(new Color(60 / 255, 255 / 255, 60 / 255), 1).SetLoops(8);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PatientAddButtonOnclick()
    {
        PatientAdd.SetActive(true);
        PatientInfo.SetActive(false);
        PatientListBG.SetActive(false);
        PatientModify.SetActive(false);
        NoPatient.SetActive(false);

        PatientAllImage.color = Color.white;
        PatientQueryImage.color = Color.white;

        seq.Kill();

        PatientAddImage.color = new Color(60 / 255, 255 / 255, 60 / 255);
    }
}
