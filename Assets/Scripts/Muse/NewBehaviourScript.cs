using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muse;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log(1);
		OSCMonitor o = new OSCMonitor(5000, (MuseMessage m)=> { });
        Debug.Log(o.port);

        o.StartUDPLstener();

        Debug.Log(2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
