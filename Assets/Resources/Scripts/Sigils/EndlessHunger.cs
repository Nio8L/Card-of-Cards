using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Endless Hunger")]
public class EndlessHunger : Sigil
{
    CardInCombat cardInCombat;

    public int healthToHeal;

    public override void OnSummonEffects(CardInCombat card)
    {
        CardAcceptor cardAcceptor = card.gameObject.AddComponent<CardAcceptor>();
        cardAcceptor.sigil = this;

        cardInCombat = card;
    }

    public void Feed(Card card){ 
        cardInCombat.card.health += healthToHeal;
        
        cardInCombat.deck.UpdateCardAppearance(cardInCombat.gameObject.transform, cardInCombat.card);
    }
}
