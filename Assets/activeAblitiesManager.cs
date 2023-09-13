using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeAblitiesManager : MonoBehaviour
{
    List<Sigil> activatedActivePlayerSigil = new ();
    List<Sigil> activatedActiveEnemySigil = new ();

    public bool holding = false;

    public CombatManager combatManager;

    bool abilityHasEnded = false;

    void Update()
    {
        //Debug.Log(activatedActivePlayerSigil.Count);
        if (Input.GetMouseButtonDown(1))
        {
            if (holding) return;
            holding = true;

            CardSlot slot = TryToFindSlot();
            if (slot == null || !slot.playerSlot) return;

            SimulateClick(slot);
        }
        else holding = false;
    }

    CardSlot TryToFindSlot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.forward, 100);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "BenchSlot")
            {
                return hit.transform.GetComponent<CardSlot>();
            }
            else if (hit.collider.tag == "CardSlot")
            {
                return hit.transform.GetComponent<CardSlot>();
            }
        }
        return null;
    }

    public void SimulateClick(CardSlot slot)
    {
        TryToEndActiveSigils(slot);

        if (abilityHasEnded) return;

        CardInCombat cardClicked;

        if (slot.playerSlot) cardClicked = slot.bench ? combatManager.playerBenchCards[slot.slot] : combatManager.playerCombatCards[slot.slot];
        else cardClicked = slot.bench ? combatManager.enemyBenchCards[slot.slot] : combatManager.enemyCombatCards[slot.slot];

        if (cardClicked != null)
        {
            List<Sigil> secondStage = cardClicked.card.ActivateActiveSigilStartEffects(cardClicked);

            if (secondStage.Count == 0) return;

            if (cardClicked.playerCard)
            {
                activatedActivePlayerSigil.Clear();
                activatedActivePlayerSigil.AddRange(secondStage);
                cardClicked.SetActiveSigilStar(activatedActivePlayerSigil[0]);
            }
            else
            {
                activatedActiveEnemySigil.Clear();
                activatedActiveEnemySigil.AddRange(secondStage);
            }
        }

        abilityHasEnded = false;
    }

    public void TryToEndActiveSigils(CardSlot slot) 
    {
        CardInCombat cardClicked;

        if (slot.playerSlot)
        {
            for (int i = 0; i < activatedActivePlayerSigil.Count; i++)
            {
                
                bool hasToEnd = activatedActivePlayerSigil[i].TryToEndActiveSigil(slot,combatManager);
                if (hasToEnd)
                {
                    if (slot.playerSlot) cardClicked = slot.bench ? combatManager.playerBenchCards[slot.slot] : combatManager.playerCombatCards[slot.slot];
                    else cardClicked = slot.bench ? combatManager.enemyBenchCards[slot.slot] : combatManager.enemyCombatCards[slot.slot];

                    cardClicked.SetActiveSigilStar(activatedActivePlayerSigil[i]);
                    activatedActivePlayerSigil.RemoveAt(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < activatedActiveEnemySigil.Count; i++)
            {
                 bool hasToEnd = activatedActiveEnemySigil[i].TryToEndActiveSigil(slot,combatManager);
                 if (hasToEnd) activatedActiveEnemySigil.RemoveAt(i);
            }
        }
    }
}