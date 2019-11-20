/* ============================================================================== 
* ClassName：UIAnimator 
* Author：ChenShuwei 
* CreateDate：2019/11/6 15:55:03 
* Version: 1.0
* ==============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UIAnimator使用Unity自带的动画系统，需要提前录制
public class UIAnimator : MonoBehaviour {
    //动画长0.5s
    private float _WaitTime = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // play open Ui animation
    public virtual void OpenUIAnimation()
    {
        var animator = GetComponent<Animator>();
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Close"))
        //{
            animator?.Play("Open");
        //}

    }

    // play close UI animation
    public virtual void CloseUIAnimation()
    {
        var animator = GetComponent<Animator>();
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        //{
            animator?.Play("Close");
        //}
        StartCoroutine(Disactive());

    }

    // disableUI after "_WaitTime" seconds
    private IEnumerator Disactive()
    {
        yield return new WaitForSeconds(this._WaitTime); //先直接返回，之后的代码等待给定的时间周期过完后执行
        this.gameObject.SetActive(false);
    }
}
