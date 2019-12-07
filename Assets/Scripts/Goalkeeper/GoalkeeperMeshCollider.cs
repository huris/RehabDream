/* ============================================================================== 
* ClassName：GoalkeeperMeshCollider 
* Author：ChenShuwei 
* CreateDate：2019/10/13 ‏‎‏‎16:30:01 
* Version: 1.0
* ==============================================================================*/

using UnityEngine;
using System.Collections;
using System.Threading;
//generate mesh collider of goalkeeper
public class GoalkeeperMeshCollider : MonoBehaviour
{
    public static int ColliderNum = 5;
    public SkinnedMeshRenderer[] MeshRenderers;    // skinned mesh renders of goalkeeper
    public MeshCollider[] MeshColliders;           // mesh colliders of goalkeeper
    private Thread _WriteDatabaseThread;
    private float _UpdateTime = 0.1f;
    private float _UpdateTimeCount = 0f;


    // Use this for initialization
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateTimeOver())
        {
            UpdateColliders();
        }
    }

    // udpate mesh collider
    private void UpdateColliders()
    {
        for (int i = 0; i < ColliderNum; i++)
        {
            Mesh colliderMesh = new Mesh();
            MeshRenderers[i].BakeMesh(colliderMesh); //更新mesh
            MeshColliders[i].sharedMesh = colliderMesh; //将新的mesh赋给meshcollider
        }
    }

    // update mesh collider after _UpdateTime
    private bool UpdateTimeOver()
    {
        _UpdateTimeCount += Time.deltaTime;
        if (_UpdateTimeCount >= _UpdateTime)
        {
            _UpdateTimeCount = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }
}