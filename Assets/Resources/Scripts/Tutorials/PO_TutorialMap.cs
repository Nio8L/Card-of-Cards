using UnityEngine;

public class PO_TutorialMap : MonoBehaviour
{
    public Notification notification;
    public Notification arrowNotification;
    public Notification mapLegendNotification;
    public int stage = 0;

    public GameObject arrow;

    bool hasNodeArrow = false;
    bool hasLegendArrow = false;

    bool hasNodeNotification = false;
    bool hasLegendNotification = false;

    Canvas canvas;
    GameObject nodeArrow;
    GameObject legendArrow;

    private void Start() {
        NotificationManager.notificationManager.Notify(notification);
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    private void Update() {
        if(NotificationManager.notificationManager.notifications.Count == 0 ||
           NotificationManager.notificationManager.notifications[0].GetCurrentLineIndex() == 6){
            MapManager.mapManager.canTravel = true;
            MapManager.mapManager.canScroll = true;
        }else if(NotificationManager.notificationManager.notifications[0].GetCurrentLineIndex() == 3){
            if (!hasNodeArrow)
            {
                hasNodeArrow = true;
                hasNodeNotification = true;
                
                nodeArrow = Instantiate(arrow, canvas.transform.position, Quaternion.Euler(0, 0, 90));
                
                nodeArrow.transform.SetParent(canvas.transform);
                
                nodeArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                nodeArrow.transform.localPosition = new Vector3(300, -325, 0);
    
                NotificationManager.notificationManager.Notify(arrowNotification, new Vector3(600, -325, 0));
            }
        }else{
            if(nodeArrow != null){
                Destroy(nodeArrow);
            }

            if(NotificationManager.notificationManager.notifications.Count > 1 && hasNodeNotification){
                NotificationManager.notificationManager.CloseNotificationWindow(1);
                hasNodeNotification = false;
            }

            if(NotificationManager.notificationManager.notifications[0].GetCurrentLineIndex() == 4){
                if (!hasLegendArrow)
                {
                    hasLegendArrow = true;
                    hasLegendNotification = true;
    
                    legendArrow = Instantiate(arrow, canvas.transform.position, Quaternion.Euler(0, 0, 180));
    
                    legendArrow.transform.SetParent(canvas.transform);
    
                    legendArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                    legendArrow.transform.localPosition = new Vector3(-840, -70, 0);
        
                    NotificationManager.notificationManager.Notify(mapLegendNotification, new Vector3(-825, 100, 0));
                }
            }else{
                if(legendArrow != null){
                    Destroy(legendArrow);
                }

                if(NotificationManager.notificationManager.notifications.Count > 1 && hasLegendNotification){
                    NotificationManager.notificationManager.CloseNotificationWindow(1);
                    hasLegendNotification = false;
                }
            }
        }
    }
}
