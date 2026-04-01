using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] ObjectPooler enemyPool;
    [SerializeField] ObjectPooler enemyProjectilePool;
    [SerializeField] Player playerRef;
    [SerializeField] SessionStats sessionStats;

    [Space]
    [SerializeField] float preSpawnDelay = 5.0f;
    [SerializeField] float spawnDelayStart = 3.0f;
    [SerializeField] float spawnDelayReduce = 0.5f;
    [SerializeField] float spawnDelayMin = 0.5f;

    [Space]
    [SerializeField] int stageLength = 20;
    [SerializeField] int maxEnemies = 20;

    [Space]
    [SerializeField] float spawnDistance = 25f;

    private int enemiesOnScreen = 0;

    void OnEnable()
    {
        GameEvents.OnEnemyDeath += EnemyDeath;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyDeath -= EnemyDeath;
    }

    void Start()
    {
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
        GameEvents.RaiseOnGameStart();

        // Loop to spawn single enemies continuously with a delay in between
        while (GameManager.Instance.gameState != GameState.GameOver)
        {
            // Do not spawn more than max number of enemies
            while (enemiesOnScreen >= maxEnemies)
            {
                yield return null;
            }

            // Spawn a single enemy
            SpawnEnemy();

            // Get the current difficulty stage
            int newStage = (int)((sessionStats.timeAlive - preSpawnDelay) / stageLength);

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
        Vector3 playerPosition = playerRef.transform.position;

        // Get random direction from player and set vector magnitude to 1
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDirection.Normalize();

        // Set spawn position at exactly a certain distance from player
        Vector3 spawnPosition = playerPosition + (randomDirection * spawnDistance);

        // Get new enemy from the pool, set its position and increment enemy count
        GameObject newEnemy = enemyPool.GetFromPool();
        if (newEnemy != null)
        {
            newEnemy.transform.position = spawnPosition;
            newEnemy.GetComponent<Enemy>().Initialize(playerRef, enemyProjectilePool);
            enemiesOnScreen++;
        }
    }

    void EnemyDeath(int _)
    { enemiesOnScreen--; }
}
