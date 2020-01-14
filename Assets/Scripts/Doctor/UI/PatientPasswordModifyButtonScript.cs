using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PatientSymptomModifyButtonScript : MonoBehaviour {

    public Text PatientName;
    public InputField NewPassword;
    public InputField NewPasswordAgain;

    public GameObject ModifyError;
    public GameObject ModifySuccess;
    public GameObject ModifyNull;

    public GameObject PatientSymptomModify;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void OnEnable () {

        PatientName = transform.parent.Find("ModifyPatientName/PatientName").GetComponent<Text>();
        PatientName.text = DoctorDataManager.instance.doctor.patient.PatientName;

        NewPassword = transform.parent.Find("NewPassword/InputField").GetComponent<InputField>();
        NewPassword.text = "";

        NewPasswordAgain = transform.parent.Find("NewPasswordAgain/InputField").GetComponent<InputField>();
        NewPasswordAgain.text = "";

        ModifyError = transform.parent.Find("PatientSymptomModifyError").gameObject;
        ModifyNull = transform.parent.Find("PatientSymptomModifyNull").gameObject;
        ModifySuccess = transform.parent.Find("PatientSymptomModifySuccess").gameObject;

        ModifyError.SetActive(false);
        ModifyNull.SetActive(false);
        ModifySuccess.SetActive(false);

        PatientSymptomModify = transform.parent.parent.Find("PatientSymptomModify").gameObject;
        PatientInfo = transform.parent.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.parent.Find("PatientListBG").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PatientSymptomModifyButtonOnClick()
    {
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

            // DoctorDataManager.instance.patient.setPatientPassword(MD5Encrypt(NewPassword.text));

            DoctorDatabaseManager.instance.PatientModify(DoctorDataManager.instance.doctor.patient);

            StartCoroutine(DelayTime(3));     // 延迟三秒后返回管理页面
        }

    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        PatientSymptomModify.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }

    /// <summary>
    /// 用MD5加密字符串
    /// </summary>
    /// <param name="password">待加密的字符串</param>
    /// <returns></returns>
    public string MD5Encrypt(string password)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes;
        hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));
        }
        return tmp.ToString();
    }
}
