using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Wall Breaker")]
public class WallBreaker : Sigil
{
   public int bonusDamage;

    public override void OnHitEffect(CardInCombat card)
    {
        BattleData battleData = card.card.lastBattle;
        if (battleData.enemy.benched) return;

        if(battleData.enemyCard.maxHealth > card.card.maxHealth){
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            battleData.enemyCard.health -= bonusDamage;
            if (card.playerCard){
                if (CombatManager.combatManager.enemyBenchCards[card.slot] != null){
                    CombatManager.combatManager.CardCombat1Attacker(card, CombatManager.combatManager.enemyBenchCards[card.slot], 1);
                }
            }else{
                if (CombatManager.combatManager.playerBenchCards[card.slot] != null){
                    CombatManager.combatManager.CardCombat1Attacker(card, CombatManager.combatManager.playerBenchCards[card.slot], 1);
                }
            }
        }
    }
}
