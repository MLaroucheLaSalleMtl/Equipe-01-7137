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
    Animator anim;
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

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if(infotext)infotext.text = Name + " HP: " + Hp +"  " + " ATK: " +getAttack + " \nSPD:" + GetMovingSpeed+ "\nDEF" + defense;
    }

    protected void OnMouseExit()
    {
        base.OnMouseEnter();
        if (infotext) infotext.gameObject.SetActive(false);
    }
    Vector3 previousTarget;



    protected Collider[] _col = new Collider[20];
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
        if(e == null)
        {
            print("Nothing to attack, error!");
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
    IEnumerator GoThere(Vector3 pos)
    {
        agi.isStopped = true;
        yield return new WaitForFixedUpdate();

        agi.SetDestination(pos);
        agi.isStopped = false;
        yield return new WaitForFixedUpdate();
        while (agi.remainingDistance > (.2f + agi.radius + agi.stoppingDistance + .1f))
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

        while (x && x.Hp > 0  )
        {
          
            if(agi.remainingDistance > (.2f + agi.radius + agi.stoppingDistance + .1f))
            {
                /*  agi.SetDestination(x.transform.position);
                  agi.isStopped = false;*/
                yield return StartCoroutine(GoThere(x.transform.position));
            }
            else
            {
                agi.isStopped = true;
                yield return new WaitForSeconds(1);
               _attack(x);
            }
          
        }
        currentAttackRoutine = null;
        yield  break;
    }

    entity last_agressor;
    public override void TakeDamage(float t,entity e)
    {
        base.TakeDamage(t);
    }
    void _attack(entity e)
    {
        if (e)
        {
            if (anim) anim.SetTrigger("ATK");
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

            infotext.text = Name + " HP: " + Hp + "  " + " ATK: " + getAttack + " \nSPD:" + GetMovingSpeed + "\nDEF" + defense;
            infotext.gameObject.SetActive(false);
        }
        if (GetComponent<Animator>()) anim = GetComponent<Animator>();


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
