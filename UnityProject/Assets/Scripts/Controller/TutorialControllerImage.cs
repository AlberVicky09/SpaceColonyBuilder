using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControllerImage : MonoBehaviour {
    public GameObject tutorialCanvas;
    
    public List<Sprite> tutorialImages;
    
    public Image leftTutorialImage, rightTutorialImage;
    public TMP_Text leftTutorialText, rightTutorialText;
    
    public GameObject prevTutorialButton, nextTutorialButton;
    
    private int tutorialIndex;
    
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
        tutorialIndex = GameControllerScript.Instance.currentMissionNumber switch {
            0 => 0,
            1 => Constants.TUTORIAL_MISSION_0_MAX,
            2 => Constants.TUTORIAL_MISSION_1_MAX,
            _ => 0
        };
        UpdateTutorial();
    }

    private void UpdateTutorial() {
        Debug.Log("Tutorial index before updating images: " + tutorialIndex);
        prevTutorialButton.SetActive(tutorialIndex != 0);

        switch (GameControllerScript.Instance.currentMissionNumber) {
            case 0:
                nextTutorialButton.SetActive(tutorialIndex != Constants.TUTORIAL_MISSION_0_MAX - 1);
                break;
            case 1:
                nextTutorialButton.SetActive(tutorialIndex != Constants.TUTORIAL_MISSION_1_MAX - 1);
                break;
            case 2:
                nextTutorialButton.SetActive(tutorialIndex != tutorialImages.Count - 1);
                break;
        }
        
        leftTutorialImage.sprite = tutorialImages[tutorialIndex];
        leftTutorialText.text = Constants.TUTORIAL_TEXTS[tutorialIndex];
        tutorialIndex++;
        
        rightTutorialImage.sprite = tutorialImages[tutorialIndex];
        rightTutorialText.text = Constants.TUTORIAL_TEXTS[tutorialIndex];
    }
}
