using org.bouncycastle.crypto.modes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class SkipNSeaBobath : MonoBehaviour
    {
        public static bool triggerFlag = false;
        ComfirmBtn cfb;
        void Start()
        {
            cfb = GameObject.FindGameObjectWithTag("DerectionPanel").GetComponent<ComfirmBtn>();
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.transform.name == "palm.04.R")
            {
                cfb.ComfirmBtnFunc();
            }
        }
    }
}

