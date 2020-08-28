using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Hierarchy:
 * 
 * StaticSpawnRanges
 *      |--SpawnRange (you are here)
 *             |--FishFlock
 */

namespace ShipNSea 
{
    [System.Serializable]
    public class FanArea
    {
        public float radius;
        [Range(0f, 360f)]
        public float eulerAngle;
        [Range(0f, 360f)]
        public float rotation;

        public Vector2 RandomPointInsideArea()
        {
            float rAngle = Random.Range(0f, eulerAngle) + rotation;
            float rRadius = Random.Range(0f, radius);

            float x = rRadius * Mathf.Cos(rAngle * Mathf.PI / 180);
            float y = rRadius * Mathf.Sin(rAngle * Mathf.PI / 180);

            return new Vector2(x, y);
        }
    }

    public class SpawnRange : MonoBehaviour
    {

        public FanArea area;
        public Vector2 randomIntervalRange;
        public Vector2 flockCountRange;
        public List<GameObject> possibleFlockPrefabs;

        private bool _shouldGenerateFlock = false;
        private bool _generatorWaiting = false;

        private static int maxCollisionScanTime = 10;
        private List<GameObject> StaticFlocks { get { return (GameManager.instance.currentLevel as GameState).gameController.StaticFlocks; } }
        private int InRangeFlockCount { get { return transform.childCount; } }

        //记录产生鱼群的总数fishCount
        public static int fishCount = 0;
        public static int staticFishCount = 0;
        public static int dynamicFishCount = 0;


        private float time = .5f;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, new Vector3(Mathf.Cos(area.rotation * Mathf.PI / 180), 0f, Mathf.Sin(area.rotation * Mathf.PI / 180)), area.eulerAngle, area.radius);
        }
#endif

        void Update()
        {
            if (InRangeFlockCount < flockCountRange.x)
            {
                _shouldGenerateFlock = true;
            }
            else if (InRangeFlockCount >= flockCountRange.y)
            {
                _shouldGenerateFlock = false;
            }

            if (_shouldGenerateFlock && !_generatorWaiting)
            {
                GenerateFlock();
                StartCoroutine(WaitAndToggleFlockGenerator(2f));
            }
        }
        public void GenerateFlock()
        {
            if (possibleFlockPrefabs.Count == 0)
            {
                Debug.LogError("No flock prefabs.");
            }

            Vector3 globalPosition = Vector3.zero;

            bool shouldSpawn = false;

            for (int i = 0; i <= maxCollisionScanTime; ++i)
            {
                Vector2 p = area.RandomPointInsideArea();
                globalPosition = new Vector3(p.x, 0f, p.y) + transform.position;
                if (!DetectCollision(globalPosition))
                {
                    shouldSpawn = true;
                    break;
                }
            }

            if (!shouldSpawn)
            {
                return;
            }
            GameObject go = possibleFlockPrefabs[Random.Range(0, possibleFlockPrefabs.Count - 1)];
            //如果在障碍物周围或者,船周围就不产生鱼,鱼的周围也不会产生鱼
            foreach (var item in MapDetectionController.mapOccupyDis)
            {

                if (item.Key == "BarrierS")
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var dis = Vector3.Distance(globalPosition, item.Value[i].position);
                        //print("dis:" + (dis));
                        if (dis - 2 <= 8)
                        {
                            //print("DIS+POSVOLUME:"+dis+posVolume);
                            return;
                        }
                    }
                }
                if (item.Key == "BarrierM")
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var dis = Vector3.Distance(globalPosition, item.Value[i].position);
                        //print("dis+posVolume:" + (dis + posVolume));
                        //print("dis:" + (dis));
                        if (dis - 2 <= 12)
                        {
                            //print("DIS+POSVOLUME:" + dis + posVolume);
                            return;
                        }
                    }
                }
                if (item.Key == "BarrierL")
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var dis = Vector3.Distance(globalPosition, item.Value[i].position);
                        //print("dis+posVolume:" + (dis + posVolume));
                        //print("dis:" + (dis));
                        if (dis - 2 <= 15)
                        {
                            //print("DIS+POSVOLUME:" + dis + posVolume);
                            return;
                        }
                    }
                }
                if (item.Key == "Player")
                {
                    var dis = Vector3.Distance(globalPosition, MapDetectionController.mapOccupyDis["Player"][0].position);
                    if (dis - 8 <= 15)
                    {
                        return;
                    }
                }
                if (item.Key == "Flock")
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var dis = Vector3.Distance(globalPosition, item.Value[i].position);
                        //print("dis+posVolume:" + (dis + posVolume));
                        //print("dis:" + (dis));
                        if (dis - 5 <= 15)
                        {
                            //print("DIS+POSVOLUME:" + dis + posVolume);
                            return;
                        }
                    }
                }
            }
            var fish = Instantiate(go, globalPosition, Quaternion.identity, transform);
            (GameManager.instance.currentLevel as GameState).gameController.AddFlock(go);
            fishCount++;
            if (fish.GetComponent<FishFlock>().type == FlockType.Moving) 
            {
                dynamicFishCount++;
            }
            else 
            {
                staticFishCount++;
            }

        }

        public bool DetectCollision(Vector3 p)
        {

            foreach (GameObject go in StaticFlocks)
            {
                FishFlock flock = go.GetComponent<FishFlock>();
                if (Vector3.Distance(go.transform.position, p) < flock.radius * 2)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator WaitAndToggleFlockGenerator(float time)
        {
            _generatorWaiting = true;
            yield return new WaitForSeconds(time);
            if (_generatorWaiting)
            {
                _generatorWaiting = false;
            }
        }
    }

}
