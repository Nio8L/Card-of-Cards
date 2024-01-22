using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager notificationManager;

    public Notification currentNotification;
    public int currentLineIndex = 0;

    public GameObject notificationObject;
    private TextMeshProUGUI notificationText;

    private GameObject notificationUI;
    private Button nextLineButton;

    private BoxCollider2D hitBox;

    private void Awake() {
        if(notificationManager == null){
            notificationManager = this;
        }else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        hitBox = GetComponent<BoxCollider2D>();
    }

    //Displays the given notification
    public void Notify(Notification notification){
        //Instantiate the UI
        notificationUI = Instantiate(notificationObject, transform.position, Quaternion.identity);

        //Get the text and button
        notificationText = notificationUI.GetComponentInChildren<TextMeshProUGUI>();
        nextLineButton = notificationUI.GetComponentInChildren<Button>();
        
        //Add the OnClick function to the button
        nextLineButton.onClick.AddListener(OnClick);

        //Set the notification
        SetNotification(notification);

        hitBox.enabled = true;
    }

    public void Notify(Notification notification, Vector3 position){
        //Instantiate the UI
        notificationUI = Instantiate(notificationObject, transform.position, Quaternion.identity);

        notificationUI.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().localPosition = position;

        //Get the text and button
        notificationText = notificationUI.GetComponentInChildren<TextMeshProUGUI>();
        nextLineButton = notificationUI.GetComponentInChildren<Button>();
        
        //Add the OnClick function to the button
        nextLineButton.onClick.AddListener(OnClick);

        //Set the notification
        SetNotification(notification);

        hitBox.enabled = true;
    }

    //Automatically closes the notification window after the given duration
    public void NotifyAutoEnd(Notification notification, float duration){
        Notify(notification);

        CanvasGroup canvas = notificationUI.GetComponentInChildren<CanvasGroup>();

        AnimationUtilities.ChangeCanvasAlpha(canvas.transform, duration, 1, 0);
        Invoke(nameof(CloseNotificationWindow), duration + 1);
    }

    public void NotifyAutoEnd(Notification notification, float duration, Vector3 position){
        Notify(notification, position);

        CanvasGroup canvas = notificationUI.GetComponentInChildren<CanvasGroup>();

        AnimationUtilities.ChangeCanvasAlpha(canvas.transform, duration, 1, 0);
        Invoke(nameof(CloseNotificationWindow), duration + 1);
    }

    //Changes the currently shown notification
    public void SetNotification(Notification notification){
        //Set the new notification
        currentNotification = notification;
        
        //Reset the current line back to 0
        currentLineIndex = 0;
        
        //Display the first line from the notification
        SetLine(currentLineIndex);
    }

    //Changes which line from the current notification is displayed
    public void SetLine(int index){
        notificationText.text = currentNotification.lines[index];
    }

    //Goes to the next line of the notification
    public void NextLine(){
        currentLineIndex++;
        
        notificationText.text = currentNotification.lines[currentLineIndex];
    }

    //Closes the notification and resets all the components
    public void CloseNotificationWindow(){
        notificationText = null;
        Destroy(notificationUI);

        currentLineIndex = 0;
        currentNotification = null;

        //Disable the hitbox
        hitBox.enabled = false;
        
        CancelInvoke(nameof(CloseNotificationWindow));
    }

    //This function is called when the button from the NotificationUI is pressed
    public void OnClick(){
        //Close the notification if this is the last line, otherwise display the next line
        if(currentLineIndex == currentNotification.lines.Count - 1){
            CloseNotificationWindow();
        }else{
            NextLine();
        }
    }
}
