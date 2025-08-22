using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryItemController : MonoBehaviour {
    
    public Image summaryItemImageReference;
    public TMP_Text summaryItemNameReference;
    public List<TMP_Text> summaryItemCostReference;
    public List<TMP_Text> summaryItemUnitaryCostReference;
    public TMP_Text summaryItemQuantityReference;

    public Dictionary<ResourceEnum, int> SetUpSummaryItem(PropsEnum propType, Sprite itemSprite, string itemName, int itemQuantity) {
        var totalCostResponseMap = new Dictionary<ResourceEnum, int>();
        
        //Set the cost of each resource
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            //Use number value of enum to setup the cost for each resource
            var currentResourceCost = Constants.PROPS_MANTAINING_COST[propType]
                .GetValueOrDefault(resource, Constants.DEFAULT_MISSING_RESOURCE_VALUE);
            summaryItemUnitaryCostReference[(int)resource].text = currentResourceCost.ToString();
            //Get cost of all of this type
            var propTypeQuantityCost = currentResourceCost * itemQuantity;
            summaryItemCostReference[(int)resource].text = propTypeQuantityCost.ToString();
            //Store it to return it (this way, we will only account it once)
            totalCostResponseMap.Add(resource, propTypeQuantityCost);
        }

        summaryItemImageReference.sprite = itemSprite;
        summaryItemNameReference.text = itemName;
        summaryItemQuantityReference.text = itemQuantity.ToString();

        return totalCostResponseMap;
    }
}
