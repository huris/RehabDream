using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wit.BaiduAip.Speech;
using static Wit.BaiduAip.Speech.Tts;

public class SangCtrl : MonoBehaviour {
     string APIKey = "wLzhUob7DtOLbNUItizzAGCp";
     string SecretKey = "qCiQ5zYa4Xoi61UH7o2s530g25bzm7Z2";
    private Tts _asr;
    public AudioSource _audioSource;
  //  private bool _startPlaying;
 
    void Awake () {
        _asr = new Tts(APIKey, SecretKey);
        StartCoroutine(_asr.GetAccessToken());
        _audioSource = this.transform.GetComponent<AudioSource>();
        _audioSource.volume = 0.4f;
    }
  /// <summary>
  /// 语音合成
  /// </summary>
  /// <param name="message">合成信息的内容</param>
    public  void SpeechSynthesis(string message)
    {
       
        StartCoroutine(_asr.Synthesis(message, s =>
        {
            if (s.Success)
            {
               // DescriptionText.text = "合成成功，正在播放";
                _audioSource.clip = s.clip;
                _audioSource.Play();
            }
            else
            {
                // DescriptionText.text = s.err_msg;
                // SynthesisButton.gameObject.SetActive(true);
                Debug.LogError("合成失败");
            }
        },5,5,(int)Pronouncer.Duyaya));
    }


    /// <summary>
    /// 开始听用户声音
    /// </summary>
    // public Button StartButton;
    /// <summary>
    /// 停止听取
    /// </summary>
    //  public Button StopButton;
    // private AudioClip _clipRecord;
    // [Tooltip("语音合成")]  public Button SynthesisButton;

    // public Text DescriptionText;//显示当前状态

    //public void OnClickStartButton()
    //{
    //    StartButton.gameObject.SetActive(false);
    //    StopButton.gameObject.SetActive(true);
    //    DescriptionText.text = "请讲话...";

    //    _clipRecord = Microphone.Start(null, false, 30, 16000);
    //}

    //public void OnClickStopButton()
    //{
    //    StartButton.gameObject.SetActive(false);
    //    StopButton.gameObject.SetActive(false);
    //    DescriptionText.text = "识别中...";
    //    Microphone.End(null);
    //    Debug.Log("end record");
    //    var data = Asr.ConvertAudioClipToPCM16(_clipRecord);
    //    StartCoroutine(_asr.Recognize(data, s =>
    //    {
    //        Debug.Log("未识别到语音");

    //        StartButton.gameObject.SetActive(true);
    //    }));
    //}
}
