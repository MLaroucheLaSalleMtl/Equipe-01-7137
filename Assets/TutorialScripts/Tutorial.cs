using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int Order;
    [TextArea(3, 10)]
    public string Explanation;
    
    
    void Awake()
    {
        TutorialManager.instance2.BunchOfTutorials.Add(this);
    }

    public virtual void CheckIfItsHappening() { }


}
