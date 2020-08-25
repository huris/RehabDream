#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Flocks Editor.
/// </summary>
public class FlocksWindow : EditorWindow
{	
	// Defaults values
	public static readonly string DEFAULT_FLOCK_NAME = "Flock Name";
	
	// Keys for storing preferences for min range values
	public static readonly string PREF_MIN_ROTATION_DAMPING = "minRotationDamping";
	public static readonly string PREF_MIN_LEADER_STRENGTH = "minLeaderStrength";
	public static readonly string PREF_MIN_VELOCITY = "minVelocity";
	public static readonly string PREF_MIN_SCALE = "minScale";
	public static readonly string PREF_MIN_RANDOM_FACTOR = "minRandomFactor";
	public static readonly string PREF_MIN_DISTANCE = "minDistance";
	public static readonly string PREF_MIN_LOST_LEADER_DISTANCE = "minLostLeaderDistance";
	public static readonly string PREF_MIN_FLOCK_SIZE = "minFlockSize";
	public static readonly string PREF_MIN_VISIBILITY_RADIUS = "minVisibilityRadius";
	public static readonly string PREF_MIN_BOID_MASS = "minBoidMass";
	
	// Keys for storing preferences for max range values
	public static readonly string PREF_MAX_ROTATION_DAMPING = "maxRotationDamping";
	public static readonly string PREF_MAX_LEADER_STRENGTH = "maxLeaderStrength";
	public static readonly string PREF_MAX_VELOCITY = "maxVelocity";
	public static readonly string PREF_MAX_SCALE = "maxScale";
	public static readonly string PREF_MAX_RANDOM_FACTOR = "maxRandomFactor";
	public static readonly string PREF_MAX_DISTANCE = "maxDistance";
	public static readonly string PREF_MAX_LOST_LEADER_DISTANCE = "maxLostLeaderDistance";
	public static readonly string PREF_MAX_FLOCK_SIZE = "maxFlockSize";
	public static readonly string PREF_MAX_VISIBILITY_RADIUS = "maxVisibilityRadius";
	public static readonly string PREF_MAX_BOID_MASS = "maxBoidMass";
	
	// Labels
	public static readonly string LABEL_BOID_PREFAB = "Boid Prefab";
	public static readonly string LABEL_BOID_TYPE = "Boid Type";
	public static readonly string LABEL_USE_GRAVITY = "Use Gravity";
	public static readonly string LABEL_LOOK_AT_LEADER = "Look At Leader";
	public static readonly string LABEL_ROTATION_DAMPING = "Rotation Damping";
	public static readonly string LABEL_LEADER_STRENGTH = "Leader Strength";
	public static readonly string LABEL_LOST_LEADER_DISTANCE = "Lost Leader Distance";
	public static readonly string LABEL_BOID_MIN_VELOCITY = "Boid Min Velocity";
	public static readonly string LABEL_BOID_MAX_VELOCITY = "Boid Max Velocity";
	public static readonly string LABEL_BOID_MIN_SCALE = "Boid Min Scale";
	public static readonly string LABEL_BOID_MAX_SCALE = "Boid Max Scale";
	public static readonly string LABEL_BOID_MASS = "Boid Mass";
	public static readonly string LABEL_RANDOM_FACTOR = "Random Factor";
	public static readonly string LABEL_DISTANCE = "Distance";
	public static readonly string LABEL_VISIBILITY_RADIUS = "Visibility Radius";
	public static readonly string LABEL_FLOCK_SIZE = "Flock Size";
	
	/// <summary>
	/// Static map for storing configuration settings for game's Flocks. It is used
	/// to propagate setting changes from Play Mode.
	/// </summary>
	public static Dictionary<string, Flock> configMap = new Dictionary<string, Flock>();
	/// <summary>
	/// Object used for synchronized access to configMap.
	/// </summary>
	public static readonly object obj = new object();
	
