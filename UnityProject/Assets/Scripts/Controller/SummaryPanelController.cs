using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SummaryPanelController : MonoBehaviour {

    public GameObject summaryPanel;
    public GameObject summaryItemProp;

    public List<TMP_Text> summaryTotalTexts;

    private Dictionary<ResourceEnum, int> totalCostsMap;
    private Dictionary<ResourceEnum, int> currentCostMap;

    private List<GameObject> summaryItems = new List<GameObject>();
    
    private bool isSummaryActive = false;
    
    public void DisplaySummaryPanel() {
        //Reset offset to top
        var nextItemPosition = Constants.INITIAL_SUMMARY_ITEM_POSITION;
        
        //Reset total cost map
        totalCostsMap = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            totalCostsMap.Add(resource, 0);
        }

        //Only if there is from that prop
        foreach (var item in GameControllerScript.Instance.propDictionary) {
            if (!PropsEnum.MainBuilding.Equals(item.Key) && item.Value != null && item.Value.Count != 0) {
                
                //Instantiate new summary item
                var summaryItem = Instantiate(summaryItemProp, summaryPanel.transform, false);
                //Setup position related to parent
                var summaryItemTransform = summaryItem.GetComponent<RectTransform>();
                summaryItemTransform.localPosition = nextItemPosition;
                Utils.SetAnchorPresets(summaryItemTransform, AnchorPresets.TopCenter);
                
                //Setup item values and calculate total cost
                var itemMultipliedCost = summaryItem.GetComponent<SummaryItemController>()
                    .SetUpSummaryItem(item.Key,
                        GameControllerScript.Instance.propSpriteDictionary[item.Key],
                        Constants.PROPS_SUMMARY_NAME[item.Key],
                        item.Value.Count);
                totalCostsMap = totalCostsMap.Concat(itemMultipliedCost)
                    .GroupBy(kvp => kvp.Key)
                    .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));
                
                //Move offset for next item
                nextItemPosition.y += Constants.SUMMARY_OFFSET;
            }
        }
        
        //Update total summary item
        for(int i = 0; i < summaryTotalTexts.Count; i++) {
            summaryTotalTexts[i].text = totalCostsMap[(ResourceEnum)i].ToString();
        }
    }

    public void ToggleSummaryMenu() {
        if (isSummaryActive) {
            FlushOnClose();
            summaryPanel.SetActive(false);
            GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
        } else {
            GameControllerScript.Instance.PauseGame();
            summaryPanel.SetActive(true);
            DisplaySummaryPanel();
        }

        isSummaryActive = !isSummaryActive;
    }
    
    public void FlushOnClose() {
        //Reset summary items (delete current)
        foreach (var summaryItem in summaryItems) { Destroy(summaryItem); }
        summaryItems.Clear();
    }
}
