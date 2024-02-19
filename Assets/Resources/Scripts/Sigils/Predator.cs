using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Predator")]
public class Predator : Sigil
{
    public GameObject particles;
    public override void OnHitEffect(CardInCombat card)
    {
        BattleData battle = card.card.lastBattle;
        if (battle.enemyCard.health <= 0 && card.card.health > 0)
        {
            Instantiate(particles, card.transform.position, Quaternion.identity);
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            card.card.health += 2;
            if (card.card.health > card.card.maxHealth) card.card.health = card.card.maxHealth;
        }

    }
}
