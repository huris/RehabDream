using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class DoorHandle : MonoBehaviour
{

    public delegate void OpenDoorOver(); // what happen when player is opening the door
    public event OpenDoorOver OnOpenDoorOver;

    [Header("Tag")]
    public const string DoorTrigger = "Door";
   // public const string HandTrigger = "Hand";

    [Header("DoorAngle")]
    public const float StartAngleZ = 90f;
    public const float EndAngleZ = 200f;

    [Header("GameObject")]
    public GameObject BedroomDoor;
    public Transform DoorCenter;

    [Header("RotationParameters")]
    public Vector3 DoorPivotPosition = new Vector3(-9.519f, 3.822f, 21.973f);   // position of door pivot
    public const float SmoothTime = 0.05f;
    public const float MinDistance = 0.2f; // min distance between hand and door

    private float _CurrentVelocity = 0f;


    // Use this for initialization
    void Start()
    {
        BedroomDoor = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // door rotation
    public void Rotation(Vector3 LeftHand, Vector3 RightHand)
    {
        float AngleOffsetLeft = GetRotationAngle(LeftHand);
        float AngleOffsetRight = GetRotationAngle(RightHand);

        float AngleOffset = AngleOffsetRight > AngleOffsetLeft? AngleOffsetRight: AngleOffsetLeft;

        //Debug.Log("AngleOffset " + AngleOffset);

       
        SmoothRotation(new Vector3(0,0, AngleOffset));
    }


    // calculate the rotation Angle
    private float GetRotationAngle(Vector3 Hand)
    {

        // Direction of Ray
        Vector3 RayDirection = DoorCenter.position - Hand;
        Ray ray = new Ray(Hand, RayDirection);

        // 碰撞检测信息存储
        RaycastHit[] hits = Physics.RaycastAll(ray, 5f);
        if (hits!=null)
        {//碰撞检测

            foreach(RaycastHit hit in hits)
            {
                //Debug.DrawLine(Hand, hit.point, Color.red, 1);// 画线显示
                //Debug.Log(hit.collider.name);// 打印检测到的碰撞体名称
                if (hit.collider.tag.Equals(DoorTrigger))
                {
                    Vector3 End = hit.point;    // 交点
                    Vector3 Normal = hit.normal;    // 交点处的法向量
                    float NowDistance = DisPoint2Plane(Hand, new Plane(Normal, End));   // 当前手到门的距离
                    //Debug.Log("NowDistance" + NowDistance);

                    if(NowDistance < MinDistance)
                    {
                        Vector3 Pivot = new Vector3(DoorPivotPosition.x, Hand.y, DoorPivotPosition.z);  // 与手同等高度的门轴
                        float Hand2Pivot = (Pivot - Hand).magnitude;   //手到门轴的距离

                        float Delta1 = Mathf.Acos(NowDistance / Hand2Pivot);    //手-门轴 与 法向量的夹角
                        float Delta2 = Mathf.Acos(MinDistance / Hand2Pivot);

                        float AngleOffset = Mathf.Abs(Delta1 - Delta2) * Mathf.Rad2Deg; //弧度转角度

                        if (double.IsNaN(AngleOffset))
                        {
                            return 0f;
                        }
                        else
                        {
                            return AngleOffset;
                        }
                    }

                }
            }
        }
        else
        {
            //Debug.Log("No Hit");
            Debug.DrawLine(Hand, DoorCenter.position);//没检测到碰撞体，则以最大检测距离画线
        }

        return 0f;
    }


    // rotate door smoothly
    private void SmoothRotation(Vector3 AngleOffset)
    {

        //Debug.Log(OutputInpectorEulers());

        if ((OutputInpectorEulers().y > 0 && OutputInpectorEulers().y > EndAngleZ) ||
            OutputInpectorEulers().y < 0 && OutputInpectorEulers().y + 360f > EndAngleZ
            )    // 转动到达极限
        {
            OnOpenDoorOver?.Invoke();
            Debug.Log("@DoorHandle: The door is open");
            return;
        }

        if (Mathf.Approximately(Mathf.Abs(AngleOffset.z), 0f))
        {
            return;
        }


        float SmoothAngle = Mathf.LerpAngle(0, AngleOffset.z, 0.05f);
        BedroomDoor.transform.Rotate(0, 0, SmoothAngle);
    }


    // distance between point and plane
    public float DisPoint2Plane(Vector3 Point, Plane plane)
    {
        return Mathf.Abs(plane.GetDistanceToPoint(Point));
    }

    public void Reset()    //reset
    {

    }


    // 获取Inspector上的Rotation
    private Vector3 OutputInpectorEulers()
    {
        Vector3 angle = transform.eulerAngles;
        float x = angle.x;
        float y = angle.y;
        float z = angle.z;

        if (Vector3.Dot(transform.up, Vector3.up) >= 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = angle.x - 360f;
            }
        }
        if (Vector3.Dot(transform.up, Vector3.up) < 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = 180 - angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = 180 - angle.x;
            }
        }

        if (angle.y > 180)
        {
            y = angle.y - 360f;
        }

        if (angle.z > 180)
        {
            z = angle.z - 360f;
        }


        return new Vector3(x,y,z);
    }
}
