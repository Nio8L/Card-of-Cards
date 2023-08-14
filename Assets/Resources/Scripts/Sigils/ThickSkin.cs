using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/ThickSkin")]
public class ThickSkin : Sigil
{
    public override void OnTakeDamageEffect(CardInCombat card)
    {
        card.card.health++;
    }
}
