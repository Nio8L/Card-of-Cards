using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Bleed")]
public class Bleed : Sigil
{
    public int count = 0;
    public int turnToDealDamage;
    public int damage;
    public override void PasiveEffect(CardInCombat card)
    {
        count++;
        if (count >= turnToDealDamage)
        {
            Debug.Log("dmged");
            card.card.health -= damage;
            count = 0;
            if (card.card.health <= 0)
            {
                Debug.Log("died From Bleed");
                card.card.lastBattle = null;
            }
            card.deck.UpdateCardAppearance(card.transform, card.card);
        }
    }
}
