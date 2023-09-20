using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTooltipSetter : MonoBehaviour
{
    public Card card;

    public GameObject damageType;

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
}
