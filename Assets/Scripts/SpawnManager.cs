using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    private readonly List<GameObject> enemyPool = new List<GameObject>();

    [Space]
    [SerializeField] float preSpawnDelay = 5.0f;
    [SerializeField] float spawnDelayStart = 3.0f;
    [SerializeField] float spawnDelayReduce = 0.5f;
    [SerializeField] float spawnDelayMin = 0.5f;

    [Space]
    [SerializeField] int stageLength = 20;

    [Space]
    [SerializeField] float spawnDistance = 25f;

    private GameManager gm;

    void Start()
    {
        // Get reference to singleton
        gm = GameManager.Instance;

        // Set enemy pool size as more than max enemies as a safety buffer
        int poolSize = gm.maxEnemies + 5;

        // Create an enemy pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, gm.enemyGroup);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }

        // Start coroutine to spawn enemies
        StartCoroutine(EnemySpawner());
    }

    // Coroutine to spawn enemies with increasing difficulty
    IEnumerator EnemySpawner()
    {
        float spawnDelay = spawnDelayStart;
        int currentStage = 0;

        // Wait a few seconds at the start (for tutorial to end)
        yield return new WaitForSeconds(preSpawnDelay);

        // Loop to spawn single enemies continuously with a delay in between
        while (gm.isGameActive)
        {
            // Do not spawn more than max number of enemies
            while (gm.enemiesOnScreen >= gm.maxEnemies)
            {
                yield return null;
            }

            // Spawn a single enemy
            SpawnEnemy();

            // Get the current difficulty stage
            int newStage = (int)((gm.timeAlive - preSpawnDelay) / stageLength);

            // If stage advances, set new stage and reduce delay between enemy spawns
            if (currentStage < newStage)
            {
                currentStage = newStage;
                spawnDelay -= spawnDelayReduce;
                if (spawnDelay < spawnDelayMin)
                    spawnDelay = spawnDelayMin;
            }

            // Wait out the delay before re-running spawn loop
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Method to spawn enemy
    void SpawnEnemy()
    {
        // Get player position via singleton
        Vector3 playerPosition = PlayerControl.Instance.transform.position;

        // Get random direction from player and set vector magnitude to 1
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDirection.Normalize();

        // Set spawn position at exactly a certain distance from player
        Vector3 spawnPosition = playerPosition + (randomDirection * spawnDistance);

        // Get new enemy from the pool, set its position and increment enemy count
        GameObject newEnemy = GetEnemyFromPool();
        if (newEnemy != null)
        {
            newEnemy.transform.position = spawnPosition;
            gm.enemiesOnScreen++;
        }
    }

    // Method to get an enemy from the pool
    private GameObject GetEnemyFromPool()
    {
        for (int i = 0; i < enemyPool.Count; i++)
        {
            if (!enemyPool[i].activeSelf)
            {
                enemyPool[i].SetActive(true);
                return enemyPool[i];
            }
        }
        return null;
    }
}
