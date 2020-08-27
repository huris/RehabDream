using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishTrainingInitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FishTrainingStartButtonOnClick()
	{
		SceneManager.LoadScene("Intro");  // 如果登录成功,则进入医生管理界面
	}

}
