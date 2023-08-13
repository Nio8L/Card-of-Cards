using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil")]
public class Sigil : ScriptableObject
{
    public string sigilName;
    public Sprite image;

    public virtual void ApplyEffectsOnStart(Card cardToApplyItTo)
    {
        cardToApplyItTo.health++;//effect should be here
    }
}
