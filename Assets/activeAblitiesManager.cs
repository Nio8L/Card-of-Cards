using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeAblitiesManager : MonoBehaviour
{
    Sigil activatedActivePlayerSigil;
    Sigil activatedActiveEnemySigil;

    public bool holding = false;

    public CombatManager combatManager;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(activatedActivePlayerSigil);
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
        CardInCombat cardClicked;

        if (slot.playerSlot) cardClicked = slot.bench ? combatManager.playerBenchCards[slot.slot] : combatManager.playerCombatCards[slot.slot];
        else cardClicked = slot.bench ? combatManager.enemyBenchCards[slot.slot] : combatManager.enemyCombatCards[slot.slot];

        if (cardClicked != null)
        {
            Sigil secondStage = cardClicked.card.ActivateActiveSigilStartEffects(cardClicked);
            if (secondStage != null)
            {
                if (cardClicked.playerCard) activatedActivePlayerSigil = secondStage;
                else activatedActiveEnemySigil = secondStage;
            }
        }

        TryToEndActiveSigils(slot);
    }

    public void TryToEndActiveSigils(CardSlot slot) 
    {
        if (slot.playerSlot && activatedActivePlayerSigil != null)
        {
            bool hasToEnd = activatedActivePlayerSigil.TryToEndActiveSigil(slot,combatManager);
            if (hasToEnd) activatedActivePlayerSigil = null;
        }
        else if (!slot.playerSlot && activatedActiveEnemySigil != null)
        {
            bool hasToEnd = activatedActiveEnemySigil.TryToEndActiveSigil(slot,combatManager);
            if (hasToEnd) activatedActiveEnemySigil = null;
        }
    }
}
