using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipNSea 
{
    public class CoinLevel : MonoBehaviour
    {


        public GameController gameController;

        public Text levelText;
        public Image levelImage;

        public List<int> levelMaxValue;

        public int CurrentLevel { get { return _currentLevel; } }

        private int _currentLevel = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (gameController.enabled)
            {
                int coin = gameController.TotalScore;
                if (_currentLevel < levelMaxValue.Count - 1 && coin > levelMaxValue[_currentLevel])
                {
                    _currentLevel++;
                }

                levelText.text = (_currentLevel + 1).ToString();

                int lastLevelValue = _currentLevel == 0 ? 0 : levelMaxValue[_currentLevel - 1];
                levelImage.fillAmount = (float)(coin - lastLevelValue) / (float)(levelMaxValue[_currentLevel] - lastLevelValue);
            }
        }
    }
}

