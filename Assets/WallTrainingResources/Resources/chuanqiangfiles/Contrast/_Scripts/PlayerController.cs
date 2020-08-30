using UnityEngine;
using System.Collections;
//using DG.Tweening;

public enum RotationType
{
    FixedAngle,
    FreeRotate
}

/// <summary>
/// Player related Input, movement and rotation behaviour.
/// </summary>
public class PlayerController : MonoBehaviour
{

    //The script responsible for keeping track of the score.
    private ScoreManager scoreManager;

    //The script responsible for all common game and UI events.
    private EventManager eventManager;

    //The rigidbody attached to the player.
    private Rigidbody player;

    [Header("Player Movement")]

    //The behaviour of the player when rotating around the tunnel.
    [Tooltip("The way in which the player rotates around the tunnel interior.")]
    public RotationType rotationType = RotationType.FixedAngle;

    //The initial speed of the player.
    [Tooltip("The speed of the player object.")]
    public float speed = 12f;

    //The speed at which the object will rotate.
    [Tooltip("The speed at which the player object will rotate.")]
    public float rotationSpeed = 10f;

    //The value in degrees that the player will be rotated by if the rotationType is Fixed Angle.
    [Tooltip("The Fixed Angle rotation amount in degrees.")]
    public float fixedAngle = 45f;

    [Header("Miscellaneous")]

    //The Layer that the threat objects belong to.
    public LayerMask threatLayer;

    //Should input be accepted?
    public bool acceptInput = false;

    //The initial starting rotation of the player.
    Vector3 wantedRotation;

    //The start and end points for the LineCast that will detect the distance to a threat in front of the player.
    private Vector3 detectorPosition;
    private Vector3 endPosition;

    private void Awake() {

        //Cache the ScoreManager script.
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        //Cache the EventManager script and populate the necessary variables.
        eventManager = GameObject.FindObjectOfType<EventManager>();
        eventManager.playerController = this;
        eventManager.scoreManager = scoreManager;

        //Get the RigidBody attached to this object.
        player = GetComponent<Rigidbody>();
    }

    void Update()
    {

        if (acceptInput)
        {
            if (!Application.isMobilePlatform)
            {
                #region KEYBOARD INPUT.

                #region FIXED ANGLE INPUT.

                //If we are using the fixed angle rotationType.
                if (rotationType == RotationType.FixedAngle)
                {
                    //If we press the right arrow key, set the new wantedRotation.
                    if (Input.GetKeyUp("right"))
                    {
                        wantedRotation.z += fixedAngle;

                        //Tell the AudioManager that we have moved so that it can play the required sound effect.
                        AudioManager.moved = true;
                    }

                    //If we press the left arrow key, set the new wantedRotation.
                    if (Input.GetKeyUp("left"))
                    {
                        wantedRotation.z -= fixedAngle;

                        //Tell the AudioManager that we have moved so that it can play the necessary sound effect.
                        AudioManager.moved = true;
                    }
                }

                #endregion

                #region FREE ROTATE INPUT.
                //If we are using the fixed angle rotationType.
                if (rotationType == RotationType.FreeRotate)
                {
                    //If we press the right arrow key, set the new wantedRotation.
                    if (Input.GetKey("right"))
                    {
                        wantedRotation.z += (rotationSpeed * Time.deltaTime) * 10;

                        //Tell the AudioManager that we have moved so that it can play the required sound effect.
                        AudioManager.moved = true;
                    }

                    //If we press the left arrow key, set the new wantedRotation.
                    if (Input.GetKey("left"))
                    {
                        wantedRotation.z -= (rotationSpeed * Time.deltaTime) * 10;

                        //Tell the AudioManager that we have moved so that it can play the necessary sound effect.
                        AudioManager.moved = true;
                    }
                }

                #endregion

                #endregion
            }

            if (Application.isMobilePlatform)
            {
                #region TOUCH INPUT.

                foreach (Touch touch in Input.touches)
                {
                    #region FIXED ANGLE INPUT

                    if (rotationType == RotationType.FixedAngle)
                    {
                        //If we have touched the right side of the screen.
                        if (touch.phase == TouchPhase.Began && touch.position.x > Screen.width / 2)
                        {
                            wantedRotation.z += fixedAngle;
                        }

                        //If we have touched the left side of the screen.
                        if (touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2)
                        {
                            wantedRotation.z -= fixedAngle;
                        }
                    }

                    #endregion

                    #region FREE ROTATE INPUT

                    if (rotationType == RotationType.FreeRotate)
                    {
                        //If we have touched the right side of the screen.
                        if (touch.phase == TouchPhase.Began && touch.position.x > Screen.width / 2)
                        {
                            wantedRotation.z += (rotationSpeed * Time.deltaTime) * 10;
                        }

                        //If we have touched the left side of the screen.
                        if (touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2)
                        {
                            wantedRotation.z -= (rotationSpeed * Time.deltaTime) * 10;
                        }
                    }

                    #endregion

                }

                #endregion

            }
        }
    }

    void FixedUpdate()
    {
        //Move forward at a constant speed.
        player.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);

        //Rotate the player object by the specified angle.
        player.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.Euler(wantedRotation), Time.deltaTime * rotationSpeed));

    }

    //Used to detect a threat or a near miss.
    void OnTriggerEnter(Collider threat)
    {
        //Does the object that we have collided with belong to the threat layer that we have set?
        if (((1 << threat.gameObject.layer) & threatLayer) != 0)
        {
            //Disable the threat we have collided with.
            threat.gameObject.SetActive(false);

            //If we aren't accepting input, we do not want to provide feedback.
            if (acceptInput)
            {
                //If we have collided with a threat, provide visual feedback.
                //Camera.main.DOShakePosition(.3f, .2f, 50, 50);

                //Play the imact sound asociated with a game over event.
                AudioManager.failed = true;

                //Game Over.
                eventManager.GameOver();
            }

        }

    }
}
