using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControllerImage : MonoBehaviour {
    public GameObject tutorialCanvas;
    
    public List<SpriteList> tutorialImages;
    
    public Image leftTutorialImage, rightTutorialImage;
    public TMP_Text leftTutorialText, rightTutorialText;
    
    public GameObject prevTutorialButton, nextTutorialButton;
    
    private Coroutine leftImageGifCoroutine, rightImageGifCoroutine;
    private int tutorialIndex;
    
    public void GoToNextTutorial() { UpdateTutorial(); }

    public void GoToPrevTutorial() {
        tutorialIndex -= 4;
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
        try { StopCoroutine(leftImageGifCoroutine); } catch {}
        try { StopCoroutine(rightImageGifCoroutine); } catch {}
        
        prevTutorialButton.SetActive(tutorialIndex != 0);
        
        if (tutorialImages[tutorialIndex].sprites.Count == 1) {
            leftTutorialImage.sprite = tutorialImages[tutorialIndex].sprites[0];
            leftTutorialText.text = Constants.TUTORIAL_TEXTS[tutorialIndex];
        } else {
            leftImageGifCoroutine = StartCoroutine(DisplayGifTutorial(leftTutorialImage, tutorialImages[tutorialIndex].sprites));
        }
        tutorialIndex++;

        if (tutorialImages[tutorialIndex].sprites.Count == 1) {
            rightTutorialImage.sprite = tutorialImages[tutorialIndex].sprites[0];
            rightTutorialText.text = Constants.TUTORIAL_TEXTS[tutorialIndex];
        } else {
            rightImageGifCoroutine = StartCoroutine(DisplayGifTutorial(rightTutorialImage, tutorialImages[tutorialIndex].sprites));
        }
        tutorialIndex++;
        
        switch (GameControllerScript.Instance.currentMissionNumber) {
            case 0:
                nextTutorialButton.SetActive(tutorialIndex < Constants.TUTORIAL_MISSION_0_MAX);
                break;
            case 1:
                nextTutorialButton.SetActive(tutorialIndex < Constants.TUTORIAL_MISSION_1_MAX);
                break;
            case 2:
                nextTutorialButton.SetActive(tutorialIndex < tutorialImages.Count);
                break;
        }
    }

    private IEnumerator DisplayGifTutorial(Image image, List<Sprite> imageList) {
        var index = 0;
        while (true) {
            image.sprite = imageList[index % imageList.Count];
            index++;
            yield return new WaitForSecondsRealtime(2f);
        }
    }
}
