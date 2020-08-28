using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ShipNSea 
{
	public class TestSceneScript : MonoBehaviour
	{

		/// <summary>
		/// 初始化应该提供的参数
		/// </summary>
		//[RuntimeInitializeOnLoadMethod]
		private static void Config() 
		{
			MapDetectionController.mapOccupyDis.Clear();
			GameState.returnScene = "TestScene";
			//存放id 
			IntroState.pPwd = "123456";
			//存放姓名
			IntroState.pName = "Michael";
			//改变训练时间
			GameTime.gameTimeTotal = 20;
			//改变训练着重侧
			BodySetting.setBody = BodySettingEnum.center;
			//关闭页面按钮事件
			GameState.CloseFunc = ()=> { };
		}

		//清除单条
		//PlayerPrefs.DeleteKey("用户ID");
		//清除所有
		//PlayerPrefs.DeleteAll();
		

		void Start()
		{
			var catchFishcount = GameState.outUserDAO.catchFishCount;
			var username = GameState.outUserDAO.username;
			var password = GameState.outUserDAO.password;
			var trainTime = GameState.outUserDAO.trainTime;
			var glist = GameState.outUserDAO.gList;
			var exp = GameState.outUserDAO.experience;	// 总的经验值
			var gotExp = GameState.outUserDAO.gotExp;	// 本次经验值

			var gotstaticFishCount = GameState.outUserDAO.gotStaticFishCount;
			var gotdyFishC = GameState.outUserDAO.gotDynamicFishCount;
			var eachFishGotCast = GameState.outUserDAO.eachFishGotCastTime;
			var staticFishCount = GameState.outUserDAO.staticFishCount;
			var dyFishCount = GameState.outUserDAO.dynamicFishCount;
			var distance = GameState.outUserDAO.distance;

			print($"捕鱼数{catchFishcount},用户姓名{username},用户id{password},训练时间{trainTime},获得经验{gotExp},该人物总经验{exp}," +
				$"动态鱼群数{dyFishCount},静态鱼群数{staticFishCount},捕获动态鱼群数{gotdyFishC},捕获静态鱼群数{gotstaticFishCount},总路程{distance}");
			glist.ForEach(e => { print($"重心角度变化(s) = {e}"); });
			eachFishGotCast.ForEach(e=> { print($"每条鱼花费时间(s) = {e}"); });
		}

	}
}

