using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Mimicry")]
public class Mimicry : Sigil
{
    public override void OnSummonEffects(CardInCombat card)
    {
        EventManager.CardCreated += CopyCard;
    }

    public override void OnDeadEffects(CardInCombat card) {
        EventManager.CardCreated -= CopyCard;
    }

    public override void OnBattleEndEffects(CardInCombat card)
    {
        EventManager.CardCreated -= CopyCard;
    }

    public void CopyCard(Card cardToCopy, List<Card> listToAdd){
        Card copiedCard = Instantiate(cardToCopy).ResetCard();
        copiedCard.name = cardToCopy.name;

        listToAdd.Add(copiedCard);
    }
}
