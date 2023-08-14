using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
  
    public long lastSaved;

    public List<string> cardNames;
    public List<int> cardAttacks;
    public List<int> cardHealths;
    public List<int> cardCosts;
    public List<string> cardImages;

    public GameData(){
       cardNames = new();
       cardAttacks = new();
       cardHealths = new();
       cardCosts = new();
       cardImages = new();
    }
    
}
