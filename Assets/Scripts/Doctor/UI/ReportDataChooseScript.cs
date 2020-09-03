using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportDataChooseScript : MonoBehaviour {

	public Toggle WallEvaluationToggle;
	public Toggle EvaluationToggle;
	public Toggle TrainingToggle;
    public Toggle FishTrainingToggle;

	public Dropdown FirstItem;
	public Dropdown SecondItem;
	public List<string> FirstListEvaluationTime = new List<string>();

    public GameObject EvaluationGameobject;
    public GameObject TrainingGameobject;

    public GameObject Evaluations;
    public GameObject Trainings;

    public Toggle EvaluationsToggle;
    public Toggle TrainingsToggle;

    public void EvaluationsToggleChanged()
    {
        if (EvaluationsToggle.isOn)
        {
            TrainingToggle.isOn = false;
            FishTrainingToggle.isOn = false;

            Evaluations.SetActive(true);
            WallEvaluationToggle.isOn = true;
            //EvaluationTrainingToggleChanged();

            Trainings.SetActive(false);

            //EvaluationTrainingToggleChanged();
        }
        else
        {
            WallEvaluationToggle.isOn = false;
            EvaluationToggle.isOn = false;

            //EvaluationTrainingToggleChanged();
            Trainings.SetActive(true);
            TrainingToggle.isOn = true;

            Evaluations.SetActive(false);
            //EvaluationTrainingToggleChanged();
        }
    }

    void OnEnable()
    {
    }

    public void EvaluationTrainingToggleChanged()
    {

        if (WallEvaluationToggle.isOn)
        {
            if (DoctorDataManager.instance.doctor.patient.WallEvaluations == null)
            {
                DoctorDataManager.instance.doctor.patient.WallEvaluations = DoctorDatabaseManager.instance.ReadPatientWallEvaluations(DoctorDataManager.instance.doctor.patient.PatientID);

                if (DoctorDataManager.instance.doctor.patient.WallEvaluations != null && DoctorDataManager.instance.doctor.patient.WallEvaluations.Count > 0)
                {
                    DoctorDataManager.instance.doctor.patient.SetWallEvaluationIndex(DoctorDataManager.instance.doctor.patient.WallEvaluations.Count - 1);
                }
            }

            if (DoctorDataManager.instance.doctor.patient.Evaluations.Count == 1)
            {
                FirstItem.value = 0;
                SecondItem.value = 0;

                FirstItem.gameObject.SetActive(false);
                SecondItem.gameObject.SetActive(false);
            }
            else
            {
                ClearDropdown();
                for (int i = 0; i < DoctorDataManager.instance.doctor.patient.WallEvaluations.Count; i++)
                {
                    string tempEvaluationTime = DoctorDataManager.instance.doctor.patient.WallEvaluations[i].startTime;
                    FirstListEvaluationTime.Add((i + 1).ToString() + "|" + tempEvaluationTime.Substring(3, 2) + tempEvaluationTime.Substring(6, 2));
                }
                FirstItem.AddOptions(FirstListEvaluationTime);
                SecondItem.AddOptions(FirstListEvaluationTime);
                FirstListEvaluationTime.Clear();

                FirstItem.value = 0;

                SecondItem.value = DoctorDataManager.instance.doctor.patient.WallEvaluationIndex;

                //print(FirstItem.value + "  "  + SecondItem.value);
            }
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
                FirstItem.value = 0;
                SecondItem.value = 0;

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

                //print(FirstItem.value + "  "  + SecondItem.value);

            }
        }
        else if (TrainingToggle.isOn)
        {

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
                FirstItem.value = 0;
                SecondItem.value = 0;

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
        else if (FishTrainingToggle.isOn)
        {
            FirstItem.gameObject.SetActive(false);
            SecondItem.gameObject.SetActive(false);
        }
    }

    public void ItemToggleChanged()
    {
        if (EvaluationToggle.isOn)
        {
            EvaluationGameobject.SetActive(false);
            EvaluationGameobject.SetActive(true);
        }
        else if (TrainingToggle.isOn)
        {
            TrainingGameobject.SetActive(false);
            TrainingGameobject.SetActive(true);
        }
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
