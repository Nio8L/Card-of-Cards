using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Overwhelm")]
public class Overwhelm : Sigil
{
    public override void ApplyOnHitEffect(CardInCombat card)
    {
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
        BattleData battle = card.card.lastBattle;
        int damage = Mathf.Abs(battle.enemyCard.health);
        if (battle.enemyCard.health < 0)
        {
            if (card.playerCard && card.deck.combatManager.enemyBenchCards[card.slot] != null)
            {
                card.deck.combatManager.enemyBenchCards[card.slot].card.health -= damage;
            }
            else if (!card.playerCard && card.deck.combatManager.playerBenchCards[card.slot] != null)
            {
                card.deck.combatManager.playerBenchCards[card.slot].card.health -= damage;
            }
            else
            {
                card.deck.combatManager.DirectHit(card, damage);
            }
        }
    }
}
