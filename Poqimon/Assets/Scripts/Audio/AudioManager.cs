using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    
    [SerializeField] float fadeDuration = 0.75f;

    AudioClip currMusic;
    float originalMusicVol;
    
    public static AudioManager i { get; set; }
    
    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        originalMusicVol = musicPlayer.volume;

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

    public void PlaySfx(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null) return;

        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }

        sfxPlayer.PlayOneShot(clip);
    }

    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalMusicVol, fadeDuration);
    }

}
