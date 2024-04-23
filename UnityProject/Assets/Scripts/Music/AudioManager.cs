using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    public Sound[] musicClips, sfxClips;
    public AudioSource musicSource, sfxSource;

    public void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void Start() {
        PlayMusic("MainTheme");
    }

    public void PlayMusic(string audioClip, bool loop = true) {
        var s = Array.Find(musicClips, x => x.clipName == audioClip);
        if (s != null) {
            musicSource.clip = s.clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void PlaySfx(string audioClip, bool loop = false) {
        var s = Array.Find(sfxClips, x => x.clipName == audioClip);
        if (s != null) {
            sfxSource.clip = s.clip;
            sfxSource.loop = loop;
            sfxSource.Play();
        }
    }

    public bool ToggleMusic() {
        musicSource.mute = !musicSource.mute;
        if(!musicSource.mute) {
            PlayMusic("SingleLaser", false);
        }
        return musicSource.mute;
    }

    public bool ToggleSfx() {
        sfxSource.mute = !sfxSource.mute;
        if(!sfxSource.mute) {
            PlaySfx("SingleLaser");
        }
        return sfxSource.mute;
    }

    public void SetMusicVolume(float volume) {
        musicSource.volume = volume;
        PlayMusic("SingleLaser", false);
    }

    public void SetSfxVolume(float volume) {
        sfxSource.volume = volume;
        PlaySfx("SingleLaser");
    }
}
