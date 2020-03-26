using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingStartInitScript : MonoBehaviour {

	// Use this for initialization
	public bool IsOver;
	void Start()
	{
		string StartTip = DoctorDataManager.instance.doctor.patient.PatientName + "您好！" + "欢迎来到平衡能力训练！"
						+ "屏幕文字和图片展示了训练过程，详情可以通过点击下方语音介绍按钮来获取。"
						+ "准备开始您的训练吧！";
		//+ "请直立站在Kinect的正前方，两眼注视屏幕。" + "当评估开始时会出现足球，需要您按语音和箭头指示将双手握拳移到到足球上。"
		//+ "当正确做出指示行为时，足球会呈现高亮状态，同时向外移动，您需要保持双手跟随足球移动。"
		//+ "注意，当足球出现在中间位置时，需要双手做出向前伸或向后拉的操作。"
		//+ "准备好开始了吗？请将双手握拳放于肚脐前准备吧！";
		transform.GetComponent<SangCtrl>().SpeechSynthesis(StartTip);

		IsOver = false;
		//transform.GetComponent<SangCtrl>().SpeechSynthesis("你好胡奔");
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsOver && (this.transform.localPosition - new Vector3(0f, 978f, 0)).magnitude < 10f)
		{
			this.transform.GetComponent<AudioSource>().Stop();
			IsOver = true;
		}
	}

	public void VoiceIntroductionPlayOnClick()
	{
		if (this.transform.GetComponent<AudioSource>().isPlaying)
		{
			this.transform.GetComponent<AudioSource>().Stop();
		}

		string StartTip = "请直立站在Kinect的正前方，两眼注视屏幕。" + "当训练开始时会有足球飞来，需要您按语音和箭头指示将双手握拳接住足球。"
						+ "当正确做出指示行为时，会记录得分，同时足球偏离向外移动。"
						+ "足球的速度和方向随着难度不同会有所变化"
						+ "准备好开始了吗？请将双手握拳放于肚脐前准备吧！";
		transform.GetComponent<SangCtrl>().SpeechSynthesis(StartTip);
	}

	public void DirectionReturnDoctorUI()
	{
		SceneManager.LoadScene("03-DoctorUI");
	}
}
