using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil/Discard")]
public class Discard : ActiveSigil
{
    public override void OnSummonEffects(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void PasiveEffect(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void ActiveEffect(CardInCombat card, List<CardInHand> targets)
    {
        card.deck.DiscardCard(targets[0]);
        canBeUsed = false;
    }

    public override List<CardInHand> GetCardInHandTargets(CardInCombat card)
    {
        List<CardInHand> targets = new List<CardInHand>();

        for(int i = 0; i < card.deck.cardsInHand.Count; i++){
            targets.Add(card.deck.cardsInHand[i].GetComponent<CardInHand>());
        }

        return targets;
    }
}
