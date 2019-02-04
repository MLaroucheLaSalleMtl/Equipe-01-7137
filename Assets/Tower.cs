using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : fortification
{

    public List<unit> Units;
    public float Range = 5;
    public float Atk = 1;
    public GameObject ShootingPoint;
    public int munition;
 
    public int MinimumGuard = 2;
    public float RateOfFire = 0;
    public override void interact(entity e, float efficiency = 0)
    {
        base.interact(e, efficiency);

        if(!IsBeingBuild)
        Protect();


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }

    public  void ProduceMissile(int x, Vector3 A, Vector3 B)
    {
        print("missile");
         StartCoroutine(_missile(GameManager.instance.Missiles[x], A, B));
    }
    
 
     public static IEnumerator _missile(GameObject Missile, Vector3 A, Vector3 B,float spd = 7f)
    {
        var D = (B - A).normalized;
        var e = Instantiate(Missile, A, Quaternion.Euler(D));
        e.transform.forward = D;
        
        var t = 0f;
        while(t < 1)
        {
            e.transform.position = Vector3.Lerp(A, B, t);
            e.transform.forward = D;
            t += Time.fixedDeltaTime *spd;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(.2f);
        Destroy(e.gameObject);
        yield break;
    }
    Collider[] _col = new Collider[100];
    float aitimer = 0;

 
    public void Attack(entity e)
    {
        print(this.name + " attacks " + e.name + " for " + Atk + " damage");
        foreach (var item in Units)
        {
            item.transform.LookAt(e.transform);
        }
        e.TakeDamage(Atk);
        ProduceMissile(munition, ShootingPoint.transform.position, e.transform.position);
    }

    public void Enter(unit e)
    {
        Units.Add(e);
        e.enabled = false;
        e.getAgi.enabled = false;
        e.transform.parent = transform;
        e.transform.position = ShootingPoint.transform.position -Vector3.left * MinimumGuard + Units.Count * Vector3.left;

    }
    public void DropUnit( )
    {
        if (Units.Count > 0)
        {
            var d = Units[Units.Count - 1];
            Units.Remove(d);
            d.getAgi.enabled = true;
            d.enabled = true;
            d.transform.parent = null;
            d.transform.position = transform.position + Vector3.forward;
        }
         

    }
    public void Protect()
    {
        aitimer += Time.fixedDeltaTime;
        if (aitimer >RateOfFire)
        {
            var s = Physics.OverlapSphereNonAlloc(transform.position, Range, _col, GameManager.instance.Interatable, QueryTriggerInteraction.Collide);
            print(s);
            for (int i = s - 1; i >= 0; i--)
            {

                if (_col[i].gameObject.GetComponentInParent<entity>())
                {
                    var sauce = _col[i].gameObject.GetComponentInParent<entity>();
                    if (!sauce) continue;
                   
                    if (GetOwner == null && sauce.GetOwner == null) continue;
                    
                    if ((sauce.gameObject == this.gameObject))
                        continue;
                    
                    if (GetOwner != null && sauce.GetOwner != null && sauce.GetOwner == GetOwner) continue;                
                    else
                    {
                        if(Units.Count>= MinimumGuard)
                        {
                            Attack(sauce);
                             
                            aitimer = 0;

                            return;
                        }
                    
                      

                        //Returning right now will improve performance

                    }


                }

            }

            aitimer = 0;
        }
    }

}
