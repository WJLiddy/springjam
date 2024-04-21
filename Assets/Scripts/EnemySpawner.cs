using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject spider;
    public GameObject stomper;
    public GameObject wasp;
    public Dictionary<Vector2Int,Enemy> enemies = new Dictionary<Vector2Int, Enemy>();
    public const int waveLength = 100;
    public int waveSteps = waveLength;
    public Slider waveTimeLeft;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy(spider, 0);
        SpawnEnemy(wasp, 1);
        SpawnEnemy(stomper, 2);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SpawnEnemy(GameObject g, int y)
    {
        var v = Instantiate(g);
        var pos = new Vector2Int(10, y);
        v.transform.position = new Vector3(pos.x, 0, pos.y);
        enemies[pos] = v.GetComponent<Enemy>();
    }
}
