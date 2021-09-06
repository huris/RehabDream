using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogout : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ClickYes()
    {
        GameObject.Find("Canvas").transform.Find("Game").gameObject.active = false;
        GameObject.Find("Canvas").transform.Find("Login").gameObject.active = true;
        gameObject.active = false;
    }
    public void ClickNo()
    {
        gameObject.active = false;
    }
}
