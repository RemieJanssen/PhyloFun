using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioSource : MonoBehaviour
{
    void Awake()
    {
        if(!AudioManager.Initialized)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = .5f;
            print("Number of audio clips: "+AudioManager.Initialize(audioSource));
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
