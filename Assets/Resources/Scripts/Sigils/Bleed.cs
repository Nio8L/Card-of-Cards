using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Bleed")]
public class Bleed : Sigil
{
    int count = 0;
    public int turnToDealDamage;
    public int damage;
    public override void PasiveEffect(CardInCombat card)
    {
        count++;
        if (count >= turnToDealDamage)
        {
            card.card.health -= damage;
        }
        
    }
}
