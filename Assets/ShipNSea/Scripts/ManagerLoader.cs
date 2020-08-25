using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class ManagerLoader : MonoBehaviour
    {

        public List<GameObject> managers = new List<GameObject>();

        void Awake()
        {

            foreach (GameObject go in managers)
            {
                if (!transform.Find(go.name))
                {
                    Instantiate(go);
                }
            }

            Destroy(gameObject);
        }
    }
}

