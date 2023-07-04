using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private float initialPoolSize = 15;
    private readonly Queue<GameObject> objectPool = new Queue<GameObject>();

    private void Awake()
    {
        // Populate the pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject newObject = Instantiate(prefab, transform.parent);
            newObject.SetActive(false);
            objectPool.Enqueue(newObject);
        }
    }

    public GameObject SpawnFromPool(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (objectPool.Count == 0)
        {
            // If the pool is empty, instantiate a new object
            GameObject newObject = Instantiate(prefab, parent);
            newObject.SetActive(false);
            objectPool.Enqueue(newObject);
        }

        // Get an object from the pool
        GameObject objectToSpawn = objectPool.Dequeue();

        // Set the object's position and rotation
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Activate the object
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        objectPool.Enqueue(objectToReturn);
    }
}
