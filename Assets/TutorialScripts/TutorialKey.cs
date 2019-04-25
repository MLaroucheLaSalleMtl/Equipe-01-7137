using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialKey : Tutorial
{
    public List<string> Keys = new List<string>();

    public override void CheckIfItsHappening()
    {
        
        for (int i = 0; i < Keys.Count; i++)
        {
            if (Input.inputString.Contains(Keys[i]))
            {
                Keys.RemoveAt(i);
                if (Keys.Count == 0)
                {
                    TutorialManager.instance2.CompletedTutorial();
                }
            }

            break;

            }


         
    }


}
