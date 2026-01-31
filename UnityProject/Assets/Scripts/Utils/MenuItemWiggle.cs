using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItemWiggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
    
    private bool isHovering;
    public int wiggleDirection;
    private Quaternion startingRotation;
    private float currentAngle, targetAngle;

    void Start() {
        isHovering = false;
        startingRotation = transform.localRotation;
        currentAngle = 0f;
        targetAngle = 0f;
    }

    void Update() {
        if (isHovering) {
            currentAngle += Constants.MENU_ITEM_WIGGLE_SPEED * wiggleDirection * Time.unscaledDeltaTime;
            if (Mathf.Abs(currentAngle) > Constants.MENU_ITEM_WIGGLE_DISPLACEMENT) {
                wiggleDirection *= -1;
                currentAngle = Mathf.Clamp(currentAngle, -Constants.MENU_ITEM_WIGGLE_DISPLACEMENT, Constants.MENU_ITEM_WIGGLE_DISPLACEMENT);
            }
            targetAngle = currentAngle;
            
            transform.localRotation = startingRotation * Quaternion.Euler(0f, 0f, currentAngle);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
        currentAngle = 0f;
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnHoverMenu);
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;
        transform.localRotation = startingRotation;
    }

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnClickMenu);
        transform.localRotation = startingRotation;
    }
}
