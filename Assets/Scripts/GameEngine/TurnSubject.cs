using System.Collections;

public class TurnSubject{

    public ArrayList observers;
    // Use this for initialization
    public TurnSubject()
    {
        observers = new ArrayList();
    }

    public void NotifyObservers(Player playerActive)
    {
        foreach(Observer o in observers)
        {
            if (o != null)
            {
                o.Notify(playerActive);
            }
        }
    }

    public void AddObserver(Observer o)
    {
        if (!observers.Contains(o))
        {
            observers.Add(o);
        }
    }
    
    public void RemoveObserver(Observer observer)
    {
        observers.Remove(observer);
    }
}
