using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil")]
public class Sigil : ScriptableObject
{
    public string sigilName;
    public Sprite image;

    public virtual void PasiveEffect(Card[] enemyCards, Card[] friendlyCards) 
    {
        //effect should be here
    }

    public virtual void ApplyOnHitEffect(Card cardThatApplyesIt, Card cardToApplyItTo)
    {
        //effect should be here
    }
}
