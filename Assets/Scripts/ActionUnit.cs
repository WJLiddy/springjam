using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActionTile;

public class ActionUnit : ActionSelectable
{
    public int moveTimeLeft = 0;
    public int hp = 3;


    public void Start()
    {
        clearQueueBanner();
    }

    public override void Action(GameManager gm, Vector2Int p)
    {
        // attack if someone's there.
        if (gm.enemySpawner.enemies.ContainsKey(p + new Vector2Int(1, 0)))
        {
            gm.enemySpawner.enemies[p + new Vector2Int(1, 0)].Hurt(gm, p);
            return;
        }

        // move right if no unit there. (later, move animation.)
        if (!gm.units.ContainsKey(p + new Vector2Int(1,0)))
        {
            gm.units.Remove(p);
            gm.units[p + new Vector2Int(1,0)] = this;
            this.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
            return;
        }
    }

    public override void Tick()
    {
        moveTimeLeft -= 1;
    }

    public void Hurt(GameManager gm, Vector2Int p)
    {
        hp -= 1;
        if (hp <= 0)
        {
            Destroy(gameObject);
            gm.units.Remove(p);
        }
    }
}
