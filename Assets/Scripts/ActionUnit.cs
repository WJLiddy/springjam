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

    public override void Action(GameManager gm, Vector2Int p)
    {
        // move right if no unit there. (later, move animation.)
        if(!gm.units.ContainsKey(p + new Vector2Int(1,0)))
        {
            gm.units.Remove(p);
            gm.units[p + new Vector2Int(1,0)] = this;
            this.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
        }
    }

    public override void Tick()
    {
        moveTimeLeft -= 1;
    }
}
