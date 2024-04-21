using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeartContainer : MonoBehaviour
{
    public List<GameObject> hearts;
    // Start is called before the first frame update
    public void Start()
    {
        foreach(var v in hearts)
        {
            v.SetActive(false);
        }
    }

    public void setHealth(int hp)
    {
        int ctr = 0;
        foreach (GameObject heart in hearts)
        {
            ctr++;
            heart.SetActive(hp <= ctr);
        }
    }
}
