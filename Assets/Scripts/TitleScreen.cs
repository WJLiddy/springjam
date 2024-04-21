using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public void ToGame()
    {
        // change to farm scene
        SceneManager.LoadScene("Farm");

    }

    public void Update()
    {
        transform.Find("Text").transform.position = new Vector3(450, 300 + 50 * Mathf.Sin(2*Time.time), 0);
    }
}
