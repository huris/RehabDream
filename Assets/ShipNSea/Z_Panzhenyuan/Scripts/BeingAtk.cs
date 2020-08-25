using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShipNSea 
{
    public class BeingAtk : MonoBehaviour
    {
        public Slider spSlider;
        public Slider spBufferSlider;
        public static int damage = 10;
        public static int staminaPoint = 100;
        public Image beingAtkImage;
        //public Rigidbody boatRigidBady;
        bool pauseBool = false;
        //委托是一种特殊的数据结构,他是方法的集合,能够存储与其规定类型相同的方法
        //public UnityAction<float, float, float> unityAction;
        void OnTriggerEnter(Collider collider)
        {

            if (collider.tag == "BarrierL" || collider.tag == "BarrierM" || collider.tag == "BarrierS")
            {
                StartCoroutine(CameraShackScript.GetInstance().CameraShackFunc(1, 1, 1, beingAtkImage));
            }

            if (collider.tag == "BarrierL")
            {
                damage = 20;
                staminaPoint -= damage;
                spSlider.value = staminaPoint / 100f;
                StartCoroutine(WaitChangePauseBool());
                UserLevenController.instance.DeductExp(500);
                Destroy(collider.gameObject);
            }
            else if (collider.tag == "BarrierM")
            {
                damage = 15;
                staminaPoint -= damage;
                spSlider.value = staminaPoint / 100f;
                StartCoroutine(WaitChangePauseBool());
                UserLevenController.instance.DeductExp(300);
                Destroy(collider.gameObject);
            }
            else if (collider.tag == "BarrierS")
            {
                damage = 10;
                staminaPoint -= damage;
                spSlider.value = staminaPoint / 100f;
                StartCoroutine(WaitChangePauseBool());
                UserLevenController.instance.DeductExp(200);
                Destroy(collider.gameObject);
            }
        }

        void Update()
        {
            //SP缓冲条的动画
            BeingAtkByStoneBuffer();
            EndGameBySP();
        }
        void BeingAtkByStoneBuffer()
        {
            if (pauseBool)
            {
                spBufferSlider.value = Mathf.Lerp(spBufferSlider.value, spSlider.value, 0.1f);
                if (spBufferSlider.value <= spSlider.value * 0.01f)
                {
                    pauseBool = false;
                }
            }
        }
        IEnumerator WaitChangePauseBool()
        {
            pauseBool = false;
            yield return new WaitForSeconds(1f);
            pauseBool = true;
        }


        /// <summary>
        /// 提供一个 当Sp值清零的结束游戏的方法
        /// </summary>
        void EndGameBySP()
        {
            if (staminaPoint <= 0)
            {
                GameState.instance.FinishGame();
            }
        }
    }
}

