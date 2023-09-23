using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapDeck : MonoBehaviour, IDataPersistence
{
    public List<Card> cards;

    public int playerHealth;

    public GameObject playerHealthRect;
    public GameObject playerHealthDash;
    public TextMeshProUGUI playerHealthText;

    private List<Card> cardsToBeAdded;

    private void Start() {
        if (DataPersistenceManager.DataManager.playerDeck.Count > 0)
            {
                cardsToBeAdded = CopyCardList(DataPersistenceManager.DataManager.playerDeck);
                DataPersistenceManager.DataManager.playerDeck.Clear();
                AddCard(cardsToBeAdded.Count);
                Debug.Log("Added deck");
            }
    }

    public bool HasInjuredCards(){
        foreach (Card card in cards)
        {
            if(card.injuries.Count > 0){
                return true;
            }
        }

        return false;
    }

    public void LoadData(GameData data){
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

        playerHealth = data.playerHealth;
        UpdateHPText();
    }

    public void SaveData(GameData data){
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

        data.playerHealth = playerHealth;
    }

    public void AddCard(Card card){
        Card newCard = Instantiate(card).ResetCard();
        newCard.name = card.name;
        
        cards.Add(newCard);

    }

    public void AddCard(int numOfCards)
    {
        for (int i = 0; i < numOfCards; i++) 
        {
            AddCard(cardsToBeAdded[0]);
            cardsToBeAdded.RemoveAt(0);
        }
    }

    public List<Card> CopyCardList(List<Card> listToCopy) 
    {
        List<Card> returnList = new List<Card>();
        foreach (Card card in listToCopy) returnList.Add(card);
        return returnList;
    }

    public void UpdateHPText(){
        float playerVal = playerHealth / 20f;

        if (playerVal <= 0) playerVal = 0;

        playerHealthRect.transform.localScale =    new Vector3(playerVal, 1, 1);
        playerHealthDash.transform.localPosition = new Vector3(playerVal * 200 - 150, 1, 1);

        playerHealthText.text = playerHealth + "/" + 20;
    }
}
