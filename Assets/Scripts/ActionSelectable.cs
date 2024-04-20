using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionSelectable : MonoBehaviour
{
    // all tiles have a queue banner showing when the action will trigger.
    public TMPro.TextMeshPro queueBannerText;

    internal void clearQueueBanner()
    {
        queueBannerText.transform.parent.gameObject.SetActive(false);
    }

    internal void setQueueBanner(int nextAction)
    {
        queueBannerText.transform.parent.gameObject.SetActive(true);
        queueBannerText.text = nextAction.ToString();
    }

    // all tiles must respond to a tick and and an action
    public abstract void Tick();
    public abstract void Action(GameManager gm, Vector2Int position);
}
