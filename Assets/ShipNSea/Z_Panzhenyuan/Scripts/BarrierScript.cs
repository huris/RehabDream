using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipNSea 
{
	public class BarrierScript : MonoBehaviour
	{

		void OnEnable()
		{
			if (MapDetectionController.mapOccupyDis.ContainsKey(gameObject.tag))
			{
				MapDetectionController.mapOccupyDis[gameObject.tag].Add(transform);
			}
			else
			{
				var list = new List<Transform>();
				list.Add(transform);
				MapDetectionController.mapOccupyDis.Add(gameObject.tag, list);
			}
		}
		void OnDestroy()
		{
			if (MapDetectionController.mapOccupyDis.ContainsKey(gameObject.tag))
			{
				MapDetectionController.mapOccupyDis[gameObject.tag].Remove(transform);
			}
		}
		void Update()
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -3, transform.position.z), 0.01f);
		}

	}
}

