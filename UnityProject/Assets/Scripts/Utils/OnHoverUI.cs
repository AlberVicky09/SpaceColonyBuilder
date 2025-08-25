using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string hoveringDisplayText;
    public bool usesResourceTooltip = false;
    public PropsEnum propType;

    private Vector2 canvasPosition;
    private bool isHovering = false;
    
    private const float HOVERING_DISPLACEMENT_X = 150f;
    private const float HOVERING_RESOURCE_DISPLACEMENT_X = 250f;
    private const float HOVERING_DISPLACEMENT_Y = 150f;

    public void OnPointerEnter(PointerEventData eventData) {
        ToogleCurrentCanvas(true);
        RefreshCanvasPosition();
        RefreshText();
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        ToogleCurrentCanvas(false);
        isHovering = false;
    }

    public void OnDisable() {
        ToogleCurrentCanvas(false);
        isHovering = false;
    }

    public void Update() {
        if (isHovering) {
            RefreshCanvasPosition();
        }
    }

    private void RefreshCanvasPosition() {
        canvasPosition = Input.mousePosition +
                         new Vector3(
                             usesResourceTooltip ?
                                 HOVERING_RESOURCE_DISPLACEMENT_X : HOVERING_DISPLACEMENT_X,
                             Input.mousePosition.y - HOVERING_DISPLACEMENT_Y <= 0 ? 
                                 HOVERING_DISPLACEMENT_Y : -HOVERING_DISPLACEMENT_Y,
                             0);
        
        if (usesResourceTooltip) {
            GameControllerScript.Instance.toolTipResourceCanvas.transform.position = canvasPosition;
        } else {
            GameControllerScript.Instance.toolTipCanvas.transform.position = canvasPosition;
        }
    }

    public void RefreshText() {
        if (usesResourceTooltip) {
            //Setup costs of resources
            for (int i = 0; i < Enum.GetValues(typeof(ResourceEnum)).Length; i++) {
                GameControllerScript.Instance.toolTipResourceText[i].text =
                    Constants.PROP_CREATION_PRICES[propType].GetValueOrDefault((ResourceEnum)i, 0).ToString();
            }
            //Force button width update
            LayoutRebuilder.ForceRebuildLayoutImmediate(GameControllerScript.Instance.toolTipResourceCanvasRect);
        } else {
            GameControllerScript.Instance.toolTipText.text = hoveringDisplayText;
            //Force button width update
            LayoutRebuilder.ForceRebuildLayoutImmediate(GameControllerScript.Instance.toolTipCanvasRect);
        }
    }
    
    private void ToogleCurrentCanvas(bool isActive) {
        if (usesResourceTooltip) {
            GameControllerScript.Instance.toolTipResourceCanvas.SetActive(isActive);
        } else {
            GameControllerScript.Instance.toolTipCanvas.SetActive(isActive);
        }
    }
}
