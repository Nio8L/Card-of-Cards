using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil")]
public class Sigil : ScriptableObject
{
    public string sigilName;
    public bool negative;
    public Sprite image;
    public string description;

    public virtual void PasiveEffect(CardInCombat card) 
    {
        //effect should be here
    }

    public virtual void ApplyOnHitEffect(CardInCombat card)
    {
        //effect should be here
    }

    public virtual void OnTakeDamageEffect(CardInCombat card)
    {
        //effect should be here
    }

    public virtual void OnSummonEffects(CardInCombat card) 
    {
        //maj efecto trebe da e tuka
    }
}
