using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Fearless")]
public class Fearless : Sigil
{
    bool canRevive = true;
    public GameObject visualEffect;
    public override void OnTakeDamageEffect(CardInCombat card) 
    {
        if (canRevive && card.card.health <= 0)
        {
            card.card.ResetHP();
            canRevive = false;
            GameObject effect = Instantiate(visualEffect, card.transform.position, Quaternion.identity);
            effect.transform.SetParent(card.transform);
            SoundManager.soundManager.Play("Fearless");
        }
    }

    public override void OnDeadEffects(CardInCombat card) 
    {
        if(canRevive)
        {
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            card.card.ResetHP();
            card.deck.UpdateCardAppearance(card.transform, card.card);
            canRevive = false;
            return;
        }
        canRevive = true;
    }
}
