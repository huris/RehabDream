using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
	public class DataCollection : MonoBehaviour
	{
		public Transform centerOfGAngleHelperFoot;
		public Transform centerOfGAngleHelperHead;
		public Animator characterHelper;
		private Vector3 characterGpos;
		public Transform boatTransform;
		public GameObject GameStateGO;
		private GameState gameState;
		public static float dis = 0f;
		public static List<Vector2> boatPosList = new List<Vector2>();
		public static List<float> gAngleList = new List<float>();
		// Use this for initialization
		void Start()
		{
			gameState = GameStateGO.GetComponent<GameState>();

		}

		// Update is called once per frame
		void Update()
		{
			if (!gameState.InGame && dis == 0f)
			{
				CancelInvoke();
				for (int i = 0; i < boatPosList.Count - 1; i++)
				{
					dis += (boatPosList[i + 1] - boatPosList[i]).sqrMagnitude;
				}
				//print("移动总距离:"+dis);
				GameState.outUserDAO.distance = Mathf.Round(dis).ToString();
			}
		}

		public List<string> SQLStringList = new List<string>();



		private float GAngle()
		{
			characterGpos = characterHelper.GetBoneTransform(HumanBodyBones.Neck).position;
			float dot = Vector3.Dot((centerOfGAngleHelperHead.position - centerOfGAngleHelperFoot.position).normalized, (characterGpos - centerOfGAngleHelperFoot.position).normalized);
			float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
			gAngleList.Add(angle);
			return angle;
		}
		private void PrintGAngle()
		{
			GAngle();
		}
		private void AddBoatPos()
		{
			//print(Mathf.RoundToInt(boatTransform.position.x) + "___"+ Mathf.RoundToInt(boatTransform.position.z));
			boatPosList.Add(new Vector2(Mathf.RoundToInt(boatTransform.position.x), Mathf.RoundToInt(boatTransform.position.z)));
		}
		public void StartDataCollenctionFunc()
		{
			InvokeRepeating("PrintGAngle", 0f, 2f);
			InvokeRepeating("AddBoatPos", 0f, 0.2f);
		}
	}
}

