using Unity.VisualScripting;
using UnityEngine;

public class ActionTile : ActionSelectable
{ 
    public GameObject redPlant;
    public GameObject bluePlant;
    public GameObject greenPlant;

    public GameObject growingPlantObj;

    public TMPro.TextMeshPro infoBannerText;

    public enum State
    {
        NONE,
        GROWING
    }

    public enum PlantType
    {
        RED,
        BLUE,
        GREEN
    }

    public PlantType growingPlant;
    public int growTimeLeft = 0;
    public const int GROW_TIME = 9;

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
                growTimeLeft = GROW_TIME;
                growingPlantObj = Instantiate(redPlant);
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
                growingPlantObj.transform.localPosition = Vector3.up * Mathf.Lerp(-4.75f, -6.25f, (growTimeLeft / (float)GROW_TIME));
                if (growTimeLeft > 0)
                {
                    growTimeLeft -= 1;
                }
                setInfoBanner(growTimeLeft);
                break;
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
