using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManager : MonoBehaviour {
    public GameObject model;
    public float limitTime=5f;
    float time = 0;
    public static bool isCheckTime = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isCheckTime)
        {
            model.active = false;
            time = 0;
        }
        else
        {
            if (KinectManager.Instance.IsUserDetected() == true)
            {
                model.active = true;
                time = 0;
            }
            else
            {
                model.active = false;
                time += Time.deltaTime;

            }

            if (time >= limitTime)
            {
                GameObject go = GameObject.Find("Canvas");
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    go.transform.GetChild(i).gameObject.active = false;
                }
                go.transform.Find("Login").gameObject.active = true;
            }
        }
        
		
	}
}
