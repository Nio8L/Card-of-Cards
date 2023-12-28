using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/FireBreathing")]
public class FireBreathing : Sigil
{
    public int ignitionDuration = 1;

    public override void ApplyOnHitEffect(CardInCombat card)
    {
        BattleData battleData = card.card.lastBattle;

        if(battleData.enemyCardOldHp <= card.card.attack){
            CombatManager.combatManager.enemyCombatSlots[card.slot].GetComponent<CardSlot>().IgniteSlot(ignitionDuration);
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
        }
    }
}
