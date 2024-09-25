using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuFunctions : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;
    public Sprite activeSprite, deactivatedSprite;
    public Image fullScreenBtn, muteMusicBtn, muteSfxBtn;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    public void Start() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();

        var i = -1;
        var found = -1;
        while(found == -1 && i < resolutions.Length - 1) {
            i++;
            string option = resolutions[i].width + " X " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height) {
                found = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();

        if (found != -1) {
            resolutionDropdown.value = found;
        } else {
            resolutionDropdown.value = resolutions.Length - 1;
            SetResolution(resolutionDropdown.value);
        }
    }

    public void SetResolution(int resolutionIndex) {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    public void ToggleMusic() {
        muteMusicBtn.sprite = AudioManager.Instance.ToggleMusic() ? activeSprite : deactivatedSprite;
    }

    public void ToggleSfx() {
        muteSfxBtn.sprite = AudioManager.Instance.ToggleSfx() ? activeSprite : deactivatedSprite;
    }

    public void SetMusicVolume() {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSfxVolume() {
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);
    }

    public void ToggleFullscreen() {
        Screen.fullScreen = !Screen.fullScreen;
        fullScreenBtn.sprite = Screen.fullScreen ? activeSprite : deactivatedSprite;
    }
    
    public void EnterMissionSelection(int activateTutorial) {
        PlayerPrefs.SetInt("tutorialActivated", activateTutorial);
        SceneManager.LoadScene("MissionSelection");
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame() { Application.Quit(); }
}
