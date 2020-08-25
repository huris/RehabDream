using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ShipNSea 
{
	public class TestSceneScript : MonoBehaviour
	{

		[RuntimeInitializeOnLoadMethod]
		private static void Config() 
		{
			ReportPanelController.closeBtnFun = () => { MapDetectionController.mapOccupyDis.Clear(); SceneManager.LoadScene("TestScene"); };
			IntroState.outName = "潘振远";
		}

		// Use this for initialization
		void Start()
		{
			var catchFishcount = GameState.outUserDAO.catchFishCount;
			var outUserDao = GameState.outUserDAO.username;
			var password = GameState.outUserDAO.password;
			var trainTime = GameState.outUserDAO.trainTime;
			var glist = GameState.outUserDAO.gList;
			var exp = GameState.outUserDAO.experience;
			var gotExp = GameState.outUserDAO.gotExp;
			print($"捕鱼数{catchFishcount},用户姓名{outUserDao},用户密码{password},训练时间{trainTime},获得经验{gotExp},该人物总经验{exp}");
			glist.ForEach(e => { print($"重心角度变化(s) = {e}"); });
		}

	}
}

