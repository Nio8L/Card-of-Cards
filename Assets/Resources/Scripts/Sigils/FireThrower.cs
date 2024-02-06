using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Fire Thrower")]
public class FireThrower : Sigil
{
    public override void ApplyOnHitEffect(CardInCombat card)
    {
        if (card.playerCard)
        {
            if(card.GetSlot().status == CardSlot.Status.Ignited){
                if(CombatManager.combatManager.enemyCombatCards[card.slot] != null){
                    CombatManager.combatManager.enemyCombatSlots[card.slot].GetComponent<CardSlot>().IgniteSlot(card.GetSlot().turnsIgnited);
                }else{
                    CombatManager.combatManager.enemyBenchSlots[card.slot].GetComponent<CardSlot>().IgniteSlot(card.GetSlot().turnsIgnited);
                }
                card.deck.PlaySigilAnimation(card.transform, card.card, this);
            }
        }else{
            if(card.GetSlot().status == CardSlot.Status.Ignited){
                if(CombatManager.combatManager.playerCombatCards[card.slot] != null){
                    CombatManager.combatManager.playerCombatSlots[card.slot].GetComponent<CardSlot>().IgniteSlot(card.GetSlot().turnsIgnited);
                }else{
                    CombatManager.combatManager.playerBenchSlots[card.slot].GetComponent<CardSlot>().IgniteSlot(card.GetSlot().turnsIgnited);
                }
                card.deck.PlaySigilAnimation(card.transform, card.card, this);
            }
        }
    }
}
