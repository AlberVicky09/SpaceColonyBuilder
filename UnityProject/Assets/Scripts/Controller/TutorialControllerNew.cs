using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialControllerNew : MonoBehaviour {
    public List<Video> videoClips;
    public VideoPlayer leftVideoPlayer, rightVideoPlayer;
    public TMP_Text leftTutorialText, rightTutorialText;
    public GameObject prevTutorialButton, nextTutorialButton;
    public GameObject pauseCanvas;
    public bool wasInPause;
    private int tutorialIndex;

    private void Start() {
        RestartTutorial();
    }

    public void GoToNextTutorial() {
        tutorialIndex++;
        UpdateTutorial();
    }

    public void GoToPrevTutorial() {
        tutorialIndex -= 3;
        UpdateTutorial();
    }

    public void RestartTutorial() {
        tutorialIndex = 0;
        UpdateTutorial();
    }
    
    private void UpdateTutorial() {
        prevTutorialButton.SetActive(tutorialIndex != 0);

        leftVideoPlayer.clip = videoClips.Find(v =>
            v.clipName == Constants.VIDEO_TUTORIAL_CLIP_NAME + tutorialIndex).clip;
        leftTutorialText.text = Constants.TUTORIAL_TEXTS[tutorialIndex];
        tutorialIndex++;
        rightVideoPlayer.clip = videoClips.Find(v =>
            v.clipName == Constants.VIDEO_TUTORIAL_CLIP_NAME + tutorialIndex).clip;
        rightTutorialText.text = Constants.TUTORIAL_TEXTS[tutorialIndex];

        nextTutorialButton.SetActive(tutorialIndex != videoClips.Count - 1);
    }
}
