using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemyBase : ScriptableObject
{
    public int maxHealth;

    public Dialogue dialogue;

    public string path = "";

    [Header("Hunt settings")]
    public bool huntAI;
    public int huntRounds;

    [Header("Tutorial and pacing objects")]
    public GameObject pacingObject;
    public bool isTutorialEnemy;
    public bool isHunter;

    [HideInInspector]
    protected CombatManager combatManager;
    protected bool useDeck;

    public virtual void Initialize()
    {
        combatManager = GameObject.Find("Deck").GetComponent<CombatManager>();
        combatManager.enemyHealth = maxHealth;
        if (dialogue != null) dialogue.Initialize();

        if (dialogue != null && dialogue.StartOnLoad) dialogue.StartDialogue();
        else GameObject.Find("DialogueBox").SetActive(false);

        if (pacingObject != null)
        {
            Instantiate(pacingObject, Vector3.zero, Quaternion.identity);
        }
    }   
    public virtual void StartTurn()
    {
        if (dialogue != null && dialogue.NextLineAtStartOfTurn) dialogue.StartDialogue();
    }
    public void PlayCard(Card card, int slotNumber, bool benched)
    {
        GameObject cardToCreate;
        if (benched) cardToCreate = Instantiate(combatManager.deck.cardInCombatPrefab, combatManager.enemyBenchSlots[slotNumber].transform.position, Quaternion.identity);
        else cardToCreate = Instantiate(combatManager.deck.cardInCombatPrefab, combatManager.enemyCombatSlots[slotNumber].transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(combatManager.deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;
        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = combatManager.enemyDeck;
        cardInCombat.slot = slotNumber;
        cardInCombat.playerCard = false;

        if (benched)
        {
            cardInCombat.benched = true;
            combatManager.enemyBenchCards[slotNumber] = cardInCombat;
        }
        else
        {
            cardInCombat.benched = false;
            combatManager.enemyCombatCards[slotNumber] = cardInCombat;
        }

        if (useDeck)
        {
            combatManager.enemyDeck.energy -= card.cost;
            combatManager.enemyDeck.cardsInHandAsCards.Remove(card);
        }
    }

    public string ReturnPath(){
        return path + "/" + name;
    }
}
