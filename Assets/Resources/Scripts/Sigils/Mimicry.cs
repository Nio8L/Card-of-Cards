using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Mimicry")]
public class Mimicry : Sigil
{
    bool playerCard;

    public override void OnSummonEffects(CardInCombat card)
    {
        EventManager.CardCreated += CopyCard;
        playerCard = card.deck.playerDeck;
    }

    public override void OnDeadEffects(CardInCombat card) {
        EventManager.CardCreated -= CopyCard;
    }

    public override void OnBattleEndEffects(CardInCombat card)
    {
        EventManager.CardCreated -= CopyCard;
    }

    public void CopyCard(Card cardToCopy, List<Card> listToAdd, bool playerDeck){
        if(playerCard == playerDeck){
            Card copiedCard = Instantiate(cardToCopy).ResetCard();
            copiedCard.name = cardToCopy.name;

            listToAdd.Add(copiedCard);
        }
    }
}
