using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActionQueue;

public abstract class ActionSelectable : MonoBehaviour
{
    // all tiles have a queue banner showing when the action will trigger.
    public TMPro.TextMeshPro queueText;

    internal void clearQueueBanner()
    {
        queueText.text = "";
    }

    internal void setQueueBanner(int nextAction)
    {
        queueText.text = nextAction.ToString();
    }

    // all tiles must respond to a tick and and an action
    public abstract void Tick();
    public abstract void Action(GameManager gm, Vector2Int position, Action a);
}
