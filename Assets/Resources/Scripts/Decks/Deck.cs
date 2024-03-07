using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;


public class Deck : MonoBehaviour, IDataPersistence
{
    public bool playerDeck = false;

    public int energy = 3;
    public List<Card> cardsToBeAdded = new();
    public List<Card> cards = new();
    public List<Card> drawPile;
    public List<Card> discardPile;
    public List<GameObject> cardsInHand = new();
    public List<Card> cardsInHandAsCards = new();

    [HideInInspector]
    GameObject cardInHandPrefab;
    [HideInInspector]
    public GameObject cardInCombatPrefab;
    [HideInInspector]
    public GameObject cardInBenchPrefab;
    [HideInInspector]
    public GameObject soulHeart;

    public Transform CardsInHandParent;
    public Transform CardsInCombatParent;

    [HideInInspector]
    public Transform selectedCard;
    TextMeshProUGUI energyText;

    public Vector2 centerPointForcardsInHand;
    public float spaceBetweenCardsInHand;
    [HideInInspector]
    public Transform hoveredCard;
   
    [Header("Card Piles")]
    public float spaceForHoveredCard;
    public TextMeshProUGUI drawPileText;
    public TextMeshProUGUI DiscardPileText;

    int drawLeft = 0;
    float drawTime = 0.2f;
    float drawT = 0.1f;

    [Header("Notifications")]
    public Notification noEnergyNotification;

    #region Saving
    //--------------------------------//
    public void LoadData(GameData data){
        if (!playerDeck) return;

        cards.Clear();
        for(int i = 0; i < data.cardNames.Count; i++){
            Card newCard = ScriptableObject.CreateInstance<Card>();

            AddCard(newCard);
            cards[^1].name = data.cardNames[i];
            cards[^1].defaultAttack = data.cardAttacks[i];
            cards[^1].attack = cards[^1].defaultAttack;
            cards[^1].maxHealth = data.cardMaxHealths[i];
            cards[^1].health = cards[^1].maxHealth;
            cards[^1].cost = data.cardCosts[i];
            cards[^1].image = Resources.Load<Sprite>("Sprites/Creatures/" + data.cardImages[i]);

            for(int j = 0; j < data.cardSigils[i].list.Count; j++){
                Sigil originalSigil = Resources.Load<Sigil>("Sigils/" + data.cardSigils[i].list[j]);
                Sigil sigilToAdd = Instantiate(originalSigil);
                sigilToAdd.name = originalSigil.name;
                cards[^1].sigils.Add(sigilToAdd);
            }
            cards[^1].typeOfDamage = (Card.TypeOfDamage) Enum.Parse(typeof(Card.TypeOfDamage), data.cardDamageType[i]);

            for(int j = 0; j < data.cardInjuries[i].list.Count; j++){
                cards[^1].injuries.Add((Card.TypeOfDamage) Enum.Parse(typeof(Card.TypeOfDamage), data.cardInjuries[i].list[j]));
            }
        }
    }

    public void SaveData(GameData data){
        if (!playerDeck) return;
        
        data.cardNames.Clear();
        data.cardAttacks.Clear();
        //data.cardHealths.Clear();
        data.cardMaxHealths.Clear();
        data.cardCosts.Clear();
        data.cardImages.Clear();
        
        data.cardSigils.Clear();
        
        data.cardInjuries.Clear();

        data.cardDamageType.Clear();

        for(int i = 0; i < cards.Count; i++){
            data.cardNames.Add(cards[i].name);
            data.cardAttacks.Add(cards[i].defaultAttack);
            data.cardMaxHealths.Add(cards[i].maxHealth);
            data.cardCosts.Add(cards[i].cost);
            data.cardImages.Add(cards[i].image.name);

            data.cardDamageType.Add(cards[i].typeOfDamage.ToString());

            data.cardSigils.Add(new ListWrapper());

            data.cardInjuries.Add(new ListWrapper());

            for(int j = 0; j < cards[i].injuries.Count; j++){
                data.cardInjuries[i].list.Add(cards[i].injuries[j].ToString());
            }

            for(int j = 0; j < cards[i].sigils.Count; j++){
                string sigilName = cards[i].sigils[j].name;
                data.cardSigils[i].list.Add(sigilName);
            }
        }
    }
    //--------------------------------//
    #endregion

    private void Start()
    {
        cardInHandPrefab = Resources.Load<GameObject>("Prefabs/CardPrefab/CardInHand");
        cardInCombatPrefab = Resources.Load<GameObject>("Prefabs/CardPrefab/CardInCombat");

        soulHeart = Resources.Load<GameObject>("Prefabs/LostSoulHeart");

        CardsInHandParent = GameObject.Find("CardsInHand").transform;
        CardsInCombatParent = GameObject.Find("CardsInCombat").transform;

        energyText = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        drawPile = CopyCardList(cards);

        if (playerDeck)
        {
            if (ScenePersistenceManager.scenePersistence.playerDeck.Count > 0 && ScenePersistenceManager.scenePersistence.inTutorial)
            {
                cardsToBeAdded = CopyCardList(ScenePersistenceManager.scenePersistence.playerDeck);
                ScenePersistenceManager.scenePersistence.playerDeck.Clear();
                AddCard(cardsToBeAdded.Count);
            }
           
        }

        for(int i = 0; i < cards.Count; i++){
            cards[i].playerCard = playerDeck;
        }
    }

