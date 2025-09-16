using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance { get; private set; }
    public Sound[] musicClips, sfxClips;
    public AudioSource musicSource, sfxSource, auxSource;
    public List<SfxSource> sfxSourceList;
    public Image fadeToBlackImage;

    public void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        musicSource.Stop();
        sfxSource.Stop();
        auxSource.Stop();
        fadeToBlackImage.gameObject.SetActive(false);
    }

    public void PlayMusic(MusicTrackNamesEnum audioClip, bool loop = true) {
        var s = Array.Find(musicClips, x => x.musicTrackName.Equals(audioClip));
        if (s != null) {
            musicSource.clip = s.clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void PlaySfx(SfxTrackNamesEnum audioClip, bool loop = false) {
        var s = Array.Find(sfxClips, x => x.sfxTrackName.Equals(audioClip));
        if (s != null) {
            if (loop) {
                sfxSource.clip = s.clip;
                sfxSource.loop = loop;
                sfxSource.Play();
            } else {
                sfxSource.PlayOneShot(s.clip);
            }
        }
    }

    private void TestSound(float volume) {
        auxSource.volume = volume;
        auxSource.PlayDelayed(0.2f);
    }

    public bool ToggleMusic() {
        musicSource.mute = !musicSource.mute;
        if(!musicSource.mute) {
            TestSound(musicSource.volume);
        }
        return musicSource.mute;
    }

    public bool ToggleSfx() {
        sfxSource.mute = !sfxSource.mute;
        if(!sfxSource.mute) {
            TestSound(sfxSource.volume);
        }
        sfxSourceList.ForEach(UpdateSfxSources);
        return sfxSource.mute;
    }

    public void SetMusicVolume(float volume) {
        musicSource.volume = volume;
        TestSound(volume);
    }

    public void SetSfxVolume(float volume) {
        sfxSource.volume = volume;
        sfxSourceList.ForEach(UpdateSfxSources);
        TestSound(volume);
    }

    public void AddNewSfxSource(SfxSource observerSfxSource) {
        //Add to observer list
        sfxSourceList.Add(observerSfxSource);
        UpdateSfxSources(observerSfxSource);
    }

    public void RemoveSfxSource(SfxSource observerSfxSource) { sfxSourceList.Remove(observerSfxSource); }

    private void UpdateSfxSources(SfxSource observerSfxSource) {
        observerSfxSource.source.mute = sfxSource.mute;
        observerSfxSource.source.volume = sfxSource.volume;
    }

    public IEnumerator UpdateScene(float duration, bool increaseVolume, bool fadeBg, String sceneToLoad) {
        //Clear all SFX before moving from scene
        sfxSourceList.Clear();
        yield return StartCoroutine(StartFade(duration, increaseVolume, fadeBg));
        SceneManager.LoadScene(sceneToLoad);
        fadeToBlackImage.gameObject.SetActive(false);
    }
    
    public IEnumerator StartFade(float duration, bool increaseVolume, bool fadeBackground) {
        fadeToBlackImage.gameObject.SetActive(true);
        float currentTime = 0;
        float start = musicSource.volume = increaseVolume ? 0f : 1f;
        var fadeToBlack = fadeToBlackImage.color;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            var currentValue = Mathf.Lerp(start, 1 - start, currentTime / duration);
            musicSource.volume = currentValue;
            
            if (fadeBackground) {
                fadeToBlack.a = 1 - currentValue;
                fadeToBlackImage.color = fadeToBlack;
            }

            yield return null;
        }
    }
}
