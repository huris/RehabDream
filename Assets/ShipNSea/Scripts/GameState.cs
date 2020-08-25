using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.CodeDom.Compiler;

namespace ShipNSea 
{
    public class GameState : LevelControllerBase
    {

        public static GameState instance;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public GameController gameController;
        public GameObject staticSpawnRangeGO;
        public GameObject tutorialPopupGO;
        public GameObject inGameGO;
        public GameObject settingsPopupGO;
        public GameObject pausePopupGO;
        public GameObject gameoverGO;
        public Text totalScoreText;
        public Text lastScoreText;
        public Text currentTimeText;
        public Text totalTimeText;
        public Text staticCounterText;
        public Text movingCounterText;
        public Text totalTime;
        public Text totalScore;
        public Text totalStaticFlockCount;
        public Text totalMovingFlockCount;
        public ProgressRing timeProgressRing;
        public FistButton settingsFistButton;
        public FistButton pauseFistButton;
        public AudioClip catchCompleteSE;
        public AudioClip bgm;
        public AudioClip bgs;
        public AvatarController playerAvatarController;
        public GameObject dataCollection;
        private DataCollection dataCollectionScript;
        public int photoCaptureInterval = 3;

        private bool _inGame = false;
        public static float time = 0f;

        public static UserDAO outUserDAO = new UserDAO();

        private static void GetUserDAO(string name) 
        {

            //提供USERDAO
            var temp = PlayerPrefs.GetString(name);
            print(temp);
            var tempstring = temp.Split('|');
            outUserDAO.username = tempstring[0];
            outUserDAO.password = tempstring[1];
            outUserDAO.experience = int.Parse(tempstring[2]);
            outUserDAO.trainTime = Mathf.Round(GameController._currentTime).ToString();
            outUserDAO.catchFishCount = FishFlock.catchFishCount.ToString();
            outUserDAO.distance = Mathf.Round(DataCollection.dis).ToString();
            outUserDAO.gotExp = (FishFlock.catchFishCount * 100).ToString();
            outUserDAO.gList = DataCollection.gAngleList;
        }

        public bool InGame
        {
            get
            {
                return _inGame;
            }
        }
        private float _lastScoreTimer = 0f;

        private string startTime;
        private string endTime;

        private StreamWriter gravityCenterWriter;

        private int photoCounter = 1;

        public override void Start()
        {
            base.Start();

            SpawnRangeEnabler(false);
            gameController.enabled = false;
            tutorialPopupGO.SetActive(true);
            SoundManager.instance.Play(bgs, SoundManager.SoundType.BGS, 1f);
            gameController.OnGetCoin += GameController_OnGetCoin;
            dataCollectionScript = dataCollection.GetComponent<DataCollection>();
        }

        private void GameController_OnGetCoin(int count)
        {
            lastScoreText.text = count.ToString();
            lastScoreText.enabled = true;
            _lastScoreTimer = 0f;
            SoundManager.instance.Play(catchCompleteSE, SoundManager.SoundType.SE);
        }

        public override void Update()
        {
            base.Update();

            if (lastScoreText.enabled)
            {
                _lastScoreTimer += Time.deltaTime;
                if (_lastScoreTimer > 1f)
                {
                    lastScoreText.enabled = false;
                }
            }

            if (_inGame)
            {
                //计时器
                time += Time.deltaTime;
                totalScoreText.text = gameController.TotalScore.ToString();
                // lastScoreText.text = gameController.LastScore.ToString();
                totalTimeText.text = gameController.totalTime.ToString();
                currentTimeText.text = ((int)gameController.CurrentTime).ToString();
                staticCounterText.text = gameController.StaticCounter.ToString();
                movingCounterText.text = gameController.MovingCounter.ToString();

                timeProgressRing.SetValue(gameController.CurrentTime / gameController.totalTime);
                GravityCenter.instance.UpdateData();
            }
            else
            {

                //if (GravityCenter.instance.x != 0)
                //{
                //    foreach (var item in GravityCenter.instance.gravityDic)
                //    {
                //        print(item.Key+":"+item.Value);
                //    }
                //}
                //GravityCenter.instance.x = 0;
            }
        }

        private void OnApplicationQuit()
        {
            if (GameManager.instance.currentUser != null)
            {
                gravityCenterWriter.Close();
            }
        }

        public void SkipTutorial_Click()
        {
            tutorialPopupGO.SetActive(false);
            inGameGO.SetActive(true);
            PlayGame();
            dataCollectionScript.StartDataCollenctionFunc();
        }

        public void RestartButton_Click()
        {
            SceneManager.LoadScene("BalanceFishing");
        }

        public void ReturnButton_Click()
        {
            SoundManager.instance.StopSounds();
            SceneManager.LoadScene("Intro");
        }

        public void PauseButton_Click()
        {
            PauseGame();
            pausePopupGO.SetActive(true);

        }

        public void ResumeFromPauseButton_Click()
        {
            PlayGame();
            pausePopupGO.SetActive(false);
        }

        public void SettingsButton_Click()
        {
            PauseGame();
            settingsPopupGO.SetActive(true);
        }

        public void ResumeFromSettingsButton_Click()
        {
            PlayGame();
            settingsPopupGO.SetActive(false);
        }

