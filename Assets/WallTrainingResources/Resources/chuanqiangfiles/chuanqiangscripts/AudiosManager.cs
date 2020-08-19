using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiosManager : MonoBehaviour
{
    public AudioClip[] Sounds;


    //------------------------------//
    /// <summary>
    /// 单例模式
    /// </summary>
    public static AudiosManager instance = new AudiosManager();
    //------------------------------//


    /// <summary>
    /// 将声音放入字典中，方便管理
    /// </summary>
    private Dictionary<string, AudioClip> _soundDictionary;

    public AudioSource bgAudioSource;
    public AudioSource audioSourceEffect;
    public AudioSource actionAudio;
    void Awake()
    {
        instance = this;

        _soundDictionary = new Dictionary<string, AudioClip>();

        //存放到字典
        foreach (AudioClip item in Sounds)
        {
            Debug.Log("@AudioManager: AudioClips " + item.name);
            _soundDictionary.Add(item.name, item);
        }
    }

    //播放背景音乐
    public void PlayBGaudio(string audioName)
    {
        if (_soundDictionary.ContainsKey(audioName))
        {
            bgAudioSource.clip = _soundDictionary[audioName];
            bgAudioSource.Play();
        }
    }
    public void StopBGaudio()
    {
        if (bgAudioSource!=null&& bgAudioSource.clip!=null)
        {
            bgAudioSource.Stop();
        }
        
    }
    //播放音效
    public void PlayAudioEffect(string audioEffectName)
    {
        Debug.Log("@AudioManager: audioEffectName: " + audioEffectName);

        if (_soundDictionary.ContainsKey(audioEffectName))
        {
            audioSourceEffect.clip = _soundDictionary[audioEffectName];
            audioSourceEffect.Play();
           
            
        }
    }
    public void StopAudioEffect()
    {
        if (audioSourceEffect.clip != null)
        {
            audioSourceEffect.Stop();
        }
    }
    public void PlayActionAudio(string audioEffectName)
    {
        Debug.Log("@AudioManager: audioEffectName " + audioEffectName);
        if (_soundDictionary.ContainsKey(audioEffectName))
        {
            actionAudio.clip = _soundDictionary[audioEffectName];
            actionAudio.Play(50000);
            Debug.Log("@AudioManager: audioEffectName111 " + audioEffectName);

        }
    }
    public void StopActionAudio()
    {
        if (actionAudio.clip != null)
        {
            actionAudio.Stop();
        }
    }
    private void Start()
    {
        //DontDestroyOnLoad(this);
    }
}
