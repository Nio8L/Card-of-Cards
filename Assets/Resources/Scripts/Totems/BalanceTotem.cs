using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Totem/BalanceTotem")]
public class BalanceTotem : Totem
{
    int counter;
    public int cardsNeededForActivation;
    public int healBy;
    public GameObject passiveParticles;
    public override void Setup()
    {
        EventManager.CardPlayed += Check;
    }
    
    public override void Unsubscribe()
    {
        EventManager.CardPlayed -= Check;
    }

    public override void Passive()
    {
        List<CardInCombat> cards = new List<CardInCombat>();
        
        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            CardInCombat target = CombatManager.combatManager.playerCombatCards[i];
            if (target != null) cards.Add(target);
            target = CombatManager.combatManager.playerBenchCards[i];
            if (target != null) cards.Add(target);
        }

        CardInCombat selectedCard = cards[Mathf.FloorToInt(Random.value * cards.Count)];
        selectedCard.card.health += healBy;
        selectedCard.UpdateCardAppearance();

        Instantiate(passiveParticles, selectedCard.transform.position, Quaternion.identity);
    }

    public override void Active()
    {
        List<CardInCombat> cards = new List<CardInCombat>();
        
        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            CardInCombat target = CombatManager.combatManager.playerCombatCards[i];
            if (target != null) cards.Add(target);
            target = CombatManager.combatManager.playerBenchCards[i];
            if (target != null) cards.Add(target);
        }

        for (int i = 0; i < cards.Count; i++){
            CardInCombat selectedCard = cards[i];
            selectedCard.card.health += healBy;
            selectedCard.UpdateCardAppearance();

            Instantiate(passiveParticles, selectedCard.transform.position, Quaternion.identity);
        }
    }

    void Check(Card card){
        if (card.playerCard) counter++;
        if (counter == cardsNeededForActivation){
            Passive();
            counter = 0;
        }
    }

}
