using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Empower")]
public class Empower : Sigil
{
    public GameObject visualEffect;
    public override void OnBattleEndEffect(Card card)
    {
        card.ResetAttack();
    }

    public override void OnHitEffect(CardInCombat card)
    {
        BattleData battle = card.card.lastBattle;

        if (battle.enemyCard.health <= 0)
        {
            card.deck.PlaySigilAnimation(card.transform, card.card, this);

            card.card.attack++;

            GameObject effect = Instantiate(visualEffect, card.transform.position, Quaternion.identity);
            effect.transform.SetParent(card.transform);
            SoundManager.soundManager.Play("Fearless");
        }
        
    }
}
