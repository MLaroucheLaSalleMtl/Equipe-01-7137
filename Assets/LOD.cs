using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOD : MonoBehaviour
{
    public float Range = 50;
    public GameObject Tree;
    Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }
    private void LateUpdate()
    {
       Tree.SetActive(Vector3.Distance(cam.transform.position, transform.position) < Range);
    }
}
