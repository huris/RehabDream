using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ShipNSea 
{
    public class SkyboxRotator : MonoBehaviour
    {

        public float speedMultiplier;

        // Update is called once per frame
        void Update()
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * speedMultiplier);
        }
    }

}
