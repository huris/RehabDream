using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class FishFlockMoveScript : MonoBehaviour
    {

        // Update is called once per frame
        private float speed = .1f;
        public GameObject circleLoopStatic;
        public GameObject circleLoopDynamic;
        public bool flag;
        public float rNum = .7f;
        FishFlock fishFlock;
        void OnTriggerEnter(Collider collider)
        {
            //print(collider.name);

        }
        void OnEnable()
        {
            flag = Random.Range(0f, 1f) >= rNum ? flag = true : flag = false;
            //动态鱼
            if (flag)
            {
                fishFlock = GetComponent<FishFlock>();
                fishFlock.type = FlockType.Moving;
                fishFlock.score = 150;
                speed = Random.Range(-0.5f, 0.5f);
            }
        }
        void Move(bool flag)
        {
            if (flag)
            {
                if (!circleLoopDynamic.activeSelf)
                {
                    circleLoopDynamic.SetActive(true);
                    circleLoopStatic.SetActive(false);
                }
                Quaternion rotation = Quaternion.AngleAxis(speed, Vector3.up);
                transform.position = rotation * transform.position;
            }
            else
            {
                if (!circleLoopStatic.activeSelf)
                {
                    circleLoopStatic.SetActive(true);
                    circleLoopDynamic.SetActive(false);
                }
            }
        }
        void Update()
        {
            Move(flag);
        }

    }

}
