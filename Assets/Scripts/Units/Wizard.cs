using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Unit
{
    Animator anim;
    public Transform animTransform;
    public GameObject magicAttackAnimation;
    public Vector3 attackOffset;

    // Use this for initialization
    public override void Setup()
    {
        anim = GetComponentInChildren<Animator>();
        anim.logWarnings = false;
        base.Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            MoveStep();
        }
    }

    protected override void MoveStep()
    {
        float delta = 0.5f;
        Vector3 currentAnimBodyPosition = anim.bodyPosition;
        if (path.Count == 0)
        {
            FinishMove();
        }
        if ((currentAnimBodyPosition.x <= path[0].position.x + delta && currentAnimBodyPosition.x >= path[0].position.x - delta) && (currentAnimBodyPosition.z <= path[0].position.z + delta && currentAnimBodyPosition.z >= path[0].position.z - delta))
        {
            //If anim reached the node, update the sphere and reset the anim
            Vector3 animPos = new Vector3(animTransform.localPosition.x, 0, animTransform.localPosition.z);
            animPos = Quaternion.Euler(0, movementSphere.localEulerAngles.y, 0) * animPos;
            movementSphere.localPosition = movementSphere.localPosition + animPos;
            animTransform.localPosition = new Vector3(0f, 0f, 0f);
            base.MoveStep();

            path.Remove(path[0]);
            if (path.Count == 0)
            {
                FinishMove();
            }
            else
            {
                FaceNextNode(path[0]);
                if (path[0].Attackable(this.currentPosition))
                {
                    StartCoroutine(Attack(path[0], false));
                }else if (path[path.Count - 1].Attackable(this.currentPosition) && path.Count <= range)
                {
                    StartCoroutine(Attack(path[path.Count - 1], false));
                }
                
            }
        }
    }
    protected override IEnumerator Attack(Node target, bool riposte)
    {
        GameObject attackAnim = (GameObject)Instantiate(magicAttackAnimation, target.position + attackOffset, new Quaternion(0, 0, 0, 0));
        Destroy(attackAnim, 5);
        anim.SetTrigger("Attack1Trigger");
        animTransform.localPosition = new Vector3(0f, 0f, 0f);
        yield return StartCoroutine(base.Attack(target, riposte));
    }
    protected override void FinishMove()
    {
        base.FinishMove();
        anim.ResetTrigger("Moving");
    }

    public override IEnumerator StartMoving(bool hideUI = false)
    {
        StartCoroutine(base.StartMoving(hideUI:hideUI));
        if (path[0].Attackable(this.currentPosition))
        {
            StartCoroutine(Attack(path[0], false));
        }
        else if (path[path.Count - 1].Attackable(this.currentPosition) && path.Count <= range)
        {
            StartCoroutine(Attack(path[path.Count - 1], false));
        }
        else
        {
            anim.SetTrigger("Moving");
        }
        yield return new WaitForSeconds(currentMovementPoints < 1 ? 0.2f : currentMovementPoints - 1.0f);

    }

    public override IEnumerator AITransitionToMove()
    {
        anim.SetTrigger("Moving");
        yield return new WaitForSeconds(currentMovementPoints < 1 ? 0.2f : currentMovementPoints - 1.0f);
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
