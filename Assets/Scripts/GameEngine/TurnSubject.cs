using System.Collections.Generic;

public class TurnSubject{

    public List<IObserver> observers;
    public NOTIFICATION_TYPE subjectType;
    // Use this for initialization
    public TurnSubject(NOTIFICATION_TYPE subjectType_)
    {
        observers = new List<IObserver>();
        subjectType = subjectType_;
    }

    public void NotifyObservers(Player playerActive)
    {
        foreach(IObserver o in observers)
        {
            if (o != null)
            {
                o.Notify(playerActive, subjectType);
            }
        }
    }

    public void AddObserver(IObserver o)
    {
        if (!observers.Contains(o))
        {
            observers.Add(o);
        }
    }
    
    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }
    public enum NOTIFICATION_TYPE
    {
        START_OF_TURN,
        END_OF_TURN,
        UI_BUTTON_PRESSED,
        TARGET_DEATH
    }
}