        public void PlayGame()
        {
            gameEndBool = false;
            _inGame = true;
            gameController.enabled = true;
            gameController.boatAudio.enabled = true;
            SpawnRangeEnabler(true);
            settingsFistButton.enabled = true;
            pauseFistButton.enabled = true;
            SoundManager.instance.Play(bgm, SoundManager.SoundType.BGM);

            playerAvatarController.playerId = KinectManager.Instance.GetPrimaryUserID();
            if (!KinectManager.Instance.avatarControllers.Contains(playerAvatarController))
            {
                KinectManager.Instance.avatarControllers.Add(playerAvatarController);
            }

            startTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            if (GameManager.instance.currentUser != null)
            {

                if (!Directory.Exists(GameManager.instance.currentUser.UserFolderPath + startTime))
                {
                    Directory.CreateDirectory(GameManager.instance.currentUser.UserFolderPath + startTime);
                }

                string gravityCenterFileName = GameManager.instance.currentUser.UserFolderPath + startTime + "/" + "GravityCenter.txt";
                gravityCenterWriter = new StreamWriter(gravityCenterFileName);
            }

            StartCoroutine(WaitAndTakeKinectPhoto(photoCaptureInterval));
        }

        public void PauseGame()
        {
            gameEndBool = true;
            _inGame = false;
            gameController.enabled = false;
            gameController.boatAudio.enabled = false;
            SpawnRangeEnabler(false);
            settingsFistButton.enabled = false;
            pauseFistButton.enabled = false;
            if (MapDetectionController.mapOccupyDis.ContainsKey("Flock"))
            {
                for (int i = 0; i < MapDetectionController.mapOccupyDis["Flock"].Count; i++)
                {
                    MapDetectionController.mapOccupyDis["Flock"][i].GetComponent<FishFlockMoveScript>().flag = false;
                }
            }
        }

        public void FinishGame()
        {
            PauseGame();
            gameoverGO.SetActive(true);

            string totalTimeString = ((int)gameController.CurrentTime).ToString();
            string totalScoreString = gameController.TotalScore.ToString();
            string totalStaticFlockCountString = gameController.StaticCounter.ToString();
            string totalMovingFlockCountString = gameController.MovingCounter.ToString();

            endTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            if (GameManager.instance.currentUser != null)
            {
                gravityCenterWriter.Close();
            }

            StopCoroutine("WaitAndTakeKinectPhoto");

            totalTime.text = totalTimeString;
            totalScore.text = totalScoreString;
            totalStaticFlockCount.text = totalStaticFlockCountString;
            totalMovingFlockCount.text = totalMovingFlockCountString;

            var userLevenController = GameObject.Find("Canvas/InGame/UserLv").GetComponent<UserLevenController>();
            // SAVE TO DB
            if (IntroState.isConnectToMySql)
            {
                //try
                //{
                //    using (MySqlCommand cmd = new MySqlCommand("update user set experience = @ex where id = @id", IntroState.conn))
                //    {
                //        cmd.Parameters.AddWithValue("ex", UserLevenController.exp);
                //        cmd.Parameters.AddWithValue("id", IntroState.id);
                //        cmd.ExecuteNonQuery();
                //    }
                //    // print("更新成功");
                //}
                //catch (System.Exception)
                //{

                //    throw;
                //}
            }
            else
            {
                UserDAO userDAO = new UserDAO(IntroState.username, IntroState.password, UserLevenController.exp);
                PlayerPrefs.SetString(IntroState.username, userDAO.dataGather);
            }
            GetUserDAO(IntroState.username);
            //开启数据展示
            Invoke("OpenDataPanel", 2f);
        }

        public GameObject crartsPanel;
        public void OpenDataPanel()
        {
            crartsPanel.SetActive(true);
        }


        public void SpawnRangeEnabler(bool enabled)
        {
            foreach (Transform t in staticSpawnRangeGO.transform)
            {
                t.gameObject.GetComponent<SpawnRange>().enabled = enabled;
            }
        }

        private bool gameEndBool;
        public void RecordGCenter(string s)
        {
            if (gameEndBool)
            {
                return;
            }
            if (gravityCenterWriter != null)
            {
                print("测试问题是否处在gravityCenterWriter.WriteLineAsync(s);");
                gravityCenterWriter.WriteLineAsync(s);
            }
        }

        bool waitBool = true;
        public IEnumerator WaitAndTakeKinectPhoto(float interval)
        {

            if (waitBool)
            {
                waitBool = false;
                if (GameManager.instance.currentUser != null)
                {

                    Texture2D photo = KinectManager.Instance.GetUsersClrTex();

                    byte[] bytes = photo.EncodeToJPG();

                    string filePath = GameManager.instance.currentUser.UserFolderPath + startTime + "/" + "UserPhoto - " + photoCounter.ToString() + ".jpg";
                    print("测试问题是否处在File.WriteAllBytes");
                    File.WriteAllBytes(filePath, bytes);
                    photoCounter++;
                    yield return new WaitForSeconds(interval);
                    waitBool = true;
                }
            }

            //while (true) {
            //    if (GameManager.instance.currentUser != null) {

            //        Texture2D photo = KinectManager.Instance.GetUsersClrTex();

            //        byte[] bytes = photo.EncodeToJPG();

            //        string filePath = GameManager.instance.currentUser.UserFolderPath + startTime + "/" + "UserPhoto - " + photoCounter.ToString() + ".jpg";
            //        File.WriteAllBytes(filePath, bytes);

            //        photoCounter++;

            //        yield return new WaitForSeconds(interval);
            //    }
            //}
        }
    }
}

