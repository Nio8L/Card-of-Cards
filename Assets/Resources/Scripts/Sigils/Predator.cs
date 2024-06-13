using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Predator")]
public class Predator : Sigil
{
    public int heal;
    public GameObject particles;
    public override void OnHitEffect(CardInCombat card)
    {
        BattleData battle = card.card.lastBattle;
        if (battle.enemyCard.health <= 0 && card.card.health > 0)
        {
            Instantiate(particles, card.transform.position, Quaternion.identity);
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            card.card.health += heal;
        }

    }
}
