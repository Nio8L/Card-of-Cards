using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int hp;

    bool diedFromScratch;
    bool diedFromPoison;
    bool diedFromBite;
}
