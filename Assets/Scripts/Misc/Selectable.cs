using System;
using System.Collections.Generic;

public abstract class Selectable : Observer
{
    public Node currentPosition;
    public Player owner;
    public string cardName;
    public List<Type> unlock;
    public int goldCost;
    public int manaCost;
    public int actionPointCost;
    protected CardDisplay cardDisplay;

    private void Start()
    {
    }


    public abstract void UpdateCardDisplayInfo();
    public abstract void Select();
    public abstract void Unselect();
    public abstract List<Type> GetRequierements();
    public virtual void SetCurrentPosition(Node node)
    {
        currentPosition = node;
    }
}