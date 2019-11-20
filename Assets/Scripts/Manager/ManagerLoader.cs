using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLoader : MonoBehaviour {

   
    public List<GameObject> managers = new List<GameObject>();

    void Awake()
    {

        foreach (GameObject go in managers)
        {
            if (!transform.Find(go.name))
            {
                Instantiate(go);
                //Instantiate函数实例化是将original对象的所有子物体和子组件完全复制，
                //成为一个新的对象。这个新的对象拥有与源对象完全一样的东西，包括坐标值等。 
            }
        }
        // transform.find(root_object)
        //1.可以查找隐藏对象 
        //2.支持路径查找
        //3.查找隐藏对象的前提是transform所在的根节点必须可见，即root_object的active = true

        Destroy(gameObject);//销毁Managerloader
    }



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
