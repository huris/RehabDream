﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ShipNSea 
{
    public class QuitGameController : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                print("esc");
                SceneManager.LoadScene("Intro");
            }
        }
    }

}
