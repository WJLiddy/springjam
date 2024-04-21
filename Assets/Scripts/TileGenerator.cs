using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    private int WIDTH = 10;
    private int HEIGHT = 4;
    public GameObject tileBase;
    public Dictionary<Vector2Int, ActionTile> tiles;


    // Start is called before the first frame update
    void Start()
    {
        tiles = new Dictionary<Vector2Int, ActionTile>();
        for(int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                var t = Instantiate(tileBase);
                t.transform.position = new Vector3(x, 0, y);
                tiles[new Vector2Int(x, y)] = t.GetComponent<ActionTile>();
            }
        }
    }

}
