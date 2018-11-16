using UnityEngine;

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

    public enum UnitEffect
    {
        NORMAL,
        FROST,
        BURN
    }

    public static void ApplyEffect(Unit u, UnitEffect e)
    {

    }
    public enum SchoolOfMagic
    {
        BASIC,
        FIRE,
        FROST
    }
}