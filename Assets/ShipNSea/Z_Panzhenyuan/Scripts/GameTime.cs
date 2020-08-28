using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class GameTime : MonoBehaviour
    {
        public static int gameTimeTotal = 300;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}
