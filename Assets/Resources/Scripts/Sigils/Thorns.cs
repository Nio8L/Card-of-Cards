using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Thorns")]
public class Thorns : Sigil
{
    public int damage;
    public GameObject thornsParticlesPrefab;
    public override void OnTakeDamageEffect(CardInCombat card)
    {
        card.card.lastBattle.enemyCard.health -= damage;
        card.card.lastBattle.enemy.lastTypeOfDamage = Card.TypeOfDamage.Scratch;
        Instantiate(thornsParticlesPrefab, card.transform.position, Quaternion.identity);
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }
}
