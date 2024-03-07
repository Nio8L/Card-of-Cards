using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Consuming Sigil/Endless Hunger")]
public class EndlessHunger : ConsumingSigil
{
    public int healthToHeal;

    public override void OnConsumeEffect(CardInCombat card, Card consumedCard)
    {
        base.OnConsumeEffect(card, consumedCard);
        card.card.health += healthToHeal;

        card.UpdateCardAppearance();

        cardAcceptor.AcceptCard(consumedCard);
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }
}
