using UnityEngine;
using System.Collections.Generic;

public class SpellUtils : MonoBehaviour
{
    public Sprite noRequirementSprite;
    public Sprite fireSprite;
    public Sprite frostSprite;

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
            default:
                throw new System.NotImplementedException("This school of magic has not been implemented yet.");
        }
    }

    public enum EffectTypes
    {
        NORMAL,
        FROST,
        BURN,
        ATTACK_MODIFIER
    }

    public static Dictionary<EffectTypes, string> effectDescriptionAbsolute = new Dictionary<EffectTypes, string>
    {
        {EffectTypes.FROST, "<b>FREEZE: </b>Start of Turn: Lose 1 Movement Point"},
        {EffectTypes.BURN, "<b>BURN: </b>Start of Turn: Lose 5 Health" },
        {EffectTypes.ATTACK_MODIFIER, "<b>ATTACK MODIFIED: </b>Attack is modified" }
    };
    public enum SchoolOfMagic
    {
        BASIC,
        FIRE,
        FROST
    }
}