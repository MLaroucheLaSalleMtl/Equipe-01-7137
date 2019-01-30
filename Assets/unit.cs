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
    protected virtual float GetMovingSpeed
    {
        get { return Speed; }
    }
    public virtual float getAttack
    {
        get { return attack; }
    }
    [SerializeField]
      float attack = 5;
    public float defense = 5;

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if(infotext)infotext.text = Name + " HP: " + Hp +"  " + " ATK: " +getAttack + " \nSPD:" + GetMovingSpeed+ "\nDEF" + defense;
    }
    Vector3 previousTarget;

    Army _army;
    public void Attack(entity e)
    {
        //Need to go there first and ofremost
        if (currentAttackRoutine == null)
        {
            currentAttackRoutine = AttackSequence(e);
            StartCoroutine(currentAttackRoutine);
            return;
        }

        print(this.ToString() + " attacks " + e.name + " for " + getAttack+ " damages" );
        e.TakeDamage(getAttack);
    }

    public void Return()
    {

    }
    IEnumerator _return()
    {
        yield return StartCoroutine( GoThere(_army.transform.position));
        _army.RegainRank(this);
        yield break;
    }
    IEnumerator GoThere(Vector3 pos)
    {
        agi.isStopped = true;
        yield return new WaitForFixedUpdate();

        agi.SetDestination(pos);
        agi.isStopped = false;
        yield return new WaitForFixedUpdate();
        while (agi.remainingDistance > (.2f + agi.radius))
        {
            yield return new WaitForSeconds(.15f);
            yield return null;
        }

        yield break;
    }
    IEnumerator currentMergingRoutine;
    IEnumerator MergingSequence(unit x)
    {
        yield return StartCoroutine(GoThere(x.transform.position));
        Merge(x);
        yield break;
    }
    public virtual Army Merge(unit z)
    {
        if (currentMergingRoutine == null)
        {
            currentMergingRoutine = MergingSequence(z);
            StartCoroutine(currentMergingRoutine);
            return null;
        }

        if (z is Army)
        {
            (z as Army).AddToArmy(this);
            _army = z as Army;
            return (z as Army);
        }
        else
        {
            var x = Instantiate(GameManager.ArmyPrefab, z.transform.position, z.transform.rotation).GetComponent<Army>();
            x.AddToArmy(this);
            _army = x;
            z._army = x;
            x.AddToArmy(z);
            x.TransferOwner(GetOwner);
            return x;
        }
    }
    IEnumerator currentAttackRoutine;
    IEnumerator AttackSequence(entity x) {

        yield return StartCoroutine(GoThere(x.transform.position));

        while (x && x.Hp > 0 && agi.remainingDistance < (.2f + agi.radius))
        {
          
            if(agi.remainingDistance > (.2f + agi.radius))
            {
                /*  agi.SetDestination(x.transform.position);
                  agi.isStopped = false;*/
                yield return StartCoroutine(GoThere(x.transform.position));
            }
            else
            {
                yield return new WaitForSeconds(1);
                Attack(x);
            }
          
        }
        currentAttackRoutine = null;
        yield  break;
    }
    private void Awake()
    {
        if (infotext) {

            infotext.text = Name + " HP: " + Hp + "  " + " ATK: " + getAttack + " \nSPD:" + GetMovingSpeed + "\nDEF" + defense;
            infotext.gameObject.SetActive(false);
        }

    }

    float timer = 0;
    private void FixedUpdate()
    {

        agi.speed = GetMovingSpeed;
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
