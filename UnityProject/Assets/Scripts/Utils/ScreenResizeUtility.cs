using UnityEngine;

public class ScreenResizeUtility : MonoBehaviour {
    
    public static ScreenResizeUtility Instance { get; private set; }
    private static float targetAspectRatio = 16f / 9f;
    
    private int lastWidth;
    private int lastHeight;
    
    public void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    void Update() {
        if (Screen.width != lastWidth || Screen.height != lastHeight) {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            
            OnWindowResize();
        }
    }

    void OnWindowResize() { ApplyLetterbox(); }
    
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
