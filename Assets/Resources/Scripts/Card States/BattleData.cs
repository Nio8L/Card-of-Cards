using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleData
{
    public Card enemyCard;
    public int enemyCardOldHp;
    public Card thisCard;
    public int thisCardOldHp;
    public CardInCombat enemy;
   
    public BattleData(Card _thisCard, Card _enemyCard, int _thisHp, int _enemyHp, CardInCombat _enemy)
    {
        enemyCard = _enemyCard;
        enemyCardOldHp = _enemyHp;
        thisCard = _thisCard;
        thisCardOldHp = _thisHp;
        enemy = _enemy;
    }

    public void LogResult()
    {
        Debug.Log(thisCard.name + " with hp= " + thisCardOldHp + " fought " + enemyCard + " with hp= " + enemyCardOldHp + " -> " + thisCard.name + " hp= " + thisCard.health + " " + enemyCard.name + " hp= " + enemyCard.health); 
    }
}
