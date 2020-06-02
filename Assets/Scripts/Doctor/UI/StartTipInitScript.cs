using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartTipInitScript : MonoBehaviour {

	// Use this for initialization
	public bool IsOver;

	public AudioSource audiosource;

	public static StartTipInitScript _instance;

	public RawImage rawImage;
	public MovieTexture _Movie;

	void Awake()
	{
		//audiosource = gameObject.AddComponent<AudioSource>();

		audiosource.playOnAwake = false;  //playOnAwake设为false时，通过调用play()方法启用

		_instance = this; //通过Sound._instance.方法调用
		_instance.PlayMusicByName("EvaluationIntroduce");
		rawImage.gameObject.SetActive(false);
	}

	void Start () {
		//string StartTip = DoctorDataManager.instance.doctor.patient.PatientName + "您好！" + "欢迎来到重心范围评估！"
		//				+ "屏幕文字和图片展示了评估过程，详情可以通过点击下方语音介绍按钮来获取。"
		//				+ "准备开始您的评估吧！";
		//				//+ "请直立站在Kinect的正前方，两眼注视屏幕。" + "当评估开始时会出现足球，需要您按语音和箭头指示将双手握拳移到到足球上。"
		//				//+ "当正确做出指示行为时，足球会呈现高亮状态，同时向外移动，您需要保持双手跟随足球移动。"
		//				//+ "注意，当足球出现在中间位置时，需要双手做出向前伸或向后拉的操作。"
		//				//+ "准备好开始了吗？请将双手握拳放于肚脐前准备吧！";
		//transform.GetComponent<SangCtrl>().SpeechSynthesis(StartTip);

		IsOver = false;
		////transform.GetComponent<SangCtrl>().SpeechSynthesis("你好胡奔");
	}

	// Update is called once per frame
	void Update()
	{
		//if (!IsOver && (this.transform.localPosition - new Vector3(0f, 978f, 0)).magnitude < 10f)
		//{
		//	audiosource.Stop();
		//	IsOver = true;
		//}
	}

	public void VideoIntroductionPlayOnClick()
	{
		//if (this.transform.GetComponent<AudioSource>().isPlaying)
		//{
		//	this.transform.GetComponent<AudioSource>().Stop();
		//}

		//string StartTip = "请直立站在Kinect的正前方，两眼注视屏幕。" + "当评估开始时会出现足球，需要您按语音和箭头指示将双手握拳移到到足球上。"
		//				+ "当正确做出指示行为时，足球会呈现高亮状态，同时向外移动，您需要保持双手跟随足球移动。"
		//				+ "注意，当足球出现在中间位置时，需要双手做出向前伸或向后拉的操作。"
		//				+ "准备好开始了吗？请将双手握拳放于肚脐前准备吧！";
		//transform.GetComponent<SangCtrl>().SpeechSynthesis(StartTip);
		if (audiosource.isPlaying)
		{
			audiosource.Stop();
		}

		rawImage.gameObject.SetActive(true);
		_Movie.Play();
	}

	//如果当前有其他音频正在播放，停止当前音频，播放下一个
	public void PlayMusicByName(string name)
	{
		AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);

		if (audiosource.isPlaying)
		{
			audiosource.Stop();
		}

		audiosource.clip = clip;
		audiosource.Play();
	}
}
