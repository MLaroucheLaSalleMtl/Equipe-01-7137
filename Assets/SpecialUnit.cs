using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUnit : unit
{

    [Header("Special")]
    public float Experience= 0;
    public int level = 0;
    public float BaseEXPNeed = 5;
    
    public void LevelUP()
    {
        Experience -= BaseEXPNeed * 2 * Mathf.Exp(level);
        level++;
        Stat += stats.Rand;
        if (Experience > BaseEXPNeed * 2 * Mathf.Exp(level))
            LevelUP();
    }
    public stats Stat;
    [System.Serializable]
    public struct stats
    {
        public float STR, AGI, PER, DEX, END;

        public static stats Rand 
        {
            get
            {
                var e = Random.Range(0, 100);
                if(e > 90)
                {
                    print("Unit Critical Level up!");
                    return new stats(1, 1, 1, 1, 1);
                }
                  

                return new stats(Random.Range(0,1), Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));       
            }
        }
        public stats (float str, float agi, float per, float dex, float end)
        {
            STR = str;
            AGI = agi;
            PER = per;
            DEX = dex;
            END = end;
        }
        
        public static stats operator + (stats t1, stats t2)
        {
            var t3 = new stats();
            t3.DEX = t1.DEX + t2.DEX;
            t3.STR = t1.STR + t2.STR;
            t3.AGI= t1.AGI + t2.AGI;
            t3.END = t1.END + t2.END;
            t3.PER = t1.PER + t2.PER;
            return t3;

        }
    }

    protected override void OnKill(entity z)
    {
        base.OnKill(z);
        if(z is unit)
        {
            var y = z as unit;
            Experience += (y.getAttack + y.getDefense + y.GetMovingSpeed +y.GetAttackSpeed/2);
        }
     
    }
    public void GainEXP(float z)
    {
        Experience += z;
        if (Experience > BaseEXPNeed * 2 * Mathf.Exp(level))
            LevelUP();
    }

    public override float GetMovingSpeed
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
    public override float GetDetectionRange  
    {
        get { return base.GetDetectionRange + Stat.PER / 5; }
    }

    public override float GetAttackSpeed  
    {
        get { return base.GetAttackSpeed - Stat.DEX / 40; }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
