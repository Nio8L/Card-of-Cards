using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Totem : ScriptableObject
{
    public string visibleName;
    public string description;

    public Sprite totemSprite;
    public virtual void Setup(){
        /*
           Subscribe to events
        */
    }

    public virtual void Unsubscribe(){
        /*
           Reverse the things done in setup
        */
    }
    public virtual void Passive(){
        /*
           Use with events
        */
    }

    public virtual void Active(){
        /*
            Activates through totem manager
        */
    }
}
