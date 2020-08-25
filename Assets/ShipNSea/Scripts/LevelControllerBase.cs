using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public abstract class LevelControllerBase : MonoBehaviour
    {

        public string levelName;

        public virtual void Start()
        {
            GameManager.instance.currentLevel = this;
        }

        public virtual void Update()
        {

        }
    }
}

