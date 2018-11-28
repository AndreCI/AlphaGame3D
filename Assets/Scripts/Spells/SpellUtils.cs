using UnityEngine;
using System.Collections.Generic;

public class SpellUtils : MonoBehaviour
{
    public Sprite noRequirementSprite;
    public Sprite fireSprite;
    public Sprite frostSprite;
    public Sprite greenSprite;

    public static SpellUtils Instance;

    private void Awake()
    {
        Instance = this;
    }
    public Sprite GetSpriteFromSchoolOfMagic(SchoolOfMagic schoolOfMagic)
    {
        switch (schoolOfMagic)
        {
            case SchoolOfMagic.BASIC:
                return noRequirementSprite;
            case SchoolOfMagic.FIRE:
                return fireSprite;
            case SchoolOfMagic.FROST:
                return frostSprite;
            case SchoolOfMagic.GREEN:
                return greenSprite;
            default:
                throw new System.NotImplementedException("This school of magic has not been implemented yet.");
        }
    }

    public enum EffectTypes
    {
        NORMAL,
        FROST,
        BURN,
        ATTACK_MODIFIER,
        STARVING,
        ARMOR_GAIN,
        MVT_GAIN,
        NO_FOOD_CONSO,
        REGEN,
        VISION_MODIFIER
    }

    public static Dictionary<EffectTypes, string> effectDescriptionAbsolute = new Dictionary<EffectTypes, string>
    {
        {EffectTypes.STARVING, "<b>STARVING: </b>You need more food!" },
        {EffectTypes.FROST, "<b>FREEZE: </b>Start of Turn: Lose 1 Movement Point"},
        {EffectTypes.BURN, "<b>BURN: </b>Start of Turn: Lose 5 Health" },
        {EffectTypes.ATTACK_MODIFIER, "<b>ATTACK MODIFIED: </b>Attack is modified" },
        {EffectTypes.ARMOR_GAIN, "<b>ARMOR: </b>This unit has armor, which prevents damages taken" },
        {EffectTypes.MVT_GAIN, "<b>FAST: </b>Mouvement points gained at the start of the turn is modified" },
        {EffectTypes.NO_FOOD_CONSO, "<b>SELF SUSTAINING: </b>This unit does not consume food" },
        {EffectTypes.REGEN, "<b>REGENERATION: </b>Start of Turn: Recover some health" },
        {EffectTypes.VISION_MODIFIER, "<b>MODIFIED VISION: </b>Vision range is modified" }
    };
    public enum SchoolOfMagic
    {
        BASIC,
        FIRE,
        FROST,
        GREEN
    }
}