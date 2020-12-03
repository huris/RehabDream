using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShipNSea
{
	public class MapDetectionController : MonoBehaviour
	{
		public static MapDetectionController instance;
		//public GameObject cubeShow;
		public GameObject barrierS;
		public GameObject barrierM;
		public GameObject barrierL;
		public GameObject playerBoat;
		public static int barrierNum = 2;
		private WaitForSeconds wait = new WaitForSeconds(1f);
		private float waitTime = 1f;
		//private GameObject radiusTemp;
		//public List<Vector3> mapPosList = new List<Vector3>();
		public static Dictionary<string, List<Transform>> mapOccupyDis = new Dictionary<string, List<Transform>>();
		void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}
		void Start()
		{
			#region 弃用
			#region 画正方形是不对的 因为地图是个圆
			//for (int i = -75; i < 76; i++)
			//{
			//	for (int j = -75; j < 76; j++)
			//	{
			//		Instantiate(cubeShow,new Vector3(i,0,j),Quaternion.identity,this.transform);
			//	}
			//}
			#endregion
			//radiusTemp = new GameObject("RadiusTemp");
			//radiusTemp.transform.parent = this.transform;
			//StringBuilder sb2 = new StringBuilder("RadiusTemp");
			#region 画个圆,还是不行,这么做太卡了
			//for (int i = -74; i < 75; i++)
			//{
			//	Instantiate(cubeShow, new Vector3(i, 0, 0), Quaternion.identity, radiusTemp.transform).name = "Cube";
			//}
			////旋转radiusTemp
			////Instantiate(radiusTemp,new Vector3(0,0,0),Quaternion.AngleAxis(10,Vector3.up),this.transform);
			//for (int i = 2; i < 180; i += 2)
			//{
			//	Instantiate(radiusTemp, new Vector3(0, 0, 0), Quaternion.AngleAxis(i, Vector3.up), this.transform).name = "RadiusTemp" + i;
			//}
			#endregion

			#region 采用列表存储位置信息 首先是整张地图的(圆形的)位置 
			//GameObject[] gos =  GameObject.FindGameObjectsWithTag("MapCube");
			//for (int i = 0; i < gos.Length; i++)
			//{
			//	mapPosList.Add(gos[i].transform.position);
			//}
			//for (int i = 0; i < transform.childCount; i++)
			//{
			//	Destroy(transform.GetChild(i).gameObject);
			//}
			#endregion
			#endregion
			//玩家位置也要加上
			var playerList = new List<Transform>();
			playerList.Add(playerBoat.transform);
			mapOccupyDis.Add(playerBoat.tag, playerList);
			Debug.Log("初始化完成");
			//TODO鱼群位置也要加上
			//InvokeRepeating("TextAbout", 2, .5f);
			//Invoke("Func",30);
			//InvokeRepeating("CreatBarrier",2f,1);
		}
		//void Func()
		//{
		//	CancelInvoke("TextAbout");
		//}
		void CreatBarrier()
		{
			int count = GetBarrierCount();
			while (count < barrierNum)
			{
				if (BarrierInvoke())
				{
					count = GetBarrierCount();
				}
			}
		}
		void Update()
		{
			CreatBarrier();
		}
		int GetBarrierCount()
		{
			int countTemp = 0;
			if (mapOccupyDis.ContainsKey("BarrierS"))
			{
				countTemp += mapOccupyDis["BarrierS"].Count;
			}
			if (mapOccupyDis.ContainsKey("BarrierM"))
			{
				countTemp += mapOccupyDis["BarrierM"].Count;
			}
			if (mapOccupyDis.ContainsKey("BarrierL"))
			{
				countTemp += mapOccupyDis["BarrierL"].Count;
			}
			return countTemp;
		}
		bool BarrierInvoke()
		{
			//Random左闭右开
			int x = Random.Range(-74, 75);
			int z = Random.Range(-74, 75);
			int r = Random.Range(0, 3);
			float rotateAngle = Random.Range(0, 360);
			var pos = new Vector3(x, 0, z);
			var c = Vector3.Distance(pos, Vector3.zero);
			//障碍物半径
			int posVolume = 0;

			//判断是否与玩家位置重叠,给一定的安全区域,不会产生障碍物
			if (mapOccupyDis.ContainsKey("Player"))
			{
				var dis = Vector3.Distance(pos, playerBoat.transform.position);
				if (dis - 8 <= 15)
				{
					//print("安全距离不够,不会产生障碍物:"+(dis-8));
					return false;
				}
			}
			//判断鱼群的位置是不是足够安全距离
			if (mapOccupyDis.ContainsKey("Flock"))
			{
				foreach (var item in mapOccupyDis)
				{
					if (item.Key == "Flock")
					{
						for (int i = 0; i < item.Value.Count; i++)
						{
							var dis = Vector3.Distance(pos, item.Value[i].position);
							if (dis - 5 <= 15)
							{
								//print("与鱼群安全距离不够,不会产生障碍物:" + (dis - 8));
								return false;
							}
						}
					}
				}
			}
			if (75 > c)
			{
				GameObject go = null;
				if (r == 0)
				{
					go = barrierS;
					posVolume = 4;
				}
				else if (r == 1)
				{
					go = barrierM;
					posVolume = 6;
				}
				else if (r == 2)
				{
					go = barrierL;
					posVolume = 8;
				}
				foreach (var item in mapOccupyDis)
				{
					if (item.Key == "BarrierS")
					{
						for (int i = 0; i < item.Value.Count; i++)
						{
							var dis = Vector3.Distance(pos, new Vector3(item.Value[i].position.x, 0, item.Value[i].position.z));
							//print("dis:" + (dis));
							if (dis - posVolume <= 8)
							{
								//print("DIS+POSVOLUME:"+dis+posVolume);
								return false;
							}
						}
					}
					if (item.Key == "BarrierM")
					{
						for (int i = 0; i < item.Value.Count; i++)
						{
							var dis = Vector3.Distance(pos, new Vector3(item.Value[i].position.x, 0, item.Value[i].position.z));
							//print("dis+posVolume:" + (dis + posVolume));
							//print("dis:" + (dis));
							if (dis - posVolume <= 12)
							{
								//print("DIS+POSVOLUME:" + dis + posVolume);
								return false;
							}
						}
					}
					if (item.Key == "BarrierL")
					{
						for (int i = 0; i < item.Value.Count; i++)
						{
							var dis = Vector3.Distance(pos, new Vector3(item.Value[i].position.x, 0, item.Value[i].position.z));
							//print("dis+posVolume:" + (dis + posVolume));
							//print("dis:" + (dis));
							if (dis - posVolume <= 15)
							{
								//print("DIS+POSVOLUME:" + dis + posVolume);
								return false;
							}
						}
					}
				}
				Instantiate(go, new Vector3(pos.x, -20, pos.z), Quaternion.AngleAxis(rotateAngle, Vector3.up), this.transform);
				return true;
			}
			else
			{
				//print("pos:" + pos + "不在圆内"+c);
				return false;
			}
		}
	}
}
