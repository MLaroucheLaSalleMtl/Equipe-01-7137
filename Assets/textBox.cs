using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class textBox : MonoBehaviour
{
    public UnityEngine.UI.Text[] Texts;
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public Text Header
    {
        get { return Texts[0]; }
    }
}
