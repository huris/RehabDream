using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ShipNSea 
{
    public class ComfirmBtn : MonoBehaviour
    {
        private AsyncOperation async = null;
        public GameObject loadScenceSliderGO;
        private Slider loadScenceSlider;
        private long userID;
        public GameObject comfirmBtnGO;
        public Text gameTimeLabelText;
        public GameObject characterGO;
        public GameObject selecttimeGO;
        public GameTime gameTimeScript;
        public Text seletBodyText;
        public BodySetting bodySettingScript;
        public GameObject selectBodyGO;
        [HideInInspector]
        public int gameTime;
        IEnumerator loadSceneIE = null;
        public void Awake()
        {
            loadScenceSlider = loadScenceSliderGO.GetComponent<Slider>();
        }
        public void ComfirmBtnFunc()
        {
            loadSceneIE = LoadSceneAsync();
            loadScenceSliderGO.SetActive(true);
            StartCoroutine(loadSceneIE);
        }

        public void SubmitGameTime()
        {
            int targer;
            if (int.TryParse(gameTimeLabelText.text, out targer))
            {
                gameTime = targer;
            }
            else
            {
                gameTime = 300;
            }
            //gameTimeScript.gameTimeTotal = gameTime;
            selecttimeGO.SetActive(false);
            characterGO.SetActive(true);
        }

        public void SubmitBodySet()
        {
            //switch (seletBodyText.text)
            //{
            //    case "两侧一致":
            //        bodySettingScript.setBody = BodySettingEnum.center;
            //        break;
            //    case "左侧重点":
            //        bodySettingScript.setBody = BodySettingEnum.left;
            //        break;
            //    case "右侧重点":
            //        bodySettingScript.setBody = BodySettingEnum.right;
            //        break;
            //    default:
            //        break;
            //}
            //selectBodyGO.SetActive(false);
        }

        IEnumerator LoadSceneAsync()
        {
            async = SceneManager.LoadSceneAsync("BalanceFishing");
            //async.allowSceneActivation = false;
            yield return async;
            StopCoroutine(loadSceneIE);
        }
        private void Update()
        {
            userID = KinectManager.Instance.GetPrimaryUserID();
            if (async != null)
            {
                // print(async.progress);
                loadScenceSlider.value = async.progress;
            }
            if (userID == 0)
            {
                if (comfirmBtnGO.activeSelf)
                {
                    comfirmBtnGO.SetActive(false);
                }
            }
            else
            {
                if (comfirmBtnGO.activeSelf == false)
                {
                    comfirmBtnGO.SetActive(true);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                print("esc");
#if UNITY_EDITOR

                SceneManager.LoadScene("Intro");

#else

            SceneManager.LoadScene("Intro");

#endif
            }
        }
    }
}

