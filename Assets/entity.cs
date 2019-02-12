using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class entity : MonoBehaviour
{
    [Header("Unity Side")]
    [Tooltip("The only reason this exists is because the Hp is virtual, so set that to reflect the change in code")]

    [SerializeField]
    float HealthToSet = 1;
   

    public enum DamageType
    {
        A = 0, B = 1, C = 2, Null = 3
    }

    [SerializeField]
    Owner owner;

    public void SetOwner(Owner s)
    {
        owner = s;
        owner = s;
    }
    public DamageType Type = DamageType.Null;
    public virtual void TransferOwner(Owner n)
    {
        if (owner != null) owner.onLostEntites(this);
        owner = n;
        n.onNewEntites(this);
    }
    public float GoldCost = 5;
    protected node currentNode;
    protected float maximumHp = 1;
    [SerializeField]
    protected Animator uianim;
    public node GetCurrentNode
    {
        get { return currentNode; }
    }
    public Owner GetOwner { get { return owner; } }
    public virtual float Hp { get; set; } = 5;
    public List<Goods> Inventory = new List<Goods>();
    public GameObject info;
    [SerializeField]
    protected Text infotext;
    private void Start()
    {
        Hp = HealthToSet;
        maximumHp = HealthToSet;
        if (info) infotext = info.GetComponentInChildren<Text>();
        if (info) { info.gameObject.SetActive(false);
        }
    }
    protected virtual void OnMouseEnter()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (info)
        {
            if (uianim) uianim.SetTrigger("open");
            else info.gameObject.SetActive(true);
        }
    }

    public virtual void OnSelected()
    {

    }
    public virtual void OnDeselected()
    {

    }
    protected virtual void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (info) {
            if (uianim) uianim.SetTrigger("close");
            else info.gameObject.SetActive(false);
        }
    }
    public entity()
    {
        maximumHp = Hp;
    }

    public void Heal()
    {
        Hp = maximumHp;
        Hp = Mathf.Clamp(Hp, 0, maximumHp);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<node>())
        {
            currentNode = other.gameObject.GetComponent<node>();
        }
    }
    //A > B, B > C, C > A
    public static float GetTypeEfficiency(DamageType primary, DamageType secondary)
    {
        if (primary == DamageType.Null || secondary == DamageType.Null || primary == secondary) return 1;
         
        else if(primary == DamageType.A)
        {
            if (secondary == DamageType.B) return 1.5f;
            else return .5f;
        }
        else if (primary == DamageType.B)
        {
            if (secondary == DamageType.C) return 1.5f;
            else return .5f;
        }
        else
        {
            if (secondary == DamageType.A) return 1.5f;
            else return .5f;
        }
    }
 
    public virtual void TakeDamage(float t,DamageType p = DamageType.Null)
    {
   
        Hp -= t * GetTypeEfficiency(Type,p);
        if (Hp < 0) Death();
    }
    protected entity last_agressor;
    int killcount = 0;
    public int GetKillCount
    {
        get { return killcount; }
    }

    protected virtual void OnKill(entity z)
    {
        killcount++;
    }
    public virtual void TakeDamage(float t,entity e, DamageType p = DamageType.Null)
    {
        last_agressor = e;
        TakeDamage(t,p);
    }
    public virtual void Death()
    {

        if(GetOwner != null)
        GetOwner.onLostEntites(this);
        foreach (var item in Inventory)
            item.Drop(transform.position);

        if (last_agressor) last_agressor.OnKill(this);
        Destroy(this.gameObject);

    }
}
