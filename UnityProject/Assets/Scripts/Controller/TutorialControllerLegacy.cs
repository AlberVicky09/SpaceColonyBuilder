using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControllerLegacy : MonoBehaviour {
    public GameObject tutorialCanvas;
    public TMP_Text tutorialText;
    public Button nextStepButton, closeButton;

    private List<string> tutorialTexts;
    private int tutorialTextsPointer;

    private int[] tutorialMoveToNextCases = {13, 19 };
    private int[] tutorialCloseCases = { 5, 12 };

    public void Start() {
        tutorialTexts = new List<string>();
        tutorialTexts.Add("This is the main building.\nProtect it or you will lose.");
        tutorialTexts.Add("These are your current resources (Water, food, iron, gold and platinum)");
        tutorialTexts.Add("These are the missions you have to complete to win in this map.\nThey will turn green when completed.");
        tutorialTexts.Add("This countdown marks the time left for enemy ships to attack you.\nIn this tutorial, it won´t happen until the very end, so don´t worry for now.");
        tutorialTexts.Add("Click on the main building to display possible actions.");
        tutorialTexts.Add("Click here to show all buildings that you can create.");
        tutorialTexts.Add("Now, select the gatherer. It will get resources from the ores in the map");
        tutorialTexts.Add("Place it in a valid location. Then, click on it.");
        tutorialTexts.Add("Gatherers have two actions: Gather and Retreat. Select the first one");
        tutorialTexts.Add("Now you can select the material you want. The gatherer will go automatically to the nearest ore.\nFirst, select water");
        tutorialTexts.Add("Once the gatherer is full, it will automatically go back to base to store the materials.");
        tutorialTexts.Add("Click on the gatherer again and change the objective material");
        tutorialTexts.Add("Now, create a food generator from the main building. It will turn your water into food automatically");
        tutorialTexts.Add("Enemies are coming! Click on your gatherer and return it to the base");
        tutorialTexts.Add("Now click on the main building and create a soldier ship");
        tutorialTexts.Add("This ship will patrol surrounding the main base, until he finds an enemy to fight");
        tutorialTexts.Add("Your base has been damaged. Select your main building and click on 'Restore base' button");
        tutorialTexts.Add("Press 'E' to open the summary tab. It will display the monthly cost of all current buildings");
        tutorialTexts.Add("Here you have the count of each item, the cost of each, and the total monthly cost.\nIt will be deducted each 1st of month from your resources");
        tutorialTexts.Add("Keep track of your resources! If you stay 15 days with negative of any of them, you will fail!");
        tutorialTexts.Add("This is all you need to learn. Now complete the missions to end this level.\nHave fun!");
    }

    public void DisplayNextTutorial() {
        //First, pause game and explain the main building
        GameControllerScript.Instance.PauseGame();
        tutorialCanvas.SetActive(true);
        UpdateNextTutorialText();
        CheckNextTutorial();
    }
    
    public void UpdateNextTutorialText() {
        tutorialText.text = tutorialTexts[tutorialTextsPointer++];
    }

    public void CheckNextTutorial() {
        //If tutorial should start closing after this one, change behaviour
        if (tutorialCloseCases.Contains(tutorialTextsPointer)) {
            closeButton.gameObject.SetActive(true);
            nextStepButton.gameObject.SetActive(false);
        //If tutorial should start keeping opened after this one, change behaviour
        }else if (tutorialMoveToNextCases.Contains(tutorialTextsPointer)) {
            closeButton.gameObject.SetActive(false);
            nextStepButton.gameObject.SetActive(true);
        }
    }

    public void MoveToNextTutorial() {
        UpdateNextTutorialText();
        
    }
    
    public void CloseCurrentTutorial() {
        GameControllerScript.Instance.PlayNormalVelocity();
        tutorialCanvas.SetActive(false);
    }
}
