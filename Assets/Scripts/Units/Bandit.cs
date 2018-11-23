using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : Unit
{
    Animator anim;
    public Transform animTransform;
    public GameObject physicalAttackAnimation;
    public Vector3 attackOffset;
    public TurnSubject OnTargetDeathTurnSubject;
    public BasicUnitAbility ability;

    // Use this for initialization
    public override void Setup()
    {
        anim = GetComponentInChildren<Animator>();
        anim.logWarnings = false;
        base.Setup();
        OnTargetDeathTurnSubject = new TurnSubject(TurnSubject.NOTIFICATION_TYPE.TARGET_DEATH);
        OnTargetDeathTurnSubject.AddObserver(ability);
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            MoveStep();
        }
    }

    protected override IEnumerator Attack(Node target, bool riposte)
    {
        Unit attacked = target.unit;
        GameObject attackAnim = (GameObject)Instantiate(physicalAttackAnimation, target.position + attackOffset, new Quaternion(0, 0, 0, 0));
        Destroy(attackAnim, 5);
        anim.SetTrigger("Attack1Trigger");
        animTransform.localPosition = new Vector3(0f, 0f, 0f);
        yield return StartCoroutine(base.Attack(target, riposte));

        if (!TurnManager.Instance.inactivePlayer.currentUnits.Contains(attacked))
        {
            OnTargetDeathTurnSubject.NotifyObservers(owner);
        }
    }
    protected override void FinishMove()
    {
        anim.ResetTrigger("Moving");
        base.FinishMove();
    }

    public override IEnumerator StartMoving()
    {
        anim.SetTrigger("Moving");
        StartCoroutine(base.StartMoving());
        yield return new WaitForSeconds(currentMovementPoints<1? 0.2f :currentMovementPoints - 1.0f);

    }

    public void FootR()
    {
    }
    public void FootL()
    {
        
    }
    public void Hit()
    {
        animTransform.localPosition = new Vector3(0f, 0f, 0f);
    }


    public override Type GetSpawnPoint()
    {
        return typeof(Barracks);
    }
}
