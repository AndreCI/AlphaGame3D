using UnityEngine;

public interface IObserver 
{
    void Notify(Player player, TurnSubject.NOTIFICATION_TYPE subjectType);
}
