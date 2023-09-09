using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/ThickSkin")]
public class ThickSkin : Sigil
{
    public override void OnTakeDamageEffect(CardInCombat card)
    {
        if (card.card.lastBattle.enemyCard.attack > 0)
        {
            card.card.health++;
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
        }
    }
}
