using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : building
{
    [Header("Science")]
    public float ResearchPointsPerTick = 1;
    public int MaximumTier = 1;
    public int MaximumScientist = 5;
    public DraggableTbox tbox;

    public override void PerTick()
    {
        base.PerTick();
        GetOwner.AdvancedResearch(ResearchPointsPerTick);
    }
    public override void OpenContextMenu()
    {
        base.OpenContextMenu();
        tbox.Header.text = "Laboratory at" + transform.position;
        tbox.Texts[1].text = description + "\n\nProduce " + ResearchPointsPerTick + " RP/Seconds";
    }

}
