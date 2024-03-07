using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Short Fuse")]
public class ShortFuse : Sigil
{
    public int normalBonus;
    public int onFireBonus;
    bool onFire = false;
    public override void OnDrawEffect(Card card)
    {
        card.attack += normalBonus;
    }
    public override void OnDiscardEffect(Card card)
    {
        card.attack -= normalBonus;
    }
    public override void OnFightStartEffect(CardInCombat card){
    
        if (!onFire && card.GetSlot().status == CardSlot.Status.Ignited){
            onFire = true;
            card.card.attack += onFireBonus;
            card.UpdateCardAppearance();
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
        }
    }

    public override void OnTurnStartEffect(CardInCombat card)
    {
        if (onFire && card.GetSlot().status != CardSlot.Status.Ignited){
            onFire = false;
            card.card.attack -= onFireBonus;
            card.UpdateCardAppearance();
        }
    }

    public override void OnDeadEffect(CardInCombat card)
    {
        card.card.attack -= normalBonus;
        if (onFire){
            onFire = false;
            card.card.attack -= onFireBonus;
        }
    }
}

