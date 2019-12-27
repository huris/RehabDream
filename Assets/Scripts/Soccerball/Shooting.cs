/* ============================================================================== 
* ClassName：Shooting 
* Author：ChenShuwei 
* CreateDate：2019/10/30 20:16:27 
* Version: 1.0
* ==============================================================================*/

using UnityEngine;

public class Shooting : MonoBehaviour {

   


    //[Header("Position")]
    //public Transform SoccerStart;   // 足球初始位置
    //public Transform SoccerTarget;  // 目标

    [Header("Setting")]
    public int PointNumber = 1000;       // 采样点数
    public float gravity = -9.8f;   // 重力加速度


    [Header("Range")]
    public float range_x = 5.0f;
    public float range_y = 1.0f;
    public float range_z = 4.0f;

    // 速度15情况下，1s内可以到达球门
    public static float StandardVelocity = 15.0f;
    public static float StandardTime = 1.0f;

    private Rigidbody _Rigid;
    private bool _IsShooting = false;
    private bool _IsShootingBeforePause = false;
    private Vector3 _VelocityBeforePause;
    private Vector3[] _path;      //  贝塞尔曲线运动轨迹
    private float _Velocity = 0f;
    private float _TimeSum = 0f;
    private Vector3 SimulaterVelocity;
    private int _OldIndex = 0;
    void Start()
    {
        _Rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_IsShooting)
        {
            // update the rotation of the projectile during trajectory motion
            _TimeSum += Time.deltaTime;

            int index = (int)((_Velocity / StandardVelocity) * (_TimeSum / StandardTime) * PointNumber);
            //Debug.Log(PointNumber);
            if(index >= PointNumber)
            {
                ShootOver();
                _Rigid.velocity = SimulaterVelocity;
                return;
            }

            this.gameObject.transform.position = _path[index];
            transform.rotation = Quaternion.LookRotation(_path[index]- _path[_OldIndex]);
            SimulaterVelocity= (_path[index] - _path[_OldIndex]) / Time.deltaTime;
            _OldIndex = index;
        }
    }


    // Shoot
    public void Shoot(Vector3 Start, Vector3 ControlPoint, Vector3 Target, float Velocity)
    {
        _IsShooting = true;
        _Velocity = Velocity;
        _path = BezierUtils.GetBeizerList(Start, ControlPoint, Target, this.PointNumber);
        Debug.Log("@Shooting: Soccer Shoot");
    }


    // save soccerball's physical data
    private void SaveBeforePause()
    {
        _IsShootingBeforePause = _IsShooting;
        _VelocityBeforePause = _Rigid.velocity;
    }

    // freeze soccerball(can't move)
    private void PauseSoccerball()
    {
        _IsShooting = false;
        _Rigid.constraints = RigidbodyConstraints.FreezeAll;
    }

    // recover soccerball
    private void RecoverAfterPause()
    {
        _Rigid.constraints = RigidbodyConstraints.None;
        _Rigid.velocity = _VelocityBeforePause;
        _IsShooting = _IsShootingBeforePause;
    }


    // Pause shooting
    public void PauseShooting()
    {
        SaveBeforePause();
        PauseSoccerball();
    }

    // continue shooting
    public void ContinueShooting()
    {
        RecoverAfterPause();
    }

    // call Reset after every shooting
    public void Reset(Vector3 Start)
    {
        _Rigid.constraints = RigidbodyConstraints.FreezeAll; //stop soccer
        _Rigid.constraints = RigidbodyConstraints.None;
        transform.position = Start;                 //reset soccer position
    }

    // after ShootOver(), soccer will move naturally
    public void ShootOver()
    {
        _IsShooting = false;
        _TimeSum = 0f;
        _OldIndex = 0;
        _path = new Vector3[]{ };
    }

    // Soccer directly move (only for test)
    public void SoccerMoveDirect(Vector3 Start, Vector3 Target)
    {
        //random target based on SoccerTarget
        Vector3 actual_target;
        actual_target.x = Target.x + Random.Range(-range_x, range_x);
        actual_target.y = Target.y + Random.Range(-range_y, range_y);
        actual_target.z = Target.z + Random.Range(-range_z, range_z);
        //direction of force
        Vector3 direction;
        direction = actual_target - transform.position;
        direction.Normalize();
        //Debug.Log(direction);
        GetComponent<Rigidbody>().AddForceAtPosition(direction * 250, transform.position);
        GetComponent<Rigidbody>().AddForce(Vector3.up * 70);

    }
}
