using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Input.mousePosition.x * Time.deltaTime * 0.1f, 0, Input.mousePosition.y * Time.deltaTime * 0.1f) ;
    }
}
