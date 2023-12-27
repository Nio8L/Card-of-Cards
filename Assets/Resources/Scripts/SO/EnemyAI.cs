using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/AI")]
public class EnemyAI : EnemyBase
{
    [Header("AI settings")]

    [Header("Deck")]
    public List<Card> cards = new();

    [Header("Settings")]
    public int maxCardsPerTurn = 3; // Can't be more than 3 or less than 1
    public int maxEnergy = 3;
    public int startPlayingDefensivelyAt;
    public int startPlayingAggressivelyAt;
    public bool canUseLostSoul = true;
    public bool useKillerStrategyInstead;
    public bool canSeePlayerCardsPlacedThisTurn = false;
    public bool canHideCardsThatAreAboutToDie = false;

    // Settings ^

    bool useTypeOfDamageToDecideCard = true;

    int bias = 0;
    int thinkLimit;
    int cardsPlayedThisTurn = 0;

    bool[] savedLastRound;
    CardInCombat[] playerCards;
    CardInCombat[] playerBenchedCards;

    Strategy currentStrategy;
    enum Strategy
    {
        Aggressive,
        Killer,
        Defensive,
        Savior,
        Random
    }
    public override void Initialize()
    {
        base.Initialize();
        useDeck = true;
        foreach (Card card in cards)
        {
            Card cardToAdd = Instantiate(card).ResetCard();
            cardToAdd.name = card.name;
            CombatManager.combatManager.enemyDeck.AddCard(cardToAdd);
            CombatManager.combatManager.enemyDeck.Shuffle();
        }
        savedLastRound = new bool[3];
    }
    public override void StartTurn()
    {
        base.StartTurn();

        CombatManager.combatManager.enemyDeck.energy = maxEnergy;

        useTypeOfDamageToDecideCard = true;
        thinkLimit = 5;
        cardsPlayedThisTurn = 0;
        if (canSeePlayerCardsPlacedThisTurn)
        {
            playerCards = CombatManager.combatManager.playerCombatCards;
            playerBenchedCards = CombatManager.combatManager.playerBenchCards;
        }
        else 
        { 
            playerCards = CombatManager.combatManager.playerCombatCardsAtStartOfTurn;
            playerBenchedCards = CombatManager.combatManager.playerBenchCardsAtStartOfTurn; 
        }
        
        MoveCardsForward();
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
                if (playerCards[i] != null && CombatManager.combatManager.enemyCombatCards[i] == null)
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
        if (currentStrategy == Strategy.Savior)
        {
            int targetValue = 0;

            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i] != null && CombatManager.combatManager.enemyBenchCards[i] == null)
                {
                    hasPlay = true;
                    if (CombatManager.combatManager.enemyCombatCards[i].card.cost > targetValue)
                    {
                        targetValue = CombatManager.combatManager.enemyCombatCards[i].card.cost;
                        targetIndex = i;
                    }
                }
            }

            if (!hasPlay)
            {
                bias += 10;
                currentStrategy = Strategy.Random;
            }

        }
        if (currentStrategy == Strategy.Killer)
        {

            int targetHealth = 0;
            
            for (int i = 0; i < 3; i++)
            {
                if (playerCards[i] != null && CombatManager.combatManager.enemyCombatCards[i] == null)
                {
                    //Debug.Log("Can be played at slot: " + i);
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
                if (playerCards[i] == null && CombatManager.combatManager.enemyCombatCards[i] == null)
                {
                    hasPlay = true;
                    targetIndex = i;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (hasPlay) break;
                if (playerBenchedCards[i] == null && CombatManager.combatManager.enemyBenchCards[i] == null)
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
            } while (CombatManager.combatManager.enemyBenchCards[targetIndex] != null && failEscape < 20);

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
            if (card != null && CombatManager.combatManager.enemyBenchCards[targetIndex] == null)
            {
                PlayCard(card, targetIndex, true);
                cardsPlayedThisTurn++;
            }
        }

        // Use lost soul
        if (canUseLostSoul)
        {
            for (int i = 0; i < CombatManager.combatManager.enemyDeck.cardsInHandAsCards.Count; i++)
            {
                Card cardInHand = CombatManager.combatManager.enemyDeck.cardsInHandAsCards[i];
                if (cardInHand.name == "LostSoul") UseLostSoul(cardInHand);
            }
        }

        // Rethink if the enemy has enough energy
        if (CombatManager.combatManager.enemyDeck.energy > 0 && thinkLimit > 0 && cardsPlayedThisTurn < maxCardsPerTurn)
        {
            thinkLimit--;
            Think();
        }
        else if (canHideCardsThatAreAboutToDie) TryToSaveCards();
    }
    Strategy PickStrategy()
    {

        if (CombatManager.combatManager.enemyHealth + bias <= startPlayingDefensivelyAt)
        {
            bias = 0;
            for (int i = 0; i < 3; i++)
            {
                if (CombatManager.combatManager.enemyCombatCards[i] == null)
                {
                    return Strategy.Defensive;
                }
            }
            return Strategy.Savior;
        }
        else if (CombatManager.combatManager.enemyHealth - bias <= startPlayingAggressivelyAt || CombatManager.combatManager.playerHealth - bias < CombatManager.combatManager.enemyHealth)
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
        if (currentStrategy == Strategy.Defensive || currentStrategy == Strategy.Savior)
        {
            foreach (Card card in CombatManager.combatManager.enemyDeck.cardsInHandAsCards)
            {
                if ((cardToPick == null || cardToPick.maxHealth < card.maxHealth) && card.cost <= CombatManager.combatManager.enemyDeck.energy) cardToPick = card;
            }
        }
        else if (currentStrategy == Strategy.Aggressive || currentStrategy == Strategy.Killer)
        {
            foreach (Card card in CombatManager.combatManager.enemyDeck.cardsInHandAsCards)
            {
                if ((cardToPick == null || cardToPick.attack < card.attack) && card.cost <= CombatManager.combatManager.enemyDeck.energy) cardToPick = card;
            }
        }
        else if (currentStrategy == Strategy.Random)
        {
            foreach (Card card in CombatManager.combatManager.enemyDeck.cardsInHandAsCards)
            {
                if (cardToPick == null && card.cost <= CombatManager.combatManager.enemyDeck.energy) cardToPick = card;
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
        //Debug.Log("Trying to play lost soul");
        foreach (CardInCombat card in CombatManager.combatManager.enemyCombatCards)
        {
            if (card != null && card.card.injuries.Count > 0)
            {
                Card healedCard = card.card;

                healedCard.AcceptLostSoul();
                CombatManager.combatManager.deck.UpdateCardAppearance(card.transform, healedCard);
                //Debug.Log("playing lost soul on " + healedCard.name);
                for (int i = 0; i < healedCard.sigils.Count; i++)
                {
                    Debug.Log(healedCard.sigils[i].name);
                }

                CombatManager.combatManager.enemyDeck.cards.Remove(lostSoulCard);
                CombatManager.combatManager.enemyDeck.cardsInHandAsCards.Remove(lostSoulCard);

                Instantiate(CombatManager.combatManager.deck.soulHeart, card.gameObject.transform.position, Quaternion.identity);

                // Visual and sound effects
                SoundManager.soundManager.Play("LostSoul");

                LostSoulVisuals soulHeart;

                soulHeart = Instantiate(CombatManager.combatManager.deck.soulHeart, card.gameObject.transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                soulHeart.angle = 120f;
                soulHeart.primaryHeart = false;

                soulHeart = Instantiate(CombatManager.combatManager.deck.soulHeart, card.gameObject.transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                soulHeart.GetComponent<LostSoulVisuals>().angle = 240f;
                soulHeart.primaryHeart = false;

                return;
            }
        }
    }
    void Bench(int slot)
    {
        if (CombatManager.combatManager.enemyBenchCards[slot] != null)
        {
            CombatManager.combatManager.enemyBenchCards[slot].BenchOrUnbenchEnemy();
        }
        else if (CombatManager.combatManager.enemyCombatCards[slot] != null)
        {
            CombatManager.combatManager.enemyCombatCards[slot].BenchOrUnbenchEnemy();
        }
    } 
    void MoveCardsForward()
    {
        for (int i = 0; i < 3; i++)
        {
            if (CombatManager.combatManager.enemyBenchCards[i] != null && CombatManager.combatManager.enemyCombatCards[i] == null)
            {
                Bench(i);
            }
        }
    }
    void TryToSaveCards()
    {
        if (CombatManager.combatManager.enemyHealth/(float)maxHealth >= 0.25f)
        {
            for (int i = 0; i < 3; i++)
            {
                if (savedLastRound[i] == true)
                {
                    savedLastRound[i] = false;
                    continue;
                }

                if (CombatManager.combatManager.playerCombatCards[i] == null || CombatManager.combatManager.enemyCombatCards[i] == null) continue;

                if (CombatManager.combatManager.enemyCombatCards[i].card.health - CombatManager.combatManager.playerCombatCards[i].card.attack <= 0)
                {
                    foreach (Card.TypeOfDamage injury in CombatManager.combatManager.enemyCombatCards[i].card.injuries)
                    {
                        if (injury == CombatManager.combatManager.playerCombatCards[i].card.typeOfDamage)
                        {
                            Bench(i);
                            savedLastRound[i] = true;
                        }
                    }
                }
            }
        }
    }
}
