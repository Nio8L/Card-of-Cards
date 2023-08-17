using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Fearless")]
public class Fearless : Sigil
{
    bool canRevive = true;

    public override void OnDeadEffects(CardInCombat card) 
    {
        if (canRevive)
        {
            card.card.ResetHP();
            canRevive = false;
            return;
        }
        canRevive = true;
    }
}
