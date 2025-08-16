using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    public Sound[] musicClips, sfxClips;
    public AudioSource musicSource, sfxSource, auxSource;
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
        return sfxSource.mute;
    }

    public void SetMusicVolume(float volume) {
        musicSource.volume = volume;
        TestSound(volume);
    }

    public void SetSfxVolume(float volume) {
        sfxSource.volume = volume;
        TestSound(volume);
    }
    
    public IEnumerator UpdateScene(float duration, bool increaseVolume, bool fadeBg, String sceneToLoad) {
        yield return StartCoroutine(StartFade(duration, increaseVolume, fadeBg));
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public IEnumerator StartFade(float duration, bool increaseVolume, bool fadeBackground)
    {
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
