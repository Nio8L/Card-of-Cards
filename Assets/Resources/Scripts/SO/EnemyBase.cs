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
        combatManager = CombatManager.combatManager;
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
        CardSlot slotToUse;
        // Find the slot to play the card at
        if (benched) slotToUse = combatManager.enemyBenchSlots [slotNumber].GetComponent<CardSlot>();
        else         slotToUse = combatManager.enemyCombatSlots[slotNumber].GetComponent<CardSlot>();
        
        // Call the proper PlayCard();
        combatManager.PlayCard(card, slotToUse, useDeck);
    }

    public string ReturnPath(){
        return path + "/" + name;
    }
}
