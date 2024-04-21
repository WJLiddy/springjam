using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodWall : MonoBehaviour
{
    public bool alive = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Trigger()
    {
        alive = false;
        GetComponentInChildren<Animator>().SetTrigger("Attack");
        Invoke("clear", 0.7f);
    }

    public void clear()
    {
        this.gameObject.SetActive(false);
    }
}