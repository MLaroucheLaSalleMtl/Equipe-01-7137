using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject toSpawn;
    public float InitialeWait = 10;
    public int MaximumNumberOfSpawn = 10;
    public float interval = 1;
    List<entity> spawns = new List<entity>();
    private void Start()
    {
        StartCoroutine(_pop());
    }
    IEnumerator _pop()
    {

        yield return new WaitForSeconds(InitialeWait);
        while (gameObject.activeSelf)
        {
          
            yield return new WaitForSeconds(interval);
            if (spawns.Count < MaximumNumberOfSpawn)
                Spawn();
        }
        yield break;
   
    }
    public void Spawn()
    {
        var e = Random.insideUnitSphere;
        e.y = 0;
        e *= 3;
        var p = Instantiate(toSpawn, transform.position + transform.forward , Quaternion.identity).GetComponent<entity>();
        if(p is unit)
            (p as unit).MoveTo(transform.position + e);
        spawns.Add(p);
    }
}
