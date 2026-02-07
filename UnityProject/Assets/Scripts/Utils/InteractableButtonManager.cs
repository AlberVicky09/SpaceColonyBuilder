using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InteractableButtonManager : MonoBehaviour {
    
    public GameObject interactableButtonPrefab;
    public List<GameObject> interactableButtonList;
    public List<Image> interactableButtonImageList;
    private float buttonRadius = 275f;

    private void Awake() {
        interactableButtonList = new List<GameObject>();
    }

    public void PlaceButtonsInCircle(int buttonNumber) {
        //Activate canvas
        gameObject.SetActive(true);
        
        //Add needed buttons
        while(buttonNumber > interactableButtonList.Count) {
            var newButton = Instantiate(interactableButtonPrefab, transform);
            newButton.GetComponent<MenuItemWiggle>().wiggleDirection =
                interactableButtonList.Count % 2 == 0 ? 1 : -1;
            interactableButtonList.Add(newButton);
            interactableButtonImageList.Add(newButton.GetComponentsInChildren<Image>(true)
                .FirstOrDefault(c => c.gameObject != newButton));
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

    public void ForceUpdateOfButtons() {
        foreach (var button in interactableButtonList) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(button.GetComponent<RectTransform>());
        }
    }
}