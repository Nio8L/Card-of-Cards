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
public class MapNodeClass
{
    public List<float> coordinates;

    public string roomType;

    public List<int> connectionIds;

    public int id;

    public int nodeDepth;

    public bool used = false;

    public MapNodeClass(){
        coordinates = new();
        connectionIds = new();
    }
}

[System.Serializable]
public class MapClass
{
    public List<MapNodeClass> mapNodes;

    public int currentNodeId;

    public MapClass(){
        mapNodes = new();
    }
}

[System.Serializable]
public class GameData
{
  
    public long lastSaved;

    public int playerHealth;

    public List<string> cardNames;
    public List<int> cardAttacks;
    //public List<int> cardHealths;
    public List<int> cardMaxHealths;
    public List<int> cardCosts;
    public List<string> cardImages;

    public List<ListWrapper> cardSigils;

    public List<ListWrapper> cardInjuries;

    public List<string> cardDamageType;

    public MapClass map;

    public GameData(){
        playerHealth = 20;

        cardNames = new();
        cardAttacks = new();
        //scardHealths = new();
        cardMaxHealths = new();
        cardCosts = new();
        cardImages = new();

        cardSigils = new();
    
        cardInjuries = new();

        cardDamageType = new();

        map = new();
        //Debug.Log(map);
    }
    
}
