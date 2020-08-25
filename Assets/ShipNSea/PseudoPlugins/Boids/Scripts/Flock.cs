using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// Script defines behaviour of a flock. Flock consists of multiple Boids, which
/// are moving according to a set of rules. Flock's size is dynamic - new Boids can
/// be added to the Flock dynamically. Old Boids can be removed (destroyed) from 
/// Flock dynamically.
/// 
/// </summary>
public class Flock : MonoBehaviour
{
	/// <summary>
	/// Minimum Boid's velocity.
	/// </summary>
	public float minBoidVelocity = 10.0f;
	/// <summary>
	/// Maximum Boid's velocity.
	/// </summary>
	public float maxBoidVelocity = 15.0f;
	/// <summary>
	/// Random factor, which add some chaos to Boids' movements.
	/// </summary>
	public float randomFactor = 10.0f;
	/// <summary>
	/// The bigger leader's strength, the smaller is distance between leader and Boid objects.
	/// </summary>
	public float leaderStrength = 5.0f;
	/// <summary>
	/// Distance which Boids try to keep between each other. Although it is not guaranteed
	/// that Boids are going to keep exactly this distance. The actual distance highly depends
	/// on other forces which effect Boids' velocities (e.g. leader's strength, randomization, etc.).
	/// </summary>
	public float distanceBetweenBoids = 10.0f;
	/// <summary>
	/// Defines mass of the Flock's Boids.
	/// </summary>
	public float boidMass = 1.0f;
	/// <summary>
	/// Defines whether Boid's are affected by gravity or not.
	/// </summary>
	public bool useGravity = true;
	/// <summary>
	/// Type of the Boid, which will be istatiated for this Flock.
	/// </summary>
	public BoidType boidType = BoidType.AIR;
	/// <summary>
	/// If set to true, Boids will try to look at the leader when moving.
	/// If set to false, Boid will try to look ahead of themselves when moving.
	/// </summary>
	public bool lookAtLeader = true;
	/// <summary>
	/// Rotation damping defines how fast Boids will turn to keep looking ahead of themselves or
	/// looking at the leader.
	/// </summary>
	public float boidRotationDamping = 10.0f;
	/// <summary>
	/// Expected number of Boids in the flock.
	/// </summary>
	public int flockSize = 50;
	/// <summary>
	/// Minimum Boid's scale.
	/// </summary>
	public float minBoidScale = 0.5f;
	/// <summary>
	/// Maximum Boid's scale.
	/// </summary>
	public float maxBoidScale = 1.5f;
	/// <summary>
	/// Visibility radius - makes Boid aware of objects on its way.
	/// </summary>
	public float boidVisibilityRadius = 0.5f;
	/// <summary>
	/// Boid's graphics prefab, which is used to instantiate Boid's graphics clones.
	/// </summary>
	public GameObject boidPrefab;
	/// <summary>
	/// Flock's leader, which is followed by the moving Boids.
	/// </summary>
	public Transform flockLeader;
	
	/// <summary>
	/// When leader moves away to this distance, Boid tries to find itself another leader.
	/// New leader should be within this distance. If this setting is set to 0, then leader
	/// change won't be performed.
	/// </summary>
	internal float lostLeaderDistance = 0.0f;
	internal Vector3 flockCenter;
	internal Vector3 flockVelocity;
	internal List<Boid> boids = new List<Boid>();
	[SerializeField]
	[HideInInspector]
	private string id;
	
	/// <summary>
	/// Copies configuration of other flock
	/// </summary>
	/// <param name="other">
	/// A <see cref="Flock"/> - other flock.
	/// </param>
	public void CopyConfigurationFrom(Flock other)
	{
		minBoidVelocity = other.minBoidVelocity;
		maxBoidVelocity = other.maxBoidVelocity;
		randomFactor = other.randomFactor;
		leaderStrength = other.leaderStrength;
		distanceBetweenBoids = other.distanceBetweenBoids;
		lookAtLeader = other.lookAtLeader;
		boidRotationDamping = other.boidRotationDamping;
		flockSize = other.flockSize;
		boidPrefab = other.boidPrefab;
		minBoidScale = other.minBoidScale;
		maxBoidScale = other.maxBoidScale;
		lostLeaderDistance = other.lostLeaderDistance;
		boidVisibilityRadius = other.boidVisibilityRadius;
		boidMass = other.boidMass;
		boidType = other.boidType;
	}
	
