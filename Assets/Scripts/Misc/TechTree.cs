using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TechTree
{
    public void Initialize()
    {
        R baseR = new R("HallCenter", new List<R>());
        R barracks = new R("Barracks", new List<R> { baseR });
        R magicCenter = new R("MagicCenter", new List<R> { baseR });
        R warrior = new R("Warrior", new List<R> { barracks });
        R wizard = new R("Wizard", new List<R> { barracks });
        R fireblast = new R("Fireblast", new List<R> { magicCenter });
    }


    public class R
    {
        string name;
        List<R> requierments;
        public R(string n, List<R> r)
        {
            name = n;
            requierments = r;
        }
    }
}