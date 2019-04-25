using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOwner : MonoBehaviour
{
    public int OwnerToSet = 0;
    private void Awake()
    {
        var e = GetComponent<entity>();
        if (e)
        {
          e.TransferOwner(GameManager.owners[OwnerToSet]);

            if (e is building)
            {

                var f = (e as building);
                if(f.Bar)f.Bar.transform.parent.gameObject.SetActive(false);
            
            }
        }
         


    }
}
