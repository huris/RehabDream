using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShipNSea 
{
    [System.Serializable]
    public class GravityCenterProfile
    {
        public string name;
        public List<GravityCenterSegment> segments;
    }

    //[RequireComponent(typeof(Animator))]
    public class GravityCenter : MonoBehaviour
    {
        public Dictionary<int, float> gravityDic = new Dictionary<int, float>();
        public int usingProfile = 0;
        private bool visualize = true;
        public bool fakeGravityCenter = false;
        public float time = 0;
        public int x = 0;
        public static GravityCenter instance;
        Animator animator;
        void OnDrawGizmos()
        {
            //if (visualize) {
            //    if (GetGravityCenter() == null)
            //    {
            //        return;
            //    }
            //    Vector3 point = GetGravityCenter();
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawSphere(point, .5f);
            //}
        }
        void Start()
        {
            animator = GetComponent<Animator>();
            if (instance == null)
            {
                instance = this;
            }
        }
        public void UpdateData()
        {
            //记录关于重心位置
            time += Time.deltaTime;
            if (time > 2f)
            {
                var spine = animator.GetBoneTransform(HumanBodyBones.Neck).position;
                var manPos = transform.localPosition;
                //print("线1:"+(new Vector3(manPos.x, manPos.y + 200, manPos.z) - manPos).normalized);
                //print("线2:"+ (spine - manPos).normalized);
                gravityDic.Add(x, Vector3.Dot((new Vector3(manPos.x, manPos.y + 200, manPos.z) - manPos).normalized, (spine - manPos).normalized) * Mathf.Rad2Deg);
                x++;
                time = 0;
            }
        }
        public Vector3 GetGravityCenter()
        {
            if (animator == null)
            {
                return Vector3.zero;
            }
            return animator.GetBoneTransform(HumanBodyBones.Spine).position;
        }
    }
}


