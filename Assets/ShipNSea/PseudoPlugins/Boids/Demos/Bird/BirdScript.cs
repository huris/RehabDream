using UnityEngine;
using System.Collections;

public class BirdScript : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (this.GetComponent<Animation>() != null)
		{
			foreach (AnimationState state in GetComponent<Animation>())
			{
				float randomspeed = Random.Range(10.0f, 20.0f);
		    	state.speed = randomspeed;
			}
			GetComponent<Animation>().Play(GetComponent<Animation>().clip.name);
		}
	}
}
