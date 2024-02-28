using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Loyal")]
public class Loyal : Sigil
{
    public override void OnNotDrawn(Card card)
    {
        if(card.playerCard){
            CombatManager.combatManager.deck.DrawCard(card);
        }else{
            CombatManager.combatManager.enemyDeck.DrawCard(card);
        }
    }
}
