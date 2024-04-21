using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ActionQueue;
using static UnityEngine.GraphicsBuffer;

// in this household we love god classes
public class GameManager : MonoBehaviour
{
    public double timeForNextTick;
    public const double tickStepTime = 1f;
    private float loseTime = 0f;
    private double timeForNextEnemyTick = (tickStepTime / 2);
    public GameObject breachingEnemy;
    private int combo = 0;
    public TMPro.TMP_Text comboCounter;

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

    public PlayerInventory playerInventory;



    private void Start()
    {
        units = new Dictionary<Vector2Int, ActionUnit>();
    }

    private void comboEnd()
    {
        // play a sound
        // display fancy if combo lost
        if (combo > 0)
        {
            transform.Find("Break").GetComponent<AudioSource>().Play();
            // at least 90, up to 120.
            comboCounter.fontSize = Mathf.Max(90, Mathf.Min(120, combo * 5));
        }
        combo = 0;
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
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation,rotation,Time.deltaTime);
            loseTime -= Time.deltaTime;
            if(loseTime <= 0)
            {
                SceneManager.LoadScene("Title");
            }
        }

        if(Time.timeSinceLevelLoadAsDouble >= timeForNextEnemyTick)
        {
            timeForNextEnemyTick += tickStepTime;
            // all enemies tick on offbeat?
            // convert to list so that underlying datastruct can change
            foreach (var enemy in enemySpawner.enemies.ToList())
            {
                // holy fuck!
                enemy.Value.Tick(this, enemy.Key);
            }
            enemySpawner.Tick();
            enemySpawner.waveSteps -= 1;
            enemySpawner.waveTimeLeft.value = (enemySpawner.waveSteps / (float)EnemySpawner.WAVE_DURATION);


            // if enemy destroyed something update the queue
            validateActions();
        }


        if (Time.timeSinceLevelLoadAsDouble >= timeForNextTick)
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

            if (actionQueue.queue.Count > 0)
            {
                combo += 1;
                giveComboRewards();
                var action = actionQueue.queue[0];
                actionQueue.queue.RemoveAt(0);
                actionQueue.TriggerAnimateCard(action, findPosition(action));

                // what kind of action was this?
                if (action.unit is ActionTile)
                {
                    if(!tileGenerator.tiles[findPosition(action)].Action(this, findPosition(action), action))
                    {
                        comboEnd();
                    }
                }

                if (action.unit is ActionUnit)
                {
                    if(!units[findPosition(action)].Action(this, findPosition(action), action))
                    {
                        comboEnd();
                    }
                }
                transform.Find("Hit").GetComponent<AudioSource>().Play();
            } else
            {
                comboEnd();
            }



            updateActionQueueUI();
        }

        // update combo widget
        comboCounter.text = "Combo X " + combo;
        comboCounter.fontSize = Mathf.Lerp(comboCounter.fontSize, 45, Time.deltaTime * 2f);
        comboCounter.color = Color.Lerp(comboCounter.color, Color.black, Time.deltaTime * 2f);
    }

    public void giveComboRewards()
    {
        if((combo % 10) == 0)
        {
            playerInventory.strawberrySeeds += 1;
            playerInventory.carrotSeeds += (Random.Range(10,50) > combo) ? 0 : 1;
            playerInventory.eggplantSeeds += (Random.Range(20, 100) > combo) ? 0 : 1;
            comboCounter.color = Color.white;
            comboCounter.fontSize = 120;
        }
    }

    // called after enemy move: make sure all actions are still valid..
    public void validateActions()
    {
        // remove all actions where unit no longer exists on map
        actionQueue.queue.RemoveAll(a => findPosition(a).x == -99);

        // remove all harvests where there's no plant anymore
        actionQueue.queue.RemoveAll(a => (!a.isPlantCommand && a.unit is ActionTile && tileGenerator.tiles[findPosition(a)].state == ActionTile.State.NONE));
        updateActionQueueUI();
    }

    public void updateActionQueueUI()
    {
        HashSet<ActionUnit> seenUnits = new HashSet<ActionUnit>();

        int nextAction = 1;
        foreach (var action in actionQueue.queue)
        {
            if (action.unit is ActionTile)
            {
                tileGenerator.tiles[findPosition(action)].setQueueBanner(nextAction);
            }

            if (action.unit is ActionUnit)
            {
                // queued commands - do "..."
                if (!seenUnits.Contains(action.unit))
                {
                    units[findPosition(action)].setQueueBanner(nextAction);
                }
                else
                {
                    int idx = actionQueue.queue.FindIndex(a => a.unit == action.unit);
                    units[findPosition(action)].setQueueBannerMore(idx+1);
                }
                seenUnits.Add(action.unit as ActionUnit);
            }
            nextAction += 1;
        }
        queueSizeText.text = (1 + actionQueue.queue.Count).ToString();
    }

    // slow method, but who cares?
    public Vector2Int findPosition(Action a)
    {
        foreach(var v in tileGenerator.tiles)
        {
            if(v.Value == a.unit)
            {
                return v.Key;
            }
        }
        foreach (var v in units)
        {
            if (v.Value == a.unit)
            {
                return v.Key;
            }
        }
        // fuck you magic numbers
        return new Vector2Int(-99, -99);
    }

    public void Lose(GameObject breachingEnemy)
    {
        if (this.breachingEnemy == null)
        {
            loseTime = 2f;
            this.breachingEnemy = breachingEnemy;
        }
    }
}
