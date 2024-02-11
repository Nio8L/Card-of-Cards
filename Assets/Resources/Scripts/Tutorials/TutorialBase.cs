using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TutorialBase : MonoBehaviour
{
    public Notification[] notifications;
    public int stage = 0;
    protected bool stageExecuted = false;
    protected bool checkIfNotificationIsGone = false;

    public GameObject cardShowCasePrefab;
    public GameObject slotArrowsPrefab;
    protected GameObject cardShowcaseInstance;
    protected GameObject slotArrowsInstance;

    protected Button endTurnButton;
    protected bool boolCheck;
    protected CardInCombat cardToTrack;
    void Start(){
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
    }
    void Update()
    {
        if (checkIfNotificationIsGone) ChangeStageAfterTextIsGone(stage+1);
        if (!stageExecuted) ExecuteChanges();
    }
    public abstract void ExecuteChanges();

    protected void ChangeStage(int newStage){
        if (newStage == stage) return;
        stageExecuted = false;
        stage = newStage;
    }

    void ChangeStageAfterTextIsGone(int newStage){
        if (NotificationManager.notificationManager.notifications.Count == 0){
            ChangeStage(newStage);
        }
    }

    protected void ChangeEndTurnButton(){
        endTurnButton.interactable = !endTurnButton.interactable;
    }

    protected void ChangeEndTurnButton(bool usable){
        endTurnButton.interactable = usable;
    }
}
