using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientPasswordModifyButtonScript : MonoBehaviour {

    public Text PatientName;
    public Text NewPassword;
    public Text NewPasswordAgain;

    public GameObject ModifyError;
    public GameObject ModifySuccess;
    public GameObject ModifyNull;

    public GameObject PatientPasswordModify;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void OnEnable () {

        PatientName = transform.parent.Find("ModifyPatientName/PatientName").GetComponent<Text>();
        PatientName.text = DoctorDataManager.instance.patient.PatientName;

        print(transform.parent.name);

        NewPassword = transform.parent.Find("NewPassword/InputField").GetComponent<Text>();
        NewPasswordAgain = transform.parent.Find("NewPasswordAgain/InputField").GetComponent<Text>();

        ModifyError = transform.parent.Find("PatientPasswordModifyError").gameObject;
        ModifyNull = transform.parent.Find("PatientPasswordModifyNull").gameObject;
        ModifySuccess = transform.parent.Find("PatientPasswordModifySuccess").gameObject;

        ModifyError.SetActive(false);
        ModifyNull.SetActive(false);
        ModifySuccess.SetActive(false);

        PatientPasswordModify = transform.parent.parent.Find("PatientPasswordModify").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PatientPasswordModifyButtonOnClick()
    {
        print(NewPassword.text);
        print(NewPasswordAgain.text);
        if (NewPassword.text == "" || NewPasswordAgain.text == "")
        {
            ModifyError.SetActive(false) ;
            ModifyNull.SetActive(true);
        }
        else if (NewPassword.text != NewPasswordAgain.text)
        {
            NewPassword.text = NewPasswordAgain.text = "";
            ModifyNull.SetActive(false);
            ModifyError.SetActive(true);
        }
        else if(NewPassword.text == NewPasswordAgain.text)
        {
            ModifyNull.SetActive(false);
            ModifyError.SetActive(false);
            ModifySuccess.SetActive(true);

            DoctorDataManager.instance.patient.setPatientPassword(NewPassword.text);

            DoctorDatabaseManager.instance.PatientModify(DoctorDataManager.instance.patient);

            StartCoroutine(DelayTime(3));     // 延迟三秒后返回管理页面
        }

    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        PatientPasswordModify.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
