using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource FXPlayer;

    public static AudioManager i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            return;
        }
        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();
    }
}
