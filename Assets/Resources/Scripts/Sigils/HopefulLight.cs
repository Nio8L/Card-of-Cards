using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Hopeful Light")]
public class HopefulLight : Sigil
{
    public int energyPerTurn = 1;
    public override void OnTurnStartEffect(CardInCombat card)
    {
        card.deck.energy += energyPerTurn;
    }
}
