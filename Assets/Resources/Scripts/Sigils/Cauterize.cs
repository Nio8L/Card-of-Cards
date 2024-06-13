using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Active Sigil/Cauterize")]
public class Cauterize : ActiveSigil
{
    public int heal;
    public int turnsToIgnite;

    public override void OnSummonEffect(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void ActiveEffect(CardInCombat card)
    {
        card.card.health += heal;
        
        card.GetSlot().IgniteSlot(turnsToIgnite);

        canBeUsed = false;
    }
}
