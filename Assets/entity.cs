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




    [SerializeField]
    Owner owner;


    public virtual void TransferOwner(Owner n)
    {
        if (owner != null) owner.onLostEntites(this);
        owner = n;
        n.onNewEntites(this);
    }
    public float GoldCost = 5;
    protected node currentNode;
    protected float maximumHp = 1;
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
    private void Awake()
    {
        Hp = HealthToSet;
      if(info) infotext= info.GetComponentInChildren<Text>();
    }
    protected virtual void OnMouseEnter()
    {
       
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(info)info.gameObject.SetActive(true);
    }

    public virtual void OnSelected()
    {

    }
    public virtual void OnDeselected()
    {

    }
    private void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (info) info.gameObject.SetActive(false);
    }
    public entity()
    {
        maximumHp = Hp;
    }
  
    public void Heal()
    {
        Hp = maximumHp;
        Hp = Mathf.Clamp (Hp, 0, maximumHp);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<node>())
        {
            currentNode = other.gameObject.GetComponent<node>();
        }
    }
 
    public virtual void TakeDamage(float t)
    {
        Hp -= t;
        if (Hp < 0) Death();
    }
    public virtual void TakeDamage(float t,entity e)
    {
        Hp -= t;
        if (Hp < 0) Death();
    }
    public virtual void Death()
    {

        if(GetOwner != null)
        GetOwner.onLostEntites(this);
        foreach (var item in Inventory)
            item.Drop(transform.position);


        Destroy(this.gameObject);

    }
}
