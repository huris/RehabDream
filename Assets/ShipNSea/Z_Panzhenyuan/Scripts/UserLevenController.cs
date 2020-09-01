using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ShipNSea 
{
	public enum LevelE
	{
		zero = 100,
		one = 300,
		two = 600,
		three = 1500,
		four = 3000,
		five = 6000,
		six = 10000
	}
	public class UserLevenController : MonoBehaviour
	{
		public static UserLevenController instance;
		void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}
		public Image lvCircle;
		public Text username;
		public Text lvl;
		public static LevelE levelE;
		public static int exp;
		public static string levelStr;
		private LevelE levelETemp;
		// Use this for initialization
		void Start()
		{
			if ((exp = int.Parse(IntroState.experience)) >= 10000)
			{
				exp = 10000;
			}
			ChangeLv();
			//print("当前玩家: " + IntroState.pName);
			username.text = "当前玩家: " + IntroState.pName;
		}

		public void GetLevel()
		{
			exp += 100;
			ChangeLv();
		}
		public void DeductExp(int num)
		{
			if ((exp -= num) <= 0)
			{
				exp = 0;
			}
			ChangeLv();
		}
		private void ChangeLv()
		{
			if (exp < (int)LevelE.one)
			{
				lvl.text = "Lv 0";
				levelStr = "等级 0";
				levelETemp = LevelE.zero;
				levelE = LevelE.one;
				MapDetectionController.barrierNum = 3;
				FishFlock.captureTime = .5f;
			}
			else if (exp < (int)LevelE.two)
			{
				lvl.text = "Lv 1";
				levelStr = "等级 1";
				levelETemp = LevelE.one;
				levelE = LevelE.two;
				MapDetectionController.barrierNum = 4;
				FishFlock.captureTime = 1f;

			}
			else if (exp < (int)LevelE.three)
			{
				lvl.text = "Lv 2";
				levelStr = "等级 2";
				levelETemp = LevelE.two;
				levelE = LevelE.three;
				MapDetectionController.barrierNum = 5;
				FishFlock.captureTime = 1.5f;

			}
			else if (exp < (int)LevelE.four)
			{
				lvl.text = "Lv 3";
				levelStr = "等级 3";
				levelETemp = LevelE.three;
				levelE = LevelE.four;
				MapDetectionController.barrierNum = 6;
				FishFlock.captureTime = 2f;

			}
			else if (exp < (int)LevelE.five)
			{
				lvl.text = "Lv 4";
				levelStr = "等级 4";
				levelETemp = LevelE.four;
				levelE = LevelE.five;
				MapDetectionController.barrierNum = 7;
				FishFlock.captureTime = 2.5f;

			}
			else if (exp < (int)LevelE.six)
			{
				lvl.text = "Lv 5";
				levelStr = "等级 5";
				levelETemp = LevelE.five;
				levelE = LevelE.six;
				MapDetectionController.barrierNum = 8;
				FishFlock.captureTime = 3f;

			}
			else
			{
				lvl.text = "Lv 6";
				levelStr = "等级 6";
				levelETemp = LevelE.six;
				levelE = LevelE.six;
				MapDetectionController.barrierNum = 9;
				FishFlock.captureTime = 3.5f;
			}
			if (levelETemp == LevelE.six)
			{
				lvCircle.fillAmount = 1;
				return;
			}
			lvCircle.fillAmount = ((exp - (float)levelETemp) / ((float)levelE - (float)levelETemp));
		}
	}

}
