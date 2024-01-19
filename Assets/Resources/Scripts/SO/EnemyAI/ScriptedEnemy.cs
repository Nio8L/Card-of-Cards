using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Scripted")]
public class ScriptedEnemy : EnemyBase
{
    public bool turnZero;
    [SerializeField]
    public Turn[] turns;
    
    [System.Serializable]
    public class Turn
    {
        public Card[] combatCards  = new Card[5];
        public Card[] benchCards   = new Card[5];

        public bool[] benchColumbs = new bool[5];
        public bool forcePlace;

        public bool pushBench;
    }
    public override void Initialize()
    {
        base.Initialize();
        useDeck = false;
    }
    public override void StartTurn()
    {
        base.StartTurn();

        // Return if there are no pre made rounds left to play
        if (CombatManager.combatManager.round > turns.Length) return;

        // Check if there is turn zero
        int bonusForZeroTurn = 0;
        if (turnZero) bonusForZeroTurn = 1;

        int turnNumber = CombatManager.combatManager.round-1 + bonusForZeroTurn;
        if (turns.Length <= turnNumber) return;

        // Find the current turn
        Turn currentTurn = turns[CombatManager.combatManager.round-1 + bonusForZeroTurn];

        // Push bench
        if (currentTurn.pushBench) Bench();

        // The enemy plays cards where the player hit it
        PlayTurn(currentPacingObject.GetComponent<HuntManager>().PlayStrongCards());

        // Play cards
        if (currentTurn != null)
        {
            if (currentTurn.forcePlace) ForceCards(currentTurn);
            else                        PlayTurn(currentTurn);
        }
        // Bench specified columbs
        Bench(currentTurn);
    }
    void ForceCards(Turn turn)
    {
        /* This function forces the enemies side of the board to be the same as the cards in Turn turn s lists
            If there is a null somewhere in the lists it just kills the card in that slot
            If there is a card it kills the old card and places the new one in its place
        */
        for (int i = 0; i < 5; i++)
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
    public void PlayTurn(Turn turn) 
    {
        for (int i = 0; i < 5; i++)
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
    void Bench(Turn turn){
        // Bench columbs specified in benchColumbs array
        for (int i = 0; i < turn.benchColumbs.Length; i++){
            if (turn.benchColumbs[i]){
                Bench(i);
            }
        }
    }
    void Bench(){
        // Push bench
        for (int i = 0; i < 5; i++){
            if(CombatManager.combatManager.enemyBenchCards[i] != null && CombatManager.combatManager.enemyCombatCards[i] == null){
                Bench(i);
            }
        }
    }
    void Bench(int col){
        // Bench a columb
        if (CombatManager.combatManager.enemyBenchCards[col] != null){
            CombatManager.combatManager.enemyBenchCards[col].BenchOrUnbench(false, true);
        }
        else if (CombatManager.combatManager.enemyCombatCards[col] != null){
            CombatManager.combatManager.enemyCombatCards[col].BenchOrUnbench(false, true);
        }
    }

    public override ScriptedEnemy GetScriptedEnemy(){
        return this;
    }
}
