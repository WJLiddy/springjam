using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public ActionQueue actionQueue;
    public GameManager gameManager;
    public ActionTile.PlantType selectedPlant;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            var gridSpot = new Vector2Int((int)objectHit.position.x, (int)objectHit.position.z);

            // check if we hit something
            if (objectHit)
            {
                // apply if the tile isn't queued already
                if (Input.GetMouseButton(0) && actionQueue.queue.ToList().Find(r => r.tile == gridSpot) == null)
                {
                    // priority always given to units
                    if (gameManager.units.ContainsKey(gridSpot))
                    {
                        actionQueue.queue.Add(new ActionQueue.Action(gridSpot, gameManager.units[gridSpot]));
                        gameManager.updateActionQueueUI();
                        return;
                    }
                    // else it's a tile action
                    actionQueue.queue.Add(new ActionQueue.Action(gridSpot, gameManager.tileGenerator.tiles[gridSpot]));
                    gameManager.updateActionQueueUI();
                }

                // erase
                if (Input.GetMouseButton(2) && actionQueue.queue.ToList().Find(r => r.tile == gridSpot) != null)
                {
                    var v = actionQueue.queue.Find(r => r.tile == gridSpot);
                    actionQueue.queue.Remove(v);
                }
            }
        }
    }
}
