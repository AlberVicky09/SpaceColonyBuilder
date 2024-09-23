using System.Collections.Generic;
using UnityEngine;

public class InteractableButtonManager : MonoBehaviour {
    
    public GameObject interactableButtonPrefab;
    public List<GameObject> interactableButtonList;
    private float buttonRadius = 275f;

    private void Awake() {
        interactableButtonList = new List<GameObject>();
    }

    public void PlaceButtonsInCircle(int buttonNumber) {
        //Activate canvas
        gameObject.SetActive(true);
        
        //Add needed buttons
        while(buttonNumber > interactableButtonList.Count) {
            interactableButtonList.Add(Instantiate(interactableButtonPrefab, transform));
        }

        //Locate all buttons
        for (int i = 0; i < buttonNumber; i++) {
            //Get angle for each button
            var positionAngle = Mathf.PI * 2 * i / buttonNumber;
            
            //Get relative position of each button
            var posX = Mathf.Cos(positionAngle) * buttonRadius;
            var posY = Mathf.Sin(positionAngle) * buttonRadius;
            
            //Ensure the button is active
            interactableButtonList[i].SetActive(true);
            //Locate relatively button
            interactableButtonList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
        }
        
        //Hide exceeding buttons
        for (int i = buttonNumber; i < interactableButtonList.Count; i++) {
            interactableButtonList[i].SetActive(false);
        }
    }
}