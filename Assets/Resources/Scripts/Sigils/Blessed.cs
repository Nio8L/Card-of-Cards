using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Blessed")]
public class Blessed : Sigil
{
    public int costReduction;
    public int damageBuff;
    public int healthBuff;

    public override void OnDrawEffect(Card card)
    {
        card.cost -= costReduction;
        card.attack += damageBuff;
        card.health += healthBuff;
    }
    
}
