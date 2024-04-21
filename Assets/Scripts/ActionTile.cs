using Unity.VisualScripting;
using UnityEngine;
using static ActionQueue;

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

    public const float STRAW_PLANT_POS = -3.4f;
    public const float CARROT_PLANT_POS = -5.5f;
    public const float EGGPLANT_PLANT_POS = -7.4f;

    public State state = State.NONE;
    // Start is called before the first frame update

    public GameObject textPopup;

    public void Start()
    {
        clearInfoBanner();
        clearQueueBanner();
    }
    
    public override bool Action(GameManager gm, Vector2Int position, Action a)
    {
        switch(state)
        {
            // player wants to plant
            case State.NONE:
                state = State.GROWING;
                growingPlant = a.plantType;
                switch (a.plantType)
                {
                    case PlantType.STRAWBERRY:
                    growingPlantObj = Instantiate(strawberryPlant);
                    growTimeLeft = GROW_TIME_STRAW;
                        growingPlantObj.transform.SetParent(this.transform);
                        growingPlantObj.transform.localPosition = STRAW_PLANT_POS * Vector3.up;
                        break;
                    case PlantType.CARROT:
                    growingPlantObj = Instantiate(carrotPlant);
                    growTimeLeft = GROW_TIME_CARROT;
                        growingPlantObj.transform.SetParent(this.transform);
                        growingPlantObj.transform.localPosition = CARROT_PLANT_POS * Vector3.up;
                        break;
                    case PlantType.EGGPLANT:
                    growingPlantObj = Instantiate(eggPlantPlant);
                    growTimeLeft = GROW_TIME_EGGPLANT;
                        growingPlantObj.transform.SetParent(this.transform);
                        growingPlantObj.transform.localPosition = EGGPLANT_PLANT_POS * Vector3.up;
                        break;
                }
                growTimeMax = growTimeLeft;

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
                } else
                {
                    var v = Instantiate(textPopup);
                    v.transform.position = this.transform.position;
                    v.GetComponent<TMPro.TextMeshPro>().text = "EARLY!";
                    v.transform.position += Vector3.up;
                    Destroy(v, 1f);
                    return false;
                }
                break;
        }
        return true;
    }

    public override void Tick()
    {
        switch(state)
        {
            case State.NONE:
                infoBannerText.transform.parent.gameObject.SetActive(false);
                break;

            case State.GROWING:
                //growingPlantObj.transform.localPosition = Vector3.up * Mathf.Lerp(-4.75f, -6.25f, (growTimeLeft / growTimeMax));
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
