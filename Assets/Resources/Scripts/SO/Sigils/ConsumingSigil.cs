using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Consuming Sigil")]
public class ConsumingSigil : Sigil
{
    public CardAcceptor cardAcceptor;
    public GameObject consumeParticles;

    public override void OnSummonEffect(CardInCombat card)
    {
        CardAcceptor hasAcceptor = card.GetComponent<CardAcceptor>();
        if (hasAcceptor == null){
            cardAcceptor = card.gameObject.AddComponent<CardAcceptor>();
        }else{
            cardAcceptor = hasAcceptor;
        }
        
    }

    public override void OnConsumeEffect(CardInCombat card, Card consumedCard)
    {
        SoundManager.soundManager.Play("Consume");

        if (consumeParticles != null){
            Instantiate(consumeParticles, card.transform.position, Quaternion.identity);
        }
    }

    public override void OnDeadEffect(CardInCombat card)
    {
        cardAcceptor.ReturnCards(card.deck.discardPile);
    }
}
