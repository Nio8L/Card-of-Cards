using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil")]
public class ActiveSigil : Sigil
{
    [Header("Active Sigil")]
    public bool canBeUsed;
    public int neededTargets;

    public enum TargetType
    {
        Slot,
        Hand
    }

    public TargetType targetType;

    public virtual void ActiveEffect(CardInCombat card, List<CardSlot> targets) 
    {
        /*
            CardInCombat card is the card object this sigil is attached to
            CardSlot[] targets are the slots where this active effect will take place (found using GetPossibleTargets())
        */
        //Active effect should be here
    }

    public virtual void ActivateEffect(CardInCombat card, List<CardInHand> targets){
        /*
            CardInCombat card is the card object this sigil is attached to
            CardInHand[] targets are the cards in hand which this active effect will affect (found using GetCardInHandTargets())
        */
        //Active effect should be here
    }

    public virtual List<CardSlot> GetPossibleTargets(CardInCombat card) 
    {
        /*
            This should return the slots at which this ability is castable
            If no targets are required just ignore this
        */
        return new List<CardSlot>();
    }

    public virtual List<CardInHand> GetCardInHandTargets(CardInCombat card){
        /*
            This should return the cards in hand on which this ability is castable
            If no targets are required just ignore this
        */
        
        return new List<CardInHand>();
    }

    public override ActiveSigil GetActiveSigil(){
        /*
            Return this ActiveSigil so there is a way to get a reference to this
        */
        return this;
    }

}
