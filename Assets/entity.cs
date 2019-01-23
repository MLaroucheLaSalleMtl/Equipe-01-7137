using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entity : MonoBehaviour
{

    [SerializeField]
     Owner owner;

    node currentNode;
    public node GetCurrentNode
    {
        get { return currentNode; }
    }
    public Owner GetOwner { get { return owner; } }
    public float Hp = 1;
    public List<Resource> Inventory = new List<Resource>();

    public void Take(Resource r)
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
