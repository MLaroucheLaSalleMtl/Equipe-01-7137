using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Technology 
{
    public static Dictionary<int, Technology> ResearchKnownToMankind = new Dictionary<int, Technology>()
    {
        { numb,new Technology(30,new int[0],"How To count","Knowing how to count is vital for any civilization" ) },
          {numb,new Technology(200,new int[1]{ 0},"Arithmetic","Advanced Math, so you can barter" ) },
           { numb,new Technology(200,new int[0],"How To Read","Knowing how to read is vital for any civilization" ) },
            { numb,new Technology(200,new int[2]{1,2 },"Rules","A set of rules is crucial for any civilization" ) },
        { numb,new Technology(200,new int[]{},"Stone work","There is suddently more use for our sharp rock" ) }, 
               { numb,new Technology(200,new int[]{4},"Lancer","An defensive Units , slower than most but make exellent guard" ) },
             { numb,new Technology(200,new int[]{2,4},"Sun Dials","The knowledge of Time is in your hands!" ) },

    }

    ;
    public string Name, Description;
    public delegate void OnResearchHandler(Technology t);
    public event OnResearchHandler OnFinish;
    public int ID = 0;
    public int[] Dependencies;
    public float PtsNeed = 100;
    float rspts = 0;
    public float GetCurrentPtsInvest
    {
        get { return rspts; }
    }

    public Technology(int z, float R, int[] c)
    {
        ID = z;
        PtsNeed = R;
        Dependencies = c;
        rspts = 0;
    }
    static int numb = 0;
    /// <summary>
    /// You know what it's tbh, served to create new piece of technology
    /// </summary>
    /// <param name="RPneed">The number of Research Points need to complete this tech</param>
    /// <param name="c">Depencies, the ID of the Tech that's prerequisite</param>
    /// <param name="Nam">Name</param>
    /// <param name="Des">Desc, plz include</param>
    private Technology( float RPneed, int[] c, string Nam, string Des)
    {
        ID =  numb++;
        PtsNeed = RPneed;
        Dependencies = c;
        rspts = 0;
        Name = Nam;
        Description = Des;
    }
    bool IsFullyResearch
    {
        get { return rspts >= PtsNeed; }
    }

    public void Research(float x)
    {
        rspts += x;
        if (IsFullyResearch) OnFinish?.Invoke(this);
    }
}
