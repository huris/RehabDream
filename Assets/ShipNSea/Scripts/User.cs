using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ShipNSea 
{
    public class User
    {

        // UUID.
        public string username;

        // Personal info.
        public string email;

        public string UserFolderPath { get; private set; }

        public User(string username)
        {
            this.username = username;
            Login();
        }

        private void Login()
        {

            Debug.Log("User login: " + username);

            CheckUserFolder();
            LoadUserDB();
        }

        public void Logout()
        {
            Debug.Log("User logout: " + username);
            WriteUserLogoutAction();
            // Database.CloseConnection();
        }

        private void CheckUserFolder()
        {
            UserFolderPath = "users/" + username + "/";
            if (!Directory.Exists(UserFolderPath))
            {
                Directory.CreateDirectory(UserFolderPath);
            }
        }

        public void LoadUserDB()
        {
            // Database = new SQLiteHelper("Data Source=" + UserFolderPath + username + ".db");

            WriteUserLoginAction();
        }

        private void WriteUserLoginAction()
        {

            //if (!Database.IsTableExists("login")) {
            //    Database.CreateTable("login", new string[] { "Time" }, new string[] { "TEXT" });
            //}

            //Database.InsertValues("login", new string[] { GenerateLinuxTimestamp() });
        }

        private void WriteUserLogoutAction()
        {

            //if (!Database.IsTableExists("logout")) {
            //    Database.CreateTable("logout", new string[] { "Time" }, new string[] { "TEXT" });
            //}

            //Database.InsertValues("logout", new string[] { GenerateLinuxTimestamp() });
        }

        public string GenerateLinuxTimestamp()
        {
            return System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }
    }
}

