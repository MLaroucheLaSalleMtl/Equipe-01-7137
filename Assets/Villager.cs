using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : unit
{
    public int Age = 1;
    float tot = 0;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        tot += Time.fixedDeltaTime;
        if(tot >= 120)
        {
            if(tot % 5 ==0)
            {

            }
            Age++;
            OnAgeChange();
        }
    }
    void OnAgeChange()
    {
        var e = rendies[0].material.color;
        e += Color.white / 3;
        ChangeColor(e);
        tot = 0;
    }

}
