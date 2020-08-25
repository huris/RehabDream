using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    [RequireComponent(typeof(LineRenderer))]
    public class PositionIndicatorLine : MonoBehaviour
    {

        public GameController gameController;
        public float uvScrollSpeed = 1f;
        public float textureTilingXRatio = 1; // Tiling X / length

        private LineRenderer _lineRenderer;
        private bool _visible = true;

        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();

        }

        // Update is called once per frame
        void Update()
        {
            if (gameController.Speeding)
            {
                if (_visible == false)
                {
                    _lineRenderer.enabled = true;
                }
                _visible = true;
            }
            else
            {
                if (_visible == true)
                {
                    _lineRenderer.enabled = false;
                }
                _visible = false;
            }

            if (_visible)
            {
                Vector3[] array = { gameController.boat.transform.position, gameController.posIndicator.transform.position };
                _lineRenderer.SetPositions(array);

                float offset = Time.time * uvScrollSpeed;
                float distance = Vector3.Distance(gameController.boat.transform.position, gameController.posIndicator.transform.position);
                _lineRenderer.material.mainTextureOffset = new Vector2(offset % 1, 0f);
                _lineRenderer.material.mainTextureScale = new Vector2(distance * textureTilingXRatio, 1f);
            }
        }
    }
}

