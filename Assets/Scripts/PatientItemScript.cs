using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientItemScript : MonoBehaviour {

    public GameObject Prefab;

    void OnEnable()
    {
        print(DoctorDataManager.instance.Patients.Count);

        if(this.transform.childCount > DoctorDataManager.instance.Patients.Count)   // 如果数目大于患者，说明足够存储了，需要把之后的几个给设置未激活
        {
            for (int i=this.transform.childCount-1;i>= DoctorDataManager.instance.Patients.Count; i--)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else   // 否则说明数目不够，需要再生成几个预制体
        {
            for(int i=this.transform.childCount;i< DoctorDataManager.instance.Patients.Count; i++)
            {
                Prefab = Resources.Load("Prefabs/PatientItem") as GameObject;
                Instantiate(Prefab).transform.SetParent(this.transform);
            }
        }
        // 在将列表中的内容放入
        for (int i = 0; i < DoctorDataManager.instance.Patients.Count; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);  // 要设置激活状态
            
            this.transform.GetChild(i).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientName;
            this.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientSex;
            this.transform.GetChild(i).GetChild(0).GetChild(2).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientAge.ToString();
            this.transform.GetChild(i).GetChild(0).GetChild(4).gameObject.GetComponent<Text>().text = DoctorDataManager.instance.Patients[i].PatientID.ToString();
        }

        if (DoctorDataManager.instance.Patients.Count != 0)
        {
            DoctorDataManager.instance.patient = DoctorDataManager.instance.Patients[0];
        }

        //name = childGame.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        //childGame.GetChild(0).GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(OnButtonClick);

        ////获取到父物体，设置为父物体的子物体


    }
	void Update () {
      
    }
    void OnButtonClick()
    {
       
    }
}
