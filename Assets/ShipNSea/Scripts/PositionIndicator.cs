using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class PositionIndicator : MonoBehaviour
    {

        public GameController gameController;

        private bool _visible = true;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (gameController.Speeding)
            {
                if (_visible == false)
                {
                    GetComponent<MeshRenderer>().enabled = true;
                }
                _visible = true;
            }
            else
            {
                if (_visible == true)
                {
                    GetComponent<MeshRenderer>().enabled = false;
                }
                _visible = false;
            }
        }
    }
}

