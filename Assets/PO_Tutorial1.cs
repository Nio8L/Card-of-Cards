using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PO_Tutorial1 : MonoBehaviour
{
    public Notification[] notifications;
    public int stage = 0;
    bool stageExecuted = false;
    bool checkIfNotificationIsGone = false;

    public GameObject cardShowCasePrefab;
    public GameObject slotArrowsPrefab;
    GameObject cardShowcaseInstance;
    GameObject slotArrowsInstance;

    Button endTurnButton;
    bool boolCheck;
    CardInCombat cardToTrack;
    void Start(){
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
    }
    void Update()
    {
        if (checkIfNotificationIsGone) ChangeStageAfterTextIsGone(stage+1);
        if (!stageExecuted) ExecuteChanges();
    }
    void ExecuteChanges(){
        stageExecuted = true;

        if (stage == 0){
            // Say welcome
            ChangeEndTurnButton();
            NotificationManager.notificationManager.Notify(notifications[0], new Vector3(0, -200, 0));
            checkIfNotificationIsGone = true;
        }else if (stage == 1){
            // Display card showcase
            NotificationManager.notificationManager.Notify(notifications[1], new Vector3(0, -200, 0));
            cardShowcaseInstance = Instantiate(cardShowCasePrefab, GameObject.Find("Canvas").transform);
            cardShowcaseInstance.transform.localScale = Vector3.one;
        }else if (stage == 2){
            // Display health text and arrow
            NotificationManager.notificationManager.Notify(notifications[2], new Vector3(0, -200, 0));
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(0), 0.5f, 0, 1);
        }else if (stage == 3){
            // Display attack text and arrow
            NotificationManager.notificationManager.Notify(notifications[3], new Vector3(0, -200, 0));
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(0), 0.5f, 0, 0);
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(1), 0.5f, 0, 1);
        }else if (stage == 4){
            // Display cost text and arrow
            NotificationManager.notificationManager.Notify(notifications[4], new Vector3(0, -200, 0));
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(1), 0.5f, 0, 0);
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(2), 0.5f, 0, 1);
        }else if (stage == 5){
            // Display sigils text and arrow
            NotificationManager.notificationManager.Notify(notifications[5], new Vector3(0, -200, 0));
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(2), 0.5f, 0, 0);
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(3), 0.5f, 0, 1);
        }else if (stage == 6){
            // Remove card showcase and start board explanation
            NotificationManager.notificationManager.Notify(notifications[6], new Vector3(0, -200, 0));

            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform.GetChild(3), 0.5f, 0, 0);
            AnimationUtilities.ChangeAlpha(cardShowcaseInstance.transform, 0.5f, 0, 0);

            cardShowcaseInstance.GetComponent<DestroyTimer>().enabled = true;
        }else if (stage == 7){
            checkIfNotificationIsGone = false;

            NotificationManager.notificationManager.Notify(notifications[7], new Vector3(0, -200, 0));
            ChangeStage(stage+1);
        }else if (stage == 8){
            // Keep checking until a card is found
            stageExecuted = false;
            
            bool result = false;
            for  (int i = 0; i < CombatManager.combatManager.playerCombatCards.Length; i++){
                if (CombatManager.combatManager.playerCombatCards[i] != null)
                {
                    cardToTrack = CombatManager.combatManager.playerCombatCards[i];
                    result = true;
                    break;
                }
                else if (CombatManager.combatManager.playerBenchCards[i] != null)
                {
                    cardToTrack = CombatManager.combatManager.playerBenchCards[i];
                    result = true;
                    break;
                }
            }
            if (result){
                NotificationManager.notificationManager.CloseNotificationWindow(0);
                Debug.Log(notifications[0].lines[0]);
                boolCheck = cardToTrack.benched;
                ChangeStage(stage+1);
            }
        }else if (stage == 9){
            // Spawn slot arrows and show combat slots
            checkIfNotificationIsGone = true;

            slotArrowsInstance = Instantiate(slotArrowsPrefab, GameObject.Find("Canvas").transform);
            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(1), 0.5f, 0, 1);
            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(2), 0.5f, 0, 1);

            NotificationManager.notificationManager.Notify(notifications[8], new Vector3(0, -200, 0));
        }else if (stage == 10){
            // Show bench slots
            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(0), 0.5f, 0, 1);
            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(3), 0.5f, 0, 1);

            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(1), 0.5f, 0, 0);
            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(2), 0.5f, 0, 0);

            NotificationManager.notificationManager.Notify(notifications[9], new Vector3(0, -200, 0));
        }else if (stage == 11){
            // Benching
            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(0), 0.5f, 0, 0);
            AnimationUtilities.ChangeAlpha(slotArrowsInstance.transform.GetChild(3), 0.5f, 0, 0);

            checkIfNotificationIsGone = false;
            NotificationManager.notificationManager.Notify(notifications[10], new Vector3(0, -200, 0));
            ChangeStage(stage+1);
        }else if (stage == 12){
            // Keep checking until the card is benched
            stageExecuted = false;
            if (cardToTrack.benched != boolCheck){
                ChangeStage(stage+1);
                NotificationManager.notificationManager.CloseNotificationWindow(0);
            }
        }else if (stage == 13){
            // Move the card to a combat slot
            if (cardToTrack.benched){
                NotificationManager.notificationManager.Notify(notifications[11], new Vector3(0, -200, 0));
                ChangeStage(stage+1);
            }else{
                ChangeStage(stage+2);
            }
        }else if (stage == 14){
            // Move the card to a combat slot
            stageExecuted = false;
            if (!cardToTrack.benched){
                NotificationManager.notificationManager.CloseNotificationWindow(0);
                ChangeStage(stage+1);
            }
        }else if (stage == 15){
            // End turn
            ChangeEndTurnButton();
            NotificationManager.notificationManager.Notify(notifications[12], new Vector3(0, -200, 0));
            ChangeStage(stage + 1);
        }else if (stage == 16){
            stageExecuted = false;
            if (CombatManager.combatManager.round != 1){
                ChangeStage(stage + 1);
            }
        }else if (stage == 17){
            // Show combat text
            NotificationManager.notificationManager.CloseNotificationWindow(0);
            checkIfNotificationIsGone = true;
            NotificationManager.notificationManager.Notify(notifications[13], new Vector3(0, -200, 0));
        }else if (stage == 18){
            // Show final text
            NotificationManager.notificationManager.Notify(notifications[14], new Vector3(0, -200, 0));
        }
    }

    void ChangeStage(int newStage){
        if (newStage == stage) return;
        stageExecuted = false;
        stage = newStage;
    }

    void ChangeStageAfterTextIsGone(int newStage){
        if (NotificationManager.notificationManager.notifications.Count == 0){
            ChangeStage(newStage);
        }
    }

    void ChangeEndTurnButton(){
        endTurnButton.interactable = !endTurnButton.interactable;
    }

    void ChangeEndTurnButton(bool usable){
        endTurnButton.interactable = usable;
    }
}
