using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAI")]
public class EnemyAI : ScriptableObject
{

    [Header("Deck")]
    public List<Card> cards = new();

    [Header("Settings")]
    public int maxHealth;
    public int maxCardsPerTurn = 3; // Can't more than 3 or less than 1
    public int startPlayingDefensivelyAt;
    public int startPlayingAggressivelyAt;
    //public bool canBench;
    //public bool tryToPredictBench;
    public bool canUseLostSoul = true;
    public bool useKillerStrategyInstead;
    public bool canSeePlayerCardsPlacedThisTurn = false;
    // Settings ^
    bool useTypeOfDamageToDecideCard = true;

    int bias = 0;
    int thinkLimit;
    int cardsPlayedThisTurn = 0;

    CardInCombat[] playerCards;

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
        //Debug.Log("Initializing enemy ai");
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
        useTypeOfDamageToDecideCard = true;
        thinkLimit = 5;
        cardsPlayedThisTurn = 0;
        if (canSeePlayerCardsPlacedThisTurn) playerCards = combatManager.playerCards;
        else playerCards = combatManager.playerCardsAtStartOfTurn;
        Think();
    }
    void Think()
    {
        currentStrategy = PickStrategy();

        bool hasPlay = false;
        int targetIndex = 0;

        // Pick slot
        if (currentStrategy == Strategy.Defensive)
        {
            int targetDamage = 0;
            
            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i] != null && combatManager.enemyCards[i] == null)
                {
                    hasPlay = true;
                    if (playerCards[i].card.attack > targetDamage)
                    {
                        targetDamage = playerCards[i].card.attack;
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

            int targetHealth = 0;
            
            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i] != null && combatManager.enemyCards[i] == null)
                {
                    Debug.Log("Can be played at slot: " + i);
                    hasPlay = true;
                    if (playerCards[i].card.health < targetHealth || targetHealth == 0)
                    {
                        targetHealth = playerCards[i].card.health;
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

            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i] == null && combatManager.enemyCards[i] == null)
                {
                    hasPlay = true;
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

            if (failEscape < 20)
            {
                hasPlay = true;
            }
        }

        // Pick a card;
        Card card;
        if (playerCards[targetIndex] != null)
        {
            card = PickCard(playerCards[targetIndex].card.typeOfDamage);
        }
        else
        {
            useTypeOfDamageToDecideCard = false;
            card = PickCard(Card.TypeOfDamage.Bite);
        }

        // Play card
        if (hasPlay)
        {
            if (card != null && combatManager.enemyCards[targetIndex] == null)
            {
                combatManager.EnemyPlayCard(card, targetIndex);
                cardsPlayedThisTurn++;
            }
        }

        // Use lost soul
        if (canUseLostSoul)
        {
            for (int i = 0; i < combatManager.enemyDeck.cardsInHandAsCards.Count; i++)
            {
                Card cardInHand = combatManager.enemyDeck.cardsInHandAsCards[i];
                if (cardInHand.name == "LostSoul") UseLostSoul(cardInHand);
            }
        }

        // Rethink if the enemy has enough energy
        if (combatManager.enemyDeck.energy > 0 && thinkLimit > 0 && cardsPlayedThisTurn < maxCardsPerTurn)
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
        else if (combatManager.enemyHealth - bias <= startPlayingAggressivelyAt || combatManager.playerHealth - bias < combatManager.enemyHealth)
        {
            bias -= bias / 2;
            if (useKillerStrategyInstead) return Strategy.Killer;

            return Strategy.Aggressive;
        }
        return Strategy.Random;
    }
    Card PickCard(Card.TypeOfDamage enemyType)
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
        if (cardToPick != null)
        {
            if (cardToPick.name == "LostSoul") return null;
            else if (useTypeOfDamageToDecideCard && (currentStrategy == Strategy.Defensive || currentStrategy == Strategy.Killer))
            {
                foreach (Card.TypeOfDamage injury in cardToPick.injuries)
                {
                    if (injury == enemyType) { useTypeOfDamageToDecideCard = false; bias += Random.Range(-15, 15); return null; }
                }
            }
        }

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

                Instantiate(combatManager.deck.soulHeart, card.gameObject.transform.position, Quaternion.identity);

                // Visual and sound effects
                SoundManager.soundManager.Play("LostSoul");

                LostSoulVisuals soulHeart;

                soulHeart = Instantiate(combatManager.deck.soulHeart, card.gameObject.transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                soulHeart.angle = 120f;
                soulHeart.primaryHeart = false;

                soulHeart = Instantiate(combatManager.deck.soulHeart, card.gameObject.transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                soulHeart.GetComponent<LostSoulVisuals>().angle = 240f;
                soulHeart.primaryHeart = false;

                return;
            }
        }
    }
}
