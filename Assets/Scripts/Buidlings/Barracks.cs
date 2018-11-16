using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Barracks : Building
{
    void Start()
    {
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        unlock = new List<Type> { typeof(Warrior), typeof(Wizard), typeof(Brute) };
    }

    public override List<Type> GetRequierements()
    {
        return new List<Type> { typeof(HallCenter) };
    }

    void Update()
    {
        
    }

    public override void UpgradeToT2()
    {
        base.UpgradeToT2();
        unlock.Add(typeof(Warrior));
    }

}