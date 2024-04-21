using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum State
    {
        IDLE,
        MOVE,
        ATTACK
    }

    State state;
    public Animator animator;
    public Vector2Int targ;
    public HeartContainer hc;

    int hp = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Tick(GameManager gm, Vector2Int pos)
    { 
        // attack if allied in front.
        var nextTile = pos + new Vector2Int(-1, 0);
        if (gm.units.ContainsKey(nextTile))
        {
            targ = nextTile;
            state = State.ATTACK; animator.SetBool("move", false); animator.SetTrigger("attack");
            gm.units[nextTile].Hurt(gm,nextTile);
 
        }

        // attack if crop in front.
        else if (gm.tileGenerator.tiles.ContainsKey(nextTile) && gm.tileGenerator.tiles[nextTile].state == ActionTile.State.GROWING)
        {
            targ = nextTile;
            state = State.ATTACK; animator.SetBool("move", false); animator.SetTrigger("attack");
            gm.tileGenerator.tiles[nextTile].Hurt();
        }

        // if we reached rank 0, attack the back fences
        else if (pos.x == 0)
        {
            if (gm.backWalls[pos.y].hp >= 0)
            {
                state = State.ATTACK; animator.SetBool("move", false); animator.SetTrigger("attack");
                gm.backWalls[pos.y].Hurt();
            }
            else
            {
                state = State.MOVE;
                // game lost
                gm.Lose(this.gameObject);
                // move enemy past line
                targ = nextTile + new Vector2Int(-1, 0);
            }


        }

        // either idle or move.
        else
        {
            var s = Random.Range(0, 3);
            switch (s)
            {
                case 0: state = State.MOVE; animator.SetBool("move", true);
                    targ = nextTile;
                    gm.enemySpawner.enemies.Remove(pos);
                    gm.enemySpawner.enemies[nextTile] = this;
                    break;
                case 1: case 2: state = State.IDLE; animator.SetBool("move", false); break;
            }
        }
    }

    public void Update()
    {
        switch (state)
        {
            case State.MOVE:
                this.transform.position = Vector3.MoveTowards(this.transform.position,new Vector3(targ.x,0,targ.y),Time.deltaTime);
                break;
        }
    }

    public void Hurt(GameManager gm, Vector2Int pos)
    {
        hp -= 1;
        hc.setHealth(hp);
        if(hp <= 0)
        {
            gm.enemySpawner.enemies.Remove(pos);
        }
        Destroy(this.gameObject);
    }
}
