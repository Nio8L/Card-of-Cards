using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Overwhelm")]
public class Overwhelm : Sigil
{
    public override void ApplyOnHitEffect(CardInCombat card)
    {
        BattleData battle = card.card.lastBattle;
        int damage = Mathf.Abs(battle.enemyCard.health);
        if (battle.enemyCardOldHp < card.card.attack)
        {
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            if (card.playerCard && card.deck.combatManager.enemyBenchCards[card.slot] != null)
            {
                CardInCombat cardToHit = card.deck.combatManager.enemyBenchCards[card.slot];
                cardToHit.card.health -= damage;
                cardToHit.lastTypeOfDamage = card.card.typeOfDamage;
                
            }
            else if (!card.playerCard && card.deck.combatManager.playerBenchCards[card.slot] != null)
            {
                CardInCombat cardToHit = card.deck.combatManager.playerBenchCards[card.slot];
                cardToHit.card.health -= damage;
                cardToHit.lastTypeOfDamage = card.card.typeOfDamage;
            }
            else
            {
                card.deck.combatManager.DirectHit(card, damage);
            }
        }
    }
}
