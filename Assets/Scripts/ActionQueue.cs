using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class ActionQueue : MonoBehaviour
{
    // where was this placed, and who was the target?
    public class Action
    {
        private static int idctr = 0;
        // we def want to stack attacks/moves so only associate with the unit
        // need to also pass a plant/grow command
        public Action(ActionSelectable unit, bool isPlantCommand, ActionTile.PlantType plantType)
        {
            this.unit = unit;
            this.ID = ++idctr;
            this.isPlantCommand = isPlantCommand;
            this.plantType = plantType;
        }
        public bool isPlantCommand;
        public ActionTile.PlantType plantType;
        public ActionSelectable unit;
        public int ID; // if actions are lost/discarded, now the UI will delete the associated game obj.
    }

    // queue of all actions 
    public List<Action> queue;

    // action currently animating
    private Tuple<GameObject, Vector2Int> actionCurrentlyAnimating;

    // prefabs for the cards
    public GameObject cardHarvest;
    public GameObject cardPlant;
    public GameObject cardMove;

    // Gamemanger knows how long to next
    public GameManager gameManager;

    // all current cards
    public Dictionary<int,GameObject> cards;


    // Start is called before the first frame update
    void Start()
    {
        queue = new List<Action>();
        cards = new Dictionary<int, GameObject>();
    }

    public void TriggerAnimateCard(Action a, Vector2Int position)
    {
        // destroy old card if set
        if(actionCurrentlyAnimating != null)
        {
            Destroy(actionCurrentlyAnimating.Item1);
        }

        // send the animated card to the position
        actionCurrentlyAnimating = new Tuple<GameObject, Vector2Int>(createCardForAction(a), position);
        // must be direct child of cavas
        actionCurrentlyAnimating.Item1.transform.SetParent(this.transform.parent);
    }

    public void setAllColors(GameObject o, Color cPrimary, Color cSecondary)
    {
        o.GetComponent<UnityEngine.UI.Image>().color = cPrimary;
        o.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().color = cSecondary;
        o.GetComponentInChildren<TMPro.TMP_Text>().color = cSecondary;
    }

    public GameObject createCardForAction(Action v)
    {
        ActionTile.PlantType p = ActionTile.PlantType.STRAWBERRY;
        // create a new card
        GameObject card = null;
        switch (v.unit)
        {
            case ActionUnit a:
                card = Instantiate(cardMove, this.transform);
                p = a.plantType;

                break;
            case ActionTile a:
                card = Instantiate(v.isPlantCommand ? cardPlant : cardHarvest, this.transform);
                if (v.isPlantCommand)
                {
                    p = v.plantType;
                }
                else
                {
                    p = a.growingPlant;
                }
                // use growingplant
                break;
        }

        switch (p)
        {
            case ActionTile.PlantType.STRAWBERRY:
                setAllColors(card, new Color(1f,0.8f,0.8f),Color.red);
                break;
            case ActionTile.PlantType.CARROT:
                setAllColors(card, new Color(1f, 0.9f, 0.8f), new Color(1f, 0.5f, 0f));
                break;
            case ActionTile.PlantType.EGGPLANT:
                setAllColors(card, new Color(1f, 0.8f, 1f), new Color(0.5f,0f,0.5f));
                break;
        }
        return card;
    }
    // Update is called once per frame
    void Update()
    {
        int queuePos = 0;
        float queueDispPtr = 0;
        // Will be a float between 0 and 1, so pow it
        float remainderTime = MathF.Pow(((gameManager.timeForNextTick - Time.time) / GameManager.tickStepTime),4);
        foreach(var v in queue)
        {
            // lookup if there's an item for this card. if not, create it.
            if (!cards.ContainsKey(v.ID))
            {
                cards[v.ID] = createCardForAction(v);
            }

            // animation for cards.
            float size = (float)Math.Pow(0.978, 5.0*((double)(queuePos) + remainderTime));
            queueDispPtr += (100*size);
            cards[v.ID].transform.localPosition = new Vector3(queueDispPtr + -100 + (100*remainderTime), 0, 0);
            cards[v.ID].transform.localScale = new Vector3(size,size,size);
            queuePos++;
        }

        // any cards to remove? also n^2 LOL
        foreach(var v in cards)
        {
            if (queue.ToList().Find(x => x.ID == v.Key) == null)
            {
                Destroy(v.Value);
                cards.Remove(v.Key);
                break;
            }
        }

        // animate the current action
        if (actionCurrentlyAnimating != null)
        {
            // aim down a little bit
            var tilePos = new Vector3(actionCurrentlyAnimating.Item2.x, 0, actionCurrentlyAnimating.Item2.y - 0.5f);

            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(tilePos);  //convert game object position to VievportPoint

            // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
            // the fuck i need to add 0.4 for..?
            actionCurrentlyAnimating.Item1.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(actionCurrentlyAnimating.Item1.GetComponent<RectTransform>().anchorMin,viewportPoint + (0.4f * Vector2.one),Time.deltaTime * 5);
            actionCurrentlyAnimating.Item1.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(actionCurrentlyAnimating.Item1.GetComponent<RectTransform>().anchorMin, viewportPoint + (0.4f * Vector2.one), Time.deltaTime * 5);
            actionCurrentlyAnimating.Item1.GetComponent<RectTransform>().localScale = Vector3.Lerp(actionCurrentlyAnimating.Item1.GetComponent<RectTransform>().localScale,  Vector3.zero, Time.deltaTime * 5);
            actionCurrentlyAnimating.Item1.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(actionCurrentlyAnimating.Item1.GetComponent<UnityEngine.UI.Image>().color, new Color(1,1,1,0), Time.deltaTime * 10);

        }
    }



}
