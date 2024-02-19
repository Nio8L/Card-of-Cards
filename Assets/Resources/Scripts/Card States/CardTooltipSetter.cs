using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardTooltipSetter : MonoBehaviour, IPointerEnterHandler
{
    public Card card;

    public GameObject damageType;

    public GameObject heart;

    private void Start() {
        if(gameObject.name == "CardDisplay(Clone)"){
            card = gameObject.GetComponent<CardDisplay>().card;
        }else if(gameObject.name == "CardInHand(Clone)"){
            card = gameObject.GetComponent<CardInHand>().card;
        }else{
            card = gameObject.GetComponent<CardInCombat>().card;
        }

        damageType.GetComponent<TooltipTrigger>().content = card.typeOfDamage.ToString();
    }

    public void UpdateHeartTooltip(){
        if(card.name == "Lost Soul"){
            heart.GetComponent<TooltipTrigger>().content = "A soul can't be injured...";
            return;
        }
        
        if(card.injuries.Count > 0){
            string tooltipText = "";
            
            foreach (Card.TypeOfDamage injury in card.injuries)
            {
                tooltipText += injury.ToString() + "\n";
            }

            heart.GetComponent<TooltipTrigger>().content = tooltipText;
        }else{
            heart.GetComponent<TooltipTrigger>().content = "This card has no injuries.";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateHeartTooltip();
    }
}
