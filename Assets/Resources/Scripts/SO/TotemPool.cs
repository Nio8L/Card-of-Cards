using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Totem/> Totem pool")]
public class TotemPool : ScriptableObject
{
    public List<Totem> totems = new List<Totem>();

    public Totem GetTotem(){
        return totems[Mathf.FloorToInt(Random.Range(0, totems.Count))];
    }
}
