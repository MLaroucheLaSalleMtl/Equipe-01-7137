using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class unit : entity
{   [Header("----ID---")]
    public int ID = 0;
    [Header("DEBUG")]
    public int DEBUG_OWNER = -1;
    [SerializeField]
     protected NavMeshAgent agi;
    [SerializeField]
    GameObject indicator;
    [SerializeField]
    Renderer lifeindicator;
    public bool Barbarian = false;

    public float TimeToDeploy = 1;
    private void Start()
    {
        if(onCreated)
                AudioSource.PlayClipAtPoint(onCreated, transform.position);
        updateLifeIndiactor();

        minimumdistance = AdditionalDist + .35f + agi.radius + agi.stoppingDistance;
    }
    public bool HasIssuesCommand
    {
        get { return Ordered; }
    }
    protected bool Ordered = false;
    public override void Death(bool f = false)
    {
        if(Oof)
        AudioSource.PlayClipAtPoint(Oof, transform.position);
        StopAllCoroutines();
        base.Death(false);
        Destroy(agi);
        var r = GetComponent<Rigidbody>();
        lifeindicator.gameObject.SetActive(false);
        r.isKinematic = false;
        r.useGravity = true;
        r.AddExplosionForce(400,(Random.insideUnitSphere + transform.position),20);
        Destroy(gameObject,3);
    }
    void updateLifeIndiactor()
    {
        if (!lifeindicator) return;

        var e = new Color();
        if (Hp > maximumHp * .75f) e = Color.green;
        else if (Hp > maximumHp * .50) e = Color.yellow;
        else if (Hp > maximumHp * .35f) e = Color.yellow + Color.red;
        else e = Color.red;
        lifeindicator.material.color = e;
    }
    public NavMeshAgent getAgi
    {
        get { return agi; }
    }

    //So it's 10m per turn or 1 nodes
    public string Name = "Security";
    public float Speed = 10;

    [Header("Flair")]
    public AudioClip Oof, Hurt;
    public AudioClip onCreated;
   protected Animator anim;
    [SerializeField]
    protected MeshRenderer[] rendies;
    public virtual float GetMovingSpeed
    {
        get { return Speed * MainUI.SpeedMult; }
    } 
    public virtual float getAttack
    {
        get {

            float bonus = 1;
            if (GetOwner.HasResearch(7))
                bonus += .25f;
            return attack * bonus; }
    }
    [SerializeField]
      float attack = 5;
    [SerializeField]
    float defense = 5;

    public virtual float getDefense
    {
        get{ return defense; }
    }
    public virtual float GetDetectionRange
    {
        get { return DetectionRange; }
    }
    public virtual float GetAttackSpeed
    {
        get {
            if (Barbarian && Hp > 0)
            {
                var t = Mathf.Clamp((GetMaxmimumHP / Hp) * AtkSpeed, .05f, AtkSpeed);
                return t;
            }

            return AtkSpeed; }
    }
    public float AtkSpeed = 1;
    public float DetectionRange = 1;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GetDetectionRange);
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
        if (infotext) infotext.text = Name + " Type " + Type.ToString() + "\n" + "HP:" + Hp + " /" + maximumHp + " lvl:" + (getAttack + getDefense + GetMovingSpeed).ToString("0.0"); 
    }
   
    protected override void OnMouseExit()
    {
        base.OnMouseExit();
     
       // if (infotext) infotext.gameObject.SetActive(false);
    }
    Vector3 previousTarget;

    


    //Maximum of 500 entity, after that, it cannot detect more than that. HardCap for performance;
    protected Collider[] _col = new Collider[500];
    public Queue<entity> TargetToHunt = new Queue<entity>();

    protected float aitimer = 0;
    public static entity[] GetAlliesAtPosition(Vector3 pos, float size, Owner x)
    {
        var ee = new List<entity>();
        var s = Physics.OverlapSphere(pos, size, GameManager.instance.Unit, QueryTriggerInteraction.Collide);

        foreach (var item in s)
        {
            if (item.GetComponent<entity>())
            {
                var y = item.GetComponent<entity>();
                if (y.GetOwner == x) ee.Add(y);
            }
        }

        return ee.ToArray();
        }

    public void Attack(entity z)
    {
        if (!this) return;
        agi.isStopped = false;
        agi.SetDestination(z.transform.position);
        _attack(z); 
    }
