using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Blessed")]
public class Blessed : Sigil
{
    public int costReduction;
    public int damageBuff;
    public int healthBuff;

    public Sigil fakeSigil;

    public override void OnAcquireEffect(Card card)
    {
        card.cost -= costReduction;
        card.defaultAttack += damageBuff;
        card.defaultHealth += healthBuff;

        string sigilName = this.sigilName;
        Sigil sigil = Instantiate(fakeSigil);
        sigil.sigilName = sigilName;
        sigil.name = fakeSigil.name;

        card.sigils[card.sigils.IndexOf(this)] = sigil;
    }
}
