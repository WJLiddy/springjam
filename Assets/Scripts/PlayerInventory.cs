using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int strawberrySeeds = 0;
    public int carrotSeeds = 0;
    public int eggplantSeeds = 0;

    public TMPro.TMP_Text textStraw;
    public TMPro.TMP_Text textCarrot;
    public TMPro.TMP_Text textEggPlant;

    public GameObject cyan;
    public bool cynFadeOut = true;

    public void Update()
    {
        if(textStraw.text != strawberrySeeds.ToString())
        {
            textStraw.fontSize = 72;
        }
        if(textCarrot.text != carrotSeeds.ToString())
        {
            textCarrot.fontSize = 72;
        }
        if(textEggPlant.text != eggplantSeeds.ToString())
        {
            textEggPlant.fontSize = 72;
        }

        textStraw.text = strawberrySeeds.ToString();
        textCarrot.text = carrotSeeds.ToString();
        textEggPlant.text = eggplantSeeds.ToString();

        textEggPlant.fontSize = Mathf.Lerp(textEggPlant.fontSize, 36, 2*Time.deltaTime);
        textStraw.fontSize = Mathf.Lerp(textStraw.fontSize, 36 , 2*Time.deltaTime);
        textCarrot.fontSize = Mathf.Lerp(textCarrot.fontSize, 36 ,2 * Time.deltaTime);

        // 450 to -1000
        if (cynFadeOut && cyan.transform.position.x > -1000)
        {
            cyan.transform.position += ((new Vector3(-1, 1, 0)) * 1500 * Time.deltaTime);
        }

        if (!cynFadeOut && cyan.transform.position.x < 450)
        {
            cyan.transform.position += ((new Vector3(1, -1, 0)) * 1500 * Time.deltaTime);
        }
    }

}
