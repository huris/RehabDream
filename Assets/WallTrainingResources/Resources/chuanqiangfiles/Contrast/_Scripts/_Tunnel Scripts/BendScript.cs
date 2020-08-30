using UnityEngine;
using System.Collections;


/// <summary>
/// World distortion behaviour.
/// </summary>
public class BendScript : MonoBehaviour
{

	[Range(0, 0.5f)]
	public float extraCullHeight;


    // The point from which the world will be distorted.
    // This will usually be the player character.
	public GameObject bendPoint;

    //The camera from which the bend will be processed.
    //This will usually be the camera following the player.
	public Camera _camera;

    //How much the scene should be warped.
    [Header("Bend Controllers")]
	[Range(-1.0f, 1.0f)]
	public float bendAmount_Y = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float bendAmount_X = 0.0f;

    //Bend Start position modifiers.
    [Header("Bend Height and Distance Offset")]
    [Tooltip("The height from which the bend will be processed")]
    public float horizonOffset = 0.0f;
    [Tooltip("The distance from bendPoint that the bend will begin")]
	public float spread = 0.0f;
	public float Horizon { get { return bendPoint != null ? bendPoint.transform.position.z + horizonOffset : 0; } }

    private void Update()
	{
        //If we have a bendPoint set, alter the global floats within the Shader.
		if (bendPoint != null)
		{
			Shader.SetGlobalFloat("_HORIZON", bendPoint.transform.position.z + horizonOffset);
			Shader.SetGlobalFloat("_SPREAD", spread);
			Shader.SetGlobalFloat("_BEND_Y", bendAmount_Y / 10);
            Shader.SetGlobalFloat("_BEND_X", bendAmount_X / 10);
        }
	}

    //Reset global floats when destroyed.
	private void OnDestroy()
	{
        Shader.SetGlobalFloat("_BEND_Y", 0);
        Shader.SetGlobalFloat("_BEND_X", 0);
        Shader.SetGlobalFloat("_SPREAD", 0);
		Shader.SetGlobalFloat("_HORIZON", 0);
	}

    //Reset global floats on quit.
	private void OnApplicationQuit()
	{
        Shader.SetGlobalFloat("_BEND_Y", 0);
        Shader.SetGlobalFloat("_BEND_X", 0);
        Shader.SetGlobalFloat("_SPREAD", 0);
		Shader.SetGlobalFloat("_HORIZON", 0);
	}

	private void Start()
	{
		if(_camera == null)
			_camera = GetComponent<Camera>();
	}

    //Take into account the extra cull distance that we have set.
    //Without this, as the world is warped, objects that were initially outside of the camera field of view
    //can come into view while culled.
	private void OnPreCull()
	{
		Shader.SetGlobalMatrix("_Camera2World", _camera.cameraToWorldMatrix);
		Shader.SetGlobalMatrix("_World2Camera", _camera.worldToCameraMatrix);

		float ar = _camera.aspect;
		float fov = _camera.fieldOfView;
		float viewPortHeight = Mathf.Tan(Mathf.Deg2Rad * fov * 0.5f);
		float viewPortwidth = viewPortHeight * ar;

		float newfov = fov * (1 + extraCullHeight);
		float newheight = Mathf.Tan(Mathf.Deg2Rad * newfov * 0.5f);
		float newar = viewPortwidth / (newheight);

		_camera.projectionMatrix = Matrix4x4.Perspective(newfov, newar, _camera.nearClipPlane, _camera.farClipPlane);
	}

	private void OnPreRender()
	{
		_camera.ResetProjectionMatrix();
	}
}
