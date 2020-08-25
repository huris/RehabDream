using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ShipNSea 
{
    public enum FlockType { Static, Moving }

    [RequireComponent(typeof(SphereCollider))]
    public class FishFlock : MonoBehaviour
    {
        public FlockType type;
        [HideInInspector]
        public static float captureTime = 1f;
        public float radius = 7f;
        public int score;
        public TextMeshPro counter;
        public GameObject fishIconQuad;
        private UserLevenController userLevenController;

        private float _currentCaptureTime = 0f;
        private bool _isCapturing = false; // Pause switch

        private GameController gameController { get { return (GameManager.instance.currentLevel as GameState).gameController; } }

        //记录每次捕鱼花费时间 静态 公共持有不被对象删除就好了
        public static List<float> fishFlockTimeList = new List<float>();
        //记录补了多少鱼
        public static int catchFishCount = 0;
        private void Start()
        {
            GetComponent<SphereCollider>().radius = radius;
            userLevenController = GameObject.Find("Canvas/InGame/UserLv").GetComponent<UserLevenController>();
        }

        void Update()
        {
            if (_isCapturing)
            {
                _currentCaptureTime += Time.deltaTime;
                //counter.text = ((int)(captureTime - _currentCaptureTime + 1)).ToString();
                if (_currentCaptureTime >= captureTime)
                {
                    FinishCapture();
                }
            }
        }
        void OnEnable()
        {
            if (MapDetectionController.mapOccupyDis.ContainsKey(gameObject.tag))
            {
                MapDetectionController.mapOccupyDis[gameObject.tag].Add(transform);
            }
            else
            {
                var list = new List<Transform>();
                list.Add(transform);
                MapDetectionController.mapOccupyDis.Add(gameObject.tag, list);
            }
        }
        void OnDestroy()
        {
            if (MapDetectionController.mapOccupyDis.ContainsKey(gameObject.tag))
            {
                MapDetectionController.mapOccupyDis[gameObject.tag].Remove(transform);
            }
        }
        private void FinishCapture()
        {

            _isCapturing = false;
            gameController.CompleteFishingFlock(gameObject, type);
            //获得得分
            userLevenController.GetLevel();
            //TODO记录每次捕鱼的时间
            fishFlockTimeList.Add((float)Math.Round(GameState.time, 1));
            GameState.time = 0f;
            //for (int i = 0; i < fishFlockTimeList.Count; i++)
            //{
            //    Debug.LogError(fishFlockTimeList[i]);
            //}
            //删除当前元素 会把当前对象的list列表删除
            catchFishCount++;
            Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                _isCapturing = true;
                //counter.enabled = true;
                fishIconQuad.SetActive(false);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                _isCapturing = false;
                _currentCaptureTime = 0f;
                //counter.enabled = false;
                fishIconQuad.SetActive(true);
            }

        }
    }

}
