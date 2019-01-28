using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class unit : entity
{
    [SerializeField]
     NavMeshAgent agi;
    //So it's 10m per turn or 1 nodes
    public string Name = "Security";
    public float Speed = 10;
    public float attack = 5;
    public float defense = 5;

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
        infotext.text = Name + " HP: " + Hp +"  " + " ATK: " + attack + " \nSPD:" + Speed + "\nDEF" + defense;
    }
    Vector3 previousTarget;

    public void Attack(entity e)
    {
        //Need to go there first and ofremost
        if (currentAttackRoutine == null)
        {
            currentAttackRoutine = AttackSequence(e);
            StartCoroutine(currentAttackRoutine);
            return;
        }

        print(this.ToString() + " attacks " + e.name + " for " + attack + " damages" );
        e.TakeDamage(attack);
    }

    IEnumerator currentAttackRoutine;
    IEnumerator AttackSequence(entity x) {

        agi.isStopped = true;
        yield return new WaitForFixedUpdate();
    
        agi.SetDestination(x.transform.position);
        agi.isStopped = false;
        yield return new WaitForFixedUpdate();
        while (agi.remainingDistance > (.2f + agi.radius))
        {
            yield return new WaitForSeconds(.5f);
            yield return null;
        }

        while (x && x.Hp > 0)
        {
            yield return new WaitForSeconds(1);
            Attack(x);
        }
        yield  break;
    }


    float timer = 0;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer > 1)
        {

            timer = 0;
        }
    }

    public void Chill ()
    {
        agi.isStopped = true;
        if (currentAttackRoutine != null) StopCoroutine(currentAttackRoutine);
    }
    public void MoveTo(Vector3 v)
    {
        previousTarget = v;
        agi.SetDestination(v);
        agi.isStopped = false;
        if (currentAttackRoutine != null) StopCoroutine(currentAttackRoutine);
    }
    public override string ToString()
    {
        return Name + " lv." + (Speed + attack + defense).ToString();
    }
}
