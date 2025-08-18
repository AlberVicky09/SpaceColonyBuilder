using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string hoveringDisplayText;
    public GameObject hoveringCanvas;
    public TMP_Text hoveringCanvasText;
    private const float HOVERING_DISPLACEMENT_X = 50f;
    private const float HOVERING_DISPLACEMENT_Y = 150f;

    public void OnPointerEnter(PointerEventData eventData) {
        hoveringCanvas.SetActive(true);
        //Place over or under depending on current position
        hoveringCanvas.transform.position = eventData.position +
            new Vector2(HOVERING_DISPLACEMENT_X, (eventData.position.y - HOVERING_DISPLACEMENT_Y <= 0) ?
                HOVERING_DISPLACEMENT_Y : -HOVERING_DISPLACEMENT_Y);

        RefreshText();
    }

    public void RefreshText() {
        hoveringCanvasText.text = hoveringDisplayText;
        //Force button width update
        LayoutRebuilder.ForceRebuildLayoutImmediate(hoveringCanvas.GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData) {
        hoveringCanvas.SetActive(false);
    }

    public void OnDisable() {
        hoveringCanvas.SetActive(false);
    }
}
