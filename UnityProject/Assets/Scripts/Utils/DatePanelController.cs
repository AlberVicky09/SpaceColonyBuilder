using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatePanelController : MonoBehaviour {

    public static DatePanelController Instance;
    public int day, month, year;
    public TMP_Text dayText, monthText, yearText;
    public Button pauseButton, playButton, fastButton;
    public Sprite pauseButtonOn, pauseButtonOff, playButtonOn, playButtonOff, fastButtonOn, fastButtonOff;
    public SpeedLevels currentSpeed = SpeedLevels.NORMAL;
    public SpeedLevels prevSpeed = SpeedLevels.NORMAL;

    void Start() {
        if (Instance == null) {
            Instance = this;
        }
        UpdateDayText();
        UpdateMonthText();
        UpdateYearText();
    }

    public void SwapButtons(SpeedLevels newSpeed) {
        switch(currentSpeed) {
            case SpeedLevels.STOPPED:
                pauseButton.GetComponent<Image>().sprite = pauseButtonOff;
                break;
            case SpeedLevels.NORMAL:
                playButton.GetComponent<Image>().sprite = playButtonOff;
                break;
            case SpeedLevels.FAST:
                fastButton.GetComponent<Image>().sprite = fastButtonOff;
                break;
        }

        switch(newSpeed) {
            case SpeedLevels.STOPPED:
                pauseButton.GetComponent<Image>().sprite = pauseButtonOn;
                break;
            case SpeedLevels.NORMAL:
                playButton.GetComponent<Image>().sprite = playButtonOn;
                break;
            case SpeedLevels.FAST:
                fastButton.GetComponent<Image>().sprite = fastButtonOn;
                break;
        }

        //Update old speed
        prevSpeed = currentSpeed;
        currentSpeed = newSpeed;
    }

    public IEnumerator StartDayCicle() {
        while (true) {
            //Every 15 seconds, a day passes
            yield return new WaitForSeconds(5);
            Debug.Log("Day passed");
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
