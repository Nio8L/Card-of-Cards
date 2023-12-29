using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Scripted")]
public class ScriptedEnemy : EnemyBase
{
    [SerializeField]
    private Turn[] turns;
    
    [System.Serializable]
    private class Turn
    {
        public Card[] combatCards = new Card[3];
        public Card[] benchCards  = new Card[3];
        public bool forcePlace;
    }
    public override void Initialize()
    {
        base.Initialize();
        useDeck = false;
    }
    public override void StartTurn()
    {
        base.StartTurn();
        if (CombatManager.combatManager.round > turns.Length) return;
        Turn currentTurn = turns[CombatManager.combatManager.round-1];
        if (currentTurn != null)
        {
            if (currentTurn.forcePlace) ForceCards(currentTurn);
            else                        PlayTurn(currentTurn);
        }
    }

    void ForceCards(Turn turn)
    {
        /* This function forces the enemies side of the board to be the same as the cards in Turn turn s lists
            If there is a null somewhere in the lists it just kills the card in that slot
            If there is a card it kills the old card and places the new one in its place
        */
        for (int i = 0; i < 3; i++)
        {
            // Kill existing cards
            CardInCombat combatCard = CombatManager.combatManager.enemyBenchCards[i] ;
            if (combatCard != null) combatCard.card.health = 0;
            combatCard              = CombatManager.combatManager.enemyCombatCards[i];
            if (combatCard != null) combatCard.card.health = 0;

            // Spawn combat cards
            Card card;
            if (i < turn.combatCards.Length){
                card = turn.combatCards[i];
                if (card != null) 
                {
                    string cardName = card.name;
                    card = Instantiate(card).ResetCard();
                    card.name = cardName;
                    PlayCard(card, i, false); 
                }
            }

            // Spawn bench cards
            if (i < turn.benchCards.Length){
                card = turn.benchCards[i];
                if (card != null)
                {
                    string cardName = card.name;
                    card = Instantiate(card).ResetCard();
                    card.name = cardName;
                    PlayCard(card, i, true);
                }
            }
        } 
    }
    void PlayTurn(Turn turn) 
    {
        for (int i = 0; i < 3; i++)
        {
            Card card;

            if (i < turn.combatCards.Length){
                card = turn.combatCards[i];
                if (card != null && CombatManager.combatManager.enemyCombatCards[i] == null)
                {
                    string cardName = card.name;
                    card = Instantiate(card).ResetCard();
                    card.name = cardName;
                    PlayCard(card, i, false);
                }
            }

            if (i < turn.benchCards.Length){
                card = turn.benchCards[i];
                if (card != null && CombatManager.combatManager.enemyBenchCards[i] == null)
                {
                    string cardName = card.name;
                    card = Instantiate(card).ResetCard();
                    card.name = cardName;
                    PlayCard(card, i, true);
                }
            }
        }
    }

}
