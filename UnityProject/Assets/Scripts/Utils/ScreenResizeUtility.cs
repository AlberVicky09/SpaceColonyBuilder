using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenResizeUtility : MonoBehaviour {
    
    public static ScreenResizeUtility Instance { get; private set; }
    private static float targetAspectRatio = 16f / 9f;
    public FullScreenMode currentFullScreenMode;
    public Resolution[] resolutions;
    public Resolution selectedResolution;
    
    private int lastWidth;
    private int lastHeight;

    public void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += OnSceneChanged;
        SetUpResolutions();
    }
    
    void OnSceneChanged(Scene prevScene, Scene newScene) {
        if (Screen.fullScreenMode != FullScreenMode.FullScreenWindow ||
            Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) {
            ApplyLetterbox();
        }
    }
    
    void Update() {
        if (Screen.width != lastWidth || Screen.height != lastHeight) {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            
            ApplyLetterbox();
        }
    }

    private void SetUpResolutions() {
        resolutions = Screen.resolutions
            .Where(r => Mathf.Abs((float)r.width / r.height - 16f / 9f) < 0.01f)
            .GroupBy(r => (r.width, r.height))
            .Select(g => g.First())
            .ToArray();
        foreach (var resolution in resolutions) {
            if (resolution.width == Screen.width &&
                resolution.height == Screen.height) {
                selectedResolution = resolution;
            }
        }
    }
    
    public void UpdateResolution(int resolutionIndex) {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, currentFullScreenMode);
        ApplyLetterbox();
    }
    
    public static void ApplyLetterbox() {
        Camera cam = Camera.main;
        
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

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
}
