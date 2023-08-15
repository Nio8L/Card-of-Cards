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
    
    int bias = 0;
    int thinkLimit;

    Strategy currentStrategy;
    enum Strategy
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
        Debug.Log("Starting enemy turn");
        thinkLimit = 5;
        Think();
    }
    void Think()
    {
        Debug.Log("Thinking");
        currentStrategy = PickStrategy();
        Debug.Log("Strategy: " + currentStrategy);

        if (currentStrategy == Strategy.Defensive)
        {
            bool[] possibleSlots = new bool[3];

            int targetDamage = 0;
            int targetIndex = 0;
            bool hasPlay = false;
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
            if (hasPlay) { 
                Card card = PickCard();
                if (card != null)
                {
                    combatManager.EnemyPlayCard(card, targetIndex);
                }
            }
            else
            {
                currentStrategy = Strategy.Aggressive;
                bias += 5;
            }

        }
        else if (currentStrategy == Strategy.Killer)
        {
            bool[] possibleSlots = new bool[3];

            int targetHealth = 0;
            int targetIndex = 0;
            bool hasPlay = false;
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
            if (hasPlay)
            {
                Card card = PickCard();
                if (card != null)
                {
                    combatManager.EnemyPlayCard(card, targetIndex);
                }
            }

        }
        else if (currentStrategy == Strategy.Aggressive)
        {
            bool[] possibleSlots = new bool[3];

            int targetIndex = 0;
            bool hasPlay = false;
            for (int i = 0; i < 3; i++)
            {
                if (combatManager.playerCards[i] == null && combatManager.enemyCards[i] == null)
                {
                    hasPlay = true;
                    possibleSlots[i] = true;
                    targetIndex = i;
                }
            }
            if (hasPlay)
            {
                Card card = PickCard();
                if (card != null)
                {
                    combatManager.EnemyPlayCard(card, targetIndex);
                }
            }

        }
        else if (currentStrategy == Strategy.Random)
        {
            Card cardToPlay = PickCard();

            int rand, failEscape = 0;
            do
            {
                failEscape++;
                rand = Random.Range(0, 3);
            } while (combatManager.enemyCards[rand] != null && failEscape < 20);

            if (failEscape < 20 && cardToPlay != null)
            {
                combatManager.EnemyPlayCard(cardToPlay, rand);
            }
        }


        if (combatManager.enemyDeck.energy > 0 && thinkLimit > 0)
        {
            Debug.Log("Think again | energy left: " + combatManager.enemyDeck.energy);
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
        }else if (currentStrategy == Strategy.Random)
        {
            foreach (Card card in combatManager.enemyDeck.cardsInHandAsCards)
            {
                if (cardToPick == null && card.cost <= combatManager.enemyDeck.energy) cardToPick = card;
            }
        }
        if (cardToPick != null) Debug.Log("Card picked: " + cardToPick.name);
        else Debug.Log("No card found");
        return cardToPick;
    }
}
