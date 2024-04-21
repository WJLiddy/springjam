using UnityEngine;
using static ActionQueue;
using static UnityEditor.PlayerSettings;

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
        if(p.x == 9)
        {
            var t = Instantiate(textPopup);
            t.transform.position = this.transform.position;
            t.GetComponent<TMPro.TextMeshPro>().text = "BLOCKED!";
            t.transform.position += Vector3.up;
            Destroy(t, 1f);
            return false;
        }

        // attack if someone's there.
        if (gm.enemySpawner.enemies.ContainsKey(p + new Vector2Int(1, 0)))
        {
            animator.SetTrigger("attack");
            gm.enemySpawner.enemies[p + new Vector2Int(1, 0)].Hurt(gm, p + new Vector2Int(1, 0));
            return true;
        }

        // move right if no unit there, or nothing growing
        if (!gm.units.ContainsKey(p + new Vector2Int(1,0)) && gm.tileGenerator.tiles[p+new Vector2Int(1,0)].state != ActionTile.State.GROWING)
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
        heartContainer.setHealth(hp);
        if (hp <= 0)
        {
            Destroy(gameObject);
            gm.units.Remove(p);
        }
    }
}
