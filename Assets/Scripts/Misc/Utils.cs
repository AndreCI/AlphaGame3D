using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static void EatFood(Player currentPlayer)
    {
        currentPlayer.food = currentPlayer.foodPrediction;
        int i = currentPlayer.food;
        int j = currentPlayer.currentUnits.Count;
        if (i < currentPlayer.currentUnits.Count)
        {
            List<Unit> unitFed = GetRandomElements<Unit>(currentPlayer.currentUnits, i);
            foreach(Unit u in currentPlayer.currentUnits)
            {
                if (!unitFed.Contains(u))
                {
                    u.TakesDamage(10);
                }
            }
        }
        currentPlayer.food -= j;
        currentPlayer.foodPrediction = 0; //TODO:FOOD DEBUG
    }
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
    {//TODO: change this
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }
    public static Dictionary<string, Spell> stringToSpell = new Dictionary<string, Spell>{
        {"naturewrath", Naturewrath.Instance },
        {"fireblast", Fireblast.Instance},
        {"firehammer", FireHammer.Instance },
        {"flammingswords", FlammingSwords.Instance },
        {"frostblast", Frostblast.Instance },
        {"frostlance", Frostlance.Instance },
        {"arcaneburn", Arcaneblast.Instance },
        {"arcaneintellect", ArcaneIntellect.Instance }
        };

    public static void ApplyNotification(notificationTypes notType, int amount, Player currentPlayer)
    {
        switch (notType)
        {
            case notificationTypes.GOLD:
                currentPlayer.gold += amount;
                break;
            case notificationTypes.MANA:
                currentPlayer.mana += amount;
                break;
            case notificationTypes.FOOD:
                currentPlayer.foodPrediction += amount;
                break;
            case notificationTypes.ACTION_POINT:
                currentPlayer.actionPoints += amount;
                break;
        }
    }

    public static Dictionary<notificationTypes, Color> typesToColors = new Dictionary<notificationTypes, Color>
    {
        { notificationTypes.GOLD, Color.yellow },
        { notificationTypes.MANA, Color.blue },
        { notificationTypes.ACTION_POINT, Color.red },
        { notificationTypes.FOOD, Color.green }

    };

    public enum notificationTypes {
        GOLD,
        MANA,
        ACTION_POINT,
        FOOD
    }
}
public static class hasComponent
{
    public static bool HasComponent<T>(GameObject flag) where T : Component
    {
        return flag.GetComponent<T>() != null;
    }
}