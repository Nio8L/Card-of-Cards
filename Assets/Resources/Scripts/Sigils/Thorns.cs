using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Thorns")]
public class Thorns : Sigil
{
    public int damage;
    public override void OnTakeDamageEffect(CardInCombat card)
    {
        card.card.lastBattle.enemyCard.health -= damage;
    }
}
