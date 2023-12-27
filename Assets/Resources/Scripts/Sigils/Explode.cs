using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Sigil/Consuming Sigil/Explode")]
public class Explode : ConsumingSigil
{
    public int damageIncrement;

    public int damageToDeal = 0;

    public bool splashDamage = false;

    public override void OnConsumeEffects(CardInCombat card, Card consumedCard)
    {
        damageToDeal += damageIncrement;

        description = "Upon death deal " + damageToDeal + " damage. Increase this damage by feeding cards.";

        cardAcceptor.AcceptCard(consumedCard);
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }

    public override void OnDeadEffects(CardInCombat card)
    {
        card.deck.drawPile.AddRange(cardAcceptor.cardsAccepted);

        if(!splashDamage){
            card.card.lastBattle.enemyCard.health -= damageToDeal;
        }else{
            //Implement splash damage after the combat manager is turned into a proper manager.
        }

        damageToDeal = 0;

        description = "Upon death deal " + damageToDeal + " damage. Increase this damage by feeding cards.";
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }
}
