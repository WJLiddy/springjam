using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodWall : MonoBehaviour
{
    public int hp = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt()
    {
        hp -= 1;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
