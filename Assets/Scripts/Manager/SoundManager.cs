//
//  SoundManager.cs
//  Retrace - The Videogame
//
//  Created by Gavin KG on 2017/2/26.
//  Copyright © 2017 Gavin_KG. All rights reserved.
//

/*
 * --- Sound Manager ---
 * Manage BGM & SE playback and status.
 * NOTE: Sounds with 3D effects should be hooked on the related GameObject, not here.
 */

using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{

    // Singleton instance holder
    public static SoundManager instance = null;

    // List holding Sounds
    public List<GameObject> bgmList = new List<GameObject>();
    public List<GameObject> bgsList = new List<GameObject>();
    public GameObject Se ;

    // volume percentage
    private float _bgmVolume
    {
        get { return PatientDataManager.instance.bgmVolume; }
    }
    private float _bgsVolume
    {
        get { return PatientDataManager.instance.bgsVolume; }
    }
    private float _seVolume
    {
        get { return PatientDataManager.instance.seVolume; }
    }

    // Sound type group, used for type list
    public enum SoundType
    {
        BGM,    //背景音乐
        BGS,    //背景声效
        SE  //声效
    }

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            Debug.Log("@SoundManager: Singleton created.");

        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public GameObject Play(AudioClip clip, SoundType type,float bgmVolume, float fadeInTime = 0f)
    {     //播放音乐

        bool loop;
        float volume;

        switch (type)
        {
            case SoundType.BGM:
                loop = true;
                volume = bgmVolume;
                break;
            case SoundType.BGS:
                loop = true;
                volume = bgmVolume;
                break;
            default:
                loop = false;       //声效不循环
                volume = bgmVolume;
                break;
        }

        GameObject sourceGO = Play(clip, volume, 1f, loop);

        switch (type)
        {
            case SoundType.BGM:
                bgmList.Add(sourceGO);
                break;
            case SoundType.BGS:
                bgsList.Add(sourceGO);
                break;
            default:
                Se = sourceGO;
                break;
        }

        return sourceGO;
    }

    public GameObject Play(AudioClip clip, float volume, float pitch, bool loop)
    {

        GameObject go = new GameObject("Audio_" + clip.name);

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.Play();

        DontDestroyOnLoad(source);

        return go;
    }

    public void PauseSounds(SoundType type)
    {
        switch (type)
        {
            case SoundType.BGM:
                foreach (GameObject go in bgmList)
                {
                    AudioSource source = go.GetComponent<AudioSource>();
                    source.Pause();
                }
                break;
            case SoundType.BGS:
                foreach (GameObject go in bgsList)
                {
                    AudioSource source = go.GetComponent<AudioSource>();
                    source.Pause();
                }
                break;
            default:
                break;
        }
    }

    public void StopSounds(SoundType type)
    {    //关闭音乐
        switch (type)
        {
            case SoundType.BGM:
                foreach (GameObject go in bgmList)
                {
                    Destroy(go);
                }
                break;
            case SoundType.BGS:
                foreach (GameObject go in bgsList)
                {
                    Destroy(go);
                }
                break;
            case SoundType.SE:
                Destroy(this.Se);
                break;
            default:
                break;
        }
    }

    public void StopSounds()
    {
        foreach (GameObject go in bgmList)
        {
            Destroy(go);
        }
        foreach (GameObject go in bgsList)
        {
            Destroy(go);
        }
        Destroy(this.Se);
    }

    public void SetVolume(SoundType type, float volume)
    {   //音量设置（用于改变当前正在播放的BGM BGS）
        switch (type)
        {
            case SoundType.BGM:
                foreach (GameObject bgm in bgmList)
                {
                    AudioSource a = bgm.GetComponent<AudioSource>();
                    a.volume = volume;
                }
                break;
            case SoundType.BGS:
                foreach (GameObject bgs in bgsList)
                {
                    AudioSource a = bgs.GetComponent<AudioSource>();
                    a.volume = volume;
                }
                break;
            default:
                break;
        }
    }

}
