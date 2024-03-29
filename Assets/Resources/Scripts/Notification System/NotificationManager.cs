using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager notificationManager;

    public GameObject notificationObject;

    [SerializeField]
    public List<NotificationWindow> notifications;
    [SerializeField]
    public class NotificationWindow{
        private Notification currentNotification;
        private int currentLineIndex;
        private GameObject notificationUI;
        private TextMeshProUGUI notificationText;
        private Button nextLineButton;
        private Button previousLineButton;

        public NotificationWindow(Notification notification){
            //Instantiate the UI
            notificationUI = Instantiate(notificationManager.notificationObject, Vector3.zero, Quaternion.identity);
            notificationUI.transform.SetParent(notificationManager.transform);

            //Get the notification text and buttons
            notificationText = notificationUI.GetComponentInChildren<TextMeshProUGUI>();
            nextLineButton = notificationText.transform.GetChild(0).GetComponent<Button>();
            previousLineButton = notificationText.transform.GetChild(1).GetComponent<Button>();

            //Attach OnClick to the buttons
            nextLineButton.onClick.AddListener(NextLine);
            previousLineButton.onClick.AddListener(PreviousLine);

            nextLineButton.gameObject.SetActive(notification.closable);

            //Set the notification
            SetNotification(notification);
        }

        public NotificationWindow(Notification notification, Vector3 position){
            //Instantiate the UI
            notificationUI = Instantiate(notificationManager.notificationObject, Vector3.zero, Quaternion.identity);
            notificationUI.transform.SetParent(notificationManager.transform);

            //Gets the Background (which is a child of the Canvas) and sets its position
            notificationUI.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().localPosition = position;

           //Get the notification text and buttons
            notificationText = notificationUI.GetComponentInChildren<TextMeshProUGUI>();
            nextLineButton = notificationText.transform.GetChild(0).GetComponent<Button>();
            previousLineButton = notificationText.transform.GetChild(1).GetComponent<Button>();

            //Attach OnClick to the buttons
            nextLineButton.onClick.AddListener(NextLine);
            previousLineButton.onClick.AddListener(PreviousLine);

            nextLineButton.gameObject.SetActive(notification.closable);

            //Set the notification
            SetNotification(notification);
        }

        public NotificationWindow(Notification notification, Vector3 position, float duration){
            //Instantiate the UI
            notificationUI = Instantiate(notificationManager.notificationObject, Vector3.zero, Quaternion.identity);
            notificationUI.transform.SetParent(notificationManager.transform);

            //Gets the Background (which is a child of the Canvas) and sets its position
            notificationUI.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().localPosition = position;

            //Get the notification text and buttons
            notificationText = notificationUI.GetComponentInChildren<TextMeshProUGUI>();
            nextLineButton = notificationText.transform.GetChild(0).GetComponent<Button>();
            previousLineButton = notificationText.transform.GetChild(1).GetComponent<Button>();

            //Attach OnClick to the buttons
            nextLineButton.onClick.AddListener(NextLine);
            previousLineButton.onClick.AddListener(PreviousLine);

            nextLineButton.gameObject.SetActive(notification.closable);

            //Set the notification
            SetNotification(notification);
            Animate(duration, 1);
        }

        public Notification GetNotification(){
            return currentNotification;
        }

        public GameObject GetNotificationUI(){
            return notificationUI;
        }

        public int GetCurrentLineIndex(){
            return currentLineIndex;
        }

        //Go to the next line in the notification
        public void NextLine(){
            //Close the notification if this was the last line, otherwise go to the next line
            if(currentLineIndex == currentNotification.lines.Count - 1){
                CloseNotificationWindow();
            }else{
                currentLineIndex++;

                SetLine(currentLineIndex);
                
                previousLineButton.gameObject.SetActive(true);
            }
        }

        //Go to the previous line in the notification
        public void PreviousLine(){
            currentLineIndex--;

            if(currentLineIndex == 0){
                previousLineButton.gameObject.SetActive(false);
            }

            SetLine(currentLineIndex);
        }

        //Set the notification
        public void SetNotification(Notification notification){
            currentNotification = notification;

            currentLineIndex = 0;

            SetLine(currentLineIndex);
        }

        //Set the line to the given index
        public void SetLine(int index){
            notificationText.text = currentNotification.lines[index];
        }

        public void Animate(float duration, float delay){
            CanvasGroup canvas = notificationUI.GetComponentInChildren<CanvasGroup>();
            AnimationUtilities.ChangeCanvasAlpha(canvas.transform, duration, delay, 0);
        }

        //Close this notification
        public void CloseNotificationWindow(){
            Destroy(notificationUI);
            notificationManager.notifications.Remove(this);
        }
    };

    private void Awake() {
        if(notificationManager == null){
            notificationManager = this;
        }else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        notifications = new();
    }

    //Displays the given notification
    public void Notify(Notification notification){
        NotificationWindow newNotification = new NotificationWindow(notification);
        
        notifications.Add(newNotification);
    }

    //Displays a given notification with a position
    public void Notify(Notification notification, Vector3 position){
        NotificationWindow newNotification = new NotificationWindow(notification, position);

        notifications.Add(newNotification);
    }

    //Displays a notificaton and automatically closes it after a given duration
    public void NotifyAutoEnd(Notification notification, Vector3 position, float duration){
        NotificationWindow newNotification = new NotificationWindow(notification, position, duration);

        notifications.Add(newNotification);
        
        //Close the notification after the given duration
        StartCoroutine(AutoCloseNotification(newNotification, duration));
    }

    //Closes the notification after a given delay
    public IEnumerator AutoCloseNotification(NotificationWindow notification, float delay){
        yield return new WaitForSeconds(delay);

        notification.CloseNotificationWindow();
    }

    //Goes to the next line of the notification
    public void NextLine(int index){
        notifications[index].NextLine();
    }

    //Closes the notification
    public void CloseNotificationWindow(int index){
        notifications[index].CloseNotificationWindow();
    }

    //Close all the notifications if they shouldn't persist between scenes
    public void SceneChange(Scene scene, LoadSceneMode mode){
        for(int i = 0; i < notifications.Count; i++){
            if(!notifications[i].GetNotification().persistBetweenScenes){
                notifications[i].CloseNotificationWindow();
                i--;
            }
        }
    }

    public void SetActiveNotificaitons(bool active){
        for(int i = 0; i < notifications.Count; i++){
            notifications[i].GetNotificationUI().SetActive(active);
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += SceneChange;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= SceneChange;
    }
}
