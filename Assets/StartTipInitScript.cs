using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTipInitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string StartTip = DoctorDataManager.instance.doctor.patient.PatientName + "你好！" + "欢迎来到重心范围评估！"
						+ "请直立站在Kinect的正前方，两眼注视屏幕。" + "当评估开始时会出现足球，需要您按语音和箭头指示将双手握拳移到到足球上。"
						+ "当正确做出指示行为时，足球会呈现高亮状态，同时向外移动，您需要保持双手跟随足球移动。"
						+ "注意，当足球出现在中间位置时，需要双手做出向前伸或向后拉的操作。"
						+ "准备好开始了吗？请将双手握拳放于肚脐前准备吧！";
		transform.GetComponent<SangCtrl>().SpeechSynthesis(StartTip);
	}
	
	// Update is called once per frame
	void Update () {

		

	}
}
