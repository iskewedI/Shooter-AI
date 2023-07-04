using UnityEngine;

public class SpawnAreaController : MonoBehaviour
{
    // Define a delegate type for your event.
    public delegate void TargetDespawnedHandler();
    // Define the event using the delegate type.
    public static event TargetDespawnedHandler OnTargetDespawned;

    private ObjectPooler pooler;
    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private Vector3 size;
    [SerializeField] private Color gizmoColor = Color.red;

    [Range(1, 4)]
    public float minLifeTime;
    [Range(1, 4)]
    public float maxLifeTime;

    // In this moment the collider can be set, and this is called BEFORE the gizmos are drawn.
    // We need this to get the box collider size to be sync with the spawn size changes.
    void OnPostRender()
    {
        boxCollider = GetComponent<BoxCollider>();

    }

    private void Start()
    {
        pooler = GetComponent<ObjectPooler>();

        StartTargetSpawn();
    }

    void StartTargetSpawn()
    {
        float y_distance = -(size.y / 2);
        float x = size.x / 2;

        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-x, x), Random.Range(y_distance, size.y + y_distance), 0);

        SpawnTarget(Random.Range(minLifeTime, maxLifeTime), randomPosition, Quaternion.identity);
    }

    private void SpawnTarget(float lifeTime, Vector3 position, Quaternion rotation)
    {
        GameObject target = pooler.SpawnFromPool(position, rotation, transform);

        TargetController targetController = target.GetComponent<TargetController>();

        // Infinite target
        targetController.Initialize(-1, HandleTargetDestroy);
    }

    private void HandleTargetDestroy(GameObject gameObject)
    {
        //OnTargetDespawned();

        pooler.ReturnToPool(gameObject);

        StartTargetSpawn();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, size);

        boxCollider.size = size;    
    }
}