    internal string flockName = DEFAULT_FLOCK_NAME;
	internal GameObject boidPrefab = null;
	internal Flock chosenFlock = null;
	internal Vector2 scrollPosition;
	
	/// <summary>
	/// Stores editor's last state - whether in Play Mode or not.
	/// </summary>
	internal bool lastState = false;
	
	// Min values for editor ranges
	internal float minRotationDamping = 0.0f;
	internal float minLeaderStrength = 0.0f;
	internal float minVelocity = 0.0f;
	internal float minScale = 0.1f;
	internal float minRandomFactor = 0.0f;
	internal float minDistance = 0.0f;
	internal float minLostLeaderDistance = 0.0f;
	internal float minFlockSize = 1.0f;
	internal float minVisibilityRadius = 0.0f;
	internal float minBoidMass = 0.0f;
	
	// Max values for editor ranges
	internal float maxRotationDamping = 10.0f;
	internal float maxLeaderStrength = 100.0f;
	internal float maxVelocity = 100.0f;
	internal float maxScale = 10.0f;
	internal float maxRandomFactor = 1000.0f;
	internal float maxDistance = 100.0f;
	internal float maxLostLeaderDistance = 100.0f;
	internal float maxFlockSize = 300.0f;
	internal float maxVisibilityRadius = 100.0f;
	internal float maxBoidMass = 100.0f;
	
	// Limits flags
	internal bool rotationDmapingLimits = false;
	internal bool leaderStrengthLimits = false;
	internal bool boidVelocityLimits = false;
	internal bool boidScaleLimits = false;
	internal bool randomFactorLimits = false;
	internal bool distanceLimits = false;
	internal bool visibilityRadiusLimits = false;
	internal bool flockSizeLimits = false;
	internal bool boidMassLimits = false;
    
	/// <summary>
	/// Adds Flocks Window to the Menu
	/// </summary>
    [MenuItem ("Window/Flocks")]
    static void Init () {
        EditorWindow.GetWindow (typeof (FlocksWindow), false, "Flocks");
    }
	
	/// <summary>
	/// Loads ranges' preferences on load.
	/// </summary>
	void Awake() {
        minRotationDamping = EditorPrefs.GetFloat(PREF_MIN_ROTATION_DAMPING, 0.0f);
		minLeaderStrength = EditorPrefs.GetFloat(PREF_MIN_LEADER_STRENGTH, 0.0f);
		minVelocity = EditorPrefs.GetFloat(PREF_MIN_VELOCITY, 0.0f);
		minScale = EditorPrefs.GetFloat(PREF_MIN_SCALE, 0.1f);
		minRandomFactor = EditorPrefs.GetFloat(PREF_MIN_RANDOM_FACTOR, 0.0f);
		minDistance = EditorPrefs.GetFloat(PREF_MIN_DISTANCE, 0.0f);
		minLostLeaderDistance = EditorPrefs.GetFloat(PREF_MIN_LOST_LEADER_DISTANCE, 0.0f);
		minFlockSize = EditorPrefs.GetFloat(PREF_MIN_FLOCK_SIZE, 1.0f);
		minVisibilityRadius = EditorPrefs.GetFloat(PREF_MIN_VISIBILITY_RADIUS, 0.0f);
		minBoidMass = EditorPrefs.GetFloat(PREF_MIN_BOID_MASS, 0.0f);
		
		maxRotationDamping = EditorPrefs.GetFloat(PREF_MAX_ROTATION_DAMPING, 10.0f);
		maxLeaderStrength = EditorPrefs.GetFloat(PREF_MAX_LEADER_STRENGTH, 100.0f);
		maxVelocity = EditorPrefs.GetFloat(PREF_MAX_VELOCITY, 100.0f);
		maxScale = EditorPrefs.GetFloat(PREF_MAX_SCALE, 10.0f);
		maxRandomFactor = EditorPrefs.GetFloat(PREF_MAX_RANDOM_FACTOR, 1000.0f);
		maxDistance = EditorPrefs.GetFloat(PREF_MAX_DISTANCE, 100.0f);
		maxLostLeaderDistance = EditorPrefs.GetFloat(PREF_MAX_LOST_LEADER_DISTANCE, 100.0f);
		maxFlockSize = EditorPrefs.GetFloat(PREF_MAX_FLOCK_SIZE, 300.0f);
		maxVisibilityRadius = EditorPrefs.GetFloat(PREF_MAX_VISIBILITY_RADIUS, 100.0f);
		maxBoidMass = EditorPrefs.GetFloat(PREF_MAX_BOID_MASS, 0.0f);
    }
	
