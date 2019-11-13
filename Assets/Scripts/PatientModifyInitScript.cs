using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientModifyInitScript : MonoBehaviour {

    public InputField PatientName;
    public InputField PatientAge;
    public InputField PatientHeight;
    public InputField PatientWeight;

    public Toggle Man;
    public Toggle Woman;
    public string PatientSex;

    public GameObject ErrorInput;
    public GameObject ModifySuccess;

    public GameObject PatientModify;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    // Use this for initialization
    void Start()
    {
    }

    void OnEnable()
    {
        PatientName = transform.Find("ModifyPatientName/InputField").GetComponent<InputField>();
        PatientAge = transform.Find("ModifyPatientAge/InputField").GetComponent<InputField>();
        PatientHeight = transform.Find("ModifyPatientHeight/InputField").GetComponent<InputField>();
        PatientWeight = transform.Find("ModifyPatientWeight/InputField").GetComponent<InputField>();

        Man = transform.Find("ModifyPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.Find("ModifyPatientSex/Woman").GetComponent<Toggle>();
        
        ErrorInput = transform.Find("ErrorInput").gameObject;
        ErrorInput.SetActive(false);
        ModifySuccess = transform.Find("ModifySuccess").gameObject;
        ModifySuccess.SetActive(false);

        PatientModify = transform.parent.Find("PatientModify").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;

        PatientName.text = DoctorDataManager.instance.TempPatient.PatientName;
        PatientAge.text = DoctorDataManager.instance.TempPatient.PatientAge.ToString();
        PatientHeight.text = DoctorDataManager.instance.TempPatient.PatientHeight.ToString();
        PatientWeight.text = DoctorDataManager.instance.TempPatient.PatientWeight.ToString();

        if(DoctorDataManager.instance.patient.PatientSex == "男") { Man.isOn = true; }
        else if(DoctorDataManager.instance.patient.PatientSex == "女") { Woman.isOn = true; }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientModifyButtonScript()
    {
        PatientSex = "";

        if (Man.isOn) PatientSex = "男";
        else if (Woman.isOn) PatientSex = "女";

        if (PatientName.text == "" || PatientSex == "" || PatientAge.text == "" || PatientHeight.text == "" || PatientWeight.text == "") 
        {
            ErrorInput.SetActive(true);
        }
        DoctorDataManager.instance.TempPatient.ModifyPatientInfo(PatientName.text, PatientSex, long.Parse(PatientAge.text), long.Parse(PatientHeight.text), long.Parse(PatientWeight.text));
        DoctorDataManager.instance.Patients[DoctorDataManager.instance.TempPatientIndex] = DoctorDataManager.instance.TempPatient;

        DoctorDatabaseManager.instance.PatientModify(DoctorDataManager.instance.TempPatient);  // 修改数据库

        ErrorInput.SetActive(false);
        ModifySuccess.SetActive(true);

        StartCoroutine(DelayTime(3));
    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        PatientModify.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
