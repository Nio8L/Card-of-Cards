using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static Action<Card, List<Card>> CardCreated;

    public static Action CardDeath;
}
