using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PO_Tutorial3 : TutorialBase
{
    public Card injuredMouse;
    public Card lostSoul;
    public Card borke;
    public override void ExecuteChanges()
    {
        stageExecuted = true;

        if (stage == 0){
            // Say welcome'
            CombatManager.combatManager.playerHealth = 20;
            ChangeEndTurnButton();
            NotificationManager.notificationManager.Notify(notifications[0], new Vector3(0, -200, 0));
            checkIfNotificationIsGone = true;
        }else if (stage == 1){
            // Explain damage types and add rat
            NotificationManager.notificationManager.Notify(notifications[1], new Vector3(0, -200, 0));

            Card copiedCard = Instantiate(injuredMouse).ResetCard();
            copiedCard.name = injuredMouse.name;
            CombatManager.combatManager.deck.drawPile.Add(copiedCard);
            CombatManager.combatManager.deck.DrawCard(1);
        }else if (stage == 2){
            NotificationManager.notificationManager.Notify(notifications[2], new Vector3(0, -200, 0));
            checkIfNotificationIsGone = false;
            ChangeStage(stage + 1);
        }else if (stage == 3){
            // Find the rat
            stageExecuted = false;
            for (int i = 0; i < CombatManager.combatManager.playerCombatCards.Length; i++){
                CardInCombat combatCard = CombatManager.combatManager.playerCombatCards[i];
                CardInCombat benchCard = CombatManager.combatManager.playerBenchCards[i];
                if (combatCard != null && combatCard.card.name == injuredMouse.name){
                    cardToTrack = combatCard;
                    ChangeStage(stage + 3);
                    break;
                }else if (benchCard != null && benchCard.card.name == injuredMouse.name){
                    cardToTrack = benchCard;
                    ChangeStage(stage + 1);
                    break;
                }
            }
        }else if (stage == 4){
            // Unbench notif
            NotificationManager.notificationManager.CloseNotificationWindow(0);
            NotificationManager.notificationManager.Notify(notifications[3], new Vector3(0, -200, 0));
            ChangeStage(stage + 1);
        }else if (stage == 5){
            // Check if the card in unbenched
            stageExecuted = false;
            if (!cardToTrack.benched){
                ChangeStage(stage + 1);
            }
        }else if (stage == 6){
            // End turn text
            NotificationManager.notificationManager.CloseNotificationWindow(0);
            NotificationManager.notificationManager.Notify(notifications[4], new Vector3(0, -200, 0));
            ChangeEndTurnButton();
            ChangeStage(stage + 1);
        }else if (stage == 7){
            // Check if the card is still benched and then if the turn has changed
            stageExecuted = false;
            if (cardToTrack.benched){
                ChangeEndTurnButton(false);
            }else{  
                ChangeEndTurnButton(true);
            }

            if (CombatManager.combatManager.round != 1){
                ChangeStage(stage + 1);
            }
        }else if (stage == 8){
            // Combat
            NotificationManager.notificationManager.CloseNotificationWindow(0);
            NotificationManager.notificationManager.Notify(notifications[5], new Vector3(0, -200, 0));
            ChangeEndTurnButton();
            checkIfNotificationIsGone = true;
        }else if (stage == 9){
            NotificationManager.notificationManager.Notify(notifications[6], new Vector3(0, -200, 0));

            Card copiedCard = Instantiate(lostSoul).ResetCard();
            copiedCard.name = lostSoul.name;
            CombatManager.combatManager.deck.drawPile.Add(copiedCard);
            CombatManager.combatManager.deck.DrawCard(1);

            AnimationUtilities.StartTimer(transform, 1f);

            ChangeStage(stage + 1);
        }else if (stage == 10){
            stageExecuted = false;

            if (AnimationUtilities.GetTimer(gameObject)) return;

            bool soulUsed = true;
            for (int i = 0; i < CombatManager.combatManager.deck.cardsInHand.Count; i++){
                if (CombatManager.combatManager.deck.cardsInHandAsCards[i].name == lostSoul.name){
                    soulUsed = false;
                }
            }
            if (soulUsed){
                ChangeStage(stage + 1);
            }
        }else if (stage == 11){
            NotificationManager.notificationManager.CloseNotificationWindow(0);
            NotificationManager.notificationManager.Notify(notifications[7], new Vector3(0, -200, 0));
            ChangeEndTurnButton(true);

            Card copiedCard = Instantiate(borke).ResetCard();
            copiedCard.name = borke.name;
            CombatManager.combatManager.deck.drawPile.Add(copiedCard);
            CombatManager.combatManager.deck.DrawCard(1);
        }
    }
}
