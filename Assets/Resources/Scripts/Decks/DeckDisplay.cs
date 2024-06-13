using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplay : MonoBehaviour
{
    public GameObject cardDisplayPrefab;
    public List<Card> cards = new List<Card>();
    [Space(10)]
    [Header("Display name")]
    public TextMeshProUGUI deckName;
    public GameObject nameplate;
    
    RectTransform deckHolder;
    
    [Space(10)]
    [Header("Positioning")]
    public float width;
    public float height;

    public float cardOffset;
    public float cardWidth;

    public float cardHeight;

    public Vector2 point;
    int maxScrollLines = 0;
    float currentScroll = 0;
    public bool placedInPrefab;
    public float widthModifier;
    public float heightModifier;
    Canvas canvas;
    CanvasScaler canvasScaler;

    void Start(){    
        // Setup variables
        deckHolder = transform.GetChild(0).GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();
        DeckUtilities.AddDisplay(this);

        canvasScaler.referenceResolution = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        // Show the display
        ShowDisplay();
    }
    void Update(){
        // Check if the player is trying to scroll
        Scroll();
        
    }
    public void ShowDisplay(){
        // Show the display
        AnimationUtilities.ChangeCanvasAlpha(transform, 0.5f, 0, 1);

        if (!placedInPrefab){
            // If the display is created via code ajust it
            ChangePosition(point);
            RepositionNameplate(new Vector3(point.x, point.y + height/2, 0));
        }else{
            widthModifier = Camera.main.pixelWidth/canvas.pixelRect.width;
            heightModifier = Camera.main.pixelHeight/canvas.pixelRect.height;

            width  = deckHolder.rect.width * widthModifier;
            height = deckHolder.rect.height * heightModifier;
            point = deckHolder.rect.center;
            ShowCards();
            ChangePosition(point);
        }
    }
    public void ChangePosition(Vector2 centerPoint){
        // Move and resize the display
        float botOffset  = Camera.main.pixelHeight/2 + centerPoint.y - height/2;
        float leftOffset = Camera.main.pixelWidth /2 + centerPoint.x - width /2;

        deckHolder.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, botOffset , height);
        deckHolder.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left  , leftOffset, width );

        // After the ajustmets are done cards have to be replaced with new ones to fit properly
        ShowCards();
    }

    public void CloseDisplay(){
        // Destroy this display
        AnimationUtilities.ChangeCanvasAlpha(transform, 0.5f, 0, 0);
        AnimationUtilities.DestroyAfter(transform, 0.6f);
        DeckUtilities.deckUtilities.activeDisplays.Remove(this);
    }

    public void ClearCards(){
        // Clear the card displays of this display
        for (int i = 0; i < deckHolder.childCount; i++){
            Destroy(deckHolder.GetChild(i).gameObject);
        }
    }

    public void ShowCards(){
       ShowCards(cards);
    }
    public void ShowCards(List<Card> newCards){
        // Sort the array
        SortList();

        // Updates the cards shown
        ClearCards();
        int cardsPerLine = Mathf.RoundToInt(width/(cardWidth + cardOffset));
        int pixelsPerCard = Mathf.RoundToInt((width - 0.5f * (cardWidth - cardOffset))/cardsPerLine);
        int lines = Mathf.CeilToInt(newCards.Count/(float)cardsPerLine);
        int maxLines = Mathf.FloorToInt(height/Mathf.RoundToInt(cardHeight));

        // Calculate if scrolling is needed
        if (lines > maxLines){
            maxScrollLines = lines - maxLines;
        }

        // Spawn the cards
        for (int i = 0; i < newCards.Count; i++){
            // Calculate where they have to be placed
            float cardX = (i % cardsPerLine + 0.5f) * pixelsPerCard;
            float cardY = height - i/cardsPerLine * cardHeight - 175 + currentScroll;

            // Instantiate them 
            Transform newDisplay = Instantiate(cardDisplayPrefab, deckHolder).transform;
            newDisplay.localScale = Vector3.one;
            newDisplay.localPosition = new Vector3(cardX, cardY, 0);
            newDisplay.GetComponent<CardDisplay>().card = newCards[i];
        }
    }

    public void RepositionNameplate(Vector3 position){
        if (deckName.text == ""){
            nameplate.gameObject.SetActive(false);
        }else{
            nameplate.transform.localPosition = position;
        }
    }

    void SortList(){
        // Sort the list of cards with DIRECT INSERTION
        for (int x = 0; x < cards.Count; x++){
            for (int y = x; y < cards.Count; y++){
                if (cards[x].cost > cards[y].cost){
                    Card store = cards[x];
                    cards[x] = cards[y];
                    cards[y] = store;
                }
                if (cards[x].cost == cards[y].cost){
                    if (cards[x].name[0] > cards[y].name[0]){
                        Card store = cards[x];
                        cards[x] = cards[y];
                        cards[y] = store;
                    }
                }
            }
        }
    }
    public void Scroll(){
        float scrollBy = 0;
        // Scrolling
        if (Input.GetAxis("Mouse ScrollWheel") != 0f){
            // See how much it has to scroll
            scrollBy = -Input.GetAxis("Mouse ScrollWheel") * 350;
        }
        
        // Calculate if it can scroll
        float oldScroll = currentScroll;
        currentScroll += scrollBy;
        currentScroll = Mathf.Clamp(currentScroll, 0, maxScrollLines * cardHeight);
        scrollBy = currentScroll - oldScroll;

        // Ajust all cards
        for (int i = 0; i < deckHolder.childCount; i++){
            Transform cardToMove = deckHolder.GetChild(i).transform;
            cardToMove.localPosition += new Vector3(0, scrollBy, 0);
        }
    }
}
