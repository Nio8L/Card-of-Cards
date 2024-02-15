using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Mimicry")]
public class Mimicry : Sigil
{
    public CardCreator cardCreator;
    public override void OnSummonEffects(CardInCombat card)
    {
        EventManager.CardCreated += CopyCard;
        cardCreator = card.AddComponent<CardCreator>();
        cardCreator.playerCard = card.deck.playerDeck;
    }

    public override void OnDeadEffects(CardInCombat card) {
        EventManager.CardCreated -= CopyCard;
    }

    public override void OnBattleEndEffects(CardInCombat card)
    {
        EventManager.CardCreated -= CopyCard;
    }

    public void CopyCard(Card cardToCopy, List<Card> listToAdd, bool playerDeck){
        if(cardCreator.playerCard == playerDeck){
            Card copiedCard = Instantiate(cardToCopy).ResetCard();
            copiedCard.name = cardToCopy.name;

            listToAdd.Add(copiedCard);
        }
    }
}
