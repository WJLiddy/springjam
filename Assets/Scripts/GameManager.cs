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
    public float timeForNextTick;
    public const float tickStepTime = 1f;
    public float loseTime = 0f;
    public float timeForNextEnemyTick = (tickStepTime / 2);
    public GameObject breachingEnemy;
    public int combo = 0;
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

        if(Time.time >= timeForNextEnemyTick)
        {
            timeForNextEnemyTick += tickStepTime;
            enemySpawner.waveSteps -= 1;
            enemySpawner.waveTimeLeft.value = 1 - (timeForNextEnemyTick / enemySpawner.waveSteps);
            // all enemies tick on offbeat?
            // convert to list so that underlying datastruct can change
            foreach (var enemy in enemySpawner.enemies.ToList())
            {
                // holy fuck!
                enemy.Value.Tick(this, enemy.Key);
            }
            enemySpawner.Tick();
        }


        if (Time.time >= timeForNextTick)
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
                    tileGenerator.tiles[findPosition(action)].Action(this, findPosition(action));
                }

                if (action.unit is ActionUnit)
                {
                    units[findPosition(action)].Action(this, findPosition(action));
                }
                GetComponent<AudioSource>().Play();
            } else
            {
                combo = 0;
            }
            comboCounter.text = "Combo X " + combo;

            updateActionQueueUI();
        }
    }

    public void giveComboRewards()
    {
        if((combo % 10) == 0)
        {
            playerInventory.strawberrySeeds += 1;
        }
    }

    public void updateActionQueueUI()
    {
        int nextAction = 1;
        foreach (var action in actionQueue.queue.ToArray())
        {
            if (action.unit is ActionTile)
            {
                tileGenerator.tiles[findPosition(action)].setQueueBanner(nextAction);
            }

            if (action.unit is ActionUnit)
            {
                units[findPosition(action)].setQueueBanner(nextAction);
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
        Debug.LogError("No action for" + a.unit);
        return new Vector2Int(0, 0);
    }

    public void Lose(GameObject breachingEnemy)
    {
        loseTime = 2f;
        this.breachingEnemy = breachingEnemy;
    }
}
