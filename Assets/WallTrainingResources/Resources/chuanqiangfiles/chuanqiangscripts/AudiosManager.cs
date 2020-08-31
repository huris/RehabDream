using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiosManager : MonoBehaviour
{
    //------------------------------//
    /// <summary>
    /// 单例模式
    /// </summary>
    public static AudiosManager instance = new AudiosManager();
    //------------------------------//


    /// <summary>
    /// 将声音放入字典中，方便管理
    /// </summary>
    private Dictionary<string, AudioClip> _soundDictionary = new Dictionary<string, AudioClip>();
    private AudioSource[] audioSources;

    private AudioSource bgAudioSource;
    private AudioSource audioSourceEffect;
    private AudioSource actionAudio;
    void Awake()
    {
        instance = this;

        _soundDictionary = new Dictionary<string, AudioClip>();

        //本地加载 
        AudioClip[] audioArray = Resources.LoadAll<AudioClip>("Audios");

        audioSources = GetComponents<AudioSource>();
        bgAudioSource = audioSources[0];
        audioSourceEffect = audioSources[1];

        //Debug
        actionAudio = audioSources[1];
        //存放到字典
        foreach (AudioClip item in audioArray)
        {
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
        if (_soundDictionary.ContainsKey(audioEffectName))
        {
            actionAudio.clip = _soundDictionary[audioEffectName];
            actionAudio.Play(50000);


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
