using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatePanelController : MonoBehaviour
{
    private GameControllerScript gameControllerScript;
    private UIUpdateController uiUpdateController;
    public int day, month, year;
    public TMP_Text dayText, monthText, yearText;
    public Button pauseButton, playButton, fastButton;
    public Sprite pauseButtonOn, pauseButtonOff, playButtonOn, playButtonOff, fastButtonOn, fastButtonOff;
    int prevActiveButton = 0;

    void Start()
    {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        uiUpdateController = GameObject.Find("GameController").GetComponent<UIUpdateController>();
        StartCoroutine("StartDayCicle");
        UpdateDayText();
        UpdateMonthText();
        UpdateYearText();
    }

    public void PauseVelocity() {
        gameControllerScript.PauseGame();
        SwapButtons(-1);
    }

    public void NormalVelocity() {
        gameControllerScript.PlayVelocity(Constants.TIME_SCALE_NORMAL);
        SwapButtons(0);
    }

    public void FastVelocity() {
        gameControllerScript.PlayVelocity(Constants.TIME_SCALE_FAST);
        SwapButtons(1);
    }

    public void SwapButtons(int newButton) {
        switch(prevActiveButton) {
            case -1:
                pauseButton.GetComponent<Image>().sprite = pauseButtonOff;
                break;
            case 0:
                playButton.GetComponent<Image>().sprite = playButtonOff;
                break;
            case 1:
                fastButton.GetComponent<Image>().sprite = fastButtonOff;
                break;
        }

        switch(newButton) {
            case -1:
                pauseButton.GetComponent<Image>().sprite = pauseButtonOn;
                break;
            case 0:
                playButton.GetComponent<Image>().sprite = playButtonOn;
                break;
            case 1:
                fastButton.GetComponent<Image>().sprite = fastButtonOn;
                break;
        }

        prevActiveButton = newButton;
    }

    private IEnumerator StartDayCicle() {
        while (true) {
            //Every 15 seconds, a day passes
            yield return new WaitForSeconds(15);
            IncreaseDay();
        }
    }

    private void IncreaseDay() {
        
        //Increase day and month/year if needed
        Debug.Log("Day passed");
        day++;
        if (day > 30) {
            day = 1;
            month++;
            if (month > 12) {
                month = 1;
                year++;
                UpdateYearText();
            }
            UpdateMonthText();
        }
        UpdateDayText();

        //Consume resources every 15 days
        if (day % 15 == 0) {
            uiUpdateController.ConsumeResources();
        }
    }

    private void UpdateDayText() {
        dayText.text = day.ToString();
    }
    private void UpdateMonthText() {
        monthText.text = Constants.CALENDAR_MAP[month]; 
    }
    private void UpdateYearText() {
        yearText.text = year.ToString();
    }
}
