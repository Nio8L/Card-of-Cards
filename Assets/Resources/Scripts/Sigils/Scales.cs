using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Scales")]
public class Scales : Sigil
{
    public GameObject particles;
    public override void OnSummonEffect(CardInCombat card)
    {
        // Give this card fire immunity
        card.card.fireImmune = true;
    }
    public override void OnTurnStartEffect(CardInCombat card)
    {   
        // Check if the card is standing on a fire and heal it if it is
        if (card.GetSlot().status == CardSlot.Status.Ignited) Heal(card);
    }
    void Heal(CardInCombat card){
        card.card.health += 1;
        if (card.card.health > card.card.maxHealth){
            card.card.health = card.card.maxHealth;
        }
        Instantiate(particles, card.transform.position, Quaternion.identity);
        card.UpdateCardAppearance();

        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }
}
