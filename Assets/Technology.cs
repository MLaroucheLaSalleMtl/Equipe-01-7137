using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Technology 
{
    public static Dictionary<int, Technology> ResearchKnownToMankind = new Dictionary<int, Technology>()
    {
        { numb,new Technology(30,new int[0],"How To count","Knowing how to count is vital for any civilization. +10% Gold Production" ) },//c
          {numb,new Technology(200,new int[1]{ 0},"Arithmetic","Advanced Math, so you can barter. +20% Gold production" ) },//c
           { numb,new Technology(200,new int[0],"How To Read","Knowing how to read is vital for any civilization. Science Production +25%" ) },//c
            { numb,new Technology(300,new int[2]{1,2 },"Laws","A set of rules is crucial for any civilization. Units are produced 15% faster" ) },//c
        { numb,new Technology(30,new int[]{},"Stone work","There is suddenly more use for our sharp rocks." ) }, 
               { numb,new Technology(80,new int[]{4},"Lancer","An defensive Units , slower than most but make an exellent guard" ) }, //c
             { numb,new Technology(80,new int[]{2,4},"Sun Dials","The knowledge of Time is in your hands! Gatherers are 20% faster" ) },//c
  { numb,new Technology(290,new int[]{5},"Grinder","Augments the damage dealt by 25%." ) },//c
    { numb,new Technology(190,new int[]{17},"Barbarian","Unlocks the barbarian. A unit that deals more damage the less health it has." ) }, //c
    { numb,new Technology(110,new int[]{2},"Nature","Why we shouldn't eat those herb 101" ) },//9
     { numb,new Technology(40,new int[]{9},"Biology","Gain insight on how the human body works. +25% HP" ) },//c
        { numb,new Technology(200,new int[]{1,4},"Engineering","How to build efficiently. Unlock Engie House" ) },//c
                { numb,new Technology(70,new int[]{1,2},"Economics","How to make more money. + 50% Gold Production" ) },//c
                { numb,new Technology(600,new int[]{3,9,6},"Religion","Give prayers to the lost. Unlocks the Church" ) },
                { numb,new Technology(200,new int[]{11,10},"Medecine","Unlocks the Doctor. Unit that heals other nearby units" ) },//14
                { numb,new Technology(300,new int[]{9,1},"Physics","Learn about the laws of nature. Unlocks the Archer" ) },
                { numb,new Technology(900,new int[]{12,6},"Longinus","A powerful Unit that can soak up a lot of damage." ) },
                  { numb,new Technology(70,new int[]{5,2},"Leather Work","Leather provide Good protection. + 1 Defense for each unit" ) },//17
            
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
