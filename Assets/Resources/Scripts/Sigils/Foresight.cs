using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Foresight")]
public class Foresight : Sigil
{
    public int numberOfCardsToDraw;
    public GameObject visualEffect;
    public override void PasiveEffect(CardInCombat card)
    {
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
        Instantiate(visualEffect, card.transform.position, Quaternion.identity);
        if (card.playerCard)         card.deck.DrawCard(numberOfCardsToDraw);
        else card.deck.combatManager.enemyDeck.DrawCard(numberOfCardsToDraw);
    }
}
