using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUnit : unit
{

    [Header("Special")]
    public float Experience= 1;
    public int level = 0;

    public stats Stat;
    [SerializeField]
    public struct stats
    {
        public float STR, AGI, PER, DEX, END;
    }
    protected override float GetMovingSpeed
    {
        get { return base.GetMovingSpeed + Stat.AGI / 10; }
    }
    public override float getAttack
    {
        get { return base.getAttack + Stat.STR / 20; }
    }

    public override float getDefense  
    {
        get { return base.getDefense + Stat.END / 30; }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
