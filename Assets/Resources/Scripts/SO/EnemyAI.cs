using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI")]
public class EnemyAI : ScriptableObject
{
    public int maxHealth;

    public List<Card> cards = new();

    public int startPlayingDefensivelyAt;
    public int startPlayingAggressivelyAt;
    //public bool canBench;
    //public bool tryToPredictBench;
    public bool useKillerStrategyInstead;

    public int bias = 0;
    public int thinkLimit;

    public Strategy currentStrategy;
    public enum Strategy
    {
        Aggressive,
        Killer,
        Defensive,
        Random
    }

    CombatManager combatManager;
    public void Initialize()
    {
        Debug.Log("Initializing enemy ai");
        combatManager = GameObject.Find("Deck").GetComponent<CombatManager>();
        combatManager.enemyHealth = maxHealth;
        foreach (Card card in cards)
        {
            Card cardToAdd = Instantiate(card).ResetCard();
            cardToAdd.name = card.name;
            combatManager.enemyDeck.AddCard(cardToAdd);
            combatManager.enemyDeck.Shuffle();
        }
    }
    public void StartTurn()
    {
        thinkLimit = 5;
        Think();
    }
    void Think()
    {
        currentStrategy = PickStrategy();
        Card card = PickCard();
        bool hasPlay = false;
        int targetIndex = 0;

        // Pick slot
        if (currentStrategy == Strategy.Defensive)
        {
            bool[] possibleSlots = new bool[3];

            int targetDamage = 0;
            
            for (int i = 0; i < 3; i++)
            {
                if (combatManager.playerCards[i] != null && combatManager.enemyCards[i] == null)
                {
                    hasPlay = true;
                    possibleSlots[i] = true;
                    if (combatManager.playerCards[i].card.attack > targetDamage)
                    {
                        targetDamage = combatManager.playerCards[i].card.attack;
                        targetIndex = i;
                    }
                }
            }

            if (!hasPlay)
            {
                bias += 5;
                currentStrategy = Strategy.Random;
            }

        }
        if (currentStrategy == Strategy.Killer)
        {
            bool[] possibleSlots = new bool[3];

            int targetHealth = 0;
            
            for (int i = 0; i < 3; i++)
            {
                if (combatManager.playerCards[i] != null && combatManager.enemyCards[i] == null)
                {
                    hasPlay = true;
                    possibleSlots[i] = true;
                    if (combatManager.playerCards[i].card.health < targetHealth)
                    {
                        targetHealth = combatManager.playerCards[i].card.health;
                        targetIndex = i;
                    }
                }
            }
            if (!hasPlay)
            {
                currentStrategy = Strategy.Aggressive;
            }

        }
        if (currentStrategy == Strategy.Aggressive)
        {
            bool[] possibleSlots = new bool[3];

            for (int i = 0; i < 3; i++)
            {
                if (combatManager.playerCards[i] == null && combatManager.enemyCards[i] == null)
                {
                    hasPlay = true;
                    possibleSlots[i] = true;
                    targetIndex = i;
                }
            }
            if (!hasPlay)
            {
                currentStrategy = Strategy.Random;
            }

        }
        if (currentStrategy == Strategy.Random)
        {
            int failEscape = 0;
            do
            {
                failEscape++;
                targetIndex = Random.Range(0, 3);
            } while (combatManager.enemyCards[targetIndex] != null && failEscape < 20);

            if (failEscape < 20 && card != null)
            {
                hasPlay = true;
            }
        }

        // Play card
        if (hasPlay)
        {
            if (card != null)
            {
                combatManager.EnemyPlayCard(card, targetIndex);
            }
        }

        // Use lost soul
        for (int i = 0; i < combatManager.enemyDeck.cardsInHandAsCards.Count; i++) 
        {
            Card cardInHand = combatManager.enemyDeck.cardsInHandAsCards[i];
            if (cardInHand.name == "LostSoul") UseLostSoul(cardInHand);
        }

        // Rethink if the enemy has enough energy
        if (combatManager.enemyDeck.energy > 0 && thinkLimit > 0)
        {
            thinkLimit--;
            Think();
        }
    }
    Strategy PickStrategy()
    {

        if (combatManager.enemyHealth + bias <= startPlayingDefensivelyAt)
        {
            bias = 0;
            return Strategy.Defensive;
        }
        else if (combatManager.playerHealth - bias <= startPlayingAggressivelyAt || combatManager.playerHealth - bias < combatManager.enemyHealth)
        {
            if (useKillerStrategyInstead) return Strategy.Killer;

            return Strategy.Aggressive;
        }
        return Strategy.Random;
    }

    Card PickCard()
    {
        Card cardToPick = null;
        if (currentStrategy == Strategy.Defensive)
        {
            foreach (Card card in combatManager.enemyDeck.cardsInHandAsCards)
            {
                if ((cardToPick == null || cardToPick.maxHealth < card.maxHealth) && card.cost <= combatManager.enemyDeck.energy) cardToPick = card;
            }
        }
        else if (currentStrategy == Strategy.Aggressive || currentStrategy == Strategy.Killer)
        {
            foreach (Card card in combatManager.enemyDeck.cardsInHandAsCards)
            {
                if ((cardToPick == null || cardToPick.attack < card.attack) && card.cost <= combatManager.enemyDeck.energy) cardToPick = card;
            }
        }
        else if (currentStrategy == Strategy.Random)
        {
            foreach (Card card in combatManager.enemyDeck.cardsInHandAsCards)
            {
                if (cardToPick == null && card.cost <= combatManager.enemyDeck.energy) cardToPick = card;
            }
        }

        if (cardToPick != null && cardToPick.name == "LostSoul") return null;

        return cardToPick;
    }

    void UseLostSoul(Card lostSoulCard)
    {
        Debug.Log("Trying to play lost soul");
        foreach (CardInCombat card in combatManager.enemyCards)
        {
            if (card != null && card.card.injuries.Count > 0)
            {
                Card healedCard = card.card;

                healedCard.AcceptLostSoul();
                combatManager.deck.UpdateCardAppearance(card.transform, healedCard);
                Debug.Log("playing lost soul on " + healedCard.name);
                for (int i = 0; i < healedCard.sigils.Count; i++)
                {
                    Debug.Log(healedCard.sigils[i].name);
                }

                combatManager.enemyDeck.cards.Remove(lostSoulCard);

                combatManager.enemyDeck.cardsInHandAsCards.Remove(lostSoulCard);
                return;
            }
        }
    }
}
