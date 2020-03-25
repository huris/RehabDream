using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public Dictionary<string, int> DoctorString2Int = new Dictionary<string, int>();
    public Dictionary<int, string> DoctorInt2String = new Dictionary<int, string>();

    public List<string> PatientDoctorName = new List<string>();

    public int PatientDoctorIndex;

    public EventSystem system;

    private Selectable SelecInput;   // 当前焦点所处的Input
    private Selectable NextInput;   // 目标Input

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
        PatientDoctor = transform.Find("ModifyPatientDoctor/Dropdown").GetComponent<Dropdown>();

        Man = transform.Find("ModifyPatientSex/Man").GetComponent<Toggle>();
        Woman = transform.Find("ModifyPatientSex/Woman").GetComponent<Toggle>();
        
        ErrorInput = transform.Find("ErrorInput").gameObject;
        ErrorInput.SetActive(false);
        ModifySuccess = transform.Find("ModifySuccess").gameObject;
        ModifySuccess.SetActive(false);

        PatientModify = transform.parent.Find("PatientModify").gameObject;
        PatientInfo = transform.parent.Find("PatientInfo").gameObject;
        PatientListBG = transform.parent.Find("PatientListBG").gameObject;

        PatientName.text = DoctorDataManager.instance.doctor.TempPatient.PatientName;
        PatientAge.text = DoctorDataManager.instance.doctor.TempPatient.PatientAge.ToString();
        PatientHeight.text = DoctorDataManager.instance.doctor.TempPatient.PatientHeight.ToString();
        PatientWeight.text = DoctorDataManager.instance.doctor.TempPatient.PatientWeight.ToString();
        PatientSymptom.text = DoctorDataManager.instance.doctor.TempPatient.PatientSymptom;

        if(DoctorDataManager.instance.doctor.TempPatient.PatientSex == "男") { Man.isOn = true; Woman.isOn = false; }
        else if(DoctorDataManager.instance.doctor.TempPatient.PatientSex == "女") { Woman.isOn = true; Man.isOn = false; }

        // DoctorDataManager.instance.Doctors = DoctorDataManager.instance.Doctors.OrderBy(s => s.DoctorPinyin).ToList();

        if(DoctorDataManager.instance.Doctors != null && DoctorDataManager.instance.Doctors.Count > 0)
        {
            DoctorString2Int.Clear();
            DoctorInt2String.Clear();
            PatientDoctorName.Clear();
            PatientDoctor.ClearOptions();

            for (int i = 0; i < DoctorDataManager.instance.Doctors.Count; i++)
            {
                DoctorString2Int.Add(DoctorDataManager.instance.Doctors[i].DoctorName, i);
                DoctorInt2String.Add(i, DoctorDataManager.instance.Doctors[i].DoctorName);
                PatientDoctorName.Add(DoctorDataManager.instance.Doctors[i].DoctorName);

                if (DoctorDataManager.instance.Doctors[i].DoctorID == DoctorDataManager.instance.doctor.patient.PatientDoctorID)
                {
                    PatientDoctorIndex = i;
                }
            }

            PatientDoctor.AddOptions(PatientDoctorName);

            PatientDoctor.value = PatientDoctorIndex;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //在Update内监听Tap键的按下
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //是否聚焦Input
            if (system.currentSelectedGameObject != null)
            {
                //获取当前选中的Input
                SelecInput = system.currentSelectedGameObject.GetComponent<Selectable>();
                //监听Shift
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    //Shift按下则选择出去上方的Input
                    NextInput = SelecInput.FindSelectableOnUp();
                    //上边没有找左边的
                    if (NextInput == null) NextInput = SelecInput.FindSelectableOnLeft();
                }
                else
                {
                    //没按shift就找下边的Input
                    NextInput = SelecInput.FindSelectableOnDown();
                    //或者右边的
                    if (NextInput == null) NextInput = SelecInput.FindSelectableOnRight();
                }
            }

            //下一个Input不空的话就聚焦
            if (NextInput != null) NextInput.Select();
        }

        // 按回车键进行登录
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PatientModifyButtonScript();
        }
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

            DoctorDataManager.instance.doctor.TempPatient.ModifyPatientInfo(PatientName.text, PatientSex, long.Parse(PatientAge.text), long.Parse(PatientHeight.text), long.Parse(PatientWeight.text), PatientSymptom.text, DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorID);

            if(DoctorDataManager.instance.Doctors[PatientDoctor.value].DoctorID != DoctorDataManager.instance.doctor.DoctorID)
            {
                DoctorDataManager.instance.doctor.Patients.Remove(DoctorDataManager.instance.doctor.TempPatient);
                
                if(DoctorDataManager.instance.doctor.PatientIndex == DoctorDataManager.instance.doctor.TempPatientIndex)
                {
                    DoctorDataManager.instance.doctor.patient = DoctorDataManager.instance.doctor.Patients[0];
                    //DoctorDataManager.instance.doctor.patient.SetPatientData();
                }
            }

            //DoctorDataManager.instance.doctor.Patients[DoctorDataManager.instance.doctor.TempPatientIndex] = DoctorDataManager.instance.doctor.TempPatient;
            //if(DoctorDataManager.instance.doctor.PatientIndex == DoctorDataManager.instance.doctor.TempPatientIndex)
            //{
            //    DoctorDataManager.instance.doctor.patient = DoctorDataManager.instance.doctor.TempPatient;
            //}

            DoctorDatabaseManager.instance.PatientModify(DoctorDataManager.instance.doctor.TempPatient);  // 修改数据库

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
