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
    public bool hasCaptain = true;

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
    public float spaceForHoveredCard;

    [HideInInspector]
    public Sprite biteDamageIcon;
    public Sprite scrachDamageIcon;
    public Sprite poisonDamageIcon;

    public TextMeshProUGUI drawPileText;
    public TextMeshProUGUI DiscardPileText;
    [HideInInspector]
    public Sprite deathMarkScratch;
    [HideInInspector]
    public Sprite deathMarkBite;
    [HideInInspector]
    public Sprite deathMarkPoison;
    [HideInInspector]
    public Sprite activeStar;
    [HideInInspector]
    public Sprite selectedActiveStar;

    int drawLeft = 0;
    float drawTime = 0.2f;
    float drawT = 0.1f;

    [Header("Notifications")]
    public Notification noEnergyNotification;

    #region Saving
    //--------------------------------//
    public void LoadData(GameData data){
        if (!playerDeck) {
            return;
        }

        cards.Clear();
        for(int i = 0; i < data.cardNames.Count; i++){
            Card newCard = new();

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

        biteDamageIcon = Resources.Load<Sprite>("Sprites/DamageTypeBite");
        scrachDamageIcon = Resources.Load<Sprite>("Sprites/DamageTypeSlash");
        poisonDamageIcon = Resources.Load<Sprite>("Sprites/DamageTypePoison");

        deathMarkScratch = Resources.Load<Sprite>("Sprites/DeathMarkScratch");
        deathMarkBite = Resources.Load<Sprite>("Sprites/DeathMarkBite");
        deathMarkPoison = Resources.Load<Sprite>("Sprites/DeathMarkPoison");

        activeStar = Resources.Load<Sprite>("Sprites/Sigils/ActiveStar");
        selectedActiveStar = Resources.Load<Sprite>("Sprites/Sigils/SelectedActiveStar");

        soulHeart = Resources.Load<GameObject>("Prefabs/LostSoulHeart");

        CardsInHandParent = GameObject.Find("CardsInHand").transform;
        CardsInCombatParent = GameObject.Find("CardsInCombat").transform;

        energyText = GameObject.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        drawPile = CopyCardList(cards);

        if (playerDeck)
        {
            if (DataPersistenceManager.DataManager.playerDeck.Count > 0 && DataPersistenceManager.DataManager.inTutorial)
            {
                cardsToBeAdded = CopyCardList(DataPersistenceManager.DataManager.playerDeck);
                DataPersistenceManager.DataManager.playerDeck.Clear();
                AddCard(cardsToBeAdded.Count);
                //Debug.Log("Added deck");
            }
           
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
        UpdatePileNumbers();
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
                targetCard.UpdateTilt();
            }
        }

        //Update tilt so there are 2 center cards if there is an even amount of cards in hand
        if(cardsInHand.Count % 2 == 0){
            for(int i = 0; i < cardsInHand.Count / 2; i++){
                cardsInHand[i].GetComponent<CardInHand>().tiltAngle -= 10;
                cardsInHand[i].GetComponent<CardInHand>().UpdateTilt();
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
        
        drawPile.RemoveAt(0);
        TidyHand();
        UpdatePileNumbers();
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
        UpdatePileNumbers();
    }

    public void DiscardCard(CardInHand card){
        cardsInHand.Remove(card.gameObject);
        cardsInHandAsCards.Remove(card.card);
        
        discardPile.Add(card.card);

        if(playerDeck){
            GameObject cardObject = card.gameObject;

            card.dontTidy = true;
            card.discarding = true;
            card.travelTime = 0.5f;
            card.startPos = cardObject.transform.localPosition;
            card.targetLocation = DiscardPileText.transform.parent.localPosition;
        }

        UpdatePileNumbers();
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

    public void UpdateCardAppearance(Transform cardGameObject, Card card)
    {
        //Debug.Log("UpdateCardAppearance " + card.name + " " + cardGameObject.name);
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
        for (int i = 0; i < card.sigils.Count; i++){
            cardGameObject.GetChild(7+i).GetComponent<Image>().sprite = card.sigils[i].image;
            cardGameObject.GetChild(7+i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
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

        if (card.captain)
        {
            cardGameObject.GetChild(13).gameObject.SetActive(true);
        }
        else
        {
            cardGameObject.GetChild(13).gameObject.SetActive(false);
        }

        CardInCombat combatCard = cardGameObject.GetComponent<CardInCombat>();
        CardInHand handCard = cardGameObject.GetComponent<CardInHand>();


        if (combatCard != null)
        {
            cardGameObject.GetChild(14).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            cardGameObject.GetChild(15).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            cardGameObject.GetChild(16).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            combatCard.ShowSigilStars();
        }
        else if (handCard)
        {
            handCard.UpdateCostText();
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

    public void UpdatePileNumbers()
    {
        if (!playerDeck) return;
        drawPileText.text = drawPile.Count + "";
        DiscardPileText.text = discardPile.Count + "";
    }
}
