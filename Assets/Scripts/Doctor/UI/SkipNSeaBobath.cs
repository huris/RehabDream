using org.bouncycastle.crypto.modes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                DoctorDataManager.instance.doctor.patient.FishTrainingPlays.Last().SetTrainingStartTime(DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                cfb.ComfirmBtnFunc();
            }
        }
    }
}

