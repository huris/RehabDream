using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandle : MonoBehaviour
{

    public delegate void GoalkeeperFail(); // what happen when Goalkeeper fail
    public event GoalkeeperFail OnGoalkeeperFail;

    public delegate void GoalkeeperWin();  // what happen when Goalkeeper win
    public event GoalkeeperWin OnGoalkeeperWin;

    public bool SessionOver = false;   //false = this session is not finished 

    [Header("Tag")]
    public const string GateTrigger = "GateTrigger";
    public const string GoalkeeperTrigger = "GoalkeeperTrigger";
    public const string NetTrigger = "NetTrigger";
    public const string LeftNetTrigger = "LeftNetTrigger";
    public const string RightNetTrigger = "RightNetTrigger";
    public const string TopNetTrigger = "TopNetTrigger";
    public const string ButtomNetTrigger = "ButtomNetTrigger";
    public const string BackNetTrigger = "BackNetTrigger";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // happen when soccer collider
    private void OnTriggerEnter(Collider other)
    {

        switch (other.tag)
        {
            case GateTrigger:  //Goalkeeper fail
                if (SessionOver) //if this session is over
                {
                    return;
                }
                OnGoalkeeperFail?.Invoke();
                SessionOver = true;
                Debug.Log("Goalkeeper fail");
                break;

            default:

                break;


        }

    }



    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {

            case GoalkeeperTrigger:    //Goalkeeper win
                if (SessionOver) //if this session is over
                {
                    return;
                }
                OnGoalkeeperWin?.Invoke();
                SessionOver = true;
                Debug.Log("Goalkeeper win!!!!!!!!!!!!!!");
                break;

            case LeftNetTrigger:
                Debug.Log("Crash left net!");
                break;
            case RightNetTrigger:
                Debug.Log("Crash right net!");
                break;
            case ButtomNetTrigger:
                Debug.Log("Crash buttom net!");
                break;
            case TopNetTrigger:
                Debug.Log("Crash top net!");
                break;
            case BackNetTrigger:
                Debug.Log("Crash back net!");
                break;
            default:

                break;


        }

    }

    public void Reset()    //reset
    {
        SessionOver = false;
    }
}
