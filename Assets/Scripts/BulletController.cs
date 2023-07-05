using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 1;
    public float speed = 10f;
    private Vector3 direction;
    private Vector3 startPosition;

    // On Bullet Hit Event
    public delegate void BulletHitHandler(float timeTaken);
    public static event BulletHitHandler OnBulletHit;

    // On Bullet Hit Event
    public delegate void BulletMissHandler();
    public static event BulletMissHandler OnBulletMiss;

    public delegate void OnDestroy(GameObject gameObject);
    private OnDestroy onDestroy;

    public float range = 10f; // Maximum distance the skillshot can travel

    public void Initialize(Vector3 direction, OnDestroy onDestroy)
    {
        this.direction = direction;
        this.onDestroy = onDestroy;

        startPosition = transform.position;
    }

    void Update()
    {
        // Moves the skillshot in the direction at the specified speed
        transform.position += speed * Time.deltaTime * direction;

        if (Vector3.Distance(startPosition, transform.localPosition) > range)
        {
            OnBulletMiss();
            onDestroy?.Invoke(gameObject);

            // Destroy the bullet when it has traveled its maximum range
            //Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("On trigger enter");

        if (collider.gameObject.TryGetComponent<TargetController>(out var targetController))
        {
            float timeTaken = Time.time - targetController.createdAt;

            targetController.TakeDamage(damage);

            OnBulletHit(timeTaken);
            onDestroy?.Invoke(gameObject);
        }
        //if (collider.gameObject.layer == 9) // Target Spawn Area
        //{
        //    Debug.Log("Floor hit");
        //    OnBulletMiss?.Invoke(true);
        //    Destroy(gameObject);
        //}
    }
}
