using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Totem/LostTotem")]
public class LostTotem : Totem
{
    public Card lostSoul;
    public override void Setup()
    {
        EventManager.CardDrawn += Check;
    }

    public override void Unsubscribe()
    {
        EventManager.CardDrawn -= Check;
    }

    public override void Passive()
    {
        CombatManager.combatManager.deck.DrawCard();
    }

    public override void Active()
    {
        String cardName = lostSoul.name;
        lostSoul = Instantiate(lostSoul).ResetCard();
        lostSoul.name = cardName;
        CombatManager.combatManager.deck.cards.Add(lostSoul);
        CombatManager.combatManager.deck.drawPile.Add(lostSoul);
         CombatManager.combatManager.deck.DrawCard(lostSoul);
    }

    void Check(Card card){
        if (card.playerCard && card.name == lostSoul.name) Passive();
    }
}
