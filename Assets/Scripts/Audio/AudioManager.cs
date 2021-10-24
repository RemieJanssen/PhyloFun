using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    static bool initialized = false;
    static AudioSource audioSource;
    static Dictionary<AudioClipName, AudioClip> audioClips = new Dictionary<AudioClipName, AudioClip>();

    public static bool Initialized
    {
        get { return initialized; }
    }


    public static int Initialize(AudioSource source)
    {
        initialized = true;
        audioSource = source;
        audioClips.Add(AudioClipName.LevelWin, Resources.Load<AudioClip>("bell-ringing-05"));//soundjay.com
        audioClips.Add(AudioClipName.MenuClick, Resources.Load<AudioClip>("button-16"));//soundjay.com
        return audioClips.Count;
    }

    public static void Play(AudioClipName name)
    {
        audioSource.PlayOneShot(audioClips[name]);
    }

    public static float GetLength(AudioClipName name)
    {
        return audioClips[name].length;
    }
}
