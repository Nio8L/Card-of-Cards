using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/HealthyDiet")]
public class HealthyDiet : Sigil
{
    public int count = 0;
    public int turnToHeal;
    public int heal;
    public override void PasiveEffect(CardInCombat card)
    {
        count++;
        if (count >= turnToHeal && card.card.health < card.card.maxHealth)
        {
            card.card.health += heal;
            count = 0;
        }
        
    }
}
