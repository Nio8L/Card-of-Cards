using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class Deck : MonoBehaviour, IDataPersistence
{
    public bool playerDeck = false;

    public int energy = 3;
     
    public List<Card> cards = new();
    public List<Card> drawPile;
    public List<Card> discardPile;
    public List<GameObject> cardsInHand = new();
    public List<Card> cardsInHandAsCards = new();

    [HideInInspector]
    public CombatManager combatManager;
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
    public float spaceForHoveredCard;

    [HideInInspector]
    public Sprite biteDamageIcon;
    public Sprite scrachDamageIcon;
    public Sprite poisonDamageIcon;

    public TextMeshProUGUI drawPileText;
    public TextMeshProUGUI DiscardPileText;
    public Sprite deathMarkScratch;
    public Sprite deathMarkBite;
    public Sprite deathMarkPoison;

    #region Saving
    //--------------------------------//
    public void LoadData(GameData data){

        if (!playerDeck) return;

        cards.Clear();
        for(int i = 0; i < data.cardNames.Count; i++){
            Card newCard = new();

            AddCard(newCard);
            cards[^1].name = data.cardNames[i];
            cards[^1].attack = data.cardAttacks[i];
            //cards[^1].health = data.cardHealths[i];
            cards[^1].maxHealth = data.cardMaxHealths[i];
            cards[^1].health = cards[^1].maxHealth;
            cards[^1].cost = data.cardCosts[i];
            cards[^1].image = Resources.Load<Sprite>("Sprites/" + data.cardImages[i]);

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

    public void SaveData(ref GameData data){
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
            data.cardAttacks.Add(cards[i].attack);
            //data.cardHealths.Add(cards[i].health);
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

        biteDamageIcon = Resources.Load<Sprite>("Sprites/DamageTypeBite");
        scrachDamageIcon = Resources.Load<Sprite>("Sprites/DamageTypeSlash");
        poisonDamageIcon = Resources.Load<Sprite>("Sprites/DamageTypePoison");

        deathMarkScratch = Resources.Load<Sprite>("Sprites/DeathMarkScratch");
        deathMarkBite = Resources.Load<Sprite>("Sprites/DeathMarkBite");
        deathMarkPoison = Resources.Load<Sprite>("Sprites/DeathMarkPoison");

        soulHeart = Resources.Load<GameObject>("Prefabs/LostSoulHeart");

        CardsInHandParent = GameObject.Find("CardsInHand").transform;
        CardsInCombatParent = GameObject.Find("CardsInCombat").transform;

        if (playerDeck) combatManager = GetComponent<CombatManager>();
        else combatManager = GameObject.Find("Deck").GetComponent<CombatManager>(); 

        energyText = GameObject.Find("Energy").GetComponent<TextMeshProUGUI>();
        drawPile = CopyCardList(cards);

        if(cards.Count == 0 && playerDeck){
            AddCard(10);
        }
        else if (!playerDeck)
        {
            if (DataPersistenceManager.DataManager.currentCombatAI != null)
            {
                combatManager.enemy = DataPersistenceManager.DataManager.currentCombatAI;
            }
            combatManager.enemy.Initialize();
            combatManager.updateHPText();
        }
        Shuffle();
        DrawCard(5);
    }

    private void Update()
    {
        if (selectedCard != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCard.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }

        if (playerDeck) energyText.text = energy + "/3";

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
        Card newCard = Instantiate(card).ResetCard();
        newCard.name = card.name;
        
        drawPile.Add(newCard);
        cards.Add(newCard);
        PrintDeck();
        UpdatePileNumbers();
    }


    //FOR TESTING
    public List<Card> randomCardSelection = new();
    public void AddCard(){
        //AddCard(randomCardSelection[UnityEngine.Random.Range(0, randomCardSelection.Count)]);
        if (randomCardSelection.Count == 0) return;
        AddCard(Instantiate(randomCardSelection[0]).ResetCard());
        randomCardSelection.RemoveAt(0);
    }

    public void AddCard(int numOfCards)
    {
        for (int i = 0; i < numOfCards; i++) AddCard();
    }
    public void RemoveCard(Card card){
        drawPile.Remove(card);
        cards.Remove(card);
        UpdatePileNumbers();
    }

    public void RemoveCard(){
        drawPile.RemoveAt(drawPile.Count - 1);
        cards.RemoveAt(cards.Count - 1);
        PrintDeck();
        UpdatePileNumbers();
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
            var card = Instantiate(cardInHandPrefab, new Vector3(cardsInHand.Count * 2, -3.5f, 0), Quaternion.identity);
            card.transform.SetParent(CardsInHandParent);
            card.transform.localScale = Vector3.one;

            CardInHand cardInHand = card.GetComponent<CardInHand>();
            cardInHand.card = drawPile[0];
            cardInHand.deck = this;
            cardsInHand.Add(card);
        }

        cardsInHandAsCards.Add(drawPile[0]);
        
        drawPile.RemoveAt(0);
        TidyHand();
        UpdatePileNumbers();
    }

    public void DrawCard(int numOfCards)
    {
        for (int i = 0; i < numOfCards; i++)
        {
            DrawCard();
        }
    }

    public void DiscardHand() 
    {
        for (int i = 0; i < cardsInHandAsCards.Count; i++)
        {
            discardPile.Add(cardsInHandAsCards[i]);

            if (playerDeck)
            {
                GameObject cardObject = cardsInHand[i];
                Destroy(cardObject);
            }
        }
        cardsInHand.Clear();
        cardsInHandAsCards.Clear();
        UpdatePileNumbers();
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

    public void UpdateCardAppearance(Transform cardGameObject, Card card)
    {
        Debug.Log("UpdateCardAppearance " + card.name + " " + cardGameObject.name);
        cardGameObject.GetChild(0).GetComponent<Image>().sprite = card.image;

        Sprite damageIcon;
        if (card.typeOfDamage == Card.TypeOfDamage.Bite) damageIcon = biteDamageIcon;
        else if (card.typeOfDamage == Card.TypeOfDamage.Scratch) damageIcon = scrachDamageIcon;
        else damageIcon = poisonDamageIcon;
        cardGameObject.GetChild(2).GetComponent<Image>().sprite = damageIcon;

        cardGameObject.GetChild(3).GetComponent<TextMeshProUGUI>().text = card.name;
        cardGameObject.GetChild(4).GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        cardGameObject.GetChild(5).GetComponent<TextMeshProUGUI>().text = card.health.ToString();
        cardGameObject.GetChild(6).GetComponent<TextMeshProUGUI>().text = card.attack.ToString();


        cardGameObject.GetChild(7).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        cardGameObject.GetChild(8).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        cardGameObject.GetChild(9).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        // Set sigil sprites
        if (card.sigils.Count == 1)
        {
            cardGameObject.GetChild(7).GetComponent<Image>().sprite = card.sigils[0].image;
            cardGameObject.GetChild(7).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else if (card.sigils.Count == 2)
        {
            cardGameObject.GetChild(8).GetComponent<Image>().sprite = card.sigils[0].image;
            cardGameObject.GetChild(9).GetComponent<Image>().sprite = card.sigils[1].image;
            cardGameObject.GetChild(8).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            cardGameObject.GetChild(9).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else if (card.sigils.Count == 3)
        {
            cardGameObject.GetChild(7).GetComponent<Image>().sprite = card.sigils[0].image;
            cardGameObject.GetChild(8).GetComponent<Image>().sprite = card.sigils[1].image;
            cardGameObject.GetChild(9).GetComponent<Image>().sprite = card.sigils[2].image;
            cardGameObject.GetChild(7).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            cardGameObject.GetChild(8).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            cardGameObject.GetChild(9).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        
        // Set injury marks
        cardGameObject.GetChild(10).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        cardGameObject.GetChild(11).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        cardGameObject.GetChild(12).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        foreach (Card.TypeOfDamage injury in card.injuries)
        {
            if (injury == Card.TypeOfDamage.Bite) cardGameObject.GetChild(10).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            else if (injury == Card.TypeOfDamage.Scratch) cardGameObject.GetChild(11).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            else if (injury == Card.TypeOfDamage.Poison) cardGameObject.GetChild(12).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        cardGameObject.GetChild(7).GetComponent<SigilTooltip>().UpdateSigilTooltip();
        cardGameObject.GetChild(8).GetComponent<SigilTooltip>().UpdateSigilTooltip();
        cardGameObject.GetChild(9).GetComponent<SigilTooltip>().UpdateSigilTooltip();
    }

    public void UpdatePileNumbers() 
    {
        if (!playerDeck) return;
        drawPileText.text = drawPile.Count + "";
        DiscardPileText.text = discardPile.Count + "";
    }
}
