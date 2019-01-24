using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected node currentNode;
    protected float maximumHp = 1;
    public node GetCurrentNode
    {
        get { return currentNode; }
    }
    public Owner GetOwner { get { return owner; } }
    public float Hp = 1;
    public List<Goods> Inventory = new List<Goods>();

    
    public entity()
    {
        maximumHp = Hp;
    }
    public void Take(Goods r)
    {

      
        foreach (var item in Inventory)
        {
            if(item.Name == r.Name)
            {
                item.Merge(r);
                return;
            }
        }
        var s = r.Exploit(owner);
        Inventory.Add(s);
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
