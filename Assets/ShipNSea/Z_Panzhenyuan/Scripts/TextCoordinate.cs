using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace ShipNSea 
{
	public enum FishType
	{
		StaticFish = 0,
		DynamicFish = 1
	}

	public class TextCoordinate : MonoBehaviour
	{
		public GameObject boatGO;
		public TextMeshProUGUI gotPointTextMeshPro;
		public Camera UICamera;
		public RectTransform canvas;
		public Transform score;
		public bool gotPoint;
		private Vector3 SuspendedHigh;
		List<UnityAction> animaFuncList = new List<UnityAction>();
		public static FishType type = FishType.StaticFish;
		//RectTransform rect;
		void Update()
		{
			SetPos();
		}
		void OnEnable()
		{
			boatGO = GameObject.Find("FishingBoat");
			UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
			canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
			score = GameObject.Find("Canvas/InGame/Score").transform;
			//解读下这个add:取到list第0个元素 他是unityaction类型的 然后 => list[0] = SuspendedAnima
			animaFuncList.Add(SuspendedAnima);
			animaFuncList.Add(FlyToScore);
			if (type == FishType.StaticFish)
			{
				gotPointTextMeshPro.text = "100";
			}
			else if (type == FishType.DynamicFish)
			{
				gotPointTextMeshPro.text = "150";
			}
		}
		void SetPos()
		{
			if (!gotPoint)
			{
				Vector3 worldVec = new Vector3();
				Vector3 point = boatGO.transform.position;
				bool tf = RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, Camera.main.WorldToScreenPoint(point), UICamera, out worldVec);
				gotPointTextMeshPro.transform.position = worldVec;
				SuspendedHigh = gotPointTextMeshPro.transform.position + new Vector3(0, .8f, 0);
				gotPoint = true;
			}
			else
			{
				animaFuncList[0]();
			}
		}

		void SuspendedAnima()
		{
			//Debug.LogError("执行SuspendedAnima");
			gotPointTextMeshPro.transform.position = Vector3.Lerp(gotPointTextMeshPro.transform.position, SuspendedHigh, 0.03f);
			if (Vector3.Distance(gotPointTextMeshPro.transform.position, SuspendedHigh) <= 0.1)
			{
				animaFuncList.Remove(SuspendedAnima);
			}

		}
		void FlyToScore()
		{
			//Debug.LogError("执行FlyToScore");
			gotPointTextMeshPro.transform.position = Vector3.Lerp(gotPointTextMeshPro.transform.position, score.position, 0.1f);
			if (Vector3.Distance(gotPointTextMeshPro.transform.position, score.position) <= 1)
			{
				animaFuncList.Remove(FlyToScore);
				Destroy(gameObject);
			}
		}
	}
}

