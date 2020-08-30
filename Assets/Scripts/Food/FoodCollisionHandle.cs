using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollisionHandle : MonoBehaviour {

    public delegate void GetFood(); // what will happen when Get Food
    public event GetFood OnGetFood;

    public delegate void PutFood();
    public event PutFood OnPutFood;

    [Header("Tag")]
    public const string AppleTrigger = "Apple";
    public const string PotatoTrigger = "Potato";
    public const string CucumberTrigger = "Cucumber";
    public const string WashTrigger = "Wash";


    private bool _HaveGotten = false;

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
            case WashTrigger:   //到达洗菜池
            
                Debug.Log("Wash");

                InvokePutFood();
                break;

            default:

                break;


        }

    }



    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case AppleTrigger:
                Debug.Log("Apple");
                InvokeGetFood();
                break;

            case PotatoTrigger:
                Debug.Log("Potato");
                InvokeGetFood();
                break;

            case CucumberTrigger:
                Debug.Log("Cucumber");
                InvokeGetFood();
                break;

            default:

                break;


        }

    }

    private void InvokeGetFood()
    {
        // 只有第一次触发事件
        if (_HaveGotten == false)
        {
            OnGetFood?.Invoke();
            _HaveGotten = true;
        }
    }

    private void InvokePutFood()
    {
        // 只有第一次触发事件
        if (_HaveGotten == true)
        {
            OnPutFood?.Invoke();
            _HaveGotten = false;
        }
    }
}
