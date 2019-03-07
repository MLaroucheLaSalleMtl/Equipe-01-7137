using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopMessage : MonoBehaviour
{
    [SerializeField]
    Animator anim;
    [SerializeField]
    Text txt;

        
    public void SetText(string t)
    {
        txt.text = t;
        StartCoroutine(_popup());
    }
    public void PopTrueBuilding(int o)
    {
        var e = GameManager.instance.Buildings[o].GetComponent<building>();
        txt.text =  "-" + e.name + "-" + "\n " + e.description ;
        if (e.costs[0].materials.Length > 0) txt.text += "\nCOST:";
        foreach (var item in e.costs[0].materials)
        {
            txt.text += "\n" + item.getAmount + "x " + item.Name;
            if (item.getAmount > 1) txt.text += "s";
        }
        txt.text += "\nGold: " + (e.GoldCost + e.costs[0].Gold).ToString("0"); 
        anim.SetBool("pop up s", true);
    }
    public void PopClose()
    {
        anim.SetBool("pop up s", false);
        anim.SetBool("pop up", false);
       
    }
    IEnumerator _popup()
    {
        anim.SetBool("pop up", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("pop up", false);

        yield break;
    }
}
