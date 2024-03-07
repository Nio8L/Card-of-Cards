using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Active Decider")]
public class Decider : ScriptableObject
{
    public virtual List<CardSlot> GetSlots(int neededTargets){
        return null;
    }
}
