using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Rekindle")]
public class Rekindle : Sigil
{
    public Card emberCardBase;
    public int limit;
    public int firesPerCard;
    public override void OnTurnStartEffect(CardInCombat card)
    {
        int fire = 0;
        for (int i = 0; i < CombatManager.combatManager.playerBenchSlots.Length; i++){
            if (CombatManager.combatManager.playerBenchSlots [i].GetComponent<CardSlot>().status == CardSlot.Status.Ignited) fire++;
            if (CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>().status == CardSlot.Status.Ignited) fire++;
            if (CombatManager.combatManager.enemyBenchSlots  [i].GetComponent<CardSlot>().status == CardSlot.Status.Ignited) fire++;
            if (CombatManager.combatManager.enemyCombatSlots [i].GetComponent<CardSlot>().status == CardSlot.Status.Ignited) fire++;
        }
        int cards = fire/firesPerCard;
        if (cards > limit) cards = limit;
        for (int i = 0; i < cards; i++){
            Card cardToAdd = Instantiate(emberCardBase).ResetCard();
            string nameToCopy = emberCardBase.name;
            cardToAdd.name = nameToCopy;
            card.deck.discardPile.Add(cardToAdd);
        }
        
        if (cards > 0) card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }

}
