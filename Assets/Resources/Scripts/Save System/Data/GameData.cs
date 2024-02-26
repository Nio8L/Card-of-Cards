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

[System.Serializable]
public class Map{
    public int seed;
    public int layerIndex;
    public int nodeIndex;
    public bool hasTraveled;

    public Map(){
        seed = 0;
        layerIndex = 0;
        nodeIndex = 0;
        hasTraveled = false;
    }
}

[System.Serializable]
public class GameData
{
  
    public long lastSaved;

    public float runTime;

    public int playerHealth;

    public List<string> cardNames;
    public List<int> cardAttacks;
    public List<int> cardMaxHealths;
    public List<int> cardCosts;
    public List<string> cardImages;

    public List<ListWrapper> cardSigils;

    public List<ListWrapper> cardInjuries;

    public List<string> cardDamageType;

    public string enemyAI;

    public Map map;

    public GameData(){
        runTime = 0;

        playerHealth = 20;

        cardNames = new();
        cardAttacks = new();
        cardMaxHealths = new();
        cardCosts = new();
        cardImages = new();

        cardSigils = new();
    
        cardInjuries = new();

        cardDamageType = new();
        
        enemyAI = "";

        map = new();
    }
    
}
