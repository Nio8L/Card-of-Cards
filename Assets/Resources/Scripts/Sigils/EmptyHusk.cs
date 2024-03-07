using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Consuming Sigil/Empty Husk")]
public class EmptyHusk : ConsumingSigil
{
    public override void OnConsumeEffect(CardInCombat card, Card consumedCard)
    {
        base.OnConsumeEffect(card, consumedCard);
        Card cardCopy = Instantiate(consumedCard).ResetCard();

        card.card.health = cardCopy.health;
        card.card.attack = cardCopy.attack;
        card.card.injuries = cardCopy.injuries;
        card.card.typeOfDamage = cardCopy.typeOfDamage;

        card.UpdateCardAppearance();
        
        card.deck.discardPile.Add(consumedCard);
        CombatManager.combatManager.combatUI.UpdatePileNumbers();
    }

    public override void OnDrawEffect(Card card)
    {
        card.ResetCard();
    }
}