	/// <summary>
	/// Sets Flock's unique ID. This ID must uniquely identify this specific Flock instance among the others.
	/// </summary>
	/// <param name="id">
	/// A <see cref="System.String"/> - unique ID.
	/// </param>
	public void SetId(string id)
	{
		this.id = id;
	}
	
	/// <summary>
	/// Returns Flock's unique ID.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> - Flock's unique ID.
	/// </returns>
	public string GetId()
	{
		return this.id;
	}
	
	/// <summary>
	/// Returns the list of all the Flock's Boids.
	/// </summary>
	/// <returns>
	/// A <see cref="List<Boid>"/> - List of all the Flock's Boids.
	/// </returns>
	public List<Boid> GetBoids()
	{
		return boids;
	}
	
	/// <summary>
	/// Returns Flock's center, which is average value of all the Boids' local positions (within Flock). 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - Flock's center.
	/// </returns>
	public Vector3 GetFlockCenter()
	{
		return this.flockCenter;
	}
	
	/// <summary>
	/// Returns Flock's velocity, which is average velocity of all the Flock's Boids.
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/> - Flock's velocity.
	/// </returns>
	public Vector3 GetFlockVelocity()
	{
		return this.flockVelocity;
	}
	
	/// <summary>
	/// 
	/// Flock initialization.
	/// 
	/// During initialization Flock is filled with the Boids, distributed randomly within Flock's colider borders.
	/// It expects reference to Boid's graphics prefab. Flock Script will be disabled if no prefab defined.
	/// It expects defined Flock Collider. Flock Script will be disabled if no collider added.
	/// 
	/// </summary>
	void Start()
	{
		this.enabled = true;
		if (this.boidPrefab == null)
		{
			Debug.LogError("Boid template is not added to the Flock script component - disabling Flock script!");
			this.enabled = false;
		}
		if (this.GetComponent<Collider>() == null)
		{
			Debug.LogError("Collider is not added to the Flock script component - disabling Flock script!");
			this.enabled = false;
		}
		this.boids.Clear();
		if (this.enabled)
		{
			for (int i = 0; i < flockSize; i++)
			{
				AddBoid();
			}
		}
		this.enabled = this.enabled && this.boids.Count == flockSize;
	}
	
	/// <summary>
	/// Adds newly instantiated Boid's clone within the bounds of Flock's Collider (position is random).
	/// New clone is added to the list of all the Boids.
	/// </summary>
	void AddBoid()
	{
		Boid boid = InstantiateBoid();
		boid.transform.parent = transform;
		boid.PlaceInFlock(GetComponent<Collider>().bounds);
		boid.flock = this;
		boid.boidLeader = this.flockLeader;
		boid.boids = this.boids;
		boids.Add(boid);
	}
	
