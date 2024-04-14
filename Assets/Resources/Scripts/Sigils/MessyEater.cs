using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Messy Eater")]
public class MessyEater : Sigil
{
    public Card flyCard;
    public GameObject flyParticlesPrefab;

    public override void OnHitEffect(CardInCombat card)
    {
        if(card.card.lastBattle.enemyCard.health <= 0){
            Card cardToAdd = Instantiate(flyCard).ResetCard();
            cardToAdd.name = flyCard.name;
            card.deck.drawPile.Add(cardToAdd);

            EventManager.CardCreated?.Invoke(flyCard, card.deck.drawPile, card.deck.playerDeck);
            Instantiate(flyParticlesPrefab, card.GetSlot().transform.position, Quaternion.identity);

            card.deck.PlaySigilAnimation(card.transform, card.card, this);
        }
    }
}
