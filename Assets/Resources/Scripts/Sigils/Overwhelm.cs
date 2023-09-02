using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Overwhelm")]
public class Overwhelm : Sigil
{
    public override void ApplyOnHitEffect(CardInCombat card)
    {
        BattleData battle = card.card.lastBattle;
        if (battle.enemyCard.health < 0)
        {
            card.deck.combatManager.DirectHit(card, Mathf.Abs(battle.enemyCard.health));
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
        }

    }
}
