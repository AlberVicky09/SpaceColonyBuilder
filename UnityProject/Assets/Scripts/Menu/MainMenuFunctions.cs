using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuFunctions : MonoBehaviour {
    
    public Slider musicSlider, sfxSlider;
    public Sprite activeSprite, deactivatedSprite;
    public Image fullScreenBtn, muteMusicBtn, muteSfxBtn;
    public TMP_Dropdown resolutionDropdown;
    public GameObject resumeButton, deleteSaveCanvas, selectTutorialCanvas;
    public Sprite resumeButtonActive, resumeButtonInactive;
    private int currentResolutionIndex;
    
    public void Start() {
        AudioManager.Instance.SetMusic(MusicTrackNamesEnum.MenuBG);
        SetUpResolutionDropdown();

        if (!MissionInformationController.Instance.missionsRecovered) {
            DisableResumeButton();
        }

        fullScreenBtn.sprite = Screen.fullScreenMode.Equals(FullScreenMode.ExclusiveFullScreen) ||
                               Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ?
                                    activeSprite : deactivatedSprite;
        if (AudioManager.Instance.isMusicMuted) {
            muteMusicBtn.sprite = activeSprite;
            musicSlider.SetValueWithoutNotify(0f);
        } else {
            muteMusicBtn.sprite = deactivatedSprite;
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.auxMusicSourceVolume);
        }

        if (AudioManager.Instance.isSfxMuted) {
            muteSfxBtn.sprite = activeSprite;
            sfxSlider.SetValueWithoutNotify(0f);
        } else {
            muteSfxBtn.sprite = deactivatedSprite;
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.auxSfxSourceVolume);
        }
    }

    public void SetUpResolutionDropdown() {
        var currentFoundSolution = -1;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();

        foreach (var resolution in ScreenResizeUtility.Instance.resolutions) {
            string option = resolution.width + " X " + resolution.height;
            options.Add(option);
            if (resolution.Equals(ScreenResizeUtility.Instance.selectedResolution)) {
                currentResolutionIndex = options.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    public void SetResolution(int resolutionIndex) {
        resolutionDropdown.RefreshShownValue();
        ScreenResizeUtility.ApplyLetterbox();
        ScreenResizeUtility.Instance.UpdateResolution(resolutionIndex);
    }

    public void ToggleMusic() {
        var isMute = AudioManager.Instance.ToggleMusic();
        if (isMute) {
            muteMusicBtn.sprite = activeSprite;
            musicSlider.SetValueWithoutNotify(0f);
        } else {
            muteMusicBtn.sprite = deactivatedSprite;
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.auxMusicSourceVolume);
        }
    }

    public void ToggleSfx() {
        var isMute = AudioManager.Instance.ToggleSfx();
        if (isMute) {
            muteSfxBtn.sprite = activeSprite;
            sfxSlider.SetValueWithoutNotify(0f);
        } else {
            muteSfxBtn.sprite = deactivatedSprite;
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.auxSfxSourceVolume);
        }
    }

    public void SetMusicVolume() {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
        if (musicSlider.value == 0f) {
            muteMusicBtn.sprite = activeSprite;
        } else {
            muteMusicBtn.sprite = deactivatedSprite;
        }
    }

    public void SetSfxVolume() {
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);
        if (sfxSlider.value == 0f) {
            muteSfxBtn.sprite = activeSprite;
        } else {
            muteSfxBtn.sprite = deactivatedSprite;
        }
    }

    public void ToggleFullscreen() {
        if (Screen.fullScreen) {
            // Go windowed
            Screen.fullScreenMode = FullScreenMode.Windowed;
            ScreenResizeUtility.Instance.currentFullScreenMode = FullScreenMode.Windowed;
            // Optionally restore a windowed resolution
            SetResolution(currentResolutionIndex);
            fullScreenBtn.sprite = deactivatedSprite;
        } else {
            // Go fullscreen window
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            ScreenResizeUtility.Instance.currentFullScreenMode = FullScreenMode.FullScreenWindow;
            SetResolution(ScreenResizeUtility.Instance.resolutions.Length - 1);
            fullScreenBtn.sprite = activeSprite;
        }
    }

    public void EnterStartNewGame() {
        //If there is already a save game, display ensure delete save
        if (MissionInformationController.Instance.missionsRecovered) {
            deleteSaveCanvas.SetActive(true);
        } else {
            selectTutorialCanvas.SetActive(true);
        }
    }

    public void DeleteSavedGame() {
        Utils.DeleteSaveFile("missionsAvailable");
        MissionInformationController.Instance.RestartSaveFile();
        DisableResumeButton();
    }

    public void EnterMissionSelection() {
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MissionSelection"));
    }
    
    public void EnterMissionSelection(int activateTutorial) {
        PlayerPrefs.SetInt("tutorialActivated", activateTutorial);
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MissionSelection"));
    }

    public void ReturnToMainMenu() {
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MainMenu"));
    }

    private void DisableResumeButton() {
        try {
            resumeButton.GetComponent<Image>().sprite = resumeButtonInactive;
            resumeButton.GetComponent<Button>().interactable = false;
            resumeButton.GetComponent<MenuItemWiggle>().enabled = false;
        } catch {}
    }
    
    public void QuitGame() { Application.Quit(); }

    public void RedirectToLink(string link) {
        Application.OpenURL(link);
    }
}
