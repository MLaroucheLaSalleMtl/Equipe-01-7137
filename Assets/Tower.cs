using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : fortification
{

    public List<unit> Units;
    public float Range = 5;
    public float Atk = 1;
    public GameObject munition;

    public override void interact(entity e, float efficiency = 0)
    {
        base.interact(e, efficiency);
        Protect();


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }

    Collider[] _col;
    float aitimer = 0;
    public void Protect()
    {
        aitimer += Time.fixedDeltaTime;
        if (aitimer > 1f)
        {
            var s = Physics.OverlapSphereNonAlloc(transform.position, Range, _col, GameManager.instance.Interatable, QueryTriggerInteraction.Collide);
            for (int i = s - 1; i >= 0; i--)
            {

                if (_col[i].gameObject.GetComponent<entity>())
                {
                    var sauce = _col[i].gameObject.GetComponent<entity>();
                    if (!sauce) continue;

                    if (GetOwner == null && sauce.GetOwner == null) continue;
                    if ((sauce.gameObject == this.gameObject))
                        continue;
                    if (GetOwner != null && sauce.GetOwner != null && sauce.GetOwner == GetOwner) continue;

                    else
                    {

                        sauce.TakeDamage(Atk);
                        //Returning right now will improve performance
                        return;
                    }


                }

            }

            aitimer = 0;
        }
    }

}
