using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Audio Management.
/// </summary>
public class AudioManager : MonoBehaviour {

    //Has the player obejct moved left or right?
    public static bool moved = false;

    //Has the player object collided with a threat?
    public static bool failed = false;

    //The SpriteRenderer of the mute button.
    Image muteRenderer;

    //The sprites for the muted and unmuted button states.
    public Sprite mutedSprite;
    public Sprite unmutedSprite;

    //The various AudioSources for sound effects.
    private AudioSource moveSound;
    private AudioSource impact;

    void Awake()
    {
        GameObject mute = GameObject.Find("Mute");

        //Cache the SpriteRenderer.
        muteRenderer = mute.GetComponent<Image>();

        //Get the audio sources responsible for playing sound effects.
        moveSound = transform.Find("Movement").GetComponent<AudioSource>();
        impact = transform.Find("Impact").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (moved)
        {
            moved = false;
            PlayMoveSound();
        }

        if (failed)
        {
            failed = false;
            PlayFailSound();
        }
    }

    /// <summary>
    /// A check to see if the user has previously muted the audio.
    /// </summary>
    public void AudioStatus()
    {
        if (AudioListener.volume > 0)
        {
            AudioListener.volume = 0;
            muteRenderer.sprite = mutedSprite;
        }
        else if (AudioListener.volume == 0)
        {
            AudioListener.volume = 1;
            muteRenderer.sprite = unmutedSprite;
        }
    }

    void PlayMoveSound()
    {
        moveSound.Play();
    }

    void PlayFailSound()
    {
        impact.Play();
    }

}
