using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NoUseDoctorTestScript : MonoBehaviour {

	public Text Tips;
	public Sequence seq;

	// Use this for initialization
	void Start () {
		ShowTips("保持Bobath握拳");
	}

	public void ShowTips(string tip)
	{
		Tweener t1 = Tips.GetComponent<Text>().DOText(tip, 3f);
		Tweener t2 = Tips.GetComponent<Text>().DOText("", 0.5f);
		seq = DOTween.Sequence();
		seq.Append(t1);
		seq.Append(t2);
		seq.SetLoops(-1);
	}


	// Update is called once per frame
	void Update () {
		
	}
}
