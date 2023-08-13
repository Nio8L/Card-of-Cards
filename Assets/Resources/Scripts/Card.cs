using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int hp;
    public int attack;
    public int cost;
    public Sprite image;

    public enum TypeOfDmg
    {
        Poison,
        Bite,
        Scratch
    };

    public List<TypeOfDmg> typeOfDmg;

    public List<Sigil> sigils;

    public Dictionary<TypeOfDmg, bool> deadFrom = new();
    
    public void Die(List<TypeOfDmg> causes)
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

    
}
