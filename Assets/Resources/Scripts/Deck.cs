using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour, IDataPersistence
{
    public int energy = 3;
     
    public List<Card> cards = new();
    public List<Card> drawPile;

    public CombatManager combatManager;
    public List<GameObject> cardsInHand = new();
    GameObject cardInHandPrefab;
    public GameObject cardInCombatPrefab;
    public GameObject cardInBenchPrefab;

    public Transform CardsInHandParent;
    public Transform CardsInCombatParent;

    public Transform selectedCard;
    TextMeshProUGUI energyText;

    public Vector2 centerPointForcardsInHand;
    public float spaceBetweenCardsInHand;
    public Transform hoveredCard;
    public float spaceForHoveredCard;

    #region Saving
    //--------------------------------//
    public void LoadData(GameData data){
        cards.Clear();
        for(int i = 0; i < data.cardNames.Count; i++){
            Card newCard = new();

            AddCard(newCard);
            cards[^1].name = data.cardNames[i];
            cards[^1].attack = data.cardAttacks[i];
            cards[^1].health = data.cardHealths[i];
            cards[^1].cost = data.cardCosts[i];
            cards[^1].image = Resources.Load<Sprite>("Sprites/" + data.cardImages[i]);
        }
    }

    public void SaveData(ref GameData data){
        data.cardNames.Clear();
        data.cardAttacks.Clear();
        data.cardHealths.Clear();
        data.cardCosts.Clear();
        data.cardImages.Clear();

        for(int i = 0; i < cards.Count; i++){
            data.cardNames.Add(cards[i].name);
            data.cardAttacks.Add(cards[i].attack);
            data.cardHealths.Add(cards[i].health);
            data.cardCosts.Add(cards[i].cost);
            data.cardImages.Add(cards[i].image.name);
        }
    }
    //--------------------------------//
    #endregion

    private void Start()
    {
        cardInHandPrefab = Resources.Load<GameObject>("Prefabs/CardPrefab/CardInHand");
        cardInCombatPrefab = Resources.Load<GameObject>("Prefabs/CardPrefab/CardInCombat");

        CardsInHandParent = GameObject.Find("CardsInHand").transform;
        CardsInCombatParent = GameObject.Find("CardsInCombat").transform;

        combatManager = GetComponent<CombatManager>();
        
        energyText = GameObject.Find("Energy").GetComponent<TextMeshProUGUI>();
        drawPile = CopyCardList(cards);

        AddCard(10);
        DrawCard(5);
    }

    private void Update()
    {
        if (selectedCard != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCard.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }
        energyText.text = energy + "/3";

    }

    #region Deck Functions
    //--------------------------------//
    List<Card> CopyCardList(List<Card> listToCopy) 
    {
        List<Card> returnList = new List<Card>();
        foreach (Card card in listToCopy) returnList.Add(card);
        return returnList;
    }

    public void AddCard(Card card){
        Card newCard = Instantiate(card);
        newCard.name = card.name;

        drawPile.Add(newCard);
        cards.Add(newCard);
        PrintDeck();
    }


    //FOR TESTING
    public List<Card> randomCardSelection = new();
    public void AddCard(){
        AddCard(randomCardSelection[Random.Range(0, randomCardSelection.Count)]);
    }

    public void AddCard(int numOfCards)
    {
        for (int i = 0; i < numOfCards; i++) AddCard();
    }
    public void RemoveCard(Card card){
        drawPile.Remove(card);
        cards.Remove(card);
    }

    public void RemoveCard(){
        drawPile.RemoveAt(drawPile.Count - 1);
        PrintDeck();
    }

    public void PrintDeck(){
        string cardsInDeck = "";
        foreach (Card card in cards)
        {
            cardsInDeck += card.name;
            cardsInDeck += ", ";
        }
        Debug.Log(cardsInDeck);
        cardsInDeck = "";
        foreach (Card card in drawPile)
        {
            cardsInDeck += card.name;
            cardsInDeck += ", ";
        }
        Debug.Log(cardsInDeck);
    }

    public void TidyHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            if (cardsInHand[i] != null)cardsInHand[i].transform.localPosition = new Vector2(centerPointForcardsInHand.x - (0.5f * cardsInHand.Count - 0.5f) * (spaceBetweenCardsInHand / (cardsInHand.Count / 2f)) + i * (spaceBetweenCardsInHand / (cardsInHand.Count / 2f)), centerPointForcardsInHand.y);
        }

        foreach (GameObject card in cardsInHand)
        {
           if(card != hoveredCard) card.transform.SetAsLastSibling();
        }

        if (hoveredCard != null) 
        {
            hoveredCard.SetAsLastSibling();
            hoveredCard.localScale = new Vector2(1.3f,1.3f);
            hoveredCard.position = new Vector2(hoveredCard.position.x, hoveredCard.position.y+spaceForHoveredCard);
        }
    }


    // Teglene na karti v //  
    public void DrawCard()
    {
        if (drawPile.Count != 0)
        {
            var card = Instantiate(cardInHandPrefab, new Vector3(cardsInHand.Count * 2, -3.5f, 0), Quaternion.identity);
            card.transform.SetParent(CardsInHandParent);
            card.transform.localScale = Vector3.one;
            CardInHand cardInHand = card.GetComponent<CardInHand>();
            cardInHand.card = drawPile[0];
            cardInHand.deck = this;
            drawPile.RemoveAt(0);
            cardsInHand.Add(card);
            TidyHand();
        }
    }

    public void DrawCard(int numOfCards)
    {
        for (int i = 0; i < numOfCards && drawPile.Count != 0; i++)
        {
            var card = Instantiate(cardInHandPrefab, new Vector3(cardsInHand.Count * 2, -3.5f, 0), Quaternion.identity);
            card.transform.SetParent(CardsInHandParent);
            card.transform.localScale = Vector3.one;
            CardInHand cardInHand = card.GetComponent<CardInHand>();
            cardInHand.card = drawPile[0];
            cardInHand.deck = this;
            drawPile.RemoveAt(0);
            cardsInHand.Add(card);
        }
        TidyHand();
    }

    public void DiscardHand() 
    {
        foreach (GameObject cardObject in cardsInHand)
        {
            drawPile.Add(cardObject.GetComponent<CardInHand>().card);
            Destroy(cardObject);
        }
        cardsInHand.Clear();
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
}
