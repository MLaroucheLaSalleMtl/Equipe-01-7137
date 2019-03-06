using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardCam : MonoBehaviour
{
    [HideInInspector]
    public Camera cam;
    public bool MinView = false;
    public float View = 10;
    [SerializeField]
    GameObject todis;
    float dist;
    private void Awake()
    {
        if (GetComponentInParent<entity>()) ent = GetComponentInParent<entity>();
        cam = Camera.main;
    }
    entity ent;

    private void FixedUpdate()
    {
        if (!MinView) return;
        dist = GameManager.instance.GetZoomLevel;
        todis.SetActive(dist > View);
    }
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
