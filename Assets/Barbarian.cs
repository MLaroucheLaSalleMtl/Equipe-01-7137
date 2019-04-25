using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian : unit
{


    private void Start()
    {
        ChangeColor(Color.red);
        StartCoroutine(WaitForCore());
        if (info) info.SetActive(false);
    }

    private void FixedUpdate()
    {
        AI();
    }
    protected override void OnMouseEnter()
    {
      
    }
    
    IEnumerator WaitForCore()
    {
        while (GameManager.owners[0].Cores.Count == 0) yield return null;
        MoveTo(GameManager.owners[0].Cores[0].transform.position);
        yield break;

    }
}
