using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipNSea 
{
    [RequireComponent(typeof(Image))]
    public class ProgressRing : MonoBehaviour
    {

        private Image _image;

        void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetValue(float value)
        {
            _image.fillAmount = value;

        }

    }

}
