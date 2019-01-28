using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardCam : MonoBehaviour
{
    private Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        transform.LookAt(cam.transform);
    }
}
