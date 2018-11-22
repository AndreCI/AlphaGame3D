using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : Unit
{
    Animator anim;
    public Transform animTransform;
    public GameObject physicalAttackAnimation;
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
                    Attack(path[0]);
                }
            }
        }
    }
    protected override void Attack(Node target)
    {

        GameObject attackAnim = (GameObject)Instantiate(physicalAttackAnimation, target.position + attackOffset, new Quaternion(0, 0, 0, 0));
        Destroy(attackAnim, 5);
        anim.SetTrigger("Attack1Trigger");
        animTransform.localPosition = new Vector3(0f, 0f, 0f);
        base.Attack(target);

    }
    protected override void FinishMove()
    {
        anim.ResetTrigger("Moving");
        base.FinishMove();
    }

    public override IEnumerator StartMoving(bool hideUI=false)
    {
        StartCoroutine(base.StartMoving());
        if (path[0].Attackable(this.currentPosition))
        {
            Attack(path[0]);
        }
        else
        {
            anim.SetTrigger("Moving");
        }
        yield return new WaitForSeconds(3.0f);

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
