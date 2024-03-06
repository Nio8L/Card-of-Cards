using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Scales")]
public class Scales : Sigil
{
    public override void OnSummonEffect(CardInCombat card)
    {
        // Give this card fire immunity
        card.card.fireImmune = true;
    }
    public override void OnTurnStartEffect(CardInCombat card)
    {   
        // Check if the card is standing on a fire and heal it if it is
        if(card.playerCard){
            if(card.benched){
                //Player card on the bench
                if(CombatManager.combatManager.playerBenchSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    Heal(card);
                }
            }else{
                //Player card attacking
                if(CombatManager.combatManager.playerCombatSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    Heal(card);
                }
            }
        }else{
            if(card.benched){
                //Enemy card on the bench
                if(CombatManager.combatManager.enemyBenchSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    Heal(card);
                }
            }else{
                //Enemy card attacking
                if(CombatManager.combatManager.enemyCombatSlots[card.slot].GetComponent<CardSlot>().status == CardSlot.Status.Ignited){
                    Heal(card);
                }
            }
        }
        card.UpdateCardAppearance();
    }
    void Heal(CardInCombat card){
        card.card.health += 1;
        if (card.card.health > card.card.maxHealth){
            card.card.health = card.card.maxHealth;
        }

        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }
}
