using System.Collections;
using UnityEngine;
using static ActionQueue;

public class ActionUnit : ActionSelectable
{
    public int moveTimeLeft = 0;
    public int hp = 3;
    public Vector2Int targ;
    private bool targSet = false;
    public Animator animator;
    public ActionTile.PlantType plantType;
    public HeartContainer heartContainer;
    public GameObject textPopup;

    public void Start()
    {
        clearQueueBanner();
    }

    public override bool Action(GameManager gm, Vector2Int p, Action a)
    {
        animator.SetBool("move", false);

        // attack if someone's there.
        if (gm.enemySpawner.enemies.ContainsKey(p + new Vector2Int(1, 0)))
        {
            animator.SetTrigger("attack");
            gm.enemySpawner.enemies[p + new Vector2Int(1, 0)].Hurt(gm, p + new Vector2Int(1, 0));
            return true;
        }

        // move right if no unit there, or nothing growing
        if ((p.x < 9) && !gm.units.ContainsKey(p + new Vector2Int(1,0)) && gm.tileGenerator.tiles[p+new Vector2Int(1,0)].state != ActionTile.State.GROWING)
        {
            gm.units.Remove(p);
            gm.units[p + new Vector2Int(1,0)] = this;
            targ = p + new Vector2Int(1, 0);
            targSet = true;
            animator.SetBool("move", true);
            return true;
        }

        // blocked!
        var v = Instantiate(textPopup);
        v.transform.position = this.transform.position;
        v.GetComponent<TMPro.TextMeshPro>().text = "BLOCKED!";
        v.transform.position += Vector3.up;
        Destroy(v, 1f);
        return false;
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

        var corot = HurtFinal(new object[] { gm, p });
        // induce a small delay to help with heart animation
        StartCoroutine(corot);

    }

    public IEnumerator HurtFinal(object[] param)
    {
        GameManager gm = (GameManager)param[0];
        Vector2Int pos = (Vector2Int)param[1];
        // won't work at higher tempos!!! at 0.5 though this will probably run before the next update.
        yield return new WaitForSeconds(0.5f);
        heartContainer.setHealth(hp);
        if (hp <= 0)
        {
            Destroy(gameObject);
            gm.units.Remove(pos);
        }
        yield return null;
    }

}