public virtual void AI()
    {
        aitimer += Time.fixedDeltaTime;
     
        if (aitimer > .3f)
        {
            if (last_agressor)
            { target = last_agressor; last_agressor = null; OrderedAttack(target); aitimer = 0; return; }
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
                    if (sauce.GetOwner == GetOwner) continue;
                    if ((sauce.gameObject == this.gameObject))
                        continue;
                    if (GetOwner.Relation.ContainsKey(sauce.GetOwner.Name) && GetOwner.Relation[sauce.GetOwner.Name] > -10) continue;

                    if (Vector3.Distance(transform.position, sauce.transform.position) < dist)
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
        //Default Defense Mode
  
    }

    private void OnTriggerStay(Collider other)
    {
        if(target)
        if (other.gameObject == target.gameObject) _attack(target);
    }

    Army _army;
    protected entity target;
    public void OrderedAttack(entity e)
    {
        if(!e)
        {

            return;
        }
        //Need to go there first and ofremost
        if (currentAttackRoutine == null)
        {
          //  lastatk = e;
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
    public float AdditionalDist = 0;
    IEnumerator GoThere(Vector3 pos)
    {
        if(!agi || !agi.isOnNavMesh)
        {
            print("There is no valid nav mesh nor agi!");
            yield break;
        }
        agi.isStopped = true;
        yield return new WaitForFixedUpdate();

        agi.SetDestination(pos);
        agi.isStopped = false;
        minimumdistance = AdditionalDist +  .35f + agi.radius + agi.stoppingDistance;
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
        if (e is building) minimumdistance += (e as building).SpaceNeed.magnitude;
        else minimumdistance += .2f;
        Vector3 ok = e.transform.position;
        while ( agi.isOnNavMesh&& Vector3.Distance(transform.position, ok) > (minimumdistance) && e && e.gameObject )
        {
            yield return new WaitForSeconds(.01f);
            if (e && e.gameObject) { ok = e.transform.position; agi.SetDestination(e.transform.position); } 
            else { break; }
          //  print(name + " distance to target : " + minimumdistance + "m. ");
            yield return null;
        }

        yield break;
    }
    IEnumerator currentMergingRoutine;
    IEnumerator MergingSequence(unit x)
    {
        yield break;
        yield return StartCoroutine(GoThere(x.transform.position));
        Merge(x);
        yield break;
    }
    public virtual Army Merge(unit z)
    {
         return null;
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

        Ordered = true;
        yield return StartCoroutine(GoThere(x.transform.position));

         while (x && x.Hp > 0  )
         {    
             if(Vector3.Distance(transform.position,x.transform.position)> (minimumdistance  ))
             { 
                 yield return StartCoroutine(GoThere(x ));
                 yield return null;
             }
             else
             {
                 agi.isStopped = true;
                 _attack(x);
             }
            yield return null;
         }
         agi.isStopped = true;
         currentAttackRoutine = null;

         MoveTo(previousTarget);
        Ordered = false;
        currentAttackRoutine = null;
        yield  break;
    }

    protected override void OnKill(entity z)
    {
        base.OnKill(z);
        if (z == target) target = null;
    Chill();
    }

    public override void TakeDamage(float t, entity e, DamageType p = DamageType.Null)
    {
        base.TakeDamage( t - getDefense, e, p);
        updateLifeIndiactor();
       /* if (!Ordered && e)
            Attack(e);*/
   
    }
    public override void TakeDamage(float t, DamageType p = DamageType.Null)
    {
        base.TakeDamage(t - getDefense, p);
        updateLifeIndiactor();
        anim.SetTrigger("damaged");
        if (Hurt)
            AudioSource.PlayClipAtPoint(Hurt, transform.position);
    
    }
    protected virtual void _attack(entity e)
    {


        if (attackTimer < GetAttackSpeed) return;
        if (Vector3.Distance(transform.position, e.transform.position) > minimumdistance) return;

        if (e)
        {
            if (anim) anim.SetTrigger(Type.ToString());
            print(this.ToString() + " attacks " + e.name + " for " + getAttack  + " damages");
            transform.LookAt(e.gameObject.transform.position, Vector3.up);
            e.TakeDamage(getAttack,this);

        }
        else  MoveTo(previousTarget);
           
    
        
       
        attackTimer = 0;
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

    float attackTimer = 0;

    public virtual void FixedUpdate()
    {
        if(Hp >= 0)
            agi.speed = GetMovingSpeed;

      
        attackTimer += Time.fixedDeltaTime;
    }



    public void Chill ()
    {
        Ordered = false;
        last_agressor = null;
        target = null;
        if (!this) return;
        if (agi)
        {
            agi.isStopped = true;
            agi.SetDestination(transform.position);
        }
 
        TargetToHunt = new Queue<entity>();
 
        if (currentAttackRoutine != null) StopCoroutine(currentAttackRoutine);
    }

    public void MoveTo(Vector3 v)
    {
        agi.isStopped = true;
        Ordered = true;
        previousTarget = v;
        if (!agi) return;
        agi.SetDestination(v);
        agi.isStopped = false;
            if (currentAttackRoutine != null) StopCoroutine(currentAttackRoutine);
        StartCoroutine(reachedPosition(v));
      

    }
    IEnumerator reachedPosition(Vector3 v)
    {
        yield return new WaitForSeconds(.1f);
        while (Ordered)
        {
            if (Vector3.Distance(v, transform.position) < 2) Ordered = false;
            yield return new WaitForSeconds(.1f);
        }
        yield break;
    }
    public override string ToString()
    {
        return Name + " lv." + (GetMovingSpeed + attack + defense).ToString();
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
