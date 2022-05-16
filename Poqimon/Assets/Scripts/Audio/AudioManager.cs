using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    public static AudioManager i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        // no audio clip
        if (clip == null)
        {
            return;
        }
        // reproduce audio clip
        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();
    }
}
