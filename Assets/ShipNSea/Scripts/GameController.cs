using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
    public class GameController : MonoBehaviour
    {

        public enum SimulationType { None, Absolute, Relative }

        //原点(0,0,0)
        public Transform centerPoint;

        [Header("References")]
        public GameObject boat;
        //船内有个小球
        public GameObject posIndicator;
        public AudioSource boatAudio;
        // public GameObject spawningRangeRoot;

        [Header("Properties")]
        //
        public float maxSpeed;
        public Vector2 rangeSize;

        [Header("Kinect")]
        public bool kinect = false;
        //叫什么Kinect敏感度 默认值给80 就是一个系数
        public float kinectSensitivity;
        public GravityCenter gravityCenterScript;

        [Header("Control Simulation")]
        public SimulationType simulationType;
        public float relativeSensitivity = 10f;

        [Header("Game Settings")]
        public int totalTime; // s
        public GameObject centerFGO;
        public GameObject leftFGO;
        public GameObject rightFGO;

        [Header("Sound Effects")]
        public Vector2 volumeRange = new Vector2(0.1f, 1f);
        public Vector2 pitchRange = new Vector2(0.5f, 1f);

        [Header("我的加分Prefab")]
        public GameObject gotPointTextMeshPro;
        [Header("我的Canvas")]
        public Canvas canvas;

        public bool Speeding { get { return _isSpeeding; } }
        public List<GameObject> StaticFlocks { get { return _staticFlocks; } }
        public int StaticCounter { get { return _staticCounter; } }
        public int MovingCounter { get { return _movingCounter; } }
        public int TotalScore { get { return _totalScore; } }
        public int LastScore { get { return _lastScore; } }
        public float CurrentTime { get { return _currentTime; } }
        public Vector2 RelativeGravityCenter { get { return _relativeGravityCenter; } }

        public delegate void GetCoinDelegate(int count);
        public event GetCoinDelegate OnGetCoin;


        private bool _isSpeeding = false;
        private Vector2 _lastSizedInput = Vector2.zero;
        private List<GameObject> _staticFlocks = new List<GameObject>();
        private int _lastScore = 0;
        private int _totalScore = 0;
        private int _staticCounter = 0;
        private int _movingCounter = 0;
        public static float _currentTime = 0;
        private Vector2 _relativeGravityCenter;

        private GameState _gameState;



        public void AddFlock(GameObject go)
        {
            StaticFlocks.Add(go);
        }
        private WaitForSeconds wait = new WaitForSeconds(.01f);
        IEnumerator IGotPoint(FishFlock fishFlock)
        {
            for (int i = 0; i < fishFlock.score; i++)
            {
                yield return wait;
                _totalScore += 1;
                _lastScore += 1;
            }
        }

        public void CompleteFishingFlock(GameObject go, FlockType type)
        {

            FishFlock fishFlock = go.GetComponent<FishFlock>();
            //_totalScore += fishFlock.score;
            //_lastScore = fishFlock.score;
            StartCoroutine(IGotPoint(fishFlock));
            OnGetCoin?.Invoke(fishFlock.score);

            StaticFlocks.Remove(go);

            if (type == FlockType.Static)
            {
                _staticCounter += 1;
                TextCoordinate.type = FishType.StaticFish;
            }
            else if (type == FlockType.Moving)
            {
                _movingCounter += 1;
                TextCoordinate.type = FishType.DynamicFish;
            }
            //Instantiate(gotPointTextMeshPro, canvas.transform);
        }

        public void Reset()
        {
            _totalScore = 0;
            _lastScore = 0;
            _staticFlocks.Clear();
            _staticCounter = 0;
            _movingCounter = 0;
            _currentTime = 0;
            //TODO
        }

        void Start()
        {
            // Reset();
            _gameState = GameManager.instance.currentLevel as GameState;
            //改变时间
            totalTime = GameObject.Find("KinectManager").GetComponent<GameTime>().gameTimeTotal;
            //改变身体侧重方向
            switch (GameObject.Find("KinectManager").GetComponent<BodySetting>().setBody)
            {
                case BodySettingEnum.center:
                    centerFGO.SetActive(true);
                    leftFGO.SetActive(false);
                    rightFGO.SetActive(false);
                    break;
                case BodySettingEnum.left:
                    centerFGO.SetActive(false);
                    leftFGO.SetActive(true);
                    rightFGO.SetActive(false);
                    break;
                case BodySettingEnum.right:
                    centerFGO.SetActive(false);
                    leftFGO.SetActive(false);
                    rightFGO.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(rangeSize.x, 0f, rangeSize.y));
        }

        void Update()
        {

            Vector2 sizedInput = Vector2.zero; // absolute // ALWAYS CHANGE THIS
            Vector3 indicatorPos = posIndicator.transform.position; ;
            // Kinect input (absolute)
            if (kinect && KinectManager.Instance.IsInitialized())
            {

                if (KinectManager.Instance.GetPrimaryUserID() == 0)
                {
                    _gameState.FinishGame();
                }
                //脖子节点
                Vector3 center = gravityCenterScript.GetGravityCenter();
                //原点0,0,0
                Vector3 footCenter = centerPoint.position;
                //脖子节点->v.zero
                Vector3 diff = center - footCenter;

                _relativeGravityCenter = new Vector2(diff.x, diff.z);

                //print("_relativeGravityCenter:"+ _relativeGravityCenter.x+","+ _relativeGravityCenter.y);

                _gameState.RecordGCenter(diff.x.ToString() + "," + diff.z.ToString());

                sizedInput.x = Mathf.Clamp(_relativeGravityCenter.x * kinectSensitivity, -rangeSize.x / 2, rangeSize.x / 2);
                sizedInput.y = Mathf.Clamp(_relativeGravityCenter.y * kinectSensitivity, -rangeSize.y / 2, rangeSize.y / 2);

                //print("sizedInput:"+sizedInput.x + "," + sizedInput.y);
                //print("rangeSize" + (rangeSize.x) + "," + rangeSize.y);
            }

            #region 无用代码
            //switch (simulationType) {
            //    case SimulationType.Absolute:
            //        RaycastHit hitInfo;
            //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //        if (Physics.Raycast(ray, out hitInfo, 1000f, LayerMask.GetMask("RaycastBlocker"))) {
            //            Vector3 point = hitInfo.point;
            //            sizedInput.x = Mathf.Clamp(point.x, -rangeSize.x / 2, rangeSize.x / 2);
            //            sizedInput.y = Mathf.Clamp(point.z, -rangeSize.y / 2, rangeSize.y / 2);
            //        }
            //        break;
            //    case SimulationType.Relative:
            //        float h = Input.GetAxis("Horizontal");
            //        float v = Input.GetAxis("Vertical");

            //        sizedInput = new Vector2(indicatorPos.x + h * relativeSensitivity * Time.deltaTime, indicatorPos.z + v * relativeSensitivity * Time.deltaTime);
            //        break;
            //    default:
            //        break;
            //} 
            #endregion

            // Indicator
            indicatorPos.x = sizedInput.x;
            indicatorPos.z = sizedInput.y;
            //把小球的位置改变到 sizedInput位置
            posIndicator.transform.position = indicatorPos;

            // Boat
            if (indicatorPos != boat.transform.position)
            {
                boat.transform.LookAt(indicatorPos);
            }
            //
            Vector2 sizedInputSpeed = (sizedInput - _lastSizedInput) / Time.deltaTime;
            _lastSizedInput = sizedInput;
            //maxspeed 15
            if (sizedInputSpeed.magnitude > maxSpeed)
            {
                _isSpeeding = true;
            }
            if (_isSpeeding)
            {
                //向球的方向走
                Vector3 realSpeed = (indicatorPos - boat.transform.position).normalized * maxSpeed;

                Vector3 boatPos = boat.transform.position;
                boatPos += realSpeed * Time.deltaTime;
                boat.transform.position = boatPos;

                if ((indicatorPos - boat.transform.position).magnitude < maxSpeed * Time.deltaTime)
                {
                    _isSpeeding = false;
                }
            }
            else
            {
                boat.transform.position = indicatorPos;
            }

            // Time
            _currentTime += Time.deltaTime;
            if (_currentTime > totalTime)
            {
                _gameState.FinishGame();
            }

            // Cheating
            if (Input.GetKey(KeyCode.Delete))
            {
                _gameState.FinishGame();
            }
        }





    }
}

