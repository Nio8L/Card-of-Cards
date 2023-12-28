using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/SecondSkin")]
public class SecondSkin : Sigil
{
    bool hasASkin = true;
    public Card cardToSpawn;
    public override void OnDeadEffects(CardInCombat card) 
    {
        // Make it so this effect can be used the next time this card is played
        hasASkin = true;
    }

    public override void OnBattleStartEffects(CardInCombat card)
    {
        // Return if this sigil has already been used 
        if (!hasASkin) return;

        // Return if there isn't an enemy, or if this card won't take direct damage
        if (    card.playerCard
            && (CombatManager.combatManager.enemyCombatCards[card.slot] == null
            ||  CombatManager.combatManager.enemyCombatCards[card.slot].card.attack <= 0)) return;
        else if (!card.playerCard
            && (CombatManager.combatManager.playerCombatCards[card.slot] == null
            || CombatManager.combatManager.playerCombatCards[card.slot].card.attack <= 0)) return;

        // Find the slot to play the card at
        CardSlot slotToUse;
        if   (card.playerCard) slotToUse = CombatManager.combatManager.playerCombatSlots[card.slot].GetComponent<CardSlot>();
        else                   slotToUse = CombatManager.combatManager.enemyCombatSlots [card.slot].GetComponent<CardSlot>();

        // Instantiate the card to prevent the game from overriding the original scriptable object
        Card newCard = Instantiate(cardToSpawn).ResetCard();
        newCard.name = cardToSpawn.name;

        // Play the card
        if (card.playerCard && CombatManager.combatManager.playerBenchCards[card.slot] == null)
        {
            hasASkin = false;
            card.BenchOrUnbench(card.playerCard);
            CombatManager.combatManager.PlayCard(newCard, slotToUse, false);
        }
        else if (!card.playerCard && CombatManager.combatManager.enemyBenchCards[card.slot] == null)
        {
            hasASkin = false;
            card.BenchOrUnbench(card.playerCard);
            CombatManager.combatManager.PlayCard(newCard, slotToUse, false);
        }

    }
}
