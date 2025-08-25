using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatePanelController : MonoBehaviour
{
    public int day, month, year;
    public TMP_Text dayText, monthText, yearText;
    public Button pauseButton, playButton, fastButton;
    public Sprite pauseButtonOn, pauseButtonOff, playButtonOn, playButtonOff, fastButtonOn, fastButtonOff;
    int prevActiveButton = 0;

    void Start()
    {
        StartCoroutine("StartDayCicle");
        UpdateDayText();
        UpdateMonthText();
        UpdateYearText();
    }

    public void PauseVelocity() {
        GameControllerScript.Instance.PauseGame();
        SwapButtons(-1);
    }

    public void NormalVelocity() {
        GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
        SwapButtons(0);
    }

    public void FastVelocity() {
        GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_FAST);
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
            yield return new WaitForSeconds(5);
            IncreaseDay();
        }
    }

    private void IncreaseDay() {
        
        //Increase day and month/year if needed
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
        if (day % day == 0) { //TODO FIXME: Revert /15
            GameControllerScript.Instance.uiUpdateController.ConsumeResources();
        }
        
        //Check date mission if needed
        GameControllerScript.Instance.missionController.CheckDateMission(month + (year - 3500) * 12);
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
