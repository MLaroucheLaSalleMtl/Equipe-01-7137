using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class entity : MonoBehaviour
{

    [SerializeField]
     Owner owner;

    
    public virtual void TransferOwner(Owner n)
    {
        if (owner!= null) owner.onLostEntites(this);
        owner =  n;
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
    public float Hp = 1;
    public List<Goods> Inventory = new List<Goods>();
    public GameObject info;
    protected Text infotext;
    private void Awake()
    {
      if(info) infotext= info.GetComponentInChildren<Text>();
    }
    protected virtual void OnMouseEnter()
    {
       
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if(info)info.gameObject.SetActive(true);
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
    public virtual void Death()
    {
        foreach (var item in Inventory)
        {
            item.Drop(transform.position);
        }
        Destroy(this.gameObject);

    }
}
