using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Boid object.
/// </summary>
public class Boid : MonoBehaviour
{
	// Reference to its Flock
	internal Flock flock;
	// Object which is the leader for this Boid
	internal Transform boidLeader;
	// Array of all the Boids in the Flock
	internal List<Boid> boids;
	
	/// <summary>
	/// Starting Boid behaviour.
	/// </summary>
	void Start()
	{
		if (flock == null)
		{
			Debug.LogError("Boid doesn't belong to any Flock (apparently there is an error during Flock initialization) - disabling Boid script!");
			this.enabled = false;
		}
		if (boids == null)
		{
			Debug.LogError("Boid doesn't see other Boids in the Flock (apparently there is an error during Flock initialization) - disabling Boid script!");
			this.enabled = false;
		}
		if (this.enabled)
		{
			StartCoroutine(MainLoop());
		}
	}
	
	/// <summary>
	/// Places Boid within the Bounds.
	/// </summary>
	/// <param name="bounds">
	/// A <see cref="Bounds"/> - bounds of the Boid's Flock.
	/// </param>
	public virtual void PlaceInFlock(Bounds bounds)
	{
		transform.localPosition = new Vector3
		(
			Random.value * bounds.size.x,
			Random.value * bounds.size.y,
			Random.value * bounds.size.z
		) - bounds.extents;
	}
	
	/// <summary>
	/// Boid's behaviour main loop.
	/// </summary>
	/// <returns>
	/// A <see cref="IEnumerator"/>
	/// </returns>
	IEnumerator MainLoop()
	{
		while (true)
		{
			this.ProcessMovement();
			float waitTime = Random.Range(0.1f, 0.5f);
			yield return new WaitForSeconds(waitTime);
		}
	}
	
	/// <summary>
	/// Movement processing logic.
	/// </summary>
	protected virtual void ProcessMovement()
	{
		float deltaTime = Time.deltaTime;
		Vector3 v = Move() * deltaTime;
		GetComponent<Rigidbody>().velocity += v;
		this.LimitVelocity();
	}
	
	/// <summary>
	/// Clamp velocity between its min and max values.
	/// </summary>
	protected void LimitVelocity()
	{
		float speed = GetComponent<Rigidbody>().velocity.magnitude;
		if (speed > flock.maxBoidVelocity)
		{
			GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * flock.maxBoidVelocity;
		}
		else if (speed < flock.minBoidVelocity)
		{
			GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * flock.minBoidVelocity;
		}
	}
	
	/// <summary>
	/// Boid's movement:
	/// 
	/// B = C + V + D + L + R, where
	/// 	B - Boid velocity,
	/// 	C - Movement to Flock's center,
	/// 	V - Movement to match Flock's velocity,
	/// 	D - Movement to keep distance from other Boids,
	/// 	L - Movement towards the leader,
	/// 	R - Random movement
	/// 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - velocity.
	/// </returns>
	protected Vector3 Move()
	{
		Vector3 velocity =
			MoveToFlockCenter() +
			MoveToMatchFlockVelocity() +
			MoveToKeepDistanceFromOthers() +
			MoveToLeader() +
			MoveRandomly();
		return velocity;
	}
	
	/// <summary>
	/// 
	/// C = F - B, where
	/// 	C - Movement to Flock's center,
	/// 	F - Flock's center position,
	/// 	P - Current Boid's position
	/// 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - movement to Flock's center.
	/// </returns>
	Vector3 MoveToFlockCenter()
	{
		Vector3 center = flock.flockCenter - transform.localPosition;
		return center;
	}
	
	/// <summary>
	/// 
	/// V = F - B, where
	/// 	V - Movement to match Flock's velocity,
	/// 	F - Flock's velocity,
	/// 	B - Current Boid's velocity
	/// 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - movement to match Flock's velocity.
	/// </returns>
	Vector3 MoveToMatchFlockVelocity()
	{
		Vector3 velocity = flock.flockVelocity - GetComponent<Rigidbody>().velocity;
		return velocity;
	}
	
