using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Bleed")]
public class Bleed : Sigil
{
    public int count = 0;
    public int turnToDealDamage;
    public int damage;

    public GameObject bloodSplatParticles;
    public override void PasiveEffect(CardInCombat card)
    {
        count++;
        if (count >= turnToDealDamage)
        {
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            card.card.health -= damage;
            count = 0;
            Instantiate(bloodSplatParticles, card.transform.position, Quaternion.identity);
            if (card.card.health <= 0)
            {
                //Debug.Log("died From Bleed");
                card.card.lastBattle = null;
            }
            card.deck.UpdateCardAppearance(card.transform, card.card);
        }

        if(turnToDealDamage - count == 1){
            description = "Lose 1 health after " + (turnToDealDamage - count) + " turn.";
        }else{
            description = "Lose 1 health after " + (turnToDealDamage - count) + " turns.";
        }
    }
    public override void OnDeadEffects(CardInCombat card)
    {
        count = 0;
        description = "Lose 1 health after " + (turnToDealDamage - count) + " turns.";
    }
}
