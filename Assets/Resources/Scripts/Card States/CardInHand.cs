using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Mathematics;


public class CardInHand : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public Card card;
    public Deck deck;

    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    [HideInInspector]
    public float tiltAngle;
    public bool dontTidy = true;
    public bool discarding = false;
    public Vector3 targetLocation;
    public float targetAngle;
    public Vector3 startPos;
    public float travelTime = 0.5f;

    void Start()
    {
        deck.UpdateCardAppearance(transform, card);

        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        tiltAngle = 0;
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            OnStopDrag();
        }
        if (dontTidy)
        {
            if (!discarding)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetLocation, 1 - travelTime / 0.5f);
                float scale = Mathf.Lerp(0.5f, 1, 1 - travelTime / 0.5f);
                tiltAngle = Mathf.Lerp(0, targetAngle, 1 - travelTime / 0.5f);
                transform.localScale = Vector3.one * scale;
                travelTime -= Time.deltaTime;
                UpdateTilt();
                if (travelTime <= 0)
                {
                    dontTidy = false;
                    deck.TidyHand();
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(startPos, targetLocation, 1 - travelTime / 0.5f);
                float scale = Mathf.Lerp(1f, 0.5f, 1 - travelTime / 0.5f);
                tiltAngle = Mathf.Lerp(targetAngle, 0, 1 - travelTime / 0.5f);
                transform.localScale = Vector3.one * scale;
                travelTime -= Time.deltaTime;
                UpdateTilt();
                if (travelTime <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    #region Dragging functions
    //--------------------------------//
    public void OnDrag(PointerEventData eventData)
    {
        if (deck.selectedCard != transform) deck.selectedCard = transform; 
    }

    public void OnStopDrag()
    {
        CardSlot cardSlot = CheckForSlot();
        
            if (cardSlot != null && cardSlot.playerSlot && deck.energy >= card.cost)
            {
                if(card.name != "LostSoul"){
                    // Trying to play a card in a bench slot
                    if(cardSlot.bench)
                    {
                        if (deck.combatManager.playerBenchCards[cardSlot.slot] == null)
                        {
                            PlayCard(cardSlot);
                            SoundManager.soundManager.Play("CardPlaced");
                        }
                        else if(deck.combatManager.playerBenchCards[cardSlot.slot].GetComponent<CardAcceptor>() != null){
                            CardInCombat consumingCard = deck.combatManager.playerBenchCards[cardSlot.slot].GetComponent<CardInCombat>();
                            consumingCard.card.ActivateOnConsumeEffects(consumingCard, card);
                            ConsumeCard();
                            //Debug.Log(deck.combatManager.playerBenchCards[cardSlot.slot].GetComponent<CardInCombat>().card.name + " accepting " + card.name + " on bench slot");
                        }
                        else if (deck.combatManager.playerCombatCards[cardSlot.slot] == null)
                        {
                            deck.combatManager.playerBenchCards[cardSlot.slot].BenchOrUnbench();
                            PlayCard(cardSlot);
                            SoundManager.soundManager.Play("CardPlaced");
                        }
                    }
                    // Trying to play a card in a combat slot
                    else if (!cardSlot.bench)
                    {
                        if (deck.combatManager.playerCombatCards[cardSlot.slot] == null)
                        {
                            PlayCard(cardSlot);
                            SoundManager.soundManager.Play("CardPlaced");
                        }
                        
                        else if(deck.combatManager.playerCombatCards[cardSlot.slot].GetComponent<CardAcceptor>() != null){
                            CardInCombat consumingCard = deck.combatManager.playerCombatCards[cardSlot.slot].GetComponent<CardInCombat>();
                            consumingCard.card.ActivateOnConsumeEffects(consumingCard, card);
                            ConsumeCard();
                            //Debug.Log(deck.combatManager.playerCombatCards[cardSlot.slot].GetComponent<CardInCombat>().card.name + " accepting " + card.name + " on combat slot");
                        }

                        else if (deck.combatManager.playerBenchCards[cardSlot.slot] == null)
                        {
                            deck.combatManager.playerCombatCards[cardSlot.slot].BenchOrUnbench();
                            PlayCard(cardSlot);
                            SoundManager.soundManager.Play("CardPlaced");
                        }
                    }
                    // Trying to play a card in a bench slot
                    else
                    {
                        if(!GameObject.Find("GameMenu").GetComponent<GameMenu>().aboutToOpenMenu){
                            SoundManager.soundManager.Play("CardRetract");
                        }
                    }
                }
                else
                {
                    if (!GameObject.Find("GameMenu").GetComponent<GameMenu>().aboutToOpenMenu){
                        SoundManager.soundManager.Play("CardRetract");
                    }
                }
            }
            else
            {
                if(!GameObject.Find("GameMenu").GetComponent<GameMenu>().aboutToOpenMenu){
                    SoundManager.soundManager.Play("CardRetract");
                }
            }

        //Check if the card to which this script is attachd is a "Lost Soul"
        //Check if we have selected a card
        //Check if the selected card is a "Lost Soul" 
        if (card.name == "LostSoul" && deck.selectedCard != null && deck.selectedCard.GetComponent<CardInHand>().card.name == "LostSoul")
        {
            PlayLostSoul();
        }
    
        deck.selectedCard = null;
        deck.TidyHand();
    }
    //--------------------------------//
    #endregion
   

    public void PlayLostSoul(){
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            //Set the Pointer Event Position to that of the mouse position
            position = Input.mousePosition
        };

        //Create a list of Raycast Results
        List<RaycastResult> results = new();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            //Check if the card on which we play "Lost Soul" is in combat and if it's a player's card and not an enemy's card and if the card has any injuries
            if(result.gameObject.name == "CardInCombat(Clone)" && result.gameObject.GetComponent<CardInCombat>().playerCard && result.gameObject.GetComponent<CardInCombat>().card.injuries.Count > 0){
                SoundManager.soundManager.Play("LostSoul");
                Card healedCard = result.gameObject.GetComponent<CardInCombat>().card;

                healedCard.AcceptLostSoul();
                deck.UpdateCardAppearance(result.gameObject.transform, healedCard);

                // Visual effect v
                Instantiate(deck.soulHeart, result.gameObject.transform.position, Quaternion.identity);
                
                LostSoulVisuals soulHeart;

                soulHeart = Instantiate(deck.soulHeart, result.gameObject.transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                soulHeart.angle = 120f;
                soulHeart.primaryHeart = false;

                soulHeart = Instantiate(deck.soulHeart, result.gameObject.transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                soulHeart.GetComponent<LostSoulVisuals>().angle = 240f;
                soulHeart.primaryHeart = false;
                //               ^

                deck.cards.Remove(card);
                //GameObject.Find("DeckDisplayManager").GetComponent<DeckDisplay>().cards.Remove(card);

                if (deck.cardsInHand.Contains(gameObject)) deck.cardsInHand.Remove(gameObject);
                if (deck.cardsInHandAsCards.Contains(card)) deck.cardsInHandAsCards.Remove(card);
                Destroy(gameObject);
            }
        }
    }

    //that is for deck.TidyHand()
    public void GetOnTop(Transform card)
    {
        if (deck.hoveredCard == null) 
        {
            deck.hoveredCard = card;
            deck.TidyHand();
        }
    }

    public void Unselect(Transform card) 
    {
        if (card == deck.hoveredCard)
        {
            deck.hoveredCard.localScale = new Vector2(1,1);
            deck.hoveredCard = null;
            deck.TidyHand();
        }
    }
    //that is for deck.TidyHand()

    CardSlot CheckForSlot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.forward, 100);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "BenchSlot")
            {
                return hit.transform.GetComponent<CardSlot>();
            }
        }
        return null;
    }

    public void PlayCard(CardSlot slot)
    {
        if (deck.combatManager.gamePhase == 1) return;

        if (!deck.hasCaptain) 
        {
            card.captain = true;
            deck.hasCaptain = true;
        }
        

        GameObject cardToCreate = Instantiate(deck.cardInCombatPrefab, slot.transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = deck;
        cardInCombat.slot = slot.slot;

        deck.energy -= card.cost;
        if (slot.bench)
        {
            deck.combatManager.playerBenchCards[slot.slot] = cardInCombat;
            cardInCombat.benched = true;
        }
        else
        {
            deck.combatManager.playerCombatCards[slot.slot] = cardInCombat;
            cardInCombat.benched = false;
        }

        

        //maha go ot deck.cardsInHand
        deck.cardsInHand.Remove(gameObject);
        deck.cardsInHandAsCards.Remove(card);
        Destroy(gameObject);
    }

    public void ConsumeCard(){
        deck.energy -= card.cost;

        deck.cardsInHand.Remove(gameObject);
        deck.cardsInHandAsCards.Remove(card);
        Destroy(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundManager.soundManager.Play("CardPickUp");
    }

    public void UpdateTilt(){
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }

    public void UpdateCostText()
    {
        if (deck.energy < card.cost)
        {
            transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = new Color(0.75f, 0, 0, 1);
        }
        else
        {
            transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 1);
        }
    }

}
