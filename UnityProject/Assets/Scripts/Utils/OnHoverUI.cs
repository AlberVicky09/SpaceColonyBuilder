using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string hoveringDisplayText;
    public GameObject hoveringCanvas;
    public TMP_Text hoveringCanvasText;
    private Vector2 HOVERING_DISPLACEMENT = new Vector2(50f, -70f);

    public void OnPointerEnter(PointerEventData eventData) {
        hoveringCanvas.SetActive(true);
        hoveringCanvas.transform.position = eventData.position + HOVERING_DISPLACEMENT;
        hoveringCanvasText.text = hoveringDisplayText;
    }

    public void OnPointerExit(PointerEventData eventData) {
        hoveringCanvas.SetActive(false);
    }

    public void OnDisable() {
        hoveringCanvas.SetActive(false);
    }
}
