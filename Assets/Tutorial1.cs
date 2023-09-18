using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : MonoBehaviour
{
    Dialogue dialogue;
    CombatManager combatManager;
    int counter;
    bool boolCounter;
    bool addedCards = false;
    CardInCombat mark;
    Button endTurnButton;
    public List<Card> cardsToAdd = new();
    void Start()
    {
        combatManager = GameObject.Find("Deck").GetComponent<CombatManager>();
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.interactable = false;
        dialogue = combatManager.enemy.dialogue;
        dialogue.NextLineAtStartOfTurn = false;
        dialogue.UpdateClickRule(true);
        dialogue.StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogue.line == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                if (combatManager.playerBenchCards[i] != null)
                {
                    mark = combatManager.playerBenchCards[i];
                    boolCounter = mark.benched;
                    dialogue.NextLine();
                }
                if (combatManager.playerCombatCards[i] != null)
                {
                    mark = combatManager.playerCombatCards[i];
                    boolCounter = mark.benched;
                    dialogue.NextLine();
                }
            }
        }
        else if (dialogue.line == 6)
        {
            if (mark.benched != boolCounter)
            {
                counter++;
                boolCounter = !boolCounter;
            }
            if (counter >= 3)
            {
                if (mark.benched)
                {
                    dialogue.NextLine();
                }
                else
                {
                    dialogue.NextLine(2);
                }
            }
        }
        else if (dialogue.line == 7)
        {
            if (!mark.benched)
            {
                dialogue.NextLine();
            }
        }
        else if (dialogue.line == 8)
        {
            if (mark.benched)
            {
                endTurnButton.interactable = false;
                dialogue.GoBack();
            }
            if (combatManager.round == 2)
            {
                dialogue.NextLine();
            }
        }
        else if (dialogue.line == 12)
        {
            counter = 0;
            for (int i = 0; i < 3; i++)
            {
                if (combatManager.playerBenchCards[i] != null)
                {
                    counter++;
                }
                if (combatManager.playerCombatCards[i] != null)
                {
                    counter++;
                }
            }
            if (counter == 2)
            {
                dialogue.NextLine();
            }
        }
        else if (dialogue.line == 13)
        {
            if (counter != combatManager.round)
            {
                dialogue.NextLine();
            }
        }
        else if (dialogue.line == 22)
        {
            for (int i = 0; i < combatManager.deck.cards.Count; i++)
            {
                if (combatManager.deck.cards[i].name == "LostSoul")
                {
                    dialogue.NextLine();
                }
            }
        }

        UpdateRules();


    }

    void UpdateRules()
    {
        if (dialogue.line == 2)
        {
            dialogue.UpdateClickRule(false);
        }
        else if (dialogue.line == 3)
        {
            dialogue.UpdateClickRule(true);
        }
        else if (dialogue.line == 6)
        {
            dialogue.UpdateClickRule(false);
        } 
        else if (dialogue.line == 8)
        {
            endTurnButton.interactable = true;  
        }
        else if (dialogue.line == 9)
        {
            endTurnButton.interactable = false;
            dialogue.UpdateClickRule(true);
        }
        else if (dialogue.line == 12)
        {
            dialogue.UpdateClickRule(false);
        }
        else if (dialogue.line == 13)
        {
            endTurnButton.interactable = true;
            counter = combatManager.round;
        }
        else if (dialogue.line == 14)
        {
            endTurnButton.interactable = false;
            dialogue.UpdateClickRule(true);
        }
        else if (dialogue.line == 22)
        {
            endTurnButton.interactable = true;
            dialogue.UpdateClickRule(false);
        }
        else if (dialogue.line == 23)
        {
            endTurnButton.interactable = false;
            dialogue.UpdateClickRule(true);
        }
        else if (dialogue.line == 25)
        {
            if (addedCards) return;
            addedCards = true;
            combatManager.deck.cardsToBeAdded.AddRange(cardsToAdd);
            combatManager.deck.AddCard(cardsToAdd.Count);
            combatManager.deck.DrawCard(cardsToAdd.Count);
        }
        else if (dialogue.line == 33)
        {
            endTurnButton.interactable = true;
            dialogue.UpdateClickRule(true);
            dialogue.NextLineAtStartOfTurn = true;
        }
    }
}

