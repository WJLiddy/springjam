using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject bug;
    public Dictionary<Vector2Int,Enemy> enemies = new Dictionary<Vector2Int, Enemy>();

    // Start is called before the first frame update
    void Start()
    {
        Spawn();

    }

    // Update is called once per frame
    void Update()
    {
    }

    void Spawn()
    {
        var v = Instantiate(bug);
        var pos = new Vector2Int(10, Random.Range(0, 3));
        v.transform.position = new Vector3(pos.x, 0, pos.y);
        enemies[pos] = v.GetComponent<Enemy>();
    }
}
