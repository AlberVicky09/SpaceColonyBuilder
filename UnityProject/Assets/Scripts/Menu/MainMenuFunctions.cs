using System.Collections.Generic;
using System.Linq;
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
    private bool isThereASaveGame;
    Resolution[] resolutions;
    private int currentResolutionIndex;
    private FullScreenMode currentFullScreenMode;
    
    public void Start() {
        AudioManager.Instance.SetMusic(MusicTrackNamesEnum.MenuBG);
        SetUpResolutions();

        if (Utils.ReadFile("missionsAvailable").Equals(Constants.FILE_NOT_FOUND)) {
            try {
                resumeButton.GetComponent<Image>().sprite = resumeButtonInactive;
                resumeButton.GetComponent<Button>().interactable = false;
                resumeButton.GetComponent<MenuItemWiggle>().enabled = false;
            } catch {}
        } else {
            isThereASaveGame = true;
        }
    }

    public void SetUpResolutions() {
        resolutions = Screen.resolutions
            .Where(r => Mathf.Abs((float)r.width / r.height - 16f / 9f) < 0.01f)
            .GroupBy(r => (r.width, r.height))
            .Select(g => g.First())
            .ToArray();

        var currentFoundSolution = -1;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        var found = false;

        foreach (var resolution in resolutions) {
            string option = resolution.width + " X " + resolution.height;
            
            options.Add(option);
                
            if (resolution.width == Screen.width &&
                    resolution.height == Screen.height) {
                Debug.Log("Current resolution is " + option);
                currentFoundSolution = options.Count - 1;
                found = true;
            }
        }

        resolutionDropdown.AddOptions(options);
        if (!found) {
            resolutionDropdown.value = resolutionDropdown.options.Count - 1;
            SetResolution(resolutionDropdown.value);
        } else {
            resolutionDropdown.value = currentFoundSolution;
        }
        resolutionDropdown.RefreshShownValue();
    }
    
    public void SetResolution(int resolutionIndex) {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, currentFullScreenMode);
        currentResolutionIndex = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
        ScreenResizeUtility.ApplyLetterbox();
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
        if (Screen.fullScreen) {
            // Go windowed
            Screen.fullScreenMode = currentFullScreenMode = FullScreenMode.Windowed;
            // Optionally restore a windowed resolution
            SetResolution(currentResolutionIndex);
            fullScreenBtn.sprite = deactivatedSprite;
        } else {
            // Go fullscreen window
            Screen.fullScreenMode = currentFullScreenMode = FullScreenMode.FullScreenWindow;
            SetResolution(resolutions.Length - 1);
            fullScreenBtn.sprite = activeSprite;
        }
    }

    public void EnterStartNewGame() {
        //If there is already a save game, display ensure delete save
        if (isThereASaveGame) {
            deleteSaveCanvas.SetActive(true);
        } else {
            selectTutorialCanvas.SetActive(true);
        }
    }
    
    public void StartNewGame(int activateTutorial) {
        Utils.DeleteSaveFile("missionsAvailable");
        EnterMissionSelection(activateTutorial);
    }
    
    public void EnterMissionSelection(int activateTutorial) {
        PlayerPrefs.SetInt("tutorialActivated", activateTutorial);
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MissionSelection"));
    }

    public void ReturnToMainMenu() {
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MainMenu"));
    }
    
    public void QuitGame() { Application.Quit(); }

    public void RedirectToLink(string link) {
        Application.OpenURL(link);
    }
}
