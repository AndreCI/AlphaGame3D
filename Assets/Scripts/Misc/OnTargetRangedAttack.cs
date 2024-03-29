﻿using UnityEngine;
using System.Collections;

public class OnTargetRangedAttack : RangedAttackAnimation
{
    public GameObject projectile;
    public ParticleSystem hitAnimation;
    public ParticleSystem previewAnimation;

    public override void HideAttackPreview()
    {

        previewAnimation.Stop();
    }

    public override void PlayAnimation()
    {
        StartCoroutine(WaitDelay());
    }

    private IEnumerator WaitDelay()
    {
        yield return new WaitForSeconds(delay);
        hitAnimation.Play();
    }

    public override void ShowAttackPreview(HexCell source, HexCell target)
    {
        projectile.transform.position = target.Position;
        if (!TurnManager.Instance.currentPlayer.isAi)
        {
            previewAnimation.Play();
        }
        
    }
}
