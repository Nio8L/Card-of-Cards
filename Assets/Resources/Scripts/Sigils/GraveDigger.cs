using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Grave Digger")]
public class GraveDigger : Sigil
{
    public int numberOfCardsToDraw = 0;
    public bool playerCard = false;

    public override void OnSummonEffect(CardInCombat card)
    {
        EventManager.CardDeath += OnCardDeath;
        playerCard = card.card.playerCard;
    }

    public override void OnDeadEffect(CardInCombat card)
    {
        EventManager.CardDeath -= OnCardDeath;
    }

    public override void OnBattleEndEffect(Card card)
    {
        EventManager.CardDeath -= OnCardDeath;
    }

    public override void OnTurnStartEffect(CardInCombat card)
    {
        if (numberOfCardsToDraw > 0){
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            if (card.playerCard)         card.deck.DrawCard(numberOfCardsToDraw);
            else CombatManager.combatManager.enemyDeck.DrawCard(numberOfCardsToDraw);
        }
        numberOfCardsToDraw = 0;
        
    }

    private void OnCardDeath(Card card)
    {
        if(card.playerCard == playerCard){
            numberOfCardsToDraw++;
        }
    }
}
