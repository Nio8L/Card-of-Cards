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
        if(card.card.health + heal <= card.card.maxHealth){
            card.card.health += heal;
        }else if(card.card.maxHealth > card.card.health){
            card.card.health = card.card.maxHealth;
        }
        
        card.GetSlot().IgniteSlot(turnsToIgnite);

        canBeUsed = false;
    }
}
