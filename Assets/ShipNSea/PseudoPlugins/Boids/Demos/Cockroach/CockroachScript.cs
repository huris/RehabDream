using UnityEngine;
using System.Collections;

public class CockroachScript : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (this.GetComponent<Animation>() != null)
		{
			foreach (AnimationState state in GetComponent<Animation>())
			{
				float randomspeed = Random.Range(15.0f, 25.0f);
		    	state.speed = randomspeed;
			}
			GetComponent<Animation>().Play(GetComponent<Animation>().clip.name);
		}
	}
}
