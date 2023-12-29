using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEditor.U2D.Common;
using System;

public class ActiveAbilityManager : MonoBehaviour
{
    public static ActiveAbilityManager activeAbilityManager;
    public Transform selectedCardSpot;
    public CardInCombat selectedCard;
    Transform sigilMenu;
    public GameObject abilityInterface;
    public TextMeshProUGUI namePlate;

    public List<CardSlot> targetableSlots;
    List<CardSlot> selectedSlots  = new List<CardSlot>();
    ActiveSigil selectedSigil;

    float activateBenchingTimer;
    public bool cardsCanBench = true;

    float vignetteAlphaTimer = 0;
    public Image vignette;

    void Start()
    {
        activeAbilityManager = this;
        sigilMenu = abilityInterface.transform.GetChild(0);
    }

    void Update(){
        if (activateBenchingTimer > 0f){
            activateBenchingTimer -= Time.deltaTime;
        }else{
            cardsCanBench = true;
        }

        if (selectedCard != null){
            activateBenchingTimer = 0.5f;
            cardsCanBench = false;


            if (vignetteAlphaTimer < 1f) vignetteAlphaTimer += Time.deltaTime;

            if (selectedSigil.canBeUsed){
                if (Input.GetMouseButtonDown(0)){
                    CardSlot newTarget = CheckForSlot();
                    if (newTarget != null){
                        
                        bool canBeTargeted = false;
                        for (int i = 0; i < targetableSlots.Count; i++){
                            if (targetableSlots[i] == newTarget){
                                canBeTargeted = true;
                            }
                        }

                        if (canBeTargeted){
                            selectedSlots.Add(newTarget);
                            HighlightSelectedSlots();
                            if (selectedSlots.Count >= selectedSigil.neededTargets){
                                UseSigil();
                            }
                        }
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)){
                Deselect();
            }
        }else{
            if (vignetteAlphaTimer > 0f) vignetteAlphaTimer -= Time.deltaTime;
        }

        vignetteAlphaTimer = Math.Clamp(vignetteAlphaTimer, 0f, 1f);
        vignette.color = new Color(1, 1, 1, vignetteAlphaTimer);
    }
    public void SelectCard(CardInCombat card){
        if (selectedCard != null){
            Vector3 slot;
            if (selectedCard.playerCard){
                if (selectedCard.benched) slot = CombatManager.combatManager.playerBenchSlots [selectedCard.slot].transform.position;
                else                      slot = CombatManager.combatManager.playerCombatSlots[selectedCard.slot].transform.position;
            }else{
                if (selectedCard.benched) slot = CombatManager.combatManager.enemyBenchSlots  [selectedCard.slot].transform.position;
                else                      slot = CombatManager.combatManager.enemyCombatSlots [selectedCard.slot].transform.position;
            }
            selectedCard.MoveAnimationStarter(0.5f, slot, false, 0f); 
        }
        selectedCard = card;
        card.MoveAnimationStarter(0.5f, selectedCardSpot.transform.position, false, 0);
        abilityInterface.SetActive(true);

        cardsCanBench = false;

        SetupSigilIcons(GetUsableAbility());
    }
    public void Deselect(){
        // This is just an exit function
        if (selectedCard == null) return;

        Vector3 slot;
        if (selectedCard.playerCard){
            if (selectedCard.benched) slot = CombatManager.combatManager.playerBenchSlots [selectedCard.slot].transform.position;
            else                      slot = CombatManager.combatManager.playerCombatSlots[selectedCard.slot].transform.position;
        }else{
            if (selectedCard.benched) slot = CombatManager.combatManager.enemyBenchSlots  [selectedCard.slot].transform.position;
            else                      slot = CombatManager.combatManager.enemyCombatSlots [selectedCard.slot].transform.position;
        }
        selectedCard.MoveAnimationStarter(0.5f, slot, false, 0f);
        selectedCard.ShowSigilStars();
        selectedCard = null;
        selectedSigil = null;

        abilityInterface.SetActive(false);

        RemoveHighlight();
    }
    void SetupSigilIcons(int selectedSigilIndex){
        selectedCard.ShowSigilStars();
        int indexToPlaceSigilAt = 0;
        for (int i = 0; i < 3; i++){
            // Loop through all 3 sigils of a card to find the active ones

            // Deactivate the buttons
            sigilMenu.GetChild(i).gameObject.SetActive(false);
            if (selectedCard.card.sigils.Count > i){
                Sigil sigil = selectedCard.card.sigils[i] ;
                if (sigil.GetActiveSigil() != null){
                    // When a sigil is found dedicate a button to it
                    GameObject button = sigilMenu.GetChild(indexToPlaceSigilAt).gameObject;
                    button.transform.GetChild(0).GetComponent<Image>().sprite = sigil.image;
                    button.SetActive(true);

                    // Increase the button index
                    indexToPlaceSigilAt++;

                    // If this is the selected sigil mark it and highlight targetable slots
                    if (i == selectedSigilIndex){ 
                        button.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);

                        selectedSigil = sigil.GetActiveSigil();
                        namePlate.text = selectedSigil.name;

                        selectedSlots.Clear();

                        targetableSlots = selectedSigil.GetPossibleTargets(selectedCard);
                        HighlightTargetableSlots();

                        selectedCard.SetActiveSigilStar(sigil);
                    }
                    else{
                        // If its not the selected sigil remove the selector
                        button.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }
                    continue;
                }
            }
        }
    }
    void RemoveHighlight(){
        // Turns on the red border on targetable slots
        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            // Turn off all slots
            CombatManager.combatManager.playerCombatSlots[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color (0, 0, 0, 0);
            CombatManager.combatManager.playerBenchSlots [i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color (0, 0, 0, 0);
            CombatManager.combatManager.enemyCombatSlots [i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color (0, 0, 0, 0);
            CombatManager.combatManager.enemyBenchSlots  [i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color (0, 0, 0, 0);
        }
    }
    void HighlightTargetableSlots(){
        RemoveHighlight();
        foreach (CardSlot slot in targetableSlots){
            // Loop through all targetable slots and activate their borders
            slot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    void HighlightSelectedSlots(){
        foreach (CardSlot slot in selectedSlots){
            // Loop through all selected slots and activate their borders
            slot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
    public void SigilButton(int index){
        // Used by the UI buttons
        SetupSigilIcons(index);
    }
    public void UseSigil(){
        // Activate the sigil
        selectedSigil.ActiveEffect(selectedCard, selectedSlots);
        selectedCard.ShowSigilStars();
        Deselect();
        Debug.Log("Slots: " + selectedSlots.Count);
    }
    CardSlot CheckForSlot()
    {
        // Raycast down to find if there are suitable slots
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.forward, 100);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "BenchSlot")
            {
                return hit.transform.GetComponent<CardSlot>();
            }
        }
        return null;
    }

    int GetUsableAbility(){
        for (int i = 0; i < selectedCard.card.sigils.Count; i++){
            ActiveSigil activeSigil = selectedCard.card.sigils[i].GetActiveSigil();
            if (activeSigil != null && activeSigil.canBeUsed){
                return i;
            }
        }
        return 0;
    }
}
