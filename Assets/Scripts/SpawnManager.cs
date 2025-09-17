using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [Space]
    [SerializeField] float preSpawnDelay = 5.0f;
    [SerializeField] float spawnDelayStart = 3.0f;
    [SerializeField] float spawnDelayReduce = 0.5f;
    [SerializeField] float spawnDelayMin = 0.5f;

    [Space]
    [SerializeField] int stageLength = 20;

    [Space]
    [SerializeField] float spawnDistance = 25f;

    void Start()
    {
        // Start coroutine to spawn enemies
        StartCoroutine(SpawnEnemyWave());
    }

    // Coroutine to spawn enemies with increasing difficulty
    IEnumerator SpawnEnemyWave()
    {
        float spawnDelay = spawnDelayStart;
        int currentStage = 0;

        // Wait a few seconds at the start (for tutorial to end)
        yield return new WaitForSeconds(preSpawnDelay);

        // Loop to spawn single enemies continuously with a delay in between
        while (GameManager.Instance.isGameActive)
        {
            // Do not spawn more than max number of enemies
            while (GameManager.Instance.enemiesOnScreen >= GameManager.Instance.maxEnemies)
            {
                yield return null;
            }

            // Spawn a single enemy and increase the enemy count
            SpawnEnemy();
            GameManager.Instance.enemiesOnScreen++;

            // Get the current difficulty stage
            int newStage = (int)((GameManager.Instance.timeAlive - preSpawnDelay) / stageLength);

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

        // Set spawn rotation to prefab default and parent spawned enemy to enemy group object
        Quaternion spawnRotation = enemyPrefab.transform.rotation;
        Transform spawnParent = GameManager.Instance.enemyGroupObject;

        // Spawn the enemy
        Instantiate(enemyPrefab, spawnPosition, spawnRotation, spawnParent);
    }
}
