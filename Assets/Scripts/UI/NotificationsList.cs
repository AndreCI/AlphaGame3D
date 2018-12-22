using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationsList : MonoBehaviour
{
    public static NotificationsList Instance;
    
    public int maxNumberNotifications = 10;
    public Sprite startGameSprite;
    public Sprite needFoodSprite;
    public Sprite spellCastableSprite;
    public Sprite buildingFinishSprite;
    public Sprite unitDeathSprite;
    public Notification prefab;
    private Dictionary<Notification.NOTIFICATION_TYPE, Sprite> typeToSprite;
    private List<Notification> notifications;
    private Stack<Notification> waitingNotifications;
    private List<GameObject> notificationsHandler;
    private Stack<Notification> inactiveNotifications;
    // Use this for initialization
    void Start()
    {
        notifications = new List<Notification>();
        notificationsHandler = new List<GameObject>();
        inactiveNotifications = new Stack<Notification>();
        waitingNotifications = new Stack<Notification>();
        typeToSprite = new Dictionary<Notification.NOTIFICATION_TYPE, Sprite>
        {
            {Notification.NOTIFICATION_TYPE.START_GAME, startGameSprite },
            {Notification.NOTIFICATION_TYPE.NEED_FOOD, needFoodSprite },
            {Notification.NOTIFICATION_TYPE.SPELL_CASTABLE, spellCastableSprite },
            {Notification.NOTIFICATION_TYPE.BUILDING_FINISH, buildingFinishSprite },
            {Notification.NOTIFICATION_TYPE.UNIT_DEATH, unitDeathSprite }
        };
        Instance = this;
        for(int i =0; i< maxNumberNotifications; i++)
        {

            GameObject handler = new GameObject();

            handler.AddComponent<RectTransform>();
            handler.transform.SetParent(transform);
            handler.transform.SetAsFirstSibling();
            handler.transform.localScale = new Vector3(1, 1, 1);
            notificationsHandler.Add(handler);
            Notification notif = Instantiate(prefab, transform.parent);
            notif.gameObject.SetActive(false);
            inactiveNotifications.Push(notif);
        }
    }

    public void ClearNotifications()
    {
        while (notifications.Count > 0)
        {
            Notification notif = notifications[0];
            notif.Remove();
            notifications.Remove(notif);
            inactiveNotifications.Push(notif);
        }
        while (waitingNotifications.Count > 0)
        {
            inactiveNotifications.Push(waitingNotifications.Pop());
        }
    }

    public void AddNotification(string text, Notification.NOTIFICATION_TYPE type, HexCell emitter = null)
    {
        if(inactiveNotifications.Count == 0)
        {
            Notification newNotif = Instantiate(prefab, transform.parent);
            newNotif.gameObject.SetActive(false);
            inactiveNotifications.Push(newNotif);
        }
        Notification notif = inactiveNotifications.Pop();
        notif.Setup(text, type, typeToSprite[type], target_:emitter);
        if (notifications.Count >= maxNumberNotifications)
        {
            waitingNotifications.Push(notif);
        }
        else
        {
            GameObject handler = notificationsHandler[notifications.Count];
            notif.gameObject.SetActive(true);
            notif.Add(handler);
            notifications.Add(notif);
        }
    }
    public void RemoveNotification(Notification target)
    {
        int idx = notifications.IndexOf(target);
        notifications.Remove(target);
        for(int i = idx; i < notifications.Count; i++)
        {
            notifications[i].ChangeHandler(notificationsHandler[i]);

        }
        inactiveNotifications.Push(target);
        if (waitingNotifications.Count > 0 && notifications.Count < maxNumberNotifications)
        {
            Notification notif = waitingNotifications.Pop();
            GameObject handler = notificationsHandler[notifications.Count];
            notif.gameObject.SetActive(true);
            notif.Add(handler);
            notifications.Add(notif);

        }
    }
    

   
}
