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
        if (combatManager.round > turns.Length) return;
        Turn currentTurn = turns[combatManager.round-1];
        if (currentTurn != null)
        {
            if (currentTurn.forcePlace) ForceCards(currentTurn);
            else                        PlayTurn(currentTurn);
        }
    }

    void ForceCards(Turn turn)
    {
        for (int i = 0; i < 3; i++)
        {
            CardInCombat combatCard = combatManager.enemyBenchCards[i] ;
            if (combatCard != null) combatCard.card.health = 0;
            combatCard              = combatManager.enemyCombatCards[i];
            if (combatCard != null) combatCard.card.health = 0;

            Card card = turn.combatCards[i];
            if (card != null) 
            {
                string cardName = card.name;
                card = Instantiate(card).ResetCard();
                card.name = cardName;
                PlayCard(card, i, false); 
            }
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
    void PlayTurn(Turn turn) 
    {
        for (int i = 0; i < 3; i++)
        {
            Card card = turn.combatCards[i];
            if (card != null && combatManager.enemyCombatCards[i] == null)
            {
                string cardName = card.name;
                card = Instantiate(card).ResetCard();
                card.name = cardName;
                PlayCard(card, i, false);
            }
            card = turn.benchCards[i];
            if (card != null && combatManager.enemyBenchCards[i] == null)
            {
                string cardName = card.name;
                card = Instantiate(card).ResetCard();
                card.name = cardName;
                PlayCard(card, i, true);
            }
        }
    }

}
