using UnityEngine;

public abstract class Observer : MonoBehaviour
{
    public abstract void Notify(Player player);
}
