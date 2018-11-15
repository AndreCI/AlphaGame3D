using UnityEngine;
using System.Collections;

public abstract class Observer : MonoBehaviour
{
    public abstract void Notify(Player player);
}
