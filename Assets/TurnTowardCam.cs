using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardCam : MonoBehaviour
{
    [HideInInspector]
    public Camera cam;
    private void Awake()
    {
        if (GetComponentInParent<entity>()) ent = GetComponentInParent<entity>();
        cam = Camera.main;
    }
    entity ent;
    private void Update()
    {

        if (!cam) cam = Camera.main;
        else
        transform.LookAt(cam.transform);
    }

    public void  Menu(int f)
    {
      //  ent.info.gameObject.SetActive(f >0);
    }
   

}
