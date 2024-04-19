using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// in this household we love god classes
public class GameManager : MonoBehaviour
{
    public float timeForNextTick;
    public const float tickStepTime = 1f;

    // all the actions
    public ActionQueue actionQueue;
    // all the tiles, container.
    public TileGenerator tileGenerator;
    // all the units
    public Dictionary<Vector2Int,ActionUnit> units;
    // player cursor
    public PlayerCursor playerCursor;

    public TMPro.TMP_Text queueSizeText;

    private void Start()
    {
        units = new Dictionary<Vector2Int, ActionUnit>();
    }
    // Update is called once per frame
    void Update()
    {
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

            if (actionQueue.queue.Count > 0)
            {
                var action = actionQueue.queue.Dequeue();

                // what kind of action was this?
                if (action.unit is ActionTile)
                {
                    tileGenerator.tiles[action.tile].Action(this);
                }

                if (action.unit is ActionUnit)
                {
                    units[action.tile].Action(this);
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
}
