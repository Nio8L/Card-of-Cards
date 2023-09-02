using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Soulless")]
public class Soulless : Sigil
{
    public override void OnDeadEffects(CardInCombat card) 
    {
        card.card.canRevive = false;
    }
}
