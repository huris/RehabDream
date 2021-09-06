using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerballCreate : MonoBehaviour {

    public GameObject Soccerball;   //a new soccer
    public Transform SoccerRoot;    //root node of soccer


    void Awake()
    {
        if (Soccerball == null)
        {
            GenerateSoccerball();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DestorySoccerball()
    {
        Destroy(Soccerball);
    }

    public void GenerateSoccerball()
    {
        
    }

}
