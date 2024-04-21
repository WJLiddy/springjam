using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static int progression;
    public GameObject[] progressionBoxes;
    // Start is called before the first frame update
    void Start()
    {
        Progress(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Progress(int step)
    {
        progression = Mathf.Max(progression, step);
        for(int i = 0; i != progressionBoxes.Length; i++)
        {
            progressionBoxes[i].SetActive(i == progression);
        }

    }
}
