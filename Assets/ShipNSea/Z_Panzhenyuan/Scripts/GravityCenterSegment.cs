using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShipNSea 
{
    [System.Serializable]
    public class GravityCenterSegment : MonoBehaviour
    {
        public string name;

        [Tooltip("肢体远端定标点位置。")]
        public HumanBodyBones boneD;

        [Tooltip("肢体近端定标点位置。")]
        public HumanBodyBones boneP;

        [Tooltip("节段重心的位置百分比。")]
        [Range(0f, 1f)]
        public float com = 0.5f;

        [Tooltip("身体节段重量占全身重量的百分比。")]
        [Range(0f, 1f)]
        public float mi;

        [Tooltip("Customizable segment weight. Always set to 1 if unsure.")]
        [Range(0f, 1f)]
        public float weight = 1f;
    }

}


