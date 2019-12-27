using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PatientModifyInitScript : MonoBehaviour {

    public InputField PatientName;
    public InputField PatientAge;
    public InputField PatientHeight;
    public InputField PatientWeight;
    public InputField PatientSymptom;
    public Dropdown PatientDoctor;

    public Toggle Man;
    public Toggle Woman;
    public string PatientSex;

    public GameObject ErrorInput;
    public GameObject ModifySuccess;

    public GameObject PatientModify;
    public GameObject PatientInfo;
    public GameObject PatientListBG;

    public Dictionary<string, int> DoctorString2Int;
    public Dictionary<int, string> DoctorInt2String;

    public List<string> PatientDoctorName;

    public int PatientDoctorIndex;

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
        PatientSymptom = transform.Find("ModifyPatientSymptom/InputField").GetComponent<InputField>();
        PatientDoctor = transform.parent.Find("ModifyPatientDoctor/Dropdown").GetComponent<Dropdown>();

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

        DoctorDataManager.instance.Doctors = DoctorDataManager.instance.Doctors.OrderBy(s => s.DoctorPinyin).ToList();

        DoctorString2Int = new Dictionary<string, int>();
        DoctorInt2String = new Dictionary<int, string>();
        PatientDoctorName = new List<string>();
        
        for (int i = 0; i < DoctorDataManager.instance.Doctors.Count; i++)
        {
            DoctorString2Int.Add(DoctorDataManager.instance.Doctors[i].DoctorName, i);
            DoctorInt2String.Add(i, DoctorDataManager.instance.Doctors[i].DoctorName);
            PatientDoctorName.Add(DoctorDataManager.instance.Doctors[i].DoctorName);
            
            if(DoctorDataManager.instance.Doctors[i].DoctorID == DoctorDataManager.instance.patient.PatientDoctorID)
            {
                PatientDoctorIndex = i;
            }
        }

        PatientDoctor.AddOptions(PatientDoctorName);

        PatientDoctor.value = PatientDoctorIndex;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PatientModifyButtonScript()
    {
        ErrorInput.SetActive(false);
        ModifySuccess.SetActive(false);

        PatientSex = "";
        if (Man.isOn) PatientSex = "男";
        else if (Woman.isOn) PatientSex = "女";

        if (PatientName.text == "" || PatientSex == "" || PatientAge.text == "" || PatientSymptom.text == "") 
        {
            ErrorInput.SetActive(true);
        }
        else
        {
            if (PatientHeight.text == "")
            {
                PatientHeight.text = "-1";
            }

            if (PatientWeight.text == "")
            {
                PatientWeight.text = "-1";
            }

            DoctorDataManager.instance.TempPatient.ModifyPatientInfo(PatientName.text, PatientSex, long.Parse(PatientAge.text), long.Parse(PatientHeight.text), long.Parse(PatientWeight.text), PatientSymptom.text, DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorID);
            DoctorDataManager.instance.Patients[DoctorDataManager.instance.TempPatientIndex] = DoctorDataManager.instance.TempPatient;

            DoctorDatabaseManager.instance.PatientModify(DoctorDataManager.instance.TempPatient);  // 修改数据库

            ErrorInput.SetActive(false);
            ModifySuccess.SetActive(true);

            StartCoroutine(DelayTime(3));
        }
    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        PatientModify.SetActive(false);
        PatientInfo.SetActive(true);
        PatientListBG.SetActive(true);
    }
}
