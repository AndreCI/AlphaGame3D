using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Selectable : Observer
{
    public Node currentPosition;
    public Player owner;
    public string cardName;
    public int goldCost;
    public int manaCost;
    public int actionPointCost;
    protected CardDisplay cardDisplay;
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;

    public bool isTier2;
    private void Start()
    {
    }


    public abstract void UpdateCardDisplayInfo();
    public abstract void Select();
    public abstract void Unselect();
    public virtual void SetCurrentPosition(Node node)
    {
        currentPosition = node;
    }

    public IEnumerator FadeNotification(string notif, Utils.NotificationTypes type)
    {
        Color color = Utils.typesToColors[type];
        if (type == Utils.NotificationTypes.BUILDING)
        {
            float fontSize = notificationText.fontSize;

            notificationText.fontSize = fontSize / 2; //TODO: DEBUG
            notificationText.text = "BUILDING FOR " + notif + " TURNS";
            notificationText.fontSize = fontSize;
        }
        else
        {
            notificationText.text = notif;
        }
        notificationText.color = color;
        float duration = Time.time + 1.0f;
        Vector3 pos = notificationPanel.transform.localPosition;

        while (Time.time < duration)
        {
            notificationPanel.transform.localPosition = new Vector3(pos.x, pos.y + 1 - duration + Time.time, pos.z);
            notificationText.canvasRenderer.SetAlpha(duration - Time.time);
            yield return null;
        }
        notificationPanel.transform.localPosition = pos;
        yield break;
    }
}