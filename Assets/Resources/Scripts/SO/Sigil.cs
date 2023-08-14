using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil")]
public class Sigil : ScriptableObject
{
    public string sigilName;
    public Sprite image;
    Deck deck;

    private void OnEnable()
    {
        deck = GameObject.Find("Deck").GetComponent<Deck>();
        Debug.Log(deck.name + " found by sigil");
    }

    public virtual void PasiveEffect() 
    {
        //effect should be here
    }

    public virtual void ApplyOnHitEffect()
    {
        //effect should be here
    }
}
