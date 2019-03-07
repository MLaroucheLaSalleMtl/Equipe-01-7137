using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetRessourceInfo : MonoBehaviour
{

    node nd;
 
 
    public void SetNode(node n)
    {
        nd = n;
    }
    public Text Main;
    public Animator anim;

    private void OnMouseEnter()
    {
        if (!nd) return;
            anim.gameObject.SetActive(true);
            Main.text = nd.resource.Name + ": " + nd.resource.getAmount;
            anim.SetTrigger("open");
       
    }



    private void OnMouseExit()
    {
        if (!Main || !nd) return;
        anim.gameObject.SetActive(false);
        Main.text = nd.resource.Name + ": " + nd.resource.getAmount;
        anim.SetTrigger("close");

    }

}
