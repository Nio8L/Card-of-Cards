using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PO_TutorialHunt : HuntManager
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
    public void ExecuteChanges(){
        stageExecuted = true;
        if (stage == 0){
            // Say welcome
            ChangeEndTurnButton();
            NotificationManager.notificationManager.Notify(notifications[0], new Vector3(0, -200, 0));
            checkIfNotificationIsGone = true;
        }else if (stage == 1){
            ChangeEndTurnButton();
            NotificationManager.notificationManager.Notify(notifications[1], new Vector3(0, -200, 0));
            checkIfNotificationIsGone = false;
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
