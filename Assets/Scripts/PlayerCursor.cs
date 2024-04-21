using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    public ActionQueue actionQueue;
    public GameManager gameManager;
    public PlayerInventory playerInventory;
    public ActionTile.PlantType selectedPlant;

    // move cursor rig
    public Canvas canvas;
    public GameObject cursorParent;
    public Image cursorImage;

    public Sprite plantSprite;
    public Sprite harvestSprite;
    public Sprite moveSprite;

    public HashSet<ActionSelectable> actionUnitsSeenThisClick = new HashSet<ActionSelectable>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setIconForCursor()
    {
        cursorImage.color = Color.white;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            var gridSpot = new Vector2Int((int)objectHit.position.x, (int)objectHit.position.z);

            // check if we hit something
            if (objectHit)
            {
                // check if there's a unit here
                if (gameManager.units.ContainsKey(gridSpot))
                {
                    // set Move image.
                    cursorImage.sprite = moveSprite;
                    return;
                }

                // check for a plant/harvest
                var targetTile = gameManager.tileGenerator.tiles[gridSpot];
                if (targetTile.state == ActionTile.State.NONE)
                {
                    var legal = checkIfPlantLegal(false);
                    // set plant
                    if (legal)
                    {
                        cursorImage.sprite = plantSprite;
                        switch(selectedPlant)
                        {
                            case ActionTile.PlantType.STRAWBERRY: cursorImage.color = Color.red; break;
                            case ActionTile.PlantType.CARROT: cursorImage.color = new Color(1f,0.5f,0f); break;
                            case ActionTile.PlantType.EGGPLANT: cursorImage.color = new Color(0.5f,0,0,5f); break;

                        }
                        return;
                    }
                }
                else
                {
                    // set harvest
                    cursorImage.sprite = harvestSprite;
                    return;
                }
            }
        }
        // set nothing
        cursorImage.sprite = null;
        cursorImage.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        setIconForCursor();
        moveCursor();
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            var gridSpot = new Vector2Int((int)objectHit.position.x, (int)objectHit.position.z);

            // check if we hit something
            if (objectHit)
            {
                if (Input.GetMouseButton(0))
                {
                    // check if there's a unit here
                    if (gameManager.units.ContainsKey(gridSpot) && !actionUnitsSeenThisClick.Contains(gameManager.units[gridSpot]))
                    {
                        actionUnitsSeenThisClick.Add(gameManager.units[gridSpot]);
                        // if there is, feel free to assign another move.
                        actionQueue.queue.Add(new ActionQueue.Action(gameManager.units[gridSpot], false));
                        gameManager.updateActionQueueUI();
                        return;
                    }

                    // check for a plant/harvest
                    var targetTile = gameManager.tileGenerator.tiles[gridSpot];

                    if (targetTile != null && !actionUnitsSeenThisClick.Contains(targetTile))
                    {
                        // don't retrigger on same click
                        actionUnitsSeenThisClick.Add(targetTile);
                        // it's never permitted to queue harvest or plant commands.
                        if (actionQueue.queue.Find(f => f.unit == targetTile) == null)
                        {
                            if (targetTile.state == ActionTile.State.NONE)
                            {
                                var legal = checkIfPlantLegal(true);
                                if (legal)
                                {
                                    actionQueue.queue.Add(new ActionQueue.Action(targetTile, false));
                                    gameManager.updateActionQueueUI();
                                }
                            }
                            else
                            {
                                // else it's a harvest, always legal
                                actionQueue.queue.Add(new ActionQueue.Action(targetTile, true));
                                gameManager.updateActionQueueUI();
                            }
                        }
                    }
                }
                else
                {
                    actionUnitsSeenThisClick.Clear();
                }

                // erase (TODO)
                //if (Input.GetMouseButton(2) && actionQueue.queue.ToList().Find(r => r.tile == gridSpot) != null)
                //{
                //    var v = actionQueue.queue.Find(r => r.tile == gridSpot);
                //    actionQueue.queue.Remove(v);
                //}
            }
        }
    }

    public bool checkIfPlantLegal(bool takeAction)
    {
        // check if plant is even legal
        if (selectedPlant == ActionTile.PlantType.STRAWBERRY)
        {
            if (playerInventory.strawberrySeeds > 0)
            {
                playerInventory.strawberrySeeds -= takeAction ? 1 : 0;
                return true;
            }
        }

        if (selectedPlant == ActionTile.PlantType.CARROT)
        {
            if (playerInventory.carrotSeeds > 0)
            {
                playerInventory.carrotSeeds -= takeAction ? 1 : 0; 
                return true;
            }
        }

        if (selectedPlant == ActionTile.PlantType.EGGPLANT)
        {
            if (playerInventory.eggplantSeeds > 0)
            {
                playerInventory.eggplantSeeds -= takeAction ? 1 : 0; 
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


    public void moveCursor()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
        cursorParent.transform.position = canvas.transform.TransformPoint(pos);
    }
}