	/// <summary>
	/// 
	/// D = - (O1 - B) ... - (ON - B), where
	/// 	D - Movement to keep distance from other Boids,
	/// 	O1...ON - Positions of other Boids in the Flock (whose distance to current Boid is less than configured),
	/// 	B - Current Boid's position
	/// 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - movement to keep distance from other Boids.
	/// </returns>
	Vector3 MoveToKeepDistanceFromOthers()
	{
		Vector3 result = Vector3.zero;
		if (flock.distanceBetweenBoids > 0.0f)
		{
			foreach (Boid boid in boids)
			{
				if (!Object.Equals(this, boid))
				{
					Vector3 dst = boid.transform.localPosition - this.transform.localPosition;
					if (dst.magnitude < flock.distanceBetweenBoids)
					{
						result = result - dst;
					}
				}
			}
		}
		return result;
	}
	
	/// <summary>
	/// 
	/// L = (F - B) * S, where
	/// 	L - Movement to Flock's leader,
	/// 	F - Flock's leader position,
	/// 	B - Current Boid's position,
	/// 	S - Leader's strength
	/// 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - movement to leader.
	/// </returns>
	Vector3 MoveToLeader()
	{
		Vector3 leader = Vector3.zero;
		if (this.boidLeader != null && flock.leaderStrength > 0)
		{
			leader = this.boidLeader.localPosition - transform.localPosition;
			leader *= flock.leaderStrength;
		}
		return leader;
	}
	
	/// <summary>
	/// 
	/// R = N * F, where
	/// 	R - Random movement,
	/// 	N - Random normalized velocity,
	/// 	F - Random strength factor
	/// 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - random movement.
	/// </returns>
	Vector3 MoveRandomly()
	{
		Vector3 randomVelocity = Vector3.zero;
		if (flock.randomFactor > 0.0f)
		{
			randomVelocity = new Vector3
			(
				Random.Range(-1.0f, 1.0f),
			    Random.Range(-1.0f, 1.0f), 
			    Random.Range(-1.0f, 1.0f)
			);
			randomVelocity.Normalize();
			randomVelocity = randomVelocity * flock.randomFactor;
		}
		return randomVelocity;
	}
	
	/// <summary>
	/// Updates Boid's rotation.
	/// </summary>
	protected virtual void Update()
	{
		if (this.flock.lookAtLeader)
		{
			Quaternion rotation = Quaternion.LookRotation((this.boidLeader.position) - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * flock.boidRotationDamping);
		}
		else
		{
			if (GetComponent<Rigidbody>().velocity != Vector3.zero)
			{
				Quaternion rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * flock.boidRotationDamping);
			}
		}
		this.CheckLeader();
	}
	
	/// <summary>
	/// Checks leader's location.
	/// </summary>
	void CheckLeader()
	{
		try
		{
			if (this.flock.lostLeaderDistance > 0 && this.boidLeader != null && flock.leaderStrength > 0)
			{
				Vector3 leader = this.flock.flockLeader.localPosition - transform.localPosition;
				if (leader.magnitude <= this.flock.lostLeaderDistance)
				{
					this.boidLeader = this.flock.flockLeader;
				}
				else
				{
					this.SearchForNewLeader();
				}
			}
			else
			{
				this.boidLeader = this.flock.flockLeader;
			}
		}
		catch (MissingReferenceException e)
		{
			Debug.LogWarning("" + e);
			this.boidLeader = this.flock.flockLeader;
		}
	}
	
	/// <summary>
	/// Searches for new leader and switches if found.
	/// </summary>
	void SearchForNewLeader()
	{
		Vector3 leader = this.boidLeader.localPosition - transform.localPosition;
		if (leader.magnitude > this.flock.lostLeaderDistance)
		{
			foreach (Boid boid in boids)
			{
				if (!Object.Equals(this, boid))
				{
					Vector3 closestLeader = boid.transform.localPosition - transform.localPosition;
					if (closestLeader.magnitude <= this.flock.lostLeaderDistance && isValidLeader(this.transform, boid.boidLeader))
					{
						this.boidLeader = boid.transform;
						break;
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Checks Boid's hierarchy to determine whether it is valid Boid for being a leader for the current Boid.
	/// </summary>
	/// <param name="originalBoid">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="nextBoid">
	/// A <see cref="Transform"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	bool isValidLeader(Transform originalBoid, Transform nextBoid)
	{
		bool valid = true;
		try
		{
			valid = !Object.Equals(originalBoid, nextBoid);
			if (valid)
			{
				Boid boid = nextBoid.GetComponent<Boid>();
				if (boid != null)
				{
					valid = isValidLeader(originalBoid, boid.boidLeader);
				}
			}
		}
		catch (MissingReferenceException e)
		{
			Debug.LogWarning("" + e);
			valid = false;
		}
		return valid;
	}
}