using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShipNSea 
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager instance;

        public LevelControllerBase currentLevel;

        public User currentUser = null;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

        }

        public void LoadScene(string name)
        {

            currentLevel = null;
            SceneManager.LoadScene(name);
        }

        private void OnApplicationQuit()
        {
            currentUser?.Logout();
        }
    }
}

