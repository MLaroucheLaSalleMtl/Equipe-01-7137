using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopMessage : MonoBehaviour
{
    [SerializeField]
    Animator anim;
    [SerializeField]
    Text txt;
    Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }


    public void SetText(string t)
    {
        gameObject.SetActive(true);
        txt.text = t;
      if(cam != null)  AudioSource.PlayClipAtPoint(GameManager.instance.error, cam.gameObject.transform.position);
        StartCoroutine(_popup());
    }
    public void PopTrueBuilding(int o)
    {
        var e = GameManager.instance.Buildings[o].GetComponent<building>();
       
    }
    public void PopClose()
    {
        anim.SetBool("pop up s", false);
        anim.SetBool("pop up", false);
       
    }
    IEnumerator _popup()
    {
        anim.SetBool("pop up", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("pop up", false);

        yield break;
    }
}
