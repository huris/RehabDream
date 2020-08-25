using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public enum BodySettingEnum
    {
        center = 0,
        left = 1,
        right = 2
    }
    public class BodySetting : MonoBehaviour
    {
        public BodySettingEnum setBody = 0;
    }

}