	/// <summary>
	/// Stores ranges' preferences on exit.
	/// </summary>
    void OnDestroy() {
        EditorPrefs.SetFloat(PREF_MIN_ROTATION_DAMPING, minRotationDamping);
		EditorPrefs.SetFloat(PREF_MIN_LEADER_STRENGTH, minLeaderStrength);
		EditorPrefs.SetFloat(PREF_MIN_VELOCITY, minVelocity);
		EditorPrefs.SetFloat(PREF_MIN_SCALE, minScale);
		EditorPrefs.SetFloat(PREF_MIN_RANDOM_FACTOR, minRandomFactor);
		EditorPrefs.SetFloat(PREF_MIN_DISTANCE, minDistance);
		EditorPrefs.SetFloat(PREF_MIN_LOST_LEADER_DISTANCE, minLostLeaderDistance);
		EditorPrefs.SetFloat(PREF_MIN_FLOCK_SIZE, minFlockSize);
		EditorPrefs.SetFloat(PREF_MIN_VISIBILITY_RADIUS, minVisibilityRadius);
		EditorPrefs.SetFloat(PREF_MIN_BOID_MASS, minBoidMass);
		
		EditorPrefs.SetFloat(PREF_MAX_ROTATION_DAMPING, maxRotationDamping);
		EditorPrefs.SetFloat(PREF_MAX_LEADER_STRENGTH, maxLeaderStrength);
		EditorPrefs.SetFloat(PREF_MAX_VELOCITY, maxVelocity);
		EditorPrefs.SetFloat(PREF_MAX_SCALE, maxScale);
		EditorPrefs.SetFloat(PREF_MAX_RANDOM_FACTOR, maxRandomFactor);
		EditorPrefs.SetFloat(PREF_MAX_DISTANCE, maxDistance);
		EditorPrefs.SetFloat(PREF_MAX_LOST_LEADER_DISTANCE, maxLostLeaderDistance);
		EditorPrefs.SetFloat(PREF_MAX_FLOCK_SIZE, maxFlockSize);
		EditorPrefs.SetFloat(PREF_MAX_VISIBILITY_RADIUS, maxVisibilityRadius);
		EditorPrefs.SetFloat(PREF_MAX_BOID_MASS, maxBoidMass);
    }
    
	/// <summary>
	/// Draws Flocks Editor GUI.
	/// </summary>
    void OnGUI () {
		this.AddFlockMenu();
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		this.EditFlocksMenu();
		this.EditPreferencesMenu();
		EditorGUILayout.EndScrollView();
    }
	
