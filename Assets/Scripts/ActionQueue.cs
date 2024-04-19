using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionQueue : MonoBehaviour
{
    // where was this placed, and who was the target?
    public class Action
    {
        private static int idctr = 0;
        public Action(Vector2Int tile, ActionSelectable unit)
        {
            this.tile = tile;
            this.unit = unit;
            this.ID = ++idctr;
        }
        public Vector2Int tile;
        public ActionSelectable unit;
        public int ID; // if actions are lost/discarded, now the UI will delete the associated game obj.
    }

    // queue of all actions 
    public Queue<Action> queue;

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
        queue = new Queue<Action>();
        cards = new Dictionary<int, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        int queuePos = 0;
        float queueDispPtr = 0;
        float remainderTime = ((gameManager.timeForNextTick - Time.time) / GameManager.tickStepTime);
        foreach(var v in queue)
        {
            // lookup if there's an item for this card. if not, create it.
            if (!cards.ContainsKey(v.ID))
            {
                // create a new card
                GameObject card = null;
                switch(v.unit)
                {
                    case ActionUnit a:
                        card = Instantiate(cardMove,this.transform);
                        break;
                    case ActionTile a:
                        card = Instantiate(a.state == ActionTile.State.NONE ? cardPlant : cardHarvest,this.transform);
                        break;
                }
                cards[v.ID] = card;
            }

            // animation for cards.
            float size = (float)Math.Pow(0.99, 5.0*((double)(queuePos) + remainderTime));
            queueDispPtr += (100*size);
            cards[v.ID].transform.localPosition = new Vector3(queueDispPtr + (100*remainderTime), 0, 0);
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
    }
}
