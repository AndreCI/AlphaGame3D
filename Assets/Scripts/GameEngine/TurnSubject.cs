using System.Collections.Generic;
using System.Linq;

public class TurnSubject{

    public Dictionary<IObserver, int> observers;
    public NOTIFICATION_TYPE subjectType;
    // Use this for initialization
    public TurnSubject(NOTIFICATION_TYPE subjectType_)
    {
        observers = new Dictionary<IObserver, int>();
        subjectType = subjectType_;
    }

    public void NotifyObservers(Player playerActive)
    {
        observers = observers.OrderBy(x=>x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        foreach(IObserver o in observers.Keys.ToList<IObserver>())
        {
            if (o != null)
            {
                o.Notify(playerActive, subjectType);
            }
            else
            {
                observers.Remove(o);
            }
        }
    }

    public void AddObserver(IObserver o, int priority=5)
    {
        if (!observers.ContainsKey(o))
        {
            observers.Add(o, priority);
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
        TARGET_DEATH,
        ATTACKING,
        NO_MOUVEMENT
    }
}
