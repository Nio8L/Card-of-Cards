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

        if(battleData.enemyCard.health > card.card.health){
            battleData.enemyCard.health -= bonusDamage;
        }
    }
}
