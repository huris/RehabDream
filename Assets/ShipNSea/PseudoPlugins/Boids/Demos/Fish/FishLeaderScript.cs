using UnityEngine;
using System.Collections;

/// <summary>
/// Moves Flock leader.
/// </summary>
public class FishLeaderScript : MonoBehaviour {
	
	public Vector3 home = Vector3.zero;
	public Vector3 halfaway = Vector3.zero;
	public Vector3 away = Vector3.zero;
	
	internal float timeDelta = 0.1f;
	internal float speed = 20.0f;
	
	/// <summary>
	/// Start main routine.
	/// </summary>
	void Start () {
		StartCoroutine(MainLoop());
	}
	
	/// <summary>
	/// Main loop, which controls leader's movement.
	/// </summary>
	/// <returns>
	/// A <see cref="IEnumerator"/>
	/// </returns>
	IEnumerator MainLoop()
	{
		while(true)
		{
			yield return StartCoroutine(SwimToTarget(this.halfaway));
			yield return StartCoroutine(SwimToTarget(this.away));
			yield return StartCoroutine(SwimToTarget(this.halfaway));
			yield return StartCoroutine(SwimToTarget(this.home));
		}
	}
	
	/// <summary>
	/// Swimt to target.
	/// </summary>
	/// <param name="target">
	/// A <see cref="Vector3"/> - target place leader swims to.
	/// </param>
	/// <returns>
	/// A <see cref="IEnumerator"/>
	/// </returns>
	IEnumerator SwimToTarget(Vector3 target)
	{
		while ((this.transform.position - target).magnitude > 10)
		{
			this.GetComponent<Rigidbody>().velocity = (target - this.transform.position).normalized * speed;
			yield return new WaitForSeconds(timeDelta);
		}
	}
}
