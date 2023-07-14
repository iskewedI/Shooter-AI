using System.Collections;
using UnityEngine;

public class SpawnAreaController : MonoBehaviour
{
    // Define a delegate type for your event.
    public delegate void TargetDespawnedHandler();
    // Define the event using the delegate type.
    public static event TargetDespawnedHandler OnTargetDespawned;

    [Range(1, 5)]
    public float minLifeTime;
    [Range(1, 5)]
    public float maxLifeTime;

    [SerializeField] private float maxWaitTime;

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private Vector3 size;
    [SerializeField] private Color gizmoColor = Color.red;

    private ObjectPooler pooler;
    private Coroutine runningCoroutine;

    // In this moment the collider can be set, and this is called BEFORE the gizmos are drawn.
    // We need this to get the box collider size to be sync with the spawn size changes.
    void OnPostRender()
    {
        boxCollider = GetComponent<BoxCollider>();

    }

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        pooler = GetComponent<ObjectPooler>();

        StartTargetSpawn();
    }

    void StartTargetSpawn()
    {
        float y_distance = -(size.y / 2);
        float x = size.x / 2;

        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-x, x), Random.Range(y_distance, size.y + y_distance), 0);

        ClearCoroutine();
        runningCoroutine = StartCoroutine(SpawnTarget(Random.Range(minLifeTime, maxLifeTime), randomPosition, Quaternion.identity));
    }

    private IEnumerator SpawnTarget(float lifeTime, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(Random.Range(0, maxWaitTime));

        GameObject target = pooler.SpawnFromPool(position, rotation, transform);

        TargetController targetController = target.GetComponent<TargetController>();

        targetController.Initialize(-1, HandleTargetDespawn, HandleTargetDestroy);
        //targetController.MoveBetween(-size.x, size.x, 2f);
    }

    private void ClearCoroutine()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private void HandleTargetDespawn(GameObject obj)
    {
        OnTargetDespawned();
        HandleTargetDestroy(obj);

        ClearCoroutine();
    }

    private void HandleTargetDestroy(GameObject obj)
    {
        Debug.Log("Handle target destroy");
        pooler.ReturnToPool(obj);

        ClearCoroutine();
        StartTargetSpawn();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, size);

        boxCollider.size = size;    
    }
}
