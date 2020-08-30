using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void clickLogout()
    {
        for (int i=0;i<transform.root.childCount;i++)
        {
            transform.root.GetChild(i).gameObject.active = false;
        }
        transform.root.Find("Login").gameObject.active = true;
    }
    public void clickTransToChildSystem()
    {
        
        SceneManager.LoadScene("childsystem");
        
    }
    public void clickQuit()
    {
        Application.Quit();
    }
}
