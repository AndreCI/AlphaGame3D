using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static void EatFood(Player currentPlayer)
    {
        currentPlayer.food = currentPlayer.foodPrediction;
        List<Unit> eatingUnits = currentPlayer.currentUnits.ToList(); //Don't forget to copy!
        eatingUnits.RemoveAll(u => u.foodConso == 0);
        int i = currentPlayer.food;
        if (i < eatingUnits.Count)
        {
            List<Unit> unitFed = GetRandomElements<Unit>(eatingUnits, i);
            foreach(Unit u in eatingUnits)
            {
                if (!unitFed.Contains(u))
                {
                    u.currentEffect.Add(new StarvationEffect(SpellUtils.EffectTypes.STARVING, u, 1));
                }
            }
        }
        currentPlayer.food -= eatingUnits.Count;
        currentPlayer.foodPrediction = 0; 
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
        {"cropfreeze", CropFreeze.Instance },
        {"manafreeze", ManaFreeze.Instance },
        {"frostlance", Frostlance.Instance },
        {"arcaneburn", Arcaneblast.Instance },
        {"arcanemissile", ArcaneMissile.Instance },
        {"arcanemirage", ArcaneMirage.Instance },
        {"earthlink", EarthLink.Instance },
        {"waveofvigor", WaveOfVigor.Instance },
        {"berserkerspirit", BerserkerSpirit.Instance },
        {"arcaneintellect", ArcaneIntellect.Instance },
        {"naturesblessing", NaturesBlessing.Instance }
        };

    public static void ApplyNotification(NotificationTypes notType, int amount, Player currentPlayer)
    {
        switch (notType)
        {
            case NotificationTypes.GOLD:
                currentPlayer.gold += amount;
                break;
            case NotificationTypes.MANA:
                currentPlayer.mana += amount;
                break;
            case NotificationTypes.FOOD:
                currentPlayer.foodPrediction += amount;
                break;
            case NotificationTypes.ACTION_POINT:
                currentPlayer.actionPoints += amount;
                break;
        }
    }

    public static Dictionary<NotificationTypes, Color> typesToColors = new Dictionary<NotificationTypes, Color>
    {
        { NotificationTypes.GOLD, Color.yellow },
        { NotificationTypes.MANA, Color.cyan },
        { NotificationTypes.ACTION_POINT, Color.red },
        { NotificationTypes.BUILDING, Color.red },
        { NotificationTypes.FOOD, Color.green },

        { NotificationTypes.BUFF_ATCK, Color.green },
        { NotificationTypes.DEBUFF_ATCK, Color.red },
        { NotificationTypes.BUFF_MVT, Color.blue },
        { NotificationTypes.BUFF_ARMOR, Color.grey },
        { NotificationTypes.HEAL, Color.green },
        { NotificationTypes.DEBUFF_MVT, Color.cyan },
        { NotificationTypes.DAMAGE, Color.red },

    };

    public enum NotificationTypes {
        BUILDING,
        GOLD,
        MANA,
        ACTION_POINT,
        FOOD,
        DAMAGE,
        DEBUFF_MVT,
        BUFF_ATCK,
        DEBUFF_ATCK,
        BUFF_MVT,
        BUFF_ARMOR,
        HEAL
    }
}
public static class hasComponent
{
    public static bool HasComponent<T>(GameObject flag) where T : Component
    {
        return flag.GetComponent<T>() != null;
    }
}