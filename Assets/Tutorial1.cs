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
            if (counter != combatManager.round)
            {
                dialogue.NextLine(2);
            }
        }
        else if (dialogue.line == 8)
        {
            if (counter != combatManager.round)
            {
                dialogue.NextLine();
            }
        }
        }

    void UpdateRules()
    {
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
            useMouse = false;
        } 
        else if (dialogue.line == 7 || dialogue.line == 8)
        {
            endTurnButton.interactable = true;
        }
        else if (dialogue.line == 9)
        {
            endTurnButton.interactable = false;
            useMouse = true;
        }
    }
}
