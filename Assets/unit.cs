﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class unit : entity
{

    [Header("DEBUG")]
    public int DEBUG_OWNER = -1;
    [SerializeField]
     NavMeshAgent agi;
    [SerializeField]
    GameObject indicator;

    public override void Death()
    {
        StopAllCoroutines();
        base.Death();
    }
    public NavMeshAgent getAgi
    {
        get { return agi; }
    }

    //So it's 10m per turn or 1 nodes
    public string Name = "Security";
    public float Speed = 10;
 
   protected Animator anim;
    [SerializeField]
    MeshRenderer[] rendies;
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
    public float DetectionRange = 1;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
 
    
    public void ChangeColor(Color z)
    {
        if(rendies.Length > 0)
        {
            foreach (var item in rendies)
                item.material.color = z;
        }
    }

    IEnumerator _delay;
    IEnumerator DelayClose()
    {
        yield return new WaitForSeconds(1f);
        if (uianim.GetCurrentAnimatorStateInfo(0).IsTag("open")) uianim.SetTrigger("close");
        yield break;
    }
    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (_delay != null) StopCoroutine(_delay);
        _delay = DelayClose();
        StartCoroutine(_delay);
        if (infotext) infotext.text = Name + " Type " + Type.ToString() + "\n" + "HP:" + Hp + " /" + maximumHp + " lvl:" + (getAttack + defense + Speed).ToString("0.0"); 
    }
   
    protected override void OnMouseExit()
    {
        base.OnMouseExit();
     
       // if (infotext) infotext.gameObject.SetActive(false);
    }
    Vector3 previousTarget;

    


    //Maximum of 500 entity, after that, it cannot detect more than that. HardCap for performance;
    protected Collider[] _col = new Collider[500];
   protected float aitimer = 0;
    public virtual void AI()
    {
        aitimer += Time.fixedDeltaTime;
       
        if (last_agressor)
            Attack(last_agressor);
        else
        if (aitimer > .3f)
        {
            var s = Physics.OverlapSphereNonAlloc(transform.position, DetectionRange, _col, GameManager.instance.Interatable, QueryTriggerInteraction.Collide);
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

                        Attack(sauce);
                        //Returning right now will improve performance
                        return;
                    }


                }

            }

            aitimer = 0;
        }
        //Default Defense Mode
  
    }
 
    Army _army;
    public void Attack(entity e)
    {
        if(!e)
        {
            print("Nothing to attack, error!");
            return;
        }
        //Need to go there first and ofremost
        if (currentAttackRoutine == null)
        {
            print("Initiating Attack on " + e.name);
            currentAttackRoutine = AttackSequence(e);
            StartCoroutine(currentAttackRoutine);
            return;
        }

   
    }

    public void Return()
    {
        StartCoroutine(GoThere(previousTarget));
    }
    IEnumerator _return()
    {
        yield return StartCoroutine( GoThere(_army.transform.position));
        _army.RegainRank(this);
        yield break;
    }
    float minimumdistance = 0;
    IEnumerator GoThere(Vector3 pos)
    {
        agi.isStopped = true;
        yield return new WaitForFixedUpdate();

        agi.SetDestination(pos);
        agi.isStopped = false;
        minimumdistance = .35f + agi.radius + agi.stoppingDistance;
        yield return new WaitForFixedUpdate();
        while (agi.remainingDistance > (minimumdistance + .1f))
        {
            yield return new WaitForSeconds(.05f);
            agi.SetDestination(pos);
            if (Vector3.Distance(transform.position, pos) < minimumdistance) break;
            //print(name + " distance to target : " + minimumdistance + "m. ");
            yield return null;
        }

        yield break;
    }

    IEnumerator GoThere(entity e)
    {

        if (!agi) yield break;
        agi.SetDestination(e.transform.position);
        agi.isStopped = false;
        minimumdistance =  agi.radius + agi.stoppingDistance;
       
        while ( agi.isOnNavMesh&& agi.remainingDistance > (minimumdistance) && e && e.gameObject )
        {
            yield return new WaitForSeconds(.01f);
            if(e && e.gameObject)agi.SetDestination(e.transform.position);
            else { break; }
            print(name + " distance to target : " + minimumdistance + "m. ");
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
    IEnumerator AttackSequence(entity x)  {

        yield return StartCoroutine(GoThere(x.transform.position));

        while (x && x.Hp > 0  )
        {    
            if(agi.remainingDistance> (minimumdistance))
            { 
                yield return StartCoroutine(GoThere(x ));
                yield return null;
            }
            else
            {
               
                
                agi.isStopped = true;
                yield return new WaitForSeconds(1);
                _attack(x);
            }
          
        }
        agi.isStopped = true;
        currentAttackRoutine = null;

        MoveTo(previousTarget);
        yield  break;
    }

    protected override void OnKill(entity z)
    {
        base.OnKill(z);
        Chill();
    }
    public override void TakeDamage(float t, DamageType p = DamageType.Null)
    {
        base.TakeDamage(t, p);
        anim.SetTrigger("damaged");
    }
    void _attack(entity e)
    {
        if (e)
        {
            if (anim) anim.SetTrigger(Type.ToString());
            print(this.ToString() + " attacks " + e.name + " for " + getAttack + " damages");
            transform.LookAt(e.gameObject.transform.position, Vector3.up);
            e.TakeDamage(getAttack,this);

        }
        else
        {
            MoveTo(previousTarget);
        }

    }
    private void Awake()
    {
        if (infotext) {


            info.gameObject.SetActive(false);
        }
        if (GetComponent<Animator>()) anim = GetComponent<Animator>();
        if(DEBUG_OWNER > -1)
        {
            TransferOwner(GameManager.owners[DEBUG_OWNER]);
           
        }
        OnDeselected();
        previousTarget = transform.position;
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
        agi.SetDestination(transform.position);
        if (currentAttackRoutine != null) StopCoroutine(currentAttackRoutine);
    }

    public void MoveTo(Vector3 v)
    {
        previousTarget = v;
        if (!agi) return;
        agi.SetDestination(v);
        agi.isStopped = false;
        if (currentAttackRoutine != null) StopCoroutine(currentAttackRoutine);
    }
    public override string ToString()
    {
        return Name + " lv." + (Speed + attack + defense).ToString();
    }

    public override void OnSelected()
    {
        if (indicator) indicator.SetActive(true);
    }
    public override void OnDeselected()
    {
        if (indicator) indicator.SetActive(false);
    }
}
