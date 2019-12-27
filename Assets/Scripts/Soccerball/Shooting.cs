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
    public float height = 1.8f;      // 高度
    public float gravity = -9.8f;   // 重力加速度


    [Header("Range")]
    public float range_x = 5.0f;
    public float range_y = 1.0f;
    public float range_z = 4.0f;

    private Rigidbody _Rigid;
    private bool _IsShooting = false;
    private bool _IsShootingBeforePause = false;
    private Vector3 _VelocityBeforePause;
    private ParabolaPath _path;      // 抛物线运动轨迹
    void Start()
    {
        _Rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_IsShooting)
        {
            // update the rotation of the projectile during trajectory motion
            transform.rotation = Quaternion.LookRotation(_Rigid.velocity);

        }
    }


    // Shoot
    public void Shoot(float VelocityX, Vector3 Start, Vector3 Target, float HeightLimit, float gravity)
    {
        _IsShooting = true;
        _Rigid.velocity = CalculateInitialVelocity(VelocityX, Start, Target, HeightLimit, gravity);
        Debug.Log("@Shooting: Soccer Shoot");
    }


    // calculate Initial Velocity based on (Start, Target, MaxHeight)
    // The sphere is only affected by gravity，no drag
    public Vector3 CalculateInitialVelocity(float VelocityX, Vector3 Start, Vector3 Target, float HeightLimit, float gravity)
    {

        _path = new ParabolaPath(VelocityX, Start, Target, HeightLimit, gravity);
        _path.isClampStartEnd = true;
        return _path.GetVelocity(0f);
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
