using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public string poolName; // Only for visual clarity in inspector

    [Space]
    [SerializeField] GameObject objectToPool;
    [SerializeField] int poolSize;

    private readonly List<GameObject> objectPool = new List<GameObject>();

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
}
