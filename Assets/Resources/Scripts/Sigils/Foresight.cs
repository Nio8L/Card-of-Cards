using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Foresight")]
public class Foresight : Sigil
{
    public int numberOfCardsToDraw;
    public override void PasiveEffect(CardInCombat card)
    {
        card.deck.DrawCard(numberOfCardsToDraw);
    }
}
