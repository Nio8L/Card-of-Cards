using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Split")]
public class Split : Sigil
{
    public Card splittedCard;

    public int coppiesToAdd = 2;

    public override void OnDeadEffects(CardInCombat card)
    {
        for(int i = 0; i < coppiesToAdd; i++){
            Card cardToAdd = Instantiate(splittedCard).ResetCard();
            cardToAdd.name = splittedCard.name;

            card.deck.drawPile.Add(cardToAdd);
        }

        //card.deck.DrawCard(coppiesToAdd);
    }

}
