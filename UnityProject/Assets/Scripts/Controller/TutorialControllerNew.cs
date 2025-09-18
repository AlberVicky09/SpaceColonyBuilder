using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialControllerNew : MonoBehaviour {
    public GameObject tutorialCanvas;
    public List<Video> videoClips;
    public VideoPlayer leftVideoPlayer, rightVideoPlayer;
    public TMP_Text leftTutorialText, rightTutorialText;
    public GameObject prevTutorialButton, nextTutorialButton;
    private int tutorialIndex, currentMissionNumber;

    private void Start() {
        currentMissionNumber = PlayerPrefs.GetInt("mission", 0);
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

    public void DisplayTutorialForMission() {
        tutorialCanvas.SetActive(true);
        tutorialIndex = currentMissionNumber switch {
            0 => 0,
            1 => Constants.TUTORIAL_MISSION_0_MAX,
            2 => Constants.TUTORIAL_MISSION_1_MAX,
            _ => 0
        };
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

        switch (currentMissionNumber) {
            case 0:
                nextTutorialButton.SetActive(tutorialIndex != Constants.TUTORIAL_MISSION_0_MAX - 1);
                break;
            case 1:
                nextTutorialButton.SetActive(tutorialIndex != Constants.TUTORIAL_MISSION_1_MAX - 1);
                break;
            case 2:
                nextTutorialButton.SetActive(tutorialIndex != videoClips.Count - 1);
                break;
        }
        
    }
}
