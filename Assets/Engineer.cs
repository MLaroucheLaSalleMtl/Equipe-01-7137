﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : unit
{
    public override void AI()
    {

        aitimer += Time.fixedDeltaTime;

        if (aitimer > .3f)
        {
           
            if (!target && TargetToHunt.Count > 0)
            {

                target = TargetToHunt.Dequeue();
                Attack(target);
                aitimer = 0;
                return;
            }

            if (Ordered) { aitimer = 0; return; }
            float dist = GetDetectionRange;
            var s = Physics.OverlapSphere(transform.position, GetDetectionRange, GameManager.instance.Unit, QueryTriggerInteraction.Collide);
            for (int i = s.Length - 1; i >= 0; i--)
            {

                if (s[i].gameObject.GetComponent<entity>())
                {
                    var sauce = s[i].gameObject.GetComponent<entity>();

                    //ShortCut

                    if (!sauce) continue;
                    if (GetOwner == null && sauce.GetOwner == null) continue;
                    if (sauce.GetOwner != GetOwner) continue;
                    if (!(sauce is building)) continue;
                    if ((sauce.gameObject == this.gameObject)) continue;
                  
                    if ( sauce.Hp < sauce.GetMaxmimumHP && Vector3.Distance(transform.position, sauce.transform.position) < dist)
                    {
                            
                        dist = Vector3.Distance(transform.position, sauce.transform.position);
                        agi.SetDestination(sauce.transform.position);
                        agi.isStopped = false;
                        _attack(sauce);

                    }
                    else
                    {
                        continue;
                    }



                    aitimer = 0;
                    //Returning right now will improve performance
                    return;

                }

            }

            aitimer = 0;
        }


    }
    public override float getAttack => -Mathf.Abs(base.getAttack);
}
