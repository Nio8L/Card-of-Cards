using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Totem/LostTotem")]
public class LostTotem : Totem
{
    public Card lostSoulBase;
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
        Card lostSoul;
        lostSoul = Instantiate(lostSoulBase).ResetCard();
        lostSoul.name = lostSoulBase.name;
        CombatManager.combatManager.deck.cards.Add(lostSoul);
        CombatManager.combatManager.deck.drawPile.Add(lostSoul);
        CombatManager.combatManager.deck.DrawCard(lostSoul);
    }

    void Check(Card card){
        if (card.playerCard && card.name == lostSoulBase.name) Passive();
    }
}
