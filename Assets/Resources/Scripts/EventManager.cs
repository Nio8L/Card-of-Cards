using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static Action<Card, List<Card>, bool> CardCreated;

    public static Action CardDeath;

    public static Action NextTurn;

    public static Action CombatEnd;

    public void InvokeNextTurn(){
        NextTurn?.Invoke();
    }
}
