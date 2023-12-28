using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Scales")]
public class Scales : Sigil
{
    //Checks if the card has been saved from dying from an ignited slot
    bool hasProtected = false;

    public override void OnEndOfTurnEffects(CardInCombat card){
        hasProtected = false;
    }

    public override void PasiveEffect(CardInCombat card)
    {   
        //If it has been saved from dying return, otherwise the card will heal
        if(hasProtected) return;
        
        if(card.playerCard){
            if(card.benched){
                //Player card on the bench
                if(CombatManager.combatManager.playerBenchSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }else{
                //Player card attacking
                if(CombatManager.combatManager.playerCombatSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }
        }else{
            if(card.benched){
                //Enemy card on the bench
                if(CombatManager.combatManager.enemyBenchSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }else{
                //Enemy card attacking
                if(CombatManager.combatManager.enemyCombatSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }
        }

        card.deck.UpdateCardAppearance(card.gameObject.transform, card.card);
    }

    //Used for protecting the card from dying while on an ignited slot. This happens when the card is on 1 HP
    public override void OnDeadEffects(CardInCombat card)
    {   
        //If it has been saved from dying return, otherwise the card will heal
        if(hasProtected) return;
        
        if(card.playerCard){
            if(card.benched){
                //Player card on the bench
                if(CombatManager.combatManager.playerBenchSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }else{
                //Player card attacking
                if(CombatManager.combatManager.playerCombatSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }
        }else{
            if(card.benched){
                //Enemy card on the bench
                if(CombatManager.combatManager.enemyBenchSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }else{
                //Enemy card attacking
                if(CombatManager.combatManager.enemyCombatSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    card.card.health++;
                }
            }
        }

        hasProtected = true;
        card.deck.UpdateCardAppearance(card.gameObject.transform, card.card);
    }
}
