using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Blessed")]
public class Blessed : Sigil
{
    public int costReduction;
    public int damageBuff;
    public int healthBuff;
    bool used;

    public override void OnDrawEffect(Card card)
    {
        if (used) return;

        card.cost -= costReduction;
        card.attack += damageBuff;
        card.maxHealth += healthBuff;
        card.health += healthBuff;

        used = true;
    }
    public override void OnBattleEndEffect(Card card)
    {
        if (!used) return;

        card.cost += costReduction;
        card.ResetAttack();
        card.maxHealth -= healthBuff;

        used = false;
    }


}
