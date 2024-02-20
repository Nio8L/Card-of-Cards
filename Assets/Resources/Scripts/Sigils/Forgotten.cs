using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Forgotten")]
public class Forgotten : Sigil
{
    public override void OnNotDrawn(Card card)
    {
       CombatManager.combatManager.deck.drawPile.Remove(card);
       CombatManager.combatManager.enemyDeck.drawPile.Remove(card);
    }
}
