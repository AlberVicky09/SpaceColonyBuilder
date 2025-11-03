using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private float targetAspectWidth = 16f;
    private float targetAspectHeight = 9f;
    
    private void ApplyLetterbox() {
        Camera cam = Camera.main;

        float targetAspect = targetAspectWidth / targetAspectHeight;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f) {
            // Add black bars top/bottom (pillarbox)
            Rect rect = cam.rect;

            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;

            cam.rect = rect;
        } else {
            // Add black bars left/right (letterbox)
            float scaleWidth = 1f / scaleHeight;

            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;

            cam.rect = rect;
        }
    }
    
    private int lastWidth;
    private int lastHeight;
    
    void Update() {
        if (Screen.width != lastWidth || Screen.height != lastHeight) {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            
            OnWindowResize(lastWidth, lastHeight);
        }
    }

    void OnWindowResize(int width, int height) {
        Debug.Log($"Window resized to {width}x{height}");
        ApplyLetterbox();
    }
    
    public void Start() {
        AudioManager.Instance.SetMusic(MusicTrackNamesEnum.MenuBG);
        lastWidth = Screen.width;
        lastHeight = Screen.height;
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
        ApplyLetterbox();
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
