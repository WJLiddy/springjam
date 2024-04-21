using Unity.VisualScripting;
using UnityEngine;

public class ActionTile : ActionSelectable
{ 
    public GameObject strawberryPlant;
    public GameObject carrotPlant;
    public GameObject eggPlantPlant;

    public GameObject growingPlantObj;

    public TMPro.TextMeshPro infoBannerText;

    public enum State
    {
        NONE,
        GROWING
    }

    public enum PlantType
    {
        STRAWBERRY,
        CARROT,
        EGGPLANT
    }

    public PlantType growingPlant;
    public int growTimeLeft = 0;
    public int growTimeMax = 0;
    public const int GROW_TIME_STRAW = 3;
    public const int GROW_TIME_CARROT = 6;
    public const int GROW_TIME_EGGPLANT = 12;

    public State state = State.NONE;
    // Start is called before the first frame update

    public void Start()
    {
        clearInfoBanner();
        clearQueueBanner();
    }
    
    public override void Action(GameManager gm, Vector2Int position)
    {
        switch(state)
        {
            // player wants to plant
            case State.NONE:
                state = State.GROWING;
                growingPlant = gm.playerCursor.selectedPlant;
                switch(gm.playerCursor.selectedPlant)
                {
                    case PlantType.STRAWBERRY:
                    growingPlantObj = Instantiate(strawberryPlant);
                    growTimeLeft = GROW_TIME_STRAW;
                    break;
                    case PlantType.CARROT:
                    growingPlantObj = Instantiate(carrotPlant);
                    growTimeLeft = GROW_TIME_CARROT;
                    break;
                    case PlantType.EGGPLANT:
                    growingPlantObj = Instantiate(eggPlantPlant);
                    growTimeLeft = GROW_TIME_EGGPLANT;
                    break;
                }
                growTimeMax = growTimeLeft;
                growingPlantObj.transform.SetParent(this.transform);
                growingPlantObj.transform.localPosition = -6.25f * Vector3.up;
                setInfoBanner(growTimeLeft);
                break;

            // player wants to harvest
            case State.GROWING:
                if (growTimeLeft == 0)
                {
                    state = State.NONE;
                    // transfer the growing unit to the list of all
                    clearInfoBanner();
                    gm.units[position] = growingPlantObj.GetComponent<ActionUnit>();
                    gm.units[position].transform.position = new Vector3(position.x, 0, position.y);
                    growingPlantObj = null;
                }
                break;
        }
    }

    public override void Tick()
    {
        switch(state)
        {
            case State.NONE:
                infoBannerText.transform.parent.gameObject.SetActive(false);
                break;

            case State.GROWING:
                growingPlantObj.transform.localPosition = Vector3.up * Mathf.Lerp(-4.75f, -6.25f, (growTimeLeft / growTimeMax));
                if (growTimeLeft > 0)
                {
                    growTimeLeft -= 1;
                }
                setInfoBanner(growTimeLeft);
                break;
        }
    }


    public void Hurt()
    {
        if(state == State.GROWING)
        {
            state = State.NONE;
            Destroy(growingPlantObj);
            clearInfoBanner();
        }
    }

    private void clearInfoBanner()
    {
        infoBannerText.transform.parent.gameObject.SetActive(false);
    }

    private void setInfoBanner(int info)
    {
        infoBannerText.transform.parent.gameObject.SetActive(true);
        infoBannerText.text = info.ToString();
    }
}
