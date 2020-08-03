using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportDataChooseScript : MonoBehaviour {

	public Toggle WallEvaluationToggle;
	public Toggle EvaluationToggle;
	public Toggle TrainingToggle;

	public Dropdown FirstItem;
	public Dropdown SecondItem;
	public List<string> FirstListEvaluationTime = new List<string>();

    public GameObject EvaluationGameobject;
    public GameObject TrainingGameobject;

	void OnEnable()
	{
        if (WallEvaluationToggle.isOn)
        {
            FirstItem.gameObject.SetActive(false);
            SecondItem.gameObject.SetActive(false);
        }
        else if (EvaluationToggle.isOn)
        {
            if (DoctorDataManager.instance.doctor.patient.Evaluations == null)
            {
                DoctorDataManager.instance.doctor.patient.Evaluations = DoctorDatabaseManager.instance.ReadPatientEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

                if (DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
                {
                    DoctorDataManager.instance.doctor.patient.SetEvaluationIndex(DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1);
                }
            }

            if (DoctorDataManager.instance.doctor.patient.Evaluations.Count == 1)
            {
                FirstItem.gameObject.SetActive(false);
                SecondItem.gameObject.SetActive(false);
            }
            else
            {
                ClearDropdown();
                for (int i = 0; i < DoctorDataManager.instance.doctor.patient.Evaluations.Count; i++)
                {
                    string tempEvaluationTime = DoctorDataManager.instance.doctor.patient.Evaluations[i].EvaluationStartTime;
                    FirstListEvaluationTime.Add((i + 1).ToString() + "|" + tempEvaluationTime.Substring(4, 2) + tempEvaluationTime.Substring(6, 2));
                }
                FirstItem.AddOptions(FirstListEvaluationTime);
                SecondItem.AddOptions(FirstListEvaluationTime);
                FirstListEvaluationTime.Clear();

                FirstItem.value = 0;

                SecondItem.value = DoctorDataManager.instance.doctor.patient.EvaluationIndex;
            }
        }
        else if (TrainingToggle.isOn)
        {
            ClearDropdown();

            if (DoctorDataManager.instance.doctor.patient.TrainingPlays == null)
            {
                DoctorDataManager.instance.doctor.patient.TrainingPlays = DoctorDatabaseManager.instance.ReadPatientRecord(DoctorDataManager.instance.doctor.patient.PatientID, 0);
                if (DoctorDataManager.instance.doctor.patient.TrainingPlays != null && DoctorDataManager.instance.doctor.patient.TrainingPlays.Count > 0)
                {
                    foreach (var item in DoctorDataManager.instance.doctor.patient.TrainingPlays)
                    {
                        if (DoctorDataManager.instance.doctor.patient.MaxSuccessCount < item.SuccessCount)
                        {
                            DoctorDataManager.instance.doctor.patient.SetMaxSuccessCount(item.SuccessCount);
                        }
                    }

                    DoctorDataManager.instance.doctor.patient.SetTrainingPlayIndex(DoctorDataManager.instance.doctor.patient.TrainingPlays.Count - 1);
                    //print(DoctorDataManager.instance.doctor.patient.TrainingPlayIndex);
                }
            }

            if (DoctorDataManager.instance.doctor.patient.TrainingPlays.Count == 1)
            {
                FirstItem.gameObject.SetActive(false);
                SecondItem.gameObject.SetActive(false);
            }
            else
            {
                ClearDropdown();
                for (int i = 0; i < DoctorDataManager.instance.doctor.patient.TrainingPlays.Count; i++)
                {
                    string tempTrainingTime = DoctorDataManager.instance.doctor.patient.TrainingPlays[i].TrainingStartTime;
                    FirstListEvaluationTime.Add((i + 1).ToString() + "|" + tempTrainingTime.Substring(4, 2) + "." + tempTrainingTime.Substring(6, 2));
                }
                FirstItem.AddOptions(FirstListEvaluationTime);
                SecondItem.AddOptions(FirstListEvaluationTime);
                FirstListEvaluationTime.Clear();

                FirstItem.value = 0;

                SecondItem.value = DoctorDataManager.instance.doctor.patient.TrainingPlayIndex;
            }
        }
    }

    public void EvaluationToggleChanged()
    {
        EvaluationGameobject.SetActive(false);
        EvaluationGameobject.SetActive(true);
    }

    public void TrainingToggleChanged()
    {
        TrainingGameobject.SetActive(false);
        TrainingGameobject.SetActive(true);
    }


    public void ClearDropdown()
    {
        FirstItem.gameObject.SetActive(true);
        SecondItem.gameObject.SetActive(true);

        FirstItem.ClearOptions();
        SecondItem.ClearOptions();

        FirstListEvaluationTime.Clear();
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
