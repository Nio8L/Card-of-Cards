using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Messy Eater")]
public class MessyEater : Sigil
{
    public Card flyCard;

    public override void OnHitEffect(CardInCombat card)
    {
        if(card.card.lastBattle.enemyCard.health <= 0){
            Card cardToAdd = Instantiate(flyCard).ResetCard();
            cardToAdd.name = flyCard.name;
            card.deck.drawPile.Add(cardToAdd);

            EventManager.CardCreated?.Invoke(flyCard, card.deck.drawPile, card.deck.playerDeck);
        }
    }
}
