using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Consuming Sigil")]
public class ConsumingSigil : Sigil
{
    public CardAcceptor cardAcceptor;

    public override void OnSummonEffects(CardInCombat card)
    {
        cardAcceptor = card.gameObject.AddComponent<CardAcceptor>();
    }

    public override void OnConsumeEffects(CardInCombat card, Card consumedCard)
    {
        SoundManager.soundManager.Play("Consume");
    }
}
