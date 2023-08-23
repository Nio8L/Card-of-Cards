using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Fearless")]
public class Fearless : Sigil
{
    bool canRevive = true;

    public override void OnTakeDamageEffect(CardInCombat card) 
    {
        if (canRevive && card.card.health <= 0)
        {
            card.card.ResetHP();
            canRevive = false;
            card.deck.UpdateCardAppearance(card.transform, card.card);
            Debug.Log("revived");
        }
    }

    public override void OnDeadEffects(CardInCombat card) 
    {
        if(canRevive && card.card.lastBattle == null)
        {
            card.card.ResetHP();
            canRevive = false;
            card.deck.UpdateCardAppearance(card.transform, card.card);
            Debug.Log("revived From Bleed");
            return;
        }
        canRevive = true;
    }
}
