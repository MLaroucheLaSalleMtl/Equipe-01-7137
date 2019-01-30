using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Goods  
{

    // Mostly useless, but create it for now - for future functionality
    
    public string Name = "NILL";
    public float Value = 5;
    public int StorageSize =1;
    [Header("Visual")]
    public Image icon;
    public GameObject model, bit;

 
    [SerializeField]
    float amount = 0;
    public float getAmount
    {
        get { return amount; }
    }
    public Goods()
    {

    }
    public Goods(Goods r,float am)
    {
        Name = r.Name;
        Value = r.Value;
        icon = r.icon;
        model = r.model;
        bit = r.bit;
        amount = am;
    }
 
    public void setRessource(Goods r, float am)
    {
        Name = r.Name;
        Value = r.Value;
        icon = r.icon;
        model = r.model;
        bit = r.bit;
        amount = am;
    }
    public virtual void Merge(Goods r)
    {
        if(r.Name == this.Name)
        {
            amount += r.amount;
            //Destroy(r);

        }
    }
    public virtual Goods Exploit(int am = 1)
    {
        amount-=am;
   
        if (amount <= 0) {  Delete(); return null; }
        else {
            var y = new Goods(this, am);
            return y;
        } 
    }
    public virtual void Drop(Vector3 pos)
    {
         if(amount > 1)
        {
            //Instantiate(bit, pos, Quaternion.identity);
            amount--;
        }
    }
    void Delete()
    {

    }
}
