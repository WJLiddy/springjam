using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    // enemy prefabs
    public GameObject spider;
    public GameObject stomper;
    public GameObject wasp;

    public Slider waveTimeLeft;
    // current live enemies
    public Dictionary<Vector2Int,Enemy> enemies = new Dictionary<Vector2Int, Enemy>();


    public const int WAVE_DURATION = 100;
    // ticks left in wave
    public int waveSteps = WAVE_DURATION;
    // how many subwaves
    int subwaves;
    // enemies in subwaves.
    List<List<GameObject>> spawnList;

    // Start is called before the first frame update
    void Start()
    {
        makeWavePlan(30);
    }

    // Update is called once per frame
    public void Tick()
    {
        if(waveSteps == (spawnList.Count * ((double)WAVE_DURATION / (double)subwaves)))
        {
            // pop off the spawnlist
            int xptr = 0;
            int yptr = 0;
            foreach(var v in spawnList[0])
            {
                yptr++;
                if(yptr == 5)
                {
                    yptr = 0;
                    xptr++;
                }
                SpawnEnemy(v, Random.Range(xptr, 10+yptr));
            }
            spawnList.RemoveAt(0);
        }
    }

    void makeWavePlan(int difficulty)
    {
        int difficultyLeft = difficulty;
        // choose 4 - 6 discrete steps
        subwaves = Random.Range(4, 8);
        spawnList = new List<List<GameObject>>();
        for(int currentStep = subwaves - 1; currentStep >= 0; currentStep--)
        {
            List<GameObject> entry = new List<GameObject>();
            // while more difficulty than steps
            while((double)difficultyLeft > difficulty * ((float)currentStep / (float)subwaves))
            {
                entry.Add(spider);
                difficultyLeft -= 1;
            }
            spawnList.Add(entry);
        }
    }

    void SpawnEnemy(GameObject g, int y)
    {
        var v = Instantiate(g);
        var pos = new Vector2Int(10, y);
        v.transform.position = new Vector3(pos.x, 0, pos.y);
        enemies[pos] = v.GetComponent<Enemy>();
    }
}
