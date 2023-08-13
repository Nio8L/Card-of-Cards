using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil")]
public class Sigil : ScriptableObject
{
    public string name;

    public virtual void ApplyEffectsOnStart(Card cardToApplyItTo)
    {
        cardToApplyIt.hp++;//effect should be here
    }
}
