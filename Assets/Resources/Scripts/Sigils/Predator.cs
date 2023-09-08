using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Predator")]
public class Predator : Sigil
{
    public override void ApplyOnHitEffect(CardInCombat card)
    {
        BattleData battle = card.card.lastBattle;
        if (battle.enemyCard.health <= 0 && card.card.health > 0)
        {
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            card.card.health += 2;
            if (card.card.health > card.card.maxHealth) card.card.health = card.card.maxHealth;
        }

    }
}
