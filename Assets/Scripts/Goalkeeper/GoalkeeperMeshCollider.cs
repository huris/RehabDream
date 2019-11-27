/* ============================================================================== 
* ClassName：GoalkeeperMeshCollider 
* Author：ChenShuwei 
* CreateDate：2019/10/13 ‏‎‏‎16:30:01 
* Version: 1.0
* ==============================================================================*/

using UnityEngine;
using System.Collections;
//generate mesh collider of goalkeeper
public class GoalkeeperMeshCollider : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;    // skinned mesh render of goalkeeper
    public MeshCollider meshcollider;           // mesh collider of goalkeeper
    void Awake()
    {
        meshcollider = gameObject.GetComponent<MeshCollider>();
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh); //更新mesh
        meshcollider.sharedMesh = colliderMesh; //将新的mesh赋给meshcollider

    }
}