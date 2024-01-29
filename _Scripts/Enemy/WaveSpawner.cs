using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    public List<Enemies> enemies = new List<Enemies>();
    public int currentWave;
    private int waveValue;

    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    [SerializeField] List<GameObject> spawnPortals = new List<GameObject>();

    public Transform spawnlocation;
    [HideInInspector] public Vector2 origin = Vector2.zero;
    [HideInInspector] public float radius = 0.5f;

    //determines overall lenght of the waves
    public int waveDuration;
    private float waveTimer;
    //for controlling when enemies spawn during the wave
    private float spawnInterval;
    private float spawnTimer;

    //Variables for resolving the end of the level
    public int nrOfBosses;
    private int bossCount;
    public GameObject bossPrefab;
    public GameObject chestPrefab;

    [Header("Portal for continuing")]
    public GameObject portalPrefab;
    private bool isCreated, chestCreated;
    private float timer = 5f;
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        isCreated = false;
        chestCreated = false;
        bossCount = 0;

        GenerateWave();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //spawnPortals = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
    }


    private void FixedUpdate()
    {
        int randomPortal = Random.Range(0, spawnPortals.Count);
        Transform randomPos = spawnPortals[randomPortal].transform;
        //spawn enemy if its less than 0
        if (spawnTimer <= 0)
        {
            if (enemiesToSpawn.Count > 0)
            {
                //instantiate the first enemy in the list
                Instantiate(enemiesToSpawn[0], randomPos);
                //after being instantiated remove from the list
                enemiesToSpawn.RemoveAt(0);
                spawnTimer = spawnInterval;
            }
            //end the wave
            else
            {
                waveTimer = 0;
            }
        }
        //reduce the spawn timer and wave timer
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        /*if (GameObject.FindGameObjectWithTag("Enemy") != null && !isCreated)
        {
            Instantiate(portalPrefab, randomPos, Quaternion.identity);
            isCreated = true;
        }*/

        if(enemiesToSpawn.Count <= 0)
        {
            timer -= Time.deltaTime;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0 && !isCreated && timer <= 0)
            {
                GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
                
                if (bossCount < nrOfBosses)
                {
                    Instantiate(bossPrefab, randomPos);
                    bossCount++;
                    Debug.Log(bossCount + "/" + nrOfBosses);
                    timer = 5f;
                }
                else if(bossCount >= nrOfBosses && !chestCreated && bosses.Length == 0)
                {
                    
                    for(int i = spawnPortals.Count - 1; i >= 0; i--)
                    {
                        spawnPortals[i].SetActive(false);
                        Debug.Log(i);
                    }
                    Debug.Log(true);
                    playerTransform.GetChild(5).gameObject.SetActive(true);
                    chestCreated = true;
                }
                else if (!GameObject.FindGameObjectWithTag("Chest") && chestCreated)
                {
                    Vector3 newPortalTransform = playerTransform.position + new Vector3(2.5f,2.5f,0);

                    if(portalPrefab.transform.position.x - playerTransform.position.x < 1 && portalPrefab.transform.position.x - playerTransform.position.x > -1)
                    {
                        Instantiate(portalPrefab, newPortalTransform, Quaternion.identity);
                    }
                    else if(portalPrefab.transform.position.y - playerTransform.position.y < 1 && portalPrefab.transform.position.y - playerTransform.position.y > -1)
                    {
                        Instantiate(portalPrefab, newPortalTransform, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(portalPrefab);
                    }
                    isCreated = true;
                }
            }
            else if(enemies.Length > 0) 
            {
                timer = 2f;
            }

        }
    }

    public void GenerateWave()
    {
        waveValue = currentWave * 10;
        GenerateEnemies();

        //this gives a fixed amount of time between each enemy
        spawnInterval = waveDuration / enemiesToSpawn.Count;

        //wave duration is read only
        waveTimer = waveDuration;
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0)
        {
            int randomEnemyID = Random.Range(0, enemies.Count);
            int randomEnemyCost = enemies[randomEnemyID].cost;

            if (waveValue - randomEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randomEnemyID].enemyPrefab);
                waveValue -= randomEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }

        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }
}

[System.Serializable]
public class Enemies
{
    public GameObject enemyPrefab;
    public int cost;
}

