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
            description = "When this card takes lethal damage it is returned to its max health. (Used)";
        }
    }

    public override void OnDeadEffect(CardInCombat card) 
    {
        if(canRevive)
        {
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            card.card.ResetHP();
            card.UpdateCardAppearance();
            canRevive = false;
            return;
        }

        canRevive = true;
        description = "When this card takes lethal damage it is returned to its max health. (Unused)";
    }
}
