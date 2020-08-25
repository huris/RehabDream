using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ShipNSea 
{
	public class CameraShackScript : MonoBehaviour
	{

		private static CameraShackScript instance;
		private static Vector3 originalPos = new Vector3(0, 104.1f, -95.5f);
		//private WaitForSeconds wait = new WaitForSeconds(.2f);
		private WaitForFixedUpdate wait = new WaitForFixedUpdate();
		public static CameraShackScript GetInstance()
		{
			if (instance == null)
			{
				instance = new CameraShackScript();
			}
			return instance;
		}

		/// <summary>
		/// 提供的一个摄像机震动方法
		/// </summary>
		/// <param name="shakeAmount"></param>
		/// <param name="decreaseFactor"></param>
		public IEnumerator CameraShackFunc(float shakeAmount, float decreaseFactor, float shakeTime, Image image = null)
		{
			for (float i = shakeTime; i > 0; i -= Time.deltaTime * decreaseFactor)
			{
				Camera.main.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
				if (i > 0.05f)
				{
					if (image != null)
					{
						if (!image.gameObject.activeSelf)
						{
							image.gameObject.SetActive(true);
							image.color = new Color(1, 0, 0, 0.8f);
						}
						image.color = Color.Lerp(image.color, new Color(0, 0, 0, 0), Time.deltaTime * decreaseFactor);
					}
					yield return wait;
				}
				else
				{
					Camera.main.transform.localPosition = originalPos;
					if (image != null)
					{
						if (image.gameObject.activeSelf)
						{
							image.gameObject.SetActive(false);
							i = 0;
							yield break;
						}
					}
				}
			}
		}


	}
}

