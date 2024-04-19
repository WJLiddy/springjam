using UnityEngine;

public class ActionTile : ActionSelectable
{ 
    public GameObject redPlant;
    public GameObject bluePlant;
    public GameObject greenPlant;

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

    public State state = State.NONE;
    // Start is called before the first frame update

    public void Start()
    {
        clearInfoBanner();
        clearQueueBanner();
    }
    
    public override void Action(GameManager gm)
    {
        switch(state)
        {
            // player wants to plant
            case State.NONE:
                state = State.GROWING;
                growingPlant = gm.playerCursor.selectedPlant;
                growTimeLeft = 9;
                setInfoBanner(growTimeLeft);
                break;

            // player wants to harvest
            case State.GROWING:
                if (growTimeLeft == 0)
                {
                    state = State.NONE;
                    GameObject v = null;
                    switch(growingPlant)
                    {
                        case PlantType.RED:
                            v = Instantiate(redPlant);
                            break;
                        case PlantType.GREEN:
                            v  = Instantiate(greenPlant);
                            break;
                        case PlantType.BLUE:
                            v = Instantiate(bluePlant);
                            break;
                    }
                    v.transform.position = this.transform.position;
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
