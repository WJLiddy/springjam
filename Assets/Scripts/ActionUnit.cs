using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActionTile;

// make Status base class?
public class ActionUnit : ActionSelectable
{
    public int moveTimeLeft = 0;

    public void Start()
    {
        clearQueueBanner();
    }

    public override void Action(GameManager gm)
    {
        // move right if no unit there.
    }

    public override void Tick()
    {
        moveTimeLeft -= 1;
    }
}
