using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItemWiggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
    
    private bool isHovering;
    public int wiggleDirection;
    private Quaternion startingRotation;
    private float minYRotation, maxYRotation;

    void Start() {
        isHovering = false;
        startingRotation = transform.localRotation;
        minYRotation = startingRotation.eulerAngles.y - Constants.MENU_ITEM_WIGGLE_DISPLACEMENT;
        maxYRotation = startingRotation.eulerAngles.y + Constants.MENU_ITEM_WIGGLE_DISPLACEMENT;
    }

    void Update() {
        if(isHovering) {
            transform.Rotate(Vector3.up * Constants.MENU_ITEM_WIGGLE_SPEED * wiggleDirection * Time.deltaTime);

            if (transform.localRotation.eulerAngles.y < minYRotation
                || transform.localRotation.eulerAngles.y > maxYRotation) {
                wiggleDirection *= -1;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnHoverMenu);
    }

    public void OnPointerExit(PointerEventData eventData) {
        ForceStopHovering();
    }

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnClickMenu);
    }

    public void ForceStopHovering() {
        isHovering = false;
        transform.rotation = startingRotation;
    }
}
