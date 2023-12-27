using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Consuming Sigil/Endless Hunger")]
public class EndlessHunger : ConsumingSigil
{
    public int healthToHeal;

    public override void OnConsumeEffects(CardInCombat card, Card consumedCard)
    {
        card.card.health += healthToHeal;

        card.deck.UpdateCardAppearance(card.gameObject.transform, card.card);

        cardAcceptor.AcceptCard(consumedCard);
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }
}
