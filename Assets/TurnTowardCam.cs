using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardCam : MonoBehaviour
{
    private Camera cam;
    private void Awake()
    {
        if (GetComponentInParent<entity>()) ent = GetComponentInParent<entity>();
        cam = Camera.main;
    }
    entity ent;
    private void Update()
    {
        transform.LookAt(cam.transform);
    }

    public void  Menu(int f)
    {
        ent.info.gameObject.SetActive(f >0);
    }
   

}
