//
//  IntroState.cs
//
//  Created by Gavin_KG on 2017/11/4.
//  Copyright © 2017 Gavin_KG. All rights reserved.
//
//  DISCLAIMER! THIS IS A FAKE UI CONTROLLER SCRIPT! IT DOES NOT HANDLE ACTUAL LOGIC!
//  

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ShipNSea 
{
    public class UserDAO
    {   
        public string username;
        public string password;
        public int experience;


        public string dataGather;
        public string trainTime;
        public string catchFishCount;
        public string distance;
        public string gotExp;

        public string gotStaticFishCount;
        public string gotDynamicFishCount;
        public string startDateTime;//开始时间
        public string endDateTime;//结束时间
        public List<float> eachFishGotCastTime;
        public List<float> gList;


        public int fishCount;
        public int staticFishCount;
        public int dynamicFishCount;

        public UserDAO(string username, string password, int experience)
        {
            this.username = username;
            this.password = password;
            this.experience = experience;
            dataGather = username + "|" + password + "|" + experience;
        }
        public UserDAO() { }
    }

    public class IntroState : LevelControllerBase
    {
        public static Text hintText;
        public Text kinectStatusText;
        public static string username;
        public static int id;
        public static string password;
        public static string experience;
        [Header("Popups")]
        public GameObject loginRegisterGO;
        public GameObject avatarGO;
        public GameObject loginPopupGO;
        public GameObject registerPopupGO;
        public GameObject settingsPopupGO;

        [Header("Login")]
        public InputField loginUsernameInputField;
        public InputField loginPasswordInputField;
        public Button unLog;

        [Header("Register")]
        public InputField registerUsernameInputField;
        public InputField registerEmailInputField;
        public InputField registerPasswordInputField;

        [Header("Avatar")]
        public Text avatarUsernameText;

        private static bool isLogin = false;
        public static bool isConnectToMySql = false;

        [HideInInspector]
        public static string pPwd = "myid";
        [HideInInspector]
        public static string pName = "myName";

        // private SQLiteHelper sql;
        // private SQLiteHelper tempSql;
        public override void Start()
        {

            base.Start();
            hintText = GameObject.Find("HintText").GetComponent<Text>();
            if (username == null || username == "")
            {
                loginRegisterGO.SetActive(true);
            }
            else
            {
                avatarGO.SetActive(true);
                loginRegisterGO.SetActive(false);
                avatarUsernameText.text = username;
                unLog.transform.gameObject.SetActive(true);
            }
            unLog.onClick.AddListener(() => {
                isLogin = false;
                loginRegisterGO.SetActive(true);
                avatarGO.SetActive(false);
                unLog.transform.gameObject.SetActive(false);
            });
            QuickStartButton_Click();
        }
        private static void ShowHintText(string str)
        {
            if (hintText != null)
            {
                hintText.text = str;
                hintText.gameObject.SetActive(false);
                hintText.gameObject.SetActive(true);
            }
        }
        //public static void SqlConnection()
        //{
        //    if (!isConnectToMySql)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        string connStr = "database=test001;server=127.0.0.1;port=3306;user=root;password=Qwetycpzypzf1234.;CharSet=utf8;";
        //        conn = new MySqlConnection(connStr);
        //        conn.Open();
        //        isConnectToMySql = true;

        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.Log("无法连接到服务器,启用本地储存");
        //        ShowHintText("无法连接到服务器,启用本地储存");
        //        isConnectToMySql = false;
        //    }

        //}

        public override void Update()
        {
            base.Update();

            // Kinect status
            if (KinectManager.Instance.IsInitialized())
            {
                if (KinectManager.Instance.IsUserDetected())
                {
                    kinectStatusText.text = "已检测到用户";
                }
                else
                {
                    kinectStatusText.text = "未检测到用户";
                }
            }
            else
            {
                kinectStatusText.text = "Kinect 未就绪";
            }

            if (Input.GetKey(KeyCode.Space))
            {
                QuickStartButton_Click();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                print("esc");
                if (loginPopupGO.activeSelf)
                {
                    loginPopupGO.SetActive(false);
                    return;
                }
#if UNITY_EDITOR

                UnityEditor.EditorApplication.isPlaying = false;

                Debug.Log("编辑状态游戏退出");

#else

            Application.Quit();

#endif
            }
        }

        #region Main

        //public void OpenRegisterPopup()
        //{
        //    SqlConnection();
        //    registerPopupGO.SetActive(true);
        //}

        //public void OpenLoginPopup()
        //{
        //    SqlConnection();
        //    loginPopupGO.SetActive(true);
        //}

        public void OpenSettings()
        {
            settingsPopupGO.SetActive(true);
        }

        public void QuickStartButton_Click()
        {
            if (isLogin)
            {
                GameManager.instance.LoadScene("JointDetectionScene");
            }
            //提供快捷进入接口
            else
            {
                string temp = PlayerPrefs.GetString(pPwd);

                if (temp != null && temp != "")
                {
                    string[] strArr = temp.Split('|');
                    print("登录成功");
                    ShowHintText("登录成功");
                    loginRegisterGO.SetActive(false);
                    avatarGO.SetActive(true);
                    avatarUsernameText.text = strArr[0];
                    IntroState.username = strArr[0];
                    IntroState.password = strArr[1];
                    IntroState.experience = strArr[2];
                    CloseLoginPopup();
                    unLog.transform.gameObject.SetActive(true);

                    GameManager.instance.LoadScene("JointDetectionScene");
                }
                else
                {
                    UserDAO userDAO = new UserDAO(pPwd, pName, 0);
                    PlayerPrefs.SetString(pPwd, userDAO.dataGather);
                    print("注册成功");
                    ShowHintText("注册新用户成功");

                    string temp1 = PlayerPrefs.GetString(pPwd);
                    string[] strArr = temp1.Split('|');
                    print("登录成功");
                    ShowHintText("登录成功");
                    loginRegisterGO.SetActive(false);
                    avatarGO.SetActive(true);
                    avatarUsernameText.text = strArr[0];
                    IntroState.username = strArr[0];
                    IntroState.password = strArr[1];
                    IntroState.experience = strArr[2];
                    CloseLoginPopup();
                    unLog.transform.gameObject.SetActive(true);

                    GameManager.instance.LoadScene("JointDetectionScene");
                }

            }

        }

        #endregion

        #region Register

        //public void CloseRegisterPopup()
        //{
        //    conn.Close();
        //    registerPopupGO.SetActive(false);
        //}

        //public void ConfirmRegister()
        //{

        //    MySqlDataReader reader = null;

        //    string username = registerUsernameInputField.text;
        //    string password = registerPasswordInputField.text;
        //    if (username == "" || password == "")
        //    {
        //        print("注册的用户名或者密码不能为空");
        //        ShowHintText("注册的用户名或者密码不能为空");
        //        return;
        //    }
        //    if (isConnectToMySql)
        //    {
        //        try
        //        {
        //            using (MySqlCommand cmd = new MySqlCommand("Select * from user where username = @username", conn))
        //            {
        //                cmd.Parameters.AddWithValue("username", username);
        //                reader = cmd.ExecuteReader();
        //                if (reader.HasRows)
        //                {
        //                    Debug.Log("该用户已存在");
        //                    ShowHintText("该用户已存在");
        //                    reader.Close();
        //                    return;
        //                }
        //            }
        //            reader.Close();
        //            conn.Close();
        //            SqlConnection();
        //            using (MySqlCommand cmd_Ins = new MySqlCommand("insert into user set username = @username , password = @password", conn))
        //            {
        //                cmd_Ins.Parameters.AddWithValue("username", username);
        //                cmd_Ins.Parameters.AddWithValue("password", password);
        //                cmd_Ins.ExecuteNonQuery();
        //            }
        //            Debug.Log("插入新用户成功");
        //            ShowHintText("注册新用户成功");
        //        }
        //        catch (System.Exception)
        //        {
        //            throw;
        //        }
        //    }
        //    else
        //    {
        //        UserDAO userDAO = new UserDAO(username, password, 0);
        //        if (PlayerPrefs.GetString(username) != "")
        //        {
        //            print(PlayerPrefs.GetString(username));
        //            return;
        //        }
        //        PlayerPrefs.SetString(username, userDAO.dataGather);
        //        print("注册成功");
        //        ShowHintText("注册新用户成功");
        //    }
        //    RegisterToLogin(username);
        //}

        //public void RegisterToLogin(string username)
        //{
        //    CloseRegisterPopup();
        //    OpenLoginPopup();
        //    loginUsernameInputField.text = username;
        //}

        #endregion

        #region Login

        public void CloseLoginPopup()
        {
            //conn.Close();
            loginPopupGO.SetActive(false);
        }

        //public void ConfirmLogin()
        //{

        //    string username = null;
        //    username = loginUsernameInputField.text;
        //    string password = loginPasswordInputField.text;
        //    if (isConnectToMySql)
        //    {
        //        MySqlDataReader reader = null;
        //        try
        //        {
        //            MySqlCommand cmd = new MySqlCommand("Select * from user where username = @username and password = @password", conn);
        //            cmd.Parameters.AddWithValue("username", username);
        //            cmd.Parameters.AddWithValue("password", password);
        //            reader = cmd.ExecuteReader();
        //            if (reader.Read())
        //            {
        //                loginRegisterGO.SetActive(false);
        //                avatarGO.SetActive(true);
        //                avatarUsernameText.text = username;
        //                IntroState.username = username;
        //                IntroState.password = password;
        //                IntroState.experience = reader["experience"].ToString();
        //                IntroState.id = int.Parse(reader["id"].ToString());
        //                isLogin = true;
        //                //查到就关闭连接
        //                CloseLoginPopup();
        //                unLog.transform.gameObject.SetActive(true);
        //                return;
        //            }
        //            else
        //            {
        //                Debug.Log("用户名或者密码错误");
        //                ShowHintText("用户名或者密码错误");
        //                return;
        //            }
        //        }
        //        catch (System.Exception e)
        //        {
        //            Debug.Log("在VerifyUser的时候出现异常" + e);
        //        }
        //        finally
        //        {
        //            if (reader != null)
        //            {
        //                reader.Close();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("启用本地储存登录");
        //        string temp = PlayerPrefs.GetString(username);
        //        if (temp != null && temp != "")
        //        {
        //            string[] strArr = temp.Split('|');
        //            if (strArr[0] == username && strArr[1] == password)
        //            {
        //                print("登录成功");
        //                ShowHintText("登录成功");
        //                loginRegisterGO.SetActive(false);
        //                avatarGO.SetActive(true);
        //                avatarUsernameText.text = username;
        //                IntroState.username = username;
        //                IntroState.password = password;
        //                IntroState.experience = strArr[2];
        //                isLogin = true;
        //                CloseLoginPopup();
        //                unLog.transform.gameObject.SetActive(true);
        //            }
        //            else
        //            {
        //                print("用户名或者密码错误");
        //                ShowHintText("用户名或者密码错误");
        //            }
        //        }
        //        else
        //        {
        //            print("用户名或者密码错误");
        //            ShowHintText("用户名或者密码错误");
        //        }
        //    }

        //}

        public void LoginToRegister()
        {
            CloseLoginPopup();
            //OpenRegisterPopup();
        }

        #endregion

        #region Settings

        public void CloseSettings()
        {
            settingsPopupGO.SetActive(false);
        }

        #endregion

    }
}

