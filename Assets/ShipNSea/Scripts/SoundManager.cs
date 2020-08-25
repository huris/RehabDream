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

namespace ShipNSea 
{
    public class SoundManager : MonoBehaviour
    {

        // Singleton instance holder
        public static SoundManager instance = null;

        // List holding Sounds
        public List<GameObject> bgmList = new List<GameObject>();
        public List<GameObject> bgsList = new List<GameObject>();

        // volume percentage
        public float bgmVolume = 0.8f;
        public float bgsVolume = 0.8f;
        public float seVolume = 1f;

        // Sound type group, used for type list
        public enum SoundType
        {
            BGM,
            BGS,
            SE
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

        public GameObject Play(AudioClip clip, SoundType type, float fadeInTime = 0f)
        {

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
                    volume = bgsVolume;
                    break;
                default:
                    loop = false;
                    volume = seVolume;
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
        {
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
        }

        public void SetVolume(SoundType type, float volume)
        {
            switch (type)
            {
                case SoundType.BGM:
                    foreach (GameObject bgm in bgmList)
                    {
                        AudioSource a = bgm.GetComponent<AudioSource>();
                        a.volume = volume;
                    }
                    break;
                default:
                    break;
            }
        }

    }

}
