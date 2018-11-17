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
        currentPlayer.foodPrediction = 4; //TODO:FOOD DEBUG
    }
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
    {//TODO: change this
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }
    public static Dictionary<string, Spell> stringToSpell = new Dictionary<string, Spell>{
        {"fireblast", Fireblast.Instance},
        {"firehammer", FireHammer.Instance },
        {"flammingswords", FlammingSwords.Instance },
        {"frostblast", Frostblast.Instance },
        {"frostlance", Frostlance.Instance },
        {"arcaneburn", Arcaneblast.Instance },
        {"arcaneintellect", ArcaneIntellect.Instance }
        };
}
public static class hasComponent
{
    public static bool HasComponent<T>(GameObject flag) where T : Component
    {
        return flag.GetComponent<T>() != null;
    }
}