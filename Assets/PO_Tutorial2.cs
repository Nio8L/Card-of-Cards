using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PO_Tutorial2 : TutorialBase
{
    public List<Card> cardToAdd = new List<Card>();
    public override void ExecuteChanges()
    {
        stageExecuted = true;

        if (stage == 0){
            // Say welcome
            CombatManager.combatManager.playerHealth = 20;
            ChangeEndTurnButton();
            NotificationManager.notificationManager.Notify(notifications[0], new Vector3(0, -200, 0));
            checkIfNotificationIsGone = true;
        }else if (stage == 1){
            // Give cards to player and let him play the game by himself
            NotificationManager.notificationManager.Notify(notifications[1], new Vector3(0, -200, 0));

            for (int i = 0; i < cardToAdd.Count; i++){
                Card copiedCard = Instantiate(cardToAdd[i]).ResetCard();
                copiedCard.name = cardToAdd[i].name;
                CombatManager.combatManager.deck.drawPile.Add(copiedCard);
            }

            CombatManager.combatManager.deck.DrawCard(cardToAdd.Count);

            ChangeEndTurnButton();
        }
    }
}
