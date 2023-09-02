using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckHolder : MonoBehaviour, IDataPersistence
{
    public List<Card> cards;

    public static DeckHolder deckHolder;

    private void Awake() {
        if(deckHolder != null){
            Destroy(gameObject);
            return;
        }
        deckHolder = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start(){
    }

    public void LoadData(GameData data)
    {
        cards.Clear();
        for(int i = 0; i < data.cardNames.Count; i++){
            Card newCard = new();

            cards.Add(newCard);
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

    public void SaveData(ref GameData data)
    {
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

    public void AddCard(Card card){
        Card newCard = Instantiate(card).ResetCard();
        newCard.name = card.name;
        
        
        cards.Add(newCard);
    }


    //FOR TESTING
    public List<Card> randomCardSelection = new();
    public void AddCard(){
        AddCard(randomCardSelection[UnityEngine.Random.Range(0, randomCardSelection.Count)]);
        //if (randomCardSelection.Count == 0) return;
        //AddCard(Instantiate(randomCardSelection[0]).ResetCard());
        //randomCardSelection.RemoveAt(0);
    }

    public void AddCard(int numOfCards)
    {
        for (int i = 0; i < numOfCards; i++) AddCard();
    }
}
