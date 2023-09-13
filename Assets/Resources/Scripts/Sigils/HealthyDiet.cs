using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/HealthyDiet")]
public class HealthyDiet : Sigil
{
    public GameObject particles;
    public int count = 0;
    public int turnToHeal;
    public int heal;
    public override void PasiveEffect(CardInCombat card)
    {
        if (!card.benched) return;

        count++;
        if (count >= turnToHeal && card.card.health < card.card.maxHealth)
        {
            Instantiate(particles, card.transform.position, Quaternion.identity);
            card.deck.PlaySigilAnimation(card.transform, card.card, this);
            card.card.health += heal;
            count = 0;
        }
        
    }
}
