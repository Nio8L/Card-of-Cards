using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ListWrapper
{
    public List<string> list;

    public ListWrapper(){
        list = new();
    }
}

public class GameData
{
  
    public long lastSaved;

    public List<string> cardNames;
    public List<int> cardAttacks;
    //public List<int> cardHealths;
    public List<int> cardMaxHealths;
    public List<int> cardCosts;
    public List<string> cardImages;

    public List<ListWrapper> cardSigils;

    public List<ListWrapper> cardInjuries;

    public List<string> cardDamageType;

    public GameData(){
        cardNames = new();
        cardAttacks = new();
        //scardHealths = new();
        cardMaxHealths = new();
        cardCosts = new();
        cardImages = new();

        cardSigils = new();
    
        cardInjuries = new();

        cardDamageType = new();
    }
    
}
