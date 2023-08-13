using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int hp;
    public int attack;
    public int cost;

    public List<TypeOfDmg> typeOfDmg;

    public List<Sigil> sigils;

    Dictionary<TypeOfDmg, bool> deadFrom;

    public void Die(List<TypeOfDmg> cause)
    {
        foreach (TypeOfDmg cause in causes)
        {
            if (deadFrom[cause])
            {
                //turnToLostSoul
                return;
            }
            //new card
        }
    }

    public void Die(TypeOfDmg cause)
    {
        if (deadFrom[cause]){
            //turnToLostSoul
            return;
        }
        //new card
    }

    public enum TypeOfDmg
    {
        Poison,
        Bite,
        Scratch
    };
}
