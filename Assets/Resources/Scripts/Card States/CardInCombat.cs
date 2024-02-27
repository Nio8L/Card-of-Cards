using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInCombat : MonoBehaviour
{
    public bool benched = true;

    public Card card;
    public Deck deck;
    public Card.TypeOfDamage lastTypeOfDamage;

    [HideInInspector]
    public bool passivesTurnedOnThisTurn = false;
    [HideInInspector]

    public bool canBeBenched = true;
    public bool playerCard = true;
    public int slot = 0;
    public int direction = 1;
    public float animationStartDelay;

    GameObject bloodSplat;
    GameObject deathMark;
    bool rightClickedRecently;
    float rightClickTimer;
    void Start()
    {
        card.ActivateOnSummonEffects(this);
        deck.UpdateCardAppearance(transform, card);
        bloodSplat = Resources.Load<GameObject>("Prefabs/Particles/BloodSplatPart");
        deathMark = Resources.Load<GameObject>("Prefabs/DeathMark");
    }
    private void Update()
    {

        if (card.health <= 0 && !AnimationUtilities.CheckForAnimation(gameObject))
        {
            OnDeath();
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        
        // Check if benching should be enabled again
        if (rightClickTimer <= 0){
            // Enable benching if the 0.1 second timer since last right click has passed
            rightClickedRecently = false;
        }else{
            // Count down
            rightClickTimer -= Time.deltaTime;
        }
        // Check if the right button of the mouse has been clicked
        if (Input.GetMouseButton(1)){
            // Disable card benching for a bit to avoid considering right clicks as left ones
            rightClickedRecently = true;
            rightClickTimer = 0.1f;
        }

        
    }
    public void CallBenchOrUnbench(){
        // Unity event trigger crashes if BenchOrUnbench gets called directly...
        BenchOrUnbench(true, false);
    }
    public bool BenchOrUnbench(bool playerInput, bool force) 
    {
        /*  
            Tries to bench or unbench cards.
            Returns true if it succedess
            Returns false if it fails
        */ 
        
        // Return if this card can't be benched (currently not used)
        if (!canBeBenched) return false;
        
        // Benching
        if (playerCard && ((CombatManager.combatManager.gamePhase == 0 && playerInput) || force)){
            // Player card
            if (rightClickedRecently || !ActiveAbilityManager.activeAbilityManager.cardsCanBench) return false;
            
            benched = !benched;
            SoundManager.soundManager.Play("CardSlide");
            if (benched)
            {
                // Benched -> Combat
                CombatManager.combatManager.playerCombatCards[slot] = CombatManager.combatManager.playerBenchCards[slot];
                CombatManager.combatManager.playerBenchCards[slot] = this;
                if (CombatManager.combatManager.playerCombatCards[slot] != null)
                {
                    CombatManager.combatManager.playerCombatCards[slot].benched = !benched;
                    CombatManager.combatManager.playerCombatCards[slot].MoveAnimationStarter(0.5f, CombatManager.combatManager.playerCombatSlots[slot].transform.position, false, 0f);
                }
                MoveAnimationStarter(0.5f, CombatManager.combatManager.playerBenchSlots[slot].transform.position, false, 0f);
                
            }
            else
            {
                // Combat -> Benched
                CombatManager.combatManager.playerBenchCards[slot] = CombatManager.combatManager.playerCombatCards[slot];
                CombatManager.combatManager.playerCombatCards[slot] = this;
                if (CombatManager.combatManager.playerBenchCards[slot] != null)
                {
                    CombatManager.combatManager.playerBenchCards[slot].benched = !benched;
                    CombatManager.combatManager.playerBenchCards[slot].MoveAnimationStarter(0.5f, CombatManager.combatManager.playerBenchSlots[slot].transform.position, false, 0f);
                }
                MoveAnimationStarter(0.5f, CombatManager.combatManager.playerCombatSlots[slot].transform.position, false, 0f);
            }
            return true;
        }else if (!playerCard && (!playerInput || force)){
            // Enemy card
            benched = !benched;
            SoundManager.soundManager.Play("CardSlide");
            if (benched)
            {
                // Benched -> Combat
                CombatManager.combatManager.enemyCombatCards[slot] = CombatManager.combatManager.enemyBenchCards[slot];
                CombatManager.combatManager.enemyBenchCards[slot] = this;
                if (CombatManager.combatManager.enemyCombatCards[slot] != null)
                {
                    CombatManager.combatManager.enemyCombatCards[slot].benched = !benched;
                    CombatManager.combatManager.enemyCombatCards[slot].MoveAnimationStarter(0.5f, CombatManager.combatManager.enemyCombatSlots[slot].transform.position, false, 0f);
                }
                MoveAnimationStarter(0.5f, CombatManager.combatManager.enemyBenchSlots[slot].transform.position, false, 0f);
            }
            else
            {
                // Combat -> Benched
                CombatManager.combatManager.enemyBenchCards[slot] = CombatManager.combatManager.enemyCombatCards[slot];
                CombatManager.combatManager.enemyCombatCards[slot] = this;
                if (CombatManager.combatManager.enemyBenchCards[slot] != null)
                {
                    CombatManager.combatManager.enemyBenchCards[slot].benched = !benched;
                    CombatManager.combatManager.enemyBenchCards[slot].MoveAnimationStarter(0.5f, CombatManager.combatManager.enemyBenchSlots[slot].transform.position, false, 0f);
                }
                MoveAnimationStarter(0.5f, CombatManager.combatManager.enemyCombatSlots[slot].transform.position, false, 0f);
            }
            return true;
        }
        return false;   
    }
    public void MoveAnimationStarter(float time, Vector3 end, bool returnMove, float startDelay)
    {
        //AnimationUtilities.CancelAnimations(gameObject);
        Invoke("CallUpdateCardAppearance", time + startDelay);
        if (!returnMove){
            AnimationUtilities.MoveToPoint(transform, time, startDelay, end);
        }else{
            AnimationUtilities.ReturnMoveToPoint(transform, time, startDelay, GetSlot().transform.position, end);
            Invoke("CallUpdateCardAppearance", (time+startDelay)/2);
            Invoke("PlayCombatSound", (time+startDelay)/1.25f);
        }
    }
    public void OnDeath()
    {
        if (GetComponent<DestroyTimer>().enabled == false)
        {
            card.ActivateOnDeadEffects(this);

            if (card.health > 0) return;
            
            card.CreateCard(lastTypeOfDamage);

            if (playerCard)
            {
                RemoveCardFromCardCollections();
                if (card.canRevive) {
                    deck.discardPile.Add(card);
                }else{
                    EventManager.CardDeath?.Invoke(card);
                }
            }
            else
            {
                RemoveCardFromCardCollections();
                if (!CombatManager.combatManager.enemy.huntAI)
                {
                    if (card.canRevive) {
                        CombatManager.combatManager.enemyDeck.discardPile.Add(card);
                    }else{
                        EventManager.CardDeath?.Invoke(card);
                    }
                }
                else if (CombatManager.combatManager.battleReward.Count < 3)
                {
                    if (card.canRevive) {
                        CombatManager.combatManager.battleReward.Add(card);
                    }else{
                        EventManager.CardDeath?.Invoke(card);
                    }
                }

            }

            Sprite markSprite;
            if (lastTypeOfDamage == Card.TypeOfDamage.Scratch) markSprite = deck.deathMarkScratch;
            else if (lastTypeOfDamage == Card.TypeOfDamage.Bite) markSprite = deck.deathMarkBite;
            else markSprite = deck.deathMarkPoison;

            Instantiate(bloodSplat, transform.position, Quaternion.identity);
            GameObject deathMarkObject = Instantiate(deathMark, transform.position, Quaternion.identity);
            deathMarkObject.GetComponent<SpriteRenderer>().sprite = markSprite;
            GetComponent<DestroyTimer>().enabled = true;

            AnimationUtilities.CancelAnimations(gameObject);
        }
    }
    private void OnDestroy() {
        SoundManager.soundManager.Play("CardDeath");
    }
    public void ShowSigilStars()
    {
        for (int i = 0; i < card.sigils.Count; i++){
            Sigil sigil = card.sigils[i];
            int alpha = 0;
            ActiveSigil activeSigil = sigil.GetActiveSigil();
            if (activeSigil != null && activeSigil.canBeUsed) alpha = 1;

            if (card.sigils[0] == sigil)
            {
                transform.GetChild(13).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
                transform.GetChild(13).GetComponent<Image>().sprite = deck.activeStar;
            }
            else if (card.sigils[1] == sigil)
            {
                transform.GetChild(14).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
                transform.GetChild(14).GetComponent<Image>().sprite = deck.activeStar;
            }
            else
            {
                transform.GetChild(15).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
                transform.GetChild(15).GetComponent<Image>().sprite = deck.activeStar;
            }
        }
    }
    public void SetActiveSigilStar(Sigil sigil)
    {
        Sprite spriteToUse = deck.activeStar;
        ActiveSigil activeSigil = sigil.GetActiveSigil();
        if (activeSigil != null && activeSigil.canBeUsed) spriteToUse = deck.selectedActiveStar;
        else                                              spriteToUse = deck.activeStar;

        if (card.sigils[0] == sigil)
        {
            transform.GetChild(13).GetComponent<Image>().sprite = spriteToUse;
        }
        else if (card.sigils[1] == sigil)
        {
            transform.GetChild(14).GetComponent<Image>().sprite = spriteToUse;
        }
        else
        {
            transform.GetChild(15).GetComponent<Image>().sprite = spriteToUse;
        }
    }
    public void RemoveCardFromCardCollections() 
    {
        if (benched)
        {
            if(playerCard && CombatManager.combatManager.playerBenchCards[slot] == this) CombatManager.combatManager.playerBenchCards[slot] = null;
            else if (CombatManager.combatManager.enemyBenchCards[slot] == this) CombatManager.combatManager.enemyBenchCards[slot] = null;
        }
        else
        {
            if (playerCard && CombatManager.combatManager.playerCombatCards[slot] == this) CombatManager.combatManager.playerCombatCards[slot] = null;
            else if (CombatManager.combatManager.enemyCombatCards[slot] == this) CombatManager.combatManager.enemyCombatCards[slot] = null;
        }
    }
    public void ForceKill()
    {
        
        if (GetComponent<DestroyTimer>().enabled == false)
        {
            RemoveCardFromCardCollections();
            card.CreateCard(lastTypeOfDamage);

            if (playerCard)
            {
                if (card.canRevive) {
                    deck.discardPile.Add(card);
                }else{
                    EventManager.CardDeath?.Invoke(card);
                }
            }
            else
            {
                if (!CombatManager.combatManager.enemy.huntAI)
                {
                    if (card.canRevive) {
                        CombatManager.combatManager.enemyDeck.discardPile.Add(card);
                    }else{
                        EventManager.CardDeath?.Invoke(card);
                    }
                }
                else if (CombatManager.combatManager.battleReward.Count < 3)
                {
                    if (card.canRevive) {
                        CombatManager.combatManager.battleReward.Add(card);
                    }else{
                        EventManager.CardDeath?.Invoke(card);
                    }
                }

            }

            Sprite markSprite;
            if (lastTypeOfDamage == Card.TypeOfDamage.Scratch) markSprite = deck.deathMarkScratch;
            else if (lastTypeOfDamage == Card.TypeOfDamage.Bite) markSprite = deck.deathMarkBite;
            else markSprite = deck.deathMarkPoison;

            Instantiate(bloodSplat, transform.position, Quaternion.identity);
            GameObject deathMarkObject = Instantiate(deathMark, transform.position, Quaternion.identity);
            deathMarkObject.GetComponent<SpriteRenderer>().sprite = markSprite;
            GetComponent<DestroyTimer>().enabled = true;
        }
    }
    public void TryToSelectForActiveAbility(){
        if (!playerCard || !rightClickedRecently || CombatManager.combatManager.gamePhase != 0) return;

        for (int i = 0; i < card.sigils.Count; i++){
            if (card.sigils[i].GetActiveSigil() != null){
                if (ActiveAbilityManager.activeAbilityManager.selectedCard == this) ActiveAbilityManager.activeAbilityManager.Deselect();
                else ActiveAbilityManager.activeAbilityManager.SelectCard(this);
                break;
            }
        }
    }
    public CardSlot GetSlot(){
        if (playerCard){
            if (benched) return CombatManager.combatManager.playerBenchSlots [slot].GetComponent<CardSlot>();
            else         return CombatManager.combatManager.playerCombatSlots[slot].GetComponent<CardSlot>();
        }else{
            if (benched) return CombatManager.combatManager.enemyBenchSlots  [slot].GetComponent<CardSlot>();
            else         return CombatManager.combatManager.enemyCombatSlots [slot].GetComponent<CardSlot>();
        }
    }
    public void PlayCombatSound(){
        SoundManager.soundManager.Play("CardCombat");
    }
    void CallUpdateCardAppearance(){
        deck.UpdateCardAppearance(transform, card);
    }
}
