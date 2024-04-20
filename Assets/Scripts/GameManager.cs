using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// in this household we love god classes
public class GameManager : MonoBehaviour
{
    public float timeForNextTick;
    public const float tickStepTime = 1f;
    public float loseTime = 0f;
    public GameObject breachingEnemy;

    // all the actions
    public ActionQueue actionQueue;
    // all the tiles, container.
    public TileGenerator tileGenerator;
    // all the units
    public Dictionary<Vector2Int,ActionUnit> units;
    // enemy spawner, container
    public EnemySpawner enemySpawner;
    // player cursor
    public PlayerCursor playerCursor;

    public TMPro.TMP_Text queueSizeText;

    public List<WoodWall> backWalls;

    private void Start()
    {
        units = new Dictionary<Vector2Int, ActionUnit>();
    }
    // Update is called once per frame
    void Update()
    {
        if(breachingEnemy != null)
        {
            //Camera.main.transform.LookAt(breachingEnemy.transform.position);
            Vector3 relativePos = breachingEnemy.transform.position - Camera.main.transform.position;
            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            Camera.main.transform.rotation = rotation;
        }


        if(Time.time >= timeForNextTick)
        {
            timeForNextTick += tickStepTime;

            // all tiles tick
            foreach (var tile in tileGenerator.tiles.Values)
            {
                tile.Tick();
                // will be reset in UpdateActionQueue
                tile.clearQueueBanner();
            }

            // all units tick
            foreach (var unit in units.Values)
            {
                unit.Tick();
                // will be reset in UpdateActionQueue
                unit.clearQueueBanner();
            }

            // all enemies tick (maybe on offbeat??)
            // convert to list so that underlying datastruct can change
            foreach(var enemy in enemySpawner.enemies.ToList())
            {
                // holy fuck!
                enemy.Value.Tick(this, enemy.Key);
            }

            if (actionQueue.queue.Count > 0)
            {
                var action = actionQueue.queue[0];
                actionQueue.queue.RemoveAt(0);
                actionQueue.TriggerAnimateCard(action);

                // what kind of action was this?
                if (action.unit is ActionTile)
                {
                    tileGenerator.tiles[action.tile].Action(this, action.tile);
                }

                if (action.unit is ActionUnit)
                {
                    units[action.tile].Action(this, action.tile);
                }
                GetComponent<AudioSource>().Play();
            }

            updateActionQueueUI();
        }
    }

    public void updateActionQueueUI()
    {
        int nextAction = 1;
        foreach (var action in actionQueue.queue.ToArray())
        {
            if (action.unit is ActionTile)
            {
                tileGenerator.tiles[action.tile].setQueueBanner(nextAction);
            }

            if (action.unit is ActionUnit)
            {
                units[action.tile].setQueueBanner(nextAction);
            }
            nextAction += 1;
        }
        queueSizeText.text = (1 + actionQueue.queue.Count).ToString();
    }

    public void Lose(GameObject breachingEnemy)
    {
        loseTime = 2f;
        this.breachingEnemy = breachingEnemy;
    }
}
