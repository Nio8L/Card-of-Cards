using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public class SigilTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private TooltipTrigger tooltipTrigger;
    
    private int index;
    private CardInHand cardInHand;
    private CardInCombat cardInCombat;

    private void Start() {
        UpdateSigilTooltip();
    }

    public void UpdateSigilTooltip(){
        
        tooltipTrigger = GetComponent<TooltipTrigger>();

        cardInHand = transform.parent.gameObject.GetComponent<CardInHand>();

        index = (int)char.GetNumericValue(name[^1]) - 1;

        tooltipTrigger.header = "";
        tooltipTrigger.content = "";

        if(cardInHand != null){
            if(index < cardInHand.card.sigils.Count){
                if(cardInHand.card.sigils.Count == 1){
                    tooltipTrigger.content = cardInHand.card.sigils[index].description;
                    tooltipTrigger.header = cardInHand.card.sigils[index].name;
                }else if(cardInHand.card.sigils.Count == 3){
                    tooltipTrigger.content = cardInHand.card.sigils[index].description;
                    tooltipTrigger.header = cardInHand.card.sigils[index].name;
                }
            }
            if(cardInHand.card.sigils.Count == 2){
                if(index == 1){
                    tooltipTrigger.content = cardInHand.card.sigils[0].description;
                    tooltipTrigger.header = cardInHand.card.sigils[0].name;
                }else if(index == 2){
                    tooltipTrigger.content = cardInHand.card.sigils[1].description;
                    tooltipTrigger.header = cardInHand.card.sigils[1].name;
                }
            }
        }else{
            cardInCombat = transform.parent.gameObject.GetComponent<CardInCombat>();
            if(index < cardInCombat.card.sigils.Count){
                if(cardInCombat.card.sigils.Count == 1){
                    tooltipTrigger.content = cardInCombat.card.sigils[index].description;
                    tooltipTrigger.header = cardInCombat.card.sigils[index].name;
                }else if(cardInCombat.card.sigils.Count == 3){
                    tooltipTrigger.content = cardInCombat.card.sigils[index].description;
                    tooltipTrigger.header = cardInCombat.card.sigils[index].name;
                }
            }
            if(cardInCombat.card.sigils.Count == 2){
                if(index == 1){
                    tooltipTrigger.content = cardInCombat.card.sigils[0].description;
                    tooltipTrigger.header = cardInCombat.card.sigils[0].name;
                }else if(index == 2){
                    tooltipTrigger.content = cardInCombat.card.sigils[1].description;
                    tooltipTrigger.header = cardInCombat.card.sigils[1].name;
                }
            }
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.tooltipSystem.hoveredIsSigil = true;
        TooltipSystem.tooltipSystem.hoveredSigil = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.tooltipSystem.hoveredIsSigil = false;
        TooltipSystem.tooltipSystem.hoveredSigil = null;
    }

        private void OnDestroy() {
        if(TooltipSystem.tooltipSystem.hoveredSigil != null && TooltipSystem.tooltipSystem.hoveredSigil == this){
            Debug.Log("trigger destroyed");
            TooltipSystem.tooltipSystem.hoveredSigil = null;
            TooltipSystem.Hide();
        }
    }

        
}
