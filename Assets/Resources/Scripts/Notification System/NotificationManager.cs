using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager notificationManager;

    public Notification currentNotification;
    public int currentLineIndex = 0;

    public GameObject notificationObject;
    public TextMeshProUGUI notificationText;

    private GameObject notificationUI;
    private Button nextLineButton;

    private void Awake() {
        if(notificationManager == null){
            notificationManager = this;
        }else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
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