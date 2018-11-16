using System;
using System.Collections.Generic;

public class Shrine : Building
{
    public SpellUtils.SchoolOfMagic schoolOfMagic;

    public override List<Type> GetRequierements()
    {
        return new List<Type> { typeof(MagicCenter) };
    }

    // Use this for initialization
    void Start()
    {
        owner.schoolOfMagicLevels[schoolOfMagic] += 1;
        unlock = new List<Type> {};
    }
}
