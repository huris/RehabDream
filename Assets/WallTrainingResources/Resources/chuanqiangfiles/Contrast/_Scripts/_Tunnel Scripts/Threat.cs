using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attached to each threat object to define its colour.
/// </summary>
public class Threat : MonoBehaviour {

    //The available materials for threats.
    public Material[] threatMaterials = new Material[2];

	// Use this for initialization
	void Awake ()
    {
        Init();
    }

    //Set up this threat ready to be used in the game environment.
    void Init()
    {
        //Get the Renderer for this object.
        Renderer thisRenderer = GetComponent<Renderer>();
        thisRenderer.sharedMaterial = threatMaterials[Random.Range(0, threatMaterials.Length)];
    }
}
