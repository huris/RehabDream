using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using Vectrosity;
//using Windows.Kinect;


public class SkeletonProjection : MonoBehaviour
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Whether to flip left and right, relative to the sensor.")]
	public bool flipLeftRight = false;

	[Tooltip("Game object used to overlay the joints.")]
	public GameObject jointPrefab;
	public GameObject GreenPrefab;
	public GameObject RedPefab;

	[Tooltip("Line object used to overlay the bones.")]
	public LineRenderer linePrefab;


	private GameObject[] joints = null;
	private LineRenderer[] lines = null;

	private Quaternion initialRotation = Quaternion.identity;

	//LineRenderer
	//private List<VectorLine> FistLines;
	//VectorLine.Destroy(ref myLine);   // ɾ��������
	//private LineRenderer FistLine;   // ������
	private VectorLine ColorFistLine; // ��ɫ������

	//private LineRenderer ConvexHullLine;   // ͹����

	//���������˵�
	private int index = 0;

	//��¼��������ָ�������
	private Vector3 HandTipLeft;
	private Vector3 LastPosition;
	private Vector3 NowPosition;

	// ��¼���꼯,ȡǰ��������,����z��,��͹��
	// ��͹��ʹ��Melkman�㷨
	public class Point
	{
		public float x { get; set; } = 0.0f;
		public float y { get; set; } = 0.0f;
		public Point(float _x = 0.0f, float _y = 0.0f)
		{
			x = _x;
			y = _y;
		}
		public static float PointsDistance(Point A, Point B)
		{
			float Distance = 1 - (Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y));
			if (Distance < 0.0f) return 0.0f;
			else return Distance;
		}
	}
	public class CoordinateComparer : IComparer<Point>
	{
		//ʵ����������
		public int Compare(Point x, Point y)
		{
			if(x.y.CompareTo(y.y)==0) return (x.x.CompareTo(y.x));
			else return (x.y.CompareTo(y.y));
		}

	}
	private List<Point> Points;  // ���ݵ�ļ��� 
	private Point[] pointArray;  //��������
	private int PointNum = 0;    // ���ݵ�ĸ���
	private Point[] ConvexHull;  // ͹����
	private int ConvexHullNum;  // ͹����ĸ���
	private Point[] TwoTable; // ����������˫���

	public Canvas canvas;

	void Start()
	{
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			int jointsCount = manager.GetJointCount();

			//if (jointPrefab)
			//{
				// array holding the skeleton joints
			joints = new GameObject[jointsCount];

			GreenPrefab = Resources.Load("Prefabs/RadarPrefabs/GreenBall") as GameObject;
			RedPefab = Resources.Load("Prefabs/RadarPrefabs/RedBall") as GameObject;

			for (int i = 0; i < joints.Length; i++)
			{
				if (i == 21 || i == 23)
				{
					jointPrefab = RedPefab;
				}
				else
				{
					jointPrefab = GreenPrefab;
				}

				joints[i] = Instantiate(jointPrefab) as GameObject;
				joints[i].transform.parent = transform;
				joints[i].name = ((KinectInterop.JointType)i).ToString();
				//print(joints[i].name + "    " + i.ToString());
				joints[i].SetActive(false);
			}
			//}

			// array holding the skeleton lines
			lines = new LineRenderer[jointsCount];
		}
	}

	void OnEnable()
	{
		//VectorLine.SetLine(Color.green, new Vector2(0, 0), new Vector2(222,322));
		//PointHashSet = new HashSet<Point>();
		Points = new List<Point>();
		index = 0;

		ColorFistLine = new VectorLine("FistLine", new List<Vector2>(), 7.0f, LineType.Continuous, Joins.Weld);
		ColorFistLine.smoothColor = true;	// ����ƽ����ɫ
		ColorFistLine.smoothWidth = true;   // ����ƽ�����
		ColorFistLine.endPointsUpdate = 2;   // Optimization for updating only the last couple points of the line, and the rest is not re-computed

		// always mirrored
		initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

		////���LineRenderer���
		//FistLine = gameObject.AddComponent<LineRenderer>();
		////���ò���
		//FistLine.material = new Material(Resources.Load("Prefabs/RadarPrefabs/VirtualLineYellow") as Material);

		//////������ɫ
		////FistLine.startColor = Color.yellow;
		////FistLine.endColor = Color.red;
		////���ÿ��
		//FistLine.startWidth = 0.01f;
		//FistLine.endWidth = 0.02f;
		////���ó�ʼ�������Ϊ0
		//FistLine.positionCount = 0;

		//FistColors = new Color32;
		
		////���LineRenderer���
		//ConvexHullLine = gameObject.AddComponent<LineRenderer>();
		////���ò���
		////ConvexHullLine.material = new Material(Resources.Load("Prefabs/RadarPrefabs/VirtualLineYellow") as Material);
		////������ɫ
		//ConvexHullLine.startColor = Color.white;
		//ConvexHullLine.endColor = Color.white;
		////���ÿ��
		//ConvexHullLine.startWidth = 0.02f;
		//ConvexHullLine.endWidth = 0.02f;
		////���ó�ʼ�������Ϊ0
		//ConvexHullLine.positionCount = 0;
	}
	
	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		//FistLine = GetComponent<LineRenderer>();
		//ConvexHullLine = GetComponent<LineRenderer>();

		if (manager && manager.IsInitialized())
		{
			// overlay all joints in the skeleton
			if(manager.IsUserDetected(playerIndex))
			{
				long userId = manager.GetUserIdByIndex(playerIndex);
				int jointsCount = manager.GetJointCount();

				for(int i = 0; i < jointsCount; i++)
				{
					int joint = i;

					if(manager.IsJointTracked(userId, joint))
					{ 

						Vector3 posJoint = manager.GetJointPosition(userId, joint);

						//posJoint = Camera.main.WorldToScreenPoint(posJoint);

						if (flipLeftRight)
							posJoint.x = -posJoint.x;

						if (joints != null)
						{
							// overlay the joint
							if(posJoint != Vector3.zero)
							{
								posJoint.z = 0;   // ��z���Ϊ0,ƽ����ʾ
								joints[i].SetActive(true);
								joints[i].transform.position = posJoint;

								if (i == 21) { HandTipLeft = posJoint; } 

								// �������־���С��0.1f��ʱ����
								if (i == 23 && (HandTipLeft-posJoint).magnitude < 0.1f)
								{
									//print((HandTipLeft - posJoint).magnitude);
									//PointSet[PointNum++] = new Point(posJoint.x, posJoint.y);
									//print(posJoint);
									//PointHashSet.Add(new Point(posJoint.x, posJoint.y));


									//tempLine.Add(new Vector2(0, 0));
									//tempLine.Add(new Vector2(Screen.width - 1, Screen.height - 1));
									//TempVectorLine = new VectorLine("L1", tempLine, 7.0f, LineType.Continuous, Joins.Fill);
									//TempVectorLine.color = new Color(255 / 255, 255 / 255 * 1.0f, 0);
									//TempVectorLine.Draw();

									Points.Add(new Point(posJoint.x, posJoint.y));

									//float tempDis = Point.PointsDistance(Points[Points.Count - 1], Points[Points.Count - 2]);
									//if (tempDis < 1.0f)
									//{
									//	Color tempColor = new Color(255 / 255, 255 / 255 * tempDis, 0);
									//	FistLine.startColor = tempColor;
									//	FistLine.endColor = tempColor;
									//}



									//ColorFistLine.points2.Add(new Vector2(-posJoint.x * 960 + 780, posJoint.y * 540));
									ColorFistLine.points2.Add(SetMousePosition(posJoint));

									ColorFistLine.Draw();

									//List<Vector2> tempLine = new List<Vector2>();
									//tempLine.Add(new Vector2(-Points[Points.Count - 2].x * 960 + 780, Points[Points.Count - 2].y * 540));
									//tempLine.Add(new Vector2(-Points[Points.Count - 1].x * 960 + 780, Points[Points.Count - 1].y * 540));

									//FistLines.Add(new VectorLine("L", tempLine, 7.0f, LineType.Continuous, Joins.Fill));
									//FistLines[FistLines.Count - 1].color = Color.red;
									//FistLines[FistLines.Count - 1].Draw();

									//Points.Add(new Point(posJoint.x, posJoint.y));

									//FistLine.positionCount++;

									//while (Points.Count > 1 && index < FistLine.positionCount)
									//{
									//	//����ȷ��һ��ֱ�ߣ������������λ��Ƶ�Ϳ����γ��߶���
									//	//FistLine.SetPosition(index, posJoint);
									//	//������ɫ
									//	float tempDis = Point.PointsDistance(Points[Points.Count - 1], Points[Points.Count - 2]);
									//	if(tempDis < 1.0f)
									//	{
									//		Color tempColor = new Color(255 / 255, 255 / 255 * tempDis, 0);
									//		FistLine.startColor = tempColor;
									//		FistLine.endColor = tempColor;
									//	}
									//	FistLine.SetPosition(index, new Vector3(posJoint.x, posJoint.y, 0));
									//	index++;
									//}
								}

								Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
								rotJoint = initialRotation * rotJoint;
								joints[i].transform.rotation = rotJoint;
							}
							else
							{
								joints[i].SetActive(false);
							}
						}

						if(lines[i] == null && linePrefab != null)
						{
							lines[i] = Instantiate(linePrefab) as LineRenderer;
							lines[i].transform.parent = transform;
							lines[i].gameObject.SetActive(false);
						}

						if(lines[i] != null)
						{
							// overlay the line to the parent joint
							int jointParent = (int)manager.GetParentJoint((KinectInterop.JointType)joint);

							Vector3 posParent = manager.GetJointPosition(userId, jointParent);
							if(flipLeftRight)
								posParent.x = -posParent.x;

							if(posJoint != Vector3.zero && posParent != Vector3.zero)
							{
								lines[i].gameObject.SetActive(true);

								//lines[i].SetVertexCount(2);
								//lines[i].SetPosition(0, posParent);
								//lines[i].SetPosition(1, posJoint);
								// ��������z���Ϊ0,ƽ����ʾ
								lines[i].positionCount = 2;
								lines[i].SetPosition(0, new Vector3(posParent.x, posParent.y, 0));
								lines[i].SetPosition(1, new Vector3(posJoint.x, posJoint.y, 0));
							}
							else
							{
								lines[i].gameObject.SetActive(false);
							}
						}
						
					}
					else
					{
						if(joints != null)
						{
							joints[i].SetActive(false);
						}
						
						if(lines[i] != null)
						{
							lines[i].gameObject.SetActive(false);
						}
					}
				}

			}
		}
	}

	public Vector2 SetMousePosition(Vector3 pos)
	{
		//print(pos);
		Vector2 uisize = canvas.GetComponent<RectTransform>().sizeDelta;//�õ������ĳߴ�
		Vector2 screenpos = Camera.main.WorldToScreenPoint(pos);//����������ת��Ϊ��Ļ����
		Vector2 screenpos2;
		screenpos2.x = screenpos.x;//ת��Ϊ����Ļ����Ϊԭ�����Ļ����
		screenpos2.y = screenpos.y;
		Vector2 uipos;
		uipos.x = (screenpos2.x / Screen.width) * uisize.x;
		uipos.y = (screenpos2.y / Screen.height) * uisize.y;//�õ�UGUI��anchoredPosition

		return uipos;
		//Mouse.transform.DOMove(uipos, 0.02f);
		//print(uipos);
	}

	public void ButtonOnClick() // ��͹��
	{
		//VectorLine.Destroy(FistLines);   // ɾ��������

		//FistLine.positionCount = 0;
		//print("@@@@@"+ConvexHullSet.Count);
		//print(ConvexHullSet[0]);
		//print(Points.Count);  // ������ȭʶ���������Ŀ


		if (Points != null && Points.Count > 0)
		{
			Points.Sort(new CoordinateComparer());   // �Ե�����һ�²�����͹���㷨

			pointArray = new Point[Points.Count];
			Points.Add(new Point(0.0f, 0.0f));    // ����һ���㷽�������ѭ������

			//print(Points.Count);
			for (int i = 0; i < Points.Count - 1; i++)
			{

				//print(Points[i].x+" "+Points[i].y+" "+i);
				// ȥ��һЩ�ظ��ĵ�
				if (Points[i].x == Points[i + 1].x && Points[i].y == Points[i + 1].y)
				{
					Points.RemoveAt(i + 1);
					i--;
				}
				else
				{
					// ��¼���ظ��ĵ�
					pointArray[PointNum++] = Points[i];
				}
			}
			//pointArray[PointNum++] = Points[Points.Count - 1];
			//print(PointNum);  // ���ظ��ĵ���
			//print(ConvexHullMelkman(pointArray, PointNum)); // ͹����ĸ���

			ConvexHullNum = ConvexHullMelkman(pointArray, PointNum);

			//FistLine.positionCount = FistLine.positionCount + ConvexHullNum;

			//for (int i = 0; i < ConvexHullNum; i++)
			//{
			//	//ConvexHullLine.SetPosition(i, new Vector3(ConvexHull[i].x, ConvexHull[i].y, 0));
			//	//print(ConvexHull[i].x+" "+ ConvexHull[i].y + " " + i);
			//	FistLine.SetPosition(i + FistLine.positionCount - ConvexHullNum, new Vector3(ConvexHull[i].x, ConvexHull[i].y, 0));
			//}
			//FistLine.positionCount++;
			//FistLine.SetPosition(FistLine.positionCount - 1, new Vector3(ConvexHull[0].x, ConvexHull[0].y, 0));

			//pointArray = new Point[10];
			//pointArray[0] = new Point(1,0);
			//pointArray[1] = new Point(2,2);
			//pointArray[2] = new Point(2,1);
			//pointArray[3] = new Point(3,1);
			//pointArray[4] = new Point(3,2);
			//pointArray[5] = new Point(3,3);
			//pointArray[6] = new Point(4,1);
			//pointArray[7] = new Point(5,2);

			//PointNum = 8;

			//ConvexHullNum = ConvexHullMelkman(pointArray, PointNum);
			//print(ConvexHull);
			//for (int i = 0; i < ConvexHullNum; i++)
			//{
			//	print(ConvexHull[i].x + " " + ConvexHull[i].y);
			//}
		}

	}

	// isLeft(): test if a point is Left|On|Right of an infinite line.
	//    Input:  three points P0, P1, and P2
	//    Return: >0 for P2 left of the line through P0 and P1
	//            =0 for P2 on the line
	//            <0 for P2 right of the line
	//    See: Algorithm 1 on Area of Triangles
	public float isLeft(Point P0, Point P1, Point P2)
	{
		//print(P0.x + " " + P0.y +" "+P1.x+ " " + P1.y + " " + P2.x+" "+P2.y);
		return (P1.x - P0.x) * (P2.y - P0.y) - (P2.x - P0.x) * (P1.y - P0.y);
	}

	// ConvexHullMelkman(): Melkman's 2D simple polyline O(n) convex hull algorithm
	//    Input:  P[] = array of 2D vertex points for a simple polyline
	//            n   = the number of points in V[]
	//    Output: H[] = output convex hull array of vertices (max is n)
	//    Return: h   = the number of points in H[]
	public int ConvexHullMelkman(Point[] P, int n)
	{
		// initialize a deque D[] from bottom to top so that the
		// 1st three vertices of P[] are a ccw triangle
		TwoTable = new Point[2 * n + 1];
		ConvexHull = new Point[n];

		int bot = n - 2, top = bot + 3;    // initial bottom and top deque indices
		TwoTable[bot] = TwoTable[top] = P[2];        // 3rd vertex is at both bot and top
		if (isLeft(P[0], P[1], P[2]) > 0)
		{
			TwoTable[bot + 1] = P[0];
			TwoTable[bot + 2] = P[1];           // ccw vertices are: 2,0,1,2
		}
		else
		{
			TwoTable[bot + 1] = P[1];
			TwoTable[bot + 2] = P[0];           // ccw vertices are: 2,1,0,2
		}

		// compute the hull on the deque D[]
		for (int i = 3; i < n; i++)
		{
			//print(i + "!!!!!");
			// process the rest of vertices
			// test if next vertex is inside the deque hull
			if ((isLeft(TwoTable[bot], TwoTable[bot + 1], P[i]) > 0) &&
				(isLeft(TwoTable[top - 1], TwoTable[top], P[i]) > 0))
				continue;         // skip an interior vertex

			// incrementally add an exterior vertex to the deque hull
			// get the rightmost tangent at the deque bot
			while (isLeft(TwoTable[bot], TwoTable[bot + 1], P[i]) <= 0)
				++bot;                 // remove bot of deque
			TwoTable[--bot] = P[i];           // insert P[i] at bot of deque

			// get the leftmost tangent at the deque top
			while (isLeft(TwoTable[top - 1], TwoTable[top], P[i]) <= 0)
				--top;                 // pop top of deque
			TwoTable[++top] = P[i];           // push P[i] onto top of deque
		}
		//print("!!!!\n");
		// transcribe deque D[] to the output hull array H[]
		int h;        // hull vertex counter
		for (h = 0; h <= (top - bot); h++)
		{
			ConvexHull[h] = TwoTable[bot + h];
		}

		return h - 1;
	}

}