	/// <summary>
	/// 
	/// </summary>
	void AddFlockMenu()
	{
		// Label
		GUILayout.Label ("Add Flock", EditorStyles.boldLabel);
		// Content
        flockName = EditorGUILayout.TextField ("Flock Name", flockName);
		this.boidPrefab = (GameObject) EditorGUILayout.ObjectField("Boid Prefab", this.boidPrefab, typeof(GameObject), false);
		if (GUILayout.Button("Add New Flock"))
		{
        	this.AddFlock();
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	void EditFlocksMenu()
	{
		// Label
		GUILayout.Label ("Edit Flocks", EditorStyles.boldLabel);
		// Content
		Flock[] flocks = FindObjectsOfType(typeof(Flock)) as Flock[];
        foreach( Flock flock in flocks )
        {
			bool isChosenFlock = this.chosenFlock != null && System.Object.Equals(this.chosenFlock, flock);
			bool flockOpen = EditorGUILayout.Foldout(isChosenFlock, flock.name);
			if (flockOpen) 
			{
				this.chosenFlock = flock;
				this.AssignFollower(this.chosenFlock);
				this.EditFlock(this.chosenFlock);
			}
			else if (isChosenFlock)
			{
				this.chosenFlock = null;
			}
        }
	}
	
	/// <summary>
	/// 
	/// </summary>
	void EditPreferencesMenu()
	{
		// Label
		GUILayout.Label ("Editor Preferences (Limits)", EditorStyles.boldLabel);
		// Content
		EditPreferences();
	}
	
	/// <summary>
	/// Searches the scene. If FlockFollower is present, then it draws a checkbox to allow
	/// following the target Flock.
	/// </summary>
	/// <param name="flock">
	/// A <see cref="Flock"/> - target Flock to follow.
	/// </param>
	void AssignFollower(Flock flock)
	{
		FlockFollower follower = FindObjectOfType(typeof(FlockFollower)) as FlockFollower;
		if (follower != null)
		{
			bool wasAssigned = follower.flock != null && System.Object.Equals(follower.flock, flock);
			bool assigned = EditorGUILayout.Toggle("Followed By Cam: ", wasAssigned);
			if (assigned)
			{
				follower.flock = flock;
			}
			else if (wasAssigned)
			{
				follower.flock = null;
			}
		}
	}
	
	/// <summary>
	/// Adds new Flock with specified name and Boid's prefab.
	/// </summary>
	void AddFlock()
	{
		bool valid = true;
		if (this.flockName == null || this.flockName.Length == 0)
		{
			Debug.LogError("Flock name is not defined - flock will not be created!");
			valid = false;
		}
		if (this.boidPrefab == null)
		{
			Debug.LogError("Boid prefab is not defined - flock will not be created!");
			valid = false;
		}
		else
		{
			PrefabType prefabType = PrefabUtility.GetPrefabType(this.boidPrefab);
			if (prefabType != PrefabType.Prefab && prefabType != PrefabType.ModelPrefab)
			{
				Debug.LogError("This is not valid prefab type: " + prefabType + ". Expected PrefabType.Prefab or PrefabType.ModelPrefab");
				valid = false;
			}
		}
		if (valid)
		{
			this.CreateFlock();
		}
	}
	
	/// <summary>
	/// Instantiation of new Flock game object.
	/// </summary>
	void CreateFlock()
	{
		// Flock game object
		GameObject flock = new GameObject(this.flockName);
		flock.transform.position = new Vector3(0,0,0);
		flock.AddComponent<Flock>();
		flock.AddComponent<SphereCollider>();
		flock.GetComponent<Collider>().isTrigger = true;
		((SphereCollider) flock.GetComponent<Collider>()).radius = 10.0f;
		// Flock leader game object
		GameObject flockLeader = new GameObject(this.flockName + " Leader");
		flockLeader.transform.parent = flock.transform;
		Flock flockScript = flock.GetComponent<Flock>();
		flockScript.SetId(Guid.NewGuid().ToString());
		flockScript.flockLeader = flockLeader.transform;
		flockScript.boidPrefab = this.boidPrefab;
		UpdateFlockConfiguration(flockScript);
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="flock">
	/// A <see cref="Flock"/>
	/// </param>
	void UpdateFlockConfiguration(Flock flock)
	{
		bool previousState = lastState;
		bool currentState = EditorApplication.isPlaying;
		lastState = currentState;
		if (flock.GetId() != null)
		{
			if (previousState != currentState)
			{
				lock (obj)
				{
					if (!previousState && currentState)
					{
						configMap.Remove(flock.GetId());
						configMap.Add(flock.GetId(), flock);
					}
					else if (previousState && !currentState)
					{
						Flock tmp = null;
						if (configMap.TryGetValue(flock.GetId(), out tmp))
						{
							flock.CopyConfigurationFrom(tmp);
						}
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Edit Flock's settings.
	/// </summary>
	/// <param name="flock">
	/// A <see cref="Flock"/>
	/// </param>
	void EditFlock(Flock flock) {
		UpdateFlockConfiguration(flock);
		GUILayout.Label("");
		flock.boidPrefab = (GameObject) EditorGUILayout.ObjectField(LABEL_BOID_PREFAB, flock.boidPrefab, typeof(GameObject), false);
		flock.boidType = (BoidType) EditorGUILayout.EnumPopup(LABEL_BOID_TYPE, flock.boidType);
		flock.minBoidScale = EditorGUILayout.FloatField(LABEL_BOID_MIN_SCALE, flock.minBoidScale);
		flock.maxBoidScale = EditorGUILayout.FloatField(LABEL_BOID_MAX_SCALE, flock.maxBoidScale);
		EditorGUILayout.MinMaxSlider(ref flock.minBoidScale, ref flock.maxBoidScale, this.minScale, this.maxScale);
		flock.boidMass = EditorGUILayout.Slider(LABEL_BOID_MASS, flock.boidMass, this.minBoidMass, flock.boidMass >= this.maxBoidMass ? flock.boidMass : this.maxBoidMass);
		if (flock.boidType == BoidType.WATER || flock.boidType == BoidType.AIR)
		{
			flock.boidVisibilityRadius = EditorGUILayout.Slider(LABEL_VISIBILITY_RADIUS, flock.boidVisibilityRadius, this.minVisibilityRadius, flock.boidVisibilityRadius >= this.maxVisibilityRadius ? flock.boidVisibilityRadius : this.maxVisibilityRadius);
		}
		flock.useGravity = EditorGUILayout.Toggle(LABEL_USE_GRAVITY, flock.useGravity);
		GUILayout.Label("");
		flock.lookAtLeader = EditorGUILayout.Toggle(LABEL_LOOK_AT_LEADER, flock.lookAtLeader);
		flock.boidRotationDamping = EditorGUILayout.Slider (LABEL_ROTATION_DAMPING, flock.boidRotationDamping, this.minRotationDamping, flock.boidRotationDamping >= this.maxRotationDamping ? flock.boidRotationDamping : this.maxRotationDamping);
		flock.leaderStrength = EditorGUILayout.Slider (LABEL_LEADER_STRENGTH, flock.leaderStrength, this.minLeaderStrength, flock.leaderStrength >= this.maxLeaderStrength ? flock.leaderStrength : this.maxLeaderStrength);
		//flock.lostLeaderDistance = EditorGUILayout.Slider (LABEL_LOST_LEADER_DISTANCE, flock.lostLeaderDistance, this.minLostLeaderDistance, flock.lostLeaderDistance >= this.maxLostLeaderDistance ? flock.lostLeaderDistance : this.maxLostLeaderDistance);
		flock.minBoidVelocity = EditorGUILayout.FloatField(LABEL_BOID_MIN_VELOCITY, flock.minBoidVelocity);
        flock.maxBoidVelocity = EditorGUILayout.FloatField(LABEL_BOID_MAX_VELOCITY, flock.maxBoidVelocity);
		EditorGUILayout.MinMaxSlider(ref flock.minBoidVelocity, ref flock.maxBoidVelocity, this.minVelocity, this.maxVelocity);
		flock.randomFactor = EditorGUILayout.Slider(LABEL_RANDOM_FACTOR, flock.randomFactor, this.minRandomFactor, flock.randomFactor >= this.maxRandomFactor ? flock.randomFactor : this.maxRandomFactor);
		flock.distanceBetweenBoids = EditorGUILayout.Slider(LABEL_DISTANCE, flock.distanceBetweenBoids, this.minDistance, flock.distanceBetweenBoids >= this.maxDistance ? flock.distanceBetweenBoids : this.maxDistance);
		flock.flockSize = (int) EditorGUILayout.Slider(LABEL_FLOCK_SIZE, flock.flockSize, this.minFlockSize, flock.flockSize >= this.maxFlockSize ? flock.flockSize : this.maxFlockSize);
		GUILayout.Label("");
		UpdateFlockConfiguration(flock);
	}
	
	/// <summary>
	/// Store ranges' preferences.
	/// </summary>
	void EditPreferences()
	{	
		this.rotationDmapingLimits = EditorGUILayout.Foldout(this.rotationDmapingLimits, "Rotation Damping Limits");
		if (this.rotationDmapingLimits) 
		{
			this.minRotationDamping = EditorGUILayout.FloatField("Min", this.minRotationDamping);
			this.maxRotationDamping = EditorGUILayout.FloatField("Max", this.maxRotationDamping);
		}
		this.leaderStrengthLimits = EditorGUILayout.Foldout(this.leaderStrengthLimits, "Leader Strength Limits");
		if (this.leaderStrengthLimits) 
		{
			this.minLeaderStrength = EditorGUILayout.FloatField("Min", this.minLeaderStrength);
			this.maxLeaderStrength = EditorGUILayout.FloatField("Max", this.maxLeaderStrength);
		}
		this.boidVelocityLimits = EditorGUILayout.Foldout(this.boidVelocityLimits, "Boid Velocity Limits");
		if (this.boidVelocityLimits) 
		{
			this.minVelocity = EditorGUILayout.FloatField("Min", this.minVelocity);
			this.maxVelocity = EditorGUILayout.FloatField("Max", this.maxVelocity);
		}
		this.boidScaleLimits = EditorGUILayout.Foldout(this.boidScaleLimits, "Boid Scale Limits");
		if (this.boidScaleLimits) 
		{
			this.minScale = EditorGUILayout.FloatField("Min", this.minScale);
			this.maxScale = EditorGUILayout.FloatField("Max", this.maxScale);;
		}
		this.boidMassLimits = EditorGUILayout.Foldout(this.boidMassLimits, "Boid Mass Limits");
		if (this.boidMassLimits) 
		{
			this.minBoidMass = EditorGUILayout.FloatField("Min", this.minBoidMass);
			this.maxBoidMass = EditorGUILayout.FloatField("Max", this.maxBoidMass);;
		}
		this.randomFactorLimits = EditorGUILayout.Foldout(this.randomFactorLimits, "Random Factor Limits");
		if (this.randomFactorLimits) 
		{
			this.minRandomFactor = EditorGUILayout.FloatField("Min", this.minRandomFactor);
			this.maxRandomFactor = EditorGUILayout.FloatField("Max", this.maxRandomFactor);;
		}
		this.distanceLimits = EditorGUILayout.Foldout(this.distanceLimits, "Distance Limits");
		if (this.distanceLimits) 
		{
			this.minDistance = EditorGUILayout.FloatField("Min", this.minDistance);
			this.maxDistance = EditorGUILayout.FloatField("Max", this.maxDistance);;
		}
		this.visibilityRadiusLimits = EditorGUILayout.Foldout(this.visibilityRadiusLimits, "Visibility Radius Limits");
		if (this.visibilityRadiusLimits) 
		{
			this.minVisibilityRadius = EditorGUILayout.FloatField("Min", this.minVisibilityRadius);
			this.maxVisibilityRadius = EditorGUILayout.FloatField("Max", this.maxVisibilityRadius);;
		}
		this.flockSizeLimits = EditorGUILayout.Foldout(this.flockSizeLimits, "Flock Size Limits");
		if (this.flockSizeLimits) 
		{
			this.minFlockSize = EditorGUILayout.FloatField("Min", this.minFlockSize);
			this.maxFlockSize = EditorGUILayout.FloatField("Max", this.maxFlockSize);;
		}
	}
}

#endif