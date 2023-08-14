using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Overwhelm")]
public class Overwhelm : Sigil
{
    public override void ApplyOnHitEffect(CardInCombat card)
    {
        card.deck.combatManager.DirectHit(card);
    }
}
