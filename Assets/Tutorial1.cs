using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : MonoBehaviour
{
    Dialogue dialogue;
    CombatManager combatManager;
    bool useMouse = true;
    int counter;
    bool boolCounter;
    CardInCombat mark;
    Button endTurnButton;

    void Start()
    {
        combatManager = GameObject.Find("Deck").GetComponent<CombatManager>();
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();

        endTurnButton.interactable = false;
        dialogue = combatManager.enemy.dialogue;
        dialogue.StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (useMouse && Input.GetMouseButtonDown(0))
        {
            dialogue.NextLine();
            UpdateRules();
        }

        if (dialogue.line == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                if (combatManager.playerBenchCards[i] != null)
                {
                    mark = combatManager.playerBenchCards[i];
                    boolCounter = mark.benched;
                    dialogue.NextLine();
                    UpdateRules();
                }
                else if (combatManager.playerCombatCards[i] != null)
                {
                    mark = combatManager.playerCombatCards[i];
                    boolCounter = mark.benched;
                    dialogue.NextLine();
                    UpdateRules();
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
                    UpdateRules();
                }
                else
                {
                    dialogue.NextLine(2);
                    UpdateRules();
                }
                counter = combatManager.round;
            }
        }
        else if (dialogue.line == 7)
        {
            if (!mark.benched)
            {
                dialogue.NextLine();
                UpdateRules();
            }
        }
        else if (dialogue.line == 8)
        {
            if (counter != combatManager.round)
            {
                dialogue.NextLine();
                UpdateRules();
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
                else if (combatManager.playerCombatCards[i] != null)
                {
                    counter++;
                }
            }
            if (counter == 2)
            {
                dialogue.NextLine();
                UpdateRules();
            }
        }
        else if (dialogue.line == 13)
        {
            if (counter != combatManager.round)
            {
                dialogue.NextLine();
                UpdateRules();
            }
        }
    }

    void UpdateRules()
    {
        Debug.Log("line " + dialogue.line);
        if (dialogue.line == 2)
        {
            useMouse = false;
        }
        else if (dialogue.line == 3)
        {
            useMouse = true;
        }
        else if (dialogue.line == 6)
        {
            counter = 0;
            useMouse = false;
        } 
        else if (dialogue.line == 8)
        {
            endTurnButton.interactable = true;  
        }
        else if (dialogue.line == 9)
        {
            endTurnButton.interactable = false;
            useMouse = true;
        }
        else if (dialogue.line == 12)
        {
            useMouse = false;
        }
        else if (dialogue.line == 13)
        {
            endTurnButton.interactable = true;
            counter = combatManager.round;
        }
        else if (dialogue.line == 14)
        {
            endTurnButton.interactable = false;
            useMouse = true;
        }
    }
}

