using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int strawberrySeeds = 0;
    public int CarrotSeeds = 0;
    public int eggplantSeeds = 0;

    public TMPro.TMP_Text textStraw;
    public TMPro.TMP_Text textCarrot;
    public TMPro.TMP_Text textEggPlant;

    public void Update()
    {
        textStraw.text = strawberrySeeds.ToString();
        textCarrot.text = CarrotSeeds.ToString();
        textEggPlant.text = eggplantSeeds.ToString();
    }

}
