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
    public MapNode.RoomType roomType;

    public bool used = false;

    public bool isCurrentNode = false;
}

[System.Serializable]
public class MapClass
{
    public List<LayerClass> mapLayers;

    public MapClass(){
        mapLayers = new();
    }
}

[System.Serializable]
public class LayerClass
{
    public List<MapNodeClass> mapNodeClasses;
    public int placeInTheArray;
    public int enterConectionType;

    public LayerClass(){
        mapNodeClasses = new();
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

    public List<LayerClass> mapLayers;

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

        mapLayers = new();
        //Debug.Log(map);
    }
    
}
