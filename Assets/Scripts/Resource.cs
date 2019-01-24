using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{

    // Mostly useless, but create it for now - for future functionality
    
    public string Name = "Wood";
    public float Value = 5;
    public int StorageSize =1;
    [Header("Visual")]
    public Image icon;
    public GameObject model, bit;

    float amount = 1;
    public float getAmount
    {
        get { return amount; }
    }
    public Resource()
    {

    }
    public Resource(Resource r,float am)
    {
        name = r.name;
        Value = r.Value;
        icon = r.icon;
        model = r.model;
        amount = am;
    }
 
    public void setRessource(Resource r, float am)
    {
        name = r.name;
        Value = r.Value;
        icon = r.icon;
        model = r.model;
        amount = am;
    }
    public virtual void Merge(Resource r)
    {
        if(r.Name == this.Name)
        {
            amount += r.amount;
            Destroy(r);

        }
    }
    public virtual Resource Exploit(Owner o = null)
    {
        amount--;
       
        if (amount <= 0) { Delete(); return null; }
        else {
            var y = new Resource(this, 1);
            return y;
        } 
    }
    public virtual void Drop(Vector3 pos)
    {
         if(amount > 1)
        {
            Instantiate(bit, pos, Quaternion.identity);
            amount--;
        }
    }
    void Delete()
    {

    }
}
