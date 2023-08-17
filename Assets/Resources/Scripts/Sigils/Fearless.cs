using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Fearless")]
public class Fearless : Sigil
{
    public override void OnSummonEffects(CardInCombat card) 
    {
        card.canBeBenched = false;
    }
}