	/// <summary>
	/// 
	/// Instantiates GameObject from Boid's graphics prefab. When instantiating prefab's position and rotation are used.
	/// 
	/// It is expected that Boid's graphics prefab doesn't contain Rigidbody. If it does contain Rigidbody, it will be destroyed
	/// after instantiated of prefab - Rigidbody is added to Boid's graphics container object. Container allows better flexibility
	/// for graphics object - it can use local positions, rotations. The same is true for Boid Script - both Boid Script and Rigidbody
	/// are applied to Boid's graphics container object and removed from graphics object itself.
	/// 
	/// It is expected that Boid's graphics prefab defines Collider. If it doesn't have Collider, default SphereCollider will be added
	/// to instantiated graphics object.
	/// 
	/// Boid's container object is a parent for Boid's graphics object. Boid Script is added to Boid's container object. Rigidbody is added
	/// to Boid's container object. X,Y,Z rotations are disabled for container's Rigidbody. 
	/// Related motions are controlled by
	/// Boid Script.
	/// 
	/// </summary>
	/// 
	/// <returns>
	/// A <see cref="Boid"/> - Boid Script, which is a component of Boid's container object.
	/// </returns>
	Boid InstantiateBoid()
	{
       
        GameObject flockBoid = Instantiate(this.boidPrefab, this.boidPrefab.transform.position, this.boidPrefab.transform.rotation) as GameObject;
		if (flockBoid.GetComponent<Rigidbody>() != null)
		{
			Debug.LogWarning("Boid graphics prefab contains Rigidbody - it will be removed from Boid graphics object. Rigidbody will be added to Boid graphics container.");
			Destroy(flockBoid.GetComponent<Rigidbody>());
		}
		Boid boid = flockBoid.GetComponent<Boid>();
		if (boid != null)
		{
			Destroy(boid);
			Debug.LogWarning("Boid graphics prefab contains Boid Script - it will be removed from Boid graphics object. Boid Script will be added to Boid graphics container.");
		}
		if (flockBoid.GetComponent<Collider>() == null)
		{
			flockBoid.AddComponent<SphereCollider>();
			Debug.LogWarning("Boid graphics prefab doesn't contain collider - default SphereCollider will be added.");
		}
		GameObject flockBoidContainer = new GameObject(flockBoid.name + " Container");
		flockBoid.transform.parent = flockBoidContainer.transform;
		flockBoidContainer.AddComponent<Rigidbody>();
		flockBoidContainer.GetComponent<Rigidbody>().useGravity = this.useGravity;
		flockBoidContainer.GetComponent<Rigidbody>().mass = this.boidMass <= 0 ? 0.0000000001f : this.boidMass;
		flockBoidContainer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		if (this.boidType == BoidType.GROUND)
		{
			flockBoidContainer.AddComponent<GroundBoid>();
		}
		else if (this.boidType == BoidType.WATER)
		{
			flockBoidContainer.AddComponent<WaterBoid>();
		}
		else
		{
			flockBoidContainer.AddComponent<AirBoid>();
		}
		boid = flockBoidContainer.GetComponent<Boid>();
		boid.transform.localScale = this.boidPrefab.transform.localScale * Random.Range(this.minBoidScale, this.maxBoidScale);
		flockBoidContainer.AddComponent<SphereCollider>();
		SphereCollider visibilityCollider = flockBoidContainer.GetComponent<SphereCollider>();
		visibilityCollider.isTrigger = true;
		visibilityCollider.radius = boidVisibilityRadius * (1.0f / this.boidPrefab.transform.localScale.magnitude);
		return boid;
	}
	
	/// <summary>
	/// 
	/// On frame each frame change, expected Flock size is compared with actual size.
	/// If expected size is larger, then new Boid clones are added. If expected size is smaller,
	/// then corresponding number of Boids is removed from the Flock.
	/// 
	/// Flock center position and flock velocity are updated on each frame.
	/// Flock center is an average value of all the Boids' local positions: C = (C1 + ... + CN) / N .
	/// Flock velocity is an average value of all the Boid's velocities: V = (V1 + ... + VN) / N . 
	/// 
	/// </summary>
	void Update()
	{
		if (this.flockSize > boids.Count)
		{
			for (int i = 0; i < flockSize - boids.Count; i++)
			{
				AddBoid();
			}
		}
		else if (this.flockSize < boids.Count)
		{
			for (int i = 0; i < boids.Count - flockSize; i++)
			{
				if (this.boids.Count > 0)
				{
					Boid boid = this.boids[0];
					if (boid != null)
					{
						this.boids.RemoveAt(0);
						Destroy(boid.gameObject);
					}
				}
			}
		}
		Vector3 center = Vector3.zero;
		Vector3 velocity = Vector3.zero;
		foreach (Boid boid in boids)
		{
			center += boid.transform.localPosition;
			velocity += boid.GetComponent<Rigidbody>().velocity;
		}
		flockCenter = boids.Count == 0 ? Vector3.zero : (center / boids.Count);
        
		flockVelocity = boids.Count == 0 ? Vector3.zero : (velocity / boids.Count);
        
	}
}