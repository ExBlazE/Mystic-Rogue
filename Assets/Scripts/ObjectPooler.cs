using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates an object pool of set size for use by reference.
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    public string poolName; // Only for visual clarity in inspector

    [Space]
    [SerializeField] GameObject objectToPool;
    [SerializeField] int poolSize;

    private readonly List<GameObject> objectPool = new();

    void Awake()
    {
        if (objectToPool != null && poolSize > 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(objectToPool, this.transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }
        }
        else
            Debug.Log("Object pool is not set");
    }

    /// <summary>
    /// Returns the first inactive object from the pool and sets it active.
    /// </summary>
    public GameObject GetFromPool()
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the first inactive object from the pool, sets its position and rotation and sets it active.
    /// </summary>
    public GameObject GetFromPool(Vector3 spawnPos, Quaternion spawnRot)
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeSelf)
            {
                obj.transform.position = spawnPos;
                obj.transform.rotation = spawnRot;
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }
}
