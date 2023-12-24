using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SigilTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    private TooltipTrigger tooltipTrigger;
    
    private int index;
    private CardInHand cardInHand;
    private CardInCombat cardInCombat;
    private CardDisplay cardDisplay;

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
                    tooltipTrigger.header = cardInHand.card.sigils[index].sigilName;
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
            if(cardInCombat != null){
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
            }else{
                cardDisplay = transform.parent.gameObject.GetComponent<CardDisplay>();
                if(index < cardDisplay.card.sigils.Count){
                    if(cardDisplay.card.sigils.Count == 1){
                        tooltipTrigger.content = cardDisplay.card.sigils[index].description;
                        tooltipTrigger.header = cardDisplay.card.sigils[index].name;
                    }else if(cardDisplay.card.sigils.Count == 3){
                        tooltipTrigger.content = cardDisplay.card.sigils[index].description;
                        tooltipTrigger.header = cardDisplay.card.sigils[index].name;
                    }
                }
                if(cardDisplay.card.sigils.Count == 2){
                    if(index == 1){
                        tooltipTrigger.content = cardDisplay.card.sigils[0].description;
                        tooltipTrigger.header = cardDisplay.card.sigils[0].name;
                    }else if(index == 2){
                        tooltipTrigger.content = cardDisplay.card.sigils[1].description;
                        tooltipTrigger.header = cardDisplay.card.sigils[1].name;
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateSigilTooltip();
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
            TooltipSystem.tooltipSystem.hoveredSigil = null;
            TooltipSystem.Hide();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().raycastTarget = false;
        if(cardInHand != null){
            cardInHand.OnDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().raycastTarget = true;
        if(cardInHand != null){
            cardInHand.OnStopDrag();
        }
    }
}
