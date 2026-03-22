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
    private bool firstFrame = true;
    public float auxMusicSourceVolume = 1f;
    public bool isMusicMuted = false;
    public float auxSfxSourceVolume = 1f;
    public bool isSfxMuted = false;
    public float auxMusicTransitionVolume = 1f;

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }

    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { StartCoroutine(FadeInScene(1.5f)); }
    
    public void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public bool SetMusic(MusicTrackNamesEnum audioClip, bool loop = true) {
        var s = Array.Find(musicClips, x => x.musicTrackName.Equals(audioClip));
        if (s != null) {
            musicSource.clip = s.clip;
            musicSource.loop = loop;
            musicSource.Play();
            return true;
        }

        return false;
    }
    
    public void PlayMusic(MusicTrackNamesEnum audioClip, bool loop = true) {
        if(SetMusic(audioClip, loop)) { StartCoroutine(MusicFadeIn(1.5f)); }
    }

    public IEnumerator MusicFadeIn(float fadeTime) {
        var volumeLimit = musicSource.volume;
        float elapsed = 0f;
        musicSource.volume = 0f;
        
        while (elapsed < fadeTime) {
            elapsed += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            musicSource.volume = Mathf.Lerp(0f, volumeLimit, elapsed / fadeTime);
            yield return null;
        }

        //Ensure volume is exactly set
        musicSource.volume = volumeLimit;
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
        if(musicSource.volume == 0f) {
            musicSource.volume = auxMusicSourceVolume;
            isMusicMuted = false;
            TestSound(musicSource.volume);
            return false;
        }

        isMusicMuted = true;
        musicSource.volume = 0f;
        return true;
    }

    public bool ToggleSfx() {
        if (sfxSource.volume == 0f) {
            sfxSource.volume = auxSfxSourceVolume;
            TestSound(sfxSource.volume);
            sfxSourceList.ForEach(UpdateSfxSources);
            isSfxMuted = false;
            return false;
        }

        isSfxMuted = true;
        sfxSource.volume = 0f;
        return true;
    }

    public void SetMusicVolume(float volume) {
        musicSource.volume = volume;
        auxMusicSourceVolume = volume;
        TestSound(volume);
    }

    public void SetSfxVolume(float volume) {
        sfxSource.volume = volume;
        sfxSourceList.ForEach(UpdateSfxSources);
        auxSfxSourceVolume = volume;
        TestSound(volume);
    }

    public void AddNewSfxSource(SfxSource observerSfxSource) {
        //Add to observer list
        sfxSourceList.Add(observerSfxSource);
        UpdateSfxSources(observerSfxSource);
    }

    public void RemoveSfxSource(SfxSource observerSfxSource) { sfxSourceList.Remove(observerSfxSource); }

    private void UpdateSfxSources(SfxSource observerSfxSource) {
        observerSfxSource.source.volume = sfxSource.volume;
    }
    
    public IEnumerator UpdateScene(float duration, String sceneToLoad) {
        //Clear all SFX before moving from scene
        sfxSourceList.Clear();
        yield return StartCoroutine(FadeOutScene(duration));
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
    
    public IEnumerator FadeOutScene(float duration) {
        float currentTime = 0;
        float currentImgValue, currentAudioValue;
        auxMusicTransitionVolume = musicSource.volume;
        fadeToBlackImage.gameObject.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        //Activate black screen and set alpha depending on situation
        var originalBlack = fadeToBlackImage.color;
        originalBlack.a = 0f;
        fadeToBlackImage.color = originalBlack;
        
        while (currentTime < duration) {
            if (firstFrame) {
                firstFrame = false;
            } else {
                //Get increased/decreased alpha and volume
                currentTime += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
                currentImgValue = Mathf.Lerp(0, 1, currentTime / duration);
                currentAudioValue = Mathf.Lerp(auxMusicTransitionVolume, 0, currentTime / duration);

                //Set updated alpha and volume
                originalBlack.a = currentImgValue;
                fadeToBlackImage.color = originalBlack;
                musicSource.volume = currentAudioValue;
            }

            yield return null;
        }

        musicSource.volume = 0f;
        originalBlack.a = 1f;
        fadeToBlackImage.color = originalBlack;
    }
    
    public IEnumerator FadeInScene(float duration) {
        float currentTime = 0;
        float currentImgValue, currentAudioValue;

        //Activate black screen and set alpha depending on situation
        var originalBlack = fadeToBlackImage.color;
        originalBlack.a = 1f;
        fadeToBlackImage.color = originalBlack;
        
        while (currentTime < duration) {
            if (firstFrame) {
                firstFrame = false;
            } else {
                //Get increased/decreased alpha and volume
                currentTime += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
                currentImgValue = Mathf.Lerp(1, 0, currentTime / duration);
                currentAudioValue = Mathf.Lerp(0, auxMusicTransitionVolume, currentTime / duration);
                
                //Set updated alpha and volume
                originalBlack.a = currentImgValue;
                fadeToBlackImage.color = originalBlack;
                musicSource.volume = currentAudioValue;
            }

            yield return null;
        }
        
        musicSource.volume = auxMusicTransitionVolume;
        originalBlack.a = 0f;
        fadeToBlackImage.color = originalBlack;
        fadeToBlackImage.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
