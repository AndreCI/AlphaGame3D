using UnityEngine;
using System.Collections;

public abstract class RangedAttackAnimation : MonoBehaviour
{

    [HideInInspector]
    public Node source;
    [HideInInspector]
    public Node target;
    [Header("Ranged Attack Animation data")]
    public float delay;
    public float animationDuration;

    public abstract void ShowAttackPreview(Node source, Node target);
    public abstract void HideAttackPreview();
    public abstract void PlayAnimation();
}