    private void Update()
    {
        if (selectedCard != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCard.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }

        if (playerDeck){
            energyText.text = energy + "/3";
        }

        if (drawLeft > 0)
        {
            drawT -= Time.deltaTime;
            if (drawT <= 0f)
            {
                drawT = drawTime;
                drawLeft--;
                DrawCard(); 
            }
        }
    }

    #region Deck Functions
    //--------------------------------//
    public List<Card> CopyCardList(List<Card> listToCopy) 
    {
        List<Card> returnList = new List<Card>();
        foreach (Card card in listToCopy) returnList.Add(card);
        return returnList;
    }

    public void AddCard(Card card){
        Card newCard = Instantiate(card).ResetCard();
        newCard.name = card.name;
        
        drawPile.Add(newCard);
        cards.Add(newCard);

        PrintDeck();
        CombatManager.combatManager.combatUI.UpdatePileNumbers();
    }

    public void AddCard(int numOfCards)
    {
        for (int i = 0; i < numOfCards; i++) 
        {
            AddCard(cardsToBeAdded[0]);
            cardsToBeAdded.RemoveAt(0);
        }
    }
    public void RemoveCard(Card card){
        drawPile.Remove(card);
        cards.Remove(card);
        CombatManager.combatManager.combatUI.UpdatePileNumbers();
    }
    public void RemoveCard(){
        drawPile.RemoveAt(drawPile.Count - 1);
        cards.RemoveAt(cards.Count - 1);
        PrintDeck();
        CombatManager.combatManager.combatUI.UpdatePileNumbers();
    }
    public void PrintDeck(){
        string cardsInDeck = "";
        foreach (Card card in cards)
        {
            cardsInDeck += card.name;
            cardsInDeck += ", ";
        }
        cardsInDeck = "";
        foreach (Card card in drawPile)
        {
            cardsInDeck += card.name;
            cardsInDeck += ", ";
        }
    }
    public void TidyHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            if (cardsInHand[i] != null){
                CardInHand targetCard = cardsInHand[i].GetComponent<CardInHand>();
                targetCard.UpdateCostText();
                if (targetCard.dontTidy)
                {
                    targetCard.targetLocation = new Vector2(centerPointForcardsInHand.x - (0.5f * cardsInHand.Count - 0.5f) * (spaceBetweenCardsInHand / (cardsInHand.Count / 2f)) + i * (spaceBetweenCardsInHand / (cardsInHand.Count / 2f)), centerPointForcardsInHand.y);
                    targetCard.targetAngle = (cardsInHand.Count / 2 - i) * 10;
                    continue;
                }
                
                //Tilt cards so they form //|\\
                cardsInHand[i].transform.localPosition = new Vector2(centerPointForcardsInHand.x - (0.5f * cardsInHand.Count - 0.5f) * (spaceBetweenCardsInHand / (cardsInHand.Count / 2f)) + i * (spaceBetweenCardsInHand / (cardsInHand.Count / 2f)), centerPointForcardsInHand.y);
                targetCard.tiltAngle = (cardsInHand.Count / 2 - i) * 10;
                targetCard.UpdateTilt(targetCard.tiltAngle);
            }
        }

        //Update tilt so there are 2 center cards if there is an even amount of cards in hand
        if(cardsInHand.Count % 2 == 0){
            for(int i = 0; i < cardsInHand.Count / 2; i++){
                CardInHand cardInHand = cardsInHand[i].GetComponent<CardInHand>();
                cardInHand.tiltAngle -= 10;
                cardInHand.UpdateTilt(cardInHand.tiltAngle);
            }
        }

        foreach (GameObject card in cardsInHand)
        {
           if(card != hoveredCard) card.transform.SetAsLastSibling();
        }

        //Take the cards on the right of the center card and order them so they form a "staircase" 
        for(int i = cardsInHand.Count - 1; i > cardsInHand.Count / 2; i--){
            cardsInHand[i].transform.SetSiblingIndex(cardsInHand[i].transform.GetSiblingIndex() - i);
        }
        
        if (hoveredCard != null) 
        {
            hoveredCard.SetAsLastSibling();
            hoveredCard.localScale = new Vector2(1.3f,1.3f);
            hoveredCard.position = new Vector2(hoveredCard.position.x, hoveredCard.position.y+spaceForHoveredCard);
            hoveredCard.rotation = Quaternion.Euler(0, 0, 0);
        }
        
    }

    // Drawing cards  
    public void DrawCard()
    {
        if (drawPile.Count == 0)
        {
            if (discardPile.Count != 0)
            {
                drawPile.AddRange(discardPile);
                discardPile.Clear();
                Shuffle();
            }
            else return;
        }

        if (playerDeck)
        {
            SoundManager.soundManager.Play("CardDraw", UnityEngine.Random.Range(0, 13));
            var card = Instantiate(cardInHandPrefab, drawPileText.transform.position, Quaternion.identity);
            card.transform.SetParent(CardsInHandParent);
            card.transform.localScale = Vector3.zero;
            card.transform.position = drawPileText.transform.position;

            CardInHand cardInHand = card.GetComponent<CardInHand>();
            cardInHand.card = drawPile[0];
            cardInHand.deck = this;
            cardsInHand.Add(card);
            cardInHand.startPos = drawPileText.transform.parent.localPosition;
        }

        cardsInHandAsCards.Add(drawPile[0]);
        
        drawPile[0].ActivateOnDrawEffects();

        drawPile.RemoveAt(0);
        TidyHand();
        CombatManager.combatManager.combatUI.UpdatePileNumbers();
    }

    public void DrawCard(int numOfCards)
    {
        if (playerDeck)
        {
            drawLeft += numOfCards;
        }
        else
        {
            for (int i = 0; i < numOfCards; i++)
            {
                DrawCard();
            }
        }
    }

    public void DrawCard(Card card){
        drawPile.RemoveAt(drawPile.IndexOf(card));
        drawPile.Insert(0, card);
        DrawCard();
    }

    public void ForceDraw()
    {
        for (int i = 0; i < drawLeft; i++)
        {
            DrawCard();
        }
        drawLeft = 0;
        drawT = drawTime;
    }
    public void DiscardHand() 
    {
        if(playerDeck){
            SoundManager.soundManager.Play("DiscardHand");
        }

        for (int i = 0; i < cardsInHandAsCards.Count; i++)
        {
            discardPile.Add(cardsInHandAsCards[i]);
            cardsInHandAsCards[i].ActivateOnDiscardEffects();

            if (playerDeck)
            {
                if (cardsInHand.Count <= i) break;
                GameObject cardObject = cardsInHand[i];
                if (cardObject == null) continue;

                CardInHand cardInHand = cardObject.GetComponent<CardInHand>();
                cardInHand.dontTidy = true;
                cardInHand.discarding = true;
                cardInHand.travelTime = 0.5f;
                cardInHand.startPos = cardObject.transform.localPosition;
                cardInHand.targetLocation = DiscardPileText.transform.parent.localPosition;
            }
        }
        cardsInHand.Clear();
        cardsInHandAsCards.Clear();
        CombatManager.combatManager.combatUI.UpdatePileNumbers();
    }

    public void DiscardCard(CardInHand card){
        cardsInHand.Remove(card.gameObject);
        cardsInHandAsCards.Remove(card.card);
        
        discardPile.Add(card.card);
        card.card.ActivateOnDiscardEffects();

        if(playerDeck){
            GameObject cardObject = card.gameObject;

            card.dontTidy = true;
            card.discarding = true;
            card.travelTime = 0.5f;
            card.startPos = cardObject.transform.localPosition;
            card.targetLocation = DiscardPileText.transform.parent.localPosition;
        }

        CombatManager.combatManager.combatUI.UpdatePileNumbers();
        TidyHand();
    }

    //Shuffle the deck using the Fisher-Yates shuffle
    System.Random random = new System.Random();
    public void Shuffle(){
        for(int i = drawPile.Count - 1; i > 0; i--){
            int k = random.Next(i + 1);
            (drawPile[i], drawPile[k]) = (drawPile[k], drawPile[i]);
        }
    }
    //--------------------------------//
    #endregion

    public void UpdateAllCardAppearances(){
        //Update cards in hand
        for(int i = 0; i < cardsInHand.Count; i++){
            cardsInHand[i].GetComponent<CardInHand>().UpdateCardAppearance();
        }

        //Update cards in combat
        for(int i = 0; i < 5; i++){
            if(CombatManager.combatManager.enemyCombatCards[i] != null){
                CombatManager.combatManager.enemyCombatCards[i].UpdateCardAppearance();
            }

            if(CombatManager.combatManager.enemyBenchCards[i] != null){
                CombatManager.combatManager.enemyBenchCards[i].UpdateCardAppearance();
            }

            if(CombatManager.combatManager.playerCombatCards[i] != null){
                CombatManager.combatManager.playerCombatCards[i].UpdateCardAppearance();
            }

            if(CombatManager.combatManager.playerBenchCards[i] != null){
                CombatManager.combatManager.playerBenchCards[i].UpdateCardAppearance();
            }
        }
    }

    

    public void PlaySigilAnimation(Transform cardGameObject, Card card, Sigil sigilToAnimate)
    {
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            if (card.sigils[i] == sigilToAnimate)
            {
                index = i;
                break;
            }
        }
        cardGameObject.GetChild(7 + index).GetComponent<AnimateSigil>().StartAnimation();
    }
}
