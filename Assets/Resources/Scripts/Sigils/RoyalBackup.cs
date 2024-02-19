using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Royal Backup")]
public class RoyalBackup : Sigil
{
    public Card[] cardsToGive;

    public override void OnDeadEffect(CardInCombat card)
    {
        card.deck.PlaySigilAnimation(card.transform, card.card, this);

        Card chosenCard = cardsToGive[Random.Range(0, cardsToGive.Length)];
        Card cardToAdd = Instantiate(chosenCard).ResetCard();
        cardToAdd.name = chosenCard.name;

        card.deck.drawPile.Add(cardToAdd);

        EventManager.CardCreated?.Invoke(chosenCard, card.deck.drawPile, card.deck.playerDeck);
    }

}
