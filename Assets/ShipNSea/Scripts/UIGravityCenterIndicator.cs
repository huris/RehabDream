using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class UIGravityCenterIndicator : MonoBehaviour
    {

        public GameController gameController;
        public float sensitivity;
        public RectTransform indicatorTransform;
        public float maxRange;


        void Update()
        {
            if (gameController == null)
            {
                return;
            }
            Vector2 center = gameController.RelativeGravityCenter * sensitivity;
            center.x = Mathf.Clamp(center.x, -maxRange, maxRange);
            center.y = Mathf.Clamp(center.y, -maxRange, maxRange);
            Vector3 pos = indicatorTransform.localPosition;
            pos.x = center.x;
            pos.y = center.y;
            indicatorTransform.localPosition = pos;
        }
    }
}

