using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Overwhelm")]
public class Overwhelm : Sigil
{
    bool activatedThisTurn = false;
    public override void ApplyOnHitEffect(CardInCombat card)
    {
        if (activatedThisTurn) return;

        int bonusDamage = 0;

        BattleData battle = card.card.lastBattle;

        if (battle.enemyCardOldHp < card.card.attack)
        {
            int damage = Mathf.Abs(battle.enemyCard.health) + bonusDamage;
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            activatedThisTurn = true;

            if (card.playerCard && CombatManager.combatManager.enemyBenchCards[card.slot] != null)
            {
                CardInCombat cardToHit = CombatManager.combatManager.enemyBenchCards[card.slot];
                CombatManager.combatManager.CardCombat1Attacker(card, cardToHit, damage);
            }
            else if (!card.playerCard && CombatManager.combatManager.playerBenchCards[card.slot] != null)
            {
                CardInCombat cardToHit = CombatManager.combatManager.playerBenchCards[card.slot];
                CombatManager.combatManager.CardCombat1Attacker(card, cardToHit, damage);
            }
            else
            {
                CombatManager.combatManager.DirectHit(card, damage);
            }
        }
        
    }

    public override void OnBattleStartEffects(CardInCombat card)
    {
        activatedThisTurn = false;
    }
}
