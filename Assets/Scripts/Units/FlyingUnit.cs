using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlyingUnit : Unit
{

    public override void Setup()
    {
        base.Setup();
        TurnManager.Instance.EndTurnSubject.AddObserver(this);
    }
    public override bool IsValidDestination(HexCell cell)
    {
        if (flying)
        {
            return cell.IsExplored(owner) && (!cell.unit || cell.unit.owner != owner) && !cell.building;
        }
        else
        {
            return base.IsValidDestination(cell);
        }
    }
    public override int GetMoveCost(
    HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        if (flying) {
            if (!IsValidDestination(toCell))
            {
                return -1;
            }
        else
        {
                //	moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
                //	moveCost +=
                //		toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
                return 1;
            }
        }
        else
        {
            return base.GetMoveCost(fromCell, toCell, direction);
        }
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        base.Notify(player, type);
        if(type == TurnSubject.NOTIFICATION_TYPE.END_OF_TURN && 
            player == owner && 
            currentMovementPoints == maxMovementPoints &&
            !flying)
        {
            StartFlying();
        }
    }
    private IEnumerator ChangeHeight(float amount, float duration=1.2f, int iterations=40, float waitValue=1f)
    {
        yield return new WaitForSeconds(waitValue);
        for(int i = 0; i < iterations; i++)
        {
            movementSphere.position = new Vector3(movementSphere.position.x,
                movementSphere.position.y + (amount / iterations),
                movementSphere.position.z);
            yield return new WaitForSeconds(duration / iterations);
        }
        yPositionOffset += amount;
    }

    public void StartFlying()
    {
        flying = true;
        anim.SetTrigger("Flying");
        
        StartCoroutine(ChangeHeight(10, duration:0.5f, waitValue:1.5f));

    }
    public void StopFlying()
    {
        flying = false;
        anim.ResetTrigger("Flying");
        StartCoroutine(ChangeHeight(-10, duration:0.6f));
    }
    public override void TakesDamage(int amount, bool unsafeDeath = false)
    {
        int baseHealth = currentHealth;
        base.TakesDamage(amount, unsafeDeath);
        if(currentHealth>0 && flying && currentHealth != baseHealth && !currentPosition.IsUnderwater)
        {
            StopFlying();
        }
    }

}
