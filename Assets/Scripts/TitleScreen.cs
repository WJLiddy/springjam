using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cyan;
    bool trigCyan = false;

    public void ToGame()
    {
        // change to farm scene
        Invoke("ToGameb", 1f);
        trigCyan = true;

    }

    public void ToGameb()
    {
        SceneManager.LoadScene("Farm");
    }

    public void Update()
    {
        transform.Find("Text").transform.position = new Vector3(450, 450 + 20 * Mathf.Sin(2*Time.time), 0);


        if (trigCyan && cyan.transform.position.x < 450)
        {
            cyan.transform.position += ((new Vector3(1, -1, 0)) * 1500 * Time.deltaTime);
        }
    }
}
