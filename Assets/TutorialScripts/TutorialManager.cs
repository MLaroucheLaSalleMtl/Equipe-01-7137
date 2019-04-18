using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialManager : MonoBehaviour
{

    public List<Tutorial> BunchOfTutorials = new List<Tutorial>();

    public Text Instructions;
    public GameObject window;

    private static TutorialManager instance1;
    public static TutorialManager instance2

    {
        get
        {
            if(instance1== null)
            {
                instance1 = GameObject.FindObjectOfType<TutorialManager>();
            }
            if(instance1== null)
            {
                Debug.Log("There's no tutorial Manager  ");
            }

            return instance1;

        }
    }
    
    private Tutorial CurrentTutorial;
    
    // Start is called before the first frame update
    void Start()
    {
        SetNextTutorial(0);
        window.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentTutorial)
        {
            CurrentTutorial.CheckIfItsHappening();
            
        }
        
    }

    public void CompletedTutorial()
    {
        SetNextTutorial(CurrentTutorial.Order + 1);

    }
        
    public void SetNextTutorial(int currentOrder)
    {
        CurrentTutorial = GetTutorialByOrder(currentOrder);
        
        if (!CurrentTutorial)
        {
            CompletedAllTutorials();
            return;
        }

        Instructions.text = CurrentTutorial.Explanation;
    }

    public void CompletedAllTutorials()
    {
        Instructions.text = "You Have Finished All the tutorials enjoy the Game ";
    }


    public Tutorial GetTutorialByOrder(int order)
    {
        for (int i = 0; i < BunchOfTutorials.Count; i++)
        {
            if (BunchOfTutorials[i].Order ==order)
            {
                return BunchOfTutorials[i];
            }

            
        }
        return null;
    }

}
