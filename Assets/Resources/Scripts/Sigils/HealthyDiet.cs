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
        int bonusHeal = 0;
        count++;
        if (count >= turnToHeal)
        {
            count = 0;
            if (card.card.health + heal + bonusHeal <= card.card.maxHealth){
                Instantiate(particles, card.transform.position, Quaternion.identity);
                card.deck.PlaySigilAnimation(card.transform, card.card, this);
                card.card.health += heal;
                
            }else if (card.card.health < card.card.maxHealth){
                Instantiate(particles, card.transform.position, Quaternion.identity);
                card.deck.PlaySigilAnimation(card.transform, card.card, this);
                card.card.health = card.card.maxHealth;
            }
        }
    }
}
