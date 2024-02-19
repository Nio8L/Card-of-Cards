using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Adrenaline")]
public class Adrenaline : Sigil
{
    public override void OnDrawEffect(Card card)
    {
        card.attack += card.injuries.Count;
    }

    public override void OnDiscardEffect(Card card)
    {
        card.attack -= card.injuries.Count;
    }

    public override void OnDeadEffect(CardInCombat card) 
    {
        card.card.attack -= card.card.injuries.Count;
    }

    public override void OnBattleEndEffect(CardInCombat card)
    {
        card.card.ResetAttack();
    }
}
