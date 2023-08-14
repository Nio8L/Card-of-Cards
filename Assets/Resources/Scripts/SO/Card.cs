using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int health;
    public int attack;
    public int cost;
    public Sprite image;

    public enum TypeOfDamage
    {
        Poison,
        Bite,
        Scratch
    };

    public TypeOfDamage typeOfDamage;

    public List<Sigil> sigils;

    public List<TypeOfDamage> injuries = new();

    public void CreateCard(TypeOfDamage causeOfDeath)
    {
        foreach (TypeOfDamage type in injuries)
        {
            if (type == causeOfDeath)
            {
                // Lost soul
                return;
            }
        }
        injuries.Add(causeOfDeath);
        //new card
    }

    
}
