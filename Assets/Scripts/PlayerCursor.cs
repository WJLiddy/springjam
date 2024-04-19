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

            // check if we hit something and the tile isn't queued already
            if (objectHit && Input.GetMouseButton(0) && actionQueue.queue.ToList().Find(r => r.tile == gridSpot) == null)
            {
                // priority always given to units
                if(gameManager.units.ContainsKey(gridSpot))
                {
                    actionQueue.queue.Enqueue(new ActionQueue.Action(gridSpot, gameManager.units[gridSpot]));
                    gameManager.updateActionQueueUI();
                    return;
                }
                // else it's a tile action
                actionQueue.queue.Enqueue(new ActionQueue.Action(gridSpot, gameManager.tileGenerator.tiles[gridSpot]));
                gameManager.updateActionQueueUI();
            }
        }
    }
}
