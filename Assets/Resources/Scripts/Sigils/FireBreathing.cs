using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Fire Breathing")]
public class FireBreathing : Sigil
{
    public int ignitionDuration = 1;

    public override void OnHitEffect(CardInCombat card)
    {
        BattleData battleData = card.card.lastBattle;
        
        if(battleData.enemyCard.health <= 0){
            if (card.playerCard){
                CombatManager.combatManager.enemyCombatSlots[card.slot].GetComponent<CardSlot>().IgniteSlot(ignitionDuration);
                CombatManager.combatManager.enemyBenchSlots[card.slot] .GetComponent<CardSlot>().IgniteSlot(ignitionDuration);
            }else{
                CombatManager.combatManager.playerCombatSlots[card.slot].GetComponent<CardSlot>().IgniteSlot(ignitionDuration);
                CombatManager.combatManager.playerBenchSlots[card.slot] .GetComponent<CardSlot>().IgniteSlot(ignitionDuration);
            }
        card.deck.PlaySigilAnimation(card.transform, card.card, this);

        SoundManager.soundManager.Play("FireBreathing");

        }
    }
}

