using UnityEngine;

public class ActionUnit : ActionSelectable
{
    public int moveTimeLeft = 0;
    public int hp = 3;
    public Vector2Int targ;
    private bool targSet = false;
    public Animator animator;


    public void Start()
    {
        clearQueueBanner();
    }

    public override void Action(GameManager gm, Vector2Int p)
    {
        animator.SetBool("move", false);
        // attack if someone's there.
        if (gm.enemySpawner.enemies.ContainsKey(p + new Vector2Int(1, 0)))
        {
            animator.SetTrigger("attack");
            gm.enemySpawner.enemies[p + new Vector2Int(1, 0)].Hurt(gm, p);
            return;
        }

        // move right if no unit there. (later, move animation.)
        if (!gm.units.ContainsKey(p + new Vector2Int(1,0)))
        {
            gm.units.Remove(p);
            gm.units[p + new Vector2Int(1,0)] = this;
            targ = p + new Vector2Int(1, 0);
            targSet = true;
            animator.SetBool("move", true);
            return;
        }
    }
    public void Update()
    {
        if (targSet)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(targ.x, 0, targ.y), Time.deltaTime);
        }
    }

    public override void Tick()
    {
        animator.SetBool("move", false);
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
