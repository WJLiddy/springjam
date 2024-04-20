using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public ActionQueue actionQueue;
    public GameManager gameManager;
    public PlayerInventory playerInventory;
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

                    var targetTile = gameManager.tileGenerator.tiles[gridSpot];
                    if (targetTile.state == ActionTile.State.NONE)
                    {
                        var legal = checkIfPlantLegal();
                        if (legal)
                        {
                            actionQueue.queue.Add(new ActionQueue.Action(gridSpot, targetTile));
                            gameManager.updateActionQueueUI();
                        }
                    }
                    else
                    {
                        // else it's a harvest, always legal
                        actionQueue.queue.Add(new ActionQueue.Action(gridSpot, targetTile));
                        gameManager.updateActionQueueUI();
                    }
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

    public bool checkIfPlantLegal()
    {
        // check if plant is even legal
        if (selectedPlant == ActionTile.PlantType.STRAWBERRY)
        {
            if (playerInventory.strawberrySeeds >= 0)
            {
                playerInventory.strawberrySeeds -= 1;
                return true;
            }
        }

        if (selectedPlant == ActionTile.PlantType.CARROT)
        {
            if (playerInventory.carrotSeeds >= 0)
            {
                playerInventory.carrotSeeds -= 1;
                return true;
            }
        }

        if (selectedPlant == ActionTile.PlantType.EGGPLANT)
        {
            if (playerInventory.eggplantSeeds >= 0)
            {
                playerInventory.eggplantSeeds -= 1;
                return true;
            }
        }
        return false;
    }

    public void setPlantType(int i)
    {
        switch(i)
        {
            case 0:
                selectedPlant = ActionTile.PlantType.STRAWBERRY;
                break;
            case 1:
                selectedPlant = ActionTile.PlantType.CARROT;
                break;
            case 2:
                selectedPlant = ActionTile.PlantType.EGGPLANT;
                break;
        }
    }
}
