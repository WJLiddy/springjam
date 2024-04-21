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

    private int currentDifficulty = 10;
    public const int WAVE_DURATION = 70;
    private int currentWave = 1;
    // ticks left in wave
    public int waveSteps;
    // how many subwaves
    int subwaves;
    // enemies in subwaves.
    List<List<GameObject>> spawnList;
    public TMPro.TMP_Text waveText;

    // Start is called before the first frame update
    void Start()
    {
        waveSteps = WAVE_DURATION;
        makeWavePlan(currentDifficulty);

    }

    // Update is called once per frame
    public void Tick()
    {
        Debug.Log(waveSteps + "/" + (int)(spawnList.Count * ((double)WAVE_DURATION / (double)subwaves)));

        if(waveSteps > 0 && (waveSteps == (int)(spawnList.Count * ((double)WAVE_DURATION / (double)subwaves))))
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
                SpawnEnemy(v, xptr, yptr);
            }
            spawnList.RemoveAt(0);
        }

        if(waveSteps == 0)
        {
            // will get -'d later
            waveSteps = WAVE_DURATION + 1;
            currentDifficulty += 5;
            makeWavePlan(currentDifficulty);
            currentWave += 1;
            waveText.text = "Wave " + currentWave.ToString();
        }
    }

    void makeWavePlan(int difficulty)
    {
        int difficultyLeft = difficulty;
        Debug.Log(difficulty);
        // choose 4 - 8 discrete steps
        subwaves = Random.Range(3, 5);
        spawnList = new List<List<GameObject>>();
        for(int currentStep = subwaves - 1; currentStep >= 0; currentStep--)
        {
            List<GameObject> entry = new List<GameObject>();
            // while more difficulty than steps
            while((double)difficultyLeft > difficulty * ((float)currentStep / (float)subwaves))
            {
                var cnt = Random.Range(0, 3);
                switch(cnt)
                {
                    case 0:
                        entry.Add(wasp);
                        difficultyLeft -= 1;
                        break;
                    case 1:
                        if (difficulty > 15)
                        {
                            entry.Add(spider);
                            difficultyLeft -= 3;
                        }
                        break;
                    case 2:
                        if (difficulty > 25)
                        {
                            entry.Add(stomper);
                            difficultyLeft -= 5;
                        }
                        break;
                }
            }
            Debug.Log("Adding Entry of" + entry.Count + " enemies.");
            spawnList.Add(entry);
        }
    }

    void SpawnEnemy(GameObject g, int x, int y)
    {
        var v = Instantiate(g);
        var pos = new Vector2Int(11 + x, y);
        v.transform.position = new Vector3(pos.x, 0, pos.y);
        enemies[pos] = v.GetComponent<Enemy>();
    }
}
