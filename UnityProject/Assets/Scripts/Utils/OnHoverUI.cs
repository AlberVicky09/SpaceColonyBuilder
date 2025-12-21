using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string hoveringDisplayText;
    public bool usesResourceTooltip = false;
    public PropsEnum propType;
    
    private bool isHovering = false;
    
    private Vector2 localPoint, clampedPoint;
    
    private const float HOVERING_DISPLACEMENT_X = 120f;
    private const float HOVERING_RESOURCE_DISPLACEMENT_X = 200f;
    private const float HOVERING_DISPLACEMENT_Y = -100f;
    private const float HOVERING_RESOURCE_DISPLACEMENT_Y = 20f;
    private Vector2 hoveringDisplacement, hoveringResourceDisplacement;

    
    private float hoveringDisplacementCalculatedX;
    private float hoveringResourceDisplacementCalculatedX;
    private float hoveringDisplacementCalculatedY;

    private RectTransform toolTipRT, resourceTooltipRT, canvasRect;
    
    private int lastWidth;
    private int lastHeight;

    void Start() {
        OnWindowResize();
        canvasRect = GameControllerScript.Instance.toolTipCanvas.transform as RectTransform;
        hoveringResourceDisplacement = new Vector2(HOVERING_RESOURCE_DISPLACEMENT_X, HOVERING_RESOURCE_DISPLACEMENT_Y);
        hoveringDisplacement = new Vector2(HOVERING_RESOURCE_DISPLACEMENT_X, HOVERING_DISPLACEMENT_Y);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        ToggleCurrentCanvas(true);
        RefreshCanvasPosition();
        RefreshText();
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        ToggleCurrentCanvas(false);
        isHovering = false;
    }

    public void OnDisable() {
        ToggleCurrentCanvas(false);
        isHovering = false;
    }

    public void Update() {
        if (Screen.width != lastWidth || Screen.height != lastHeight) {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            
            OnWindowResize();
        }
        
        if (isHovering) {
            RefreshCanvasPosition();
        }
    }

    private void RefreshCanvasPosition() {
        // Convert screen mouse position into the canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            GameControllerScript.Instance.cameraMove.cameraGO,
            out localPoint
        );
        
        if (usesResourceTooltip) {
            GameControllerScript.Instance.toolTipResourceGO.localPosition = ClampInsideCanvas(localPoint + hoveringResourceDisplacement);
        } else {
            GameControllerScript.Instance.toolTipGO.localPosition = ClampInsideCanvas(localPoint + hoveringDisplacement);
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
            LayoutRebuilder.ForceRebuildLayoutImmediate(GameControllerScript.Instance.toolTipResourceGO);
        } else {
            GameControllerScript.Instance.toolTipText.text = hoveringDisplayText;
            //Force button width update
            LayoutRebuilder.ForceRebuildLayoutImmediate(GameControllerScript.Instance.toolTipGO);
        }
    }
    
    private void ToggleCurrentCanvas(bool isActive) {
        if (usesResourceTooltip) {
            GameControllerScript.Instance.toolTipResourceGO.gameObject.SetActive(isActive);
        } else {
            GameControllerScript.Instance.toolTipGO.gameObject.SetActive(isActive);
        }
    }
    
    private void OnWindowResize() {
        hoveringDisplacementCalculatedX = Screen.width / 1920f * HOVERING_DISPLACEMENT_X;
        hoveringResourceDisplacementCalculatedX = Screen.width / 1920f * HOVERING_RESOURCE_DISPLACEMENT_X;
        hoveringDisplacementCalculatedY = Screen.height / 1080f * HOVERING_DISPLACEMENT_Y;
    }

    private Vector2 ClampInsideCanvas(Vector2 pos) {
        RectTransform tooltipRect = usesResourceTooltip
            ? GameControllerScript.Instance.toolTipResourceGO
            : GameControllerScript.Instance.toolTipGO;

        Vector2 tooltipSize = tooltipRect.rect.size;

        float halfWidth  = tooltipSize.x * 0.5f;
        float halfHeight = tooltipSize.y * 0.5f;

        float canvasHalfWidth  = canvasRect.rect.width * 0.5f;
        float canvasHalfHeight = canvasRect.rect.height * 0.5f;

        float minX = -canvasHalfWidth + halfWidth;
        float maxX =  canvasHalfWidth - halfWidth;
        float minY = -canvasHalfHeight + halfHeight;
        float maxY =  canvasHalfHeight - halfHeight;

        Vector2 clampedPos = new Vector2(
            Mathf.Clamp(pos.x, minX, maxX),
            Mathf.Clamp(pos.y, minY, maxY)
        );

        return clampedPos;
    }
}
