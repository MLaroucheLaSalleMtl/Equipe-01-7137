using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableTbox : textBox 
{
    // Start is called before the first frame update
    protected override void Start()
    {
        
    }
    public void OnDrag(GameObject g)
    {
        var e = Input.mousePosition;
        var d = 120;
        var y = Screen.height;var x = Screen.width;
        e.x = Mathf.Clamp(e.x, d, x - d);
        e.y = Mathf.Clamp(e.y, 2, y - d/2);
        g.transform.position = e;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
