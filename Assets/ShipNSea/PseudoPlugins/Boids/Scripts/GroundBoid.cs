using UnityEngine;
using System.Collections;

/// <summary>
/// Boid running on ground.
/// </summary>
public class GroundBoid : Boid {
	
	// Hit information
	internal RaycastHit hit = new RaycastHit();
	
	/// <summary>
	/// Places Boid within the Bounds.
	/// </summary>
	/// <param name="bounds">
	/// A <see cref="Bounds"/> - bounds of the Boid's Flock.
	/// </param>
	public override void PlaceInFlock(Bounds bounds)
	{
		transform.localPosition = new Vector3
		(
			Random.value * bounds.size.x,
			0.0f,
			Random.value * bounds.size.z
		) - bounds.extents;
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1000.0f))
		{
			Vector3 v = transform.localPosition;
			v.y -= hit.distance - hit.distance * 0.01f;
			transform.localPosition = v;
		}
		else
		{
			Vector3 v = transform.localPosition;
			v.y -= 1000.0f;
			transform.localPosition = v;
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	protected override void Update()
	{
		Quaternion r1;
		if (this.flock.lookAtLeader)
		{
			r1 = Quaternion.LookRotation((this.boidLeader.position) - transform.position);
			r1.x = 0.0f;
			r1.z = 0.0f;
		}
		else if (GetComponent<Rigidbody>().velocity != Vector3.zero)
		{
			r1 = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
			r1.x = 0.0f;
			r1.z = 0.0f;
		}
		else
		{
			r1 = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
		}
		Vector3 v = GetComponent<Rigidbody>().velocity;
		float dst = 0.0f;
		Quaternion r2;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1000.0f))
		{
			dst = hit.distance;
			r2 = Quaternion.FromToRotation(transform.up, hit.normal);
			r2.y = 0.0f;
			r2.w = 0.0f;
		}
		else
		{
			r2 = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
		}
		Quaternion rotation = new Quaternion(r1.x + r2.x, r1.y + r2.y, r1.z + r2.z, r1.w + r2.w);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * flock.boidRotationDamping);
		v.y = -dst * this.GetComponent<Rigidbody>().mass;
		GetComponent<Rigidbody>().velocity = v;
		this.LimitVelocity();
	}
}
