using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Adrenaline")]
public class Adrenaline : Sigil
{
    public override void OnSummonEffects(CardInCombat card)
    {
        card.card.attack += card.card.injuries.Count;
    }

    public override void OnDeadEffects(CardInCombat card) 
    {
        card.card.attack -= card.card.injuries.Count;
    }

    public override void OnBattleEndEffects(CardInCombat card)
    {
        card.card.ResetAttack();
    }
}
