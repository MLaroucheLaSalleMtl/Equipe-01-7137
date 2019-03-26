using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class textBox : MonoBehaviour
{
    public UnityEngine.UI.Text[] Texts;
    public bool catStart = true;
    protected virtual void Start()
    {
        gameObject.SetActive(!catStart);
    }
    public Text Header
    {
        get { return Texts[0]; }
    }
}
