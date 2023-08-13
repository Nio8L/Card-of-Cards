using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour, IDataPersistence
{
    public int energy = 3;
     
    public List<Card> cards = new();
    public List<Card> cardsInHand = new();

    public CombatManager combatManager;

    GameObject cardInHandPrefab;
    public GameObject cardInCombatPrefab;

    public Transform canvasTransform;

    public Transform selectedCard;

    TextMeshProUGUI energyText;

    #region Saving
    //--------------------------------//
    public void LoadData(GameData data){
        cards.Clear();
        for(int i = 0; i < data.cardNames.Count; i++){
            Card newCard = new Card();

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
        
        List<Card> newCardList = new();
        newCardList.AddRange(cardsInHand);
        newCardList.AddRange(cards); 
        cards = newCardList;

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
        cardInHandPrefab = Resources.Load<GameObject>("Prefabs/CardInHand");
        cardInCombatPrefab = Resources.Load<GameObject>("Prefabs/CardInCombat");
        canvasTransform = GameObject.Find("Canvas").transform;
        combatManager = GetComponent<CombatManager>();
        energyText = GameObject.Find("Energy").GetComponent<TextMeshProUGUI>();
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
    public void AddCard(Card card){
        Card newCard = Instantiate(card);
        newCard.name = card.name;

        cards.Add(newCard);
        PrintDeck();
    }

    //FOR TESTING
    public List<Card> randomCardSelection = new();
    public void AddCard(){
        AddCard(randomCardSelection[Random.Range(0, randomCardSelection.Count)]);
    }

    public void RemoveCard(Card card){
        cards.Remove(card);
    }

    public void RemoveCard(){
        cards.RemoveAt(cards.Count - 1);
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
    }

    // Teglene na karti v //  
    public void DrawCard()
    {
        cardsInHand.Add(cards[0]);
        var card = Instantiate(cardInHandPrefab, new Vector3(cardsInHand.Count * 2, -3.5f, 0), Quaternion.identity);
        card.transform.SetParent(canvasTransform);
        card.transform.localScale = Vector3.one;
        CardInHand cardInHand = card.GetComponent<CardInHand>();
        cardInHand.card = cardsInHand[cardsInHand.Count - 1];
        cardInHand.deck = this;
        cards.RemoveAt(0);
    }

    //Shuffle the deck using the Fisher-Yates shuffle
    System.Random random = new System.Random();
    public void Shuffle(){
        for(int i = cards.Count - 1; i > 0; i--){
            int k = random.Next(i + 1);
            (cards[i], cards[k]) = (cards[k], cards[i]);
        }
    }
    //--------------------------------//
    #endregion
}
