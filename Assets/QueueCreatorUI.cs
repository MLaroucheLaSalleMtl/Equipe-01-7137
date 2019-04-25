using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueCreatorUI : MonoBehaviour
{

    public GameObject Template;
 

    float timer = 0;

    public void ForceDeQueue(unit z)
    {
        if (lol.Count == 0) return;
       var y =  lol.Dequeue();
        Destroy(y.obj.gameObject);
        timer = 0;
    }
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
 
        if(lol.Count > 0)
        {
            if(lol.Peek().img)
            lol.Peek().img.fillAmount = 1 - ( timer/lol.Peek().OGTime);
        }
        else
        {
            timer = 0;
        }
    }
    public void CreateNewIcon(Garison.DeployementOrder d)
    {
        var wow = new QueueUI();
        wow.obj = Instantiate(Template, transform).GetComponent<textBox>();
        wow.obj.gameObject.SetActive(true);
        wow.img = wow.obj.transform.GetChild(0).GetComponent<Image>();
        wow.obj.Header.text = d.Unit.unit.name[0].ToString().ToUpper();
  
        wow.timer = d.GetTimeToDeploy;
        wow.img.fillAmount = 1;
        wow.obj.gameObject.SetActive(true);
        wow.OGTime = d.GetTimeToDeploy;
        lol.Enqueue(wow);
    }
    public Queue<QueueUI> lol = new Queue<QueueUI>();
    public struct QueueUI
    {
        public textBox obj;
        public Image img;
        public float timer;
        public float OGTime;

    }
}
