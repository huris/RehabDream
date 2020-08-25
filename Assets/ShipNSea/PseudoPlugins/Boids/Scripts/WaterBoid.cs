using UnityEngine;
using System.Collections;

/// <summary>
/// Boid swimming in the water.
/// </summary>
public class WaterBoid : Boid {
	
	/// <summary>
	/// Tries to avoid obstacles.
	/// </summary>
	/// <param name="c">
	/// A <see cref="Collider"/>
	/// </param>
	void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.transform.root.GetComponentInChildren<Boid>() != null)
		{
			return;
		}
		GetComponent<Rigidbody>().velocity += Vector3.up * GetComponent<Rigidbody>().velocity.magnitude;
		this.LimitVelocity();
    }
	
	/// <summary>
	/// Tries to avoid obstacles.
	/// </summary>
	/// <param name="c">
	/// A <see cref="Collision"/>
	/// </param>
	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.transform.root.GetComponentInChildren<Boid>() != null)
		{
			return;
		}
		GetComponent<Rigidbody>().velocity += Vector3.up * GetComponent<Rigidbody>().velocity.magnitude;
		this.LimitVelocity();
	}
}
