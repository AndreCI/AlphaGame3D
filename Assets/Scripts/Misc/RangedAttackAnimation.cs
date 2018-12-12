using UnityEngine;
using System.Collections;

public abstract class RangedAttackAnimation : MonoBehaviour
{

    [HideInInspector]
    public HexCell source;
    [HideInInspector]
    public HexCell target;
    [Header("Ranged Attack Animation data")]
    public float delay;
    public float animationDuration;

    public abstract void ShowAttackPreview(HexCell source, HexCell target);
    public abstract void HideAttackPreview();
    public abstract void PlayAnimation();
}
