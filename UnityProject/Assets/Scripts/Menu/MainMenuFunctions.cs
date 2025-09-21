using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuFunctions : MonoBehaviour {
    
    public Slider musicSlider, sfxSlider;
    public Sprite activeSprite, deactivatedSprite;
    public Image fullScreenBtn, muteMusicBtn, muteSfxBtn;
    public TMP_Dropdown resolutionDropdown;
    public GameObject resumeButton;
    public Sprite resumeButtonActive, resumeButtonInactive;
    Resolution[] resolutions;

    public void Start() {
        AudioManager.Instance.SetMusic(MusicTrackNamesEnum.MenuBG);
        SetUpResolutions();

        if (Utils.ReadFile("missionsAvailable") == null) {
            try {
                resumeButton.GetComponent<Image>().sprite = resumeButtonInactive;
                resumeButton.GetComponent<Button>().interactable = false;
                resumeButton.GetComponent<MenuItemWiggle>().enabled = false;
            } catch {}
        }
    }

    public void SetUpResolutions() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        var found = false;

        foreach (var resolution in Screen.resolutions) {
            if (Constants.RESOLUTIONS_VALID_WIDTHS.Contains(resolution.width)
                && Constants.RESOLUTIONS_VALID_HEIGHTS.Contains(resolution.height)) {
                
                string option = resolution.width + " X " + resolution.height;
                options.Add(option);
                
                if (resolution.width == Screen.width &&
                    resolution.height == Screen.height) {
                    resolutionDropdown.value = options.Count - 1;
                    found = true;
                }
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();

        if (!found) {
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
        AudioManager.Instance.musicSource.mute = false;
        muteMusicBtn.sprite = deactivatedSprite;
    }

    public void SetSfxVolume() {
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);
        AudioManager.Instance.auxSource.mute = false;
        muteSfxBtn.sprite = deactivatedSprite;
    }

    public void ToggleFullscreen() {
        Screen.fullScreen = !Screen.fullScreen;
        fullScreenBtn.sprite = Screen.fullScreen ? activeSprite : deactivatedSprite;
    }

    public void StartNewGame(int activateTutorial) {
        Utils.DeleteSaveFile();
        EnterMissionSelection(activateTutorial);
    }
    
    public void EnterMissionSelection(int activateTutorial) {
        PlayerPrefs.SetInt("tutorialActivated", activateTutorial);
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MissionSelection"));
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame() { Application.Quit(); }

    public void RedirectToLink(string link) {
        Application.OpenURL(link);
    }
}
